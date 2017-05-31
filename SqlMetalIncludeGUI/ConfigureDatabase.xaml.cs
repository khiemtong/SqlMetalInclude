using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.ObjectModel;
using System.Data;

namespace SqlMetalIncludeGUI
{
	/// <summary>
	/// Database configuration window
	/// </summary>
	/// <remarks></remarks>
    public partial class ConfigureDatabase : INotifyPropertyChanged
    {
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		/// <remarks></remarks>
        public event PropertyChangedEventHandler PropertyChanged;
        private DataTable _servers;
        private readonly ObservableCollection<Database> _databases = new ObservableCollection<Database>();

        internal ConfigureDatabase()
        {
            InitializeComponent();
        }

        private void SqlServerNameDropDownOpened(object sender, EventArgs e)
        {
            Cursor current = Cursor;
            Cursor = Cursors.Wait;

            _servers = SmoApplication.EnumAvailableSqlServers();
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Servers"));
            Cursor = current;
        }

        internal ObservableCollection<Database> Databases
        {
            get { return _databases; }
        }

		internal DataTable Servers
        {
            get { return _servers; }
        }

        private void SqlDatabaseNameDropDownOpened(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SqlServerName.Text))
                return;

            Cursor current = Cursor;
            Cursor = Cursors.Wait;

            _databases.Clear();
            foreach (Database db in new Server(SqlServerName.Text).Databases)
                _databases.Add(db);

            Cursor = current;
        }

        private void TestConnectionClick(object sender, RoutedEventArgs e)
        {
            Config.Default.TestConnection(true, true);
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            if (Config.Default.TestConnection(false, true))
            {
                //Write config before continuing
                Config.Default.WriteConfig();
                NavigationService.Navigate(new SqlMetalConfig());
            }
        }
    }
}
