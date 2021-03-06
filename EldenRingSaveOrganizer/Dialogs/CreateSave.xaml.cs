using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace EldenRingSaveOrganizer.Dialogs
{
    public partial class CreateSave : Window
    {
        // Se declara una variable local para manejar la ventana principal.
        readonly MainWindow mw;
        public CreateSave(MainWindow mainWindow)
        {
            InitializeComponent();
            // Se asigna la ventana original a la variable local.
            mw = mainWindow;
            // Se oculta la ventana de configuración.
            mw.Visibility = Visibility.Hidden;
            // El botón de crear Perfil se desactiva por defecto.
            btnCreate.IsEnabled = false;
            // Se le asigna el foco al textbox para agilizar el proceso.
            txSaveName.Focus();
        }

        // Función para la creación del archivo de guardado.
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            File.Copy(mw.path + "\\" + mw.mainSaveName, mw.path + "\\" + mw.selectedProfile + "\\" + txSaveName.Text);
            Close();
        }

        // Al cambiar el texto del cuadro se verifica si el contenido es alfanumérico o no para así activar o desactivar el botón.
        private void txSaveName_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnCreate.IsEnabled = IsAlphanumerical(txSaveName.Text);
        }

        // Al cerrar la ventana se vuelve a hacer visible la ventana de perfiles.
        private void Window_Closed(object sender, EventArgs e)
        {
            mw.reloadProfiles();
            mw.Visibility = Visibility.Visible;
        }

        // Al cancelar se cierra el dialogo.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Función para verificar que el texto sea alfanumérico y su longitud mayor a 0.
        public bool IsAlphanumerical(string text)
        {
            if (text.Length > 0) return new Regex(@"^[a-zA-Z0-9ñÑ\-_\s]*$").IsMatch(text); else return false;
        }
    }
}