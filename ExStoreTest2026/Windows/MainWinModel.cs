using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStoreTest2026.DebugAssist;
using static ExStorSys.SheetFieldKeys;
using static ExStorSys.UpdateRules;
using ExStorSys;
using UtilityLibrary;


// username: jeffs
// created:  9/20/2025 6:44:51 AM

namespace ExStoreTest2026.Windows
{
	public class MainWinModel : INotifyPropertyChanged
	{
		public int ObjectId;

	#region private fields

		private ExStorMgr xMgr;
		private MainWinModelUi mui;

		private ExStorLaunchMgr? lMgr;

		private int shtIdx = 0;

	#endregion

	#region ctor

		public MainWinModel()
		{
			init();
		}

		private void init()
		{
			// ObjectId = AppRibbon.ObjectIdx++;

			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(MainWinModel)) ?? -1;

			mui = MainWinModelUi.Instance;

			xMgr = ExStorMgr.Instance!;

			
		}

	#endregion

	#region public properties

	#endregion

	#region private properties

	#endregion

	#region public methods


		/* workbook */

		public bool MakeWorkBookSchema()
		{
			// must create the schema first
			if (xMgr.CreateWorkBookSchema())
			{
				Msgs.WriteLine($"*** WORKED workbook schema made ***");
				return true;
			}
			Msgs.WriteLine($"*** FAILED to create workbook schema (may already exist) ***");

			return false;
		}

		// just makes - no write
		public void MakeWorkBook()
		{
			Msgs.WriteLine("\n****  Make WorkBook ****\n");
			xMgr.MakeWorkBook();
			Msgs.WriteLine("****  Done Make WorkBook ****\n");
		}

		public void MakeEmptyWorkBook()
		{
			if (!xMgr.xData.ResetWorkBook())
			{
				Msgs.WriteLine("\n****  cannot reset WorkBook ****\n");
				return;
			}

			Msgs.WriteLine("\n****  Make Empty WorkBook ****\n");
			xMgr.MakeEmptyWorkBook();
			Msgs.WriteLine("****  Done Make Empty WorkBook ****\n");
		}

		public void MakeAndWriteWorkBook()
		{
			Msgs.WriteLine("\n****  Make WorkBook ****\n");

			Msgs.WriteLine($"\t+++ MWW1 got ds? {xMgr.xData.GotWbkDs} | is empty {xMgr.xData.IsWorkBookEmpty}  [{xMgr.xData.WorkBook?.IsEmpty}]");

			// must create the schema first
			if (xMgr.CreateWorkBookSchema())
			{
				Msgs.WriteLine($"*** WORKED workbook schema made ***");
			}
			else
			{
				Msgs.WriteLine($"*** FAILED to create workbook schema (may already exist) ***");
			}

			xMgr.MakeWorkBook();
			Msgs.WriteLine("****  Done Make WorkBook ****\n");



			Msgs.WriteLine($"\t+++ MWW2 got ds? {xMgr.xData.GotWbkDs} | is empty {xMgr.xData.IsWorkBookEmpty}  [{xMgr.xData.WorkBook?.IsEmpty}]");

			Msgs.WriteLine("\n****  Write WorkBook ****\n");
			xMgr.WriteWorkBook();
			Msgs.WriteLine("****  Done Write WorkBook ****\n");

			Msgs.WriteLine($"\t+++ MWW3 got ds? {xMgr.xData.GotWbkDs} | is empty {xMgr.xData.IsWorkBookEmpty}  [{xMgr.xData.WorkBook?.IsEmpty}]");
		}

		public void ShowWorkBook()
		{
			Msgs.WriteLine("\n****  Show WorkBook Data ****\n");
			DebugRoutines.ShowWorkBook();
		}

		public void ChangeWorkBook()
		{
			Msgs.WriteLine("\n****  Change WorkBook Field ****\n");

			xMgr.UpdateWbkField(WorkBookFieldKeys.PK_AD_DESC, new DynaValue("this is a new description"));

			Msgs.WriteLine("****  Done Change WorkBook Field ****\n");
		}



		/* sheet */

		public bool MakeSheetSchema()
		{
			// must create the schema first
			if (xMgr.CreateSheetSchema())
			{
				Msgs.WriteLine($"*** WORKED sheet schema made ***");
				return true;
			}
			Msgs.WriteLine($"*** FAILED to create sheet schema (may already exist) ***");

			return false;
		}

		public void MakeSheet()
		{
			SheetCreationData sd = makeFauxSheetData1("make sheet test 1");
			Msgs.WriteLine("\n****  Make Sheet ****\n");

			string dsName = xMgr.MakeSheet(sd);

			if (dsName.IsVoid())
			{
				Msgs.WriteLine($"*** FAILED ***");
			}
			else
			{
				Msgs.WriteLine($"*** WORKED  ({dsName}) ***");
			}

			Msgs.WriteLine("****  Done Make Sheet ****\n");
		}

		public void MakeAndWriteSheet()
		{
			string dsName;

			SheetCreationData sd = makeFauxSheetData1("make sheet test 1");

			// gotta make schema first
			if (xMgr.CreateSheetSchema())
			{
				Msgs.WriteLine($"*** WORKED sheet schema made ***");
			}
			else
			{
				Msgs.WriteLine($"*** FAILED to create sheet schema (may already exist) ***");
			}

			Msgs.WriteLine("\n****  Make Sheet ****\n");
			dsName = xMgr.MakeSheet(sd);

			if (dsName.IsVoid())
			{
				Msgs.WriteLine($"*** FAILED ***");

				Msgs.WriteLine("\n****  Done Make Sheet ****\n");
			}
			else
			{
				Msgs.WriteLine($"*** WORKED  ({dsName}) ***");

				Msgs.WriteLine("\n****  Done Make Sheet ****\n");

				Msgs.WriteLine("\n****  Write Sheet ****\n");

				xMgr.WriteSheet(dsName);
			}

			Msgs.WriteLine("****  Done Write Sheet ****\n");
		}

		public void ShowSheets()
		{
			Msgs.WriteLine("\n****  Show Sheet Data ****\n");
			DebugRoutines.ShowSheets();
		}

		public void MakeEmptySheet()
		{
			xMgr.xData.ResetSheets();

			Msgs.WriteLine("\n****  Make Empty Sheet ****\n");

			Sheet? sht = xMgr.MakeEmptySheet();

			if (sht == null)
			{
				Msgs.WriteLine($"*** FAILED ***");
			}
			else
			{
				Msgs.WriteLine($"*** WORKED  ({sht.DsName}) ***");

				DebugRoutines.ShowSheet(sht);
			}

			Msgs.WriteLine("****  Done Make Empty Sheet ****\n");
		}

		/* delete */

		public void DeleteWbkDs()
		{
			if (!xMgr.xData.GotTempWbkDsList)
			{
				Msgs.WriteLine("cannot delete WBK Ds - no list");
				return;
			}

			if (xMgr.DeleteDsList(xMgr.xData.TempWbkDsList))
			{
				Msgs.WriteLine("WBK ds deleted");
			}
			else
			{
				Msgs.WriteLine("WBK ds not deleted");
			}
		}

		public void DeleteShtDs(bool onlyOne = false)
		{
			if (!xMgr.xData.GotTempAnySheets)
			{
				Msgs.WriteLine("cannot delete SHT Ds - no list");
				return;
			}

			if (xMgr.DeleteDsList(xMgr.xData.TempShtDsList, onlyOne))
			{
				Msgs.WriteLine("SHT ds deleted");
			}
			else
			{
				Msgs.WriteLine("SHT ds not deleted");
			}
		}

		public void DeleteFirstShtDs(bool onlyOne = false)
		{
			DeleteShtDs(true);
		}

		// public void DeleteWbkSc()
		// {
		// 	if (!xMgr.xData.GotTempWbkSchemaList)
		// 	{
		// 		Msgs.WriteLine("cannot delete WBK Schema - no list");
		// 		return;
		// 	}
		//
		// 	// if (xMgr.DeleteWbkSchema())
		// 	if (xMgr.EraseScList(xMgr.xData.TempWbkSchemaList))
		// 	{
		// 		Msgs.WriteLine("WBK schema deleted");
		// 	}
		// 	else
		// 	{	
		// 		Msgs.WriteLine("WBK schema not deleted");
		// 	}
		// }
		//
		// public void DeleteShtSc()
		// {
		// 	if (!xMgr.xData.GotTempShtSchemaList)
		// 	{
		// 		Msgs.WriteLine("cannot delete SHT Schema - no list");
		// 		return;
		// 	}
		//
		// 	if (xMgr.EraseScList(xMgr.xData.TempShtSchemaList))
		// 	{
		// 		Msgs.WriteLine("SHT schema deleted");
		// 	}
		// 	else
		// 	{
		// 		Msgs.WriteLine("SHT schema not deleted");
		// 	}
		// }

		/* read */

		public void ClearAndReadWorkBook()
		{
			DataStorage? ds;
			Entity? e;
			Schema? s;

			Msgs.Col1Width = 32;

			Msgs.WriteLine("\n****  Read WorkBook ****\n");

			if (!xMgr.xData.ResetWorkBook())
			{
				Msgs.WriteLine("\n****  cannot reset WorkBook ****\n");
				return;
			}

			Msgs.WriteLine("\tconfirming objects are null");
			Msgs.WriteLine($"\twbk is null? {xMgr.xData.WorkBook == null}");

			if (xMgr.CreateWorkBookSchema())
			{
				Msgs.WriteLine("\t**** WORKED workbook schema made ****\n");
			}
			else
			{
				Msgs.WriteLine("\t**** FAILED workbook schema not made (may already exist) ****\n");
			}


			Msgs.WriteLine("\tconfirming workbook found");

			if (xMgr.FindWorkBookDs(out ds, out e, out s))
			{
				Msgs.WriteLine("\t**** FOUND ****\n");

				MakeEmptyWorkBook();

				Msgs.WriteLine("\tconfirming workbook is empty");
				Msgs.WriteLine($"\twbk is empty? {xMgr.xData.WorkBook!.IsEmpty}");

				DebugRoutines.ShowWorkBook();

				if (xMgr.ReadWorkBook(e))
				{
					Msgs.WriteLine("\t**** WORKED ****\n");

					xMgr.xData.WorkBook.UpdateExsObjects(ds, e, s);

					DebugRoutines.ShowWorkBook();
				}
				else
				{
					Msgs.WriteLine("\t**** FAILED ****\n");
				}
			}
			else
			{
				Msgs.WriteLine("\t**** NOT FOUND ****\n");
			}

			Msgs.NewLine();
			Msgs.WriteLine("****  Done read WorkBook ****\n");
		}

		public void ClearAndReadAll()
		{
			Msgs.WriteLine("\n****  Find all sheets DS's****\n");

			xMgr.xData.ResetSheets();

			Msgs.WriteLine("\tconfirming sheets are empty");
			Msgs.WriteLine($"\tshts is empty? {xMgr.xData.GotAnySheets}");

			Msgs.WriteLine("\tclear and read workbook");
			ClearAndReadWorkBook();

			if (xMgr.CreateSheetSchema())
			{
				Msgs.WriteLine("\t**** WORKED sheet schema made ****\n");
			}
			else
			{
				Msgs.WriteLine("\t**** FAILED sheet schema not made (may already exist) ****\n");
			}

			IList<DataStorage> dsList;

			Msgs.WriteLine("\tgot sheets");
			if (xMgr.ReadSheets())
			{
				Msgs.WriteLine("\t**** WORKED ****\n");

				DebugRoutines.ShowSheets();
			}
			else
			{
				Msgs.WriteLine("\t**** FAILED ****\n");
				return;
			}

			Msgs.NewLine();
			Msgs.WriteLine("\n****  Done Find all sheets DS's ****\n");
		}

		/// <summary>
		/// read the current information and save into ExStorData<br/>
		/// this can only be run after launch manager or if the information<br/>
		/// has been placed into the temp values in workbook<br/>
		/// </summary>
		public void ReadAllFromTemp()
		{
			bool result;

			DataStorage? ds;
			Entity? e;
			Schema? s;

			Msgs.Col1Width = 32;

			Msgs.WriteLine("\n****  Read all start ****");

			Msgs.Write("\tclear xData ... ");

			xMgr.ResetData();

			Msgs.WriteLine("DONE");

			Msgs.Write("\tsave temp workbook objects ... ");

			if (!xMgr.TransTempWbkObjectsToXdata())
			{
				Msgs.WriteLine("FAILED");

				Msgs.WriteLine("\n****  Read all done - FAIL ****\n");

				return;
			}
			
			Msgs.WriteLine("WORKED");

			Msgs.Write("\tsave temp sheet objects ... ");

			bool? gotShts = xMgr.TransTempShtObjectsToXdata();

			if (gotShts == false)
			{
				Msgs.WriteLine("FAILED");

				Msgs.WriteLine("\n****  Read all done - FAIL ****\n");

				return;
			}
			else if (!gotShts.HasValue)
			{
				Msgs.WriteLine("NULL");

				Msgs.WriteLine("****  Read all done - NO SHEETS ****\n");
			}

			Msgs.WriteLine("WORKED");

			Msgs.WriteLine("****  Read all done - WORKED ****\n");
		}

		/* find */


		public void FindAllSheetDs()
		{
			Msgs.WriteLine("\n****  Find all sheets DS's (and clear & read wbk) ****\n");

			xMgr.xData.ResetSheets();

			Msgs.WriteLine("\tconfirming sheets are empty");
			Msgs.WriteLine($"\tshts is empty? {xMgr.xData.GotAnySheets}");

			Msgs.WriteLine("\tclear and read workbook");
			ClearAndReadWorkBook();

			IList<DataStorage> dsList;

			Msgs.WriteLine("\tgot sheets");
			if (xMgr.FindSheetsDs(out dsList))
			{
				Msgs.WriteLine("\t**** WORKED ****\n");

				Msgs.WriteLine($"\tDS's found | {dsList.Count}");

				foreach (DataStorage ds in dsList)
				{
					Msgs.WriteLine($"\tDS found | {ds.Name}");
				}
			}
			else
			{
				Msgs.WriteLine("\t**** FAILED ****\n");
				return;
			}

			Msgs.NewLine();
			Msgs.WriteLine("\n****  Done Find all sheets DS's ****\n");
		}

		public void FindWorkBookDs()
		{
			DataStorage? ds;
			Entity? e;
			Schema? s;

			if (!xMgr.xData.ResetWorkBook())
			{
				Msgs.WriteLine("\n****  cannot reset WorkBook ****\n");
				return;
			}

			Msgs.WriteLine("\n****  Find WorkBook DS ****\n");

			Msgs.WriteLine("\tconfirming objects are null");
			Msgs.WriteLine($"\twbk is null? {xMgr.xData.WorkBook == null}");

			if (xMgr.FindWorkBookDs(out ds, out e, out s))
			{
				Msgs.WriteLine("\t**** WORKED ****\n");

				// DebugRoutines.ShowParameterSet(ds?.Parameters);

				Msgs.WriteLine("\tconfirming objects are found");
				Msgs.WriteLine($"\tds is null? {xMgr.xData.WorkBook == null} ds name [{ds.Name}]");
				Msgs.WriteLine($"\te is null? {xMgr.xData.WorkBook == null} schema name [{e.SchemaGUID}]");
				Msgs.WriteLine($"\ts is null? {xMgr.xData.WorkBook == null} name [{s.SchemaName}]");
			}
			else
			{
				Msgs.WriteLine("\t**** FAILED ****\n");
			}

			Msgs.NewLine();
			Msgs.WriteLine("****  Done Find WorkBook DS ****\n");
		}

		public void FindSheetDs()
		{
			DataStorage? ds;
			Entity? e;
			Schema? s;

			Sheet sht;

			if (!xMgr.xData.ResetWorkBook())
			{
				Msgs.WriteLine("\n****  cannot reset WorkBook ****\n");
				return;
			}

			xMgr.xData.ResetSheets();

			Msgs.WriteLine("\n****  Find Sheet DS ****\n");

			Msgs.WriteLine("\tconfirming objects are null");
			Msgs.WriteLine($"\twbk is null?  {xMgr.xData.WorkBook == null}");
			Msgs.WriteLine($"\tshts is empty? {xMgr.xData.GotAnySheets}");

			Msgs.WriteLine("\n****  Find WorkBook DS first ****\n");

			if (xMgr.FindWorkBookDs(out ds, out e, out s))
			{
				Msgs.WriteLine("\t**** WORKED ****\n");
				Msgs.WriteLine("\n**** then Find Sheet DS ****\n");

				// string? modelCode = xMgr.xLib.ReadModelCode(e);
				string id = "AAAA";

				// if (modelCode!.IsVoid()) return;


				if (xMgr.FindSheetDs(id, out ds, out e, out s))
				{
					Msgs.WriteLine("\t**** WORKED ****\n");

					Msgs.WriteLine($"\tds is null? {ds == null} ds name [{ds.Name}]");
					Msgs.WriteLine($"\te is null? {e == null} schema name [{e.SchemaGUID}]");
					Msgs.WriteLine($"\ts is null? {s == null} name [{s.SchemaName}]");
				}
				else
				{
					Msgs.WriteLine("\t**** FAILED ****\n");
				}
			}
			else
			{
				Msgs.WriteLine("\t**** FAILED ****\n");
			}

			Msgs.NewLine();
			Msgs.WriteLine("****  Done Find Sheet DS ****\n");
		}

		/* tests */

		/// <summary>
		/// create a workbook with an incorrect model name - this will mimic a model<br/>
		/// having been created and then renamed or having been "saveas"d<br/>
		/// must be done on a "clean" model and then saved
		/// </summary>
		public void WbkWithFalseModelName()
		{
			Msgs.WriteLine("make bogus workbook");

			if (!xMgr.xData.GotWbkSchema)
			{
				Msgs.WriteLine("*** FAILED - scheme missing");

				if (!MakeWorkBookSchema()) return;
			}

			if (xMgr.xData.GotWorkBook)
			{
				Msgs.WriteLine("*** FAILED - workbook exists");
				return;
			}

			WorkBook wbk = WorkBook.CreateNewWorkBook();
			wbk.UpdateRow(WorkBookFieldKeys.PK_MD_MODEL_NAME, new DynaValue("Bogus Name"));

			Msgs.WriteLine("write bogus workbook");

			xMgr.SaveWorkBook(wbk);

			if (!xMgr.WriteWorkBook())
			{
				Msgs.WriteLine("\n**** WORKED ****\n");
			}
			else
			{
				Msgs.WriteLine("\n**** FAILED ****\n");
			}

		}

		// /// <summary>
		// /// this creates a sheet with a set model code rather than using the "correct"<br/>
		// /// model code.  this must be done in a model which has a valid workboon and may<br/>
		// /// have a valid sheet
		// /// </summary>
		// public void ShtWithFalseModelCode()
		// {
		// 	Msgs.WriteLine("make bogus sheet");
		//
		// 	if (!xMgr.xData.GotShtSchema)
		// 	{
		// 		Msgs.WriteLine("*** FAILED - scheme missing");
		//
		// 		if (!MakeSheetSchema()) return;
		// 	}
		//
		// 	if (!xMgr.xData.GotWorkBook)
		// 	{
		// 		Msgs.WriteLine("*** FAILED - workbook missing");
		// 		return;
		// 	}
		//
		// 	// string origModelCode = xMgr.xData.TempModelCode;
		// 	//
		// 	// xMgr.xData.TempModelCode = "20251024_070000";
		//
		// 	string shtName = xMgr.Exid.CreateShtDsName("AAAx");
		//
		// 	// xMgr.xData.TempModelCode = origModelCode;
		//
		// 	SheetCreationData sd = makeFauxSheetData1("make sheet test 1");
		//
		// 	Sheet sht = Sheet.CreateSheet(shtName, sd);
		//
		// 	Msgs.WriteLine("write bogus sheet");
		//
		// 	ExStoreRtnCode rtnCode = xMgr.xLib.WriteSheet(sht, xMgr.xData.SheetSchema);
		//
		// 	if (rtnCode == ExStoreRtnCode.XRC_GOOD)
		// 	{
		// 		Msgs.WriteLine("\n**** WORKED ****\n");
		// 	}
		// 	else
		// 	{
		// 		Msgs.WriteLine("\n**** FAILED ****\n");
		// 	}
		// }

		public void SetWbkToInactive()
		{
			Msgs.Write("\n****  Set WorkBook To Inactive - Start ... ");

			if (xMgr.UpdateWbkField(WorkBookFieldKeys.PK_AD_STATUS, new DynaValue(ActivateStatus.AS_INACTIVE)))
			{
				Msgs.WriteLine("WORKED");
			}
			else
			{
				Msgs.WriteLine("FAILED");
			}

			Msgs.WriteLine("****  Set WorkBook To Inactive - Done ****\n");
		}
		
		/* misc */

		public void TestOnOpenDocLaunch()
		{
			StartLaunchManager();

			lMgr.Reset();

			lMgr.OnOpenDocLaunch();

			ShowMsgCache();
		}

		public void TestVerify()
		{
			StartLaunchManager();

			lMgr.Reset();

			Msgs.WriteLine("**** disabled ****\n");

			// lMgr.TestVfy1();
		}

		public void ShowMsgCache()
		{
			if (!xMgr.MessageCache!.IsVoid())
			{
				Msgs.WriteCache(xMgr.MessageCache!);
				xMgr.MessageCache = null;
			}
		}

		public void StartLaunchManager()
		{
			if (lMgr != null) return;

			if (ExStorLaunchMgr.Instance != null)
			{
				lMgr = ExStorLaunchMgr.Instance;
			}
			else
			{
				lMgr = ExStorLaunchMgr.Create();
			}
		}


		/* show */

		public void FindAndShowElements()
		{
			DebugRoutines.FindAndShowExObjects();
		}


		public void ShowInfo1()
		{
			DebugRoutines.ShowR();
			Msgs.NewLine();
			ShowConsts();
			Msgs.NewLine();
			ShowWorkBookSchema();
			Msgs.NewLine();
			ShowSheetSchema();
			Msgs.NewLine();
			ShowExidInfo();
			Msgs.NewLine();
		}

		public void ShowInfo2()
		{
			ShowExidInfo();
			Msgs.NewLine();
			ShowExMgr();
			Msgs.NewLine();
		}

		public void ShowConsts()
		{
			DebugRoutines.ShowConsts();
		}

		public void ShowWorkBookSchema()
		{
			DebugRoutines.ShowWorkBookFields();
		}

		public void ShowSheetSchema()
		{
			DebugRoutines.ShowSheetFields();
		}

		public void ShowExidInfo()
		{
			DebugRoutines.ShowExid();
		}

		public void ShowExData()
		{
			ShowWorkBook();

			ShowSheets();
		}

		public void ShowExMgr()
		{
			DebugRoutines.ShowExMgr();
		}

		public void ShowObjectId(int mainId)
		{
			DebugRoutines.ShowObjectId(mainId, this.ObjectId);

			DebugRoutines.ShowObjectId();

			// DebugRoutines.ShowObjectId(cmdId, mainId, 
			// 	this.ObjectId, MainWinModelUi.Instance.ObjectId,
			// 	xMgr.ObjectId, xMgr.xLib.ObjectId, xMgr.xData.ObjectId,
			// 	lMgr?.ObjectIdStr ?? "is null",
			// 	(xMgr.xData.WorkBook?.ObjectId ?? -1));
		}

	#endregion

	#region private methods

		private SheetCreationData makeFauxSheetData1(string xlShtName)
		{
			return new SheetCreationData("excel file path", xlShtName);
		}

		private void addFauxSheetData1(ref Sheet s)
		{
			s.UpdateRow(RK_ED_XL_FILE_PATH, new DynaValue("excel file path"));
			s.UpdateRow(RK_ED_XL_SHEET_NAME, new DynaValue("sheet name"));
			s.UpdateRow(RK_OD_STATUS, new DynaValue(true));
			s.UpdateRow(RK_OD_SEQUENCE, new DynaValue($"A00{shtIdx}")); // first
			s.UpdateRow(RK_OD_UPDATE_RULE, new DynaValue(UR_AS_NEEDED));
			s.UpdateRow(RK_OD_UPDATE_SKIP, new DynaValue(false));

			IList<string> FamsAndTypes = new List<string>();

			FamsAndTypes.Add($"FamilyName|TypeName{shtIdx++}");

			s.UpdateRow(RK_RD_FAMILY_LIST, new DynaValue(FamsAndTypes));
		}

	#endregion

	#region event consuming

	#endregion

	#region event publishing

		public event PropertyChangedEventHandler PropertyChanged;

		[DebuggerStepThrough]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"{nameof(MainWinModel)} [{ObjectId}]";
		}

	#endregion


		/*  voided
		public void FindAndShowElements1()
		{
			string? name;
			string? mn;
			string? mc1;
			string? mc2;
			string temp;

			Msgs.WriteLine("\n**** START - find and show all elements ****\n");

			Msgs.NewLine();
			Msgs.WriteLine($"\n\t** current model name {xMgr.Exid.Model_Name} [{xMgr.Exid.ModelName}] **\n");

			Msgs.WriteLine("\tWBK Schema (local)");
			if (xMgr.xData.GotWbkSchema)
			{
				Msgs.WriteLine($"\t\t{xMgr.xData.WorkBookSchema!.SchemaName,-40} | valid {xMgr.xData.WorkBookSchema!.IsValidObject,-8} | {xMgr.xData.WorkBookSchema.GUID}");

			}
			else
			{
				Msgs.WriteLine("\t\t** got no WBK schema (local) **");
			}

			Msgs.NewLine();
			Msgs.WriteLine("\tWBK DataStorage (local)");
			if (xMgr.xData.GotWbkDs)
			{
				name = xMgr.xData.WorkBook!.ExsDataStorage!.Name;
				mn = xMgr.xData.WorkBook.Model_Name;
				mc1 = xMgr.xData.WorkBook.ModelCode;
				mc2 = xMgr.xlib.ExtractModelCodeFromName(name, xMgr.Exid.WbkSearchName) ?? "is null";

				Msgs.WriteLine($"\t\t{xMgr.xData.WorkBook!.ExsDataStorage!.Name,-40} | valid {xMgr.xData.WorkBook!.ExsDataStorage!.IsValidObject,-8} | {mn,-12} | model code | stored {mc1} from name {mc2}");

				showDsSchemaGuids(xMgr.xData.WorkBook!.ExsDataStorage);

			}
			else
			{
				Msgs.WriteLine("\t\t** got no WBK datastorage (local) **");
			}

			Msgs.NewLine();
			Msgs.WriteLine("\tSHT Schema (local)");
			if (xMgr.xData.GotShtSchema)
			{
				Msgs.WriteLine($"\t\t{xMgr.xData.SheetSchema!.SchemaName,-40} | valid {xMgr.xData.SheetSchema!.IsValidObject,-8} | {xMgr.xData.SheetSchema.GUID}");
			}
			else
			{
				Msgs.WriteLine("\t\t** got no SHT schema (local) **");
			}

			Msgs.NewLine();
			Msgs.WriteLine("\tSHT DataStorage (local)");
			if (xMgr.xData.GotAnySheets)
			{
				foreach ((string? key, Sheet? sht) in xMgr.xData.Sheets)
				{
					if (sht.ExsDataStorage != null && sht.ExsDataStorage.IsValidObject)
					{
						name = sht.ExsDataStorage.Name;
						mc2 = xMgr.xlib.ExtractModelCodeFromName(name, xMgr.Exid.ShtSearchName) ?? "is null";

						Msgs.WriteLine($"\t\t{sht.ExsDataStorage.Name,-40} | valid {xMgr.xData.WorkBook!.ExsDataStorage!.IsValidObject,-8} | model code | from name {mc2}");

						showDsSchemaGuids(sht.ExsDataStorage);
					}
					else
					{
						Msgs.WriteLine($"\t\t** SHT datastorage {key} is null (local) **");
					}
				}
			}
			else
			{
				Msgs.WriteLine("\t\t** got no SHT datastorage (local) **");
			}

			// above, what is currently saved on local data objects
			// below find actual information

			Msgs.NewLine();
			Msgs.WriteLine("\tWBK Schema (live)");
			if (xMgr.FindAllWbkSchema())
			{
				foreach (Schema sc in xMgr.xData.TempWbkSchemaList)
				{
					Msgs.WriteLine($"\t\t{sc.SchemaName,-40} | valid {sc.IsValidObject,-8} | {sc.GUID}");
				}

				if (xMgr.xData.TempWbkSchemaList.Count == 1)
				{
					xMgr.xData.TempWbkSchema = xMgr.xData.TempWbkSchemaList[0];
				}
			}
			else
			{
				Msgs.WriteLine("\t\t** got no WBK schema **");
			}

			Msgs.NewLine();
			Msgs.WriteLine("\tWBK DataStorage (live)");
			if (xMgr.FindAllWbkDs())
			{
				foreach (DataStorage ds in xMgr.xData.TempWbkDsList)
				{
					name = ds.Name;

					if (xMgr.xData.TempWbkSchema != null)
					{
						mn = xMgr.ReadModelName(ds, xMgr.xData.TempWbkSchema) ?? "is null";
						mc1 = xMgr.ReadModelCode(ds, xMgr.xData.TempWbkSchema) ?? "is null";
						mc2 =  xMgr.xlib.ExtractModelCodeFromName(name, xMgr.Exid.WbkSearchName) ?? "is null";
						temp = $"{mn,-12} | model code | stored {mc1} from name {mc2}";
					}
					else
					{
						temp = "cannot get info - no temp schema";
					}

					Msgs.WriteLine($"\t\t{ds.Name,-40} | valid {ds.IsValidObject,-8} | {temp}");

					showDsSchemaGuids(ds);
				}
			}
			else
			{
				Msgs.WriteLine("\t\t** got no WBK datastorage **");
			}

			Msgs.NewLine();
			Msgs.WriteLine("\tSHT Schema (live)");

			if (xMgr.FindAllShtSchema())
			{
				foreach (Schema sc in xMgr.xData.TempShtSchemaList)
				{
					Msgs.WriteLine($"\t\t{sc.SchemaName,-40} | valid {sc.IsValidObject,-8} | {sc.GUID}");
				}

				if (xMgr.xData.TempShtSchemaList.Count == 1)
				{
					xMgr.xData.TempShtSchema = xMgr.xData.TempShtSchemaList[0];
				}

			}
			else
			{
				Msgs.WriteLine("\t\t** got no SHT schema **");
			}

			Msgs.NewLine();
			Msgs.WriteLine("\tSHT DataStorage (live)");
			if (xMgr.FindAllShtDs())
			{
				foreach (DataStorage ds in xMgr.xData.TempShtDsList)
				{
					if (xMgr.xData.TempShtSchema != null)
					{
						name = ds.Name;
						mc2 =  xMgr.xlib.ExtractModelCodeFromName(name, xMgr.Exid.ShtSearchName) ?? "is null";
						temp = $"model code | from name {mc2}";
					}
					else
					{
						temp = "cannot get info - no temp schema";
					}

					Msgs.NewLine();
					Msgs.WriteLine($"\t\t{ds.Name,-40} | valid {ds.IsValidObject,-8} | {temp}");
					showDsSchemaGuids(ds);
				}
			}
			else
			{
				Msgs.WriteLine("\t\t** got no SHT datastorage **");
			}

			Msgs.NewLine();
			Msgs.WriteLine("\n**** END - find and show all elements ****\n");
		}

		private void showDsSchemaGuids(DataStorage? ds)
		{
			if (ds == null || ds.GetEntitySchemaGuids().Count == 0)
			{
				Msgs.WriteLine($"\t\t\tno stored schema guids");
			}

			foreach (Guid g in ds.GetEntitySchemaGuids())
			{
				Msgs.WriteLine($"\t\t\t{g}");
			}
		}
		*/

		// private void XMgrOnRestartReqdChanged(object sender, bool? e)
		// {
		// 	OnPropertyChanged(nameof(RestartStatus));
		// 	OnPropertyChanged(nameof(RestartStatusDesc));
		// }
		//
		// private void XMgrOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
		// {
		// 	switch (e.PropertyName)
		// 	{
		// 	case nameof(xMgr.ExStorStatus):
		// 		{
		// 			Mui.ExStorStatus = xMgr.ExStorStatus;
		// 			// ExStorStatus = xMgr.ExStorStatus;
		// 			break;
		// 		}
		// 	}
		// }

		// public void FindWorkBookDs2()
		// {
		// 	DataStorage? ds;
		// 	Entity? e;
		// 	Schema? s;
		//
		// 	resetWorkBook();
		//
		// 	Msgs.WriteLine("\n****  Find WorkBook DS ****\n");
		// 	
		// 	ExStoreRtnCode rtnCode = xMgr.FindWorkBookDs(out ds, out e, out s);
		//
		// 	if (rtnCode == ExStoreRtnCode.XRC_GOOD)
		// 	{
		// 		Msgs.WriteLine("**** WORKED ****\n");
		//
		// 		DebugRoutines.ShowParameterSet(xMgr.WorkBook?.ExsDataStorage?.Parameters);
		// 	}
		// 	else
		// 	{
		// 		Msgs.WriteLine("**** FAILED ****\n");
		// 	}
		//
		//
		// 	Msgs.WriteLine("****  Done Find WorkBook DS ****\n");
		// }


		
		// private void resetWorkBook()
		// {
		// 	xMgr.ResetWorkbook();
		// }

		// private void resetSheets()
		// {
		// 	xMgr.ResetSheets();
		// }

	}
}