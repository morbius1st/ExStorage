using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using RevitLibrary;
using ExStoreTest2026.Windows;
using ExStorSys;
using UtilityLibrary;

// projname: ExStoreTest2026
// itemname: Command
// username: jeffs
// created:  11/27/2022 4:05:08 PM

namespace ExStoreTest2026
{
	[Transaction(TransactionMode.Manual)]
	public class Command : IExternalCommand
	{
	#region fields

		private int objectId;
		private static MainWindow win;

		private const string ROOT_TRANSACTION_NAME = "Transaction Name";

		// internal static UIControlledApplication RvtUiCtrlApp { get; private set; }
		// internal static ControlledApplication RvtCtrlApp { get; private set; }
		// internal static UIApplication RvtUiApp { get; set; }
		// internal static UIDocument RvtUidoc { get; set; }
		// internal static Application RvtApp { get; set; }
		// internal static Document RvtDoc { get; set; }
		//
		// internal static string FileName => RvtDoc.Title.IsVoid() ? null : RvtDoc.Title;
		// internal static string FilePath => RvtDoc.PathName.IsVoid() ? null : RvtDoc.PathName;

	#endregion

		public Result Execute(
			ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			objectId = AppRibbon.ObjectIdx++;

			R.RvtUiApp = commandData.Application;
			R.RvtUidoc = R.RvtUiApp.ActiveUIDocument;
			R.RvtApp   = R.RvtUiApp.Application;
			R.RvtDoc   = R.RvtUidoc.Document;

			win = new (objectId);
			// win.Hide();

			Result r = showMainWin();

			return Result.Succeeded;
		}

		private Result showMainWin()
		{
			bool? result = win.ShowDialog();

			if (result.HasValue && result.Value) return Result.Succeeded;

			return Result.Cancelled;
			
			// win.Show();
			// return Result.Succeeded;
		}
	}
}