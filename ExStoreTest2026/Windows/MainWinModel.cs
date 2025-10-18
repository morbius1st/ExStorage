using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
	public class MainWinModel : INotifyPropertyChanged
	{
		public int ObjectId;

	#region private fields

		private ExStorMgr xMgr;

	#endregion

	#region ctor

		public MainWinModel()
		{
			ObjectId = AppRibbon.ObjectIdx++;

			xMgr = ExStorMgr.Instance;

			xMgr.RestartReqdChanged += XMgrOnRestartReqdChanged;
		}

	#endregion

	#region public properties

		public bool? RestartStatus
		{
			get => xMgr.RestartRequired;
		}

		public string RestartStatusDesc => xMgr.RestartRequired.HasValue ? ( xMgr.RestartRequired.Value ? "Restart Needed (model)" : " No Restart Needed (model)") : "Not Applicable";

	#endregion

	#region private properties

	#endregion

	#region public methods

		/* verify */

		// if only one doc open, count components 
		// count schema using search name
		// count wbk ds using search name
		// count sht ds using search name
		// found wbk & sht ds must have the same model code

		/// <summary>
		/// 
		/// </summary>
		public bool StartupVerify(out int resultWbkSc, out int resultWbkDs, out int resultShtSc, out int resultShtDs)
		{
			Msgs.WriteLine("*** BEGIN - startup ***");

			bool result = InitialVerify(out resultWbkSc, out resultWbkDs, out resultShtSc, out resultShtDs);
			
			Msgs.WriteLine("*** startup - resolve results ***");

			if (!result)
			{
				xMgr.VerifyResolver(resultWbkSc, resultWbkDs, resultShtSc, resultShtDs);
			}
			else
			{
				Msgs.WriteLine("\t*** all good - no resolver needed ***");
			}


			Msgs.WriteLine("*** END - startup ***");

			return result;
		}

		/// <summary>
		/// initial verification<br/>
		/// return true, both good<br/>
		/// false wbk is bad and sht may be bad<br/>
		/// </summary>
		public bool InitialVerify(out int resultWbkSc, out int resultWbkDs, out int resultShtSc, out int resultShtDs)
		{
			string answer;

			bool result;

			Msgs.WriteLine("*** BEGIN - init validate ***");

			Msgs.WriteLine("\tverify got schemas and some datastorages");

			result = xMgr.InitVerify(out resultWbkSc, out resultWbkDs, out resultShtSc, out resultShtDs);

			// result is true when both are good and there is only one wbk ds and one+ sht ds
			// but no sht ds is not a failure

			answer = getInitVerifyResult(resultWbkSc, resultWbkDs, "WBK");
			Msgs.WriteLine($"\t{answer}");

			answer = getInitVerifyResult(resultShtSc, resultShtDs, "SHT");
			Msgs.WriteLine($"\t{answer}");

			showResult(result, "\n", "*** initial verify WORKED ***", "*** initial verify  FAILED ***", "");

			return result;
		}

		private string getInitVerifyResult(int resultSc, int resultDs, string preface)
		{
			string result;
			string[] scResults = new [] { "schema good", "no schema found", "one+ schema invalid", "more than one schema found" };
			string[] dsResults = new [] { "datastorage good", "no datastorage found", "one+ datastorage invalid", "more than one datastorage found" };
			// 0 = both good, & only one of each, etc.  (does not apply here)
			// 1, 2, 3 == wbk schema, something not good
			// 10, 20, 30 == wbk ds, something not good
			// 11, 12, 13, 21, 22, 23, 31, 32, 33 == both not good

			result = $"result | {preface} {ExStorConst.ScValidateResults[resultSc],-44} | {preface} {ExStorConst.DsValidateResults[resultDs]}";

			return result;
		}

		private bool verifyGetWbkDs()
		{
			Msgs.NewLine();
			Msgs.WriteLine("*** BEGIN - verify get wbk ds ***");
			bool result = true;
			string? testModelName;
			DataStorage ds;

			// once initial verify says we have the schema & ds lists
			// determine if there is a wbk ds for this model
			// got temp schema one or both
			// the list of found data storages is held on xMgr.tempWkbDsList and xMgr.tempShtDsList

			// step 0 - determine if can proceed
			//		> must have wbk schema, must have a wbk ds
			
			xMgr.TempWbkDs = null;
			xMgr.TempShtDs = null;

			if (!xMgr.GotTempWbkSchema) return false;

			testModelName= xMgr.Exid.Model_Name;

			if (! verifyDsModelNames(testModelName, out ds))
			{
				Msgs.WriteLine($"\n\tDs with model name of {testModelName} *NOT* found\n");
				result = false;
			}
			else
			{
				Msgs.WriteLine($"\n\tDs with model name of {testModelName} was *FOUND*\n");

				xMgr.TempWbkDs = ds;
			}

			Msgs.WriteLine("*** END - verify get wbk ds ***");
			Msgs.NewLine();
			return result;
		}

		private bool verifyDsModelNames(string testName, out DataStorage? ds)
		{
			ds = null;

			foreach (DataStorage dx in xMgr.TempWbkDsList!)
			{
				if (verifyModelName(testName, dx)){
					ds = dx;
					return true;
				}
			}

			return false;
		}

		private bool verifyModelName(string testName, DataStorage? ds)
		{
			string? modelName = "";

			Entity e = ds.GetEntity(xMgr.TempWbkSchema);

			if (!e.IsValid()) return false;

			return xMgr.VerifyModelName(e, testName, out modelName);
		}

		private void showResult(bool? result, string preface, string msgGood, string msgBad, string msgNeutral)
		{
			if (result == true)
			{
				Msgs.WriteLine($"{preface}*** {msgGood} ***");
			}
			else if (result == false)
			{
				Msgs.WriteLine($"{preface}*** {msgBad} ***");
			}
			else
			{
				Msgs.WriteLine($"{preface}*** {msgNeutral} ***");
			}
		}


		/* workbook */

		public void MakeWorkBook()
		{
			Msgs.WriteLine("\n****  Make WorkBook ****\n");
			xMgr.MakeWorkBook();
			Msgs.WriteLine("****  Done Make WorkBook ****\n");
		}
		
		public void MakeEmptyWorkBook()
		{
			resetWorkBook();
			Msgs.WriteLine("\n****  Make Empty WorkBook ****\n");
			xMgr.MakeEmptyWorkBook();
			Msgs.WriteLine("****  Done Make Empty WorkBook ****\n");
		}
		
		public void MakeAndWriteWorkBook()
		{
			Msgs.WriteLine("\n****  Make WorkBook ****\n");

			Msgs.WriteLine($"\t+++ MWW1 got ds? {ExStorMgr.Instance.GotWbkDs} | is empty {ExStorMgr.Instance.IsWorkBookEmpty}  [{ExStorMgr.Instance.WorkBook?.IsEmpty}]");

			xMgr.MakeWorkBook();
			Msgs.WriteLine("****  Done Make WorkBook ****\n");

			Msgs.WriteLine($"\t+++ MWW2 got ds? {ExStorMgr.Instance.GotWbkDs} | is empty {ExStorMgr.Instance.IsWorkBookEmpty}  [{ExStorMgr.Instance.WorkBook?.IsEmpty}]");

			Msgs.WriteLine("\n****  Write WorkBook ****\n");
			xMgr.WriteWorkBook();
			Msgs.WriteLine("****  Done Write WorkBook ****\n");

			Msgs.WriteLine($"\t+++ MWW3 got ds? {ExStorMgr.Instance.GotWbkDs} | is empty {ExStorMgr.Instance.IsWorkBookEmpty}  [{ExStorMgr.Instance.WorkBook?.IsEmpty}]");
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

		public void ClearAndReadWorkBook()
		{
			DataStorage? ds;
			Entity? e;
			Schema? s;

			Msgs.Col1width = 32;

			Msgs.WriteLine("\n****  Read WorkBook ****\n");

			resetWorkBook();

			Msgs.WriteLine("\tconfirming objects are null");
			Msgs.WriteLine($"\twbk is null? {xMgr.WorkBook == null}");

			Msgs.WriteLine("\tconfirming workbook found");

			if (xMgr.FindWorkBookDs(out ds, out e, out s))
			{

				Msgs.WriteLine("\t**** FOUND ****\n");

				MakeEmptyWorkBook();

				Msgs.WriteLine("\tconfirming workbook is empty");
				Msgs.WriteLine($"\twbk is empty? {xMgr.WorkBook!.IsEmpty}");

				DebugRoutines.ShowWorkBook();

				if (xMgr.ReadWorkBook(e))
				{
					Msgs.WriteLine("\t**** WORKED ****\n");

					xMgr.WorkBook.UpdateExsObjects(ds, e, s);

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


		/* sheet */

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
			resetSheets();

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
			if (!xMgr.GotTempWbkDsList)
			{
				Msgs.WriteLine("cannot delete WBK Ds - no list");
				return;
			}

			if (xMgr.DeleteDsList(xMgr.TempWbkDsList))
			{
				Msgs.WriteLine("WBK ds deleted");
			}
			else
			{
				Msgs.WriteLine("WBK ds not deleted");
			}
		}

		public void DeleteShtDs()
		{
			if (!xMgr.GotTempShtDsList)
			{
				Msgs.WriteLine("cannot delete SHT Ds - no list");
				return;
			}

			if (xMgr.DeleteDsList(xMgr.TempShtDsList))
			{
				Msgs.WriteLine("SHT ds deleted");
			}
			else
			{
				Msgs.WriteLine("SHT ds not deleted");
			}
		}

		public void DeleteWbkSc()
		{
			if (xMgr.DeleteWbkSchema())
			{
				Msgs.WriteLine("WBK schema deleted");
			}
			else
			{
				Msgs.WriteLine("WBK schema not deleted");
			}
		}

		public void DeleteShtSc()
		{
			if (!xMgr.GotTempShtSchemaList)
			{
				Msgs.WriteLine("cannot delete SHT Schema - no list");
				return;
			}

			if (xMgr.EraseScList(xMgr.TempWbkSchemaList))
			{
				Msgs.WriteLine("SHT schema deleted");
			}
			else
			{
				Msgs.WriteLine("SHT schema not deleted");
			}
		}
		

		/* find */

		public void FindAndReadAllShtDs()
		{
			Msgs.WriteLine("\n****  Find all sheets DS's****\n");

			resetSheets();

			Msgs.WriteLine("\tconfirming sheets are empty");
			Msgs.WriteLine($"\tshts is empty? {xMgr.Sheets == null || xMgr.Sheets.Count == 0}");

			Msgs.WriteLine("\tclear and read workbook");
			ClearAndReadWorkBook();

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

		public void FindAllSheetDs()
		{
			Msgs.WriteLine("\n****  Find all sheets DS's (and clear & read wbk) ****\n");

			resetSheets();

			Msgs.WriteLine("\tconfirming sheets are empty");
			Msgs.WriteLine($"\tshts is empty? {xMgr.Sheets == null || xMgr.Sheets.Count == 0}");

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

			resetWorkBook();

			Msgs.WriteLine("\n****  Find WorkBook DS ****\n");

			Msgs.WriteLine("\tconfirming objects are null");
			Msgs.WriteLine($"\twbk is null? {xMgr.WorkBook == null}");
			
			if (xMgr.FindWorkBookDs(out ds, out e, out s))
			{
				Msgs.WriteLine("\t**** WORKED ****\n");

				// DebugRoutines.ShowParameterSet(ds?.Parameters);

				Msgs.WriteLine("\tconfirming objects are found");
				Msgs.WriteLine($"\tds is null? {xMgr.WorkBook == null} ds name [{ds.Name}]");
				Msgs.WriteLine($"\te is null? {xMgr.WorkBook == null} schema name [{e.SchemaGUID}]");
				Msgs.WriteLine($"\ts is null? {xMgr.WorkBook == null} name [{s.SchemaName}]");
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

			resetWorkBook();
			resetSheets();

			Msgs.WriteLine("\n****  Find Sheet DS ****\n");

			Msgs.WriteLine("\tconfirming objects are null");
			Msgs.WriteLine($"\twbk is null?  {xMgr.WorkBook == null}");
			Msgs.WriteLine($"\tshts is empty? {xMgr.Sheets == null || xMgr.Sheets.Count == 0}");

			Msgs.WriteLine("\n****  Find WorkBook DS first ****\n");

			if (xMgr.FindWorkBookDs(out ds, out e, out s))
			{
				Msgs.WriteLine("\t**** WORKED ****\n");
				Msgs.WriteLine("\n**** then Find Sheet DS ****\n");

				string? modelCode = xMgr.ReadModelCode(e);
				string id = "AAAA";

				if (modelCode!.IsVoid()) return;


				if (xMgr.FindSheetDs(modelCode, id,
						out ds, out e, out s))
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

		public void FindAndShowElements()
		{
			Msgs.WriteLine("\n**** START - find and show all elements ****\n");

			Msgs.WriteLine("\tWBK Schema (local)");
			if (xMgr.GotWbkSchema)
			{
				Msgs.WriteLine($"\t\t{xMgr.WorkBook!.ExsSchema!.SchemaName,-40} | {xMgr.WorkBook!.ExsSchema!.IsValidObject,-8} |");
			}
			else
			{
				Msgs.WriteLine("\t\t** got no WBK schema (local) **");
			}
			Msgs.NewLine();
			Msgs.WriteLine("\tWBK DataStorage (local)");
			if (xMgr.GotWbkDs)
			{
				Msgs.WriteLine($"\t\t{xMgr.WorkBook!.ExsDataStorage!.Name,-40} | {xMgr.WorkBook!.ExsDataStorage!.IsValidObject,-8} |");
			}
			else
			{
				Msgs.WriteLine("\t\t** got no WBK datastorage (local) **");
			}

			Msgs.NewLine();
			Msgs.WriteLine("\tSHT Schema (local)");
			if (xMgr.GotShtSchema)
			{
				Msgs.WriteLine($"\t\t{xMgr.WorkBook!.ExsSheetSchema!.SchemaName,-40} | {xMgr.WorkBook!.ExsSheetSchema!.IsValidObject,-8} |");
			}
			else
			{
				Msgs.WriteLine("\t\t** got no SHT schema (local) **");
			}
			Msgs.NewLine();
			Msgs.WriteLine("\tSHT DataStorage (local)");
			if (xMgr.GotSheets)
			{
				foreach ((string? key, Sheet? sht) in xMgr.Sheets)
				{
					if (sht.ExsDataStorage!= null && sht.ExsDataStorage.IsValidObject)
					{
						Msgs.WriteLine($"\t\t{sht.ExsDataStorage.Name,-40} | {xMgr.WorkBook!.ExsDataStorage!.IsValidObject,-8} |");
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

			Msgs.NewLine();
			Msgs.WriteLine("\tWBK Schema");
			if (xMgr.FindToTempWbkSchema())
			{
				foreach (Schema sc in xMgr.TempWbkSchemaList)
				{
					Msgs.WriteLine($"\t\t{sc.SchemaName,-40} | {sc.IsValidObject,-8} |");
				}
			}
			else
			{
				Msgs.WriteLine("\t\t** got no WBK schema **");
			}
			Msgs.NewLine();
			Msgs.WriteLine("\tWBK DataStorage");
			if (xMgr.FindToTempWbkDs())
			{
				foreach (DataStorage ds in xMgr.TempWbkDsList)
				{
					Msgs.WriteLine($"\t\t{ds.Name,-40} | {ds.IsValidObject,-8} |");
				}
			}
			else
			{
				Msgs.WriteLine("\t\t** got no WBK datastorage **");
			}
			Msgs.NewLine();
			Msgs.WriteLine("\tSHT Schema");

			if (xMgr.FindToTempShtSchema())
			{
				foreach (Schema sc in xMgr.TempShtSchemaList)
				{
					Msgs.WriteLine($"\t\t{sc.SchemaName,-40} | {sc.IsValidObject,-8} |");
				}
			}
			else
			{
				Msgs.WriteLine("\t\t** got no SHT schema **");
			}
			Msgs.NewLine();
			Msgs.WriteLine("\tSHT DataStorage");
			if (xMgr.FindToTempShtDs())
			{
				foreach (DataStorage ds in xMgr.TempShtDsList)
				{
					Msgs.WriteLine($"\t\t{ds.Name,-40} | {ds.IsValidObject,-8} |");
				}
			}
			else
			{
				Msgs.WriteLine("\t\t** got no SHT datastorage **");
			}
			Msgs.NewLine();
			Msgs.WriteLine("\n**** END - find and show all elements ****\n");
		}

		/* show */

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

		public void ShowObjectId(int mainId, int cmdId)
		{
			DebugRoutines.ShowObjectId(cmdId, mainId, this.ObjectId, 
				xMgr.ObjectId, xMgr.xlib.ObjectId, xMgr.xData.ObjectId, 
				(xMgr.WorkBook?.ObjectId ?? -1));
		}

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


	#endregion

	#region private methods

		private int shtIdx = 0;

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

		private void resetWorkBook()
		{
			xMgr.ResetWorkbook();
		}

		private void resetSheets()
		{
			xMgr.ResetSheets();
		}

	#endregion

	#region event consuming

		private void XMgrOnRestartReqdChanged(object sender, bool? e)
		{
			OnPropertyChanged(nameof(RestartStatus));
			OnPropertyChanged(nameof(RestartStatusDesc));
		}

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
			return $"this is {nameof(MainWinModel)}";
		}

	#endregion
	}
}