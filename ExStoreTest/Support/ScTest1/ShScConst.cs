#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

// user name: jeffs
// created:   1/2/2023 7:10:28 AM

namespace ExStoreTest.Support.ScTest1
{
	public struct KEY
	{
		public string Value { get; private set; }

		public KEY(string value)
		{
			Value = value;
		}
	}

	public abstract class ShScConst
	{

		public static readonly KEY SKK_Key  = new KEY(nameof(SKK_Key));
		public static readonly KEY SKK_Name = new KEY(nameof(SKK_Name));
		public static readonly KEY SKK_Desc = new KEY(nameof(SKK_Desc));
		public static readonly KEY SKK_Guid = new KEY(nameof(SKK_Guid));


		public static List<KEY> KeysSht = new List<KEY>()
		{
			{ SKK_Key },
			{ SKK_Name },
			{ SKK_Desc },
		};

		public override string ToString()
		{
			return $"this is {nameof(ShScConst)}";
		}
	}
}
