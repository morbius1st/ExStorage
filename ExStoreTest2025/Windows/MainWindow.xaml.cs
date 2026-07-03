using System;
using ExStoreTest2025.Support;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShExStorage;

using static ExStoreTest2025.WindowsApiCalls;

// projname: ExStoreTest2025
// itemname: MainWindow
// username: jeffs
// created:  11/27/2022 4:05:08 PM

namespace ExStoreTest2025.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{

	#region private fields

		private string message;

		private MainWindowModel mwm;

		private ExId exid;

	#endregion

	#region ctor

		public MainWindow()
		{
			InitializeComponent();

			_proc = HookCallback;

			this.Loaded += (s, e) => _hookID = SetHook(_proc);
			this.Unloaded += (s, e) => UnhookWindowsHookEx(_hookID);


			Msgs.Mw = this;

			exid = ExId.GetInstance(Command.RvtDoc);

			mwm = new MainWindowModel(exid);

			TitleBarText = "ExStoreTest2025";

			BtnTitleBarTest2_OnClick(null, null);

		}

		private IntPtr SetHook(LowLevelKeyboardProc proc)
		{
			using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
			using (var curModule = curProcess.MainModule)
			{
				return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
			}
		}

		private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			// Only suppress F1 if YOUR WPF Window is actively in focus
			if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN && this.IsActive)
			{
				int vkCode = Marshal.ReadInt32(lParam);
				if (vkCode == VK_F1)
				{
					showHelpAlt();
					// Return a non-zero value without calling CallNextHookEx.
					// This swallows the F1 key before Revit's window hook can react.
					return new IntPtr(1);
				}
			}

			return CallNextHookEx(_hookID, nCode, wParam, lParam);
		}



	#endregion

	#region public properties

		public string Message
		{
			get => message;
			set
			{
				message = value;
				OnPropertyChange();
			}
		}

	#endregion

	#region private properties

	#endregion

	#region public methods

	#endregion

	#region private methods

	#endregion

	#region event consuming

	#endregion

	#region event publishing

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChange([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#endregion

	#region system overrides

		public override string ToString()
		{
			return "this is MainWindow";
		}

	#endregion

		private void Btn_Exit_OnClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Btn_Clear_OnClick(object sender, RoutedEventArgs e)
		{
			Message = "";
		}

		private void Btn_FindSchema_OnClick(object sender, RoutedEventArgs e)
		{
			mwm.FindSchema();
		}

		private void Btn_EraseSchema_OnClick(object sender, RoutedEventArgs e)
		{
			mwm.EraseSchema();
		}

		private void Btn_FindDs_OnClick(object sender, RoutedEventArgs e)
		{
			mwm.FindSheetDs();
		}

		private void Btn_Erase_OnClick(object sender, RoutedEventArgs e)
		{
			mwm.EraseSheetDs();
		}

		private void Btn_Write_OnClick(object sender, RoutedEventArgs e)
		{
			mwm.WriteSheet();
		}

		private void Btn_Read_OnClick(object sender, RoutedEventArgs e)
		{
			mwm.ReadSheet();
		}

		private void Btn_Delete_OnClick(object sender, RoutedEventArgs e)
		{
			mwm.DeleteSheet();
		}

		private void Btn_Values_OnClick(object sender, RoutedEventArgs e)
		{
			mwm.ReadSheet();
		}

		private void Btn_ChgValues_OnClick(object sender, RoutedEventArgs e)
		{
			mwm.Test2_1();
		}

		private void Btn_Test1_OnClick(object sender, RoutedEventArgs e)
		{
			mwm.Test1();
		}

		private void Btn_Test3_OnClick(object sender, RoutedEventArgs e) { }

		private bool gotMouse = false;
		private string titleBarText;
		private SolidColorBrush titleBarBackground;

		private void MainWindow_OnMouseEnter(object sender, MouseEventArgs e)
		{
			// Debug.WriteLine("mw mouse enter");

			gotMouse = true;
		}

		private void MainWindow_OnMouseLeave(object sender, MouseEventArgs e)
		{
			// Debug.WriteLine("mw mouse leave");

			gotMouse = false;

			activateRevit();
		}

		private void activateRevit()
		{
			Owner.Focus();
			Owner.Activate();
		}

		private void mainWin_Activated(object sender, System.EventArgs e)
		{
			if (!gotMouse) activateRevit();
		}

		private void MiniMain_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				// Debug.WriteLine("doing drag move");
				this.DragMove();
			}
		}

		private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			// Debug.WriteLine("mm - mouse left up");
			// DoingMove = false;

			// Debug.WriteLine("mm C revit has been activated");
			Owner.Activate();

			activateRevit();
		}

		private bool helpSwitch = true;

		private void showHelp()
		{
			if (helpSwitch)
			{
				ContextualHelp CtxHelp = new ContextualHelp(ContextualHelpType.Url, AppRibbon.AddinMainWinHelpFile);

				CtxHelp.Launch();

				helpSwitch = false;
			}
			else
			{
				showHelpAlt();

				helpSwitch = true;
			}
		}

		private void showHelpAlt()
		{	
			

			Process process = new Process();

			try
			{
				process.StartInfo.FileName = "explorer";
				process.StartInfo.Arguments = AppRibbon.AddinMainWinHelpFile;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.UseShellExecute = false;
				process.Start();

			}
			catch (Exception e)
			{
				Debug.Print(e.Message);
			}
		}

		private void mainWin_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.F1)
			{
				e.Handled = true;
				showHelp();
			}

			e.Handled = true;
		}

		private void BtnClose_OnClick(object sender, RoutedEventArgs e)
		{
			msgTd("Success", "got close", null);
		}

		private void msgTd(string title, string msg, string msg2)
		{
			TaskDialog td = new TaskDialog(title);
			td.MainInstruction = msg;
			td.MainContent = msg2;
			td.MainIcon = TaskDialogIcon.TaskDialogIconInformation;

			td.Show();
		}

		public string TitleBarText
		{
			get => titleBarText;
			set
			{
				titleBarText = value;

				OnPropertyChange();
			}
		}

		public SolidColorBrush TitleBarBackground
		{
			get => titleBarBackground;
			set
			{
				titleBarBackground = value;

				OnPropertyChange();
			}
		}

		public SolidColorBrush TitleBarForeground
		{
			get => titleBarForeground;
			set
			{
				titleBarForeground = value;

				OnPropertyChange();
			}
		}

		public SolidColorBrush CloseButtonFill
		{
			get => closeButtonFill;
			set
			{
				closeButtonFill = value;
				OnPropertyChange();
			}
		}


		private bool titleTest2 = true;

		private void BtnTitleBarTest2_OnClick(object sender, RoutedEventArgs e)
		{
			if (titleTest2)
			{
				TitleBarBackground = Brushes.Transparent;
				TitleBarForeground = Brushes.White;
				CloseButtonFill = Brushes.Lime;
			}
			else
			{
				TitleBarBackground = Brushes.Lime;
				TitleBarForeground = Brushes.Black;
				CloseButtonFill = Brushes.Black;
			}

			titleTest2 = !titleTest2;
		}

		private string finalCode;

		private SolidColorBrush titleBarForeground;
		private SolidColorBrush closeButtonFill;

		private int i = 0;
		private int j = 0;
		private int k = 0;

		private void BtnTitleBarTest1_OnClick(object sender, RoutedEventArgs e)
		{
			setTitleBarForground(i);
			finalCode += ".";

			Msgs.Write(" | ");

			setTitleBarBackground(j);

			Msgs.Write(" | ");

			finalCode += ".";
			setCloseBtnFill(k);

			Msgs.WriteLine($"| code = {finalCode} |");

			finalCode = "";

			if (++j == bgIdx)
			{
				j = 0;
				k++;

			
				if (k == closeIdx)
				{
					k = 0;
					i++;


					if (i == fgIdx)
					{
						i = 0;
					}
				}

			}
		}

		private int bgIdx = 6;

		private void setTitleBarBackground(int idx)
		{
			Msgs.Write("BG = ");

			if (idx % 6 == 0)
			{
				finalCode += "0";
				Msgs.Write("transparent".PadRight(15));

				TitleBarText = "ExStorTest2025";
				TitleBarBackground = Brushes.Transparent;
			}
			else if (idx % 6 == 1)
			{
				finalCode += "1";
				Msgs.Write("lawn green".PadRight(15));

				TitleBarText = "Please select the first point";
				TitleBarBackground = Brushes.LawnGreen;
			}
			else if (idx % 6 == 2)
			{
				finalCode += "2";
				Msgs.Write("cyan".PadRight(15));

				TitleBarText = "Please select the Second point";
				TitleBarBackground = Brushes.Cyan;
			}

			else if (idx % 6 == 3)
			{
				finalCode += "3";
				Msgs.Write("purple".PadRight(15));

				TitleBarText = "Please select the first point";
				TitleBarBackground = Brushes.Purple;
			}
			else if (idx % 6 == 4)
			{
				finalCode += "4";
				Msgs.Write("yellow".PadRight(15));

				TitleBarText = "Please select the Second point";
				TitleBarBackground = Brushes.Yellow;
			}

			else if (idx % 6 == 5)
			{
				finalCode += "5";
				Msgs.Write("yellow green".PadRight(15));

				TitleBarText = "Please select the first point";
				TitleBarBackground = Brushes.YellowGreen;
			}
		}

		private int fgIdx = 2;

		private void setTitleBarForground(int idx)
		{
			Msgs.Write("FG = ");

			if (idx % 2 == 0)
			{
				finalCode += "0";
				Msgs.Write("white".PadRight(8));

				TitleBarForeground = Brushes.White;
			}
			else if (idx % 2 == 1)
			{
				finalCode += "1";
				Msgs.Write("gray".PadRight(8));

				TitleBarForeground = Brushes.Gray;
			}
		}

		private int closeIdx = 3;

		private void setCloseBtnFill(int idx)
		{
			Msgs.Write("CL = ");

			if (idx % 3 == 0)
			{
				finalCode += "0";
				Msgs.Write("lawn green".PadRight(12));
				CloseButtonFill = Brushes.LawnGreen;
			}
			else
			if (idx % 3 == 1)
			{
				finalCode += "1";
				Msgs.Write("dodger blue".PadRight(12));
				CloseButtonFill = Brushes.DodgerBlue;
			}
			else
			if (idx % 3 == 2)
			{
				finalCode += "2";
				Msgs.Write("white".PadRight(12));

				CloseButtonFill = Brushes.White;
			}

		}
		private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.F1)
			{
				e.Handled = true;
			}

			e.Handled = true;
		}
		private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			showHelpAlt();
		}
	}
}