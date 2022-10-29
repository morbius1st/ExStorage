#region + Using Directives
using Autodesk.Revit.DB;

using System;
using System.Dynamic;

#endregion

// user name: jeffs
// created:   10/9/2022 10:09:56 PM


namespace ShExStorageN.ShSchemaFields
{
	public interface IScBase<TKey> where TKey : Enum
	{
		TKey Key { get; }
		DynaValue DyValue { get; }
		dynamic Value { get; }
		dynamic SetValue { set; }
	}

	// needs, as a minimum
	// key, value, name, description, unittype
	public interface IScData<TKey> : IScBase<TKey> where TKey : Enum
	{
		IScField<TKey> Field { get; }

		IScData<TKey> Create(IScField<TKey> field);

		// IScData<TKey> Ctor(TKey key, DynaValue value, IScField<TKey> field);
	}

	public interface IScField<TKey> : IScBase<TKey> where TKey : Enum
	{
		string Name { get;  }
		string Description { get;  }
		SchemaFieldDisplayLevel DisplayLevel { get;}

		
	}
}
