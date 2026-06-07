
using ExStorSys;
using static ExStorSys.ExStorConstFaux;
using UtilityLibrary;


// user name: jeffs
// created:   5/23/2026 9:34:23 AM

namespace ProcessTests1
{

	public struct TestResult
	{

		public string [] WbkBoolTitles = new []
		{
			"is mod", "apply btn", "undo btn",
			"desc eq", "mod name eq", "mod name ChgSrc",
			"mod date eq", "mod date ChgSrc"
		};

		public string [] ShtBoolTitles = new []
		{
			"shts lst apply", "shts lst undo", "last id eq"
		};

		public string TestName { get; set; }

		public string[] WbkTestStrings { get; set; }
		public bool[] WbkTestBools { get; set; }
		public ChgSrcId[] WbkTestChgSrc { get; set; }

		public string[] ShtTestStrings { get; set; }
		public bool[] ShtTestBools { get; set; }

		// public string S1 { get; set; }
		// public string S2 { get; set; }
		// public string S3 { get; set; }
		//
		// public bool B1 { get; set; }
		// public bool B2 { get; set; }
		// public bool B3 { get; set; }
		// public bool B4 { get; set; }
		// public bool B5 { get; set; }
		// public bool B6 { get; set; }
		// public bool B7 { get; set; }

		// public static string[] TestDesc { get; set; }

		public TestResult(string testName,
			string[] wbkTestStrings, bool[] wbkTestBools, ChgSrcId[] wbkTestChgSrc)
		{
			TestName = testName;
			WbkTestStrings = wbkTestStrings;
			WbkTestBools = wbkTestBools;
			WbkTestChgSrc = wbkTestChgSrc;
		}

		public TestResult(string testName,
			string[] wbkTestStrings, bool[] wbkTestBools, ChgSrcId[] wbkTestChgSrc,
			string[] shtTestStrings, bool[] shtTestBools)
		{
			TestName = testName;
			WbkTestStrings = wbkTestStrings;
			WbkTestBools = wbkTestBools;
			WbkTestChgSrc = wbkTestChgSrc;

			ShtTestStrings = shtTestStrings;
			ShtTestBools = shtTestBools;

		}

	}

	public abstract class ATests
	{
		protected static int shtIdx = 1;

		protected string tstId = "";
		protected string tstDesc = "";
		protected int tstAnsIdx;

		protected List<Tuple<int, string[]>> testResults;

		protected List<Tuple<string, List<Tuple<string, bool, dynamic>>>> tests;

		protected int tstNameModIdx;
		protected string[] tstNamMod = new string[1];

		protected bool doingShtLst;

		protected bool stopTestingOnError = false;

		protected string modStatusBeg;

		protected string nameModifiedOrig = "";
		protected string dateModifiedOrig = "";

		protected ExStorData _xData;

		protected Sheet _sht;
		protected Exid _exid;
		protected Tests1 t1;

		protected Validate _V;

		// ReSharper disable once InconsistentNaming
		protected WorkBook _wbk => _xData.WorkBook;

		public bool ShowWbkOverRideControl { get; set; } = true;

		protected void init()
		{
			R.AddRouteEnter(null, 1);

			t1 = Program.t1;

			t1.init();

			R.WriteLine("*******  init Test3 *******");

			UseFauxUserName();
			UseFauxModDate();

			tstNameModIdx = -1;

			_xData = ExStorData.Instance;

			_exid = new();

			_xData.RemovePlaceHolderSheet();


			doingShtLst = false;

			UseAltUserName();
			UseUpd1ModDate();

			R.AddRouteExit(null, 1);
		}

		protected void initCreateSheets()
		{
			R.AddRouteEnter(null, 1);

			R.WriteLine("*******  init create sheets test2 *******");

			UseFauxUserName();
			UseFauxModDate();

			_xData.AddSheetPreInit(t1.CreateSheetStealth());

			_sht = t1.CreateSheetStealth();

			_xData.AddSheetPreInit(_sht);

			_xData.SelectSheet = _sht.DsName;

			Sheet s = _xData.CurrentSheet;

			Sheet sht = t1.CreateSheetStealth();

			_xData.AddSheetPreInit(sht);

			s = _xData.CurrentSheet;

			_V = new Validate(_wbk, _sht, _xData);

			_xData.WorkBook.LastIdField.ApplyChg();

			addFamAndTypeStealth("family name 1", "type name 1", "props 1");
			addFamAndTypeStealth("family name 2", "type name 2", "props 2");
			addFamAndTypeStealth("family name 3", "type name 3", "props 3");
			addFamAndTypeStealth("family name 4", "type name 4", "props 4");

			bool rs = R.RunSilent;
			R.RunSilent = true;

			_sht.FamAndTypeApplyChanges();
			_sht.ApplyChange(_sht.FamilyListField, true);

			_sht.ModName_Undo(true);
			_sht.ModDate_Undo(true);

			_sht.IsModifiedExo = false;

			R.RunSilent = rs;

			UseAltUserName();
			UseUpd1ModDate();

			startTest();

			R.AddRouteExit(null, 1);
		}

		protected abstract void startTest();

		public void Reset()
		{
			R.AddRouteEnter(null, 1);

			R.WriteLine("\n****\nTEST3 perform complete date RESET\n****");

			ExStorData.Instance.ResetAll();

			init();

			initCreateSheets();

			R.AddRouteExit(null, 1);
		}

		protected void register(string name, Func<string, bool> f)
		{
			jumpTable.Add(name, f);
		}

		protected Dictionary<string, Func<string, bool>> jumpTable = new ();

		public bool RunOneTest(string testId, bool runSilent)
		{
			R.AddRouteEnter(null, 1);

			bool? result = null;

			if (runSilent) R.RunSilent = true;

			// R.AddRouteExit();
			// R.ShowRoute();

			Reset();

			R.NewLine();
			R.WriteAnyway($" *********\nrun test {testId} => ");

			Func<string, bool>? method = null;

			if (jumpTable.TryGetValue(testId, out method))
			{
				R.AddRouteEnter($"ENTER TEST {testId}", 1);

				result = method(testId);

				R.AddRouteExit($"EXIT TEST {testId}", 1);
			}

			bool saveSilent = R.RunSilent;

			R.RunSilent = false;

			if (!result.HasValue)
			{
				R.AddRoute("**** FAIL ***** TEST NOT FOUND", 1);
				R.WriteLine($"\n\n**** FAIL ***** TEST NOT FOUND\n");
			}
			else
			{
				string answer = result == true ? "PASS" : "FAIL";

				R.AddRoute($"test RESULTS | {answer}", 1);
				R.WriteLine($"\ntest RESULTS | {answer}\n\n*********\n");

			}

			R.AddRouteExit(null, 1);

			R.RunSilent = saveSilent;

			R.NewLine();

			return result == true;
		}

		/* show / utility routines */

		public Sheet CreateSheet()
		{
			R.AddRouteEnter();

			Sheet sht = Sheet.CreateSheet(_exid.CreateShtDsName(_wbk.GetId()),
				new ($"path:\\file{shtIdx}.xls", $"no sheet name {shtIdx}"));

			sht.SetTrackChanges();

			shtIdx++;

			R.AddRouteExit();

			return sht;
		}

		/* show and utility */

		protected void addFamAndTypeStealth(string fn, string tn, string p)
		{
			string key = ExStorLib.FormatFamAndType(fn, tn);

			FamAndType fat = FamAndType.GetNewItem(fn, tn, p);

			fat.IsNewItemFat = false;

			_sht.FamListWkg.Add(key, fat);
		}

	}
}
