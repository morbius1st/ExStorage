using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
			route = new List<Tuple<int, string, string>>[3];

			resetRoutes();
		}

	#endregion

		public static bool RunSilent { get; set; } = false;

		public static int prefaceColWidth = 30;

		public static void WriteOnlyWhenSilent(string msg)
		{
			if (RunSilent) Console.Write(msg);
		}

		public static void WriteLineAnyway(string msg)
		{
			Console.WriteLine(msg);
		}

		public static void WriteAnyway(string msg)
		{
			Console.Write(msg);
		}

		public static void WriteLine(string msg1, bool addRoute = false, int routeIdx = 0)
		{
			if (!RunSilent) Console.WriteLine(msg1);

			if (addRoute) AddRoute(msg1, routeIdx, -1);
		}

		public static void Write(string msg1, bool addRoute = false, int routeIdx = 0)
		{
			if (!RunSilent) Console.Write(msg1);

			if (addRoute) AddRoute(msg1, routeIdx, -1);
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
			string b = msg2.IsVoid() ? "" : $" | {msg2}";
			string c = msg3.IsVoid() ? "" : $" | {msg3}";

			Console.WriteLine($"{a}{b}{c}");
		}

		/* route */

		public static int DepthMultiplier { get; set; } = 3;

		private static List<Tuple<int, string, string>>[] route;
		private static int routeDepth;

		public static bool SuspendAddRoute { get; set; } = false;

		public static int[] RouteDepth;

		public static void StartRoute(int routeIdx = 0, [CallerMemberName] string name = "")
		{
			RouteDepth[routeIdx] = -1;
			resetRoute(routeIdx);
			// route.Add(new (0, name, ""));

			// R.AddRoute(name, 2, true);
			// R.AddRoute(null, 2, true, 1, name);
		}

		public static void AddRouteEnter(string? msg = "", int routeIdx = 0, bool addMorM = false, [CallerMemberName] string name = "", [CallerFilePath] string path = "")
		{
			string p = Path.GetFileNameWithoutExtension(path);

			if (addMorM) AddRoute(  msg.IsVoid() ? null : msg, routeIdx, 2, true, 1, $"{name} [ {p} ]", null);
			else AddRoute(  msg.IsVoid() ? null : msg, routeIdx, 0, true, 1, $"{name} [ {p} ]", null);
		}
		
		public static void AddRouteExit(string msg = "", int routeIdx = 0, [CallerMemberName] string name = "")
		{
			AddRoute(  (msg.IsVoid() ? null : msg), routeIdx, 0, true, -1, name, null);

			// AddRoute($"<<= exit {msg}", msg: true, d: -1, name: name);
		}

		/// <summary>
		/// provide none, get {name}<br/>
		/// provide r, get {r}<br/>
		/// provide r, true; get {name} ( {r} )<br/>
		/// mOrM == 0, include method name, == -1, do not include method name, &gt;0 include method name and calling path<br/>
		/// d = forced depth (which continues until changed)
		/// </summary>
		public static void AddRoute(object? r = null, int routeIdx = 0, int mOrM = 0, bool msg = false, int d = 0, [CallerMemberName] string name = "", [CallerFilePath] string? path = "")
		{
			// string ri0 = routeIdx == 0 ? $"route 0 {route[0].Count,-3}" : "";
			// string ri1 = routeIdx == 1 ? $"route 1 {route[1].Count,-3}" : "";
			// string ri2 = routeIdx == 2 ? $"route 2 {route[2].Count,-3}" : "";
			//
			// Debug.WriteLine($"add route | {ri0,-3}   {ri1,-3}   {ri2,-3} | {name} | *** {r}");

			if (SuspendAddRoute) return;

			if (d > 0) RouteDepth[routeIdx] += d;

			string result = "";
			string result2 = "";
			string dir = d > 0 ? IN_ARROW : d < 0 ? OUT_ARROW : "    ";

			string n = mOrM >= 0 ? name : "...";


			if (path != null && mOrM >= 0)
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

			route[routeIdx].Add(new (RouteDepth[routeIdx], $"{dir} {result}", result2));

			if (d < 0) RouteDepth[routeIdx] += d;
		}

		public static void ShowRoute(int routeIdx = 0, string begMsg = "", string endMsg = "")
		{
			string msg;

			if (route[routeIdx].Count == 0) return;

			Write($"ROUTE | total of {route[routeIdx].Count} routes");

			NewLine();

			R.WriteLine($"{IN_ARROW}BEGIN ROUTE{(begMsg.IsVoid() ? "" : $" | {begMsg}" )}");

			foreach ((int d, string s, string m) in route[routeIdx])
			{
				string b = d <=0 ? "" : " ".Repeat(d * DepthMultiplier);

				msg = $"{b} {s}";
				WriteLine($"{msg,-66}{m}");
			}

			R.WriteLine($"{OUT_ARROW}END ROUTE{(endMsg.IsVoid() ? "" : $" | {endMsg}")}");

			NewLine();

			resetRoute();
		}

		private static void resetRoute(int routeIdx = 0)
		{
			route[routeIdx] = new ();
			RouteDepth[routeIdx] = 0;
		}

		private static void resetRoutes()
		{
			RouteDepth = new int[route.Length];

			for (int i = 0; i < route.Length; i++)
			{
				route[i] = new ();
			}
		}

		private static string getCalling(int which, out string module, out string method)
		{
			method = CsUtilities.GetCallingInfo(which, out module);

			return $"{module} / {method}";
		}

	}
}