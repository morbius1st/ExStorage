using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExStorSys;
using UtilityLibrary;


// user name: jeffs
// created:   4/22/2026 11:11:34 PM

namespace ProcessTests1
{
	public static class ShowSht
	{
		private const int COL_DN = -30;
		private const int COL_DE = -38;

		private const int COL_NC = -18;
		private const int COL_DC = -26;
		private const int COL_NM = -18;
		private const int COL_DM = -22;

		private const int COL_SV = -12;
		private const int COL_XF = -18;
		private const int COL_XS = -18;
		private const int COL_ST = -12;
		private const int COL_SQ = -12;
		private const int COL_UR = -18;
		private const int COL_US = -12;
		private const int COL_FL = -32;

		private const int COL_SRC  = -12;
		private const int COL_TF   = -2;
		private const int COL_BTN  = -9;

		private const int COL_WFLD_ID   = -4;
		private const int COL_WFLD_SCSID0 = -15;
		private const int COL_WFLD_SCSID1 = -15;
		private const int COL_WFLD_NM   = -14;
		private const int COL_WFLD_VAL  = -34;
		private const int COL_WFLD_CS   = -15;
		private const int COL_WFLD_ST   = -15;


		#pragma warning disable CS8618
		private static string dn; // name
		private static string de; // desc
		private static string nc; // name created
		private static string dc; // date created
		private static string nm; // modified name
		private static string dm; // date modified

		private static string sv; // schema version
		private static string xf; // xl file name
		private static string xs; // xl sheet name
		private static string st; // status
		private static string sq; // sequence
		private static string ur; // update rule
		private static string us; // update skip

		private static string fl; // family list
		#pragma warning restore CS8618

		private static void setShtStrings(int type, Sheet sht)
		{
			dn = fmtFld(type, sht.DsNameField       , COL_DN);
			de = fmtFld(type, sht.DescField         , COL_DE);
			nc = fmtFld(type, sht.NameCreatedField  , COL_NC);
			dc = fmtFld(type, sht.DateCreatedField  , COL_DC);
			nm = fmtFld(type, sht.NameModifiedField , COL_NM);
			dm = fmtFld(type, sht.DateModifiedField , COL_DM);

			sv = fmtFld(type, sht.SchemaVersionField, COL_SV);
			xf = fmtFld(type, sht.XlFilePathField   , COL_XF);
			xs = fmtFld(type, sht.XlSheetNameField  , COL_XS);
			st = fmtFld(type, sht.OpStatusField     , COL_ST);
			sq = fmtFld(type, sht.OpSequenceField   , COL_SQ);
			ur = fmtFld(type, sht.UpdateRuleField   , COL_UR);
			us = fmtFld(type, sht.UpdateSkipField   , COL_US);
			fl = fmtFld(type, sht.FamilyListField   , COL_FL);
		}

		private static string fmtFld(int type, FieldData<SheetFieldKeys> fld, int width)
		{
			string result = "";

			if (type == 0) result = fmtFld0(fld, width);
			if (type == 1) result = fmtFld1(fld, width);

			return result;
		}

		// returns "[*] fld name 
		private static string fmtFld0(FieldData<SheetFieldKeys> fld, int width)
		{
			string fmt = $"{{0,{width}}}";
			string r = "null";

			DynaValue? d = fld.DyValue;

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
		private static string fmtFld1(FieldData<SheetFieldKeys> fld, int width)
		{
			string fmt = $"{{0,{width}}}";
			string r = "null";

			DynaValue? d = fld.DyValue;

			string sCsid0 = $"[ {fld.Field!.FieldChgSrcId[0].ToString()} ]";
			string sCsid1 = $"[ {fld.Field!.FieldChgSrcId[1].ToString()} ]";

			string id = d.IsDirty ? "*" : " ";

			string s = $"( {d.AsString() ?? "null"} )";

			string id1 = $"[{id}]";

			string cs = $"[ {fld.ChgSrc} ]";

			string n = fld.Field.FieldName;

			string st = fld.IsDirty() ? "[ is dirty ]" : "[ is clean ]";

			r = $"{id1,COL_WFLD_ID}  {n,COL_WFLD_NM}  {s,COL_WFLD_VAL}  {cs,COL_WFLD_CS}  {st,COL_WFLD_ST}  {sCsid0,COL_WFLD_SCSID0}  {sCsid1,COL_WFLD_SCSID1}";

			return string.Format(fmt, r);
		}

		public static void shtUiStatus(string title = "")
		{
			ExStorData xd = ExStorData.Instance;
			Sheet sht = xd.CurrentSheet!;

			string t = title.IsVoid() ? "" : $"{title}";

			R.WriteLine($"{t} | {getShtStatus(sht)}");
			R.WriteLine($"{t} | {getShtButtonStatus(sht)}");
			R.WriteLine($"{t} | {getFamLstButtonStatus(sht)}");
		}

		public static void ShowSheet()
		{
			Sheet sht = ExStorData.Instance.CurrentSheet!;

			setShtStrings(0, sht);

			R.WriteLine2($"\n\tSHEET        |{dn}|{de}|{nm}|{dm}|");
			R.WriteLine2($"\tSHEET        |{xf}|{xs}|{sq}|{ur}|{fl}|");
		}

		public static void ShowFamList()
		{
			Sheet sht = ExStorData.Instance.CurrentSheet!;

			R.WriteLine("\nFAM AND TYPE LIST\n");

			R.WriteLine($"\tlist status | is modified? {sht.IsModifiedFamList}\n");

			foreach ((string key, FamAndType fat) in sht.FamListWkg)
			{
				R.WriteLine($"\tkey {key, -15} | {fat.FamName, -10} | {fat.TypeName, -10} | mod? {fat.IsModifiedFat, -6} | new? {fat.IsNewItemFat, -6}");
			}

			R.NewLine();
		}

		public static void ShowSheetFields()
		{
			Sheet sht = ExStorData.Instance.CurrentSheet!;

			setShtStrings(1, sht);

			R.WriteLine($"\nSHEET fields | for [ {sht.DsName} ]\n");

			R.WriteLine($" |{" dty",COL_WFLD_ID} |{" name",COL_WFLD_NM} |{" value",COL_WFLD_VAL} |{" chg si",COL_WFLD_CS} |{"cln / drty",COL_WFLD_ST} |{" cs[0]",COL_WFLD_SCSID0} |{" cs[1]",COL_WFLD_SCSID1}");
			R.WriteLine($" |{dl(-COL_WFLD_ID)} |{dl(-COL_WFLD_NM)} |{dl(-COL_WFLD_VAL)} |{dl(-COL_WFLD_CS)} |{dl(-COL_WFLD_ST)} |{dl(-COL_WFLD_SCSID0)} |{dl(-COL_WFLD_SCSID1)} ");

			R.WriteLine($"  {dn}");
			R.WriteLine($"  {nc}");
			R.WriteLine($"  {dc}");

			R.WriteLine($"  {sv}");

			R.WriteLine($"  {de}");
			R.WriteLine($"  {xf}");
			R.WriteLine($"  {xs}");
			R.WriteLine($"  {st}");
			R.WriteLine($"  {sq}");
			R.WriteLine($"  {ur}");
			R.WriteLine($"  {us}");

			R.WriteLine($"  {nm}");
			R.WriteLine($"  {dm}");
			R.WriteLine($"  {fl}");

			R.NewLine();
			R.NewLine();
		}

		private static string dl(int len)
		{
			return "-".Repeat(len);
		}

		private static string fmtShtString()
		{
			// return $"|{dn}|{de}|{vi}|{li}|{nc}|{dc}|{nm}|{dm}|";
			return $"|{dn}|{de}|{nm}|{dm}|{xf}|{xs}|{sq}|{ur}|{fl}|";
		}

		public static string ChangeStatus(string title = "")
		{
			ExStorData xd = ExStorData.Instance;

			string descIsMod    = getModStatus(xd.CurrentSheet!.DescField.IsDirty());
			string statusIsMod  = getModStatus(xd.CurrentSheet!.OpStatusField.IsDirty());
			string seqIdIsMod   = getModStatus(xd.CurrentSheet!.OpSequenceField.IsDirty());
			string dateModIsMod = getModStatus(xd.CurrentSheet!.DateModifiedField.IsDirty());
			string nameModIsMod = getModStatus(xd.CurrentSheet!.NameModifiedField.IsDirty());
			string famLstIsMod  = getModStatus(xd.CurrentSheet!.FamilyListField.IsDirty());

			string descSrc    = $"[ {xd.CurrentSheet!.DescField.ChgSrc.ToString()} ]";
			string statusSrc  = $"[ {xd.CurrentSheet!.OpStatusField.ChgSrc.ToString()} ]";
			string seqSrc   = $"[ {xd.CurrentSheet!.OpSequenceField.ChgSrc.ToString()} ]";
			string dateModSrc = $"[ {xd.CurrentSheet!.DateModifiedField.ChgSrc.ToString()} ]";
			string nameModSrc = $"[ {xd.CurrentSheet!.NameModifiedField.ChgSrc.ToString()} ]";
			string famLstSrc =  $"[ {xd.CurrentSheet!.FamilyListField.ChgSrc.ToString()} ]";

			string s1 = $"{title} | desc mod ({descIsMod,COL_TF}) {descSrc, COL_SRC} | status mod ({statusIsMod,COL_TF}) {statusSrc, COL_SRC} | seq mod ({seqIdIsMod,COL_TF}) {seqSrc, COL_SRC} | fam lst mod ({famLstIsMod}) {famLstSrc}|";
			string s2 = $"{title} | mod date mod ({dateModIsMod,COL_TF}) {dateModSrc, COL_SRC} | mod name mod ({nameModIsMod,COL_TF}) {nameModSrc, COL_SRC}";

			return $"{s2}\n{s1}";
		}

		public static void ShowShtsLst(ObservableDictionary<string, Sheet> sheetsList)
		{
			ShowWbk.ShowShtsLst();
		}

		public static void shtUiStatus(string title = "", bool showShtLstBtns = false)
		{
			ExStorData xd = ExStorData.Instance;
			Sheet sht = xd.CurrentSheet!;

			string t = title.IsVoid() ? "" : $"{title}";

			R.WriteLine($"{t} | {getShtStatus(sht)}");
			R.WriteLine($"{t} | {getShtButtonStatus(sht)}");
			if (showShtLstBtns) R.WriteLine($"{t} | {getFamLstButtonStatus(sht)}");
		}


		private static string getModStatus(bool mod)
		{
			return mod ? "T" : "F";
		}

		private static string getShtStatus(Sheet sht)
		{
			string a = sht.IsModifiedExo ? "true" : "false";
			a = $"is mod = {a,COL_BTN}";

			return $"{a}";
		}

		private static string getShtButtonStatus(Sheet sht)
		{
			string b = boolStatus(sht.ApplyBtnStatus);
			string c = boolStatus(sht.UndoBtnStatus);

			b = $"SHEET buttons | APPLY is {b,COL_BTN}";
			c = $"UNDO is {c,COL_BTN}";

			return $"{b} {c}";
		}

		private static string getFamLstButtonStatus(Sheet sht)
		{
			string b;
			string c = boolStatus(sht.IsModifiedFamList);

			b = "FAMILY LST button |";
			c = $"UNDO is {c,COL_BTN}";

			return $"{b} {c}";
		}

		private static string boolStatus(bool stat)
		{
			return stat ? "ENABLED" : "DISABLED";
		}
	}
}