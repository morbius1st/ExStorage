
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ExStorSys;
using UtilityLibrary;
using static ExStorSys.ExStorConstFaux;


// user name: jeffs
// created:   5/24/2026 1:36:25 PM

namespace ProcessTests2
{
	public class Tests1 : ATestsWbk, ITests
	{
		public List<string>? RunTheseTests { get; set; } 
		public string OneTestId { get; set; }

		public Tests1()
		{
			// Program.TestSetWbk = this;

			// RunTheseTests = new List<string>();
			// RunTheseTests.AddRange(["Test101A", "Test102A", ]);
			// RunTheseTests.AddRange(["Test211A", "Test212A", "Test213A", ]);
			// RunTheseTests.AddRange(["Test231A", ]);
			// RunTheseTests.AddRange(["Test334A", "Test334B", "Test334C", "Test334D", ]);

			OneTestId = "Test101A";

			register(nameof(Test101A), Test101A, test101aDesc);
			register(nameof(Test102A), Test102A, test102aDesc);

			register(nameof(Test131A), Test131A, test131aDesc);
			register(nameof(Test132A), Test132A, test132aDesc);
			register(nameof(Test132B), Test132B, test132bDesc);

			register(nameof(Test211A), Test211A, test211aDesc);
			register(nameof(Test212A), Test212A, test212aDesc);
			register(nameof(Test213A), Test213A, test213aDesc);

			register(nameof(Test231A), Test231A, test231aDesc);

			register(nameof(Test334A), Test334A, test334aDesc);
			register(nameof(Test334B), Test334B, test334bDesc);
			register(nameof(Test334C), Test334C, test334cDesc);
			register(nameof(Test334D), Test334D, test334dDesc);

		}



		// protected bool endValidateTest2(string proc)
		// {
		// 	R.WriteLine($"****************************");
		// 	R.WriteLine($"TEST VALIDATE | {$"[ {tstAnsIdx} ]",-5} | {tstId} | {proc} ");
		// 	R.WriteLine($"\nTEST VALIDATE | workbook test result  ");
		//
		// 	bool result = _V2.ValidateTests2(stopTestingOnError, tests2[tstAnsIdx]);
		//
		// 	R.Write("\n***** >> ");
		//
		// 	string answer = getAnswer(result, 4);
		//
		// 	R.WriteAnyway($"{answer} ");
		// 	R.NewLine();
		//
		// 	testResults[tstAnsIdx].Item2[1] = answer;
		//
		// 	tstAnsIdx++;
		//
		// 	return result;
		// }

		/* tests components */

		private bool wbkCanApply(bool result)
		{
			if (_wbk.ApplyBtnStatus)
			{
				R.WriteLineAnyway(" >> Can Apply - Continue >> ");
				return true;
			}

			R.WriteLineAnyway(" >> CANNOT APPLY - ABORT ");

			ShowTestCompletionResult(result);

			R.AddRouteExit("CANNOT APPLY - ABORT");

			return false;
		}

		private bool wbkCanUndo(bool result)
		{
			if (_wbk.UndoBtnStatus)
			{
				R.WriteLineAnyway(" >> Can Undo - Continue >> ");
				return true;
			}

			R.WriteLineAnyway(" >> CANNOT UNDO - ABORT ");

			ShowTestCompletionResult(result);

			R.AddRouteExit("CANNOT UNDO - ABORT");

			return false;
		}

		private bool wbkChgDesc(string desc)
		{
			string proc = "CHANGE DESC";
			
			beginTest(proc);

			_wbk.Desc = desc;

			endTest(proc);

			return endValidateTest(proc);
		}

		private bool wbkChgLastId(string lastid)
		{
			string proc = "CHANGE LAST ID";

			beginTest(proc);

			_wbk.LastId = lastid;

			endTest(proc);

			return endValidateTest(proc);
			// return endValidateTest(proc) && endValidateTest2(proc);
		}

		private bool wbkChgNameMod(string name)
		{
			string proc = "CHANGE NAME MOD";

			beginTest(proc);

			_wbk.NameModified = name;

			endTest(proc);

			return endValidateTest(proc);
			// return endValidateTest(proc) && endValidateTest2(proc);
		}

		private bool wbkUndoAll()
		{
			string proc = "UNDO ALL";

			beginTest(proc);

			_wbk.UndoChangesAll();

			endTest(proc);

			return endValidateTest(proc);
		}

		private bool wbkApplyAll()
		{
			string proc = "WORKBOOK APPLY ALL";

			beginTest(proc);

			_wbk.ApplyChangesAll();

			endTest(proc);

			return endValidateTest(proc);
		}


		/* sheet test components */

		private bool xDCanApplyShtsLst(bool result)
		{
			if (XData.ApplyBtnShtsLstStatus)
			{
				R.WriteLineAnyway(" >> Can Apply - Continue >> ");
				return true;
			}

			R.WriteLineAnyway(" >> CANNOT APPLY - ABORT ");

			ShowTestCompletionResult(result);

			R.AddRouteExit("CANNOT APPLY - ABORT");

			return false;
		}

		private bool xDCanUndoShtsLst(bool result)
		{
			if (XData.UndoBtnShtsLstStatus)
			{
				R.WriteLineAnyway(" >> Can Undo - Continue >> ");
				return true;
			}

			R.WriteLineAnyway(" >> CANNOT UNDO - ABORT ");

			ShowTestCompletionResult(result);

			R.AddRouteExit("CANNOT UNDO - ABORT");

			return false;
		}

		private bool xDAddSheet(out string shtName)
		{
			string proc = "ADD A SHEET - CHANGE LAST ID";

			beginTest(proc);

			showLastIdStatusWbk($"before {nameof(xDAddSheet)}");

			Sheet sht = CreateSheet();

			XData.AddNewSheet(sht);

			shtName = sht.DsName;

			endTest(proc);

			return endValidateTest(proc);
		}

		private bool xDDeleteSheetC(string shtName)
		{
			string proc = $"DELETE A SHEET | {shtName}";

			beginTest(proc);

			XData.DeleteSheet(shtName);

			endTest(proc);

			return endValidateTest(proc);
		}

		private bool xDUndoShtsLst()
		{
			string proc = "UNDO SHEETS LIST";

			if (!XData.UndoBtnShtsLstStatus)
			{
				R.AddRoute("*** CANNOT undo sheets list (fail)", 0, -1);
				R.WriteLine("*** CANNOT undo sheets list (fail)");
				return false;
			}

			R.AddRoute("*** CAN undo sheets list (correct)", 0, -1);
			R.WriteLine("\n*** CAN undo sheets list (correct)\n");

			beginTest(proc);

			showLastIdStatusWbk($"before {nameof(xDAddSheet)}");

			XData.ShtsLstUndoAll();

			endTest(proc);

			return endValidateTest(proc);
		}

		private bool xDApplyShtsLst()
		{
			string proc = "APPLY SHEETS LIST";

			if (!XData.ApplyBtnShtsLstStatus)
			{
				R.AddRoute("*** CANNOT apply sheets list (fail)", 0, -1);
				R.WriteLine("*** CANNOT apply sheets list (fail)");
				return false;
			}

			R.AddRoute("*** CAN apply sheets list (correct)", 0, -1);
			R.WriteLine("\n*** CAN apply sheets list (correct)\n");

			beginTest(proc);

			showLastIdStatusWbk($"before {nameof(xDAddSheet)}");

			XData.ShtsLstApplyAll();

			endTest(proc);

			return endValidateTest(proc);
		}

		private bool xDUnDeleteSheet(string shtName)
		{
			string proc = $"UN-DELETE A SHEET | {shtName}";

			beginTest(proc);

			XData.UnDeleteSheet(shtName);

			endTest(proc);

			return endValidateTest(proc);
		}

		/* test routines */

		/* 10x tests - change user field(s) */

		// matches example 2A
		private string test101aDesc = "change user field (desc)";
		public bool Test101A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			// tests = new ([_V.Ts_WbkDescChgd_Alt1]);
			tests2 = new ([V2.Ts2_WbkStdTestsA.SetTests([
				"Alt1", "T", "A",   "Alt1", "T", "T",  "Upd1", "T", "T",  "T", "T", "T"
			])]);

			uiTests2 = new ([
				//                                A    B    C     D    E     F    G     H    I
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "F",  "T", "T",  "T", "T",  "F", "F"]),
			]);

			startTest(testDesc[testId]);
			
			result &= wbkChgDesc(FAUX_WBK_DESC_ALT1);

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		// matches example 1B
		private string test102aDesc = "change user field (desc) then undo all";
		public bool Test102A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([V2.Ts2_WbkStdTestsA.SetTests(["Alt1", "T", "A",   "Alt1", "T", "T",  "Upd1", "T", "T",  "T", "T", "T"]),
				V2.Ts2_WbkStdTestsA.SetTests(["Init", "F", "N",  "Init", "F", "N",  "Init", "F", "N",   "F", "F", "F"])
			]);

			uiTests2 = new ([
				//                                A    B    C     D    E     F    G     H    I
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "F",  "T", "T",  "T", "T",  "F", "F"]),
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "T", "F",  "F", "F",  "F", "F"]),
			]);

			startTest(testDesc[testId]);
			
			result &= wbkChgDesc(FAUX_WBK_DESC_ALT1);

			if (!wbkCanUndo(result)) return false;

			result &= wbkUndoAll();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		/* 13x tests - change 3 user field(s) */

		private string test131aDesc = "change user fields (desc, last id, name mod)";
		public bool Test131A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A", "Init", "F", "N",  "Alt1", "T", "T",
					"Upd1", "T", "T",  "T", "T", "T"
				]),

				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A", "Init", "F", "N", "Alt2", "T", "X",
					"Upd1", "T", "T",  "T", "T", "T"
				]),

				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A",  "G", "T", "A",   "Alt2", "T", "X",
					"Upd1", "T", "T",  "T", "T", "T"
				]),
			]);

			uiTests2 = new ([
				//                                A    B    C     D    E     F    G     H    I
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "F",  "T", "T",  "T", "T",  "F", "F"]),
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "T",  "T", "T",  "T", "T",  "F", "F"]),
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "T", "T",  "T", "T",  "T", "T",  "F", "F"]),
			]);

			startTest(testDesc[testId]);

			result &= wbkChgDesc(FAUX_WBK_DESC_ALT1);

			result &= wbkChgNameMod(FAUX_USER_NAME_ALT2);

			result &= wbkChgLastId(FAUX_LAST_ID_UPD_G);

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		// matches excep example 1A
		private string test132aDesc = "change user fields (desc, last id, name mod) then undo all";
		public bool Test132A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A", "Init", "F", "N",  "Alt1", "T", "T",  
					"Upd1", "T", "T",  "T", "T", "T"]),

				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A", "Init", "F", "N", "Alt2", "T", "X",  
					"Upd1", "T", "T",  "T", "T", "T"]),

				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A",  "G", "T", "A",   "Alt2", "T", "X",   
					"Upd1", "T", "T",  "T", "T", "T"]),

				V2.Ts2_WbkStdTestsC.SetTests([
					"Init", "F", "N",  "Init", "F", "N",  "Init", "F", "N",  
					"Init", "F", "N",   "F", "F", "F"])
			]);

			uiTests2 = new ([
				//                                A    B    C     D    E     F    G     H    I
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "F",  "T", "T",  "T", "T",  "F", "F"]),
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "T",  "T", "T",  "T", "T",  "F", "F"]),
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "T", "T",  "T", "T",  "T", "T",  "F", "F"]),
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "T", "F",  "F", "F",  "F", "F"]),
			]);

			startTest(testDesc[testId]);

			result &= wbkChgDesc(FAUX_WBK_DESC_ALT1);

			result &= wbkChgNameMod(FAUX_USER_NAME_ALT2);

			result &= wbkChgLastId(FAUX_LAST_ID_UPD_G);

			if (!wbkCanUndo(result)) return false;

			result &= wbkUndoAll();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		// matches excep example 1B
		private string test132bDesc = "change user fields (desc, last id, name mod) then apply all";
		public bool Test132B(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				V2.Ts2_WbkStdTestsC.SetTests(["Alt1", "T", "A",
					"Init", "F", "N",  "Alt1", "T", "T",  "Upd1", "T", "T",  "T", "T", "T"]),

				V2.Ts2_WbkStdTestsC.SetTests(["Alt1", "T", "A",
					"Init", "F", "N", "Alt2", "T", "X",  "Upd1", "T", "T",  "T", "T", "T"]),

				V2.Ts2_WbkStdTestsC.SetTests(["Alt1", "T", "A",   
					"G", "T", "A",   "Alt2", "T", "X",   "Upd1", "T", "T",  "T", "T", "T"]),

				V2.Ts2_WbkStdTestsC.SetTests(["Alt1", "F", "N",  
					"G", "F", "N",  "Alt2", "F", "N",  "Upd1", "F", "N",   "F", "F", "F"])
			]);


			startTest(testDesc[testId]);

			result &= wbkChgDesc(FAUX_WBK_DESC_ALT1);

			result &= wbkChgNameMod(FAUX_USER_NAME_ALT2);

			result &= wbkChgLastId(FAUX_LAST_ID_UPD_G);

			if (!wbkCanApply(result)) return false;

			result &= wbkApplyAll();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		/* 21x tests - add one sheet */

		// matches excep example 5a
		private string test211aDesc = "add a sheet";
		public bool Test211A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;
			bool temp;

			tstId = testId;

			// tests = new ([_V.Ts_WbkDescChgd_Alt1]);
			tests2 = new ([
				V2.Ts2_WbkShtLstTestsA.SetTests(
				[ "pos_one", "F", "E",   "E", "T", "E",   "Alt1", "T", "T",
					"Upd1", "T", "T",   "T", "F", "F",   "T", "T"
				]),
			]);

			uiTests2 = new ([
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "F", "T",  "F", "F",  "T", "T"]),
			]);

			startTest(testDesc[testId]);

			result &= xDAddSheet(out _);

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		// matches excep example 5b
		private string test212aDesc = "add a sheet then undo sheets";
		public bool Test212A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;
			bool temp;

			tstId = testId;

			// tests = new ([_V.Ts_WbkDescChgd_Alt1]);
			tests2 = new ([
				V2.Ts2_WbkShtLstTestsA.SetTests(
				[ "pos_one", "F", "E",   "E", "T", "E",   "Alt1", "T", "T",
					"Upd1", "T", "T",   "T", "F", "F",   "T", "T"
				]),
				V2.Ts2_WbkShtLstTestsA.SetTests(
				[ "neg_one", "F", "N",  "E", "F", "N", "Init", "F", "N",
					"Init", "F", "N", "F", "F", "F", "F", "T"
				])
			]);

			uiTests2 = new ([
				//                                A    B    C     D    E     F    G     H    I
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "F", "T",  "F", "F",  "T", "T"]),
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "T", "F",  "F", "F",  "F", "T"])
			]);

			startTest(testDesc[testId]);

			result &= xDAddSheet(out _);

			if (!xDCanUndoShtsLst(result)) return false;

			result &= xDUndoShtsLst();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		// matches excep example 5c
		private string test213aDesc = "add a sheet then apply sheets";
		public bool Test213A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;
			bool temp;

			tstId = testId;

			// tests = new ([_V.Ts_WbkDescChgd_Alt1]);
			tests2 = new ([
				V2.Ts2_WbkShtLstTestsA.SetTests(
				[ "pos_one", "F", "E",   "E", "T", "E",   "Alt1", "T", "T",
					"Upd1", "T", "T",   "T", "F", "F",   "T", "T"
				]),
				V2.Ts2_WbkShtLstTestsA.SetTests(
				[ "zero", "F", "N",  "E", "F", "N", "Alt1", "F", "N",
					"Upd1", "F", "N",  "F", "F", "F", "F", "F"
				])
			]);

			uiTests2 = new ([
				//                                A    B    C     D    E     F    G     H    I
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "F", "T",  "F", "F",  "T", "T"]),
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "T", "F",  "F", "F",  "F", "F"])
			]);

			startTest(testDesc[testId]);

			result &= xDAddSheet(out _);

			if (!xDCanApplyShtsLst(result)) return false;

			result &= xDApplyShtsLst();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		/* 23x tests - add one+ sheet */

		private string test231aDesc = "add three sheets";
		public bool Test231A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;
			bool temp;

			tstId = testId;

			// tests = new ([_V.Ts_WbkDescChgd_Alt1]);
			tests2 = new ([
				V2.Ts2_WbkShtLstTestsA.SetTests(
				[  "pos_one", "F", "E",   "E", "T", "E",   "Alt1", "T", "T",
					"Upd1", "T", "T",   "T", "F", "F",   "T", "T"
				]),
				V2.Ts2_WbkShtLstTestsA.SetTests(
				[  "pos_one", "F", "E",   "F", "T", "E",   "Alt1", "T", "T",
					"Upd1", "T", "T",   "T", "F", "F",   "T", "T"
				]),
				V2.Ts2_WbkShtLstTestsA.SetTests(
				[  "pos_one", "F", "E",   "G", "T", "E",   "Alt1", "T", "T",
					"Upd1", "T", "T",   "T", "F", "F",   "T", "T"
				]),
			]);

			uiTests2 = new ([
				//                                A    B    C     D    E     F    G     H    I
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "F", "T",  "F", "F",  "T", "T"]),
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "F", "T",  "F", "F",  "T", "T"]),
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "F", "T",  "F", "F",  "T", "T"]),
			]);

			startTest(testDesc[testId]);

			result &= xDAddSheet(out _);

			result &= xDAddSheet(out _);

			result &= xDAddSheet(out _);

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}


		/* 33x tests - add three fields & add one sheet */

		// matches excep example 4a
		private string test334aDesc = "change user fields (desc, last id, name mod) + add one sheet";
		public bool Test334A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A", "Init", "F", "N",  "Alt1", "T", "T",
					"Upd1", "T", "T",  "T", "T", "T"
				]),

				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A", "Init", "F", "N", "Alt2", "T", "X",
					"Upd1", "T", "T",  "T", "T", "T"
				]),

				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A",  "F", "T", "A",   "Alt2", "T", "X",
					"Upd1", "T", "T",  "T", "T", "T"
				]),

				V2.Ts2_WbkShtLstTestsA.SetTests(
				[
					"pos_one", "F", "E",   "G", "T", "E",   "Alt2", "T", "X",
					"Upd1", "T", "T",   "T", "F", "F",   "T", "T"
				])
			]);

			uiTests2 = new ([
				//                                A    B    C     D    E     F    G     H    I
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "F",  "T", "T",  "T", "T",  "F", "F"]),
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "T",  "T", "T",  "T", "T",  "F", "F"]),
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "T", "T",  "T", "T",  "T", "T",  "F", "F"]),
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "F", "T",  "F", "F",  "T", "T"]),
			]);

			startTest(testDesc[testId]);

			result &= wbkChgDesc(FAUX_WBK_DESC_ALT1);

			result &= wbkChgNameMod(FAUX_USER_NAME_ALT2);

			result &= wbkChgLastId(FAUX_LAST_ID_UPD_F);

			result &= xDAddSheet(out _);


			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}


		// matches excep example 4b
		private string test334bDesc = "change user fields (desc, last id, name mod) + add one sheet => sheets apply";
		public bool Test334B(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A", "Init", "F", "N",  "Alt1", "T", "T",
					"Upd1", "T", "T",  "T", "T", "T"
				]),

				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A", "Init", "F", "N", "Alt2", "T", "X",
					"Upd1", "T", "T",  "T", "T", "T"
				]),

				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A",  "F", "T", "A",   "Alt2", "T", "X",
					"Upd1", "T", "T",  "T", "T", "T"
				]),

				V2.Ts2_WbkShtLstTestsB.SetTests(
				[
					"Alt1", "T", "A",		// desc
					"G", "Init", "T", "E",		// last id
					"Alt2", "T", "X",		// name
					"Upd1", "T", "T",		// date
					"pos_one", "F", "E",	// shts lst
					"T", "F", "F", "T", "T" // btns
				]), 

				V2.Ts2_WbkShtLstTestsB.SetTests(
				[
					"Alt1", "T", "A",		// desc
					"G", "MT" ,"F", "N",	// last id
					"Alt2", "F", "N",		// name
					"Upd1", "F", "N",		// date
					"zero", "F", "N",		// shts lst
					"F", "F", "F", "F", "F" // btns
				])
			]);

			uiTests2 = new ([
				//                                A    B    C     D    E     F    G     H    I
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "F",  "T", "T",  "T", "T",  "F", "F"]), // first: desc changed
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "T",  "T", "T",  "T", "T",  "F", "F"]), // second: name mod changed
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "T", "T",  "T", "T",  "T", "T",  "F", "F"]), // third last id changed
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "F", "T",  "F", "F",  "T", "T"]), // add sheet after desc, name mod, last id changed
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "F",  "T", "T",  "T", "T",  "F", "F"]), // sheet apply all
			]);

			startTest(testDesc[testId]);

			result &= wbkChgDesc(FAUX_WBK_DESC_ALT1);

			result &= wbkChgNameMod(FAUX_USER_NAME_ALT2);

			result &= wbkChgLastId(FAUX_LAST_ID_UPD_F);

			result &= xDAddSheet(out _);

			if (!xDCanApplyShtsLst(result)) return false;

			result &= xDApplyShtsLst();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}


		// matches excep example 4c
		private string test334cDesc = "change user fields (desc, last id, name mod) + add one sheet => sheets apply => apply all wbk";
		public bool Test334C(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new([
				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A", "Init", "F", "N",  "Alt1", "T", "T",
					"Upd1", "T", "T",  "T", "T", "T"
				]),

				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A", "Init", "F", "N", "Alt2", "T", "X",
					"Upd1", "T", "T",  "T", "T", "T"
				]),

				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A",		// desc
					"F", "T", "A",			// last id 
					"Alt2", "T", "X",		// name
					"Upd1", "T", "T",		// date
					"T", "T", "T"			// btns
				]),

				V2.Ts2_WbkShtLstTestsB.SetTests(
				[
					"Alt1", "T", "A",		// desc
					"G", "Init", "T", "E",	// last id
					"Alt2", "T", "X",		// name
					"Upd1", "T", "T",		// date
					"pos_one", "F", "E",	// shts lst
					"T", "F", "F", "T", "T" // btns
				]),

				// after sheets apply all
				V2.Ts2_WbkShtLstTestsB.SetTests(
				[
					"Alt1", "T", "A",		// desc
					"G", "MT" ,"F", "N",	// last id
					"Alt2", "F", "N",		// name
					"Upd1", "F", "N",		// date
					"zero", "F", "N",		// shts lst
					"F", "F", "F", "F", "F" // btns
				]),

				// after wbk apply all
				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "F", "N",		// desc
					"G", "F", "N",			// last id 
					"Alt2", "F", "N",		// name
					"Upd1", "F", "N",		// date
					"F", "F", "F"			// btns
				]),
			]);

			uiTests2 = new([
				//                                A    B    C     D    E     F    G     H    I
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "F",  "T", "T",  "T", "T",  "F", "F"]), // first: desc changed
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "T",  "T", "T",  "T", "T",  "F", "F"]), // second: name mod changed
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "T", "T",  "T", "T",  "T", "T",  "F", "F"]), // third last id changed
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "F", "T",  "F", "F",  "T", "T"]), // add sheet after desc, name mod, last id changed
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "F",  "T", "T",  "T", "T",  "F", "F"]), // sheet apply all
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "T", "F",  "F", "F",  "F", "F"]), // wbk apply all
			]);

			startTest(testDesc[testId]);

			result &= wbkChgDesc(FAUX_WBK_DESC_ALT1);

			result &= wbkChgNameMod(FAUX_USER_NAME_ALT2);

			result &= wbkChgLastId(FAUX_LAST_ID_UPD_F);

			result &= xDAddSheet(out _);

			if (!xDCanApplyShtsLst(result)) return false;

			result &= xDApplyShtsLst();

			if (!wbkCanApply(result)) return false;

			result &= wbkApplyAll();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}


		// matches excep example 4c
		private string test334dDesc = "change user fields (desc, last id, name mod) + add one sheet => sheets apply => undo all wbk";
		public bool Test334D(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new([
				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A", "Init", "F", "N",  "Alt1", "T", "T",
					"Upd1", "T", "T",  "T", "T", "T"
				]),

				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A", "Init", "F", "N", "Alt2", "T", "X",
					"Upd1", "T", "T",  "T", "T", "T"
				]),

				V2.Ts2_WbkStdTestsC.SetTests([
					"Alt1", "T", "A",		// desc
					"F", "T", "A",			// last id 
					"Alt2", "T", "X",		// name
					"Upd1", "T", "T",		// date
					"T", "T", "T"			// btns
				]),

				V2.Ts2_WbkShtLstTestsB.SetTests(
				[
					"Alt1", "T", "A",		// desc
					"G", "Init", "T", "E",	// last id
					"Alt2", "T", "X",		// name
					"Upd1", "T", "T",		// date
					"pos_one", "F", "E",	// shts lst
					"T", "F", "F", "T", "T" // btns
				]),

				V2.Ts2_WbkShtLstTestsB.SetTests(
				[
					"Alt1", "T", "A",		// desc
					"G", "MT" ,"F", "N",	// last id
					"Alt2", "F", "N",		// name
					"Upd1", "F", "N",		// date
					"zero", "F", "N",		// shts lst
					"F", "F", "F", "F", "F" // btns
				]),

				V2.Ts2_WbkStdTestsC.SetTests([
					"Init", "F", "N",		// desc
					"G", "F", "N",			// last id 
					"Alt2", "F", "N",		// name
					"Upd1", "F", "N",		// date
					"F", "F", "F"			// btns
				]),
			]);

			uiTests2 = new([
				//                                A    B    C     D    E     F    G     H    I
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "F",  "T", "T",  "T", "T",  "F", "F"]), // first: desc changed
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "T",  "T", "T",  "T", "T",  "F", "F"]), // second: name mod changed
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "T", "T",  "T", "T",  "T", "T",  "F", "F"]), // third last id changed
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "F", "T",  "F", "F",  "T", "T"]), // add sheet after desc, name mod, last id changed
				V2.Ts2_WbkUiEndSequenceA.SetTests(["T", "F", "F",  "T", "T",  "T", "T",  "F", "F"]), // sheet apply all
				V2.Ts2_WbkUiEndSequenceA.SetTests(["F", "F", "F",  "T", "F",  "F", "F",  "F", "F"]), // wbk undo all
			]);

			startTest(testDesc[testId]);

			result &= wbkChgDesc(FAUX_WBK_DESC_ALT1);

			result &= wbkChgNameMod(FAUX_USER_NAME_ALT2);

			result &= wbkChgLastId(FAUX_LAST_ID_UPD_F);

			result &= xDAddSheet(out _);

			if (!xDCanApplyShtsLst(result)) return false;

			result &= xDApplyShtsLst();

			if (!wbkCanApply(result)) return false;

			result &= wbkUndoAll();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}



	}
}
