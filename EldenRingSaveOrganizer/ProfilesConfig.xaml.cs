using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace EldenRingSaveOrganizer
{
    public partial class ProfilesConfig : Window
    {
        // Se define un MainWindow local para ser asignado a la hora de ser llamado.
        public MainWindow mw;
        // Se define una variable para asignarle el perfil seleccionado del listado.
        public string selectedProfile;
        public ProfilesConfig(MainWindow mainWindow)
        {
            InitializeComponent();
            // Se asigna el mainWindow a la variable local
            mw = mainWindow;
            // Se oculta la ventana principal
            mw.Visibility = Visibility.Hidden;
            // Se obtiene el directorio y se imprime en el textblock
            txFilepath.Text = mw.path;
            // Se deshabilita la opción de eliminar y modificar por defecto
            btnDelete.IsEnabled = false;
            btnEdit.IsEnabled = false;

            // Se carga el listado de perfiles
            LoadProfileList();
        }

        public void LoadProfileList()
        {
            // Se limpia el listbox para evitar sobre-escrituras
            profileList.Items.Clear();

            // Se cargan los perfiles encontrados al listado
            foreach (string profile in mw.nonStaticProfiles) profileList.Items.Add(profile);
        }

        // Al cerrar la ventana se vuelve a hacer visible la ventana principal
        private void Window_Closed(object sender, EventArgs e)
        {
            mw.Visibility = Visibility.Visible;
        }

        // Dialogo para escoger el directorio donde se almacenarán los saves.
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                DialogResult result = dialog.ShowDialog();
                // Al seleccionar otra carpeta se cambiará el path de ubicación y se cargarán los perfiles.
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string oldPath = mw.path;
                    try
                    {
                        // Se remueve el nombre del archivo para solamente conservar la ruta de la carpeta contenedora.
                        string filepath = dialog.FileName.Substring(0, dialog.FileName.LastIndexOf('\\'));
                        // Se sobreescribe el archivo 'path.txt' con la nueva ruta definida.
                        using (StreamWriter w = File.CreateText("path.txt"))
                        {
                            w.Write(filepath);
                        }
                        // Se sobreescribe la variable de path en MainWindow con la nueva ruta.
                        mw.path = filepath;
                        // Se cargan los perfiles de la ruta asignada.
                        mw.loadProfiles();
                        LoadProfileList();
                    }
                    catch
                    {
                        // De no poder tener permisos o tener problemas con la ruta, se conservará la ruta anterior.
                        mw.path = oldPath;
                        System.Windows.MessageBox.Show("Por favor seleccione otra carpeta", "Error de directorio");
                    }
                    
                    // Se muestra la ruta en el TextBox.
                    txFilepath.Text = mw.path;
                }
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            new Dialogs.CreateProfile(this).Show();
        }

        private void profileList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (profileList.SelectedIndex >= 0)
            {
                selectedProfile = profileList.SelectedItem.ToString();
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
            else
            {
                selectedProfile = null;
                btnEdit.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Directory.Delete(txFilepath.Text + selectedProfile, true);
                mw.loadProfiles();
                LoadProfileList();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            new Dialogs.EditProfile(this).Show();
        }
    }
}
