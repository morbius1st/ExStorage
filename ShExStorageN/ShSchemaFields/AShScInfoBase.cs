#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using ShExStorageN.ShSchemaFields;

#endregion

// user name: jeffs
// created:   10/24/2022 7:07:09 PM

namespace ShExStorageN.ShSchemaFields
{
	public abstract class AShScInfoBase<Tkey, Tdata> 
		: IShScInfoBase<Tkey, Tdata> 
		where Tkey : Enum 
		where Tdata : IShScFieldBase<Tkey>, new()
	{

		protected AShScInfoBase()
		{
			Fields = new Dictionary<Tkey, Tdata>();
		}

		public abstract string SchemaName { get; }

		public abstract Dictionary<Tkey, Tdata> Fields { get; protected set; }

		public string ValAsString(Tkey key)
		{
			return Fields[key].DyValue.AsString();
		}

		public void Add(Tdata info)
		{
			Fields.Add(info.Key, info);
		}


	}
}
