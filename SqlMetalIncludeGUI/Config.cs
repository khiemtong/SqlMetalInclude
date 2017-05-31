using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace SqlMetalIncludeGUI
{
    internal class Config : INotifyPropertyChanged
    {
        private static readonly Config Instance = new Config();

        public static Config Default
        {
			get { return Instance; }
        }

        private string _username = string.Empty;
        private string _password = string.Empty;
        private bool _windowsAuth = true;
        private bool _sqlAuth;
        private string _sqlServer = string.Empty;
        private string _sqlDatabase = string.Empty;

        private string _configFilename = string.Empty;
        private string _projectName = string.Empty;
        private bool _excludeMode;
        
        private bool _sqlMetalIncludeViews;
        private bool _sqlMetalIncludeSprocs;
        private string _sqlMetalOutput = "FullDbMap.dbml";
        private bool _sqlMetalPluralise;

        private string _outputCodeFile = string.Empty;
        private string _outputCodeNamespace = string.Empty;
        private string _outputCodeLanguage = "Default";
        private string _outputCodeEntityBase = string.Empty;
        private string _outputCodeContextName = string.Empty;
        private bool _outputCodeSerializable;
        private bool _outputCodeUseDesignerFile = true;

        private readonly ObservableCollection<DbObject> _dbObjects = new ObservableCollection<DbObject>();

        public ObservableCollection<DbObject> DbObjects
        {
            get { return _dbObjects; }
        }

        public void WriteConfig()
        {
            var config = new XDocument();

            var dbObjectsElement = new XElement("DbObjects");

            foreach (DbObject dbo in _dbObjects)
            {
                dbObjectsElement.Add(new XElement("DbObject",
                    new XAttribute("Name", dbo.Name),
                    new XAttribute("ListName", dbo.ListName),
                    new XAttribute("ClassName", dbo.ClassName),
                    new XAttribute("Selected", dbo.Selected)));
            }

            config.Add(new XElement("SqlMetalIncludeConfig",
                new XAttribute("ProjectName", _projectName),
                new XAttribute("ExcludeMode", _excludeMode),
                new XElement("SqlConfiguration",
                    new XAttribute("SqlServer", _sqlServer),
                    new XAttribute("SqlDatabase", _sqlDatabase),
                    new XAttribute("WindowsAuth", _windowsAuth),
                    new XAttribute("SqlAuth", _sqlAuth),
                    new XAttribute("Username", _username),
                    new XAttribute("Password", _password)),
                new XElement("SqlMetalConfiguration",
                    new XAttribute("IncludeViews", _sqlMetalIncludeViews),
                    new XAttribute("IncludeSprocs", _sqlMetalIncludeSprocs),
                    new XAttribute("OutputFilename", _sqlMetalOutput),
                    new XAttribute("Pluralise", _sqlMetalPluralise)),
                new XElement("OutputConfiguration",
                    new XAttribute("ContextName", _outputCodeContextName),
                    new XAttribute("Namespace", _outputCodeNamespace),
                    new XAttribute("Language", _outputCodeLanguage),
                    new XAttribute("EntityBase", _outputCodeEntityBase),
                    new XAttribute("Filename", _outputCodeFile),
                    new XAttribute("Serialisable", _outputCodeSerializable),
                    new XAttribute("UseDesignerFile", _outputCodeUseDesignerFile)),
                dbObjectsElement
                ));

            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(_configFilename)))
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_configFilename));

            config.Save(_configFilename);
        }

        public void LoadConfig()
        {
            XDocument config = XDocument.Load(_configFilename);

            if (config == null)
                return;

            XElement rootConfig = config.Element("SqlMetalIncludeConfig");

            if (rootConfig == null)
                return;

            _projectName = rootConfig.Attribute("ProjectName").Value;
            _excludeMode = Convert.ToBoolean(rootConfig.Attribute("ExcludeMode").Value);

            XElement sqlConfig = rootConfig.Element("SqlConfiguration");
            if (sqlConfig != null)
            {
                _sqlServer = sqlConfig.Attribute("SqlServer").Value;
                _sqlDatabase = sqlConfig.Attribute("SqlDatabase").Value;
                _windowsAuth = Convert.ToBoolean(sqlConfig.Attribute("WindowsAuth").Value);
                _sqlAuth = Convert.ToBoolean(sqlConfig.Attribute("SqlAuth").Value);
                _username = sqlConfig.Attribute("Username").Value;
                _password = sqlConfig.Attribute("Password").Value;
            }

            XElement sqlMetalConfig = rootConfig.Element("SqlMetalConfiguration");
            if (sqlMetalConfig != null)
            {
                _sqlMetalIncludeViews = Convert.ToBoolean(sqlMetalConfig.Attribute("IncludeViews").Value);
                _sqlMetalIncludeSprocs = Convert.ToBoolean(sqlMetalConfig.Attribute("IncludeSprocs").Value);
                _sqlMetalPluralise = Convert.ToBoolean(sqlMetalConfig.Attribute("Pluralise").Value);
                _sqlMetalOutput = sqlMetalConfig.Attribute("OutputFilename").Value;
            }

            XElement outputConfig = rootConfig.Element("OutputConfiguration");
            if (outputConfig != null)
            {
                _outputCodeContextName = outputConfig.Attribute("ContextName").Value;
                _outputCodeNamespace = outputConfig.Attribute("Namespace").Value;
                _outputCodeLanguage = outputConfig.Attribute("Language").Value;
                _outputCodeEntityBase = outputConfig.Attribute("EntityBase").Value;
                _outputCodeFile = outputConfig.Attribute("Filename").Value;
                _outputCodeSerializable = Convert.ToBoolean(outputConfig.Attribute("Serialisable").Value);
                _outputCodeUseDesignerFile = Convert.ToBoolean(outputConfig.Attribute("UseDesignerFile").Value);
            }

            XElement dbObjectsElement = rootConfig.Element("DbObjects");
            _dbObjects.Clear();

            if (dbObjectsElement != null)
            {
                foreach (XElement dbo in dbObjectsElement.Elements("DbObject"))
                {
                    _dbObjects.Add(new DbObject
                                   	{
                        Name = dbo.Attribute("Name").Value,
                        ListName = dbo.Attribute("ListName").Value,
                        ClassName = dbo.Attribute("ClassName").Value,
                        Selected = Convert.ToBoolean(dbo.Attribute("Selected").Value)
                    });
                }
            }
        }

        public bool OutputCodeUseDesignerFile
        {
            get { return _outputCodeUseDesignerFile; }
            set
            { 
                _outputCodeUseDesignerFile = value;
                NotifyChanged("OutputCodeUseDesignerFile");
            }
        }

        public bool OutputCodeSerializable
        {
            get { return _outputCodeSerializable; }
            set 
            {
                _outputCodeSerializable = value;
                NotifyChanged("OutputCodeSerializable");
            }
        }

        public string OutputCodeContextName
        {
            get { return _outputCodeContextName; }
            set 
            {
                _outputCodeContextName = value;
                NotifyChanged("OutputCodeContextName");
            }
        }

        public string ProjectName
        {
            get { return _projectName; }
            set 
            { 
                _projectName = value;
                NotifyChanged("ProjectName");
            }
        }

        public bool ExcludeMode
        {
            get { return _excludeMode; }
            set
            { 
                _excludeMode = value;
                NotifyChanged("ExcludeMode");
            }
        }

        public string ConfigFilename
        {
            get { return _configFilename; }
            set 
            { 
                _configFilename = value;
                NotifyChanged("ConfigFilename");
            }
        }

        public string OutputCodeEntityBase
        {
            get { return _outputCodeEntityBase; }
            set 
            { 
                _outputCodeEntityBase = value;
                NotifyChanged("OutputCodeEntityBase");
            }
        }

        public string OutputCodeLanguage
        {
            get { return _outputCodeLanguage; }
            set
            {
                _outputCodeLanguage = value;
                NotifyChanged("OutputCodeLanguage");
            }
        }

        public string OutputCodeNamespace
        {
            get { return _outputCodeNamespace; }
            set 
            {
                _outputCodeNamespace = value;
                NotifyChanged("OutputCodeNamespace");
            }
        }

        public string OutputCodeFile
        {
            get { return _outputCodeFile; }
            set
            { 
                _outputCodeFile = value;
                NotifyChanged("OutputCodeFile");
            }
        }

        public bool SqlMetalPluralise
        {
            get { return _sqlMetalPluralise; }
            set
            { 
                _sqlMetalPluralise = value;
                NotifyChanged("SqlMetalPluralise");
            }
        }

        public string SqlMetalOutput
        {
            get { return _sqlMetalOutput; }
            set
            {
                _sqlMetalOutput = value;
                NotifyChanged("SqlMetalOutput");
            }
        }

        public bool SqlMetalIncludeSprocs
        {
            get { return _sqlMetalIncludeSprocs; }
            set
            {
                _sqlMetalIncludeSprocs = value;
                NotifyChanged("SqlMetalIncludeSprocs");
            }
        }

        public bool SqlMetalIncludeViews
        {
            get { return _sqlMetalIncludeViews; }
            set 
            {
                _sqlMetalIncludeViews = value;
                NotifyChanged("SqlMetalIncludeViews");
            }
        }

        public string ConnectionString
        {
            get
            {
                var connString = new SqlConnectionStringBuilder
                                 	{
                                 		DataSource = _sqlServer,
                                 		InitialCatalog = _sqlDatabase,
                                 		IntegratedSecurity = _windowsAuth
                                 	};
            	if (_sqlAuth)
                {
                    connString.UserID = _username;
                    connString.Password = _password;
                }

                return connString.ToString();
            }
        }

        public bool TestConnection(bool showSuccessMessage, bool showFailureMessge)
        {
            var conn = new SqlConnection(ConnectionString);

            try
            {
                conn.Open();
            }
            catch (SqlException ex)
            {
                if (showFailureMessge)
                    MessageBox.Show(ex.Message, "Connection failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (showSuccessMessage)
                MessageBox.Show("Connection successful", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }

        public bool SqlConfigValid
        {
            get
            {
                return (!string.IsNullOrEmpty(_sqlServer) && !string.IsNullOrEmpty(_sqlDatabase) &&
                    (_windowsAuth || 
                    (_sqlAuth && !string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))));
            }
        }

        public string SqlDatabase
        {
            get { return _sqlDatabase; }
            set
            { 
                _sqlDatabase = value;
                NotifyChanged("SqlDatabase", "SqlConfigValid");
            }
        }

        public string SqlServer
        {
            get { return _sqlServer; }
            set 
            { 
                _sqlServer = value;
                NotifyChanged("SqlServer", "SqlConfigValid");
            }
        }

        public bool WindowsAuth
        {
            get { return _windowsAuth; }
            set 
            { 
                _windowsAuth = value;
                _sqlAuth = !value;
                NotifyChanged("WindowsAuth", "SqlAuth", "SqlConfigValid");
            }
        }

        public bool SqlAuth
        {
            get { return _sqlAuth; }
            set 
            {
                _sqlAuth = value;
                _windowsAuth = !value;
                NotifyChanged("WindowsAuth", "SqlAuth", "SqlConfigValid");
            }
        }

        public string Password
        {
            get { return _password; }
            set 
            { 
                _password = value;
                NotifyChanged("Password", "SqlConfigValid");
            }
        }

        public string Username
        {
            get { return _username; }
            set 
            { 
                _username = value;
                NotifyChanged("Username", "SqlConfigValid");
            }
        }

        private void NotifyChanged(params string[] propertyNames)
        {
            if (PropertyChanged != null)
            {
                foreach (string propertyName in propertyNames)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
