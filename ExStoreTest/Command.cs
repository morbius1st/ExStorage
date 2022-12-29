#region using

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExStoreTest.Windows;
using UtilityLibrary;

#endregion

// projname: ExStoreTest
// itemname: Command
// username: jeffs
// created:  11/27/2022 4:05:08 PM

namespace ExStoreTest
{
	[Transaction(TransactionMode.Manual)]
	public class Command : IExternalCommand
	{
	#region fields

		private const string ROOT_TRANSACTION_NAME = "Transaction Name";

		internal static UIControlledApplication RvtUiCtrlApp { get; private set; }
		internal static ControlledApplication RvtCtrlApp { get; private set; }
		internal static UIApplication RvtUiApp { get; set; }
		internal static UIDocument RvtUidoc { get; set; }
		internal static Application RvtApp { get; set; }
		internal static Document RvtDoc { get; set; }

		internal static string FileName => RvtDoc.Title.IsVoid() ? null : RvtDoc.Title;
		internal static string FilePath => RvtDoc.PathName.IsVoid() ? null : RvtDoc.PathName;

	#endregion

	#region entry point: Execute

		public Result Execute(
			ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			RvtUiApp = commandData.Application;
			RvtUidoc = RvtUiApp.ActiveUIDocument;
			RvtApp   = RvtUiApp.Application;
			RvtDoc   = RvtUidoc.Document;

			Result r = showMainWin();

			return Result.Succeeded;

			// // Access current selection
			// Selection sel = uidoc.Selection;
			//
			// // Retrieve elements from database
			// FilteredElementCollector col
			// 	= new FilteredElementCollector(doc)
			// 	.WhereElementIsNotElementType()
			// 	.OfCategory(BuiltInCategory.INVALID)
			// 	.OfClass(typeof(Wall));
			//
			// // Filtered element collector is iterable
			// foreach (Element e in col)
			// {
			// 	Debug.Print(e.Name);
			// }
			//
			// // Modify document within a transaction
			// using (Transaction tx = new Transaction(doc))
			// {
			// 	tx.Start(ROOT_TRANSACTION_NAME);
			// 	tx.Commit();
			// }


		}


		private Result showMainWin()
		{
			MainWindow win = new MainWindow();

			bool? result = win.ShowDialog();

			if (result.HasValue && result.Value) return Result.Succeeded;

			return Result.Cancelled;
		}
	}

#endregion

#region public methods

#endregion

#region private methods

#endregion
}