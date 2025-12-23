using System.Diagnostics;
using System.Text;
using System.Windows.Interop;

using ExStoreTest2026.Windows;

using ExStorSys;

using UtilityLibrary;

using static System.Net.Mime.MediaTypeNames;


// user name: jeffs
// created:   11/27/2022 4:29:06 PM

namespace ExStoreTest2026.DebugAssist
{
	[DebuggerStepThrough]
	public static class Msgs
	{
		private static string objId => $"me {ObjectId} | win {(Mw?.ObjectId.ToString() ?? "null")}";
		private static int ObjectId;

		private static int col1Width;
		private static int tabs1 = 0;
		private static int tabs2 = 0;

		public static bool ShowDebug = false;

		public static string TAB { get; set; } = "   ";

		public static MainWindow? Mw { get; set; }

		static Msgs()
		{
			Col1Width = 0;

			// objectId = AppRibbon.objectIdx++;

			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(Msgs)) ?? -1;
		}

		
		public static int Col1Width
		{
			// ReSharper disable once UnusedMember.Global
			get => col1Width;
			set
			{
				if (value <= 0) value = 24;
				col1Width = value;
			}
		}

		/* not cached methods */

		public static void WriteLine(string text, string msg2 = "")
		{
			Write( $"{text}{msg2}\n");
		}

		public static void Write(string? text)
		{
			if (Mw != null) Mw.Message += text;

			if (ShowDebug) Debug.Write(text);
		}

		public static void WriteCache(string? text)
		{
			if (Mw != null) Mw.Message += text;
		}

		public static void NewLine()
		{
			Write("\n");
		}

		public static void WriteLineSpaced(string msg1, object? o2 = null, int colWidth = 0)
		{
			int width = colWidth == 0 ? col1Width : colWidth;

			Write( writeSpaced(msg1, o2, width) + "\n");
		}

		public static void WriteSpaced(string msg1, object? o2 = null, int colWidth = 0)
		{
			int width = colWidth == 0 ? col1Width : colWidth;

			Write(writeSpaced(msg1, o2, width));
		}

		private static string writeSpaced(string msg1, object? o2 = null, int colWidth = 0)
		{
			int width = colWidth == 0 ? col1Width : colWidth;

			string msg2 = (o2 != null ? o2.ToString() : "")!;

			return $"{msg1.PadRight(width)}{msg2}";
		}



		/* not cached but tabed methods */

		public static void SetTab2Depth(int depth)
		{
			if (depth >= 0) tabs2 = depth;
		}

		public static void WriteLineBeg(string m1, object? o2 = null, object? o3 = null)
		{
			string? m2 = o2 != null ? o2.ToString() : string.Empty;
			string? m3 = o3 != null ? o3.ToString() : string.Empty;

			WriteLineB("", $"{m1}{m2}{m3}\n");
		}

		public static void WriteLineBeg(string preface, string m1, object? o2 = null, object? o3 = null)
		{
			string? m2 = o2 != null ? o2.ToString() : string.Empty;
			string? m3 = o3 != null ? o3.ToString() : string.Empty;

			WriteLineB(preface, $"{m1}{m2}{m3}\n");
		}

		public static void WriteLineB(string preface, string msg1)
		{
			Write($"{preface}{TAB.Repeat(++tabs2)}{msg1}");
		}


		public static void WriteLineEnd(string m1, object? o2 = null, object? o3 = null)
		{
			string? m2 = o2 != null ? o2.ToString() : string.Empty;
			string? m3 = o3 != null ? o3.ToString() : string.Empty;

			WriteLineE("", $"{m1}{m2}{m3}\n");
		}

		public static void WriteLineEnd(string preface, string m1, object? o2 = null, object? o3 = null)
		{
			string? m2 = o2 != null ? o2.ToString() : string.Empty;
			string? m3 = o3 != null ? o3.ToString() : string.Empty;

			WriteLineE(preface, $"{m1}{m2}{m3}\n");
		}

		public static void WriteLineE(string preface, string msg1)
		{
			Write($"{preface}{TAB.Repeat(++tabs2)}{msg1}");
		}

		
		public static void WriteLineMid(string m1, object? o2 = null, object? o3 = null)
		{
			string? m2 = o2 != null ? o2.ToString() : string.Empty;
			string? m3 = o3 != null ? o3.ToString() : string.Empty;

			WriteLineM("", $"{m1}{m2}{m3}\n");
		}

		public static void WriteLineMid(string preface, string m1, object? o2 = null, object? o3 = null)
		{
			string? m2 = o2 != null ? o2.ToString() : string.Empty;
			string? m3 = o3 != null ? o3.ToString() : string.Empty;

			WriteLineM(preface, $"{m1}{m2}{m3}\n");
		}

		public static void WriteLineM(string preface, string msg1)
		{
			Write($"{preface}{TAB.Repeat(tabs2)}{msg1}");
		}


		// private static string injectFmtCode(string msg, string fmtCode)
		// {
		// 	if (msg.IsVoid()) return msg;
		//
		// 	if (msg.IndexOf("\n") == -1) return $"{fmtCode}{msg}";
		//
		// 	return msg.Replace("\n", $"\n{fmtCode}");
		// }


		/* cached methods */

		private static void CAddCache(string msg)
		{
			if (ExStorMgr.Instance != null) ExStorMgr.Instance.MessageCache += msg;

			if (ShowDebug) Debug.Write(msg);
		}

		public static void CWriteLine(string text, string msg2 = "")
		{
			CAddCache($"{text}{msg2}\n");
		}

		public static void CWrite(string text, string msg2 = "")
		{
			CAddCache($"{text}{msg2}");
		}

		public static void CWriteLineSpaced(string msg1, object? o2 = null, int colWidth = 0)
		{
			int width = colWidth == 0 ? col1Width : colWidth;

			CAddCache(writeSpaced(msg1, o2, width) + "\n");
		}
		
		public static void CWriteSpaced(string msg1, object? o2 = null, int colWidth = 0)
		{
			int width = colWidth == 0 ? col1Width : colWidth;

			CAddCache(writeSpaced(msg1, o2, width));
		}


		public static void CWriteCache()
		{
			if (ExStorMgr.Instance != null && ExStorMgr.Instance.MessageCache != null) Write(ExStorMgr.Instance.MessageCache);
		}

		// tabbed messages

		public static void CWriteLineBeg(string m1, object? o2 = null, object? o3 = null)
		{
			string? m2 = o2 != null ? o2.ToString() : string.Empty;
			string? m3 = o3 != null ? o3.ToString() : string.Empty;

			CWriteLineB("", $"{m1}{m2}{m3}\n");
		}

		public static void CWriteLineBeg(string preface, string m1, object? o2 = null, object? o3 = null)
		{
			string? m2 = o2 != null ? o2.ToString() : string.Empty;
			string? m3 = o3 != null ? o3.ToString() : string.Empty;

			CWriteLineB(preface, $"{m1}{m2}{m3}\n");
		}

		public static void CWriteLineB(string preface, string msg1)
		{
			CAddCache($"{preface}{TAB.Repeat(++tabs2)}{msg1}");
		}


		public static void CWriteLineEnd(string m1, object? o2 = null, object? o3 = null)
		{
			string? m2 = o2 != null ? o2.ToString() : string.Empty;
			string? m3 = o3 != null ? o3.ToString() : string.Empty;

			CWriteLineE("", $"{m1}{m2}{m3}\n");
		}

		public static void CWriteLineEnd(string preface, string m1, object? o2 = null, object? o3 = null)
		{
			string? m2 = o2 != null ? o2.ToString() : string.Empty;
			string? m3 = o3 != null ? o3.ToString() : string.Empty;

			CWriteLineE(preface, $"{m1}{m2}{m3}\n");
		}

		public static void CWriteLineE(string preface, string msg1)
		{
			CAddCache($"{preface}{TAB.Repeat(++tabs2)}{msg1}");
		}

		
		public static void CWriteLineMid(string m1, object? o2 = null, object? o3 = null)
		{
			string? m2 = o2 != null ? o2.ToString() : string.Empty;
			string? m3 = o3 != null ? o3.ToString() : string.Empty;

			CWriteLineM("", $"{m1}{m2}{m3}\n");
		}

		public static void CWriteLineMid(string preface, string m1, object? o2 = null, object? o3 = null)
		{
			string? m2 = o2 != null ? o2.ToString() : string.Empty;
			string? m3 = o3 != null ? o3.ToString() : string.Empty;

			CWriteLineM(preface, $"{m1}{m2}{m3}\n");
		}

		public static void CWriteLineM(string preface, string msg1)
		{
			CAddCache($"{preface}{TAB.Repeat(tabs2)}{msg1}");
		}


		/* system overwrites */

		public new static string ToString()
		{
			return $"{nameof(Msgs)} [{objId}]";
		}


	}
}
