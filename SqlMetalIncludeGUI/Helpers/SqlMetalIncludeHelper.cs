using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using SqlMetalInclude;

namespace SqlMetalIncludeGUI.Helpers
{
	/// <summary>
	/// Wrapper for communicating with <see cref="Program">SqlMetalInclude</see>
	/// </summary>
	/// <remarks></remarks>
	public static class SqlMetalIncludeHelper
	{
		/// <summary>
		/// Gets the SqlMetalInclude executable path.
		/// </summary>
		/// <value>The SqlMetalInclude executable path.</value>
		/// <remarks></remarks>
		public static string SqlMetalIncludeExePath
		{
			get
			{
				var directoryName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
				Debug.Assert(directoryName != null);

				string startupPath =
					directoryName
						.Substring(6);

				return System.IO.Path.Combine(startupPath, "SqlMetalInclude.exe");
			}
		}

		/// <summary>
		/// Gets the output filename.
		/// </summary>
		/// <value>The output filename.</value>
		/// <remarks></remarks>
		public static string OutputFilename
		{
			get
			{
				return Config.Default.ProjectName + "_ProcessedDbMap.dbml";
			}
		}

		/// <summary>
		/// Gets the SqlMetalInclude <see cref="ProcessStartInfo"/>.
		/// </summary>
		/// <value>The SqlMetalInclude <see cref="ProcessStartInfo"/>.</value>
		/// <remarks></remarks>
		public static ProcessStartInfo SqlMetalIncludeStartInfo
		{
			get
			{
				var args = new List<string>
                               {
                                   string.Format("/dbml:\"{0}\"",
                                                 (string.IsNullOrEmpty(Config.Default.SqlMetalOutput)
                                                      ? "FullDbMap.dbml"
                                                      : Config.Default.SqlMetalOutput)),
                                   string.Format("/output:\"{0}\"", OutputFilename),
                                   "/verbose"
                               };

				var strings = Config.Default.DbObjects
					.Where(o => o.Selected)
					.Select(dbo => string.Format("{0}={1}/{2}", dbo.Name, dbo.ListName, dbo.ClassName))
					.ToArray();
				args.Add("/include:" + string.Join(",", strings));

				string argumentString = string.Join(" ", args.ToArray());

				var sqlMetalProcessStartInfo =
					new ProcessStartInfo(SqlMetalIncludeExePath, argumentString)
						{
							RedirectStandardOutput = true,
							WorkingDirectory = System.IO.Path.GetTempPath(),
							WindowStyle = ProcessWindowStyle.Hidden,
							CreateNoWindow = true,
							UseShellExecute = false
						};


				return sqlMetalProcessStartInfo;
			}
		}
	}
}
