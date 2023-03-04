#region + Using Directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStoreTest.Support;
using ExStoreTest.Support.ScTest1;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageR.ShExStorage;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaRowKey;
using static ShExStorageN.ShSchemaFields.ShScSupport.CellUpdateRules;
#endregion

// user name: jeffs
// created:   11/27/2022 4:49:31 PM

namespace ExStoreTest.Windows
{
	public class MainWindowModel
	{

		public ShExStorManagerR<
			ScDataSheet1,
			ScDataRow1,
			ScDataLock1,
			SchemaSheetKey,
			ScFieldDefData1<SchemaSheetKey>,
			SchemaRowKey,
			ScFieldDefData1<SchemaRowKey>,
			SchemaLockKey,
			ScFieldDefData1<SchemaLockKey>
			> smR { get; private set; }


		public MainWindowModel(ExId exid)
		{
			Msgs.WriteLine("Main Window Model Created");

			smR = ShExStorManagerR<
				ScDataSheet1,
				ScDataRow1,
				ScDataLock1,
				SchemaSheetKey,
				ScFieldDefData1<SchemaSheetKey>,
				SchemaRowKey,
				ScFieldDefData1<SchemaRowKey>,
				SchemaLockKey,
				ScFieldDefData1<SchemaLockKey>>.Instance;

			smR.Init(exid);
			smR.StorLibR.smR = smR;
			smR.SchemaLibR.smR = smR;

			smR.Sheet = ScInfoMeta1.MakeInitialDataSheet1(exid);
		}

		public void Test1()
		{
			ScTest1 t1 = new ScTest1();

			t1.testA();
		}

		
		public void DeleteSheet()
		{
			Msgs.WriteLine("\n*** begin process| DELETE SHEET | ***\n");

			smR.DeleteSheet();

			Msgs.WriteLine("\n*** process complete | DELETE SHEET | ***\n");

			Msgs.WriteLine("\n*** begin | LIST ALL SCHEMAS | ***\n");

			IList<Schema> schemas;

			smR.StorLibR.GetAllSchema(out schemas);

			Msgs.NewLine();

			foreach (Schema s in schemas)
			{
				Msgs.WriteLine("schema| ", s.SchemaName);
			}
			Msgs.WriteLine("\n*** complete | LIST ALL SCHEMAS | ***\n");


		}

		public void WriteSheet()
		{
			bool result;
			ScDataSheet1 shtd = ScInfoMeta1.MakeInitialDataSheet1(smR.Exid);

			TestMakeDataRow3(smR.Exid, shtd);

			Msgs.WriteLine("\n*** begin process| WRITE SHEET | ***\n");

			smR.Sheet = shtd;

			result = smR.WriteSheet();

			if (result)
			{
				Msgs.WriteLine("\n*** WRITE SHEET worked ***\n");
			}
			else
			{
				Msgs.WriteLine($"\n*** WRITE SHEET failed ***\n");
			}
		}

		
		private void TestMakeDataRow3(ExId exid, ScDataSheet1 shtd)
		{
			// M.WriteLine("test make ROW data x3");
			ScDataRow1 rowd;

			rowd = CreateFauxRow(exid, ScInfoMeta1.MakeInitialDataRow1(shtd));
			shtd.AddRow(rowd);

			rowd = CreateFauxRow(exid, ScInfoMeta1.MakeInitialDataRow1(shtd));
			shtd.AddRow(rowd);

			rowd = CreateFauxRow(exid, ScInfoMeta1.MakeInitialDataRow1(shtd));
			shtd.AddRow(rowd);
		}

		private ScDataRow1 CreateFauxRow(ExId exid, ScDataRow1 row)
		{
			MakeFauxRow(exid, row);

			return row;
		}

		private static int fauxRowIdx = 0;

		private void MakeFauxRow(ExId exid, ScDataRow1 rowd)
		{
			string fauxIdx = (fauxRowIdx++ * 11).ToString("000");
			string fauxWksPath = Path.GetTempPath();
			string fauxWksName = $"{fauxIdx}_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}";
			string fauxFamName = $"{fauxIdx}_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}";
			string fauxSeq = $"{fauxIdx}_{Path.GetFileNameWithoutExtension(Path.GetRandomFileName()).Substring(0, 2)}";

			// row data initially created.  finish with bogus information
			rowd.Fields[RK0_SCHEMA_NAME].SetValue = exid.ExIdRowSchemaName(fauxFamName);

			rowd.Fields[RK2_CELL_FAMILY_NAME].SetValue = fauxFamName;
			rowd.Fields[RK2_XL_FILE_PATH].SetValue = fauxWksPath;
			rowd.Fields[RK2_XL_WORKSHEET_NAME].SetValue = fauxWksName;

			rowd.Fields[RK2_SEQUENCE].SetValue = fauxSeq;
			rowd.Fields[RK2_SKIP].SetValue = false;
			rowd.Fields[RK2_UPDATE_RULE].SetValue = UR_UPON_REQUEST;

		}


		public override string ToString()
		{
			return $"this is {nameof(MainWindowModel)}";
		}
	}
}
