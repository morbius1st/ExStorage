#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShExStorageN.ShSchemaFields.ShScSupport;
using ShStudyN.ShEval;

#endregion

// user name: jeffs
// created:   1/8/2023 9:42:11 PM

namespace ExStoreDev.Windows.Support
{
	public class Tests
	{
		private ShDebugMessages M { get; set; }

		private lox lx = new lox();
		private sox sx = new sox();

		public Tests(ShDebugMessages m)
		{
			M = m;
		}


		public void test1()
		{
			foreach (lxKey3 key in lxKeys3.lxKeyList)
			{
				M.WriteLine($"key| {key.Value}| type| {key.GetType().Name}");
			}
		}


		public void test2()
		{
			lx.Fields = new Dictionary<lxKey3, FD>();
			sx.Fields = new Dictionary<sxKey3, FD>();

			addMembers();

			M.WriteLine($"got value| {lx.getval<string>(lxKeys3.LX_01)}");
			M.WriteLine($"got value| {sx.getval<string>(sxKeys3.SX_01)}");


		}

		public void addMembers()
		{
			lx.Fields.Add(lxKeys3.LX_01, new FD(lxKeys3.LX_01, "name lx 01", "value ab"));
			lx.Fields.Add(lxKeys3.LX_02, new FD(lxKeys3.LX_02, "name lx 02", "value ac"));
			lx.Fields.Add(lxKeys3.LT_01, new FD(lxKeys3.LT_01, "name lt 01", "value ad"));

			// lx.Fields.Add(sxKeys3.SX_01, new FD(lxKeys3.SX_01, "name lt 01"));



			sx.Fields.Add(sxKeys3.SX_01, new FD(sxKeys3.SX_01, "name lx 01", "value 12"));
			sx.Fields.Add(sxKeys3.SX_02, new FD(sxKeys3.SX_02, "name lx 02", "value 13"));
			sx.Fields.Add(sxKeys3.ST_01, new FD(sxKeys3.ST_01, "name lt 01", "value 14"));

		}



		public override string ToString()
		{
			return $"this is {nameof(Tests)}";
		}
	}




}
