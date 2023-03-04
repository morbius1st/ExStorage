#region + Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;

#endregion

// user name: jeffs
// created:   10/29/2022 4:03:43 PM


// holds interfaces and abstract classes for the various field collection
// classes: ...DataSheet, ...DataRow, ...DataLock


namespace ShExStorageN.ShSchemaFields
{
	// defines a component of sheet which has a collection of rows which has a collection of fields
	public interface IShScRows<TRowKey, TRowFld, TRow>
		where TRowKey : Enum
		where TRowFld :  IShScFieldBase<TRowKey>, new()
		where TRow : IShScFieldsBase<TRowKey, TRowFld>, new()
	{
		Dictionary<string, TRow> Rows { get; }

		void AddRow(TRow row);
	}


	// the basic collection of fields
	// either sheet which is a collection of sheet fields which is a class of sub-fields
	// or row which is a collection of row fields which is a class of sub-fields
	// field e.g. [schema name]
	public interface IShScFieldsBase<TKey, TField>
		where TKey : Enum
		where TField : IShScFieldBase<TKey>, new()
	{
		// Dictionary<TKey, TField> Fields { get; }
	
		TField GetField(Enum fieldKey);
	
		dynamic GetValueAs<T>(Enum FieldKey);
		void SetValue(TKey FieldKey, dynamic value);
	
		void Add(TField field);
	}
}

