// using System;
// using System.Collections.Generic;
// using System.ComponentModel;
// using System.Linq;
// using System.Runtime.CompilerServices;
// using System.Text;
// using System.Threading.Tasks;
// using UtilityLibrary;

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

	#endregion

	#region ctor

		private R() {}

		public static R Instance => instance.Value;
	#endregion

	#region public properties

		public static IWin Msg { get; set; }

		public static UIApplication? RvtUiApp { get; set; }
		public static UIDocument? RvtUidoc { get; set; }
		public static Application? RvtApp { get; set; }
		public static Document? RvtDoc { get; set; }

		public static string? FileName => RvtDoc?.Title;
		public static string? FilePath => RvtDoc?.PathName;

		public static int OpenDocCount => RvtApp?.Documents.Size ?? -1;
		public static bool NoDocsOpen => OpenDocCount == 0;
		public static bool OnlyOneDocOpen => OpenDocCount == 1;
		public static bool MultipleDocsOpen => OpenDocCount > 1;

	#endregion

		public override string ToString()
		{
			return $"this is {nameof(R)}";
		}
	}
}