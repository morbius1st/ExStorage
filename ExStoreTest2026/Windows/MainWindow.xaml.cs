using System.ComponentModel;
using System.Diagnostics;

using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
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
		private ExStorData xData;
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
			ObjectId = ExStorStartMgr.Instance?.AddObjId(nameof(MainWindow)) ?? -1;

			// ui dependant objects must be connected before InitializeComponent elsewise
			// the ui gets connected to a null object that no longer exists after it gets configured
			mui = MainWinModelUi.Instance;
			xData = ExStorData.Instance;
			

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

		public MainWinModel Wm { get; set; } = new ();
		public MainWinModelUi Mui => mui;
		public ExStorData XData => xData;

		/* methods */

		private void onClose()
		{
			this.Hide();
		}

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

			Msgs.WriteLine($"\n*** window activated | model is {R.RvtDoc?.Title ?? "no title"} | model name | {ExStorMgr.Instance.xData.WorkBook?.ModelTitle ?? "null"}");

			Wm.ShowMsgCache();
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
			onClose();
		}

		private void BtnDebug_OnClick(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("** at debug **");

			// Wm.TestExtractCode();
			// Wm.TestExtractCode2();
			// Wm.TestDupSheet();
			// boolTestA();
			// enumMathTest();

			// mui.UpdateData();

			// string userName3 = Environment.UserDomainName;
			// string userName2 = CsUtilities.UserName;

			// string uname = SecurityMgrvoid.UserName;
			// string? uname2 = SecurityMgrvoid.UserName2;
			
			// secVsFelTest();

			// Dictionary<string, Sheet> shts = mui.ShtsList;

			// SecurityMgr.Instance.Init(tempUser[secIdx].Item1, tempUser[secIdx++].Item2);
			// mui.UpdateData();
			// if (secIdx > 3) secIdx = 0;

			// DebugRoutines.TestDynaValueChanges();

			// Mui.Wbk.Rows[WorkBookFieldKeys.PK_AD_DESC].DyValue!.ChangeValue(dv);

			


			
			string a = R.RvtDoc?.Title ?? String.Empty;
		}

		private int flip = 0;

		private void changeShtValueTest()
		{
			if (flip % 3 == 0)
			{
				dynamic dv = "this is a new description";
				Wm.CurrSht!.Desc = dv;
				flip++;
			}
			else if (flip % 3 == 1)
			{
				Wm.CurrSht!.ApplyChanges();
				flip++;
			}
			else
			{
				dynamic dv = "this is an old description";
				Wm.CurrSht!.Desc = dv;
				flip++;
			}
		}


		private void changeWbkValueTest()
		{
			
			bool test = false;

			if (flip % 3 == 0)
			{
				dynamic dv = "this is a new description";
				Wm.Wbk.Desc = dv;
				
				dv = ActivateStatus.AS_INACTIVE;
				Wm.Wbk.Status = dv;
				
				if (test)
				{
					Debug.Write($"** 0 flip is {flip}");

					if (flip % 6 == 0)
					{
						Wm.UpdateData();

						Debug.WriteLine(" | did mui.update");
					}else
					{
						Wm.Wbk.UpdateProps();

						Debug.WriteLine(" | did wbk.update");
					}
					
				}
				flip++;

			} else if (flip % 3 == 1)
			{
				Wm.Wbk.ApplyChanges();

				if (test)
				{
					Debug.Write($"** 1/3 flip is {flip}");

					if (flip % 6 == 1)
					{
						Wm.UpdateData();
						Debug.WriteLine(" | did mui.update");
					}else
					{
						Wm.Wbk.UpdateProps();

						Debug.WriteLine(" | did wbk.update");
					}
				}

				flip++;
			}
			else
			{
				dynamic dv = "this is an old description";
				Wm.Wbk.Desc = dv;
				
				dv = ActivateStatus.AS_ACTIVE;
				Wm.Wbk.Status = dv;
				
				if (test)
				{
					Debug.Write($"** 2/3 flip is {flip}");

					if (flip % 6 == 2)
					{
						Wm.UpdateData();
						Debug.WriteLine(" | did mui.update");
					}else
					{
						Wm.Wbk.UpdateProps();

						Debug.WriteLine(" | did wbk.update");
					}
					
				}
				flip++;
			}

		}

		private int secIdx = 0;

		private List<Tuple<string, string>> tempUser = new ()
		{
			{new ( "johns", "John S (Co)") },
			{new ("jimmys","Jimmy S (Co)") },
			{new ( "jacks", "Jack S (Co)") },
			{new ( "jeffs", "Jeff S (Co)") },
			
		};

		private void enumMathTest()
		{
			ValidateSchema ss = ValidateSchema.VSC_VRFY_UNTESTED;
			ValidateDataStorage sd = ValidateDataStorage.VDS_VRFY_UNTESTED;

			ValidateSchema ws = ValidateSchema.VSC_GOOD;
			ValidateDataStorage wd = ValidateDataStorage.VDS_ACT_IGNORE;

			int idx =  (int) wd * 1000 + 
				((int) (sd < ValidateDataStorage.VDS_GOOD ? ValidateDataStorage.VDS_DEFAULT : sd)) * 100 + 
				(int) ws * 10 + 
				((int) (ss < ValidateSchema.VSC_GOOD ? ValidateSchema.VSC_DEFAULT : ss));
		}

		private void boolTestA()
		{
			bool? b1;
			bool? b2;

			for (int i = 0; i < 3; i++)
			{
				b1 = i == 0 ? true : i == 1 ? null : false;

				Msgs.WriteLine($"\nfor loop - i is {i}\n");
				if ((b2=boolTest(b1)) == null) Msgs.WriteLine($"bool test | input b1 = {b1,-6} returned b2 = {b2,-6} | answer is null?");
				if ((b2=boolTest(b1)) == true) Msgs.WriteLine($"bool test | input b1 = {b1,-6} returned b2 = {b2,-6} | answer is true?");
				if ((b2=boolTest(b1)) == false) Msgs.WriteLine($"bool test | input b1 = {b1,-6} returned b2 = {b2,-6} | answer is false?\n");
			}
		}

		private void secVsFelTest()
		{
			Debug.Write($"{"fel",-15}{"fes is ->",-15}");

			// locked versus debug
			foreach (UserSecutityLevel usl in Enum.GetValues(typeof(UserSecutityLevel)))
			{
				Debug.Write($"{usl,-30}");
			}

			Debug.WriteLine("\n");

			foreach (FieldEditLevel fel in Enum.GetValues(typeof(FieldEditLevel)))
			{
				testFelVs(fel);
			}

		}

		// tests - begin

		private void BtnChgWbkValueTest_OnClick(object sender, RoutedEventArgs e)
		{
			changeWbkValueTest();
		}

		private void BtnChgShtValueTest_OnClick(object sender, RoutedEventArgs e)
		{
			changeShtValueTest();
		}

		private void BtnChgUserTest_OnClick(object sender, RoutedEventArgs e)
		{
			SecurityMgr.Instance.Init(tempUser[secIdx].Item1, tempUser[secIdx++].Item2);
			Wm.UpdateData();
			if (secIdx > 3) secIdx = 0;
		}

		// tests end
		
		private void testFelVs(FieldEditLevel fel)
		{
			FieldEditStatus fes;

			Debug.Write($"{fel,-30}");

			foreach (UserSecutityLevel usl in Enum.GetValues(typeof(UserSecutityLevel)))
			{
				fes = SecurityMgr.ValidateFieldEditing(fel, usl);

				Debug.Write($"{fes,-30}");
			}

			Debug.WriteLine("");
		}

		private bool? boolTest(bool? b)
		{
			return b;
		}

		private void BtnClear_OnClick(object sender, RoutedEventArgs e)
		{
			Message = "";
		}

		private void ShowInfoOne_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.ShowInfo1();
		}

		private void ShowInfoTwo_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.ShowInfo2();
		}

		private void ShowExMgr_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.ShowExMgr();
		}

		private void BtnMakeWkBk_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.MakeWorkBook();
		}

		private void BtnMakeEmptyWkBk_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.MakeEmptyWorkBook();
		}

		private void BtnShowWbk_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.ShowWbk();
		}

		private void BtnMakeSht_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.MakeSheet();
		}

		private void BtnMakeAndWriteWbk_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.MakeAndWriteWorkBook();
		}

		// private void BtnClearAndReadbk_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	Wm.ClearAndReadWorkBook();
		// }

		private void BtnMakeEmptySht_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.MakeEmptySheet();
		}

		private void BtnShowSht_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.ShowSheets();
		}

		private void BtnChgSht_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.ChangeSelectedSheetDesc();
		}

		private void BtnChgWbk_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.ChangeWorkBook();
		}

		private void BtnWriteSht_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.MakeAndWriteSheet();
		}

		private void BtnFindWbkDs_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.FindWorkBookDs();
		}

		private void BtnFindShtDs_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.FindSheetDs();
		}

		// private void BtnFindAllShtDs2_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	Wm.FindAllSheetDs();
		// }

		// private void BtnFindAllShtDs_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	Wm.FindAllSheetDs();
		// }

		private void BtnFindAndShowAll_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.FindAndShowElements();
		}

		private void BtnShowObjId_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.ShowObjectId(ObjectId);
		}

		// private void BtnClearReadAll_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	Wm.ClearAndReadAll();
		// }

		private void BtnReadAndShowAll_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.ReadAllFromTemp();

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
			onClose();
			e.Cancel = true;
		}

		private void BtnTstWbkFalseModelName_OnClick(object sender, RoutedEventArgs e) {}
		// private void BtnTstShtFalseModelCode_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	Wm.ShtWithFalseModelCode();
		// }

		private void BtnStartLaunchMgr_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.StartLaunchManager();
		}

		/* startup */

		private void BtnVerifyStartup_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.TestOnOpenDocLaunch();
		}

		/* delete */

		// private void BtnDeleteWbkSc_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	Wm.DeleteWbkSc();
		// }

		// private void BtnDeleteShtSc_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	Wm.DeleteShtSc();
		// }

		private void BtnDeleteOneSht_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.DeleteFirstShtDs();
		}

		private void BtnDeleteShts_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.DeleteShtDs();
		}

		private void BtnDeleteWbk_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.DeleteWbkDs();
		}

		// private void BtnDeleteWbkDs_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	Wm.DeleteWbkDs();
		// }

		// private void BtnDeleteShtDs_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	Wm.DeleteShtDs();
		// }

		private void BtnVerifyTest_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.TestVerify();
		}

		private void BtnTstWbkSetNotActive_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.SetWbkToInactive();
		}

		private void BtnUndo2_OnClick(object sender, RoutedEventArgs e)
		{
			Button btn = (Button) sender;
			// TextBox tbx = (TextBox) btn.Tag;
			FieldData<WorkBookFieldKeys> fd = (FieldData<WorkBookFieldKeys>) btn.Tag;
			
			Wm.Wbk.UndoChange(fd);

		}

		private void BtnUndoSht_OnClick(object sender, RoutedEventArgs e)
		{
			Button btn = (Button) sender;
			// TextBox tbx = (TextBox) btn.Tag;
			FieldData<SheetFieldKeys> fd = (FieldData<SheetFieldKeys>) btn.Tag;

			Wm.CurrSht.UndoChange(fd);
		}

		private void BtnUndo_OnClick(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("\t** got undo button");
		}


		private void BtnAddSht_OnClick(object sender, RoutedEventArgs e)
		{

		}
		private void BtnRemoveSht_OnClick(object sender, RoutedEventArgs e)
		{

		}

		private void BtnRemoveAllShts_OnClick(object sender, RoutedEventArgs e)
		{

		}

		private void BtnAddFam_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.AddFamilyAndType();
		}

		private void BtnRemoveFam_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.RemoveFamliyAndType();
		}

		private void BtnRemoveAllFams_OnClick(object sender, RoutedEventArgs e)
		{

		}

		private void BtnAddFamAndType_OnClick(object sender, RoutedEventArgs e)
		{
			Wm.AddFamilyAndType2();
		}
	}


}