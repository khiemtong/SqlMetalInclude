using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;
using SqlMetalInclude;

namespace SqlMetalIncludeGUI
{
	/// <summary>
	/// Interaction logic for SelectDbObjects.xaml
	/// </summary>
	public partial class SelectDbObjects
	{
		readonly BackgroundWorker _dbmlReader = new BackgroundWorker();
		readonly Timer _searchTimer = new Timer(500) { AutoReset = false };
		bool _hasChanged = true;
		readonly BackgroundWorker _sqlMetalIncludeWorker = new BackgroundWorker();

		/// <summary>
		/// Initializes a new instance of the <see cref="SelectDbObjects" /> class.
		/// </summary>
		/// <remarks></remarks>
		public SelectDbObjects()
		{
			Config.Default.DbObjects.CollectionChanged += DbObjects_CollectionChanged;

			InitializeComponent();

			_dbmlReader.RunWorkerCompleted += _dbmlReader_RunWorkerCompleted;
			_dbmlReader.DoWork += _dbmlReader_DoWork;
			_dbmlReader.RunWorkerAsync();
			_searchTimer.Elapsed += _searchTimer_Elapsed;

			_sqlMetalIncludeWorker.RunWorkerCompleted += _sqlMetalIncludeWorker_RunWorkerCompleted;
			_sqlMetalIncludeWorker.DoWork += _sqlMetalIncludeWorker_DoWork;
		}

		void DbObjects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			_hasChanged = true;
		}

		void _searchTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.Invoke((Action)delegate
			{
				var lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(dbObjectsDataGrid.ItemsSource);

				lcv.Filter = Filter;
			});
		}

		void _dbmlReader_DoWork(object sender, DoWorkEventArgs e)
		{
			XDocument doc = XDocument.Load(System.IO.Path.Combine(System.IO.Path.GetTempPath(), Config.Default.SqlMetalOutput));

			foreach (XElement table in doc.Descendants(XName.Get("Table", DbmlElements.SchemaName)))
			{

				DbObject dbo =
					Config.Default.DbObjects.SingleOrDefault(o =>
						o.Name == table.Attribute("Name").Value);

				if (dbo == null)
				{
					dbo = new DbObject
							{
								Name = table.Attribute("Name").Value,
								ListName = table.Attribute("Member").Value,
								ClassName = table.Element(XName.Get("Type", DbmlElements.SchemaName)).Attribute("Name").Value,
								Selected = !Config.Default.ExcludeMode //Only select if exclude mode is off
							};

					Dispatcher.Invoke((Action)delegate
					{
						Config.Default.DbObjects.Add(dbo);
					});
				}
			}
		}

		void _dbmlReader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			NextButton.Content = "Continue";
			NextButton.IsEnabled = true;
		}

		private void NextButton_Click(object sender, RoutedEventArgs e)
		{
			if (Config.Default.DbObjects.Count(dbo => dbo.Selected) > 0)
			{
				if (_hasChanged)
				{
					Config.Default.WriteConfig();
					RunSqlMetalInclude();
				}
				else
				{
					Config.Default.WriteConfig();
					NavigationService.Navigate(new CodeGen());
				}
			}
			else
			{
				MessageBox.Show("You must select at least one object");
			}
		}

		Cursor _currentCursor;

		private void RunSqlMetalInclude()
		{
			dbObjectsDataGrid.IsEnabled = false;
			NextButton.IsEnabled = false;
			_currentCursor = Cursor;
			Cursor = Cursors.AppStarting;
			sqlMetalOutputGroup.Visibility = Visibility.Visible;
			sqlMetalIncludeOutput.Text = string.Empty;
			_sqlMetalIncludeWorker.RunWorkerAsync();
		}

		void _sqlMetalIncludeWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			ProcessStartInfo sqlMetalProcessStartInfo = Helpers.SqlMetalIncludeHelper.SqlMetalIncludeStartInfo;

			Dispatcher.Invoke((Action)delegate
			{
				sqlMetalIncludeOutput.AppendText(string.Format("Executing 'SqlMetalInclude.exe {0}'\r\n", sqlMetalProcessStartInfo.Arguments));
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
				sqlMetalIncludeOutput.AppendText(Environment.NewLine);
				sqlMetalIncludeOutput.AppendText(e.Data);
				sqlMetalIncludeOutput.ScrollToEnd();
			});
		}

		void _sqlMetalIncludeWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			dbObjectsDataGrid.IsEnabled = true;
			NextButton.IsEnabled = true;
			Cursor = _currentCursor;
			if (Convert.ToInt32(e.Result) == 0)
				_hasChanged = false;
			else
				_hasChanged = true;

			sqlMetalIncludeOutput.AppendText(Environment.NewLine);
			sqlMetalIncludeOutput.AppendText("SqlMetalInclude execution completed with no errors");
			sqlMetalIncludeOutput.ScrollToEnd();
		}

		private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			_searchTimer.Stop();
			_searchTimer.Start();
		}

		private bool Filter(Object value)
		{
			return ((DbObject)value).Name.IndexOf(searchBox.Text, StringComparison.InvariantCultureIgnoreCase) != -1;
		}

		private void SelectAll_Click(object sender, RoutedEventArgs e)
		{
			foreach (DbObject dbo in Config.Default.DbObjects)
			{
				dbo.Selected = true;
			}
		}

		private void DeselectAll_Click(object sender, RoutedEventArgs e)
		{
			foreach (DbObject dbo in Config.Default.DbObjects)
			{
				dbo.Selected = false;
			}
		}
	}
}
