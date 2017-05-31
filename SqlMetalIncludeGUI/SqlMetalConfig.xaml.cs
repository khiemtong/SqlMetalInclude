using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace SqlMetalIncludeGUI
{
    /// <summary>
    /// Interaction logic for SqlMetalConfig.xaml
    /// </summary>
    public partial class SqlMetalConfig
    {
    	readonly BackgroundWorker _sqlMetalWorker = new BackgroundWorker();
        bool _generatedDbml = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlMetalConfig" /> class.
		/// </summary>
		/// <remarks></remarks>
        public SqlMetalConfig()
        {
            InitializeComponent();

			_sqlMetalWorker.RunWorkerCompleted += _sqlMetalWorker_RunWorkerCompleted;
            _sqlMetalWorker.DoWork += _sqlMetalWorker_DoWork;
        }

        void _sqlMetalWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ProcessStartInfo sqlMetalProcessStartInfo = Helpers.SqlMetalHelper.SqlMetalGenDbmlStartInfo;

            Dispatcher.Invoke((Action)delegate
            {
                sqlMetalOutput.AppendText(string.Format("Executing 'SqlMetal.exe {0}'\r\n", sqlMetalProcessStartInfo.Arguments));
                sqlMetalOutput.ScrollToEnd();
            });

            var sqlMetalProcess = new Process {StartInfo = sqlMetalProcessStartInfo};
        	sqlMetalProcess.OutputDataReceived += sqlMetalProcess_OutputDataReceived;
            sqlMetalProcess.Start();
            sqlMetalProcess.BeginOutputReadLine();
            sqlMetalProcess.WaitForExit();
            e.Result = sqlMetalProcess.ExitCode;
        }

        void sqlMetalProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke((Action)delegate
            {
                sqlMetalOutput.AppendText(Environment.NewLine);
                sqlMetalOutput.AppendText(e.Data);
                sqlMetalOutput.ScrollToEnd();
            });
        }

        void _sqlMetalWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((int)e.Result == 0 && 
                System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetTempPath(), Config.Default.SqlMetalOutput)))
            {
                _generatedDbml = true;
                RegenDbml.Visibility = Visibility.Visible;
                RegenDbml.IsEnabled = true;
                NextButton.IsEnabled = true;

                sqlMetalOutput.AppendText(Environment.NewLine);
                sqlMetalOutput.AppendText("Successfully generated dbml file");
                sqlMetalOutput.ScrollToEnd();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_generatedDbml)
            {
                //Save config before continuing
                Config.Default.WriteConfig();
                NavigationService.Navigate(new SelectDbObjects());
            }
            else
            {
                RunSqlMetal();
            }
        }

        private void RunSqlMetal()
        {
            if (!_sqlMetalWorker.IsBusy)
            {
                RegenDbml.IsEnabled = false;
                NextButton.IsEnabled = false;
                //Make sure output box is visible
                sqlMetalOutputGroup.Visibility = Visibility.Visible;
                //Clear previous output
                sqlMetalOutput.Text = "";

                _sqlMetalWorker.RunWorkerAsync();
            }
        }

        private void RegenDbml_Click(object sender, RoutedEventArgs e)
        {
            RunSqlMetal();
        }
    }
}
