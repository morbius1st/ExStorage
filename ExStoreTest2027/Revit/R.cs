// using System;
// using System.Collections.Generic;
// using System.ComponentModel;
// using System.Linq;
// using System.Runtime.CompilerServices;
// using System.Text;
// using System.Threading.Tasks;
// using UtilityLibrary;

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using UtilityLibrary;


// projname: ExStorageTest2026
// itemname: R
// username: jeffs
// created:  9/15/2025 7:41:12 PM

namespace RevitLibrary
{
	public class R
	{
	#region private fields

		private static readonly Lazy<R> instance =
			new Lazy<R>(() => new R());

		private static Document? rvtDoc;

	#endregion

	#region ctor

		private R() {}

		static R() {}

		public static R Instance => instance.Value;
	#endregion

	#region public properties

		public static IWin Msg { get; set; }

		public static UIApplication? RvtUiApp { get; set; }
		public static UIDocument? RvtUidoc { get; set; }
		public static Application? RvtApp { get; set; }

		public static Document? RvtDoc
		{
			get => rvtDoc;
			set
			{ 
				rvtDoc = value;
				FileNameChanged?.Invoke(null, new PropertyChangedEventArgs("FileName"));
			}
		}

		public static string? FileName => RvtDoc?.Title;

		public static string? FilePath => RvtDoc?.PathName;

		public static int OpenDocCount => RvtApp?.Documents.Size ?? -1;
		public static bool NoDocsOpen => OpenDocCount == 0;
		public static bool OnlyOneDocOpen => OpenDocCount == 1;
		public static bool MultipleDocsOpen => OpenDocCount > 1;

		public static bool ShowProcessMsg {get; set;} = false;

	#endregion

		public static void ProcessMsg(string msg, bool? inout = true, int objId = -1, [CallerMemberName] string who = "")
		{
			if (!ShowProcessMsg) return;

			// string p = Path.GetFileNameWithoutExtension(path);

			// StackFrame s0f = new StackFrame(0, false);
			// StackFrame s0t = new StackFrame(0, true);
			// StackFrame s2f = new StackFrame(2, false);
			// StackFrame s2t = new StackFrame(2, true);
			// StackFrame s1t = new StackFrame(1, true);
			// StackFrame s1f = new StackFrame(1, false);

			string s2 = getPriorPath(3);
			string s3 = getPriorPath(4);
			string s4 = getPriorPath(5);
			string s5 = getPriorPath(6);

			string begin = " ->";
			string end = "<- ";
			string mid = ">-<";

			string c = new StackFrame(1, false).GetMethod()?.DeclaringType.Name ?? "is null";

			string d = inout.HasValue ? (inout == true ? begin : end ) : mid;
			string m = msg.IsVoid() ? "" : $"{msg}";
			string o = objId > -1 ? $"({objId})" : "";
			string w = $"{c} / {who}";

			// Debug.WriteLine($"*** {s5,-36} -> {s4,-36} -> {s3,-36} -> {s2,-36} -> {w,-36} {o, 4} | {d} {m} ");

			// Debug.WriteLine($"*** | -> {s5}");
			// Debug.WriteLine($"*** | \t+-> {s4}");

			// if (d.Equals(begin))
			// {
			// 	Debug.WriteLine($"    |   +-> {s3}");
			// 	Debug.WriteLine($"    |   \t+-> {s2}");
			// }

			Debug.WriteLine($"*** | \t+-> {w,-46} {o, 4} | {d} {m} ");

		}

		private static string getPriorPath(int which)
		{
			string ca = "";
			string cb = "";
			try
			{
				StackFrame st = new StackFrame(which, true);
				ca = st.GetMethod()?.DeclaringType?.Name ?? "no";
				cb = st.GetMethod()?.Name ?? "data";
			}
			catch 
			{
				ca="no";
				cb="data";
			}

			return $"{ca} / {cb}";
		}


		public static event EventHandler? FileNameChanged;

		public override string ToString()
		{
			return $"this is {nameof(R)}";
		}
	}
}