using System.Xml.Linq;

namespace SqlMetalInclude
{
	/// <summary>
	/// Mapping to specific element types
	/// </summary>
	/// <remarks></remarks>
    public class DbmlElements
    {
		/// <summary>
		/// Reference to the Linq to Sql schema name
		/// </summary>
        public const string SchemaName = "http://schemas.microsoft.com/linqtosql/dbml/2007";
		/// <summary>
		/// Mapping to table information
		/// </summary>
        public static readonly XName Table = XName.Get("Table", SchemaName);
		/// <summary>
		/// Mapping to type information
		/// </summary>
        public static readonly XName Type = XName.Get("Type", SchemaName);
		/// <summary>
		/// Mapping to foreign key information
		/// </summary>
        public static readonly XName Association = XName.Get("Association", SchemaName);
		/// <summary>
		/// Mapping to database column information
		/// </summary>
        public static readonly XName Column = XName.Get("Column", SchemaName);
    }
}