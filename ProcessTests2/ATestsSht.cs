using ExStorSys;
using static ExStorSys.ExStorConstFaux;
using UtilityLibrary;


// user name: jeffs
// created:   5/23/2026 9:34:23 AM

namespace ProcessTests2
{
	public abstract class ATestsSht : ATestsCommon
	{
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

			if (tstNameModIdx == 0)
			{
				nametst = tstNamMod[tstNameModIdx++];
				nameWbk = _sht.NameModified;
				nameStatus = nametst.Equals(nameWbk);
				nameResult = nameStatus ? "they MATCH" : "they do NOT match";

				R.WriteLine($"\n\tdoes actual {nameWbk} equal test {nametst}? {nameResult}");
			}

			R.NewLine();
		}

		protected override void testPreface()
		{
			R.AddRoute();

			R.NewLine();
			R.WriteAnyway($"START TESTS SHT 1 ");
			R.NewLine();

			R.WriteLine($"\n*** Current user is {FauxUserName}");
			R.WriteLine($"*** Current sheet is {XData.CurrentSheet.DsName}");
			R.WriteLine($"*** IsModExo [ {AnySht.IsModifiedExo} ] | IsModFamLst [ {AnySht.IsModifiedFamListWkg} ]");

			ShowSht.ShowSheetFields();
			ShowSht.ShowFamList();

			testResults = new ();
		}

		protected override void endTestInfo()
		{
			ShowSht.ShowSheetFields();
			ShowSht.ShowFamList();
		}
	}
}