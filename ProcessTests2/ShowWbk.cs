using System.Text;
using ExStorSys;
using UtilityLibrary;
using static ProcessTests2.ShowWbk;


// user name: jeffs
// created:   4/18/2026 5:18:53 PM

namespace ProcessTests2
{
	public static class ShowWbk
	{
		private const int COL_DN = -28;
		private const int COL_DE = -38;
		private const int COL_MT = -38;
		private const int COL_ST = -22;
		private const int COL_VI = -22;
		private const int COL_LI = -15;
		private const int COL_NC = -18;
		private const int COL_DC = -22;
		private const int COL_NM = -18;
		private const int COL_DM = -22;
		private const int COL_SL = -22;

		private const int COL_SRC  = -12;
		private const int COL_TF   = -2;
		private const int COL_BTN  = -9;

		private const int COL_WFLD_ID	= -4;
		private const int COL_WFLD_SCSID0 = -15;
		private const int COL_WFLD_SCSID1 = -15;
		private const int COL_WFLD_NM	= -14;
		private const int COL_WFLD_VAL	= -34;
		private const int COL_WFLD_CS	= -15;
		private const int COL_WFLD_ST	= -15;
		private const int COL_WFLD_PRIOR_VALUE	= -35;
		private const int COL_WFLD_PRIOR_CHGSRC	= -15;
		

		private static string dn;
		private static string de;
		private static string mt;
		private static string st;
		private static string vi;
		private static string li;
		private static string nc;
		private static string dc;
		private static string nm;
		private static string dm;
		private static string sl;

		private static void setWbkStrings(int type, WorkBook wbk)
		{
			dn = fmtFld(type, wbk.DsNameField, COL_DN);
			de = fmtFld(type, wbk.DescField, COL_DE);
			mt = fmtFld(type, wbk.ModelTitleField, COL_MT);
			st = fmtFld(type, wbk.StatusField, COL_ST);
			vi = fmtFld(type, wbk.VendorIdField, COL_VI);
			li = fmtFld(type, wbk.LastIdField, COL_LI);
			nc = fmtFld(type, wbk.NameCreatedField, COL_NC);
			dc = fmtFld(type, wbk.DateCreatedField, COL_DC);
			nm = fmtFld(type, wbk.NameModifiedField, COL_NM);
			dm = fmtFld(type, wbk.DateModifiedField, COL_DM);
			sl = fmtFld(type, wbk.ShtsListField, COL_SL);
		}

		private static string fmtFld(int type, FieldData<WorkBookFieldKeys> fld, int width)
		{
			string result = "";

			if (type == 0) result = fmtFld0(fld, width);
			if (type == 1) result = fmtFld1(fld, width);
			if (type == 2) result = fmtFld2(fld, width);

			return result;
		}


		// returns "[*] fld name 
		private static string fmtFld0(FieldData<WorkBookFieldKeys> fld, int width)
		{
			string fmt = $"{{0,{width}}}";
			string r = "null";

			DynaValue? d= fld.DyValue;

			if (fld.DyValue != null)
			{
				string id = d.IsDirty ? "*" : " ";

				string s = d.AsString() ?? "null";

				string id1 = $"[{id}]";

				r = $"{id1,-4}{s}";
			}

			return string.Format(fmt, r);

		}

		// returns "[*] fld name [chgsrcId]
		private static string fmtFld1(FieldData<WorkBookFieldKeys> fld, int width)
		{
			string fmt = $"{{0,{width}}}";
			string r = "null";

			DynaValue? d= fld.DyValue;

			string priorValue = fld.DyValue.PriorValue;
			string priorChgSrc = $"[ {fld.DyValue.PriorChgSrc.ToString()} ]";

			string scSID0 = $"[ {fld.Field!.FieldChgSrcId[0].ToString()} ]";
			string scSID1 = $"[ {fld.Field!.FieldChgSrcId[1].ToString()} ]";

			string id = d.IsDirty ? "*" : " ";

			string s = $"( {d.AsString() ?? "null"} )";

			string id1 = $"[{id}]";

			string cs = $"[ {fld.ChgSrc} ]";

			string n = fld.Field.FieldName;

			string st = fld.IsDirty() ? "[ is dirty ]" : "[ is clean ]";

			r = $"{id1,COL_WFLD_ID}  {n,COL_WFLD_NM}  {s,COL_WFLD_VAL}  {cs,COL_WFLD_CS}  {st, COL_WFLD_ST}  {scSID0,COL_WFLD_SCSID0}  {scSID1,COL_WFLD_SCSID1}  {priorValue, COL_WFLD_PRIOR_VALUE}  {priorChgSrc,COL_WFLD_PRIOR_CHGSRC}";

			return r;

		}

		// format [*] [chgsrc] value   
		private static string fmtFld2(FieldData<WorkBookFieldKeys> fld, int width)
		{
			string fmt = $"{{0,{width}}}";
			string r = "null";

			DynaValue? d= fld.DyValue;

			string id = d.IsDirty ? "*" : " ";

			string s = $"( {d.AsString() ?? "null"} )";

			string id1 = $"[{id}]";

			string cs = $"[ {fld.ChgSrc} ]";

			string n = $"{s} {cs}";

			r = $"{cs,-12}{id1,-4}{s,-34}";

			return string.Format(fmt, r);
		}

		private static string fmtWbkString()
		{
			// return $"|{dn}|{de}|{vi}|{li}|{nc}|{dc}|{nm}|{dm}|";
			return $" {dn} {de} {st} {vi} {li} {nm} {dm}";
		}

		private static string fmtWbkString2(out string[] xtraLines)
		{
			xtraLines =
			[
				$" {st} {vi}",
				$" {li} {nm} {dm}",
			];
			
			return $" {dn} {de}";
		}

		public static string ShowHasModArray(bool[] hasMod)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < hasMod.Length; i++)
			{
				sb.Append($"[ {(ChgSrcId) i} ] {getModStatus(hasMod[i])} | ");
			}

			return sb.ToString();
		}

		public static string ShowHasModArray2(string preface, string title, int[] hasMod, int which, int[,] showSrcArr, out string[] result)
		{
			int count;

			result = new string[2];

			StringBuilder sbT = new StringBuilder($"{title}true =  ");
			StringBuilder sbF = new StringBuilder($"{title}false = ");

			for (int i = 0; i < hasMod.Length; i++)
			{
				if (showSrcArr[i, which]<0) continue;

				count = hasMod[i];

				if (hasMod[i] > 0)
				{
					sbT.Append($"| {(ChgSrcId) i} ({count}) ");
				}
				else
				{
					sbF.Append($"| {(ChgSrcId) i} ({count}) ");
				}
			}

			sbT.Append("|");
			sbF.Append("|");

			result[0] = sbT.ToString();
			result[1] = sbF.ToString();

			return $"{preface}{sbT.ToString()}{preface}{sbF.ToString()}";
		}

		public static void ShowChangeStatus(string title)
		{
			ExStorData xd = ExStorData.Instance;
			WorkBook wbk = xd.WorkBook;
			// need to show
			// end status
			// general => the source id
			// date modified => change source id
			// button undo enabled
			// button apply enabled
			// two src fields == desc & status
			// one dest field == lastId
			// sheets list modified

			string btnStat = getWbkButtonStatus(xd.WorkBook);
			// string srcId = xd.sr.ToString();
			string modFldsChgSrcId = xd.WorkBook.DateModifiedField.ChgSrc.ToString();
			string descIsMod = xd.WorkBook.DescField.IsDirty().ToString();
			string statusIsMod = xd.WorkBook.StatusField.IsDirty().ToString();
			string lastIdIsMod =  xd.WorkBook.LastIdField.IsDirty().ToString();
			string shtLstMod = xd.ApplyBtnShtsLstStatus.ToString();

			string t = title.IsVoid() ? "" : $"{title,-20}| ";

			R.WriteLine($"\n{t}CHG STAT| date chg src {modFldsChgSrcId,COL_SRC}");
			R.WriteLine($"{t}CHG STAT| desc mod {descIsMod,COL_TF} | status mod {statusIsMod,COL_TF} | lastid mod {lastIdIsMod,COL_TF} | sht lst mod {shtLstMod,COL_TF}");
			R.WriteLine($"{t}CHG STAT| {getWbkStatus(wbk)}");
			R.WriteLine($"{t}CHG STAT| {getWbkButtonStatus(wbk)}");
			R.WriteLine($"{t}CHG STAT| {getShtLstButtonStatus(xd)}");
		}

		public static void wbkUiStatus(string title = "", bool showShtLstBtns = false)
		{
			ExStorData xd = ExStorData.Instance;
			WorkBook wbk = xd.WorkBook;

			// string wbkStatus = getModStatus(ExStorData.Instance.WorkBook.IsModifiedExo);
			//
			// return $"{title} | wbk is mod [ {wbkStatus} ] | {getWbkButtonStatus(ExStorData.Instance.WorkBook)}";

			string t = title.IsVoid() ? "" : $"{title}";

			R.WriteLine($"{t} | {getWbkStatus(wbk)}");
			R.WriteLine($"{t} | {getWbkButtonStatus(wbk)}");
			if (showShtLstBtns) R.WriteLine($"{t} | {getShtLstButtonStatus(xd)}");
		}

		public static string ChangeStatus(string title = "")
		{
			ExStorData xd = ExStorData.Instance;

			string wbkMod =       getModStatus(xd.WorkBook.IsModifiedExo);
			string descIsMod =    getModStatus(xd.WorkBook.DescField.IsDirty());
			string statusIsMod =  getModStatus(xd.WorkBook.StatusField.IsDirty());
			string lastIdIsMod =  getModStatus(xd.WorkBook.LastIdField.IsDirty());
			string shtLstMod =    getModStatus(xd.ApplyBtnShtsLstStatus);
			string dateModIsMod = getModStatus(xd.WorkBook.DateModifiedField.IsDirty());
			string nameModIsMod = getModStatus(xd.WorkBook.NameModifiedField.IsDirty());

			string descSrc    = $"[ {xd.WorkBook.DescField.ChgSrc.ToString()} ]";
			string statusSrc  = $"[ {xd.WorkBook.StatusField.ChgSrc.ToString()} ]";
			string lastIdSrc  = $"[ {xd.WorkBook.LastIdField.ChgSrc.ToString()} ]";
			string dateModSrc = $"[ {xd.WorkBook.DateModifiedField.ChgSrc.ToString()} ]";
			string nameModSrc = $"[ {xd.WorkBook.NameModifiedField.ChgSrc.ToString()} ]";


			string s1 = $" | desc mod {descIsMod,COL_TF} {descSrc, COL_SRC} | status mod {statusIsMod,COL_TF} {statusSrc, COL_SRC} | lastid mod {lastIdIsMod,COL_TF} {lastIdSrc, COL_SRC} | sht lst mod {shtLstMod,COL_TF} |";
			string s2 = $"{title} | mod date mod {dateModIsMod,COL_TF} {dateModSrc, COL_SRC} | mod name mod {nameModIsMod,COL_TF} {nameModSrc, COL_SRC}";


			return $"{s2}{s1}";
		}

		public static void ShowStatus()
		{
			ExStorData xd = ExStorData.Instance;

			setWbkStrings(0, xd.WorkBook);

			R.WriteLine2("\n");

			string msg = fmtWbkString();

			R.WriteLine2(msg);
			R.WriteLine2(getWbkButtonStatus(xd.WorkBook));
			R.WriteLine2($"{getWbkStatus(xd.WorkBook)} / {getWbkButtonStatus(xd.WorkBook)}");
			R.WriteLine2(getXdataStatus(xd));
		}

		public static void ShowWorkbook()
		{
			WorkBook wbk = ExStorData.Instance.WorkBook;

			setWbkStrings(2, wbk);

			R.Write("\n\tWORKBOOK ");

			string[] msgs;
			string msg = fmtWbkString2(out msgs);

			R.WriteLine2(msg);

			foreach (string s in msgs)
			{
				R.Write("\tWORKBOOK ");
				R.WriteLine2(s);
			}
		}

		public static void ShowWorkbookFields()
		{
			WorkBook wbk = ExStorData.Instance.WorkBook;

			setWbkStrings(1, wbk);

			R.WriteLine("\nWORKBOOK fields\n");
			R.WriteLine($"IsModifiedExo? {wbk.IsModifiedExo}\n");

			R.WriteLine($" |{" dty",COL_WFLD_ID} |{" name",COL_WFLD_NM} |{" value",COL_WFLD_VAL} |{" chg src",COL_WFLD_CS} |{"cln / drty", COL_WFLD_ST} |{" cs[0]",COL_WFLD_SCSID0} |{" cs[1]",COL_WFLD_SCSID1} |{" prior value", COL_WFLD_PRIOR_VALUE} |{" prior chgsrc", COL_WFLD_PRIOR_CHGSRC} |");
			R.WriteLine($" |{dl(-COL_WFLD_ID)} |{dl(-COL_WFLD_NM)} |{dl(-COL_WFLD_VAL)} |{dl(-COL_WFLD_CS)} |{dl(-COL_WFLD_ST)} |{dl(-COL_WFLD_SCSID0)} |{dl(-COL_WFLD_SCSID1)} |{dl(-COL_WFLD_PRIOR_VALUE)} |{dl(-COL_WFLD_PRIOR_CHGSRC)} ");

			R.WriteLine($"  {dn}");
			R.WriteLine($"  {mt}");
			R.WriteLine($"  {nc}");
			R.WriteLine($"  {dc}");
			R.WriteLine($"  {vi}");
			R.WriteLine($"  {de}");
			R.WriteLine($"  {st}");
			R.WriteLine($"  {li}");
			R.WriteLine($"  {nm}");
			R.WriteLine($"  {dm}");
			R.WriteLine($"  {sl}");
			R.NewLine();
		}

		private static string dl(int len)
		{
			return "-".Repeat(len);
		}

		/* private */

		private static string getModStatus(bool mod)
		{
			return mod ? "T" : "F";
		}

		private static string getXdataStatus(ExStorData xd)
		{
			string a = xd.SheetsCount.ToString();
			// string b = xd.NeedsSaving.ToString();

			return $"sht cnt {a}";
		}

		// public static void ShowWbkStatus(WorkBook wbk)
		// {
		// 	setWbkStrings(wbk);
		//
		// 	string btnStat = getWbkButtonStatus(wbk);
		//
		// 	R.WriteLine2($"{btnStat} |",40, fmtWbkString());
		//
		// }

		private static string getWbkStatus(WorkBook wbk)
		{
			string a = wbk.IsModifiedExo ? "true" : "false";
			a = $" is mod = {a, COL_BTN} (IsModifiedExo)";

			return $"{a}";
		}

		// private static string getWbkButtonStatus(WorkBook wbk)
		// {
		// 	string a = wbk.IsModifiedExo ? "true" : "false";
		// 	a = $"is mod = {a,COL_BTN}";
		//
		// 	string b = getWbkButtonStatus(wbk);
		// 	return $"status {a} / {b}";
		// 	
		// }

		private static string getWbkButtonStatus(WorkBook wbk)
		{
			string b = boolStatus(wbk.ApplyBtnStatus);
			string c = boolStatus(wbk.UndoBtnStatus);

			b = $"WORKBOOK buttons | APPLY is [ {b,COL_BTN} ]";
			c = $"UNDO is [ {c,COL_BTN} ]";

			return $"{b} {c}";
		}

		private static string getShtLstButtonStatus(ExStorData xd)
		{
			string b = boolStatus(xd.ApplyBtnShtsLstStatus);
			string c = boolStatus(xd.UndoBtnShtsLstStatus);

			b = $"SHTS LST buttons | APPLY is [ {b,COL_BTN} ]";
			c = $"UNDO is [ {c,COL_BTN} ]";

			return $"{b} {c}";
		}

		private static string boolStatus(bool stat)
		{
			return stat ? "ENABLED" : "DISABLED";
		}

		public static void ShowShtsLst()
		{
			ExStorData xd = ExStorData.Instance;

			R.WriteLine("\nSHEETS LIST\n");

			foreach ((string key, Sheet sht) in xd.Sheets)
			{
				R.WriteLine($"\t{sht.DsName,-30} [ {sht.SheetStatus} ]");
			}
		}
	}
}
