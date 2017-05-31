using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace SqlMetalIncludeGUI.Helpers
{
	/// <summary>
	/// Wrapper for communicating with SqlMetal
	/// </summary>
	/// <remarks></remarks>
	public static class SqlMetalHelper
	{
		/// <summary>
		/// Gets the default SqlMetal <see cref="ProcessStartInfo"/>. Throws exception when SqlMetal is not found
		/// </summary>
		public static ProcessStartInfo SqlMetalGenDbmlStartInfo
		{
			get
			{
				var args = new List<string> {string.Format("/conn:\"{0}\"", Config.Default.ConnectionString)};

				if (Config.Default.SqlMetalIncludeViews)
					args.Add("/views");
				if (Config.Default.SqlMetalIncludeSprocs)
					args.Add("/sprocs");
				if (Config.Default.SqlMetalPluralise)
					args.Add("/pluralize");

				args.Add(string.Format("/dbml:{0}", Config.Default.SqlMetalOutput));

				return GetSqlMetalProcessStartInfo(args);
			}
		}

		/// <summary>
		/// Gets the SqlMetal <see cref="ProcessStartInfo"/>. Throws exception when SqlMetal is not found
		/// </summary>
		/// <param name="args">The argument used to call SqlMetal.</param>
		/// <returns>A <see cref="ProcessStartInfo"/>, usable for calling SqlMetal</returns>
		/// <exception cref="ConfigurationErrorsException">When SqlMetal is not found at the configured location.</exception>
		private static ProcessStartInfo GetSqlMetalProcessStartInfo(List<string> args)
		{
			string argumentString = string.Join(" ", args.ToArray());

			var sqlMetalExe = Properties.Settings.Default.SqlMetalExe;
			if (!new FileInfo(sqlMetalExe).Exists)
			{
				throw new ConfigurationErrorsException("Configure the location of SqlMetal in the applications' configuration file");
			}

			var sqlMetalProcessStartInfo = new ProcessStartInfo(sqlMetalExe, argumentString)
											{
												RedirectStandardOutput = true,
												WorkingDirectory = Path.GetTempPath(),
												WindowStyle = ProcessWindowStyle.Hidden,
												CreateNoWindow = true,
												UseShellExecute = false
											};


			return sqlMetalProcessStartInfo;
		}

		/// <summary>
		/// Gets the process start information for SqlMetal
		/// </summary>
		/// <value>The SqlMetal start information.</value>
		/// <remarks></remarks>
		public static ProcessStartInfo SqlMetalGenCodeStartInfo
		{
			get
			{
				var args = new List<string>
				           	{
				           		string.Format("\"{0}\"", SqlMetalIncludeHelper.OutputFilename),
				           		string.Format("/code:\"{0}\"", SqlMetalCodeOutputFullPath),
				           		string.Format("/context:{0}", Config.Default.OutputCodeContextName)
				           	};

				if (Config.Default.OutputCodeLanguage != "Default")
					args.Add(string.Format("/language:{0}", Config.Default.OutputCodeLanguage));

				if (!string.IsNullOrEmpty(Config.Default.OutputCodeNamespace))
					args.Add(string.Format("/namespace:{0}", Config.Default.OutputCodeNamespace));

				if (Config.Default.OutputCodeSerializable)
					args.Add("/serialization:Unidirectional");
				
				return GetSqlMetalProcessStartInfo(args);
			}
		}

		/// <summary>
		/// Gets the SQL metal output filename. Takes into account the user settings to save as designer.cs
		/// </summary>
		/// <value>The SQL metal output filename.</value>
		public static string SqlMetalCodeOutputFullPath
		{
			get
			{
				string codeFile = Config.Default.OutputCodeFile;

				if (Config.Default.OutputCodeUseDesignerFile &&
					(!codeFile.EndsWith(".designer.cs") || !codeFile.EndsWith(".designer.vb")))
				{
					string extension = Path.GetExtension(codeFile);

					string filename = Path.GetFileNameWithoutExtension(codeFile);

					return Path.Combine(Path.IsPathRooted(codeFile) 
					                    	? Path.GetDirectoryName(codeFile) 
					                    	: Path.GetTempPath(), 
											filename + ".designer" + extension);
				}

				return codeFile;
			}
		}
	}
}
