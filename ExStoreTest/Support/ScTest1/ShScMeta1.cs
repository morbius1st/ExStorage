#region + Using Directives
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExStoreTest.Support.ScTest1.ShScConst;
#endregion

// user name: jeffs
// created:   1/2/2023 7:14:47 AM

namespace ExStoreTest.Support.ScTest1
{
	public interface IScField
	{
		KEY Key { get; }
		string Name { get; }
		string Desc { get; }

	}

	public abstract class ShScMeta1
	{
		public Dictionary<KEY, IScField> MetaData { get; } = new Dictionary<KEY, IScField>()
		{
			{ SKK_Key,  new ScField(SKK_Key, "Key value", "Key desc")  },
			{ SKK_Name, new ScField(SKK_Name,"Name value", "Name desc")  },
			{ SKK_Desc, new ScField(SKK_Desc,"Desc value", "Desc desc")  },

		} ;

		public override string ToString()
		{
			return $"this is {nameof(ShScMeta1)}";
		}
	}
}
