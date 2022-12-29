#region + Using Directives

#endregion

// user name: jeffs
// created:   10/9/2022 10:09:56 PM


namespace ShSchemaFields2
{
	public interface IShScFieldBase<TKey> where TKey : Enum
	{
		TKey Key { get; }

		string Name { get;  }
		string Description { get;  }

		DynaValue DyValue { get; }
		dynamic Value { get; }
		dynamic SetValue { set; }
	}

	// needs, as a minimum
	// key, value, name, description, unittype
	public interface IShScFieldData<TKey> : IShScFieldBase<TKey> where TKey : Enum
	{

		IShScFieldMeta<TKey> MetaField { get; }

	}

	public interface IShScFieldMeta<TKey> : IShScFieldBase<TKey> where TKey : Enum
	{

		SchemaFieldDisplayLevel DisplayLevel { get;}

	}
}
