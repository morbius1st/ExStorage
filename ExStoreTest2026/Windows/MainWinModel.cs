using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB.ExtensibleStorage;
using ExStoreTest2026.DebugAssist;
using ExStorSys;
using UtilityLibrary;
using static ExStorSys.SheetFieldKeys;
using static ExStorSys.UpdateRules;


// username: jeffs
// created:  9/20/2025 6:44:51 AM

namespace ExStoreTest2026.Windows
{

	/// <summary>
	/// this class is to process UI operations and methods and is a
	/// go between the UI and everything else (except UI information
	/// which is in the mui class)<br/>
	/// </summary>
	public class MainWinModel : INotifyPropertyChanged
	{
		public int ObjectId;

	#region private fields

		private ExStorMgr xMgr;
		private MainWinModelUi mui;
		private ExStorLaunchMgr? lMgr;
		private ExStorData xData;

		private int shtIdx = 0;

		private string?[,] testFamAndTypes = new [,]
		{
			{ "window sill", "24'-0\"", "prop a" },
			{ "window sill", null, "none" },
			{ "window sill", "23'-0\"", "prop b" },
			{ "window sill", "12'-0\"", "prop c" },
			{ "window sill", "14'-0\"", "prop d" },
			{ "window sill", "16'-0\"", "prop e" },
		};

		private int idx = 0;
		private int idxMax = 6;


		private bool gotValuesFamList;
		private string tempFamilyName;
		private string tempFamilyType;
		private string tempProps;
		private string selValue;
		private Sheet? selSheet;
		private  KeyValuePair<string, FamAndType>? selFamilyTypeItem;

	#endregion

	#region ctor

		public MainWinModel()
		{
			init();
		}

		private void init()
		{
			// Debug.WriteLine($"\n*** MainWinModel init | begin");

			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(MainWinModel)) ?? -1;

			mui = MainWinModelUi.Instance;

			xMgr = ExStorMgr.Instance!;

			xData = ExStorData.Instance;
			xData.PropertyChanged += Xdata_OnPropertyChanged;

			// command initialization

			CmdResetWorkbook = new RelayCommand(cmdResetWorkbookExe,cmdResetWorkbookCanExe);
			CmdSaveWorkbook = new RelayCommand(cmdSaveWorkbookExe,cmdSaveWorkbookCanExe);

			CmdResetSheet = new RelayCommand(cmdResetSheetExe,cmdResetSheetCanExe);
			CmdSaveSheet = new RelayCommand(cmdSaveSheetExe,cmdSaveSheetCanExe);

			CmdResetFamList = new RelayCommand(CmdResetFamListExe,CmdResetFamListCanExe);
			CmdSaveFamList = new RelayCommand(CmdSaveFamListExe,CmdSaveFamListCanExe);
			
			SaveNewFamilyListItem = new RelayCommand(SaveNewFamItemExe,SaveNewFamItemCanExe);
			ClearNewFamilyListItem = new RelayCommand(ClearNewFamItemExe,ClearNewFamItemCanExe);
			DeleteAFamilyListItem = new RelayCommandAlwaysCanExecute(deleteAFamItemExe);
			
			CmdSheetAdd = new RelayCommand(cmdAddSheetExe, cmdAddSheetCanExe);
			CmdSheetDelete = new RelayCommand(cmdDeleteSheetExe, cmdDeleteSheetCanExe);
			CmdSheetUndo = new RelayCommand(cmdUndoSheetExe, cmdUndoSheetCanExe);
			CmdSheetListRestore = new RelayCommand(cmdResetSheetListExe, cmdResetSheetListCanExe);
			CmdSheetListCommit = new RelayCommand(cmdCommitSheetListExe, cmdCommitSheetListCanExe);

			// Debug.WriteLine($"\n*** MainWinModel init | exit ({ObjectId})");
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

			return false;
		}

		// just makes - no write
		public void MakeWorkBook()
		{
			xMgr.MakeWorkBook();
		}

		public void MakeEmptyWorkBook()
		{
			if (!xMgr.xData.ResetWorkBook())
			{
				Msgs.WriteLine("\n****  cannot reset WorkBook ****\n");
				return;
			}

			xMgr.MakeEmptyWorkBook();
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

			xMgr.UpdateWbkEntityField(WorkBookFieldKeys.PK_AD_DESC, new DynaValue("this is a new description"));

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

		public void ChangeSelectedSheetDesc()
		{
			Msgs.WriteLine("\n****  Change Sheet Field ****\n");

			

			if (xData.CurrentSheet == null)
			{
				Msgs.WriteLine("cannot change sheet - no selection - failed");
				return;
			}

			xMgr.UpdateShtEntityField(xData.CurrentSheet.DsName,
				RK_AD_DESC, new DynaValue("this is a new description"));

			Msgs.WriteLine("****  Done Change Sheet Field ****\n");
		}

		public void AddFamilyAndType()
		{
			bool result = false;

			string fam = "mt";
			string type = "mt";
			string props = "mt";

			if (idx < idxMax)
			{
				fam = testFamAndTypes[idx, 0];
				type = testFamAndTypes[idx, 1];
				props = testFamAndTypes[idx, 2];

				result = xMgr.AddSheetFamily(fam, type, props);

				idx++;
			}

			// if (!result)
			// {
			// 	Msgs.WriteLine($"*** FAILED  ({result} / {idx}) ***");
			// }
			// else
			// {
			// 	Msgs.WriteLine($"*** WORKED  ({fam} | {type}) ***");
			// }
		}

		public void RemoveFamliyAndType()
		{
			bool result = false;

			if (idx > 0 && idx <= idxMax)
			{
				idx--;
				result = xMgr.RemoveSheetFamily(testFamAndTypes[idx, 0], testFamAndTypes[idx, 1]);
			}

			if (!result)
			{
				Msgs.WriteLine($"*** FAILED  ({result} / {idx}) ***");
			}
			else
			{
				Msgs.WriteLine($"*** WORKED  ({testFamAndTypes[idx, 0]} | {testFamAndTypes[idx, 1]}) ***");
			}
		}

		public void AddFamilyAndType2()
		{
			string famName = TempFamilyName;
			string famTypeName = TempFamilyType;
			string famProps = "undefined";

			bool result = xMgr.AddSheetFamily(famName, famTypeName, famProps);

			if (result)
			{
				TempFamilyName = "";
				TempFamilyType = "";
			}
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

		// public void DeleteShtDs(bool onlyOne = false)
		// {
		// 	if (!xMgr.xData.GotTempAnySheets)
		// 	{
		// 		Msgs.WriteLine("cannot delete SHT Ds - no list");
		// 		return;
		// 	}
		//
		// 	if (xMgr.DeleteDsList(xMgr.xData.TempShtDsList, onlyOne))
		// 	{
		// 		Msgs.WriteLine("SHT ds deleted");
		// 	}
		// 	else
		// 	{
		// 		Msgs.WriteLine("SHT ds not deleted");
		// 	}
		// }
		//
		// public void DeleteFirstShtDs(bool onlyOne = false)
		// {
		// 	DeleteShtDs(true);
		// }
		
		/* misc */

		public void ShowWbkFromMemory()
		{
			WorkBook? wbk;
			ObservableDictionary<string, Sheet>? shts;
			
			if (!xMgr.ReadWorkBookViaTempInfo(out wbk, out shts)) return;

			DebugRoutines.ShowAWorkBook(wbk);
			DebugRoutines.ShowSheets(shts);
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

		public void ShowWbk()
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
			s.SetInitValueDym(RK_ED_XL_FILE_PATH , "excel file path");
			s.SetInitValueDym(RK_ED_XL_SHEET_NAME, "sheet name");
			s.SetInitValueDym(RK_OD_STATUS       , true);
			s.SetInitValueDym(RK_OD_SEQUENCE     , $"A00{shtIdx}"); // first
			s.SetInitValueDym(RK_OD_UPDATE_RULE  , UR_AS_NEEDED);
			s.SetInitValueDym(RK_OD_UPDATE_SKIP  , false);

			// IList<string> FamsAndTypes = new List<string>();
			Dictionary<string, string> FamsAndTypes = new ();

			FamsAndTypes.Add( "$FamilyName", "TypeName{shtIdx++}" );

			s.SetInitValueDym(RK_RD_FAMILY_LIST  , FamsAndTypes);
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

	#region tests

		/* tests */

		public void DynaValueTests()
		{
			DynaValue dv = DynaValue.InValid();
		}


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
			wbk.SetInitValueDym(WorkBookFieldKeys.PK_MD_MODEL_TITLE, "Bogus Name");

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

		public void SetWbkToInactive()
		{
			Msgs.Write("\n****  Set WorkBook To Inactive - Start ... ");

			if (xMgr.UpdateWbkEntityField(WorkBookFieldKeys.PK_AD_STATUS, new DynaValue(ActivateStatus.AS_INACTIVE)))
			{
				Msgs.WriteLine("WORKED");
			}
			else
			{
				Msgs.WriteLine("FAILED");
			}

			Msgs.WriteLine("****  Set WorkBook To Inactive - Done ****\n");
		}

		public void TestExtractCode()
		{
			bool b1 = false;
			bool b2 = false;
			bool b3 = false;
			bool b4 = false;
			bool b5 = false;
			bool b6 = false;
			bool b7 = false;

			string s1 = "copy";
			string s2 = "copy";
			string s3 = "copy";
			string s4 = "copy";
			string s5 = "copy";
			string s6 = "copy";
			string s7 = "copy";

			FieldCopyType fldFct;


			FieldCopyType fctA = FieldCopyType.FC_ALWAYS;  // 7
			FieldCopyType fctN = FieldCopyType.FC_NEVER;   // 0
			FieldCopyType fct1 = FieldCopyType.FC_TYPE_1;  // 1
			FieldCopyType fct2 = FieldCopyType.FC_TYPE_2;  // 2
			FieldCopyType fct3 = FieldCopyType.FC_TYPE_12; // 3
			FieldCopyType fct4 = FieldCopyType.FC_TYPE_4;  // 4
			FieldCopyType fct5 = FieldCopyType.FC_TYPE_14; // 5
			FieldCopyType fct6 = FieldCopyType.FC_TYPE_15; // 6

			FieldCopyType copyFct = fctA;

			Debug.WriteLine("test if fldFct (field value) works with test value, copyFct");
			Debug.WriteLine("that is does copy type (copyFct) need to be updated (true) or copied (false) ");


			for (int j = 0; j < 4; j++)
			{
				Debug.WriteLine($"\n*** does field {"fld fct",-10} {$"[#]",-4} need updating | when copy type is {$"[#]",-4} | results\n");

				for (int i = 0; i < 8; i++)
				{
					fldFct = (FieldCopyType) i;

					// try
					// {
					// 	b1 = ((FieldCopyType) (tst & smple)) == fctA;
					// }
					// catch (Exception e)
					// {
					// 	Debug.WriteLine($"b1 exception {e.Message}");
					// 	b1 = false;
					// }

					try
					{
						b2 = ((FieldCopyType) (fldFct & copyFct)) == fctN;
					}
					catch (Exception e)
					{
						Debug.WriteLine($"b2 exception {e.Message}");
						b2 = false;
					}

					try
					{
						b3 = 0 == ((int) fldFct & (int) copyFct);
					}
					catch (Exception e)
					{
						Debug.WriteLine($"b3 exception {e.Message}");
						b3 = false;
					}

					try
					{
						b4 = 1 == ((int) fldFct & (int) copyFct);
					}
					catch (Exception e)
					{
						Debug.WriteLine($"b4 exception {e.Message}");
						b4 = false;
					}

					// try
					// {
					// 	b5 = 7 == ((int) tst & (int) smple);
					// }
					// catch (Exception e)
					// {
					// 	Debug.WriteLine($"b5 exception {e.Message}");
					// 	b5 = false;
					// }

					try
					{
						b6 = fldFct.HasFlag(copyFct);
					}
					catch (Exception e)
					{
						Debug.WriteLine($"b6 exception {e.Message}");
						b6 = false;
					}

					// try
					// {
					// 	b7 = fldFct.HasFlag(copyFct);
					// }
					// catch (Exception e)
					// {
					// 	Debug.WriteLine($"b7 exception {e.Message}");
					// 	b7 = false;
					// }

					s1 = b1 ? "update" : "copy";
					s2 = b2 ? "update" : "copy";
					s3 = b3 ? "update" : "copy";
					s4 = b4 ? "update" : "copy";
					s5 = b5 ? "update" : "copy";
					s6 = b6 ? "update" : "copy";
					s7 = b7 ? "update" : "copy";

					Debug.WriteLine($"*** does field {fldFct,-10} {$"[{(int) fldFct}]",-4} need updating | when copy type is {$"[{(int) copyFct}]",-4} | results | s2 = {s2,8} | s3 = {s3,8} | s4 = {s4,8} | s6 = {s6, 8}");
				}

				if (j == 0) copyFct = fct4;
				if (j == 1) copyFct = fct1;
				if (j == 2) copyFct = fct2;
			}
		}

		public void TestExtractCode2()
		{
			bool b1;
			string s1;
			FieldCopyType copyFct;

			Debug.WriteLine("\n** for workbook\n");

			for (int i = 0; i < 3; i++)
			{
				copyFct = i == 0 ? FieldCopyType.FC_TYPE_1 : i == 1 ? FieldCopyType.FC_TYPE_4 : FieldCopyType.FC_TYPE_2;

				foreach ((WorkBookFieldKeys key, FieldDef<WorkBookFieldKeys>? f) in Fields.WorkBookFields)
				{
					b1 = f.FieldCopyType.HasFlag(copyFct);
					s1 = b1 ? "** update" : "*** copy";

					Debug.WriteLine($"for field {f.FieldName,-16} | fct {f.FieldCopyType,-10} vs copy type {(int) copyFct,-3} | {s1}");
				}
			}

			Debug.WriteLine("\n** for sheet\n");

			for (int i = 0; i < 3; i++)
			{
				copyFct = i == 0 ? FieldCopyType.FC_TYPE_1 : i == 1 ? FieldCopyType.FC_TYPE_4 : FieldCopyType.FC_TYPE_2;

				foreach ((SheetFieldKeys key, FieldDef<SheetFieldKeys>? f) in Fields.SheetFields)
				{
					b1 = f.FieldCopyType.HasFlag(copyFct);
					s1 = b1 ? "** update" : "*** copy";

					Debug.WriteLine($"for field {f.FieldName,-16} | fct {f.FieldCopyType,-10} vs copy type {(int) copyFct,-3} | {s1}");
				}
			}
		}

		public void TestDupSheet()
		{
			bool dbg = Msgs.ShowDebug;
			Msgs.ShowDebug = true;

			SheetCreationData sd = new SheetCreationData("path to the excel file", "sheet name in the file");

			string oldShtName = xMgr.Exid.CreateShtDsName("ZZZZ");
			string newShtName = xMgr.Exid.CreateShtDsName("XXXX");

			Sheet testSht = Sheet.CreateSheet(oldShtName, sd);

			testSht.SetInitValueDym(RK_AD_DATE_CREATED,  "a moment ago");
			testSht.SetInitValueDym(RK_AD_NAME_CREATED,  "by someone else");
			testSht.SetInitValueDym(RK_AD_DATE_MODIFIED, "right now");
			testSht.SetInitValueDym(RK_AD_NAME_MODIFIED, "by me");

			Dictionary<SheetFieldKeys, DynaValue> msd =
				xMgr.MakeCpyShtData(
					new DynaValue(DateTime.Now.ToString("s")),
					new DynaValue(ExStorConst.UserName),
					new DynaValue(DateTime.Now.ToString("s")),
					new DynaValue(ExStorConst.UserName));

			Msgs.WriteLine("\n** TEST Sheet\n");

			DebugRoutines.ShowSheet(testSht);
			;

			Sheet newSheet = xMgr.xLib.DuplicateSheet(2, testSht, newShtName, msd);

			Msgs.WriteLine("\n** NEW Sheet - type 2 (new version)\n");

			DebugRoutines.ShowSheet(newSheet);
			;

			newSheet = xMgr.xLib.DuplicateSheet(1, testSht, newShtName, msd);

			Msgs.WriteLine("\n** NEW Sheet - type 1 (template)\n");

			DebugRoutines.ShowSheet(newSheet);

			Msgs.ShowDebug = dbg;
		}

	#endregion

	#region Ui Io

		public WorkBook Wbk => xData.WorkBook;
		public Sheet? CurrSht => xData.CurrentSheet;
		public ExStorData XData => xData;

		public bool GotValuesFamList {
			get => gotValuesFamList;
			set
			{
				gotValuesFamList = value;
				OnPropertyChanged();
			}
		}

		public string? TempFamilyName
		{
			get => tempFamilyName;
			set
			{
				Debug.WriteLine($"got {value ?? "is null"}");

				if (value == tempFamilyName) return;

				tempFamilyName = value;
				OnPropertyChanged();

				// SaveNewFamilyListItem.RaiseCanExecuteChange(null);
				// ClearNewFamilyListItem.RaiseCanExecuteChange(null);

				GotValuesFamList = !tempFamilyType.IsVoid() && !TempFamilyName.IsVoid();
			}
		}

		public string? TempFamilyType
		{
			get => tempFamilyType;
			set
			{
				if (value == tempFamilyType) return;
				tempFamilyType = value;
				OnPropertyChanged();

				GotValuesFamList = !tempFamilyType.IsVoid() && !TempFamilyName.IsVoid();

				// SaveNewFamilyListItem.RaiseCanExecuteChange(null);
				// ClearNewFamilyListItem.RaiseCanExecuteChange(null);
			}
		}

		public string? TempProps	
		{
			get => tempProps;
			set
			{
				if (value == tempProps) return;
				tempProps = value;
				OnPropertyChanged();
			}
		}

		public string SelFamValue
		{
			get => selValue;
			set
			{
				if (value == selValue) return;
				selValue = value;
		
				Debug.WriteLine($"got selected value (key)| {value ?? "is null"}");
		
				OnPropertyChanged();
		
				// if (value != null && 
				// 	value.Equals(Sheet.AddNewKey))
				// {
				// 	selValue = CurrSht?.UpdateTempNewFamAndTypeEntry();
				// 	OnPropertyChanged();
				// }
			}
		}

		public KeyValuePair<string, FamAndType>? SelFamilyTypeItem
		{
			get => selFamilyTypeItem;
			set
			{
				selFamilyTypeItem = value;
				OnPropertyChanged();
		
				SaveNewFamilyListItem.RaiseCanExecuteChange(null);
			}
		}

		public void UpdateData()
		{
			OnPropertyChanged(nameof(Wbk));
			OnPropertyChanged(nameof(CurrSht));
			OnPropertyChanged(nameof(XData));

		}

		private void Xdata_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if ((e.PropertyName ?? "").Equals(nameof(XData.CurrentSheet)))
				OnPropertyChanged(nameof(CurrSht));
		}
	
	#endregion

	#region commands

	#region sheet list commands

		// add a sheet command

		public RelayCommand CmdSheetAdd {get; private set;}

		private void cmdAddSheetExe(object? parameter)
		{
			Sheet sht =
				xMgr.CreateNewSheet(new SheetCreationData("<un-assigned>", "<un-assigned>"));

			xData.AddSheet(sht);
		}

		private bool cmdAddSheetCanExe(object? parameter)
		{
			// parameter is currSheet.ismodified
			return true;
		}

		
		// delete a sheet command

		public RelayCommand CmdSheetDelete {get; private set;}

		private void cmdDeleteSheetExe(object? parameter)
		{
			xData.RemoveCurrentSheet((string) parameter!);
		}

		private bool cmdDeleteSheetCanExe(object? parameter)
		{
			// parameter is currSheet.ismodified
			return true;
		}

		
		// undo a sheet command

		public RelayCommand CmdSheetUndo {get; private set;}

		private void cmdUndoSheetExe(object? parameter)
		{
			xData.UndoRemoveCurrentSheet((string) parameter!);
		}

		private bool cmdUndoSheetCanExe(object? parameter)
		{
			// parameter is currSheet.ismodified
			return true;
		}


		// reset sheet list command

		public RelayCommand CmdSheetListRestore {get; private set;}

		private void cmdResetSheetListExe(object? parameter)
		{
		}

		private bool cmdResetSheetListCanExe(object? parameter)
		{
			// parameter is currSheet.ismodified
			return true;
		}


		// Commit sheet list command

		public RelayCommand CmdSheetListCommit {get; private set;}

		private void cmdCommitSheetListExe(object? parameter)
		{
		}

		private bool cmdCommitSheetListCanExe(object? parameter)
		{
			// parameter is currSheet.ismodified
			return true;
		}


		// Clear sheet list command

		public RelayCommand CmdSheetListClear {get; private set;}

		private void cmdClearSheetListExe(object? parameter)
		{
		}

		private bool cmdClearSheetListCanExe(object? parameter)
		{
			// parameter is currSheet.ismodified
			return true;
		}


	#endregion



	#region workbook commands
		
		// reset workbook command

		public RelayCommand CmdResetWorkbook {get; private set;}

		private void cmdResetWorkbookExe(object? parameter)
		{
			Wbk.UndoChangeWorkbook();
		}

		private bool cmdResetWorkbookCanExe(object? parameter)
		{
			// parameter is currWorkbook.ismodified
			return (bool) (parameter ?? false);
		}

		// save workbook command

		public RelayCommand CmdSaveWorkbook {get; private set;}

		private void cmdSaveWorkbookExe(object? parameter)
		{
			Wbk.CommitWorkbook();
		}

		private bool cmdSaveWorkbookCanExe(object? parameter)
		{
			// if (Wbk == null) return false;

			return (bool) (parameter ?? false);

		}



	#endregion

	#region sheet commands

		// reset sheet command

		public RelayCommand CmdResetSheet {get; private set;}

		private void cmdResetSheetExe(object? parameter)
		{
			CurrSht?.UndoChangeSheet();
		}

		private bool cmdResetSheetCanExe(object? parameter)
		{
			// parameter is currSheet.ismodified
			return (bool) (parameter ?? false);
		}


		// save sheet command

		public RelayCommand CmdSaveSheet {get; private set;}

		/// <summary>
		/// command to save a sheet
		/// </summary>
		/// <param name="parameter"></param>
		private void cmdSaveSheetExe(object? parameter)
		{
			CurrSht?.CommitSheet();

			// if (!CurrSht?.CommitSheet() == true)
			// {
			// 	Debug.WriteLine("** worked **");
			// }
			// else
			// {
			// 	Debug.WriteLine("** failed **");
			//
			// }
		}

		/// <summary>
		/// determine if the current sheet can be saved<br/>
		/// linked parameter is| currsht is modified
		/// </summary>
		private bool cmdSaveSheetCanExe(object? parameter)
		{
			if (CurrSht == null) return false;

			return (bool) (parameter ?? false);

		}

	#endregion

	#region family list commands

		// reset family list command

		public RelayCommand CmdResetFamList {get; private set;}

		public int FamListCount => CurrSht?.FamList?.Count ?? 0;

		public void CmdResetFamListExe(object? parameter)
		{
			// CurrSht?.ClearFamAndTypeList();
			CurrSht?.ResetFamAndTypeList();
		}
		
		public bool CmdResetFamListCanExe(object? parameter)
		{
			return (bool) (parameter ?? false);
		}


		// save family list command

		public RelayCommand CmdSaveFamList {get; private set;}

		public void CmdSaveFamListExe(object? parameter)
		{
			CurrSht?.CommitFamAndType();
		}

		public bool CmdSaveFamListCanExe(object? parameter)
		{
			if (CurrSht == null) return false;

			return CurrSht.IsModifiedFamList;
			// return CurrSht.FamListNewViewSourceCount > 0 || CurrSht.FamListModViewSourceCount > 0;
		}



		// save new family name and type

		public RelayCommand SaveNewFamilyListItem {get; private set;}

		public void SaveNewFamItemExe(object? parameter)
		{
			if (tempFamilyName.IsVoid()) return;

			xMgr.AddSheetFamily(tempFamilyName, tempFamilyType ?? "", (tempProps ?? ""));

			TempProps = null;
			TempFamilyType = null;
			TempFamilyName = null;
		}

		public bool SaveNewFamItemCanExe(object? parameter)
		{
			return !TempFamilyName!.IsVoid();
		}


		// clear new family name and type

		public RelayCommand ClearNewFamilyListItem {get; private set;}

		public void ClearNewFamItemExe(object? parameter)
		{
			if (tempFamilyName.IsVoid()) return;

			TempProps = null;
			TempFamilyType = null;
			TempFamilyName = null;
		}

		public bool ClearNewFamItemCanExe(object? parameter)
		{
			return true;
		}


		// delete a family name and type

		public RelayCommandAlwaysCanExecute DeleteAFamilyListItem {get; private set;}

		private void deleteAFamItemExe(object? parameter)
		{
			if (((string) parameter!).IsVoid()) return;

			CurrSht.RemoveFamAndType(((string) parameter!));
		}

	#endregion

	#endregion

	}
}