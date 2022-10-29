#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShSchemaFields;
#endregion

// user name: jeffs
// created:   10/29/2022 8:13:12 AM

namespace ShExStorageC.ShSchemaFields
{
	// example:
	// IScDataWorkBook<
	// SchemaTableKey,
	// ScDataTable   (<SchemaTableKey, ShScFieldDefData<SchemaTableKey>>,)
	// ShScFieldDefData<SchemaTableKey>


	public interface IScDataWorkBook<Tkey, TTableData, TFieldData>
	where Tkey : Enum
	// expected actual is 
	where TTableData : IShScInfoBase<Tkey, TFieldData>, new()
	where TFieldData : IShScFieldData<Tkey>, new()
	{

		Dictionary<string, TTableData> Tables { get; }


	}
}
