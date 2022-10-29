#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ExStorage.TestProcedures;
using SharedApp.Windows.ShSupport;
using ShExStorageC.ShExStorage;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageR.ShExStorage;
using ShExStorageN.ShSchemaFields;
using ShStudy.ShEval;
using TestProcedures01 = ExStorage.TestProcedures.TestProcedures01;

#endregion

// username: jeffs
// created:  10/2/2022 9:37:41 AM

namespace ExStorage.Windows
{
	public class MainWindowModel
		//: ShDebugSupport
	{
	#region private fields

		private ShFieldDisplayData shFd;
		private ShowLibrary sl;
		private ShDebugMessages M { get; set; }

		private ExStorageLibraryC exLibc;
		private ExStorageLibraryR exLibr;

		private TestProcedures01 tp01;
		private ShowsProcedures01 sp01;

		private ShShowProcedures01 shsp01;

		// private MainWindow mw;

		private ExStoreRtnCode result;


		// fields

	#endregion

	#region ctor

		public MainWindowModel(MainWindow w,  ShDebugMessages msgs)
		{

			M = msgs;

			sp01 = new ShowsProcedures01();
			tp01 = new TestProcedures01();

			shsp01 = new ShShowProcedures01(w, msgs);

			exLibr = new ExStorageLibraryR();
			exLibc = new ExStorageLibraryC();

			sl = new ShowLibrary(w);
			shFd = new ShFieldDisplayData();

			UnitType u = UnitType.UT_Acceleration;

			ForgeTypeId ut = SpecTypeId.Length;
			ForgeTypeId dt = UnitTypeId.Feet;
			ForgeTypeId st = SymbolTypeId.FootSingleQuote;
		}

	#endregion

	#region public properties

		public ExStoreRtnCode LastResult => result;
		public bool LastResultSuccess => result == ExStoreRtnCode.XRC_GOOD;

		// public void ShowMsg()
		// {
		// 	mw.ShowMsg();
		// }

	#endregion

	#region private properties

		// override properties 

	#endregion

	#region private methods

		// override methods

	#endregion

	#region event consuming

	#endregion

	#region event publishing

	#endregion

	#region system overrides

		public override string ToString()
		{
			return "this is MainWindowModel";
		}

	#endregion


	#region public methods

		public bool DoesDsExist(ExId exid)
		{
			bool result = exLibr.DoesDsExist(exid);

			return result;
		}

	#endregion


	#region tests

		// tests
		public void GetAllDs1()
		{
			tp01.TestGetAllDs1(exLibr);
		}

		public void FindDsByName3()
		{
			tp01.TestFindDsByName3(exLibr);
		}

		public void DeleteDsByName4()
		{
			tp01.TestDeleteDsByName4(exLibr);
		}

		public void GetSchemaByName2()
		{
			tp01.TestFindExistSchema2(exLibr);
		}

		public void GetDsEntity5()
		{
			tp01.TestGetDsEntity5(exLibr);
		}

		public void GetEntityData6()
		{
			tp01.TestGetEntityData6(exLibr);
		}

		public void DoesDsExist7()
		{
			tp01.TestDoesDsExist7(exLibr);
		}

		// show
		public void ShowExid(ExId exid)
		{
			sp01.ShowEid(exid);
		}

		public void ShowTableData(ScDataTable tbld)
		{
			shsp01.ShowTableData(tbld);
		}

		public void ShowProcFn11()
		{
			M.WriteDebugMsgLine("Proc Fn11", "unknown");
			// ShowMsg();
			// sp01.ShowEid(exLib);
		}

		public void ShowProcFn12()
		{
			M.WriteDebugMsgLine("Proc Fn12", "unknown");
			// ShowMsg();

			// if (result != ExStoreRtnCode.XRC_GOOD)
			// {
			// 	WriteDebugMsgLine("find all ds", "*** failed ***");
			// }
			// else
			// {
			// 	sp01.ShowAllDs(exLib);
			// }
			//
			// ShowMsg();
		}

		// schema fields and data

		public ScDataTable MakeTableData(ExId exid)
		{
			return exLibc.MakeInitialDataTable(exid);
		}

		// process

		// does the DS already exist?
		public ExStoreRtnCode TestBeginPhase1(ExId exid)
		{
			M.WriteLineAligned("begin phase 1");

			ShowExid(exid);

			M.WriteLineAligned("\nshow all DS");

			GetAllDs1();

			bool result = DoesDsExist(exid);

			M.WriteLineAligned("\n DS exists?|", $"{result}");

			return result ? ExStoreRtnCode.XRC_DS_EXISTS : ExStoreRtnCode.XRC_DS_NOT_FOUND;
		}

		// make a new schema






	#endregion
	}
}