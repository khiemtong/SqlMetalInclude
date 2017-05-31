using System;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;

namespace SqlMetalIncludeGUI
{
	/// <summary>
	/// Code generation settings window
	/// </summary>
	/// <remarks></remarks>
    public partial class CodeGen
    {
        readonly BackgroundWorker _codeGenWorker = new BackgroundWorker();

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeGen" /> class.
		/// </summary>
		/// <remarks></remarks>
        public CodeGen()
        {
            InitializeComponent();

            _codeGenWorker.RunWorkerCompleted += CodeGenWorkerRunWorkerCompleted;
            _codeGenWorker.DoWork += CodeGenWorkerDoWork;
        }

        void CodeGenWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            ProcessStartInfo sqlMetalGenCodeStartInfo = Helpers.SqlMetalHelper.SqlMetalGenCodeStartInfo;

            var sqlMetalProcess = new Process {StartInfo = sqlMetalGenCodeStartInfo};
            sqlMetalProcess.OutputDataReceived += SqlMetalProcessOutputDataReceived;
            sqlMetalProcess.Start();
            sqlMetalProcess.BeginOutputReadLine();
            sqlMetalProcess.WaitForExit();
            e.Result = sqlMetalProcess.ExitCode;
        }

        void SqlMetalProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke((Action)delegate
            {
                codeGenOutput.AppendText(Environment.NewLine);
                codeGenOutput.AppendText(e.Data);
                codeGenOutput.ScrollToEnd();
            });
        }

        void CodeGenWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var saveFile = new System.Windows.Forms.SaveFileDialog
                               {
                                   AutoUpgradeEnabled = true,
                                   OverwritePrompt = true,
                                   Title = "Save code file as",
                                   FileName = Config.Default.OutputCodeFile
                               };


            if (System.IO.Path.GetExtension(Config.Default.OutputCodeFile) == ".cs")
            {
                saveFile.DefaultExt = "cs";
                saveFile.Filter = "C# code file (*.cs)|*.cs";
            }
            else if (System.IO.Path.GetExtension(Config.Default.OutputCodeFile) == ".vb")
            {
                saveFile.DefaultExt = "vb";
                saveFile.Filter = "VB code file (*.vb)|*.vb";
            }
            else
                throw new ArgumentException("Unexpected extension for code file");

            if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string codeFile = saveFile.FileName;

                //We need to cater for if the person has added .designer to the filename themselves.
                if (Config.Default.OutputCodeUseDesignerFile)
                {
                    string nonDesignerCodeFile;

                    string extension = System.IO.Path.GetExtension(codeFile);

                    string filename = System.IO.Path.GetFileNameWithoutExtension(codeFile);

                    //They have added it themselves, just save
                    if (filename.EndsWith(".designer"))
                    {
                        nonDesignerCodeFile = System.IO.Path.Combine(
                            System.IO.Path.GetDirectoryName(codeFile),
                            (System.IO.Path.GetFileNameWithoutExtension(filename) + extension));
                    }
                    else
                    {
                        nonDesignerCodeFile = System.IO.Path.Combine(
                            System.IO.Path.GetDirectoryName(codeFile),
                            (filename + extension));
                        codeFile = System.IO.Path.Combine(
                            System.IO.Path.GetDirectoryName(codeFile),
                            (filename + ".designer" + extension));
                    }

                    if (System.IO.File.Exists(codeFile))
                        System.IO.File.Delete(codeFile);

                    System.IO.File.Copy(Helpers.SqlMetalHelper.SqlMetalCodeOutputFullPath, codeFile);

                    //Build user code file if one doesnt exist.
                    GenerateNonDesignerCodeFile(nonDesignerCodeFile);
                }
                else
                {
                    System.IO.File.Copy(Helpers.SqlMetalHelper.SqlMetalCodeOutputFullPath, codeFile);
                }
            }

            NextButton.IsEnabled = true;
            exportBatch.IsEnabled = true;
        }

        private static void GenerateNonDesignerCodeFile(string nonDesignerCodeFile)
        {
            if (!System.IO.File.Exists(nonDesignerCodeFile))
            {
                if (System.IO.Path.GetExtension(nonDesignerCodeFile) == ".cs")
                {
                    //C# file with namespace
                    if (!string.IsNullOrEmpty(Config.Default.OutputCodeNamespace))
                    {
                        System.IO.File.WriteAllText(nonDesignerCodeFile,
                            (string.IsNullOrEmpty(Config.Default.OutputCodeNamespace) ?
                            Properties.Settings.Default.NonDesignerCSharpFileWithoutNamespace :
                            Properties.Settings.Default.NonDesignerCSharpFileWithNamespace)
                            .Replace("{Namespace}", Config.Default.OutputCodeNamespace)
                            .Replace("{Class}", Config.Default.OutputCodeContextName));
                    }
                }
                else if (System.IO.Path.GetExtension(nonDesignerCodeFile) == ".vb")
                {
                    //VB file with namespace
                    if (!string.IsNullOrEmpty(Config.Default.OutputCodeNamespace))
                        System.IO.File.WriteAllText(nonDesignerCodeFile,
                            string.Format(Properties.Settings.Default.NonDesignerVbFileWithNamespace,
                                Config.Default.OutputCodeNamespace, Config.Default.OutputCodeContextName));
                    else
                        System.IO.File.WriteAllText(nonDesignerCodeFile,
                            string.Format(Properties.Settings.Default.NonDesignerVbFileWithoutNamespace,
                                Config.Default.OutputCodeContextName));
                }
            }
        }

        private void RunCodeGen()
        {
            if (!_codeGenWorker.IsBusy)
            {
                Config.Default.WriteConfig();

                NextButton.IsEnabled = false;
                exportBatch.IsEnabled = false;
                codeGenOutput.Text = string.Empty;

                _codeGenWorker.RunWorkerAsync();
            }
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            RunCodeGen();
        }

        private void ExportBatchClick(object sender, RoutedEventArgs e)
        {
            Config.Default.WriteConfig();
            var saveFile = new System.Windows.Forms.SaveFileDialog
                               {
                                   AutoUpgradeEnabled = true,
                                   DefaultExt = "cmd",
                                   Filter = "CMD file (*.cmd)|*.cmd",
                                   OverwritePrompt = true,
                                   Title = "Save batch script as"
                               };


            if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var sqlMetalIncludeStartInfo = Helpers.SqlMetalIncludeHelper.SqlMetalIncludeStartInfo;
                var sqlMetalStartInfo = Helpers.SqlMetalHelper.SqlMetalGenDbmlStartInfo;
                var sqlMetalGenCodeStartInfo = Helpers.SqlMetalHelper.SqlMetalGenCodeStartInfo;

                var batchText = new StringBuilder();
                batchText.AppendLine("@ECHO OFF");
                batchText.AppendLine();
                batchText.AppendLine(string.Format("\"{0}\" {1}", sqlMetalStartInfo.FileName,
                    sqlMetalStartInfo.Arguments));
                batchText.AppendLine();
                batchText.AppendLine(string.Format("\"{0}\" {1}", sqlMetalIncludeStartInfo.FileName,
                    sqlMetalIncludeStartInfo.Arguments));
                batchText.AppendLine();
                batchText.AppendLine(string.Format("\"{0}\" {1}", sqlMetalGenCodeStartInfo.FileName,
                    sqlMetalGenCodeStartInfo.Arguments));
                batchText.AppendLine();
                batchText.AppendLine("PAUSE");
                batchText.AppendLine();

                if (System.IO.File.Exists(saveFile.FileName))
                    System.IO.File.Delete(saveFile.FileName);

                System.IO.File.WriteAllText(saveFile.FileName, batchText.ToString());

                string sqlMetalIncludeExeNewPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(saveFile.FileName),
                    "SqlMetalInclude.exe");

                //Incase we have a new version
                if (System.IO.File.Exists(sqlMetalIncludeExeNewPath))
                    System.IO.File.Delete(sqlMetalIncludeExeNewPath);

                System.IO.File.Copy(Helpers.SqlMetalIncludeHelper.SqlMetalIncludeExePath,
                    sqlMetalIncludeExeNewPath);
            }
        }
    }
}
