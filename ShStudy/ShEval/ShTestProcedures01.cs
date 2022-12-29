#region + Using Directives
// using ShExStorageC.ShSchemaData;

using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevitSupport;
using ShExStorageC.ShExStorage;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaRowKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaSheetKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaLockKey;
using static ShExStorageN.ShSchemaFields.ShScSupport.CellUpdateRules;

#endregion

// user name: jeffs
// created:   10/16/2022 8:46:34 AM

namespace ShStudy.ShEval
{
	public class ShTestProcedures01
	{
		public static int position = 0;
		public static readonly string[] FAUX_FAMILY_NAMES = new [] { "Family1", "Family 2", "Family 3", "family4", "family 5", "family 6" };
		public static readonly string[] FAUX_XLS_PATHS = new [] { "C:\\temp1", "C:\\temp 1", "C:\\temp1", "C:\\temp 1", "C:\\temp1", "C:\\temp1" };
		public static readonly string[] FAUX_WKS_NAMES = new [] { "Sheet1", "Sheet2", "Sheet1", "Sheet3", "Sheet1", "Sheet4"};
		public static readonly string[] FAUX_UPD_SEQ = new [] { "A1", "X2", "D3", "Z4", "B5", "Y6"};

		private string fauxSeq = "abc";

		// private ExStorageLibraryC elc;

		private ShDebugMessages M { get; set; }

		private ShShowProcedures01 sp01;

		public ShTestProcedures01(IWin w, ShDebugMessages msgs)
		{
			M = msgs;

			sp01 = new ShShowProcedures01(msgs);
		}


		// public ScDataSheet1 MakeFauxSheet(ExId exid)
		// {
		// 	ScDataSheet1 shtd = ScData.MakeInitialDataSheet1(exid);
		//
		// 	return shtd;
		// }

		public void MakeFauxRow(ScDataRow rowd)
		{
			string fauxWksPath = Path.GetTempPath();
			string fauxWksName = $"WkSht_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}";
			string fauxFamName = $"Fam_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}";
			string fauxSeq = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()).Substring(0,2);
		
			// row data initially created.  finish with bogus information
			rowd.Fields[RK0_SCHEMA_NAME].SetValue = $"row_schema_{fauxFamName}";

			rowd.Fields[RK2_CELL_FAMILY_NAME].SetValue = fauxFamName;
			rowd.Fields[RK2_XL_FILE_PATH].SetValue = fauxWksPath;
			rowd.Fields[RK2_XL_WORKSHEET_NAME].SetValue = fauxWksName;

			rowd.Fields[RK2_SEQUENCE].SetValue = fauxSeq;
			rowd.Fields[RK2_SKIP].SetValue = false;
			rowd.Fields[RK2_UPDATE_RULE].SetValue = UR_UPON_REQUEST;

		}

		public string makeRowSchemaName(string rootName)
		{
			return $"row_schema_{rootName}";
		}



	/*

		public void TestDynamicValue()
		{
			// ValueDef2 v1s = new ValueDef2("test");
			// ValueDef2 v1e = new ValueDef2(SchemaSheetKey.TK_GUID);

			// string s1 = v1s.AsString();
			// SchemaSheetKey e1 = (SchemaSheetKey) v1e.AsEnum();


			DynaValue v2s = new DynaValue("test");
			DynaValue v2e = new DynaValue(SchemaSheetKey.TK9_GUID);

			string s2 = v2s.AsString();

			SchemaSheetKey e2c = v2s.GetValueAs<SchemaSheetKey>();
			bool e2cb = v2s.LastValueReturnedIsValid;

			string s2c = v2s.GetValueAs<string>();
			bool s2cb = v2s.LastValueReturnedIsValid;

			int i2c = v2s.GetValueAs<int>();
			bool i2cb = v2s.LastValueReturnedIsValid;

			double d2c = v2s.GetValueAs<double>();
			bool d2cb = v2s.LastValueReturnedIsValid;

			bool b2c = v2s.GetValueAs<bool>();
			bool b2cb = v2s.LastValueReturnedIsValid;

			SchemaSheetKey e2 = (SchemaSheetKey) v2e.AsEnum();

			SchemaSheetKey e2b = v2e.GetValueAs<SchemaSheetKey>();
			string s2b = v2e.GetValueAs<string>();
			int i2b = v2e.GetValueAs<int>();
			double d2b = v2e.GetValueAs<double>();
			bool b2b = v2e.GetValueAs<bool>();
		}




	public ScDataSheet1 MakeInitialSheetData(ExId exid)
	{
		M.WriteLine("\nmake SHEET1 data\n");

		ScDataSheet1 sheet = ScInfoMeta1.MakeInitialDataSheet1(exid, RvtCommand.RvtDoc);

		return sheet;
	}

	public void MakeInitialRowData(ExId exid, ScDataSheet1 shtd)
	{
		shtd.AddRow(
			ScInfoMeta1.MakeInitialDataRow1(exid, shtd));
		

	}




	*/
	}
}
