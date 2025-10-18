using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
		private int objectId;
		private int cmdId;

		public MainWinModel wm { get; set; }= new ();
		
		private string message;

		private Window win = new Window();
		private Window rvtWin;

		private int resultWbkSc = 1;
		private int resultWbkDs = 1;
		private int resultShtSc = 1;
		private int resultShtDs = 1;

		private int test = 0;
		private bool winShown = false;
		private bool? restartStatus;

		private string gotWbkSchema;

		public MainWindow(int cmdId)
		{
			objectId = AppRibbon.ObjectIdx++;
			this.cmdId = cmdId;

			InitializeComponent();

			Msgs.Mw = this;
			R.Msg = this;
			// wm = new MainWinModel();

			IntPtr ptr = R.RvtUiApp.MainWindowHandle;
			rvtWin = RvtLibrary.WindowHandle(ptr);

			rvtWin.LocationChanged += RvtWinOnLocationChanged;
			
			configTitleWindow();
			win.Owner = rvtWin;

			ExStorMgr.Instance.RestartReqdChanged += InstanceOnRestartReqdChanged;
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

		/* properties */

		public int WbkScResCode
		{
			get => resultWbkSc;
			set
			{
				if (value != resultWbkSc) return;
				resultWbkSc = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(WbkScResDesc));
			}
		}
		public string WbkScResDesc => $"WBK {ExStorConst.ScValidateResults[resultWbkSc]}";

		public int WbkDsResCode
		{
			get => resultWbkDs;
			set
			{
				if (value != resultWbkDs) return;
				resultWbkDs = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(WbkDsResDesc));
			}
		}
		public string WbkDsResDesc => $"WBK {ExStorConst.DsValidateResults[resultWbkDs]}";

		public int ShtScResCode
		{
			get => resultShtSc;
			set
			{
				if (value != resultShtSc) return;
				resultShtSc = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ShtScResDesc));
			}
		}
		public string ShtScResDesc => $"SHT {ExStorConst.ScValidateResults[resultShtSc]}";

		public int ShtDsResCode
		{
			get => resultShtDs;
			set
			{
				if (value != resultShtDs) return;
				resultShtDs = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ShtDsResDesc));
			}
		}
		public string ShtDsResDesc => $"SHT {ExStorConst.DsValidateResults[resultShtDs]}";


		public string RestartStatusDesc
		{
			get => restartStatus.HasValue ? (restartStatus.Value ? "Restart Needed" : "Restart Not Needed") : "Not Applicable";
		}

		public bool? RestartStatus
		{
			get => restartStatus;
			set
			{
				if (value == restartStatus) return;
				restartStatus = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(RestartStatusDesc));
			}
		}

		public string GotWbkSchema
		{
			get => gotWbkSchema;
			set
			{
				if (value == gotWbkSchema) return;
				gotWbkSchema = value;
				OnPropertyChanged();
			}
		}


		private void updateStatProps()
		{
			OnPropertyChanged(nameof(WbkScResCode));
			OnPropertyChanged(nameof(WbkScResDesc));

			OnPropertyChanged(nameof(WbkDsResCode));
			OnPropertyChanged(nameof(WbkDsResDesc));

			OnPropertyChanged(nameof(ShtScResCode));
			OnPropertyChanged(nameof(ShtScResDesc));

			OnPropertyChanged(nameof(ShtDsResCode));
			OnPropertyChanged(nameof(ShtDsResDesc));
		}

		/* process events */

		private void InstanceOnSettingChanged(object sender, SettingChangedEventArgs e)
		{
			switch (e.SettingId)
			{
				case SettingId.SI_GOT_WBK_SCHEMA:
					{
						if (e.Value.AsBool())
						{
							GotWbkSchema = "Yes, got Wbk schema";
						}
						else
						{
							GotWbkSchema = "Nope, don't got schema";
						}
						break;
					}
			}
		}

		private void RvtWinOnLocationChanged(object? sender, EventArgs e)
		{
			if (win.Visibility == Visibility.Visible)
			{
				win.Top = rvtWin.Top;
				win.Left = rvtWin.Left;
				win.Width = rvtWin.Width;
			}
		}

		private void InstanceOnRestartReqdChanged(object sender, bool? e)
		{
			RestartStatus = e;
			OnPropertyChanged(nameof(RestartStatus));
			OnPropertyChanged(nameof(RestartStatusDesc));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[DebuggerStepThrough]
		// [NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

		/* process ui events */

		private void BtnExit_OnClick(object sender, RoutedEventArgs e)
		{
			// this.Close();
			this.Hide();
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

		private void BtnClearReadAndShowAll_OnClick(object sender, RoutedEventArgs e)
		{
			wm.FindAndReadAllShtDs();
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
				win.Left = left+1;
				win.Height = height;
				win.Width = width-2;

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

		private void MainWin_Activated(object sender, EventArgs e)
		{
			Msgs.WriteLine($"window activated | model is {R.RvtDoc?.Title ?? "no title"} | model name | {ExStorMgr.Instance.WorkBook?.Model_Name ?? "null"}");
        }

		private void BtnInitVerify_OnClick(object sender, RoutedEventArgs e)
		{
			wm.InitialVerify(out resultWbkSc, out resultWbkDs, out resultShtSc, out resultShtDs);
		}

		private void BtnStartupVerify_OnClick(object sender, RoutedEventArgs e)
		{
			wm.StartupVerify(out resultWbkSc, out resultWbkDs, out resultShtSc, out resultShtDs);

			updateStatProps();

		}


		private void BtnDeleteWbkDs_OnClick(object sender, RoutedEventArgs e)
		{
			wm.DeleteWbkDs();
		}
		private void BtnDeleteWbkSc_OnClick(object sender, RoutedEventArgs e)
		{
			wm.DeleteWbkSc();
		}

		private void BtnDeleteShtDs_OnClick(object sender, RoutedEventArgs e)
		{
			wm.DeleteShtDs();
		}

		private void BtnDeleteShtSc_OnClick(object sender, RoutedEventArgs e)
		{
			wm.DeleteShtSc();
		}

		private void BtnFindAndShowAll_OnClick(object sender, RoutedEventArgs e)
		{
			wm.FindAndShowElements();
		}

		private void BtnShowObjId_OnClick(object sender, RoutedEventArgs e)
		{
			wm.ShowObjectId(objectId, cmdId);
		}
	}
}
