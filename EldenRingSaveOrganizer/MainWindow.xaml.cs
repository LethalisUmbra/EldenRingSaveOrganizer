using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace EldenRingSaveOrganizer
{
    public partial class MainWindow : Window
    {
        // Se define la ruta al save original
        public string mainSaveName = "ER0000.sl2";

        public string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

        // Se define la ruta a los archivos del juego
        public string path;

        // Se declara un listado para almacenar los perfiles encontrados dentro de la ruta.
        public static List<string> profiles = new List<string>();

        // Se declara un listado no estatico para compartirlo con la configuración de perfiles.
        public List<string> nonStaticProfiles = new List<string>();

        // Se declara un listado para almacenar los nombres de las partidas guardadas ubicadas dentro de la ruta y sus subdirectorios.
        public static List<Save> saves = new List<Save>();

        // Se declara un string para definir cual es el perfil seleccionado.
        public string selectedProfile;

        // Se declara un string para definir cual es el save seleccionado.
        public string selectedSave;

        // Clase Inicial
        public MainWindow()
        {
            // Se inicializan los componentes y se llama a la carga de listas
            InitializeComponent();

            // Para almacenar la ruta del archivo se crea un .txt con la ruta y se verifica al inicio si existe o no
            if (File.Exists("settings.erso"))
            {
                // De existir se carga la ruta y se le asigna a la variable local
                path = File.ReadLines("settings.erso").Take(1).First();
                
                // Se cargan los perfiles de la ruta por defecto
                loadProfiles();

                selectedProfile = File.ReadLines("settings.erso").Skip(1).Take(1).First();

                profileList.SelectedItem = selectedProfile;

                // Se desactivan por defecto los botones al no tener un perfil seleccionado
                btnLoad.IsEnabled = false;
                btnReplace.IsEnabled = false;
                btnDelete.IsEnabled = false;
                btnRename.IsEnabled = false;
            }
            else
            {
                /* En el caso de no existir el archivo, se crea y se le asigna por defecto la ruta del roaming de EldenRing,
                 * Alertando al usuario para que modifique la ruta a la que corresponde en la sección de 'Editar Perfiles' */
                DirectoryInfo dirinfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EldenRing\\");

                // Se carga la nueva ruta
                foreach (var dir in dirinfo.GetDirectories())
                {
                    path = dir.FullName;
                }

                using (StreamWriter w = File.CreateText("settings.erso"))
                {
                    w.WriteLine(path);
                    w.WriteLine(selectedProfile);
                }

                MessageBox.Show("Check your savepath in 'Edit Profiles' section.", "Alert");

            }

            btnImport.IsEnabled = profileList.SelectedIndex >= 0;
        }

        public void reloadProfiles()
        {
            // Se limpia el combobox y el listbox para evitar sobre-escrituras
            profiles.Clear();
            saves.Clear();

            saveList.Items.Clear();
            profileList.Items.Clear();

            // Se procesan los directorios de la ruta definida
            ProcessDirectory(path);

            // Se copian los perfiles de la variable estática a la variable no estática
            nonStaticProfiles = profiles;

            // Se cargan los perfiles encontrados al listado
            foreach (string profile in profiles) profileList.Items.Add(profile);

            // Se selecciona el perfil activo.
            profileList.SelectedItem = selectedProfile;
        }

        public void loadProfiles()
        {
            // Se limpia el combobox y el listbox para evitar sobre-escrituras
            profiles.Clear();
            saves.Clear();

            saveList.Items.Clear();
            profileList.Items.Clear();

            profileList.SelectedIndex = -1;

            // Se procesan los directorios de la ruta definida
            ProcessDirectory(path);

            // Se copian los perfiles de la variable estática a la variable no estática
            nonStaticProfiles = profiles;

            // Se cargan los perfiles encontrados al listado
            foreach (string profile in profiles) profileList.Items.Add(profile);

            foreach (Save save in saves) if (save.folder == selectedProfile) saveList.Items.Add(save.name);
        }

        // Se define la clase de Save (Guardado) para saber cuales son sus rutas, nombres y carpetas contenedoras.
        public class Save
        {
            public string name { get; set; }
            public string filepath { get; set; }
            public string folder { get; set; }  
        }

        // Se define la acción del botón para editar perfiles
        private void EditProfiles_Click(object sender, RoutedEventArgs e)
        {
            // Abre la ventana de Configuración de perfiles
            new ProfilesConfig(this).Show();
        }

        private static void ProcessDirectory(string targetDirectory)
        {
            // Procesa la lista de archivos encontrados en el directorio
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fullFilePath in fileEntries)
            {
                // Se extrae solamente el nombre del archivo
                string filename = Path.GetFileName(fullFilePath);
                // Se extrae la ruta del archivo sin su nombre
                string filepath = fullFilePath.Substring(0, fullFilePath.LastIndexOf("\\"));
                // Se extrae la carpeta que contiene el archivo
                string folder = filepath.Substring(filepath.LastIndexOf("\\") +1);

                // Se filtran los archivos de respaldo y de la cloud de steam
                if (!filename.Contains(".vdf") && !filename.Contains(".bak"))
                {
                    saves.Add(new Save{ name = filename, filepath = filepath, folder = folder });
                }                
            }

            // Si existen subdirectorios se aplica recursión para así obtener todos los archivos.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                // Se agrega el subdirectorio como perfil
                profiles.Add(subdirectory.Substring(subdirectory.LastIndexOf("\\") + 1));
                // Se recorre el subdirectorio para obtener sus archivos
                ProcessDirectory(subdirectory);
            }
        }

        // Al cambiar de seleccion en la lista se cargan los saves correspondientes
        private void profileList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Se limpia el listado
            saveList.Items.Clear();
            // Se confirma que esté seleccionado un indice
            if (profileList.SelectedIndex > -1)
            {
                // Se almacena el nombre del perfil en la variable
                selectedProfile = profileList.SelectedItem.ToString();
                // Se cargan los saves correspondientes.
                foreach (Save save in saves) if (save.folder == selectedProfile) saveList.Items.Add(save.name);
                // Se activa el botón de importar
                btnImport.IsEnabled = true;
            }
            else
            {
                btnImport.IsEnabled = false;
            }

            settingsChanger(selectedProfile, 2);
        }

        // Al pulsar import abrirá un dialogo donde se solicita un nombre para el archivo
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            new Dialogs.CreateSave(this).Show();
        }

        // Al cambiar la selección de save se activarán los botones correspondientes y se almacena el nombre del save en una variable
        private void saveList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (saveList.SelectedIndex > -1)
            {
                btnLoad.IsEnabled = true;
                btnReplace.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnRename.IsEnabled = true;
                selectedSave = saveList.SelectedItem.ToString();
            }
            else
            {
                btnLoad.IsEnabled = false;
                btnReplace.IsEnabled = false;
                btnDelete.IsEnabled = false;
                btnRename.IsEnabled = false;
            }
        }

        // Al cargar archivo buscará el archivo seleccionado y reemplazará el principal.
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            File.Copy(path + "\\" + selectedProfile + "\\" + selectedSave, path + "\\" + mainSaveName, true);
            reloadProfiles();
            saveList.SelectedItem = selectedSave;
        }

        // Al reemplazar archivo buscará el original y lo reemplazará por el seleccionado.
        private void btnReplace_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Replace Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                File.Copy(path + "\\" + mainSaveName, path + "\\" + selectedProfile + "\\" + selectedSave, true);
                reloadProfiles();
            }
        }

        // Buscará el save seleccionado y lo eliminará.
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                File.Delete(path + "\\" + selectedProfile + "\\" + selectedSave);
                reloadProfiles();
            }
        }

        // Se abre dialogo para renombrar el perfil.
        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            new Dialogs.EditSave(this).Show();
        }

        public static void settingsChanger(string text, int line)
        {
            string[] settingsFile = File.ReadAllLines("settings.erso");
            settingsFile[line - 1] = text;
            File.WriteAllLines("settings.erso", settingsFile);
        }

        private void saveList_KeyDown(object sender, KeyEventArgs e)
        {
            if (saveList.SelectedIndex >= 0)
            {
                switch (e.Key)
                {
                    case Key.Delete:
                        btnDelete_Click(sender, e);
                        break;
                    case Key.F2:
                        btnRename_Click(sender, e);
                        break;
                    case Key.F4:
                        btnLoad_Click(sender, e);
                        break;
                    default: break;
                }
            }
        }
    }
}
