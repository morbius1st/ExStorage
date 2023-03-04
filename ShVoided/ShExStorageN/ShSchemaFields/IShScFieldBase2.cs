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

	public interface IShScFieldMeta2: IShScFieldBase2
	{
		SchemaFieldDisplayLevel DisplayLevel { get;}
	}

	public interface IShScFieldData2 : IShScFieldBase2
	{
		IShScFieldMeta2 Meta1Field { get; }
	}

	public interface IShScFieldBase2


	{
	KEY FieldKey { get; }

	string FieldName { get; }
	string FieldDesc { get; }
	DynaValue DyValue { get; }

	dynamic GetValueAs<T>();
	dynamic SetValue { set; }

	IShScFieldBase2 Clone();
	}


}



