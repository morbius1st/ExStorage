#region + Using Directives
// using ShExStorageC.ShSchemaData;

using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShSchemaFields;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaRowKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaTableKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaLockKey;
using static ShExStorageN.ShSchemaFields.ShScSupport.RowUpdateRules;

#endregion

// user name: jeffs
// created:   10/16/2022 8:46:34 AM

namespace ShStudy.ShEval
{
	public class ShTestProcedures01
	{
		private string fauxSeq = "abc";

		// private ShDebugSupport D { get; set; }

		private ShDebugMessages M { get; set; }

		private ShShowProcedures01 sp01;

		public ShTestProcedures01(IWin w, ShDebugMessages msgs)
		{
			M = msgs;

			sp01 = new ShShowProcedures01(w, msgs);
		}

		public void MakeFauxRowData(ScDataRow rowd)
		{
			string fauxWksPath = Path.GetTempPath();
			string fauxWksName = $"WkTbl_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}";
			string fauxFamName = $"Fam_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}";
			string fauxSeq = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()).Substring(0,2);
			
			// row data initially created.  finish with bogus information
			rowd.Fields[CK0_SCHEMA_NAME].SetValue = $"row_schema_{fauxFamName}";

			rowd.Fields[CK9_ROW_FAMILY_NAME].SetValue = fauxFamName;
			rowd.Fields[CK9_XL_FILE_PATH].SetValue = fauxWksPath;
			rowd.Fields[CK9_XL_WORKSHEET_NAME].SetValue = fauxWksName;

			rowd.Fields[CK9_SEQUENCE].SetValue = fauxSeq;
			rowd.Fields[CK9_SKIP].SetValue = false;
			rowd.Fields[CK9_UPDATE_RULE].SetValue = UR_UPON_REQUEST;

		}

		public void TestDynamicValue()
		{
			// ValueDef2 v1s = new ValueDef2("test");
			// ValueDef2 v1e = new ValueDef2(SchemaTableKey.TK_GUID);

			// string s1 = v1s.AsString();
			// SchemaTableKey e1 = (SchemaTableKey) v1e.AsEnum();


			DynaValue v2s = new DynaValue("test");
			DynaValue v2e = new DynaValue(SchemaTableKey.SK9_GUID);

			string s2 = v2s.AsString();

			SchemaTableKey e2c = v2s.ConvertValueTo<SchemaTableKey>();
			bool e2cb = v2s.LastValueReturnedIsValid;

			string s2c = v2s.ConvertValueTo<string>();
			bool s2cb = v2s.LastValueReturnedIsValid;

			int i2c = v2s.ConvertValueTo<int>();
			bool i2cb = v2s.LastValueReturnedIsValid;

			double d2c = v2s.ConvertValueTo<double>();
			bool d2cb = v2s.LastValueReturnedIsValid;

			bool b2c = v2s.ConvertValueTo<bool>();
			bool b2cb = v2s.LastValueReturnedIsValid;

			SchemaTableKey e2 = (SchemaTableKey) v2e.AsEnum();

			SchemaTableKey e2b = v2e.ConvertValueTo<SchemaTableKey>();
			string s2b = v2e.ConvertValueTo<string>();
			int i2b = v2e.ConvertValueTo<int>();
			double d2b = v2e.ConvertValueTo<double>();
			bool b2b = v2e.ConvertValueTo<bool>();
		}


	}
}
