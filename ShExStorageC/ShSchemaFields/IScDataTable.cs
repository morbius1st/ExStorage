#region + Using Directives
using ShExStorageN.ShSchemaFields;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

// user name: jeffs
// created:   10/29/2022 8:35:18 AM

namespace ShExStorageC.ShSchemaFields
{
	public interface IScDataTable<Tkey, Tdata>
	where Tkey : Enum
	// expected data is: IShScFieldData<Tkey>
	where Tdata : IShScFieldData<Tkey>, new()
	{
	}
}
