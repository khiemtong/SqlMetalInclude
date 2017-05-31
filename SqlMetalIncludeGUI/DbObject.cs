using System.ComponentModel;

namespace SqlMetalIncludeGUI
{
	/// <summary>
	/// Database object (ie table, view, stored procedure et cetera)
	/// </summary>
	/// <remarks></remarks>
    public class DbObject : INotifyPropertyChanged
    {
        private bool _selected;
        private string _name;
        private string _listName;
        private string _className;

		/// <summary>
		/// Gets or sets the name of the class.
		/// </summary>
		/// <value>The name of the class.</value>
		/// <remarks></remarks>
        public string ClassName
        {
            get { return _className; }
            set { _className = value;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("ClassName"));
            }
        }

		/// <summary>
		/// Gets or sets the name of the list (as used in DataContext).
		/// </summary>
		/// <value>The name of the list.</value>
		/// <remarks></remarks>
        public string ListName
        {
            get { return _listName; }
            set { _listName = value;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("ListName"));
            }
        }
		/// <summary>
		/// Gets or sets the name of the object.
		/// </summary>
		/// <value>The name.</value>
		/// <remarks></remarks>
        public string Name
        {
            get { return _name; }
            set { _name = value;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="DbObject" /> is selected.
		/// </summary>
		/// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
		/// <remarks></remarks>
        public bool Selected
        {
            get { return _selected; }
            set 
            { 
                _selected = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Selected"));
            }
        }

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		/// <remarks></remarks>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
