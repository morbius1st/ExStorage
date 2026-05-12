using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

// username: jeffs
// created:  4/18/2026 3:43:01 PM

namespace UtilityLibrary
{
	public static class R
	{
	#region private fields

		private const string IN_ARROW = "=> ";
		private const string OUT_ARROW = "<= ";
	#endregion

	#region ctor

		static R()
		{
			resetRoute();
		}

	#endregion

		public static bool RunSilent { get; set; } = false;

		public static int prefaceColWidth = 30;

		public static void WriteLineAnyway(string msg)
		{
			Console.WriteLine(msg);
		}

		public static void WriteAnyway(string msg)
		{
			Console.Write(msg);
		}

		public static void WriteLine(string msg1)
		{
			if (!RunSilent) Console.WriteLine(msg1);
		}

		public static void Write(string msg1)
		{
			if (!RunSilent) Console.Write(msg1);
		}

		public static void NewLineAnyway()
		{
			Console.Write("\n");
		}

		public static void NewLine()
		{
			if (!RunSilent) Console.Write("\n");
		}

		public static void WriteLine2(string msg1, int x = -1, string msg2 = "", string msg3 = "")
		{
			if (RunSilent) return;

				int pw = x > 0 ? x : prefaceColWidth;

			string a = msg1.PadRight(pw);
			string b =msg2.IsVoid() ? "" : $" | {msg2}";
			string c =msg3.IsVoid() ? "" : $" | {msg3}";

			Console.WriteLine($"{a}{b}{c}");
		}

		/* route */

		public static int DepthMultiplier { get; set; } = 3;
		private static int Depth
		{
			get => depth;
			set
			{
				depth = (value <=0 ? 1 : value);
			}
		}

		private static List<Tuple<int, string, string>> route;
		private static int depth;

		public static void StartRoute([CallerMemberName] string name = "")
		{
			depth = -1;
			resetRoute();
			// route.Add(new (0, name, ""));

			// R.AddRoute(name, 2, true);
			// R.AddRoute(null, 2, true, 1, name);
		}

		public static void AddRouteEnter(string? msg = "", bool addMorM = false, [CallerMemberName] string name = "", [CallerFilePath] string path = "")
		{
			string p = Path.GetFileNameWithoutExtension(path);

			if (addMorM) AddRoute(msg.IsVoid() ? null : msg, 2, true, 1, $"{name} [ {p} ]", null);
			else AddRoute(msg.IsVoid() ? null : msg, 0, true, 1, $"{name} [ {p} ]", null);
		}
		public static void AddRouteExit(string msg = "", [CallerMemberName] string name = "")
		{
			AddRoute((msg.IsVoid() ? null : msg), 0, true, -1, name, null);

			// AddRoute($"<<= exit {msg}", msg: true, d: -1, name: name);
		}

		/// <summary>
		/// provide none, get {name}<br/>
		/// provide r, get {r}<br/>
		/// provide r, true; get {name} ( {r} )
		/// </summary>
		public static void AddRoute(object? r = null, int mOrM = 0, bool msg = false, int d = 0, [CallerMemberName] string name = "", [CallerFilePath] string? path = "")
		{
			if (SuspendAddRoute) return;

			if (d > 0) depth += d;

			string result = "";
			string result2 = "";
			string dir = d > 0 ? IN_ARROW : d < 0 ? OUT_ARROW : "    ";

			string n = name;


			if (path != null)
			{
				string p = Path.GetFileNameWithoutExtension(path);
				n = $"{name} [ {p} ]";
			}
			
			if (msg)
			{
				result = n;
				if (r != null) result2 = $"( {r} )";
			}
			else
			{
				result = n;
				if (r != null) result2 = $"( {r} )";
			}

			if (mOrM > 0)
			{
				string file;
				string method;

				result2 = result2.IsVoid() ? "" : $"{result2} | ";

				if (mOrM == 1) result2 = $"{result2}[ {getCalling(4, out file, out method)} ]";
				else result2 = $"{result2}[ {getCalling(5, out file, out method)} ]";
			}

			route.Add(new (depth, $"{dir} {result}", result2));

			if (d < 0) depth += d;
		}

		public static void ShowRoute(string begMsg = "", string endMsg = "")
		{
			string msg;

			if (route.Count == 0) return;

			Write("ROUTE |");

			NewLine();

			R.WriteLine($"{IN_ARROW}BEGIN ROUTE{(begMsg.IsVoid() ? "" : $" | {begMsg}" )}");

			foreach ((int d, string s, string m) in route)
			{
				string b = d <=0 ? "" : " ".Repeat(d * DepthMultiplier);

				msg = $"{b} {s}";
				WriteLine($"{msg,-66}{m}");
			}

			R.WriteLine($"{OUT_ARROW}END ROUTE{(endMsg.IsVoid() ? "" : $" | {endMsg}")}");

			NewLine();

			resetRoute();
		}

		public static bool SuspendAddRoute { get; set; } = false;

		private static void resetRoute()
		{
			route = new ();
		}

		private static string getCalling(int which, out string module, out string method)
		{
			method = CsUtilities.GetCallingInfo(which, out module);

			return $"{module} / {method}";
		}

	}
}