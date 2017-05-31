/*
* Arguments class: application arguments interpreter
*
* Authors:		R. LOPES
* Contributors:	R. LOPES
* Created:		25 October 2002
* Modified:		28 October 2002
*
* Version:		1.0
 * 
 * http://www.codeproject.com/KB/recipes/command_line.aspx
*/

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace SqlMetalInclude
{

	/// <summary>
	/// Arguments class
	/// </summary>
	public class Arguments
	{
		private readonly StringDictionary _parameters;

		/// <summary>
		/// Initializes a new instance of the <see cref="Arguments" /> class.
		/// </summary>
		/// <param name="args">The args.</param>
		/// <remarks>
		/// Valid parameters forms:
		/// <code>
		/// {-,/,--}param{ ,=,:}((",')value(",'))
		/// Examples: -param1 value1 --param2 /param3:"Test-:-work" /param4=happy -param5 '--=nice=--'
		/// </code>
		/// </remarks>
		public Arguments(IEnumerable<string> args)
		{
			_parameters = new StringDictionary();
			var spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			var remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			string parameter = null;

			// Valid parameters forms:
			// {-,/,--}param{ ,=,:}((",')value(",'))
			// Examples: -param1 value1 --param2 /param3:"Test-:-work" /param4=happy -param5 '--=nice=--'
			foreach (var txt in args)
			{
				// Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
				var Parts = spliter.Split(txt, 3);
				switch (Parts.Length)
				{
					// Found a value (for the last parameter found (space separator))
					case 1:
						if (parameter != null)
						{
							if (!_parameters.ContainsKey(parameter))
							{
								Parts[0] = remover.Replace(Parts[0], "$1");
								_parameters.Add(parameter, Parts[0]);
							}
							parameter = null;
						}
						// else Error: no parameter waiting for a value (skipped)
						break;
					// Found just a parameter
					case 2:
						// The last parameter is still waiting. With no value, set it to true.
						if (parameter != null)
						{
							if (!_parameters.ContainsKey(parameter)) _parameters.Add(parameter, "true");
						}
						parameter = Parts[1];
						break;
					// Parameter with enclosed value
					case 3:
						// The last parameter is still waiting. With no value, set it to true.
						if (parameter != null)
						{
							if (!_parameters.ContainsKey(parameter)) _parameters.Add(parameter, "true");
						}
						parameter = Parts[1];
						// Remove possible enclosing characters (",')
						if (!_parameters.ContainsKey(parameter))
						{
							Parts[2] = remover.Replace(Parts[2], "$1");
							_parameters.Add(parameter, Parts[2]);
						}
						parameter = null;
						break;
				}
			}
			// In case a parameter is still waiting
			if (parameter != null)
			{
				if (!_parameters.ContainsKey(parameter)) _parameters.Add(parameter, "true");
			}
		}

		/// <summary>
		///Retrieve a parameter value if it exists
		/// </summary>
		/// <param name="param">The param.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public string this[string param]
		{
			get
			{
				return (_parameters[param]);
			}
		}

		/// <summary>
		/// Adds the specified key and value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <remarks></remarks>
		public void Add(string key, string value)
		{
			_parameters.Add(key, value);
		}
	}
}
