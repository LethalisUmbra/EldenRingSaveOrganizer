using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace EldenRingSaveOrganizer.Dialogs
{
    public partial class EditProfile : Window
    {
        // Se declara una variable local para manejar la ventana de configuración de perfiles.
        readonly ProfilesConfig pc;
        public EditProfile(ProfilesConfig profilesConfig)
        {
            InitializeComponent();
            // Se asigna la ventana original a la variable local.
            pc = profilesConfig;
            // Se oculta la ventana de configuración.
            pc.Visibility = Visibility.Hidden;
            // El botón de crear Perfil se desactiva por defecto.
            btnEdit.IsEnabled = false;
            // Se le da el nombre inicial del directorio al campo de texto
            txProfileName.Text = pc.selectedProfile;
            // Se le asigna el foco al textbox para agilizar el proceso.
            txProfileName.Focus();
        }

        // Función para renombrar el perfil seleccionado
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Directory.Move(pc.txFilepath.Text + "\\" + pc.selectedProfile, pc.txFilepath.Text + "\\" + txProfileName.Text);
            Close();
        }

        // Al cambiar el texto se verifica que este sea alfanumérico y difiera del nombre original.
        private void txProfileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnEdit.IsEnabled = IsAlphanumerical(txProfileName.Text) && txProfileName.Text != pc.selectedProfile;
        }

        // Al cerrar la ventana se vuelve a hacer visible la ventana de perfiles
        private void Window_Closed(object sender, EventArgs e)
        {
            pc.mw.loadProfiles();
            pc.LoadProfileList();
            pc.Visibility = Visibility.Visible;
        }

        // Al cancelar se cerrará el dialogo.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Función para verificar que un texto sea alfanumérico mediante Regex.
        public bool IsAlphanumerical(string text)
        {
            if (text.Length > 0) return new Regex(@"^[a-zA-Z0-9ñÑ\-_\s]*$").IsMatch(text); else return false;
        }
    }
}
