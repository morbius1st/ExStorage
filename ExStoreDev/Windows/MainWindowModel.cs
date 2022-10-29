#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using ShExStorageC.ShExStorage;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShExStorage;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaRowKey;
using static ShExStorageC.ShSchemaFields.ScSupport.SchemaTableKey;
using ShExStorageN.ShSchemaFields;
using ShStudy.ShEval;

#endregion

// username: jeffs
// created:  10/16/2022 9:03:08 AM

namespace ExStoreDev.Windows
{
	public class MainWindowModel
		/*: ShDebugSupport */
	{
	#region private fields

		private ShDebugMessages M { get; set; }


		private MainWindow mw;

		private ShTestProcedures01 tp01;
		private ShShowProcedures01 sp01;

		private ShowLibrary sl;

		private ShFieldDisplayData shFd;

		private ExStorageLibraryC elc;

	#endregion

	#region ctor

		public MainWindowModel(MainWindow w)
		{
			mw = w;

			M = MainWindow.M;

			sl = new ShowLibrary(w);
			shFd = new ShFieldDisplayData();

			tp01 = new ShTestProcedures01(w, M);
			sp01 = new ShShowProcedures01(w, M);

			elc = new ExStorageLibraryC();
		}

	#endregion

	#region public properties

		// public override string MessageBoxText
		// {
		// 	get => mw.MessageBoxText;
		// 	set => mw.MessageBoxText = value;
		// }
		//
		// public override void ShowMsg()
		// {
		// 	mw.ShowMsg();
		// }

	#endregion

	#region private properties

	#endregion

	#region private methods

		public void ShowExid(ExId e)
		{
			M.MarginUp();
			sp01.ShowExid(e);
			M.MarginDn();
		}

		private void ShowLockFields()
		{
			M.WriteLine("\nLOCK fields\n");

			ScValues<SchemaLockKey> values = new ScValues<SchemaLockKey>();
			values.setFieldsValues(ScInfoMeta.FieldsLock);

			sp01.ShowFieldsHeader();

			sl.WriteRows(
				shFd.ScFieldsColOrder,
				shFd.ScFieldsColDefinitions,
				shFd.SchemaLockKeyOrder,
				values.ScFieldValues,
				10, JustifyVertical.TOP, false, false);
		}

		private void ShowTableFields()
		{
			M.WriteLine("\nTABLE fields\n");

			ScValues<SchemaTableKey> values = new ScValues<SchemaTableKey>();
			values.setFieldsValues(ScInfoMeta.FieldsTable);

			sp01.ShowFieldsHeader();

			sl.WriteRows(
				shFd.ScFieldsColOrder,
				shFd.ScFieldsColDefinitions,
				shFd.SchemaTableKeyOrder,
				values.ScFieldValues,
				10, JustifyVertical.TOP, false, false);
		}

		private void ShowRowFields()
		{
			M.WriteLine("\nRow fields\n");

			ScValues<SchemaRowKey> values = new ScValues<SchemaRowKey>();
			values.setFieldsValues(ScInfoMeta.FieldsRow);

			sp01.ShowFieldsHeader();

			sl.WriteRows(
				shFd.ScFieldsColOrder,
				shFd.ScFieldsColDefinitions,
				shFd.SchemaRowKeyOrder,
				values.ScFieldValues,
				10, JustifyVertical.TOP, false, false);
		}




		private void MakeFauxRowData(ExId e, ScDataTable tbld)
		{
			ScDataRow rowd;

			// make the initial set of data elements
			// initializes the data structure
			rowd = TestMakeDataRow(e, tbld);

			// make a faux data set
			tp01.MakeFauxRowData(rowd);

			tbld.AddRow(rowd);
		}


		public ScDataTable TestMakeDataTable(ExId e)
		{
			M.WriteLine("\ntest make TABLE data\n");

			ScDataTable tbld = elc.MakeInitialDataTable(e);

			return tbld;
		}

		public ScDataRow TestMakeDataRow(ExId e, ScDataTable tbld)
		{
			// make the initial set of data elements
			ScDataRow rowd = elc.MakeInitialDataRow(e, tbld);

			return rowd;
		}

		public ScDataLock TestMakeDataLock(ExId e, ScDataTable tbld)
		{
			M.WriteLine("\ntest make LOCK data\n");

			ScDataLock lokd = elc.MakeInitialDataLock(e, tbld);

			return lokd;
		}


		private ICollectionView[] vw;

		public void TestCollectionViews(ScDataTable tbld)
		{
			M.WriteLineAligned("Test Collection Views");

			if (tbld.Rows.Count == 0)
			{
				M.WriteLineAligned("No rows to count");
				return;
			}

			vw = new ICollectionView[3];

			vw[0] = CollectionViewSource.GetDefaultView(tbld.Rows);
			vw[0].SortDescriptions.Add(
				new SortDescription("Key", ListSortDirection.Ascending));


			vw[1] = new CollectionViewSource { Source = tbld.Rows }.View;
			vw[1].Filter = item =>
			{
				KeyValuePair<string, ScDataRow> kvp = (KeyValuePair<string, ScDataRow>) item ;
				M.WriteDebugMsgLine($"filtering| ", kvp.Value.Fields[CK0_SCHEMA_NAME].Value);
				return kvp.Value.Fields[CK0_SCHEMA_NAME].DyValue.AsString().Contains("0");
			};

			int a = 1;
		}

	#endregion

	#region event consuming

	#endregion

	#region event publishing

	#endregion

	#region system overrides

		public override string ToString()
		{
			return "this is MainWindowViewModel";
		}

	#endregion

	#region public methods

		public void TestDynaValue()
		{
			// testDynamicValue();
			sp01.ShowDynaValue();
		}

		public void TestFieldsLock()
		{
			ShowLockFields();
		}

		public void TestFieldsTable()
		{
			ShowTableFields();
		}

		public void TestFieldsRow()
		{
			ShowRowFields();
		}

		public ScDataTable TestDataTable(ExId e)
		{
			ScDataTable tbld = TestMakeDataTable(e);

			M.MarginUp();
			sp01.ShowTableData(tbld);
			M.MarginDn();

			return tbld;
		}

		public ScDataRow TestDataRow(ExId e, ScDataTable tbld)
		{
			ScDataRow rowd = TestMakeDataRow(e, tbld);

			M.MarginUp();
			sp01.ShowRowData(rowd);
			M.MarginDn();

			return rowd;
		}

		public ScDataLock TestDataLock(ExId e, ScDataTable tbld)
		{
			ScDataLock lokd = TestMakeDataLock(e, tbld);

			M.MarginUp();
			sp01.ShowLockData(lokd);
			M.MarginDn();

			return lokd;
		}


		public void TestBegin(ExId e, ScDataTable tbld)
		{
			ShowExid(e);

			tbld = TestMakeDataTable(e);
		}


		public void TestRowDataAndCollectionViews(ExId e, ScDataTable tbld)
		{
			M.WriteLine("\ntest make ROW data\n");

			MakeFauxRowData(e, tbld);
			MakeFauxRowData(e, tbld);
			MakeFauxRowData(e, tbld);
			MakeFauxRowData(e, tbld);
			MakeFauxRowData(e, tbld);

			ScDataTable s = tbld;

			// TestCollectionViews(tbld);

			foreach (KeyValuePair<string, ScDataRow> kvp in tbld.Rows)
			{
				sp01.ShowRowData(kvp.Value);
			}
		}


		// public void TestDataTable2(ScDataTable tbld)
		// {
		// 	M.WriteLine("\nshow TABLE data\n");
		//
		// 	ScValues<SchemaTableKey> values = new ScValues<SchemaTableKey>();
		// 	values.setDataValues(tbld.Fields);
		//
		// 	M.MarginUp();
		// 	ShowTableData(values);
		// 	M.MarginDn();
		// }
		//
		// public void TestMakeData(ExId e, ScDataTable tbld, ScDataRow rowd, ScDataLock lokd)
		// {
		// 	tbld = TestMakeTableData(e);
		// 	// rowd = TestMakeRowData(e, tbld);
		// 	lokd = TestMakeLockData(e, tbld);
		//
		// 	// ShowTableData(tbld);
		// 	ShowRowData(rowd);
		// 	ShowLockData(lokd);
		// }
		//
		// public ScDataTable TestMakeTableData2(ExId e)
		// {
		// 	M.WriteLine("\ntest make TABLE data\n");
		//
		// 	M.MarginUp();
		// 	ScDataTable tbld = elc.MakeInitialDataTable(e);
		// 	M.MarginDn();
		//
		// 	return tbld;
		// }
		//
		// public ScDataRow TestMakeRowData(ExId e, ScDataTable sd)
		// {
		// 	return elc.MakeInitialDataRow(e, sd);
		// }
		//
		// public ScDataLock TestMakeLockData(ExId e, ScDataTable sd)
		// {
		// 	M.WriteLine("\ntest make LOCK data\n");
		//
		// 	return elc.MakeInitialDataLock(e, sd);
		// }

		// public void TestRowDataAndCollectionViews(ExId e, ScDataTable tbld)
		// {
		// 	// ScRowData2 rowd2 = new ScRowData2();
		// 	// ScRowFields2 rowf2 = new ScRowFields2();
		// 	//
		// 	// List < ScFieldDef < SchemaRowKey >> listF = new List<ScFieldDef<SchemaRowKey>>()
		// 	// {
		// 	// 	new ScFieldDef<SchemaRowKey>(CK0_KEY, "Key", "key", new DynaValue("key"), SchemaFieldDisplayLevel.DL_BASIC),
		// 	// 	new ScFieldDef<SchemaRowKey>(CK0_SCHEMA_NAME, "Name", "Name", new DynaValue("Name"), SchemaFieldDisplayLevel.DL_BASIC),
		// 	// };
		// 	//
		// 	// rowf2 = rowd2.Initialize<ScRowFields2>(listF);
		//
		//
		// 	M.WriteLine("\ntest make ROW data\n");
		//
		// 	MakeFauxRowData(e, tbld);
		// 	MakeFauxRowData(e, tbld);
		// 	MakeFauxRowData(e, tbld);
		// 	MakeFauxRowData(e, tbld);
		// 	MakeFauxRowData(e, tbld);
		//
		// 	ScDataTable s = tbld;
		//
		// 	// foreach (ScDataRow rowd in tbld.RowsList)
		// 	// {
		// 	// 	ShowRowData(rowd);
		// 	// }
		// }
		//

	#endregion
	}
}