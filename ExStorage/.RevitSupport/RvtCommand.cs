#region using

using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExStorage.Windows;
using UtilityLibrary;
using static UtilityLibrary.MessageUtilities;

#endregion

// projname: ExStorage
// itemname: Command
// username: jeffs
// created:  12/27/2021 7:15:47 PM

namespace RevitSupport
{
	[Transaction(TransactionMode.Manual)]
	public class RvtCommand : IExternalCommand
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


			OutLocation = OutputLocation.DEBUG;

			// Access current selection
			// Selection sel = Command.rvt_Uidoc.Selection;

			Result r = showMainWin();

			return Result.Succeeded;
		}

	#endregion

	#region public methods

	#endregion

	#region private methods

		private Result showMainWin()
		{
			MainWindow win = new MainWindow();

			bool? result = win.ShowDialog();

			if (result.HasValue && result.Value) return Result.Succeeded;

			return Result.Cancelled;
		}

		private FilteredElementCollector getFilteredElements()
		{
			// Retrieve elements from database
			FilteredElementCollector col
				= new FilteredElementCollector(RvtCommand.RvtDoc)
				.WhereElementIsNotElementType()
				.OfCategory(BuiltInCategory.INVALID)
				.OfClass(typeof(Wall));

			return col;
		}

		private void protectedProcedure(FilteredElementCollector col)
		{
			// Filtered element collector is iterable
			foreach (Element e in col)
			{
				Debug.Print(e.Name);
			}

			// Modify document within a transaction
			using (Transaction tx = new Transaction(RvtCommand.RvtDoc))
			{
				tx.Start(ROOT_TRANSACTION_NAME);
				tx.Commit();
			}
		}

	#endregion
	}
}