#region + Using Directives
using ShExStorageC.ShSchemaFields.ScSupport;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

// user name: jeffs
// created:   10/23/2022 9:00:35 PM

namespace ShExStorageN.ShSchemaFields
{
	public interface IShScInfoBase<Tkey, Tdata> where Tkey : Enum where Tdata : IShScFieldBase<Tkey>, new()
	{
		Dictionary<Tkey, Tdata> Fields { get; }

		string SchemaName { get; }

		string ValAsString(Tkey key);

	}

	public interface IShScBaseData<Tkey, Tdata, TCkey, TCdata> : IShScInfoBase<Tkey, Tdata>
		where Tkey : Enum 
		where TCkey : Enum 
		where Tdata : ShScFieldDefData<Tkey>, new()
		where TCdata : AShScInfoBase<TCkey, ShScFieldDefData<TCkey>>, new()
	{
		Dictionary<string, TCdata> Rows { get; }
	}

	//public Dictionary<string, ScDataRow> Rows { get; protected set; }
}
