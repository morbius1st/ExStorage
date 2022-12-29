#region + Using Directives
using System;
using System.Collections.Generic;
using ShExStorageN.ShSchemaFields.ShScSupport;
using static ShStudy.ShEval.ShFieldDisplayData;
#endregion

// user name: jeffs
// created:   10/11/2022 9:28:39 PM

namespace ShStudy.ShEval
{
	public class ColData
	{
		public string ColTitle { get; set; }
		public int ColWidth { get; set; }
		public int TitleWidth { get; set; }

		public JustifyHoriz[] Just { get; set; }  = new JustifyHoriz[2]; // 0 == values, 1 == header

		public ColData(string colTitle, 
			int colWidth, int titleWidth, 
			JustifyHoriz hj, JustifyHoriz vj)
		{
			ColTitle = colTitle;
			ColWidth = colWidth;
			TitleWidth = titleWidth;
			Just[0] = hj;
			Just[1] = vj;
		}


		public static Dictionary<TE, string[]> Vz<TE>(params Tuple<TE, string, string, string>[] p)
		{
			Dictionary<TE, string[]> vz = null;

			if (p.Length > 0)
			{
				vz = new Dictionary<TE, string[]>();

				for (int i = 0; i < p.Length; i++)
				{
					vz.Add(p[i].Item1, new [] { p[i].Item2, p[i].Item3, p[i].Item4 });
				}
			}

			return vz;
		}

		public static Dictionary<TE, ColData>
			Mz<TE>(params Tuple<TE, string, int, int, JustifyHoriz, JustifyHoriz>[] p)  where TE : System.Enum
		{
			Dictionary<TE, ColData> cd = new Dictionary<TE, ColData>(5);

			if (p.Length > 0)
			{
				for (int i = 0; i < p.Length; i++)
				{
					cd.Add(p[i].Item1, new ColData(
						p[i].Item2,		// column title
						p[i].Item3,		// column width
						p[i].Item4,     // title width
						p[i].Item5,     // header justify
						p[i].Item6      // value justify
						));
				}
			}

			return cd;
		}

		public static Tuple<ScFieldColumns, string, int, int, JustifyHoriz, JustifyHoriz> 
			Tz(ScFieldColumns e, string colTitle,
			JustifyHoriz hdrJust, int colWidth, 
			JustifyHoriz valJust= JustifyHoriz.LEFT, int titleWidth = -1)

		{
			return new Tuple<ScFieldColumns, string, int, int, JustifyHoriz, JustifyHoriz>(e, colTitle,
				colWidth, titleWidth < 1 ? colWidth-2 : titleWidth, hdrJust, valJust);

		}
	}
}