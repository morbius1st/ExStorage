using ExStorSys;
using ProcessTests1.General;
using UtilityLibrary;

namespace ProcessTests1
{
	internal class Program
	{
		public static Tests1 t1 = new ();
		public static Tests2 t2 = new ();

		static Program p;

		static void Main(string[] args)
		{
			// create this only once this way
			ExStorData.Create();

			// jumpTable.Add("104A", tsk104A);
			// jumpTable.Add("XX", tskXX);

			p = new Program();

			// Dictionary<string, Func<string, string, bool>> a = p.jumpTable2;
			// Dictionary<string, Func<string, string, bool>> b = p.jumpTable3;
			//
			// string test = "XX";
			//
			// bool result = jumpTable[test](test, "b");

			ConsoleKeyInfo r;

			/* TEST ONE */

			// with full info
			// p.RunTests(false, false, true);

			// with pass fail
			// p.RunTests(true, false, false);

			// p.RunOneTest(1, false, true);

			// p.RunOneTest("1A", false, true);
			// p.RunOneTest("24C", false, true);
			// p.RunOneTest("42A", false, true);
			// p.RunOneTest("104A", false, true);

			R.AddRoute();

			p.RunOneTestJumpTable("101A", false, true);
			Console.Write("\nWaiting ... ");
			r = Console.ReadKey();

			return;

			/* TEST TWO */

			p.RunOneTest("4A", false);

			Console.Write("\nWaiting ... ");
			r = Console.ReadKey();

		}

		// private string[] runTheseTests = new [] { "1A", "1B", "1C", "1D", "2B", "2C", "3C" };
		// private string[] runTheseTests = new [] { "11A", "12A" };
		// private string[] runTheseTests = new [] { "1A", "1B", "1C", "1D" };
		// private string[] runTheseTests = new [] { "2B", "2C", "3C"};
		// private string[] runTheseTests = new [] { "21A", "21B", "22A"};
		// private string[] runTheseTests = new [] { "3C", "3D" };
		// private string[] runTheseTests = new [] { "23C", "23D" };
		private string[] runTheseTests = new [] { "4A", "24C", "24D" };

		public void RunOneTest(int idx, bool runSilent, bool shwWbkOr = true)
		{
			if (idx < 0 || idx >  runTheseTests.Length) return;

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

		private void showTestResult(bool result)
		{
			if (result)
			{
				R.WriteLine("\n\n*************");
				R.WriteLine("WORKED");
				R.WriteLine("*************\n\n");
			}
			else
			{
				R.WriteLine("\n\n*************");
				R.WriteLine("FAILED");
				R.WriteLine("*************\n\n");
			}
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

		public void RunOneTestJumpTable(string idx, bool runSilent, bool shwWbkOr = true)
		{
			R.AddRoute();

			t1.ShowWbkOverRideControl = shwWbkOr;

			runJumpTest(idx, runSilent);
		}


		private bool runJumpTest(string test, bool runSilent)
		{
			R.AddRouteEnter();

			bool? result = null;

			if (runSilent) R.RunSilent = true;

			t2.Reset();

			R.WriteAnyway($"\n*********\nrun test {test} => ");

			Func<string, bool>? method = null;

			if (p._jumpTable3.TryGetValue(test, out method))
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

			R.AddRouteExit();

			return result == true;
		}


		private readonly Dictionary<string, Func<string, bool>> _jumpTable3 = new ()
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

	}
}