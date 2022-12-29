#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ExStoreTest.Windows;

#endregion

// user name: jeffs
// created:   11/27/2022 4:29:06 PM

namespace ExStoreTest.Support
{
	public static class Msgs
	{

		public static MainWindow Mw { get; set; }

		static Msgs() { }

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

		public static void WriteLineSpaced(string msg1, string msg2 = "")
		{

			Mw.Message += $"{msg1.PadRight(15)}{msg2}\n";

		}

		public static  string ToString()
		{
			return $"this is {nameof(Msgs)}";
		}
	}
}
