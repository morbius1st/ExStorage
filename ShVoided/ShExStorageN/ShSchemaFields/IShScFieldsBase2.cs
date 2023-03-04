#region + Using Directives

using static ShExStorageN.ShSchemaFields.ShScSupport.ShExConstN;
using static ShExStorageN.ShSchemaFields.ShScSupport.ShExNTblKeys;
using static ShExStorageC.ShSchemaFields.ShScSupport.ScShtKeys;
using KeyEqualityComparer = ShExStorageN.ShSchemaFields.ShScSupport.KeyEqualityComparer;

#endregion

// user name: jeffs
// created:   10/29/2022 4:03:43 PM


// holds interfaces and abstract classes for the various field collection
// classes: ...DataSheet, ...DataRow, ...DataLock


namespace ShExStorageN.ShSchemaFields
{
	/// <summary>
	/// basic abstract class for either sheet or rows<br/>
	///	has the basic collection of fields<br/>
	/// includes some pre-defined routines<br/>
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TField"></typeparam>
	public abstract class AShScFields2<TField> : IShScFieldsBase2<TField>
		
		where TField : IShScFieldBase2, new()
	{
		protected AShScFields2()
		{
			// Fields = new Dictionary<TKey, TField>();
		}

		protected KeyEqualityComparer kc = new KeyEqualityComparer();

		public abstract Dictionary<KEY, TField> Fields { get; protected set; }

		public string SchemaName => GetValue<string>(TK_SCHEMA_NAME );
		public string SchemaDesc => GetValue<string>(TK_DESCRIPTION );
		public Guid SchemaGuid => GetValue<Guid>(TK_GUID );
		public string UserName => GetValue<string>(TK_USERNAME );

		public string SchemaKey => GetValue<string>(TK_KEY );
		public string SchemaVersion => GetValue<string>(TK_VERSION );
		public string ModelName => GetValue<string>(TK_MODEL_NAME ) ;
		public string ModelPath => GetValue<string>(TK_MODEL_PATH );

		public string Date => GetValue<string>(TK_DATE );

		public virtual dynamic GetValueAs<T>(KEY fieldKey)
		{
			// return Fields[(KEYey) fieldKey].DyValue.Value;
			return Fields[fieldKey].DyValue.GetValueAs<T>();
		}

		public virtual TField GetField(KEY fieldKey)
		{
			TField f = Fields[fieldKey];

			return f;
		}

		public abstract T GetValue<T>(KEY key);

		public virtual void SetValue(KEY fieldKey, dynamic value)
		{
			TField a = Fields[fieldKey].SetValue = value;
		}

		public virtual void Add(TField field)
		{
			Fields.Add(field.FieldKey, field);
		}

		public Dictionary<KEY, TField> CloneFields()
		{
			Dictionary<KEY, TField> copy = new Dictionary<KEY, TField>();

			foreach (KeyValuePair<KEY, TField> kvp in Fields)
			{
				copy.Add(kvp.Key, kvp.Value);
			}

			return null;
		}

		public abstract void ParseEnum(Type t, string enumName);

	}


	// defines a sheet which has a collection of fields and a collection of rows
	public interface IShScRows2<TRowFld, TRow>
		where TRowFld :  IShScFieldBase2, new()
		where TRow : IShScFieldsBase2<TRowFld>, new()
	{
		Dictionary<string, TRow> Rows { get; }

		void AddRow(TRow row);
	}

	// the basic collection of fields
	// either sheet which is a collection of sheet fields which is a class of sub-fields
	// or row which is a collection of row fields which is a class of sub-fields
	// field e.g. [schema name]
	public interface IShScFieldsBase2<TField>
		where TField : IShScFieldBase2, new()
	{
		Dictionary<KEY, TField> Fields { get; }
	
		TField GetField(KEY fieldKey);
	
		dynamic GetValueAs<T>(KEY fieldKey);

		void SetValue(KEY fieldKey, dynamic value);
	
		void Add(TField field);
	}
}

