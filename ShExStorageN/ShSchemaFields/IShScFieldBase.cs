#region using
using ShExStorageN.ShSchemaFields.ShScSupport;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ShExStorageN.ShSchemaFields;

#endregion

// username: jeffs
// created:  10/29/2022 3:54:12 PM

namespace ShExStorageN.ShSchemaFields
{

	public interface IShScFieldMeta<TKey> : IShScFieldBase<TKey>
		where TKey : Enum
	{
		SchemaFieldDisplayLevel DisplayLevel { get;}
	}

	public interface IShScFieldData<TKey> : IShScFieldBase<TKey>
		where TKey : Enum
	{
		IShScFieldMeta<TKey> MetaField { get; }
	}

	public interface IShScFieldBase<TKey> where TKey : Enum
	{
		TKey FieldKey { get; }

		string FieldName { get; }
		string FieldDesc { get; }
		DynaValue DyValue { get; }
		dynamic SetValue { set; }

		dynamic GetValueAs<T>();

		IShScFieldBase<TKey> Clone();
	}


}



