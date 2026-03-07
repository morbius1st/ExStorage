#region using
using System.Diagnostics;
using System.IO;
using System.Windows.Markup;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

using ExStoreTest2026.DebugAssist;
using ExStoreTest2026.Windows;

using ExStorSys;

using RevitLibrary;

using UtilityLibrary;

#endregion

// projname: ExStoreTest2026
// itemname: App
// username: jeffs
// created:  11/27/2022 4:05:08 PM

namespace ExStoreTest2026
{
	internal class AppRibbon : IExternalApplication
	{
		public static int objectIdx = 0;
		public static int ObjectIdx
		{
			get => objectIdx;
			set => objectIdx = value;
		}

		public Result OnShutdown(UIControlledApplication a)
		{
			return Result.Succeeded;
		}
		// application: launch with revit - setup interface elements
		// display information

		private const string NAMESPACE_PREFIX = "ExStoreTest2026.Assets";

		private const string PANEL_NAME = "ExStoreTest2026";

		private const string BUTTON_NAME = "ExStoreTest2026";
		private const string BUTTON_NAME_ALT = "ExStoreTest2026Alt";
		private const string BUTTON_TEXT = "ExStoreTest2026";
		private const string BUTTON_TEXT_ALT = "ExStoreTest2026_Alt";
		private const string BUTTON_TOOL_TIP = "Button Tool Tip";

		private const string COMMAND_CLASS_NAME = "Command";
		private const string COMMAND_CLASS_NAME_ALT = "CommandAlt";

		private static string AddInPath = typeof(AppRibbon).Assembly.Location;
		private const string CLASSPATH = "ExStoreTest2026.";

		private const string SMALLICON = "information16.png";
		private const string LARGEICON = "information32.png";


		internal UIApplication uiApp;

		public Result OnStartup(UIControlledApplication app)
		{

			// ExMgrs = new Dictionary<string, ExStorMgr>();
			stMgrs = new Dictionary<string, ExStorStartMgr>();
			try
			{
				app.ControlledApplication.ApplicationInitialized += OnAppInitalized;

				// create the ribbon tab first - this is the top level
				// ui item.  below this will be the panel that is "on" the tab
				// and below this will be a pull down or split button that is "on" the panel;

				// give the panel a name
				string panelName = PANEL_NAME;

				// create the ribbon panel if needed
				RibbonPanel ribbonPanel = null;

				ribbonPanel = app.CreateRibbonPanel(panelName);

				ribbonPanel.AddItem(
					createButton(BUTTON_NAME_ALT, BUTTON_TEXT_ALT, COMMAND_CLASS_NAME_ALT,
						BUTTON_TOOL_TIP, SMALLICON, LARGEICON));

				ribbonPanel.AddItem(
					createButton(BUTTON_NAME, BUTTON_TEXT, COMMAND_CLASS_NAME,
						BUTTON_TOOL_TIP, SMALLICON, LARGEICON));

			}
			catch (Exception e)
			{
				Debug.WriteLine("exception " + e.Message);
				return Result.Failed;
			}

			return Result.Succeeded;
		}

		private PushButtonData createButton(string ButtonName, string ButtonText,
			string className, string ToolTip, string smallIcon, string largeIcon)
		{
			PushButtonData pbd;

			try
			{
				pbd = new PushButtonData(ButtonName, ButtonText, AddInPath, string.Concat(CLASSPATH, className))
				{
					Image = CsUtilitiesMedia.GetBitmapImage(smallIcon, NAMESPACE_PREFIX),
					LargeImage = CsUtilitiesMedia.GetBitmapImage(largeIcon, NAMESPACE_PREFIX),
					ToolTip = ToolTip
				};
			}
			catch
			{
				return null;
			}

			return pbd;
		}

		private void OnAppInitalized(object sender, ApplicationInitializedEventArgs e)
		{
			app = sender as Application;

			R.RvtApp = app;

			uiApp = new UIApplication(app);

			R.RvtUiApp = uiApp;

			// app.DocumentOpened += AppOnDocumentOpened;
			app.DocumentOpening += AppOnDocumentOpening;
			app.DocumentClosing += AppOnDocumentClosing;
			
			app.DocumentSavingAs += AppOnDocumentSavingAs;
			app.DocumentSavedAs += AppOnDocumentSavedAs;
			uiApp.ViewActivated += UiAppOnViewActivated;

			

			/* currently not in use

			app.DocumentClosed += AppOnDocumentClosed;

			app.DocumentSaving += AppOnDocumentSaving;
			app.DocumentSaved += AppOnDocumentSaved;
			app.DocumentSavingAs += AppOnDocumentSavingAs;
			app.DocumentSavedAs += AppOnDocumentSavedAs;

			app.DocumentCreating += AppOnDocumentCreating;
			app.DocumentCreated += AppOnDocumentCreated;
			*/
		}

		private static Application app;
		private bool docJustOpened = false;
		private bool docIsClosing = false;
		// private Dictionary<string, ExStorMgr> ExMgrs;
		private Dictionary<string, ExStorStartMgr> stMgrs;

		// private void AppOnDocumentOpened(object? sender, DocumentOpenedEventArgs e)
		// {
		// 	// docStatus("opened");
		// }

		private void AppOnDocumentOpening(object? sender, DocumentOpeningEventArgs e)
		{
			// file is the file being opened | R.RvtDoc is null and count is 0

			// set the current documents
			R.RvtUidoc = R.RvtUiApp!.ActiveUIDocument;
			R.RvtDoc = R.RvtUidoc?.Document;


			// string docType = e?.DocumentType.ToString() ?? "null";
			// string file = Path.GetFileName(e?.PathName ?? "\\no file.txt");
			
			// docStatus($"opening ({file}) | ({docType})");
			docJustOpened = true;
		}

		private void AppOnDocumentClosing(object? sender, DocumentClosingEventArgs e)
		{	
			// for just created project, R.RvtDoc is the temp name
			// docStatus("closing");
			docIsClosing = true;
			// RemoveExMgr(e.Document.Title);
			RemoveExStMgr(e.Document.Title);
		}

		private void AppOnDocumentSavingAs(object? sender, DocumentSavingAsEventArgs e)
		{
			// doc name is the original project | file is the path to the new project | R.RvtDoc is still the original project
			// string docName = e?.Document.Title ?? "null";
			// string file = Path.GetFileName(e?.PathName ?? "\\no file.txt");
			// docStatus($"saving as | doc name {docName} | file {file}");

			RemoveExStMgr(e.Document.Title);
		}

		private void AppOnDocumentSavedAs(object? sender, DocumentSavedAsEventArgs e)
		{
			ExStorMgr i = ExStorMgr.Instance;

			// set the current documents
			R.RvtUidoc = R.RvtUiApp!.ActiveUIDocument;
			R.RvtDoc = R.RvtUidoc?.Document;

			ExStorMgr.Instance?.xData.ResetAll();

			Debug.WriteLine($"** model names | {ExStorMgr.Instance?.Exid.ModelTitle} | {ExStorMgr.Instance?.Exid.ModelName}");

			docViewJustOpened(e.Document.Title);
		}

		private void UiAppOnViewActivated(object? sender, ViewActivatedEventArgs e)
		{
			// tab switch - doc is the final project | R.RvtDoc is the orig project
			string doc = e.Document.Title;

			// docStatus($"view activated | doc {doc}");

			// this occurs before "on document opened"
			// if null, got here from startup screen
			if (!(e.PreviousActiveView?.Document?.Equals(e.CurrentActiveView.Document) ?? false ))
			{
				ExStorMgr xMgr;
				ExStorStartMgr stMgr;

				R.RvtDoc = e.Document;
				R.RvtUidoc = ((UIApplication) sender).ActiveUIDocument;

				if (docJustOpened)
				{
					docViewJustOpened(e.Document.Title);

					// docJustOpened = false;
					//
					// // if (!AddExMgr(e.Document.Title)) return;
					// if (!AddExStMgr(e.Document.Title)) return;
					// // ExStorMgr.Instance = ExMgrs[e.Document.Title];
					//
					// ExStorStartMgr.Instance = stMgrs[e.Document.Title];
					//
					// // ExStorStartMgr.Instance.Restore();
					// // ExStorMgr.Instance = ExStorStartMgr.Instance.xMgr;
					// // ExStorMgr.Instance.MessageCache = String.Empty;
					// // ExStorStartMgr.Instance.xMgr.MessageCache = String.Empty;
					//
					// // ExStorMgr.Instance.StartWinUi();
					//
					// ExStorLaunchMgr lMgr = ExStorLaunchMgr.Create();
					//
					// // this sets the Launch Status in the Mui - then
					// // the main window checks this, upon source init
					// // and starts the start manager to determine
					// // the next steps / read the exstor data or not.
					// lMgr.OnOpenDocLaunch();
					//
					// // lMgr.TestVfy1();
				}
				else 
				if (!docIsClosing)
				{
					

					// if (ExMgrs.TryGetValue(e.Document.Title, out xMgr))
					// {
					// 	ExStorMgr.Instance = xMgr;
					// 	ExStorMgr.Instance.MessageCache = String.Empty;
					// 	xMgr.Mui.LaunchCode = LaunchCode.LC_NA;
					// 	xMgr.Mui.ExSysStatus = ExSysStatus.ES_STARTED;
					// }

					if (stMgrs.TryGetValue(e.Document.Title, out stMgr))
					{
						ExStorLaunchMgr.Instance = null;

						ExStorStartMgr.Instance = stMgr;
						stMgr.Restore();
						// ExStorLaunchMgr.Instance?.Restore();

						// ExStorMgr.Instance = stMgr.xMgr;
						// stMgr.xMgr.MessageCache = String.Empty;
						// stMgr.xMgr.Mui.LaunchCode = LaunchCode.LC_NA;
						// stMgr.xMgr.Mui.ExSysStatus = ExSysStatus.ES_STARTED;

						stMgr.ShowStatus4("from app ribbon");
					}
				}
				else
				{
					docIsClosing = false;
				}
			}
		}


		private void docViewJustOpened(string docTitle)
		{
			ExStorMgr xMgr;
			ExStorStartMgr stMgr;

			docJustOpened = false;

			// if (!AddExMgr(e.Document.Title)) return;
			if (!AddExStMgr(docTitle)) return;
			// ExStorMgr.Instance = ExMgrs[e.Document.Title];
				
			ExStorStartMgr.Instance = stMgrs[docTitle];

			// ExStorStartMgr.Instance.Restore();
			// ExStorMgr.Instance = ExStorStartMgr.Instance.xMgr;
			// ExStorMgr.Instance.MessageCache = String.Empty;
			// ExStorStartMgr.Instance.xMgr.MessageCache = String.Empty;

			// ExStorMgr.Instance.StartWinUi();

			ExStorLaunchMgr lMgr = ExStorLaunchMgr.Create();

			// this sets the Launch Status in the Mui - then
			// the main window checks this, upon source init
			// and starts the start manager to determine
			// the next steps / read the exstor data or not.
			lMgr.OnOpenDocLaunch();
		}


		private bool AddExStMgr(string? key)
		{
			if (key.IsVoid()) return false;
			if (!stMgrs.ContainsKey(key))
			{
				stMgrs.Add(key, ExStorStartMgr.Create());
			}

			return true;
		}

		private void RemoveExStMgr(string? key)
		{
			if (key.IsVoid()) return;
			if (stMgrs.ContainsKey(key)) { stMgrs.Remove(key); }
		}


		// private bool AddExMgr(string? key)
		// {
		// 	if (key.IsVoid()) return false;
		// 	if (!ExMgrs.ContainsKey(key))
		// 	{
		// 		ExMgrs.Add(key, ExStorMgr.Create());
		// 	}
		//
		// 	return true;
		// }
		//
		// private void RemoveExMgr(string? key)
		// {
		// 	if (key.IsVoid()) return;
		// 	if (ExMgrs.ContainsKey(key)) { ExMgrs.Remove(key); }
		// }



		private void docStatus(string location)
		{
			Debug.WriteLine($"doc stat | {location,-35} | count {R.OpenDocCount,-5} | doc {R.RvtDoc?.Title ?? "null"}");
		}



		/* study only for now*/



		private void AppOnDocumentSaved(object? sender, DocumentSavedEventArgs e)
		{
			// doc name is the current / being saved project | status is save status | R.RvtDoc is current project
			string docName = e?.Document.Title ?? "null";
			string status = e.Status.ToString();

			docStatus($"saved as | doc name {docName} | status {status}");
		}

		private void AppOnDocumentCreated(object? sender, DocumentCreatedEventArgs e)
		{
			// docname is the temp name from revit | R.RvtDoc is the temp name that revit provides
			string docName = e?.Document.Title ?? "null";
			docStatus($"created | doc name {docName}");
		}

		// need saved / created

		private void AppOnDocumentCreating(object? sender, DocumentCreatingEventArgs e)
		{	
			// doc type is e.g. project | R.RvtDoc is the orig project
			string docType = e?.DocumentType.ToString() ?? "null";
			docStatus($"creating ({docType})");
		}

		// private void AppOnDocumentSavedAs(object? sender, DocumentSavedAsEventArgs e)
		// {
		// 	// doc name is the new file | orig path is the orig file | R.RvtDoc is the new project
		// 	string docName = e?.Document.Title ?? "null";
		// 	string file = Path.GetFileName(e?.OriginalPath ?? "\\no file.txt");
		// 	
		// 	docStatus($"saved as | doc name {docName} | file {file}");
		// }

		private void AppOnDocumentSaving(object? sender, DocumentSavingEventArgs e)
		{	
			// R.RvtDoc is the current / active project
			docStatus("saving");
		}

		private void AppOnDocumentClosed(object? sender, DocumentClosedEventArgs e)
		{	
			// R.RvtDoc is the name of the current & active project
			docStatus("closed");
		}


	}
}

/* event sequences
 * creating
 *	creating
 *	activate view
 *	created
 *
 *close document
 *	closing
 *	activate view
 *	closed
 *
 * open
 *	opening
 *	view activated
 *	opened
 *
 * save
 *	saving
 *	saved
 *
 * saving as
 *	saving as
 *	saved as
 *
 * switch project tabs
 *	view activated
 *
 */