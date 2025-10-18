using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ExStoreTest2026.Windows;


// user name: jeffs
// created:   11/27/2022 4:29:06 PM

namespace ExStoreTest2026.DebugAssist
{
	public static class Msgs
	{
		private static int col1Width;
		public static MainWindow Mw { get; set; }

		static Msgs()
		{
			Col1width = 0;
		}

		public static int Col1width
		{
			get => col1Width;
			set
			{
				if (value <= 0) value = 24;
				col1Width = value;
			}
		}

		public static void WriteLine(string text, string msg2 = "")
		{
			Mw.Message += $"{text}{msg2}\n";
		}

		public static void Write(string text)
		{
			Mw.Message += text;
		}

		public static void NewLine()
		{
			Mw.Message += "\n";
		}

		public static void WriteLineSpaced(string msg1, string msg2 = "", int colWidth = 0)
		{
			int width = colWidth == 0 ? Col1width : colWidth;

			// width = msg1.Length > width ? msg1.Length : width - msg1.Length;

			Mw.Message += $"{msg1.PadRight(width)}{msg2}\n";

		}

		public static  string ToString()
		{
			return $"this is {nameof(Msgs)}";
		}
	}
}
