
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ExStorSys;
using UtilityLibrary;


// user name: jeffs
// created:   5/3/2026 10:17:39 PM

namespace ProcessTests1
{
	public class Tests2
	{
		private static int shtIdx = 1;

		private string tstTitle = "";
		private int tstAnsIdx;
		private bool[,] tstAns = new bool[1,1];
		private int tstNameModIdx;
		private string[] tstNamMod = new string[1];

		private string modStatusBeg;

		private string nameModifiedOrig = "";
		private string dateModifiedOrig = "";

		private ExStorData _xData;
		
		private Sheet _sht;
		private Exid _exid;
		private Tests1 t1;

		public bool ShowShtOverRideControl { get; set; } = true;

		public Tests2()
		{
			_xData = null!;
			_sht = null!;
			_exid = null!;

			// init();
		}

		private void init()
		{
			t1 = Program.t1;

			t1.init();

			R.WriteLine("*******  init Test2 *******");
			// ShowCommon.ShowCurrent("@ test 2 - before init()", _xData, _wbk);

			ExStorConstFaux.UseFauxUserName();
			
			tstNameModIdx = -1;

			_xData = ExStorData.Instance;

			_exid = new ();

			_xData.RemovePlaceHolderSheet();

			ExStorConstFaux.UseAltUserName();

			// ShowCommon.ShowCurrent("@ test 2 - after init()", _xData, _wbk);
		}

		private void initCreateSheets()
		{
			R.WriteLine("*******  init create sheets test2 *******");

			ExStorConstFaux.UseFauxUserName();

			_xData.AddSheetPreInit(t1.CreateSheetStealth());

			_sht = t1.CreateSheetStealth();

			_xData.AddSheetPreInit(_sht);

			_xData.SelectSheet = _sht.DsName;

			Sheet s = _xData.CurrentSheet;

			Sheet sht = t1.CreateSheetStealth();

			_xData.AddSheetPreInit(sht);

			s = _xData.CurrentSheet;

			_xData.WorkBook.LastIdField.ApplyChg();

			addFamAndTypeStealth("family name 1", "type name 1", "props 1");
			addFamAndTypeStealth("family name 2", "type name 2", "props 2");
			addFamAndTypeStealth("family name 3", "type name 3", "props 3");
			addFamAndTypeStealth("family name 4", "type name 4", "props 4");

			bool rs = R.RunSilent;
			R.RunSilent = true;

			_sht.FamAndTypeApplyChanges();
			_sht.ApplyChange(_sht.FamilyListField);

			_sht.UndoModifiedName(SourceId.SI_NONE);
			_sht.UndoModifiedDate(SourceId.SI_NONE);

			_sht.IsModifiedExo = false;

			R.RunSilent = rs;

			ExStorConstFaux.UseAltUserName();

			startTests2();
		}

		private WorkBook _Wbk => _xData.WorkBook;


		// tstAns  isModifiedeExo    undo btn
		//                   apply btn       fam lst btn
		//         { false,  false,  false,  false}


		public bool Test101A(string testId, string desc = "chg sheet description, undo same")
		{
			R.AddRouteEnter();
			bool result = true;

			tstTitle = $"TEST{testId}";
			tstAnsIdx = 0;

			tstAns = new[,]
			{ 
				// 0      1     2      3       4       5
				// is mod       undo btn       nm mod == 
				//       apply btn     fam lst mod    dt mod ==
				{ true,  true,  true,  false, false, false },   // A
				{ false, false, false, false, true,  true } // B
			};

			// R.RunSilent = false;

			shtStartTest(desc);

			result &= shtChgDesc();

			R.NewLineAnyway();

			result &= shtUndoChgDesc();

			R.NewLineAnyway();

			shtShowTestResult(result);

			R.AddRouteExit();

			return result;
		}

		public bool Test101B(string testId, string desc = "chg sheet xl path, undo same")
		{
			R.AddRouteEnter();
			bool result = true;

			tstTitle = $"TEST{testId}";
			tstAnsIdx = 0;

			tstAns = new[,]
			{ 
				// 0      1     2      3       4       5
				// is mod       undo btn       nm mod == 
				//       apply btn     fam lst mod    dt mod ==
				{ true,  true,  true,  false, false, false },   // A
				{ false, false, false, false, true,  true } // B
			};

			// R.RunSilent = false;

			shtStartTest(desc);

			result &= shtChgXlFilePath();

			R.NewLineAnyway();

			result &= shtUndoXlFilePath();

			R.NewLineAnyway();

			shtShowTestResult(result);

			R.AddRouteExit();

			return result;
		}

		public bool Test102A(string testId, string desc = "chg sheet name modified, undo same")
		{
			R.AddRouteEnter();
			bool result = true;

			tstTitle = $"TEST{testId}";
			tstAnsIdx = 0;

			tstAns = new[,]
			{ 
				// 0      1     2      3       4       5
				// is mod       undo btn       nm mod == 
				//       apply btn     fam lst mod    dt mod ==
				{ true,  true,  true,  false, false, false },   // A
				{ false, false, false, false, true,  true } // B
			};

			// R.RunSilent = false;

			shtStartTest(desc);

			result &= shtChgNameMod();

			R.NewLineAnyway();

			result &= shtUndoNameMod();

			R.NewLineAnyway();

			shtShowTestResult(result);

			R.AddRouteExit();

			return result;
		}

		public bool Test104E(string testId, string desc = "add a family and type")
		{
			R.AddRouteEnter();
			bool result = true;

			tstTitle = $"TEST{testId}";
			tstAnsIdx = 0;

			tstAns = new[,]
			{
				// 0     1     2     3      4      5
				// is mod     undo btn     nm mod == 
				//     apply btn    fam lst mod    dt mod ==
				{ true, true, true, true, false, false }, // A
				{ false, false, false, false, false, false } // B
			};

			shtStartTest(desc);

			result &= shtChgByAddFamAndType();

			R.NewLineAnyway();

			shtShowTestResult(result);

			R.AddRouteExit();

			return result;

		}

		public bool Test104F(string testId, string desc = "add a family and type, undo same")
		{
			R.AddRouteEnter();
			bool result = true;

			tstTitle = $"TEST{testId}";
			tstAnsIdx = 0;

			tstAns = new[,]
			{
				// 0     1     2     3      4      5
				// is mod     undo btn     nm mod == 
				//     apply btn    fam lst mod    dt mod ==
				{ true, true, true, true, false, false }, // A
				{ false, false, false, false, true, true } // B
			};

			shtStartTest(desc);

			result &= shtChgByAddFamAndType();

			R.NewLineAnyway();

			result &= shtChgByUndoFamAndType();

			R.NewLineAnyway();

			shtShowTestResult(result);

			R.AddRouteExit();

			return result;

		}

		public bool Test105C(string testId, string desc = "sht modify user field / add family & type to list => sheet apply all")
		{
			R.AddRouteEnter();
			bool result = true;

			tstTitle = $"TEST{testId}";
			tstAnsIdx = 0;

			tstAns = new[,]
			{
				// 0     1     2     3      4      5
				// is mod     undo btn     nm mod == 
				//     apply btn    fam lst mod    dt mod ==
				{ true, true, true, false, false, false },  // A
				{ true, true, true, true, false, false },  // B
				{ false, false, false, false, true, true } // C
			};

			shtStartTest(desc);

			result &= shtChgDesc();

			R.NewLineAnyway();

			result &= shtChgByAddFamAndType();

			R.NewLineAnyway();

			if (_sht.ApplyBtnStatus)
			{
				R.WriteLine($"\tSHEET - CAN apply all - correct");

				result &= shtApplyAll();
			}
			else
			{
				R.WriteLine($"\tSHEET - can NOT apply all - fail");

				result = false;
			}

			shtShowTestResult(result);

			R.AddRouteExit();

			return result;

		}
		public bool Test105D(string testId, string desc = "sht modify user field / add family & type to list => sheet undo all")
		{
			R.AddRouteEnter();
			bool result = true;

			tstTitle = $"TEST{testId}";
			tstAnsIdx = 0;

			tstAns = new[,]
			{
				// 0     1     2     3      4      5
				// is mod     undo btn     nm mod == 
				//     apply btn    fam lst mod    dt mod ==
				{ true, true, true, false, false, false },  // A
				{ true, true, true, true, false, false },  // B
				{ false, false, false, false, true, true } // C
			};

			shtStartTest(desc);

			result &= shtChgDesc();

			R.NewLineAnyway();

			result &= shtChgByAddFamAndType();

			R.NewLineAnyway();

			if (_sht.UndoBtnStatus)
			{
				R.WriteLine($"\tSHEET - CAN undo all - correct");
				result &= shtUndoAll();
			}
			else
			{
				R.WriteLine($"\tSHEET - can NOT apply all - fail");
				result = false;
			}

			shtShowTestResult(result);

			R.AddRouteExit();

			return result;
		}


		/* operations */

		private bool shtChgDesc()
		{
			string proc = "CHANGE DESC";

			shtBeginTest(proc);

			_sht.Desc = "new value";

			return shtEndTest(proc);
		}

		private bool shtUndoChgDesc()
		{
			string proc = "UNDO CHANGE DESC";

			shtBeginTest(proc);

			_xData.CurrentSheet!.UndoChange(_xData.CurrentSheet.DescField);

			return shtEndTest(proc);
		}

		private bool shtChgByAddFamAndType()
		{
			string proc = "CHANGE BY ADD FAM and TYPE";

			shtBeginTest(proc);

			_sht.AddFamAndType("family name", "type name", "properties");

			return shtEndTest(proc);
		}

		private bool shtApplyAll()
		{
			string proc = "APPLY CHANGE ALL";

			shtBeginTest(proc);

			_sht.ApplyChangesAll(SourceId.SI_INDIRECT, SourceId.SI_NONE);

			return shtEndTest(proc);
		}

		private bool shtUndoAll()
		{
			string proc = "UNDO CHANGE ALL";

			shtBeginTest(proc);

			_sht.UndoChangesAll(SourceId.SI_INDIRECT, SourceId.SI_NONE);

			return shtEndTest(proc);
		}

		private bool shtChgByUndoFamAndType()
		{
			string proc = "UNDO CHANGE ADD FAM and TYPE";

			shtBeginTest(proc);

			_sht.UndoFamAndTypeListChanges();

			return shtEndTest(proc);
		}





		private bool shtChgXlFilePath()
		{
			string proc = "CHANGE XL PATH";

			shtBeginTest(proc);

			_sht.XlFilePath = "this is a new file path";

			return shtEndTest(proc);
		}


		private bool shtUndoXlFilePath()
		{
			string proc = "UNDO XL PATH";

			shtBeginTest(proc);

			_xData.CurrentSheet!.UndoChange(_xData.CurrentSheet.XlFilePathField);

			return shtEndTest(proc);
		}


		private bool shtChgNameMod()
		{
			string proc = "CHANGE NAME MOD";

			shtBeginTest(proc);

			_sht.NameModified = "My Name";

			return shtEndTest(proc);
		}


		private bool shtUndoNameMod()
		{
			string proc = "UNDO NAME MOD";

			shtBeginTest(proc);

			_xData.CurrentSheet!.UndoChange(_xData.CurrentSheet.NameModifiedField);

			return shtEndTest(proc);
		}





		/* show / utility routines */

		public void Reset()
		{
			R.WriteLine("\n****\nTEST2 perform complete date RESET\n****");

			ExStorData.Instance.ResetAll();

			init();

			initCreateSheets();
		}

		/// <summary>
		/// header at the start of a test sequence
		/// </summary>
		private void shtStartTest(string desc)
		{
			string nametst = "";
			string nameSht = "";
			bool nameStatus;
			string nameResult = "";

			R.WriteLineAnyway($"{tstTitle} START | {desc}");
			ShowSht.ShowSheetFields();
			ShowSht.ShowFamList();

			nameModifiedOrig = _sht.NameModified;
			dateModifiedOrig = _sht.DateModified;

			if (tstNameModIdx == 0)
			{
				nametst = tstNamMod[tstNameModIdx++];
				nameSht = _sht.NameModified;
				nameStatus = nametst.Equals(nameSht);
				nameResult = nameStatus ? "they MATCH" : "they do NOT match";

				R.WriteLine($"\n\tdoes actual {nameSht} equal test {nametst}? {nameResult}");
			}

			R.NewLineAnyway();
		}

		private void startTests2()
		{
			R.WriteLineAnyway($"\nSTART TESTS2");
			R.WriteLine($"\n*** Current user is {ExStorConstFaux.FauxUserName}\n");
			R.WriteLine($"*** IsModExo [ {_sht.IsModifiedExo} ] | IsModFamLst [ {_sht.IsModifiedFamList} ]");
			ShowWbk.ShowWorkbookFields();
			ShowSht.ShowSheetFields();
			ShowSht.ShowFamList();
			R.WriteLine($"IsModExo [ {_sht.IsModifiedExo} ] | IsModFamLst [ {_sht.IsModifiedFamList} ]");
		}

		/// <summary>
		/// header for thestart of each individual test operation
		/// </summary>
		private void shtBeginTest(string proc)
		{
			modStatusBeg = $"*** IsModExo [ {_sht.IsModifiedExo} ] | IsModFamLst [ {_sht.IsModifiedFamList} ]";

			R.WriteLine($"****************************");
			R.WriteAnyway($"{$"[ {tstAnsIdx} ]",-5} {tstTitle} | ");
			R.Write($"BEGIN ");
			R.WriteAnyway($"{proc} ");
			
			ShowSht.ShowSheet();

			R.NewLine();
			R.NewLine();
			R.StartRoute();
			R.AddRouteEnter();
		}

		private bool shtEndTest(string proc)
		{
			R.AddRouteExit();
			R.WriteLine($"\nAFTER {proc}");
			shtShowTestStatus(tstTitle);
			R.ShowRoute(modStatusBeg, $"IsModExo [ {_sht.IsModifiedExo} ] | IsModFamLst [ {_sht.IsModifiedFamList} ]");

			return showShtValidateStatus();
		}

		private void addFamAndTypeStealth(string fn, string tn, string p)
		{
			string key = ExStorLib.FormatFamAndType(fn, tn);

			FamAndType fat = FamAndType.GetNewItem(fn, tn, p);

			fat.IsNewItemFat = false;

			_sht.FamListWkg.Add(key, fat);
		}

		private void shtShowTestResult(bool result)
		{
			string r = result ? "WORKED" : "FAIL";

			R.WriteLine("\n**********");
			R.WriteLine($"{r}");
			R.WriteLine("****************************\n");
		}

		private void shtShowCurrent()
		{
			R.WriteLine($"\nthe current sheet is | {_xData.CurrentSheet!.DsName}");
		}

		private void shtShowTestStatus(string title)
		{
			R.NewLine();

			showShtLst();

			showShtFieldsOverride();

			ShowSht.shtUiStatus($"\t{title} END  ");
			R.NewLine();
		}

		private bool showShtValidateStatus()
		{
			return showShtValidateStatus(tstTitle, tstAns, tstAnsIdx++);
		}

		private bool showShtValidateStatus(string title, bool[,] a, int idx)
		{
			int rept = 23;
			bool status = true;

			string w;
			string shtA;
			string shtU;
			string famLstM = "";

			string nmModYn;
			string nmModMatch;
			string dmModYn;
			string dmModMatch;

			// string nametst = "";
			// string nameSht = "";
			// bool nameStatus;
			// string nameResult = "";

			status &= evalStatus(_sht.IsModifiedExo, a[idx, 0], out w);
			status &= evalStatus(_sht.ApplyBtnStatus, a[idx, 1], out shtA);
			status &= evalStatus(_sht.UndoBtnStatus, a[idx, 2], out shtU);
			status &= evalStatus(_sht.IsModifiedFamList, a[idx, 3], out famLstM);

			status &= evalStatusYn(_sht.NameModified.Equals(nameModifiedOrig), a[idx, 4], out nmModYn, out nmModMatch);
			status &= evalStatusYn(_sht.DateModified.Equals(dateModifiedOrig), a[idx, 5], out dmModYn, out dmModMatch);


			// if (tstNameModIdx >= 0)
			// {
			// 	nametst = tstNamMod[tstNameModIdx++];
			// 	nameSht = _sht.NameModified;
			// 	nameStatus = nametst.Equals(nameSht);
			// 	nameResult = nameStatus ? "they MATCH" : "they do NOT match";
			// 	status &= nameStatus;
			// }

			string i = $"[ {idx} ]";

			string s = status ? "WORKED" : "FAIL";

			R.WriteLine("**********");
			R.WriteLine($"{title,-16} {i,-6}| SHEET stat {w} [{a[idx, 0]}] ");
			R.WriteAnyway($"status {s,-15}");
			R.Write($" | SHEET buttons | apply {shtA} [is {_sht.ApplyBtnStatus}] [should be {enableDisable(a[idx, 1])}] | undo {shtU} [is {_sht.UndoBtnStatus}][should be {enableDisable(a[idx, 2])}]");
			R.NewLineAnyway();
			R.WriteLine($"{" ".Repeat(rept)}| FAM LST | is mod {famLstM} [is {_sht.IsModifiedFamList}] [should be {enableDisable(a[idx, 3])}]");
			R.WriteLine($"{" ".Repeat(rept)}| good [ {nmModYn} ] | NAME mod | (curr) [ {_sht.NameModified} ] {nmModMatch} (orig) [ {nameModifiedOrig} ]");
			R.WriteLine($"{" ".Repeat(rept)}| good [ {dmModYn} ] | DATE mod | (curr) [ {_sht.DateModified} ] {dmModMatch} (orig) [ {dateModifiedOrig} ]");

			// if (tstNameModIdx >= 0) R.WriteLine($"{" ".Repeat(23)}| does actual {nameSht} equal test {nametst}? {nameResult}");
			R.WriteLine("**********");

			return status;
		}

		private bool evalStatus(bool value, bool tst, out string answer)
		{
			if (value == tst)
			{
				answer = "MATCH";
				return true;
			}

			answer = "FAIL";
			return false;
		}

		private bool evalStatusYn(bool value, bool tst, out string answer, out string answer2)
		{
			if (value)
			{
				answer2 = "does MATCH";
			}
			else
			{
				answer2 = "does NOT match";
			}

			if (value == tst)
			{
				answer = "YES";
				return true;
			}

			answer = "NO";
			
			return false;
		}

		private String enableDisable(bool which)
		{
			return which ? "ENABLED" : "DISABLED";
		}

		private void showShtFieldsOverride()
		{
			if (!ShowShtOverRideControl) return;

			bool temp = R.RunSilent;

			R.RunSilent = false;

			ShowSht.ShowSheetFields();
			ShowSht.ShowFamList();

			R.RunSilent = temp;
		}

		private void showShtLst()
		{ 
			ShowWbk.ShowShtsLst();

			R.NewLine();
		}




	}
}
