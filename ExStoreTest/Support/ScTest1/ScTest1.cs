#region + Using Directives
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static ExStoreTest.Support.ScTest1.ScConst;

#endregion

// user name: jeffs
// created:   1/2/2023 7:30:15 AM

namespace ExStoreTest.Support.ScTest1
{
	public class ScField : IScField
	{
		public KEY Key { get; }
		public string Name { get; }
		public string Desc { get; }

		public ScField(KEY key, string name, string desc)
		{
			Key = key;
			Name = name;
			Desc = desc;
		}
	}


	public class ScConst : ShScConst
	{
		public ScConst()
		{
			config();
		}

		public static readonly KEY SKK_Dev = new KEY(nameof(SKK_Dev));

		private void config()
		{
			KeysSht.Add(SKK_Dev);
		}
	}

	public class ScMeta1 : ShScMeta1
	{
		private static ScMeta1 instance;

		private ScMeta1()
		{
			config();
		}

		public static ScMeta1 Instance
		{
			get
			{
				if (instance == null) { instance = new ScMeta1();  }

				return instance;
			}
		}


		private void config()
		{
			MetaData.Add(SKK_Dev, new ScField(SKK_Dev, "Dev value", "Dev desc"));
		}

	}


	public class ScTest1
	{
		private ScMeta1 scm;

		public ScTest1()
		{
			scm = ScMeta1.Instance;
		}

		public void testA()
		{
			Msgs.WriteLine("running test1");
			Msgs.WriteLine("list of meta values\n");

			foreach (KeyValuePair<KEY, IScField> kvp in scm.MetaData)
			{
				Msgs.WriteLine($"key| {kvp.Key.Value} | name| {kvp.Value.Name} | desc| {kvp.Value.Desc}");
			}

			Msgs.WriteLine("\nlist of keys\n");

			foreach (KEY key in ScConst.KeysSht)
			{
				Msgs.WriteLine($"key| {key.Value}");
			}
		}

		public override string ToString()
		{
			return $"this is {nameof(ScTest1)}";
		}
	}
}
