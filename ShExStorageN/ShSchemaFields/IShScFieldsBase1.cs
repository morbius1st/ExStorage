#region + Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields.ShScSupport;

#endregion

// user name: jeffs
// created:   10/29/2022 4:03:43 PM


// holds interfaces and abstract classes for the various field collection
// classes: ...DataSheet, ...DataRow, ...DataLock


namespace ShExStorageN.ShSchemaFields
{
	/// <summary>
	/// abstract class for a sheet - implements AShScFields & IShScRows1<br/>
	/// adds the collection of rows and the routine to add a row
	/// </summary>
	public abstract class AShScSheet<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow> :
			AShScFields<TShtKey, TShtFlds>,
			IShScRows1<TRowKey, TRowFlds, TRow>

		where TShtKey : Enum
		where TRowKey : Enum
		where TShtFlds : IShScFieldData1<TShtKey>, new()
		where TRowFlds : IShScFieldData1<TRowKey>, new()
		where TRow : AShScRow<TRowKey, TRowFlds>, new()
	{
		public Dictionary<string, TRow> Rows { get; protected set; }

		public abstract void AddRow(TRow row);
	}

	public abstract class AShScRow<TKey, TField> : 
		AShScFields<TKey, TField>
		where TKey : Enum
		where TField : IShScFieldBase1<TKey>, new()
	{
		protected AShScRow()
		{
			init();
		}

		protected abstract void init();

	}

	public abstract class AShScLock<TKey, TField> : 
		AShScFields<TKey, TField>
		where TKey : Enum
		where TField : IShScFieldBase1<TKey>, new()
	{
		protected AShScLock()
		{
			init();
		}

		protected abstract void init();

		public abstract void Configure(LokExId lokExId);

		public abstract string MachineName { get; }
	}

	/// <summary>
	/// basic abstract class for either sheet or rows<br/>
	///	has the basic collection of fields<br/>
	/// includes some pre-defined routines<br/>
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TField"></typeparam>
	public abstract class AShScFields<TKey, TField> : IShScFieldsBase1<TKey, TField>
		where TKey : Enum
		where TField : IShScFieldBase1<TKey>, new()
	{
		protected AShScFields()
		{
			Fields = new Dictionary<TKey, TField>();
		}

		public Dictionary<TKey, TField> Fields { get; set; }
		public abstract string SchemaKey { get; }
		public abstract string SchemaName { get; }
		public abstract string SchemaDesc { get; }
		public abstract string SchemaVersion { get; }
		public abstract string UserName { get; }

		public abstract Guid SchemaGuid { get; }

		public abstract string ModelName { get; }
		public abstract string ModelPath { get; }

		public abstract string Date { get; }

		public virtual dynamic GetValueAs<T>(Enum fieldKey)
		{
			// return Fields[(TKey) fieldKey].DyValue.Value;
			return Fields[(TKey) fieldKey].DyValue.GetValueAs<T>();
		}

		public virtual TField GetField(Enum fieldKey)
		{
			TField f = Fields[(TKey) fieldKey];

			return f;
		}

		public virtual void SetValue(TKey FieldKey, dynamic value)
		{
			Fields[FieldKey].SetValue = value;
		}

		public virtual void Add(TField field)
		{
			Fields.Add(field.FieldKey, field);
		}

		public Dictionary<TKey, TField> CloneFields()
		{
			Dictionary<TKey, TField> copy = new Dictionary<TKey, TField>();

			foreach (KeyValuePair<TKey, TField> kvp in Fields)
			{
				copy.Add(kvp.Key, kvp.Value);
			}

			return null;
		}

		public abstract void ParseEnum(Type t, string enumName);

	}

	// defines a sheet which has a collection of fields and a collection of rows
	public interface IShScRows1<TRowKey, TRowFld, TRow>
		where TRowKey : Enum
		where TRowFld :  IShScFieldBase1<TRowKey>, new()
		where TRow : IShScFieldsBase1<TRowKey, TRowFld>, new()
	{
		Dictionary<string, TRow> Rows { get; }

		void AddRow(TRow row);
	}

	// the basic collection of fields
	// either sheet which is a collection of sheet fields which is a class of sub-fields
	// or row which is a collection of row fields which is a class of sub-fields
	// field e.g. [schema name]
	public interface IShScFieldsBase1<TKey, TField>
		where TKey : Enum
		where TField : IShScFieldBase1<TKey>, new()
	{
		Dictionary<TKey, TField> Fields { get; }

		TField GetField(Enum fieldKey);

		dynamic GetValueAs<T>(Enum FieldKey);
		void SetValue(TKey FieldKey, dynamic value);

		void Add(TField field);

		string SchemaKey { get; }
		string SchemaName { get; }
		string SchemaDesc { get; }
		string SchemaVersion { get; }
		string UserName { get; }
		string Date { get; }
		string ModelPath { get; }
		string ModelName { get; }
		Guid SchemaGuid { get; }
	}
}

/*
	

Base:		IShScFieldsBase1  : none


			IShScRows1  : none
			|
			|	+-- < AShScFields : IShScFieldsBase1
			|	|
			|	+-------------------+
			|	|					v
			|	|	AshScRow  :  AShScFields
			|	|
			|	+---------------------------+
			+---------------+				|
							v				v
			AShScSheet  :  IShScRows1  : AShScFields







*/