using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace EldenRingSaveOrganizer.Dialogs
{
    public partial class EditSave : Window
    {
        // Se declara una variable local para manejar la ventana principal.
        readonly MainWindow mw;
        public EditSave(MainWindow mainWindow)
        {
            InitializeComponent();
            // Se asigna la ventana original a la variable local.
            mw = mainWindow;
            // Se oculta la ventana de configuración.
            mw.Visibility = Visibility.Hidden;
            // El botón de renombrar Perfil se desactiva por defecto.
            btnRename.IsEnabled = false;
            // Al textbox del nombre se le asigna por defecto el nombre original.
            txSaveName.Text = mw.selectedSave;
            // Se le asigna el foco al textbox para agilizar el proceso.
            txSaveName.Focus();
        }

        // Función para renombrar el Save seleccionado.
        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            File.Move(mw.path + "\\" + mw.selectedProfile + "\\" + mw.selectedSave, mw.path + "\\" + mw.selectedProfile + "\\" + txSaveName.Text);
            Close();
        }

        // Al cambiar el texto, se verifica que el sea alfanumérico y que difiera del nombre original para que se active el botón.
        private void txSaveName_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnRename.IsEnabled = IsAlphanumerical(txSaveName.Text) && txSaveName.Text != mw.selectedSave;
        }

        // Al cerrar la ventana se vuelve a hacer visible la ventana de perfiles
        private void Window_Closed(object sender, EventArgs e)
        {
            mw.reloadProfiles();
            mw.Visibility = Visibility.Visible;
        }

        // Al pulsar cancelar, simplemente se cerrará el Dialog.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Función para comparar un string con una Regular Expression y verificar que sea alfanumérico y que no sea vacío.
        public bool IsAlphanumerical(string text)
        {
            if (text.Length > 0) return new Regex(@"^[a-zA-Z0-9ñÑ\-_\s]*$").IsMatch(text); else return false;
        }
    }
}
