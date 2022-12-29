#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using RevitSupport;
using ShExStorageC.ShExStorage;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaRowKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaSheetKey;
using ShStudy.ShEval;

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

		private ExStorageLibraryC xlC;


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

			// elc = new ExStorageLibraryC(M);

			xlC = new ExStorageLibraryC();

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
				M.WriteDebugMsgLine($"filtering| ", kvp.Value.Fields[RK0_SCHEMA_NAME].DyValue.AsString());
				return kvp.Value.Fields[RK0_SCHEMA_NAME].DyValue.AsString().Contains("0");
			};

			int a = 1;
		}


		public void TestBegin(ShtExId exid, ScDataSheet shtd)
		{
			ShowExid(exid);

			shtd = TestMakeDataSheetInitial(exid);
		}

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


	#region keep methods

		public void ShowExid2()
		{
			M.MarginUp();
			sp01.ShowExid(new ShtExId("Name"));
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

			// ScDataSheet1 shtd = ScInfoMeta1.MakeInitialDataSheet1(exid);

			ScDataSheet shtd = xlC.MakeInitSheet(exid);

			// ScDataSheet1 shtd = new ScDataSheet1();

			return shtd;
		}

		public ScDataSheet TestMakeDataSheetEmpty()
		{
			M.WriteLine("test make SHEET data - initial");

			// ScDataSheet1 shtd = ScInfoMeta1.MakeInitialDataSheet1(exid);

			ScDataSheet shtd = xlC.MakeEmptySheet();

			// ScDataSheet1 shtd = new ScDataSheet1();

			return shtd;
		}

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

		public ScDataLock TestMakeDataLock(LokExId exid)
		{
			M.WriteLine("\ntest make LOCK data\n");

			ScDataLock lokd = ScData.MakeInitialDataLock1(exid);

			return lokd;
		}

		public void ShowSheetData(ScDataSheet shtd)
		{
			M.MarginUp();
			// sp01.ShowSheetData(shtd);
			sp01.ShowSheetDataGeneric(shtd);

			M.NewLine();
			M.NewLine();

			sp01.ShowSheetFieldsGeneric(shtd);

			M.MarginDn();
		}

		public void ShowLockData(ScDataLock lokd)
		{
			M.MarginUp();
			// sp01.ShowSheetData(shtd);
			sp01.ShowLockDataGeneric(lokd);
			M.MarginDn();
		}

		public ScDataRow CreateFauxRow(ScDataRow row)
		{
			tp01.MakeFauxRow(row);

			return row;
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
			where TSht : AShScSheet<TShtKey, TShtFlds, TRowKey, TRowFlds, TRow>, new()
			where TRow : AShScRow<TRowKey, TRowFlds>, new()
			where TLok : AShScFields<TLokKey, TLokFlds>, new()
			where TShtKey : Enum
			where TShtFlds : IShScFieldData1<TShtKey>, new()
			where TRowKey : Enum
			where TRowFlds : IShScFieldData1<TRowKey>, new()
			where TLokKey : Enum
			where TLokFlds : IShScFieldData1<TLokKey>, new()
		{
			TSht sht = new TSht();
			TRow row = new TRow();
			TLok lok = new TLok();
		}
	}
}