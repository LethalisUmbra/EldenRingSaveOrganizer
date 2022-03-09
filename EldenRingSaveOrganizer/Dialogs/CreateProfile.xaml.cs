using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace EldenRingSaveOrganizer.Dialogs
{
    public partial class CreateProfile : Window
    {
        // Se declara una variable local para manejar la ventana de configuración de perfiles.
        readonly ProfilesConfig pc;
        public CreateProfile(ProfilesConfig profilesConfig)
        {
            InitializeComponent();
            // Se asigna la ventana original a la variable local.
            pc = profilesConfig;
            // Se oculta la ventana de configuración.
            pc.Visibility = Visibility.Hidden;
            // El botón de crear Perfil se desactiva por defecto.
            btnCreate.IsEnabled = false;
            // Se le asigna el foco al textbox para agilizar el proceso.
            txProfileName.Focus();
        }

        // Función para crear el perfil/directorio.
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(pc.txFilepath.Text + "\\" + txProfileName.Text);
            Close();
        }

        // Al cambiar el texto se verifica que sea alfanumérico para activar el botón de crear.
        private void txProfileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnCreate.IsEnabled = IsAlphanumerical(txProfileName.Text);
        }

        // Al cerrar la ventana se vuelve a hacer visible la ventana de perfiles
        private void Window_Closed(object sender, EventArgs e)
        {
            pc.mw.loadProfiles();
            pc.LoadProfileList();
            pc.Visibility = Visibility.Visible;
        }

        // Al cancelar se cerrará la ventana de dialogo.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Función para validar que el texto sea alfanumérico mediante un Regex.
        public bool IsAlphanumerical(string text)
        {
            if (text.Length > 0) return new Regex(@"^[a-zA-Z0-9ñÑ\-_\s]*$").IsMatch(text); else return false;
        }
    }
}
