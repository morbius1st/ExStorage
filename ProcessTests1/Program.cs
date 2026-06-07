using ExStorSys;
using ProcessTests1.General;
using UtilityLibrary;

namespace ProcessTests1
{
	public class Program
	{
		public static Tests1 t1;
		public static Tests2 t2;
		public static Tests3 t3;

		static Program p;

		// private string[] runTheseTests = new [] { "1A", "1B", "1C", "1D", "2B", "2C", "3C" };
		// private string[] runTheseTests = new [] { "11A", "12A" };
		// private string[] runTheseTests = new [] { "1A", "1B", "1C", "1D" };
		// private string[] runTheseTests = new [] { "2B", "2C", "3C"};
		// private string[] runTheseTests = new [] { "21A", "21B", "22A"};
		// private string[] runTheseTests = new [] { "3C", "3D" };
		// private string[] runTheseTests = new [] { "23C", "23D" };
		// private string[] runTheseTests = new [] { "4A", "24C", "24D" };


		private static List<string> runTheseTests = new List<string>();

		// private static string oneTestName = "Test323A";
		private static readonly string oneTestId = "Test302B";

		private static readonly bool runSingleTest3 = false;

		private static readonly bool runMultiSilent = true;
		
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
			runTheseTests.AddRange([ "Test301A", "Test301B", "Test302A", "Test302B"]);
			runTheseTests.AddRange(["Test303A", "Test304A", "Test305A", "Test306A", "Test307A", "Test308A"]);
			runTheseTests.AddRange([ "Test311A", "Test312A", "Test313A"]);
			runTheseTests.AddRange([ "Test322A", "Test323A", "Test323B", "Test323C"]);
			runTheseTests.AddRange([ "Test324A", "Test324B", "Test324C","Test324D","Test324D","Test324F", ]);
			runTheseTests.AddRange([ "Test331A", "Test331B", "Test331C" ]);


			try
			{
				bool result = false;
				bool runTest2 = false;
				bool runSilent = false;

				totalCount = 0;

				R.StartRoute(1);

				// create this only once this way
				ExStorData.Create();

				p = new Program();

				t3 = new Tests3(p);
				t2 = new();
				t1 = new();

				ConsoleKeyInfo r;

				/* TEST ONE */

				R.AddRoute();

				// p.RunOneTestJumpTable2("Test105E", false, true);
				// p.RunOneTest3("Test301C", false, true);

				if (runSingleTest3)
				{
					runSilent = false;
					result = p.RunOneTest3(oneTestId, runSilent, true);

				}
				else
				{
					if (runMultiSilent)
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

				if (runTest2)
				{
					/* TEST TWO */
					p.RunOneTest("4A", false);
				}

				if (!runSilent) R.ShowRoute(1);

				R.RunSilent = false;

				showTestResult(result);

				Console.Write("\nWaiting ... ");
				r = Console.ReadKey();

			}
			catch (Exception e)
			{
				Exception? ex = e;

				while (ex != null)
				{
					Console.WriteLine(ex.Message);
					R.AddRoute( ex.Message, 1);
					ex = ex.InnerException;
				}
				
				throw;
			}

		}

		public void RunOneTest(int idx, bool runSilent, bool shwWbkOr = true)
		{
			if (idx < 0 || idx >  runTheseTests.Count) return;

			t1.ShowWbkOverRideControl = shwWbkOr;

			runTest(runTheseTests[idx], runSilent);
		}

		public void RunOneTest(string idx, bool runSilent, bool shwWbkOr = true)
		{
			t1.ShowWbkOverRideControl = shwWbkOr;

			runTest(idx, runSilent);
		}

		public void RunTests(bool runSilent, bool quitOnFail = true, bool shwWbkOr = true)
		{
			bool result = true;
			t1.ShowWbkOverRideControl = shwWbkOr;

			foreach (string test in runTheseTests)
			{
				if (!runTest(test, runSilent))
				{
					result = false;
					if (quitOnFail) break;
				}
			}

			showTestResult(result);
		}

		private static void showTestResult(bool result)
		{
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

		private bool runTest(string test, bool runSilent)
		{
			bool? result = null;

			if (runSilent) R.RunSilent = true;

			t1.Reset();

			R.WriteAnyway($"\n*********\nrun test {test} => ");

			switch (test)
			{
			case "1A":
				{
					result = t1.Test1A();
					break;
				}
			case "11A":
				{
					result = t1.Test11A();
					break;
				}
			case "12A":
				{
					result = t1.Test12A();
					break;
				}
			case "1B":
				{
					result = t1.Test1B();
					break;
				}
			case "1C":
				{
					result = t1.Test1C();
					break;
				}
			case "1D":
				{
					result = t1.Test1D();
					break;
				}
			case "1E":
				{
					result = t1.Test1E(test);
					break;
				}
			case "2B":
				{
					result = t1.Test2B();
					break;
				}
			case "2C":
				{
					result = t1.Test2C();
					break;
				}
			case "21A":
				{
					result = t1.Test21A(test);
					break;
				}
			case "21B":
				{
					result = t1.Test21B(test);
					break;
				}
			case "22A":
				{
					result = t1.Test22A(test);
					break;
				}
			case "3C":
				{
					result = t1.Test3C();
					break;
				}
			case "3D":
				{
					result = t1.Test3D();
					break;
				}
			case "4A":
				{
					result = t1.Test4A(test);
					break;
				}
			case "23C":
				{
					result = t1.Test23C(test);
					break;
				}
			case "23D":
				{
					result = t1.Test23D(test);
					break;
				}
			case "24C":
				{
					result = t1.Test24C(test);
					break;
				}
			case "24D":
				{
					result = t1.Test24D(test);
					break;
				}
			case "41A":
				{
					result = t1.Test41A(test);
					break;
				}
			case "42A":
				{
					result = t1.Test42A(test);
					break;
				}
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


		/* using jump table*/

		public void RunOneTestJumpTable2(string idx, bool runSilent, bool shwWbkOr = true)
		{
			R.AddRoute();

			t2.ShowShtOverRideControl = shwWbkOr;

			runJumpTest2(idx, runSilent);
		}

		private bool runJumpTest2(string test, bool runSilent)
		{
			R.AddRouteEnter();

			bool? result = null;

			if (runSilent) R.RunSilent = true;

			// R.AddRouteExit();
			// R.ShowRoute();

			t2.Reset();

			R.WriteAnyway($"\n*********\nrun test {test} => ");

			Func<string, bool>? method = null;

			if (_jumpTable2.TryGetValue(test, out method))
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

		public static void Register2(string name, Func<string, bool> f)
		{
			Program._jumpTable2.Add(name, f);
		}

		private static readonly Dictionary<string, Func<string, bool>> _jumpTable2 = new ()
		{
			{ "101A", tsk101A },
			{ "101B", tsk101B },
			{ "102A", tsk102A },
			{ "104E", tsk104E },
			{ "104F", tsk104F },
			{ "105C", tsk105C },
			{ "105D", tsk105D },
		};

		static Func<string, bool> tsk101A = (a) => t2.Test101A(a);
		static Func<string, bool> tsk101B = (a) => t2.Test101B(a);
		static Func<string, bool> tsk102A = (a) => t2.Test102A(a);
		static Func<string, bool> tsk104E = (a) => t2.Test104E(a);
		static Func<string, bool> tsk104F = (a) => t2.Test104F(a);
		static Func<string, bool> tsk105C = (a) => t2.Test105C(a);
		static Func<string, bool> tsk105D = (a) => t2.Test105D(a);


		private static int totalCount;
		private static int passCount;
		private static int failCount;

		public bool RunTests3(bool runSilent, bool quitOnFail = true, bool shwWbkOr = true)
		{
			bool result = true;

			totalCount = runTheseTests.Count;
			failCount = 0;
			passCount = 0;

			foreach (string test in runTheseTests)
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

			return result;
		}

		public bool RunOneTest3(string testId, bool runSilent, bool shwWbkOr = true)
		{
			t3.ShowWbkOverRideControl = shwWbkOr;

			return t3.RunOneTest(testId, runSilent);
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