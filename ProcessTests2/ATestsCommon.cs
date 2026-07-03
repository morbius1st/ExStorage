using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExStorSys;
using UtilityLibrary;
using static ExStorSys.ExStorConstFaux;


// user name: jeffs
// created:   5/31/2026 5:17:51 PM

namespace ProcessTests2
{
	public abstract class ATestsCommon
	{
		protected static int shtIdx = 1;

		protected string tstId = "";
		protected string tstDesc = "";
		protected int tstAnsIdx;

		protected List<Tuple<int, string[]>> testResults;

		public List<TestSequence2> tests2;
		public List<TestSequence2>? uiTests2;

		protected int tstNameModIdx;
		protected string[] tstNamMod = new string[1];

		protected bool doingShtLst;

		protected bool stopTestingOnError = false;

		protected string modStatusBeg;

		protected string nameModifiedOrig = "";
		protected string dateModifiedOrig = "";

		public string FatItemKey1;
		public string FatItemKey2;
		public string FatItemKey3;
		public string FatItemKey4;

		#pragma warning disable CS8618
		protected ExStorData XData;
		protected Sheet AnySht;
		private Exid exid;
		protected Validate2 V2;
		#pragma warning restore CS8618


		// ReSharper disable once InconsistentNaming
		protected WorkBook _wbk => XData.WorkBook;

		// ReSharper disable once InconsistentNaming
		protected Sheet _sht => XData.CurrentSheet!;

		public bool ShowWbkOverRideControl { get; set; } = true;

		protected void init()
		{
			R.AddRouteEnter(null, 2);

			R.WriteLine("*******  init Tests *******");

			UseFauxUserName();
			UseFauxModDate();

			tstNameModIdx = -1;

			XData = ExStorData.Instance;
			XData.WorkBook = WorkBook.CreateNewWorkBook();
			XData.WorkBook.SetTrackChanges();
			exid = new();
			XData.RemovePlaceHolderSheet();

			doingShtLst = false;

			UseAltUserName();
			UseUpd1ModDate();

			R.AddRouteExit(null, 2);
		}

		protected void initCreateSheets()
		{
			R.AddRouteEnter(null, 2);

			R.WriteLine("*******  init create sheets ATestsCommon *******");

			UseFauxUserName();
			UseFauxModDate();

			XData.AddSheetPreInit(CreateSheetStealth());

			AnySht = CreateSheetStealth();

			XData.AddSheetPreInit(AnySht);

			XData.SelectSheet = AnySht.DsName;

			Sheet s = XData.CurrentSheet;

			Sheet sht = CreateSheetStealth();

			XData.AddSheetPreInit(sht);

			s = XData.CurrentSheet;

			V2 = new Validate2(_wbk, AnySht, XData);

			XData.WorkBook.LastIdField.ApplyChg();

			addFamAndTypeStealth("family name 1", "type name 1", "props 1");
			FatItemKey1 = ExStorLib.FormatFamAndType("family name 1", "type name 1");

			addFamAndTypeStealth("family name 2", "type name 2", "props 2");
			FatItemKey1 = ExStorLib.FormatFamAndType("family name 2", "type name 2");

			addFamAndTypeStealth("family name 3", "type name 3", "props 3");
			FatItemKey1 = ExStorLib.FormatFamAndType("family name 3", "type name 3");

			addFamAndTypeStealth("family name 4", "type name 4", "props 4");
			FatItemKey1 = ExStorLib.FormatFamAndType("family name 4", "type name 4");

			bool rs = R.RunSilent;
			R.RunSilent = true;

			AnySht.ApplyFamAndTypeChanges();
			AnySht.ApplyChange(AnySht.FamilyListField, true);

			AnySht.ModName_Undo();
			AnySht.ModDate_Undo();

			AnySht.IsModifiedExo = false;

			R.RunSilent = rs;

			UseAltUserName();
			UseUpd1ModDate();

			testPreface();

			R.AddRouteExit(null, 2);
		}

		protected abstract void testPreface();

		protected abstract void startTest(string desc);

		public void Reset()
		{
			R.AddRouteEnter(null, 2);

			R.WriteLine("\n****\nperform complete date RESET\n****");

			ExStorData.Instance.ResetAll();

			init();

			initCreateSheets();

			R.AddRouteExit(null, 2);
		}

		protected void register(string name, Func<string, bool> f, string desc)
		{
			jumpTable.Add(name, f);

			testDesc.Add(name, desc);
		}

		protected Dictionary<string, Func<string, bool>> jumpTable = new ();

		protected Dictionary<string, string> testDesc = new Dictionary<string, string>();

		/* show / utility routines */

		public Sheet CreateSheet()
		{
			// R.AddRouteEnter(routeIdx: 2);

			Sheet sht = Sheet.CreateSheet(exid.CreateShtDsName(_wbk.GetId()),
				new ($"path:\\file{shtIdx}.xls", $"no sheet name {shtIdx}"));

			sht.SetTrackChanges();

			shtIdx++;

			// R.AddRouteExit(routeIdx: 2);

			return sht;
		}

		public Sheet CreateSheetStealth()
		{
			// R.AddRouteEnter(routeIdx: 2);
			Sheet sht = Sheet.CreateSheet(exid.CreateShtDsName(_wbk.GetIdStealth()),
				new ($"path:\\file{shtIdx}.xls", $"no sheet name {shtIdx}"));

			sht.SetTrackChanges();

			shtIdx++;

			// R.AddRouteExit(routeIdx: 2);

			return sht;
		}

		/* show and utility */

		protected void addFamAndTypeStealth(string fn, string tn, string p)
		{
			// R.AddRouteEnter(routeIdx: 2);
			string key = ExStorLib.FormatFamAndType(fn, tn);

			FamAndType fat = FamAndType.GetNewItem(fn, tn, p);

			fat.IsNewItemFat = false;

			AnySht.FamListWkg.Add(key, fat);

			// R.AddRouteExit(routeIdx: 2);
		}

		protected void beginTest(string proc)
		{
			testResults.Add(new(tstAnsIdx, [$"{tstId} | {proc}", ""]));
			// run test Test323D => Test323D | START | add three sheet
			R.StartRoute(0, $"**** run test | part [ {tstAnsIdx} ] | {tstId}");
			R.AddRouteEnter($"begin {proc}");

			R.NewLine();
			R.Write($"****************************\n");
			R.WriteAnyway($"{$"[ {tstAnsIdx} ]",-5} {tstId} | ");
			// R.NewLine();
			R.Write($"BEGIN ");
			R.WriteAnyway($"{proc,-30}");
			R.WriteOnlyWhenSilent(" >> ");
			R.NewLine();
			R.NewLine();

		}

		protected void endTest(string proc)
		{
			R.AddRouteExit();
			R.WriteLine($"\nAFTER {proc}");
			endTestInfo();
			R.ShowRoute();
		}

		protected abstract void endTestInfo();

		protected bool startPreTest()
		{
			R.WriteLine("\n*** pre-test values ***");

			bool results = V2.ValidateTests2(stopTestingOnError, tests2[tstAnsIdx]);
			testResults.Add(new (tstAnsIdx, [$"{tstId}",$"{getAnswer(results, 4)} ( pre-test )"]));

			tstAnsIdx++;

			return results;
		}

		protected bool endValidateTest(string proc)
		{
			string answer1;

			R.WriteLine($"****************************");
			R.WriteLine($"TEST VALIDATE | {$"[ {tstAnsIdx} ]",-5} | {tstId} | {proc} ");
			R.WriteLine($"\nTEST VALIDATE | workbook test result  ");

			bool result = V2.ValidateTests2(stopTestingOnError, tests2[tstAnsIdx]);
			bool result2;

			R.WriteLine($"\n***** >> {getAnswer(result, 4)}");

			if (uiTests2 != null && uiTests2.Count == tests2.Count &&
				uiTests2[tstAnsIdx].Choices != null!)
			{
				R.WriteLine($"\nui tests\n");

				result2 = V2.ValidateTests2(stopTestingOnError, uiTests2[tstAnsIdx], 2);

				result &= result2;

				answer1 = getAnswer(result, 4);
				answer1 += result2 ? $" ( ui tests worked )" : " ( ui tests failed )";
			}
			else
			{
				answer1 = getAnswer(result, 4);
				answer1 += " ( no ui tests) ";
			}

			testResults[tstAnsIdx].Item2[1] = answer1;

			R.Write("\n***** >> ");

			R.WriteAnyway($"{answer1} ");
			R.NewLineAnyway();
			R.NewLine();

			tstAnsIdx++;

			return result;
		}

		protected void ShowTestCompletionResult(bool result)
		{
			string r = result ? "WORKED" : "FAIL";

			R.WriteLine("\n****************************");
			R.WriteLine(tstDesc);
			R.WriteLine($"{tstId} | {r}");
			R.WriteLine("****************************\n");

			showTestResultSummary();

		}

		protected void showTestResultSummary()
		{
			if (testResults == null || testResults.Count == 0) return;

			R.WriteLine("**********\nTEST RESULT SUMMARY\n");

			foreach ((int idx, string[] item2) in testResults)
			{
				R.WriteLine($"{$"[ {idx} ]",-7} |  {item2[1],-7} ( {item2[0]} )");
			}

			R.WriteLine("\n**********");
		}

		// protected void showLastIdStatus(string location)
		// {
		// 	string status = _xData.WorkBook.LastIdField.IsDirty() ? "is Dirty" : "is Clean";
		//
		// 	R.WriteLine($"\n\t*** {location} | LastId | [ {_xData.WorkBook.LastId} ] | status [ {status} ] | chg src id [ {_xData.WorkBook.LastIdField.ChgSrc} ]\n");
		// }

		// protected void showShtLst()
		// {
		// 	if (!doingShtLst) return;
		//
		// 	ShowWbk.ShowShtsLst();
		//
		// 	R.NewLine();
		// }

		/// <summary>
		/// convert a bool into a good answer string<br/>
		/// type<br/>
		/// 0 = yes / no<br/>
		/// 1 = does match / does not match<br/>
		/// 2 = match / not match<br/>
		/// 3 = correct / wrong<br/>
		/// 4 = worked / failed<br/>
		/// 5 = pass / fail
		/// </summary>
		protected string getAnswer(bool result, int type)
		{
			string [,] answers = new [,]
				{
					{ "Yes", "No" },					// 0
					{ "DOES Match", "does NOT match" }, // 1
					{ "Match", "NOT match" },			// 2
					{ "Correct", "Wrong" },				// 3
					{ "WORKED", "FAILED" },				// 4
					{ "PASS", "FAIL" },					// 5
				};

			if (type < 0 || type > answers.GetLength(0)) return $"wrong type ( {type} ) provided to get answer ";

			return answers[type, result ? 0 : 1];

			// if (type == 0)
			// {
			// 	return result ? "Yes" : "No";
			// }
			//
			// if (type == 1)
			// {
			// 	return result ? "DOES Match" : "does NOT match";
			// }
			//
			// if (type == 2)
			// {
			// 	return result ? "Match" : "NOT match";
			// }
			//
			// if (type == 3)
			// {
			// 	return result ? "Correct" : "Wrong";
			// }
			//
			// if (type == 4)
			// {
			// 	return result ? "WORKED" : "FAILED";
			// }
			//
			// return result.ToString();
		}

	}
}