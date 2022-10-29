#region using

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Autodesk.Revit.Creation;
using ShExStorageR.ShExStorage;
using SharedApp.Windows.ShSupport;
using Autodesk.Revit.DB;
using ShExStorageN.ShExStorage;
using ShStudy.ShEval;
// using EnvDTE;
using RevitSupport;
using ShExStorageC.ShSchemaFields;
using ShStudy;

#endregion

// projname: ExStorage
// itemname: MainWindow
// username: jeffs
// created:  12/27/2021 7:15:47 PM

namespace ExStorage.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged, IWin
	{
	#region private fields

		public static ShDebugMessages M { get; private set; } 
		private string messageBoxText;

		private MainWindowModel mwModel;

		// fields
		private ScDataTable tbld;
		private ScDataRow rowd;
		private ScDataLock lokd;


		#endregion

		#region ctor

		public MainWindow()
		{
			InitializeComponent();

			M = new ShDebugMessages(this);

			init();

		}

	#endregion

	#region public properties

		public string MessageBoxText
		{
			get => messageBoxText;
			set
			{
				messageBoxText = value; 
				OnPropertyChanged();
			}
		}

		public ExId Exid { get; private set; }

		// public ExStoreRtnCode LastResult => result;
		// public bool LastResultSuccess => result == ExStoreRtnCode.XRC_GOOD;

	#endregion

	#region private properties

	#endregion

	#region public methods

		public void ShowMsg()
		{
			OnPropertyChanged(nameof(MessageBoxText));
		}

	#endregion

	#region private methods

		private void init()
		{
			mwModel = new MainWindowModel(this, M);

			Exid = new ExId(RvtCommand.RvtDoc.Title);

			// MessageBoxText = "Hello ExStorage\n";
			//
			// sp01 = new ShowsProcedures01(this);
			//
			// exsLib = new ExStorageLib(Command.RvtDoc);
		}

		#endregion

		#region event consuming

		private void BtnExit_OnClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}

	#endregion

	#region event publishing

		public new event PropertyChangedEventHandler PropertyChanged;

		protected void  OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(MainWindow)}";
		}

	#endregion


	#region shows

		private void BtnShowExid_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.ShowExid(Exid);
		}

		private void BtnShowFn11_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.ShowProcFn11();
		}

		private void BtnShowFn12_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.ShowProcFn12();
		}

	#endregion


	#region tests

		private void BtnTestFindAllDs1_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.GetAllDs1();
		}

		private void BtnTestFindDsByName3_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.FindDsByName3();
		}

		private void BtnTestDeleteDsByName4_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.DeleteDsByName4();
		}

		private void BtnGetSchemaByName2_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.GetSchemaByName2();
		}

		private void BtnTestGetDsEntity5_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.GetDsEntity5();
		}

		private void BtnTestGetEntityData6_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.GetEntityData6();
		}

		private void BtnTestDoesDsExist7_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.DoesDsExist7();
		}

		// process tests

		// begin phase 1
		private void BtnBegin8_OnClick(object sender, RoutedEventArgs e)
		{
			ExStoreRtnCode result = mwModel.TestBeginPhase1(Exid);

			if (result == ExStoreRtnCode.XRC_DS_EXISTS)
			{
				M.WriteLineAligned("DS already exists - cannot make new - exiting\n");
				return;
			}
			else
			{
				M.WriteLineAligned("DS not found - proceeding\n");
			}

			tbld = mwModel.MakeTableData(Exid);

			mwModel.ShowTableData(tbld);

		}

	#endregion
	}
}