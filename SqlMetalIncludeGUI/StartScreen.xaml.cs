using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace SqlMetalIncludeGUI
{
    /// <summary>
    /// Interaction logic for StartScreen.xaml
    /// </summary>
    public partial class StartScreen
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="StartScreen" /> class.
		/// </summary>
		/// <remarks></remarks>
        public StartScreen()
        {
            //Set up default preferences if first run
            if (string.IsNullOrEmpty(Properties.Settings.Default.ConfigDirectory))
            {
                Properties.Settings.Default.ConfigDirectory =
                    System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SqlMetalIncludeConfig");
                Properties.Settings.Default.Save();
            }

            InitializeComponent();
        }

        List<string> _configurationFiles;
		/// <summary>
		/// Gets the configuration files.
		/// </summary>
		/// <value>The configuration files.</value>
		/// <remarks></remarks>
        public List<string> ConfigurationFiles
        {
            get
            {
                if (_configurationFiles == null)
                {
                    if (!System.IO.Directory.Exists(System.IO.Path.GetFullPath(Properties.Settings.Default.ConfigDirectory)))
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetFullPath(Properties.Settings.Default.ConfigDirectory));

                    _configurationFiles =new List<string>(System.IO.Directory.GetFiles(Properties.Settings.Default.ConfigDirectory, "*.xml"));

                    for (int i = 0; i < _configurationFiles.Count; i++)
                    {
                        _configurationFiles[i] = System.IO.Path.GetFileNameWithoutExtension(_configurationFiles[i]);
                    }
                }

                return _configurationFiles;
            }
        }

        private void newConfigContinue_Click(object sender, RoutedEventArgs e)
        {
            Config.Default.ConfigFilename = System.IO.Path.Combine(Properties.Settings.Default.ConfigDirectory, projectName.Text + ".xml");
            Config.Default.WriteConfig();
            NavigationService.Navigate(new ConfigureDatabase());
        }

        private void loadConfigContinue_Click(object sender, RoutedEventArgs e)
        {
            if (loadList.SelectedItem != null)
            {
                Config.Default.ConfigFilename =
                    System.IO.Path.Combine(Properties.Settings.Default.ConfigDirectory, loadList.SelectedItem + ".xml");
                Config.Default.LoadConfig();

                NavigationService.Navigate(new ConfigureDatabase());
            }
        }

        private void savePrefButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reload();
        }

        private void editConfig_Click(object sender, RoutedEventArgs e)
        {
            if (loadList.SelectedItem != null)
            {
                Config.Default.ConfigFilename =
                    System.IO.Path.Combine(Properties.Settings.Default.ConfigDirectory, loadList.SelectedItem + ".xml");
                Config.Default.LoadConfig();
                if (new EditConfig().ShowDialog() == true)
                {
                    NavigationService.Navigate(new ConfigureDatabase());
                }
            }
        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            var browse = new System.Windows.Forms.FolderBrowserDialog();

            if (browse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.ConfigDirectory = browse.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void loadList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            loadConfigContinue_Click(sender, new RoutedEventArgs());
        }
    }
}
