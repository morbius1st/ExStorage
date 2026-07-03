
using ExStorSys;
using static ExStorSys.ExStorConstFaux;
using UtilityLibrary;


// user name: jeffs
// created:   5/23/2026 9:34:23 AM

namespace ProcessTests2
{
	public interface ITests
	{
		public bool ShowWbkOverRideControl { get; set; }
		public bool RunOneTestSeq(string testId, bool runSilent);

		public List<string>? RunTheseTests { get; set; } 
		public string OneTestId { get; set; }
	}


	public abstract class ATestsWbk : ATestsCommon
	{
		// protected static int shtIdx = 1;
		//
		// protected string tstId = "";
		// protected string tstDesc = "";
		// protected int tstAnsIdx;
		//
		// protected List<Tuple<int, string[]>> testResults;
		//
		// public List<TestSequence2> tests2;
		// public List<TestSequence2>? uiTests2;
		//
		// protected int tstNameModIdx;
		// protected string[] tstNamMod = new string[1];
		//
		// protected bool doingShtLst;
		//
		// protected bool stopTestingOnError = false;
		//
		// protected ExStorData _xData;
		//
		// protected Sheet _sht;
		// protected Exid _exid;
		//
		// protected Validate2 _V2;
		//
		// // ReSharper disable once InconsistentNaming
		// protected WorkBook _wbk => _xData.WorkBook;
		//
		// public bool ShowWbkOverRideControl { get; set; } = true;
		//
		// protected void init()
		// {
		// 	R.AddRouteEnter(null, 2);
		//
		// 	R.WriteLine("*******  init Test3 *******");
		//
		// 	UseFauxUserName();
		// 	UseFauxModDate();
		//
		// 	tstNameModIdx = -1;
		//
		// 	_xData = ExStorData.Instance;
		// 	_xData.WorkBook = WorkBook.CreateNewWorkBook();
		// 	_xData.WorkBook.SetTrackChanges();
		// 	_exid = new();
		// 	_xData.RemovePlaceHolderSheet();
		//
		// 	doingShtLst = false;
		//
		// 	UseAltUserName();
		// 	UseUpd1ModDate();
		//
		// 	R.AddRouteExit(null, 2);
		// }
		//
		// protected void initCreateSheets()
		// {
		// 	R.AddRouteEnter(null, 2);
		//
		// 	R.WriteLine("*******  init create sheets test2 *******");
		//
		// 	UseFauxUserName();
		// 	UseFauxModDate();
		//
		// 	_xData.AddSheetPreInit(CreateSheetStealth());
		//
		// 	_sht = CreateSheetStealth();
		//
		// 	_xData.AddSheetPreInit(_sht);
		//
		// 	_xData.SelectSheet = _sht.DsName;
		//
		// 	Sheet s = _xData.CurrentSheet;
		//
		// 	Sheet sht = CreateSheetStealth();
		//
		// 	_xData.AddSheetPreInit(sht);
		//
		// 	s = _xData.CurrentSheet;
		//
		// 	_V2 = new Validate2(_wbk, _sht, _xData);
		//
		// 	_xData.WorkBook.LastIdField.ApplyChg();
		//
		// 	addFamAndTypeStealth("family name 1", "type name 1", "props 1");
		// 	addFamAndTypeStealth("family name 2", "type name 2", "props 2");
		// 	addFamAndTypeStealth("family name 3", "type name 3", "props 3");
		// 	addFamAndTypeStealth("family name 4", "type name 4", "props 4");
		//
		// 	bool rs = R.RunSilent;
		// 	R.RunSilent = true;
		//
		// 	_sht.ApplyFamAndTypeChanges();
		// 	_sht.ApplyChange(_sht.FamilyListField, true);
		//
		// 	_sht.ModName_Undo();
		// 	_sht.ModDate_Undo();
		//
		// 	_sht.IsModifiedExo = false;
		//
		// 	R.RunSilent = rs;
		//
		// 	UseAltUserName();
		// 	UseUpd1ModDate();
		//
		// 	startTest();
		//
		// 	R.AddRouteExit(null, 2);
		// }

		// public void Reset()
		// {
		// 	R.AddRouteEnter(null, 2);
		//
		// 	R.WriteLine("\n****\nTEST3 perform complete date RESET\n****");
		//
		// 	ExStorData.Instance.ResetAll();
		//
		// 	init();
		//
		// 	initCreateSheets();
		//
		// 	R.AddRouteExit(null, 2);
		// }

		// protected void register(string name, Func<string, bool> f, string desc)
		// {
		// 	jumpTable.Add(name, f);
		//
		// 	testDesc.Add(name, desc);
		// }
		//
		// protected Dictionary<string, Func<string, bool>> jumpTable = new ();
		//
		// protected Dictionary<string, string> testDesc = new Dictionary<string, string>();

		public bool RunOneTestSeq(string testId, bool runSilent)
		{
			R.AddRouteEnter(routeIdx: 1);

			R.AddRouteEnter(null, 2);

			bool? result = null;

			if (runSilent) R.RunSilent = true;

			Reset();

			R.NewLine();
			R.WriteAnyway($" *********\nrun test {testId} => ");

			Func<string, bool>? method = null;

			if (jumpTable.TryGetValue(testId, out method))
			{
				R.AddRouteEnter($"ENTER TEST {testId}", 2);

				result = method(testId);

				R.AddRouteExit($"EXIT TEST {testId}", 2);
			}

			int a = tstAnsIdx;

			if (!result.HasValue)
			{
				R.AddRoute("**** FAIL ***** TEST NOT FOUND", 2);
				R.WriteLineAnyway($"\n\n**** FAIL ***** TEST NOT FOUND\n");
			}
			else
			{
				string answer; // = result == true ? "PASS" : "FAIL";

				answer = getAnswer(result.Value, 5);

				R.AddRoute($"test RESULT SUMMARY | {answer}", 2);
				R.WriteLineAnyway($"\ntest RESULT SUMMARY | {answer}\n*********\n");
			}

			R.AddRouteExit(null, 2);

			R.NewLine();

			R.AddRouteExit(routeIdx: 1);

			return result == true;
		}

		/* show / utility routines */

		// public Sheet CreateSheet()
		// {
		// 	// R.AddRouteEnter(routeIdx: 2);
		//
		// 	Sheet sht = Sheet.CreateSheet(_exid.CreateShtDsName(_wbk.GetId()),
		// 		new ($"path:\\file{shtIdx}.xls", $"no sheet name {shtIdx}"));
		//
		// 	sht.SetTrackChanges();
		//
		// 	shtIdx++;
		//
		// 	// R.AddRouteExit(routeIdx: 2);
		//
		// 	return sht;
		// }
		//
		// public Sheet CreateSheetStealth()
		// {
		// 	// R.AddRouteEnter(routeIdx: 2);
		// 	Sheet sht = Sheet.CreateSheet(_exid.CreateShtDsName(_wbk.GetIdStealth()),
		// 		new ($"path:\\file{shtIdx}.xls", $"no sheet name {shtIdx}"));
		//
		// 	sht.SetTrackChanges();
		//
		// 	shtIdx++;
		//
		// 	// R.AddRouteExit(routeIdx: 2);
		//
		// 	return sht;
		// }


		/* show and utility */

		// protected void addFamAndTypeStealth(string fn, string tn, string p)
		// {
		// 	// R.AddRouteEnter(routeIdx: 2);
		// 	string key = ExStorLib.FormatFamAndType(fn, tn);
		//
		// 	FamAndType fat = FamAndType.GetNewItem(fn, tn, p);
		//
		// 	fat.IsNewItemFat = false;
		//
		// 	_sht.FamListWkg.Add(key, fat);
		//
		// 	// R.AddRouteExit(routeIdx: 2);
		// }

		// wbk specific
		
		protected override void testPreface()
		{
			R.AddRoute();

			R.NewLine();
			R.WriteAnyway($"START TESTS WBK 1 ");
			R.NewLine();

			R.WriteLine($"\n*** Current user is {FauxUserName}\n");
			R.WriteLine($"*** IsModExo [ {AnySht.IsModifiedExo} ] | IsModFamLst [ {AnySht.IsModifiedFamListWkg} ]");

			showShtLst();

			ShowWbk.ShowWorkbookFields();
			ShowSht.ShowSheetFields();
			ShowSht.ShowFamList();

			testResults = new ();
		}

		protected override void startTest(string desc)
		{

			string nametst = "";
			string nameWbk = "";
			bool nameStatus;
			string nameResult = "";

			tstAnsIdx = 0;
			tstDesc = desc;

			R.WriteAnyway($"{tstId} | START | {tstDesc}");
			R.WriteOnlyWhenSilent("\n");

			showShtLst();
			ShowWbk.ShowWorkbookFields();

			if (tstNameModIdx == 0)
			{
				nametst = tstNamMod[tstNameModIdx++];
				nameWbk = _wbk.NameModified;
				nameStatus = nametst.Equals(nameWbk);
				nameResult = nameStatus ? "they MATCH" : "they do NOT match";

				R.WriteLine($"\n\tdoes actual {nameWbk} equal test {nametst}? {nameResult}");
			}

			R.NewLine();
		}

		// protected void wbkBeginTest(string proc)
		// {
		// 	testResults.Add(new(tstAnsIdx, [$"{tstId} | {proc}", ""]));
		//
		// 	R.StartRoute(0, $"**** run test | part [ {tstAnsIdx} ] | {tstId}");
		// 	R.AddRouteEnter($"begin {proc}");
		//
		// 	R.NewLine();
		// 	R.Write($"****************************\n");
		// 	R.WriteAnyway($"{$"[ {tstAnsIdx} ]",-5} {tstId} | ");
		// 	// R.NewLine();
		// 	R.Write($"BEGIN ");
		// 	R.WriteAnyway($"{proc, -30}");
		// 	R.WriteOnlyWhenSilent(" >> ");
		// 	R.NewLine();
		// 	R.NewLine();
		//
		// }

		// protected override void endTest(string proc)
		// {
		// 	R.AddRouteExit();
		// 	R.WriteLine($"\nAFTER {proc}");
		// 	endTestInfo();
		// 	R.ShowRoute();
		// }

		protected override void endTestInfo()
		{
			showShtLst();
			ShowWbk.ShowWorkbookFields();
		}

		// protected bool endValidateTest(string proc)
		// {
		// 	string answer1;
		//
		// 	R.WriteLine($"****************************");
		// 	R.WriteLine($"TEST VALIDATE | {$"[ {tstAnsIdx} ]",-5} | {tstId} | {proc} ");
		// 	R.WriteLine($"\nTEST VALIDATE | workbook test result  ");
		// 	
		// 	bool result = _V2.ValidateTests2(stopTestingOnError, tests2[tstAnsIdx]);
		// 	bool result2;
		//
		// 	R.WriteLine($"\n***** >> {getAnswer(result, 4)}");
		//
		// 	if (uiTests2 != null && uiTests2.Count == tests2.Count &&
		// 		uiTests2[tstAnsIdx].Choices != null!)
		// 	{
		// 		R.WriteLine($"\nui tests\n");
		//
		// 		result2 = _V2.ValidateTests2(stopTestingOnError, uiTests2[tstAnsIdx], 2);
		//
		// 		result &= result2;
		//
		// 		answer1 = getAnswer(result, 4);
		// 		answer1 += result2 ? $" ( ui tests worked )" : " ( ui tests failed )";
		// 	}
		// 	else
		// 	{
		// 		answer1 = getAnswer(result, 4);
		// 		answer1 += " ( no ui tests) ";
		// 	}
		//
		// 	testResults[tstAnsIdx].Item2[1] = answer1;
		//
		// 	R.Write("\n***** >> ");
		//
		// 	R.WriteAnyway($"{answer1} ");
		// 	R.NewLineAnyway();
		// 	R.NewLine();
		//
		// 	
		//
		// 	tstAnsIdx++;
		//
		// 	return result;
		// }

		// protected bool uiValidateTest(string title, List<string> testOpts)
		// {
		// 	R.WriteLine(title);
		//
		// 	bool result = _V2.ValidateTests2(stopTestingOnError,
		// 		_V2.Ts2_UiEndSequenceA.SetTests(testOpts), 2);
		//
		// 	R.WriteLine($"\n***** >> {getAnswer(result, 4)}\n");
		//
		// 	return result;
		// }

		// protected void ShowTestCompletionResult(bool result)
		// {
		// 	string r = result ? "WORKED" : "FAIL";
		//
		// 	R.WriteLine("\n****************************");
		// 	R.WriteLine(tstDesc);
		// 	R.WriteLine($"{tstId} | {r}");
		// 	R.WriteLine("****************************\n");
		//
		// 	showTestResultSummary();
		//
		// }
		//
		// protected void showTestResultSummary()
		// {
		// 	if (testResults == null || testResults.Count == 0) return;
		//
		// 	R.WriteLine("**********\nTEST RESULT SUMMARY\n");
		//
		// 	foreach ((int idx, string[] item2) in testResults)
		// 	{
		// 		R.WriteLine($"{$"[ {idx} ]",-7} |  {item2[1],-7} ( {item2[0]} )");
		// 	}
		//
		// 	R.WriteLine("\n**********");
		// }

		protected void showLastIdStatusWbk(string location)
		{
			string status = XData.WorkBook.LastIdField.IsDirty() ? "is Dirty" : "is Clean";

			R.WriteLine($"\n\t*** {location} | LastId | [ {XData.WorkBook.LastId} ] | status [ {status} ] | chg src id [ {XData.WorkBook.LastIdField.ChgSrc} ]\n");
		}

		protected void showShtLst()
		{
			if (!doingShtLst) return;

			ShowWbk.ShowShtsLst();

			R.NewLine();
		}

		// /// <summary>
		// /// convert a bool into a good answer string<br/>
		// /// type<br/>
		// /// 0 = yes / no<br/>
		// /// 1 = does match / does not match<br/>
		// /// 2 = match / not match<br/>
		// /// 3 = correct / wrong<br/>
		// /// 4 = worked / failed<br/>
		// /// 5 = pass / fail
		// /// </summary>
		// protected string getAnswer(bool result, int type)
		// {
		// 	string [,] answers = new [,] 
		// 		{
		// 			{ "Yes", "No" },					// 0
		// 			{ "DOES Match", "does NOT match" }, // 1
		// 			{ "Match", "NOT match" },			// 2
		// 			{ "Correct", "Wrong" },				// 3
		// 			{ "WORKED", "FAILED" },				// 4
		// 			{ "PASS", "FAIL" },					// 5
		// 		};
		//
		// 	if (type < 0 || type > answers.GetLength(0)) return $"wrong type ( {type} ) provided to get answer ";
		//
		// 	return answers[type, result ? 0 : 1];
		// }

	}

	// public struct TestResult
	// {
	//
	// 	public string [] WbkBoolTitles = new []
	// 	{
	// 		"is mod", "apply btn", "undo btn",
	// 		"desc eq", "mod name eq", "mod name ChgSrc",
	// 		"mod date eq", "mod date ChgSrc"
	// 	};
	//
	// 	public string [] ShtBoolTitles = new []
	// 	{
	// 		"shts lst apply", "shts lst undo", "last id eq"
	// 	};
	//
	// 	public string TestName { get; set; }
	//
	// 	public string[] WbkTestStrings { get; set; }
	// 	public bool[] WbkTestBools { get; set; }
	// 	public ChgSrcId[] WbkTestChgSrc { get; set; }
	//
	// 	public string[] ShtTestStrings { get; set; }
	// 	public bool[] ShtTestBools { get; set; }
	//
	// 	// public string S1 { get; set; }
	// 	// public string S2 { get; set; }
	// 	// public string S3 { get; set; }
	// 	//
	// 	// public bool B1 { get; set; }
	// 	// public bool B2 { get; set; }
	// 	// public bool B3 { get; set; }
	// 	// public bool B4 { get; set; }
	// 	// public bool B5 { get; set; }
	// 	// public bool B6 { get; set; }
	// 	// public bool B7 { get; set; }
	//
	// 	// public static string[] TestDesc { get; set; }
	//
	// 	public TestResult(string testName,
	// 		string[] wbkTestStrings, bool[] wbkTestBools, ChgSrcId[] wbkTestChgSrc)
	// 	{
	// 		TestName = testName;
	// 		WbkTestStrings = wbkTestStrings;
	// 		WbkTestBools = wbkTestBools;
	// 		WbkTestChgSrc = wbkTestChgSrc;
	// 	}
	//
	// 	public TestResult(string testName,
	// 		string[] wbkTestStrings, bool[] wbkTestBools, ChgSrcId[] wbkTestChgSrc,
	// 		string[] shtTestStrings, bool[] shtTestBools)
	// 	{
	// 		TestName = testName;
	// 		WbkTestStrings = wbkTestStrings;
	// 		WbkTestBools = wbkTestBools;
	// 		WbkTestChgSrc = wbkTestChgSrc;
	//
	// 		ShtTestStrings = shtTestStrings;
	// 		ShtTestBools = shtTestBools;
	//
	// 	}
	//
	// }

}
