using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ExStoreTest2026.DebugAssist;
using ExStorSys;
using RevitLibrary;
using UtilityLibrary;

namespace ExStoreTest2026.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged, IWin
	{
		public int ObjectId;

		private MainWinModelUi mui ; // = MainWinModelUi.Instance;
		private ExStorStartMgr? stMgr;

		private Window win = new Window();
		private Window rvtWin;

		private string message;


		// private int resultWbkSc = 1;
		// private int resultWbkDs = 1;
		// private int resultShtSc = 1;
		// private int resultShtDs = 1;

		private int test = 0;
		private bool winShown = false;

		// private bool? restartStatus;
		// private string gotWbkSchema;

		public MainWindow()
		{
			init();
		}

		private void init()
		{
			// ObjectId = AppRibbon.ObjectIdx++;

			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(MainWindow)) ?? -1;

			// ui dependant objects must be connected before InitializeComponent elsewise
			// the ui gets connected to a null object that no longer exists after it gets configured
			mui = MainWinModelUi.Instance;

			InitializeComponent();

			Msgs.Mw = this;
			R.Msg = this;
			
			IntPtr ptr = R.RvtUiApp.MainWindowHandle;
			rvtWin = RvtLibrary.WindowHandle(ptr);

			rvtWin.LocationChanged += RvtWinOnLocationChanged;

			// save this
			// configTitleWindow();

			win.Owner = rvtWin;
		}

		/* message text box */

		public string Message
		{
			get => message;
			set
			{
				if (value == message) return;
				message = value;
				OnPropertyChanged();
			}
		}

		public void DebugMsgLine(string msg) => Message += msg + "\n";
		public void DebugMsg(string msg) => Message += msg;
		public void WriteLine(string msg) => DebugMsgLine(msg);

		/* system status */

		public string ObjId {
			get
			{
				ExStorMgr? ex = ExStorMgr.Instance;

				string m1 = "null";
				string m2 = "null";
				string m3 = ExStorStartMgr.Instance == null ? "null" : "good";
				string m4 = "null";
				string m5 = "null";

				if (ex != null)
				{
					m1 = ex.ObjectId.ToString();
					m2 = ex.xData.ObjectId.ToString();
					m4 = Mui?.ObjectId.ToString() ?? "null";
					m5 = ExStorStartMgr.Instance?.ObjectId.ToString() ?? "null";
				}

				return $"[{ObjectId}] | xMgr [{m1}] | xData [{m2}] | mui [{m4}] | stMgr [{m3}]" ;
			}
	}

		// three system status methods
		// restart required - flags that a restart of revit must occur before "any" furthur
		//		ExSys operations can be preformed
		// ExSysStatus - flags the general state of the system

		/* properties */

		public MainWinModel wm { get; set; } = new ();
		public MainWinModelUi Mui => mui;

		/* process events */

		private void RvtWinOnLocationChanged(object? sender, EventArgs e)
		{
			if (win.Visibility == Visibility.Visible)
			{
				win.Top = rvtWin.Top;
				win.Left = rvtWin.Left;
				win.Width = rvtWin.Width;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[DebuggerStepThrough]
		// [NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}


		public override string ToString()
		{
			return $"{nameof(MainWindow)} [{ObjectId}]";
		}

		/* process ui events */

		
		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			// this.Visibility = Visibility.Collapsed;
			//
			// Msgs.SetTab2Depth(0);
			//
			// Msgs.CWriteLine("begin start manager");
			//
			// stMgr = ExStorStartMgr.Create();
			//
			// bool status = stMgr.Start();
			//
			// stMgr = null;
			//
			// if (!status)
			// {
			// 	
			// 	this.Close();
			//
			// 	e.Handled = true;
			// }
			// else
			// {
			// 	this.Visibility = Visibility.Visible;
			// 	this.Show();
			// }
			//
			//
			// Msgs.CWriteLine("end start manager");			
		}

		private void MainWin_Activated(object sender, EventArgs e)
		{
			OnPropertyChanged(nameof(ObjId));

			Msgs.WriteLine($"window activated | model is {R.RvtDoc?.Title ?? "no title"} | model name | {ExStorMgr.Instance.xData.WorkBook?.Model_Name ?? "null"}");

			wm.ShowMsgCache();
		}

		// private void MainWin_SourceInitialized(object sender, EventArgs e)
		// {
		// 	// Msgs.SetTab2Depth(0);
		// 	//
		// 	// Msgs.CWriteLine("begin start manager");
		// 	//
		// 	// stMgr = ExStorStartMgr.Create();
		// 	//
		// 	// bool status = stMgr.Start();
		// 	//
		// 	// stMgr = null;
		// 	//
		// 	// if (!status)
		// 	// {
		// 	// 	this.Close();
		// 	// 	return;
		// 	// }
		// 	//
		// 	// Msgs.CWriteLine("end start manager");	
		//
		// }

		// private void MainWin_SourceInitialized(object sender, EventArgs e)
		// {
		// 	Msgs.SetTab2Depth(0);
		//
		// 	Msgs.CWriteLine("begin start manager");
		//
		// 	Msgs.CWriteLineMid($"ExSysStatus = {mui.ExSysStatus}");
		//
		// 	stMgr = ExStorStartMgr.Create();
		//
		// 	Msgs.CWriteLineBeg("begin start driver");
		// 	
		// 	ExSysStatus status = stMgr.StartDriver();
		//
		// 	Msgs.CWriteLineEnd("end start driver");
		// 	
		// 	if (status == ExSysStatus.ES_VRFY_DONE_FAIL) this.Close();
		//
		// 	stMgr = null;
		//
		// 	Msgs.CWriteLine("end start manager");
		//
		// }


		private void BtnExit_OnClick(object sender, RoutedEventArgs e)
		{
			// this.Close();
			this.Hide();
			
		}

		private void BtnDebug_OnClick(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("** at debug **");

			string a = R.RvtDoc?.Title ?? String.Empty;

		}

		private void BtnClear_OnClick(object sender, RoutedEventArgs e)
		{
			Message = "";
		}

		private void ShowInfoOne_OnClick(object sender, RoutedEventArgs e)
		{
			wm.ShowInfo1();
		}

		private void ShowInfoTwo_OnClick(object sender, RoutedEventArgs e)
		{
			wm.ShowInfo2();
		}

		private void ShowExMgr_OnClick(object sender, RoutedEventArgs e)
		{
			wm.ShowExMgr();
		}

		private void BtnMakeWkBk_OnClick(object sender, RoutedEventArgs e)
		{
			wm.MakeWorkBook();
		}

		private void BtnMakeEmptyWkBk_OnClick(object sender, RoutedEventArgs e)
		{
			wm.MakeEmptyWorkBook();
		}

		private void BtnShowWbk_OnClick(object sender, RoutedEventArgs e)
		{
			wm.ShowExData();
		}

		private void BtnMakeSht_OnClick(object sender, RoutedEventArgs e)
		{
			wm.MakeSheet();
		}

		private void BtnMakeAndWriteWbk_OnClick(object sender, RoutedEventArgs e)
		{
			wm.MakeAndWriteWorkBook();
		}

		private void BtnClearAndReadbk_OnClick(object sender, RoutedEventArgs e)
		{
			wm.ClearAndReadWorkBook();
		}

		private void BtnMakeEmptySht_OnClick(object sender, RoutedEventArgs e)
		{
			wm.MakeEmptySheet();
		}

		private void BtnShowSht_OnClick(object sender, RoutedEventArgs e)
		{
			wm.ShowSheets();
		}

		private void BtnChgWbk_OnClick(object sender, RoutedEventArgs e)
		{
			wm.ChangeWorkBook();
		}

		private void BtnWriteSht_OnClick(object sender, RoutedEventArgs e)
		{
			wm.MakeAndWriteSheet();
		}

		private void BtnFindWbkDs_OnClick(object sender, RoutedEventArgs e)
		{
			wm.FindWorkBookDs();
		}

		private void BtnFindShtDs_OnClick(object sender, RoutedEventArgs e)
		{
			wm.FindSheetDs();
		}

		private void BtnFindAllShtDs2_OnClick(object sender, RoutedEventArgs e)
		{
			wm.FindAllSheetDs();
		}

		private void BtnFindAllShtDs_OnClick(object sender, RoutedEventArgs e)
		{
			wm.FindAllSheetDs();
		}

		private void BtnFindAndShowAll_OnClick(object sender, RoutedEventArgs e)
		{
			wm.FindAndShowElements();
		}

		private void BtnShowObjId_OnClick(object sender, RoutedEventArgs e)
		{
			wm.ShowObjectId(ObjectId);
		}

		private void BtnClearReadAll_OnClick(object sender, RoutedEventArgs e)
		{
			wm.ClearAndReadAll();
		}

		private void BtnReadAndShowAll_OnClick(object sender, RoutedEventArgs e)
		{
			wm.ReadAllFromTemp();

			Msgs.WriteLine("****  show info read from memory ****\n");

			DebugRoutines.ShowWorkBookFromMemory();
			DebugRoutines.ShowSheetsFromMemory();
		}
		
		// this routine does work to create a overlay window (Msg bar) to cover the title area of a
		// revit window.  note that this window cannot be modeless as the msg bar's size & location
		// will not update if the main revit window size changes.  also, make sure to implement a 
		// method to destroy the window if this app closes for ANY reason
		private void BtnChgWin_OnClick(object sender, RoutedEventArgs e)
		{
			double top = rvtWin.Top;
			double left = rvtWin.Left;
			double width = rvtWin.ActualWidth;
			double height = 24;

			// MainWindow_ContentBorder (Border)
			// MainWindow_Caption (DockPanel)
			// TitlePanel (DockPanel)
			// MainWindow_ContentGrid (Grid)

			// TextBlock t = CsWpfUtilities.FindElementByName<TextBlock>(w, "TitleNameText", true);
			// Border b = CsWpfUtilities.FindElementByName<Border>(w, "MainWindow_ContentBorder", true);
			// DockPanel dp2 = CsWpfUtilities.FindElementByName<DockPanel>(w, "TitlePanel", true);
			// Grid gd1 = CsWpfUtilities.FindElementByName<Grid>(w, "MainWindow_ContentGrid");

			// IntPtr ptr1 = RvtLibrary.FindWindowEx(ptr, IntPtr.Zero, "MainWindow_Caption", "");
			// Window w1 = RvtLibrary.WindowHandle(ptr1);
			DockPanel dp1 = CsWpfUtilities.FindElementByName<DockPanel>(rvtWin, "MainWindow_Caption");

			if (dp1 != null && dp1.ActualHeight > 0)
			{
				// TextBlock tb = new TextBlock();
				// tb.Background = Brushes.DarkOrange;
				// tb.Foreground = Brushes.White;
				// tb.FontWeight = FontWeights.Bold;
				// tb.FontSize = 18.0;
				// tb.Text = "this is a text block";
				// tb.HorizontalAlignment = HorizontalAlignment.Center;
				// tb.VerticalAlignment = VerticalAlignment.Center;
				//
				// win.Visibility = Visibility.Collapsed;
				// win.WindowStyle = WindowStyle.None;
				// win.Background = Brushes.DarkOrange;
				//
				// win.Content = tb;
				// win.BorderBrush = Brushes.Transparent;
				// win.BorderThickness = new Thickness(0);
				// win.ResizeMode = ResizeMode.NoResize;

				win.Top = top;
				win.Left = left + 1;
				win.Height = height;
				win.Width = width - 2;

				if (test++ % 2 == 0)
				{
					if (!winShown)
					{
						win.Show();
						winShown = true;
						this.Owner = win;
					}

					win.Visibility = Visibility.Visible;
				}
				else
				{
					win.Visibility = Visibility.Collapsed;
				}
			}
		}

		private void configTitleWindow()
		{
			TextBlock tb = new TextBlock();
			tb.Background = Brushes.DarkOrange;
			tb.Foreground = Brushes.White;
			tb.FontWeight = FontWeights.Bold;
			tb.FontSize = 18.0;
			tb.Text = "this is a text block";
			tb.HorizontalAlignment = HorizontalAlignment.Center;
			tb.VerticalAlignment = VerticalAlignment.Center;

			win.Visibility = Visibility.Collapsed;
			win.WindowStyle = WindowStyle.None;
			win.Background = Brushes.DarkOrange;
			win.Content = tb;
			win.BorderBrush = Brushes.Transparent;
			win.BorderThickness = new Thickness(0);
			win.ResizeMode = ResizeMode.NoResize;
		}

		private void MainWin_Closing(object sender, CancelEventArgs e)
		{
			win.Close();
		}

		private void BtnTstWbkFalseModelName_OnClick(object sender, RoutedEventArgs e)
		{
			wm.WbkWithFalseModelName();
		}

		// private void BtnTstShtFalseModelCode_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	wm.ShtWithFalseModelCode();
		// }

		private void BtnStartLaunchMgr_OnClick(object sender, RoutedEventArgs e)
		{
			wm.StartLaunchManager();
		}

		/* startup */

		private void BtnVerifyStartup_OnClick(object sender, RoutedEventArgs e)
		{
			wm.TestOnOpenDocLaunch();
		}

		/* delete */

		// private void BtnDeleteWbkSc_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	wm.DeleteWbkSc();
		// }

		// private void BtnDeleteShtSc_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	wm.DeleteShtSc();
		// }

		private void BtnDeleteOneSht_OnClick(object sender, RoutedEventArgs e)
		{
			wm.DeleteFirstShtDs();
		}

		private void BtnDeleteShts_OnClick(object sender, RoutedEventArgs e)
		{
			wm.DeleteShtDs();
		}

		private void BtnDeleteWbk_OnClick(object sender, RoutedEventArgs e)
		{
			wm.DeleteWbkDs();
		}

		// private void BtnDeleteWbkDs_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	wm.DeleteWbkDs();
		// }

		// private void BtnDeleteShtDs_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	wm.DeleteShtDs();
		// }

		private void BtnVerifyTest_OnClick(object sender, RoutedEventArgs e)
		{
			wm.TestVerify();
		}

		private void BtnTstWbkSetNotActive_OnClick(object sender, RoutedEventArgs e)
		{
			wm.SetWbkToInactive();
		}

	}
}