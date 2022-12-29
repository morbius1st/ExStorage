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
	public interface IShScFieldBase1<TFldId> where TFldId : Enum
	{
		TFldId FieldKey { get; }

		string FieldName { get; }
		string FieldDesc { get; }
		DynaValue DyValue { get; }

		dynamic GetValueAs<T>();
		dynamic SetValue { set; }

		IShScFieldBase1<TFldId> Clone();
	}

	public interface IShScFieldData1<TFldId> : IShScFieldBase1<TFldId>
		where TFldId : Enum
	{
		IShScFieldMeta1<TFldId> Meta1Field { get; }
	}

	public interface IShScFieldMeta1<TFldId> : IShScFieldBase1<TFldId>
		where TFldId : Enum
	{
		SchemaFieldDisplayLevel DisplayLevel { get;}
	}
}
