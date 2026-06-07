using System.Diagnostics;
using System.Runtime.CompilerServices;
using ExStorSys;
using ProcessTests2.General;
using UtilityLibrary;

namespace ProcessTests2
{
	public class Program
	{
		public static Tests1 t1;
		public static Tests2 t2;

		static Program p;

		// private string[] runTheseTests = new [] { "1A", "1B", "1C", "1D", "2B", "2C", "3C" };
		// private string[] runTheseTests = new [] { "11A", "12A" };
		// private string[] runTheseTests = new [] { "1A", "1B", "1C", "1D" };
		// private string[] runTheseTests = new [] { "2B", "2C", "3C"};
		// private string[] runTheseTests = new [] { "21A", "21B", "22A"};
		// private string[] runTheseTests = new [] { "3C", "3D" };
		// private string[] runTheseTests = new [] { "23C", "23D" };
		// private string[] runTheseTests = new [] { "4A", "24C", "24D" };


		public static List<string>? RunTheseTests = new List<string>();

		public static string OneTestId;

		public static bool RunSingleTest3;

		public static bool RunMultiSilent = true;

		public static ITests? TestSet;

		public static ITests? TestSetWbk = null;
		public static ITests? TestSetSht = null;

		/**
		 * notes for process tracking
		 * * tracks each level separately based on enter & exit
		 * * 3 main tracking rouitines - enter, exit, mid
		 *		> enter & exit can be provided a list of objects
		 *			> the objects have an interface that allows access to
		 *				> the objects name
		 *				> values to track as a list of name / value pairs (2 levels - basic and all - per enter & exit)
		 *		> all routines track the name of the calling routine
		 *		> enter & exit rotines have an option to record deeper calling routes
		 *		> all tracking routines record the date / time is MS
		 *		> all tracking routines can record a message
		 *		> all tracking routines have an error message
		 *		> enter routine [+1] depth change, exit [-1] depth change, mid [0] depth change
		 * * other routines
		 *		> start (and initialize)
		 *		> end (and complete)
		 *		> change depth
		 *		> overall exception
		 *		> show log
		 * * all information is written to a log file in JSON format
		**/

		static void Main(string[] args)
		{
			// AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

			p = new Program();
			t1 = new Tests1();
			t2 = new Tests2();

			ConsoleKeyInfo r;

			TestSet = TestSetWbk ?? TestSetSht;

			if (TestSet == null)
			{
				R.WriteLine("\n*****\nTestSet is null - cannot proceed - abort\n*****\n\n");
			}
			else
			{
				RunTheseTests = TestSet.RunTheseTests;
				OneTestId = TestSet.OneTestId;

				try
				{
					RunSingleTest3 = RunTheseTests == null ||
						RunTheseTests.Count == 0;

					bool result = false;
					bool runTest = false;
					bool runSilent = false;

					totalCount = 0;

					R.StartRoute(1);
					R.StartRoute(2);

					R.AddRouteEnter(routeIdx: 1);

					// create this only once this way
					ExStorData.Create();

					/* TEST ONE */

					R.AddRoute(routeIdx: 1);

					if (RunSingleTest3)
					{
						runSilent = false;
						result = p.RunOneTest3(OneTestId, runSilent, true);

					}
					else
					{
						if (RunMultiSilent)
						{
							runSilent = true;
							result = p.RunTests3(runSilent, false, false);
						}
						else
						{
							runSilent = false;
							result = p.RunTests3(runSilent, false, true);
						}
					}

					R.AddRouteExit(routeIdx: 1);

					// if (!runSilent) R.ShowRoute(1);

					R.RunSilent = false;

					showTestResult(result);



				}
				catch (Exception e)
				{
					Exception? ex = e;

					while (ex != null)
					{
						Console.WriteLine(ex.Message);
						R.AddRoute(ex.Message, 1);
						ex = ex.InnerException;
					}

					throw;
				}

			}

			Console.Write("\nWaiting ... ");
			r = Console.ReadKey();

		}

		private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			ConsoleKeyInfo r;
			Exception e = (Exception) args.ExceptionObject;

			Console.WriteLine($"\n*****\n[FATAL ERROR] Exception Caught");
			Console.WriteLine($"\nmessage | {e.Message}");
			if (e.InnerException != null)
			{
				Console.WriteLine($"\ninner message | {e.InnerException.Message}");
			}
			Console.WriteLine($"\nstack trace");

			StackTrace st = new StackTrace(e, true);

			string fileName;

			foreach (StackFrame sf in st.GetFrames())
			{
				fileName = Path.GetFileNameWithoutExtension(sf.GetFileName());

				Console.WriteLine($"line {sf.GetFileLineNumber(), -6}| {fileName, -24} | {sf.GetMethod()}");
			}

			
			Console.Write("\nWaiting ... ");
			r = Console.ReadKey();

			Environment.Exit(1);

		}

		private static void showTestResult(bool result)
		{
			if (RunSingleTest3) return;

			R.WriteLine("\n*************");

			if (result)
			{
				R.WriteLine("WORKED");
			}
			else
			{
				R.WriteLine("FAILED");
			}

			if (totalCount > 0)
			{
				R.NewLine();
				R.WriteLine($" {"Total Tests",-18} = {totalCount}");
				R.WriteLine($" {"Pass Tests",-18} = {passCount}");
				R.WriteLine($" {"Fail Tests",-18} = {failCount}");
				R.NewLine();
			}

			R.WriteLine("*************\n\n");
		}


		private static int totalCount;
		private static int passCount;
		private static int failCount;

		public bool RunTests3(bool runSilent, bool quitOnFail = true, bool shwWbkOr = true)
		{
			R.AddRouteEnter(routeIdx: 1);

			bool result = true;

			totalCount = RunTheseTests.Count;
			failCount = 0;
			passCount = 0;

			foreach (string test in RunTheseTests)
			{
				if (!RunOneTest3(test, runSilent, shwWbkOr))
				{
					result = false;
					failCount++;
					if (quitOnFail) break;
				}
				else
				{
					passCount++;
				}
			}

			R.AddRouteExit(routeIdx: 1);

			return result;
		}

		public bool RunOneTest3(string testId, bool runSilent, bool shwWbkOr = true)
		{
			R.AddRouteEnter(routeIdx: 1);

			TestSet.ShowWbkOverRideControl = shwWbkOr;

			bool result = TestSet.RunOneTestSeq(testId, runSilent);

			R.AddRouteExit(routeIdx: 1);

			return result;
		}

		/* moved to object t3

		private bool runJumpTest3(string test, bool runSilent)
		{
			R.AddRouteEnter();

			bool? result = null;

			if (runSilent) R.RunSilent = true;

			// R.AddRouteExit();
			// R.ShowRoute();

			t3.Reset();

			R.WriteAnyway($"\n*********\nrun test {test} => ");

			Func<string, bool>? method = null;

			if (_jumpTable3.TryGetValue(test, out method))
			{
				result = method(test);
			}

			R.RunSilent = false;

			if (!result.HasValue)
			{
				R.WriteLine($"\n\n**** FAIL ***** TEST NOT FOUND\n\n");
			}
			else
			{
				string answer = result == true ? "PASS" : "FAIL";

				R.WriteLine($"test run results | {answer}\n*********");
			}
			return result == true;
		}


		public static void Register3(string name, Func<string, bool> f)
		{
			Program._jumpTable3.Add(name, f);
		}
		
		public static Dictionary<string, Func<string, bool>> _jumpTable3 = new ();
		
		 */
		
	}
}