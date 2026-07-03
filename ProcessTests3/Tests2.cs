using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExStorSys;
using UtilityLibrary;
using static ExStorSys.ExStorConstFaux;


// user name: jeffs
// created:   5/31/2026 4:57:22 PM

namespace ProcessTests3
{
	public class Tests2 : ATestsSht, ITests
	{
		public List<string> RunTheseTests { get; set; }
		public string OneTestId { get; set; }

		public Tests2()
		{
			Program.TestSetSht = this;

			RunTheseTests = new List<string>();
			RunTheseTests.AddRange(["Test1001B", "Test1002B", ]);

			// OneTestId = "Test1002B";


			register(nameof(Test1403A), Test1403A, test1403aDesc);

			register(nameof(Test1402A), Test1402A, test1402aDesc);
			register(nameof(Test1401A), Test1401A, test1401aDesc);

			register(nameof(Test1315A), Test1315A, test1315aDesc);
			register(nameof(Test1314A), Test1314A, test1314aDesc);
			register(nameof(Test1313A), Test1313A, test1313aDesc);
			register(nameof(Test1312A), Test1312A, test1312aDesc);
			register(nameof(Test1311A), Test1311A, test1311aDesc);

			register(nameof(Test1213A), Test1213A, test1213aDesc);
			register(nameof(Test1212A), Test1212A, test1212aDesc);
			register(nameof(Test1211A), Test1211A, test1211aDesc);

			register(nameof(Test1032A), Test1032A, test1032aDesc);
			register(nameof(Test1031A), Test1031A, test1031aDesc);

			register(nameof(Test1001A), Test1001A, test1001aDesc);
			register(nameof(Test1001B), Test1001B, test1001bDesc);
			register(nameof(Test1002A), Test1002A, test1002aDesc);
			register(nameof(Test1002B), Test1002B, test1002bDesc);
			register(nameof(Test1003A), Test1003A, test1003aDesc);

		}

		/* tests components */

		private bool shtChgDesc(string desc)
		{
			string proc = "CHANGE DESC";

			beginTest(proc);

			_sht.Desc = desc;

			endTest(proc);

			return endValidateTest(proc);
		}

		private bool shtChgOpSeq(string path)
		{
			string proc = "CHANGE OP SEQUENCE";

			beginTest(proc);

			_sht.OpSequence = path;

			endTest(proc);

			return endValidateTest(proc);
		}

		private bool shtChgUpdateRule(UpdateRules ur)
		{
			string proc = "CHANGE FILE PATH";

			beginTest(proc);

			_sht.UpdateRule = ur;

			endTest(proc);

			return endValidateTest(proc);
		}

		private bool shtAddFamAndType(string famName, string famType, string famProp, out FamAndType? fat)
		{
			string proc = "ADD FAM AND TYPE";

			beginTest(proc);

			fat = _sht.AddFamAndType(famName, famType, famProp);

			endTest(proc);

			return endValidateTest(proc);
		}

		private bool shtDelFamAndType(string key)
		{
			string proc = "REMOVE FAM AND TYPE";

			beginTest(proc);

			_sht.RemoveFamAndType(key);

			endTest(proc);

			return endValidateTest(proc);
		}

		private bool shtChgNameMod(string name)
		{
			string proc = "CHANGE NAME MOD";

			beginTest(proc);

			_sht.NameModified = name;

			endTest(proc);

			return endValidateTest(proc);
		}



		private bool shtCanApply(bool result)
		{
			R.WriteAnyway("\n >> can sheet apply >> ");

			if (_sht.ApplyBtnStatus)
			{
				R.WriteLineAnyway(" >> Can Apply - Continue >> ");
				return true;
			}

			R.WriteLineAnyway(" >> CANNOT APPLY - ABORT ");

			ShowTestCompletionResult(result);

			R.AddRouteExit("CANNOT APPLY - ABORT");

			return false;
		}

		private bool shtCanUndo(bool result)
		{
			R.WriteAnyway("\n >> can sheet undo >> ");

			if (_sht.UndoBtnStatus)
			{
				R.WriteLineAnyway(" >> Can Undo - Continue >> ");
				return true;
			}

			R.WriteLineAnyway(" >> CANNOT UNDO - ABORT ");

			ShowTestCompletionResult(result);

			R.AddRouteExit("CANNOT UNDO - ABORT");

			return false;
		}

		private bool shtCanEdit(bool result)
		{
			R.WriteAnyway("\n >> can sheet be edited >> ");

			if (!_sht.IsModifiedExo)
			{
				R.WriteLineAnyway(" >> Can edit sheet - Continue >> ");
				return true;
			}

			R.WriteLineAnyway(" >> CANNOT EDIT SHEET - ABORT ");

			ShowTestCompletionResult(result);

			R.AddRouteExit("CANNOT EDIT SHEET - ABORT");

			return false;
		}




		private bool shtUndoAll()
		{
			string proc = "UNDO ALL";

			beginTest(proc);

			_sht.UndoChangesAll();

			endTest(proc);

			return endValidateTest(proc);
		}

		private bool shtApplyAll()
		{
			string proc = "SHEET APPLY ALL";

			beginTest(proc);

			_sht.ApplyChangesAll();

			endTest(proc);

			return endValidateTest(proc);
		}



		/* test routines */

		/* 10x tests - change user field(s) */

		private string test1001aDesc = "change user field (desc)";
		public bool Test1001A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				V2.Ts2_ShtStdTestsA.SetTests([
					"Alt1", "T", "A",   
					"Alt1", "T", "T",  "Upd1", "T", "T",  "T", "T", "T"
				])
			]);

			uiTests2 = new ([
				//                                  A    B    C     D    E     F 
				V2.Ts2_ShtUiEndSequenceA.SetTests(["T", "F", "T",  "T", "T",  "T"]),
			]);

			startTest(testDesc[testId]);

			if (!shtCanEdit(true)) return false;

			result &= shtChgDesc(FAUX_SHT_DESC_ALT1);

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		private string test1001bDesc = "change user field (name mod)";
		public bool Test1001B(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				V2.Ts2_ShtStdTestsA.SetTests([
					"Init", "F", "N",   
					"Alt2", "T", "X",  "Upd1", "T", "T",  "T", "T", "T"
				])
			]);

			uiTests2 = new ([
				//                                  A    B    C     D    E     F 
				V2.Ts2_ShtUiEndSequenceA.SetTests(["F", "T", "T",  "T", "T",  "T"]),
			]);

			startTest(testDesc[testId]);

			if (!shtCanEdit(true)) return false;

			result &= shtChgNameMod(FAUX_USER_NAME_ALT2);
			
			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}


		private string test1002aDesc = "change user field (desc) then undo all";
		public bool Test1002A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				V2.Ts2_ShtStdTestsA.SetTests([
					"Alt1", "T", "A",   "Alt1", "T", "T",  "Upd1", "T", "T",  "T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsA.SetTests([
					"Init", "F", "N",   "Init", "F", "N",  "Init", "F", "N",  "F", "F", "F"
				]),
			]);

			uiTests2 = new ([
				//                                  A    B    C     D    E     F 
				V2.Ts2_ShtUiEndSequenceA.SetTests(["T", "F", "T",  "T", "T",  "T"]),
				V2.Ts2_ShtUiEndSequenceA.SetTests(["F", "F", "T",  "F", "F",  "F"]),
			]);

			startTest(testDesc[testId]);

			if (!shtCanEdit(true)) return false;

			result &= shtChgDesc(FAUX_SHT_DESC_ALT1);

			if (!shtCanUndo(true)) return false;

			result &= shtUndoAll();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		private string test1002bDesc = "change user field (name mod) then undo all";
		public bool Test1002B(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				V2.Ts2_ShtStdTestsA.SetTests([
					"Init", "F", "N",   "Alt2", "T", "X",  "Upd1", "T", "T",  "T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsA.SetTests([
					"Init", "F", "N",   "Init", "F", "N",  "Init", "F", "N",  "F", "F", "F"
				]),
			]);

			uiTests2 = new ([
				//                                  A    B    C     D    E     F 
				V2.Ts2_ShtUiEndSequenceA.SetTests(["F", "T", "T",  "T", "T",  "T"]),
				V2.Ts2_ShtUiEndSequenceA.SetTests(["F", "F", "T",  "F", "F",  "F"]),
			]);

			startTest(testDesc[testId]);

			if (!shtCanEdit(true)) return false;

			result &= shtChgNameMod(FAUX_USER_NAME_ALT2);

			if (!shtCanUndo(true)) return false;

			result &= shtUndoAll();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}



		private string test1003aDesc = "change user field (desc) then apply all";
		public bool Test1003A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				V2.Ts2_ShtStdTestsA.SetTests([
					"Alt1", "T", "A",   "Alt1", "T", "T",  "Upd1", "T", "T",  "T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsA.SetTests([
					"Alt1", "F", "N",   "Alt1", "F", "N",  "Upd1", "F", "N",  "F", "F", "F"
				]),
			]);

			uiTests2 = new ([
				//                                  A    B    C     D    E     F 
				V2.Ts2_ShtUiEndSequenceA.SetTests(["T", "F", "T",  "T", "T",  "T"]),
				V2.Ts2_ShtUiEndSequenceA.SetTests(["F", "F", "T",  "F", "F",  "F"]),
			]);

			startTest(testDesc[testId]);

			if (!shtCanEdit(true)) return false;

			result &= shtChgDesc(FAUX_SHT_DESC_ALT1);

			if (!shtCanApply(true)) return false;

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}


		/* 103x tests - change 3 user field(s) */

		private string test1031aDesc = "change user field (desc, sheet name, update rule)";
		public bool Test1031A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				V2.Ts2_ShtStdTestsB.SetTests([ // after test desc
					"Alt1", "T", "A",   
					"Init", "F", "N",   
					"Init", "F", "N",   
					"Alt1", "T", "T",  
					"Upd1", "T", "T",  
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsB.SetTests([ // after test ur
					"Alt1", "T", "A",   
					"Alt1", "T", "A",   
					"Init", "F", "N",   
					"Alt1", "T", "T",  
					"Upd1", "T", "T",  
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsB.SetTests([ // after test opseq
					"Alt1", "T", "A",   
					"Alt1", "T", "A",   
					"Alt1", "T", "A",   
					"Alt1", "T", "T",  
					"Upd1", "T", "T",  
					"T", "T", "T"
				]),
			]);

			uiTests2 = new ([
				//                                  A    B    C     D    E    F   G    H
				V2.Ts2_ShtUiEndSequenceB.SetTests(["T", "F", "F", "F", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceB.SetTests(["T", "T", "F", "F", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceB.SetTests(["T", "T", "T", "F", "T", "T", "T", "T"]),
			]);

			startTest(testDesc[testId]);

			if (!shtCanEdit(true)) return false;

			result &= shtChgDesc(FAUX_SHT_DESC_ALT1);

			result &= shtChgUpdateRule(FAUX_SHT_UPDATE_RULE_ALT1);

			result &= shtChgOpSeq(FAUX_SHT_OP_SEQ_ALT1);

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		private string test1032aDesc = "change user field (desc, sheet name, update rule) then undo all";
		public bool Test1032A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				V2.Ts2_ShtStdTestsB.SetTests([ // after test desc
					"Alt1", "T", "A",   
					"Init", "F", "N",   
					"Init", "F", "N",   
					"Alt1", "T", "T",  
					"Upd1", "T", "T",  
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsB.SetTests([ // after test ur
					"Alt1", "T", "A",   
					"Alt1", "T", "A",   
					"Init", "F", "N",   
					"Alt1", "T", "T",  
					"Upd1", "T", "T",  
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsB.SetTests([ // after test opseq
					"Alt1", "T", "A",   
					"Alt1", "T", "A",   
					"Alt1", "T", "A",   
					"Alt1", "T", "T",  
					"Upd1", "T", "T",  
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsB.SetTests([ // after test desc
					"Init", "F", "N",
					"Init", "F", "N",
					"Init", "F", "N",
					"Init", "F", "N",
					"Init", "F", "N",
					"F", "F", "F"
				]),
			]);

			uiTests2 = new ([
				//                                  A    B    C     D    E    F   G    H
				V2.Ts2_ShtUiEndSequenceB.SetTests(["T", "F", "F", "F", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceB.SetTests(["T", "T", "F", "F", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceB.SetTests(["T", "T", "T", "F", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceB.SetTests(["F", "F", "F", "F", "T", "F", "F", "F"]),
			]);

			startTest(testDesc[testId]);

			if (!shtCanEdit(true)) return false;

			result &= shtChgDesc(FAUX_SHT_DESC_ALT1);

			result &= shtChgUpdateRule(FAUX_SHT_UPDATE_RULE_ALT1);

			result &= shtChgOpSeq(FAUX_SHT_OP_SEQ_ALT1);

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}


		/* 121x tests - add fam and type */

		private string test1211aDesc = "add a fam and type";
		public bool Test1211A(string testId)
		{
			// _FamilyList (private) FamilyListField (public) (is dirty & chgsrc)
			// FamListHasElements
			// famlistwkg (dict<string, FamAndTtpe>)
			// FamAndType
			// is mod exo
			// IsModifiedFamListWkg
			// sheet status

			// tests after add


			// sheet status (validate_ShtStatEnum)
			// fam lst has key  (validate_ShtFatHasKeyBool)
			// fat is dirty (validate_ShtFatIsDirty)
			// fat CS (validate_ShtFatChgSrc)
			// fam lst wkg has key  (validate_ShtFatHasKeyWkgBool)

			// ui
			// fam lst wkg has new fat (true)  (sht.IsModifiedFamListWkg)

			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			FamAndType? fat;

			int famLstCount;
			int famLstWkgCount;

			tstId = testId;

			tests2 = new ([
				// pre-test values
				V2.Ts2_ShtStdTestsC.SetTests([
					"F", "N", "F", "Init", "Init",
					"4", "4",
					"Init", "F", "N",
					"Init", "F", "N",
					"F", "F", "F"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Init", "Init",
					"5", "5",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
			]);

			uiTests2 = new ([
				//                                  A     B     C     D     E
				V2.Ts2_ShtUiEndSequenceC.SetTests(null), // filler - not used
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T",  "T",  "T",  "T",  "T"]),
			]);

			startTest(testDesc[testId]);

			result = startPreTest();

			famLstCount = _sht.FamilyListCnt;
			famLstWkgCount = _sht.FamilyListWkgCnt;

			if (!shtCanEdit(true)) return false;

			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_INIT, 
				FAUX_SHT_FAM_TYPE_INIT, FAUX_SHT_FAM_PROP_INIT, out fat);

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		private string test1212aDesc = "add a fam and type, then undo";
		public bool Test1212A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			FamAndType? fat;

			int famLstCount;
			int famLstWkgCount;

			tstId = testId;

			tests2 = new ([
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Init", "Init",
					"4", "4",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"

				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"F", "N", "F", "None", "None",
					"4", "4",
					"Init", "F", "N",
					"Init", "F", "N",
					"F", "F", "F"
				]),
			]);

			uiTests2 = new ([
				//                                  A    B    C   D    E
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["F", "T", "F", "F", "F"]),
			]);

			startTest(testDesc[testId]);

			// Dictionary<string, SingleTest2> b = V2.TestParameters2;

			famLstCount = _sht.FamilyListCnt;
			famLstWkgCount = _sht.FamilyListWkgCnt;

			if (!shtCanEdit(true)) return false;

			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_INIT, 
				FAUX_SHT_FAM_TYPE_INIT, FAUX_SHT_FAM_PROP_INIT, out fat);

			if (!shtCanUndo(result)) return false;

			result &= shtUndoAll();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		private string test1213aDesc = "add a fam and type, then apply";
		public bool Test1213A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			FamAndType? fat;

			int famLstCount;
			int famLstWkgCount;

			tstId = testId;

			tests2 = new ([
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Init", "Init",
					"4", "4",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"

				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"F", "N", "F", "None", "None",
					"4", "4",
					"Alt1", "F", "N",
					"Upd1", "F", "N",
					"F", "F", "F"
				]),
			]);

			uiTests2 = new ([
				//                                  A    B    C   D    E
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["F", "T", "F", "F", "F"]),
			]);

			startTest(testDesc[testId]);

			// Dictionary<string, SingleTest2> b = V2.TestParameters2;

			famLstCount = _sht.FamilyListCnt;
			famLstWkgCount = _sht.FamilyListWkgCnt;

			if (!shtCanEdit(true)) return false;

			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_INIT, 
				FAUX_SHT_FAM_TYPE_INIT, FAUX_SHT_FAM_PROP_INIT, out fat);

			if (!shtCanUndo(result)) return false;

			result &= shtApplyAll();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		/* 131x tests - add 1+ fam and types / remove fam types */

		private string test1311aDesc = "add three fam and types";
		public bool Test1311A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				// pre-test values
				V2.Ts2_ShtStdTestsC.SetTests([
					"F", "N", "F", "Init", "Init",
					"4", "4",
					"Init", "F", "N",
					"Init", "F", "N",
					"F", "F", "F"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Init", "Init",
					"5", "5",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Alt1", "Alt1",
					"6", "6",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Alt2", "Alt2",
					"7", "7",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
			]);

			uiTests2 = new ([
				//                                  A    B    C   D    E
				V2.Ts2_ShtUiEndSequenceC.SetTests(null), // pre-test filler
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
			]);

			startTest(testDesc[testId]);

			result = startPreTest();

			if (!shtCanEdit(true)) return false;

			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_INIT,
				FAUX_SHT_FAM_TYPE_INIT, FAUX_SHT_FAM_PROP_INIT, out _);


			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_ALT1,
				FAUX_SHT_FAM_TYPE_ALT1, FAUX_SHT_FAM_PROP_ALT1, out _);


			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_ALT2,
				FAUX_SHT_FAM_TYPE_ALT2, FAUX_SHT_FAM_PROP_ALT2, out _);

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		private string test1312aDesc = "add three fam and types, then undo";
		public bool Test1312A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				// pre-test values
				V2.Ts2_ShtStdTestsC.SetTests([
					"F", "N", "F", "Init", "Init",
					"4", "4",
					"Init", "F", "N",
					"Init", "F", "N",
					"F", "F", "F"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Init", "Init",
					"5", "5",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Alt1", "Alt1",
					"6", "6",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Alt2", "Alt2",
					"7", "7",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"F", "N", "F", "InitF", "InitF",
					"4", "4",
					"Init", "F", "N",
					"Init", "F", "N",
					"F", "F", "F"
				]),
			]);

			uiTests2 = new ([
				//                                  A    B    C   D    E
				V2.Ts2_ShtUiEndSequenceC.SetTests(null), // pre-test filler
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["F", "T", "F", "F", "F"]),
			]);

			startTest(testDesc[testId]);

			result = startPreTest();

			if (!shtCanEdit(true)) return false;

			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_INIT,
				FAUX_SHT_FAM_TYPE_INIT, FAUX_SHT_FAM_PROP_INIT, out _);


			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_ALT1,
				FAUX_SHT_FAM_TYPE_ALT1, FAUX_SHT_FAM_PROP_ALT1, out _);


			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_ALT2,
				FAUX_SHT_FAM_TYPE_ALT2, FAUX_SHT_FAM_PROP_ALT2, out _);

			if (!shtCanUndo(result)) return false;

			result &= shtUndoAll();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		private string test1313aDesc = "add three fam and types, then apply";
		public bool Test1313A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				// pre-test values
				V2.Ts2_ShtStdTestsC.SetTests([
					"F", "N", "F", "Init", "Init",
					"4", "4",
					"Init", "F", "N",
					"Init", "F", "N",
					"F", "F", "F"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Init", "Init",
					"5", "5",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Alt1", "Alt1",
					"6", "6",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Alt2", "Alt2",
					"7", "7",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"F", "N", "F", "Alt2", "Alt2",
					"7", "7",
					"Alt1", "F", "N",
					"Upd1", "F", "N",
					"F", "F", "F"
				]),
			]);

			uiTests2 = new ([
				//                                  A    B    C   D    E
				V2.Ts2_ShtUiEndSequenceC.SetTests(null), // pre-test filler
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["F", "T", "F", "F", "F"]),
			]);

			startTest(testDesc[testId]);

			result = startPreTest();

			if (!shtCanEdit(true)) return false;

			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_INIT,
				FAUX_SHT_FAM_TYPE_INIT, FAUX_SHT_FAM_PROP_INIT, out _);


			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_ALT1,
				FAUX_SHT_FAM_TYPE_ALT1, FAUX_SHT_FAM_PROP_ALT1, out _);


			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_ALT2,
				FAUX_SHT_FAM_TYPE_ALT2, FAUX_SHT_FAM_PROP_ALT2, out _);

			if (!shtCanUndo(result)) return false;

			result &= shtApplyAll();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		private string test1314aDesc = "add three fam and types, delete one of the new after add all";
		public bool Test1314A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				// pre-test values
				V2.Ts2_ShtStdTestsC.SetTests([
					"F", "N", "F", "Init", "Init",
					"4", "4",
					"Init", "F", "N",
					"Init", "F", "N",
					"F", "F", "F"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Init", "Init",
					"5", "5",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Alt1", "Alt1",
					"6", "6",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Alt2", "Alt2",
					"7", "7",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),

				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Alt1F", "Alt1F",
					"6", "6",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
			]);

			uiTests2 = new ([
				//                                  A    B    C   D    E
				V2.Ts2_ShtUiEndSequenceC.SetTests(null), // pre-test filler
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
			]);

			startTest(testDesc[testId]);

			result = startPreTest();

			if (!shtCanEdit(true)) return false;

			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_INIT,
				FAUX_SHT_FAM_TYPE_INIT, FAUX_SHT_FAM_PROP_INIT, out _);


			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_ALT1,
				FAUX_SHT_FAM_TYPE_ALT1, FAUX_SHT_FAM_PROP_ALT1, out _);


			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_ALT2,
				FAUX_SHT_FAM_TYPE_ALT2, FAUX_SHT_FAM_PROP_ALT2, out _);

			result &= shtDelFamAndType(Faux_FatItemKey_Alt1);
			
			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		private string test1315aDesc = "add three fam and types, delete one of the new after added";
		public bool Test1315A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				// pre-test values
				V2.Ts2_ShtStdTestsC.SetTests([
					"F", "N", "F", "Init", "Init",
					"4", "4",
					"Init", "F", "N",
					"Init", "F", "N",
					"F", "F", "F"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Init", "Init",
					"5", "5",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Alt1", "Alt1",
					"6", "6",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Alt1F", "Alt1F",
					"5", "5",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),
				V2.Ts2_ShtStdTestsC.SetTests([
					"T", "D", "T", "Alt1F", "Alt1F",
					"6", "6",
					"Alt1", "T", "T",
					"Upd1", "T", "T",
					"T", "T", "T"
				]),

			]);

			uiTests2 = new ([
				//                                  A    B    C   D    E
				V2.Ts2_ShtUiEndSequenceC.SetTests(null), // pre-test filler
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceC.SetTests(["T", "T", "T", "T", "T"]),
			]);

			startTest(testDesc[testId]);

			result = startPreTest();

			if (!shtCanEdit(true)) return false;

			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_INIT,
				FAUX_SHT_FAM_TYPE_INIT, FAUX_SHT_FAM_PROP_INIT, out _);


			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_ALT1,
				FAUX_SHT_FAM_TYPE_ALT1, FAUX_SHT_FAM_PROP_ALT1, out _);

			result &= shtDelFamAndType(Faux_FatItemKey_Alt1);
			
			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_ALT2,
				FAUX_SHT_FAM_TYPE_ALT2, FAUX_SHT_FAM_PROP_ALT2, out _);


			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}


		/* 14x tests - change user field(s) + add fat */

		private string test1401aDesc = "change user field (desc) + add fam and type";
		public bool Test1401A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				// pre-test values
				V2.Ts2_ShtStdTestsD.SetTests([
					"Init", "F", "N",              // a, b, c
					"Init", "F", "N",              // d, e, f
					"Init", "F", "N",              // g, h, i
					"F", "N", "F", "InitF", "InitF", // j, k, l ,m, n
					"4", "4",                      // o, p
					"Init", "F", "N",              // q, r, s
					"Init", "F", "N",              // t, u, v
					"F", "F", "F"                  // w, x, y
				]),

				// after change desc
				V2.Ts2_ShtStdTestsD.SetTests([
					"Alt1", "T", "A",              // a, b, c
					"Init", "F", "N",              // d, e, f
					"Init", "F", "N",              // g, h, i
					"F", "N", "F", "InitF", "InitF", // j, k, l ,m, n
					"4", "4",                      // o, p
					"Alt1", "T", "T",              // q, r, s
					"Upd1", "T", "T",              // t, u, v
					"T", "T", "T"                  // w, x, y
				]),

				// after add sheet
				V2.Ts2_ShtStdTestsD.SetTests([
					"Alt1", "T", "A",  // a, b, c
					"Init", "F", "N",  // d, e, f
					"Init", "F", "N",  // g, h, i
					"T", "D", "T", "Init", "Init", // j, k, l ,m, n
					"5", "5",          // o, p
					"Alt1", "T", "T",  // q, r, s
					"Upd1", "T", "T",  // t, u, v
					"T", "T", "T"      // w, x, y
				]),
			]);

			uiTests2 = new ([
				//                                  A    B    C    D    E    F    G    H
				V2.Ts2_ShtUiEndSequenceD.SetTests(null),
				V2.Ts2_ShtUiEndSequenceD.SetTests(["T", "F", "F", "F", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceD.SetTests(["T", "F", "F", "T", "T", "T", "T", "T"]),

			]);

			startTest(testDesc[testId]);

			result = startPreTest();

			if (!shtCanEdit(true)) return false;

			result &= shtChgDesc(FAUX_SHT_DESC_ALT1);

			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_INIT,
				FAUX_SHT_FAM_TYPE_INIT, FAUX_SHT_FAM_PROP_INIT, out _);

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}

		private string test1402aDesc = "change user fields (desc, update rule) + add fam and type, undo all";
		public bool Test1402A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				// pre-test values
				V2.Ts2_ShtStdTestsD.SetTests([
					"Init", "F", "N",              // a, b, c
					"Init", "F", "N",              // d, e, f
					"Init", "F", "N",              // g, h, i
					"F", "N", "F", "InitF", "InitF", // j, k, l ,m, n
					"4", "4",                      // o, p
					"Init", "F", "N",              // q, r, s
					"Init", "F", "N",              // t, u, v
					"F", "F", "F"                  // w, x, y
				]),

				// after change desc
				V2.Ts2_ShtStdTestsD.SetTests([
					"Alt1", "T", "A",              // a, b, c
					"Init", "F", "N",              // d, e, f
					"Init", "F", "N",              // g, h, i
					"F", "N", "F", "InitF", "InitF", // j, k, l ,m, n
					"4", "4",                      // o, p
					"Alt1", "T", "T",              // q, r, s
					"Upd1", "T", "T",              // t, u, v
					"T", "T", "T"                  // w, x, y
				]),

				// after change update rule
				V2.Ts2_ShtStdTestsD.SetTests([
					"Alt1", "T", "A",              // a, b, c
					"Alt1", "T", "A",              // d, e, f
					"Init", "F", "N",              // g, h, i
					"F", "N", "F", "InitF", "InitF", // j, k, l ,m, n
					"4", "4",                      // o, p
					"Alt1", "T", "T",              // q, r, s
					"Upd1", "T", "T",              // t, u, v
					"T", "T", "T"                  // w, x, y
				]),

				// after add sheet
				V2.Ts2_ShtStdTestsD.SetTests([
					"Alt1", "T", "A",  // a, b, c
					"Alt1", "T", "A",  // d, e, f
					"Init", "F", "N",  // g, h, i
					"T", "D", "T", "Init", "Init", // j, k, l ,m, n
					"5", "5",          // o, p
					"Alt1", "T", "T",  // q, r, s
					"Upd1", "T", "T",  // t, u, v
					"T", "T", "T"      // w, x, y
				]),

				// after undo all
				V2.Ts2_ShtStdTestsD.SetTests([
					"Init", "F", "N",                // a, b, c
					"Init", "F", "N",                // d, e, f
					"Init", "F", "N",                // g, h, i
					"F", "N", "F", "InitF", "InitF", // j, k, l ,m, n
					"4", "4",                        // o, p
					"Init", "F", "N",                // q, r, s
					"Init", "F", "N",                // t, u, v
					"F", "F", "F"                    // w, x, y
				]),
			]);

			uiTests2 = new ([
				//                                  A    B    C    D    E    F    G    H
				V2.Ts2_ShtUiEndSequenceD.SetTests(null),
				V2.Ts2_ShtUiEndSequenceD.SetTests(["T", "F", "F", "F", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceD.SetTests(["T", "T", "F", "F", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceD.SetTests(["T", "T", "F", "T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceD.SetTests(["F", "F", "F", "F", "T", "F", "F", "F"]),

			]);

			startTest(testDesc[testId]);

			result = startPreTest();

			if (!shtCanEdit(true)) return false;

			result &= shtChgDesc(FAUX_SHT_DESC_ALT1);

			result &= shtChgUpdateRule(FAUX_SHT_UPDATE_RULE_ALT1);

			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_INIT,
				FAUX_SHT_FAM_TYPE_INIT, FAUX_SHT_FAM_PROP_INIT, out _);

			result &= shtUndoAll();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}


		private string test1403aDesc = "change user fields (desc, update rule) + add fam and type, apply all";
		public bool Test1403A(string testId)
		{
			// remember to register routine
			R.AddRouteEnter();
			bool result = true;

			tstId = testId;

			tests2 = new ([
				// pre-test values
				V2.Ts2_ShtStdTestsD.SetTests([
					"Init", "F", "N",              // a, b, c
					"Init", "F", "N",              // d, e, f
					"Init", "F", "N",              // g, h, i
					"F", "N", "F", "InitF", "InitF", // j, k, l ,m, n
					"4", "4",                      // o, p
					"Init", "F", "N",              // q, r, s
					"Init", "F", "N",              // t, u, v
					"F", "F", "F"                  // w, x, y
				]),

				// after change desc
				V2.Ts2_ShtStdTestsD.SetTests([
					"Alt1", "T", "A",              // a, b, c
					"Init", "F", "N",              // d, e, f
					"Init", "F", "N",              // g, h, i
					"F", "N", "F", "InitF", "InitF", // j, k, l ,m, n
					"4", "4",                      // o, p
					"Alt1", "T", "T",              // q, r, s
					"Upd1", "T", "T",              // t, u, v
					"T", "T", "T"                  // w, x, y
				]),

				// after change update rule
				V2.Ts2_ShtStdTestsD.SetTests([
					"Alt1", "T", "A",              // a, b, c
					"Alt1", "T", "A",              // d, e, f
					"Init", "F", "N",              // g, h, i
					"F", "N", "F", "InitF", "InitF", // j, k, l ,m, n
					"4", "4",                      // o, p
					"Alt1", "T", "T",              // q, r, s
					"Upd1", "T", "T",              // t, u, v
					"T", "T", "T"                  // w, x, y
				]),

				// after add sheet
				V2.Ts2_ShtStdTestsD.SetTests([
					"Alt1", "T", "A",  // a, b, c
					"Alt1", "T", "A",  // d, e, f
					"Init", "F", "N",  // g, h, i
					"T", "D", "T", "Init", "Init", // j, k, l ,m, n
					"5", "5",          // o, p
					"Alt1", "T", "T",  // q, r, s
					"Upd1", "T", "T",  // t, u, v
					"T", "T", "T"      // w, x, y
				]),

				// after undo all
				V2.Ts2_ShtStdTestsD.SetTests([
					"Alt1", "F", "N",                // a, b, c
					"Alt1", "F", "N",                // d, e, f
					"Init", "F", "N",                // g, h, i
					"F", "N", "F", "Init", "Init", // j, k, l ,m, n
					"5", "5",                        // o, p
					"Alt1", "F", "N",                // q, r, s
					"Upd1", "F", "N",                // t, u, v
					"F", "F", "F"                    // w, x, y
				]),
			]);

			uiTests2 = new ([
				//                                  A    B    C    D    E    F    G    H
				V2.Ts2_ShtUiEndSequenceD.SetTests(null),
				V2.Ts2_ShtUiEndSequenceD.SetTests(["T", "F", "F", "F", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceD.SetTests(["T", "T", "F", "F", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceD.SetTests(["T", "T", "F", "T", "T", "T", "T", "T"]),
				V2.Ts2_ShtUiEndSequenceD.SetTests(["F", "F", "F", "F", "T", "F", "F", "F"]),

			]);

			startTest(testDesc[testId]);

			result = startPreTest();

			if (!shtCanEdit(true)) return false;

			result &= shtChgDesc(FAUX_SHT_DESC_ALT1);

			result &= shtChgUpdateRule(FAUX_SHT_UPDATE_RULE_ALT1);

			result &= shtAddFamAndType(FAUX_SHT_FAM_NAME_INIT,
				FAUX_SHT_FAM_TYPE_INIT, FAUX_SHT_FAM_PROP_INIT, out _);

			result &= shtApplyAll();

			ShowTestCompletionResult(result);

			R.AddRouteExit();

			return result;
		}


	}
}