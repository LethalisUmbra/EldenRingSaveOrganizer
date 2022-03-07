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
    /// Lógica de interacción para CreateSave.xaml
    /// </summary>
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

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            File.Copy(mw.path + mw.mainSaveName, mw.path + mw.selectedProfile + "\\" + txSaveName.Text);
            Close();
        }

        private void txSaveName_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnCreate.IsEnabled = IsAlphanumerical(txSaveName.Text);
        }

        // Al cerrar la ventana se vuelve a hacer visible la ventana de perfiles
        private void Window_Closed(object sender, EventArgs e)
        {
            mw.reloadProfiles();
            mw.Visibility = Visibility.Visible;
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