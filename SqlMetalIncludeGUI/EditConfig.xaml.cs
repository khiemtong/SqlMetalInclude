using System.Windows;

namespace SqlMetalIncludeGUI
{
    /// <summary>
    /// Interaction logic for EditConfig.xaml
    /// </summary>
    public partial class EditConfig
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="EditConfig" /> class.
		/// </summary>
		/// <remarks></remarks>
        public EditConfig()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Config.Default.WriteConfig();
            DialogResult = true;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Config.Default.LoadConfig();
            DialogResult = false;
            Close();
        }
    }
}
