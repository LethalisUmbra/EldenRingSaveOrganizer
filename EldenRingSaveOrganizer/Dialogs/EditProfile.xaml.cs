using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EldenRingSaveOrganizer.Dialogs
{
    /// <summary>
    /// Lógica de interacción para EditProfile.xaml
    /// </summary>
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

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Directory.Move(pc.txFilepath.Text + pc.selectedProfile, pc.txFilepath.Text + txProfileName.Text);
            Close();
        }

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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public bool IsAlphanumerical(string text)
        {
            if (text.Length > 0) return new Regex(@"^[a-zA-Z0-9\s]*$").IsMatch(text); else return false;
        }
    }
}
