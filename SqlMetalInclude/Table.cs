namespace SqlMetalInclude
{
	/// <summary>
	/// Simple table information, allows renaming of collection and class names
	/// </summary>
	/// <remarks></remarks>
    public class Table
    {
		/// <summary>
		/// Gets or sets the name of the table, used for identifying the original table name.
		/// </summary>
		/// <value>The name of the table.</value>
		/// <remarks></remarks>
        public string TableName { get; set; }
		/// <summary>
		/// Gets or sets the name of the list, as it appear in a DataContext.
		/// </summary>
		/// <value>The name of the list.</value>
		/// <remarks></remarks>
        public string ListName { get; set; }
		/// <summary>
		/// Gets or sets the name of the type that corresponds to the table.
		/// </summary>
		/// <value>The name of the type.</value>
		/// <remarks></remarks>
        public string TypeName { get; set; }
    }
}