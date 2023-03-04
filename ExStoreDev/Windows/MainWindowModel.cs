#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using ExStoreDev.Windows.Support;
using RevitSupport;
using ShExStorageC.ShExStorage;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
using ShExStorageN.ShSchemaFields.ShScSupport;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaRowKey;
using static ShExStorageC.ShSchemaFields.ShScSupport.SchemaSheetKey;
using ShStudyN.ShEval;

using static ExStoreDev.Windows.MainWindowModel;
using static UtilityLibrary.CsUtilities;
#endregion

// username: jeffs
// created:  10/16/2022 9:03:08 AM

namespace ExStoreDev.Windows
{
	public class MainWindowModel
	{
	#region private fields

		private ShDebugMessages M { get; set; }


		private MainWindow mw;

		private ShTestProcedures01 tp01;
		private ShShowProcedures01 sp01;

		private ShowLibrary sl;

		private ShFieldDisplayData shFd;

		// private ExStorageLibraryC xlC;


		private
			ExStorageLibraryC1<
			ScDataSheet,
			ScDataRow,
			ScDataLock,
			SchemaSheetKey,
			ScFieldDefData<SchemaSheetKey>,
			SchemaRowKey,
			ScFieldDefData<SchemaRowKey>,
			SchemaLockKey,
			ScFieldDefData<SchemaLockKey>
			> elc1;


		private Tests t3;

	#endregion

	#region ctor

		public MainWindowModel(MainWindow w)
		{
			mw = w;

			M = MainWindow.M;

			sl = new ShowLibrary(M);
			shFd = new ShFieldDisplayData();

			tp01 = new ShTestProcedures01(w, M);
			sp01 = new ShShowProcedures01(M);

			t3 = new Tests(M);


			elc1 = new ExStorageLibraryC1<
				ScDataSheet,
				ScDataRow,
				ScDataLock,
				SchemaSheetKey,
				ScFieldDefData<SchemaSheetKey>,
				SchemaRowKey,
				ScFieldDefData<SchemaRowKey>,
				SchemaLockKey,
				ScFieldDefData<SchemaLockKey>>(M);
		}

	#endregion

	#region public methods

		private ICollectionView[] vw;

		public void TestCollectionViews(ScDataSheet shtd)
		{
			M.WriteLineAligned("Test Collection Views");

			if (shtd.Rows.Count == 0)
			{
				M.WriteLineAligned("No rows to count");
				return;
			}

			vw = new ICollectionView[3];

			vw[0] = CollectionViewSource.GetDefaultView(shtd.Rows);
			vw[0].SortDescriptions.Add(
				new SortDescription("Key", ListSortDirection.Ascending));


			vw[1] = new CollectionViewSource { Source = shtd.Rows }.View;
			vw[1].Filter = item =>
			{
				KeyValuePair<string, ScDataRow> kvp = (KeyValuePair<string, ScDataRow>) item ;
				M.WriteDebugMsgLine($"filtering| ", kvp.Value.SchemaName);

				
				return kvp.Value.SchemaName.Contains("0");
			};

			int a = 1;
		}


		// public void TestBegin(ShtExId exid, ScDataSheet shtd)
		// {
		// 	ShowExid(exid);
		//
		// 	shtd = TestMakeDataSheetInitial(exid);
		// }

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(MainWindowModel)}";
		}

	#endregion


	#region show

		public void ShowExid(ShtExId exid)
		{
			M.MarginUp();
			sp01.ShowExid(exid);
			M.MarginDn();
		}

	#endregion


	#region methods

		public void TestClone()
		{

		}

		public void ShowExid2()
		{
			M.MarginUp();
			sp01.ShowExid(new ShtExId("Name", ShtExId.ROOT));
			M.MarginDn();
		}

		public void TestDynaValue()
		{
			// testDynamicValue();
			sp01.ShowDynaValue();
		}

		public void TestFieldsLock()
		{
			sp01.ShowLockFields();
		}

		public void TestFieldsSheet()
		{
			sp01.ShowSheetFields();
		}

		public void TestFieldsRow()
		{
			sp01.ShowRowFields();
		}

		public ScDataSheet TestMakeDataSheetInitial(ShtExId exid)
		{
			M.WriteLine("test make SHEET data - initial");

			ScDataSheet shtd = ScData.MakeInitialDataSheet1(exid);
			shtd.HasData = true;

			return shtd;
		}
		//
		// public ScDataSheet TestMakeDataSheetEmpty()
		// {
		// 	M.WriteLine("test make SHEET data - initial");
		//
		// 	// ScDataSheet1 shtd = ScInfoMeta1.MakeInitialDataSheet1(exid);
		//
		// 	ScDataSheet shtd = xlC.MakeEmptySheet();
		//
		// 	// ScDataSheet1 shtd = new ScDataSheet1();
		//
		// 	return shtd;
		// }

		public void TestMakeDataRow3(ShtExId exid, ScDataSheet shtd)
		{
			// M.WriteLine("test make ROW data x3");
			ScDataRow rowd;

			rowd = CreateFauxRow(TestMakeDataRow(exid, shtd));
			shtd.AddRow(rowd);

			rowd = CreateFauxRow(TestMakeDataRow(exid, shtd));
			shtd.AddRow(rowd);

			rowd = CreateFauxRow(TestMakeDataRow(exid, shtd));
			shtd.AddRow(rowd);
		}

		public ScDataRow TestMakeDataRow(ShtExId exid, ScDataSheet shtd)
		{
			// M.WriteLine("\ntest make ROW data\n");

			// make the initial set of data elements

			return ScData.MakeInitialDataRow1(shtd);
		}

		// public ScDataLock2 TestMakeDataLock(LokExId exid)
		// {
		// 	M.WriteLine("\ntest make LOCK data\n");
		//
		// 	// ScDataLock2 lokd = ScData.MakeInitialDataLock1(exid);
		//
		// 	return lokd;
		// }

		public void ShowSheetData(ScDataSheet shtd)
		{
			M.MarginUp();

			sp01.ShowSheetDataGeneric(shtd);

			M.NewLine();
			M.NewLine();

			sp01.ShowSheetFieldsGeneric(shtd);

			M.MarginDn();
		}

		public void ShowLockData(ScDataLock lokd)
		{
			M.MarginUp();

			sp01.ShowLockDataGeneric(lokd);
			M.MarginDn();
		}

		// public void ShowLockData(ScDataLock2 lokd)
		// {
		// 	M.MarginUp();
		//
		// 	// sp01.ShowLockDataGeneric(lokd);
		// 	M.MarginDn();
		// }

		public ScDataRow CreateFauxRow(ScDataRow row)
		{
			tp01.MakeFauxRow(row);

			return row;
		}

		public bool TestClone(ScDataSheet shtd)
		{
			// TestMakeNewSheet();

			if (shtd == null || !shtd.HasData) return false;

			shtd.SetValue(SK0_DESCRIPTION, "this is a new description");

			ListSheetData(shtd);

			return false;

			ScDataSheet copy = (ScDataSheet) shtd.Clone();

			ShowSheetData(shtd);

			return true;

		}

		public void TestCloneModifyData(ScDataSheet shtd)
		{
			shtd.SetValue(SK0_SCHEMA_NAME, "asdf");

		}

		public void TestMakeNewSheet()
		{
			ScDataSheet shtd = new ScDataSheet();

			ShowSheetData(shtd);

		}


		public void ListSheetData(ScDataSheet shtd)
		{
			foreach (KeyValuePair<SchemaSheetKey, ScFieldDefData<SchemaSheetKey>> kvp in shtd)
			{
				M.WriteLine($"key| {kvp.Key.ToString()}", $"{kvp.Value.FieldName} :: {kvp.Value.DyValue.Value}");
			}
		}


	#endregion

	#region schema processes

		public void CreateSheet(ShtExId exid, ScDataSheet shtd)
		{
			M.WriteLine("\n*** begin process| CREATE SHEET | ***\n");

			ExStoreRtnCode result = ExStoreRtnCode.XRC_GOOD;

			// result = elc.WriteSheetToDataStorage(exid, shtd);

			// todo: fix this
			// elc1.SheetData = shtd;

			// elc1.Exid = exid;

			elc1.SaveSheet();

			M.WriteLine($"result is | {result}");
		}

	#endregion

		public void MakeDataGeneric()
		{
			MakeSheetDataGeneric
				<ScDataSheet,
				ScDataRow,
				ScDataLock,
				SchemaSheetKey,
				ScFieldDefData<SchemaSheetKey>,
				SchemaRowKey,
				ScFieldDefData<SchemaRowKey>,
				SchemaLockKey,
				ScFieldDefData<SchemaLockKey>
				>
				();
		}

		public void MakeSheetDataGeneric<TSht, TRow, TLok, TShtKey, TShtFlds, TRowKey, TRowFlds, TLokKey, TLokFlds> ()
			where TSht : AShScSheet<TShtKey, TShtFlds, TRowKey, TRowFlds, TSht, TRow>, new()
			where TRow : AShScRow<TRowKey, TRowFlds>, new()
			where TLok : AShScLock<TLokKey, TLokFlds>, new()
			where TShtKey : Enum
			where TShtFlds : ScFieldDefData<TShtKey>, new()
			where TRowKey : Enum
			where TRowFlds : ScFieldDefData<TRowKey>, new()
			where TLokKey : Enum
			where TLokFlds : ScFieldDefData<TLokKey>, new()
		{
			TSht sht = new TSht();
			TRow row = new TRow();
			TLok lok = new TLok();
		}

		public void KeysTests()
		{
			// t3.test1();
			t3.test2();
		}



	}


}