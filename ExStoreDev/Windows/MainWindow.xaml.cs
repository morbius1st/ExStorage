#region using

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using RevitSupport;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShExStorage;
using ShStudyN.ShEval;
using ShStudyN;
using System.Windows.Documents;
using ShExStorageN.ShSchemaFields;

#endregion

// projname: ExStoreDev
// itemname: MainWindow
// username: jeffs
// created:  10/3/2022 11:02:57 PM

namespace ExStoreDev.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged, IWin
	{
	#region private fields

		private ShtExId shtExid;
		private LokExId lokExid;

		// private ScFields sf;
		// private ScData sd;


		private ScDataSheet shtd;
		private ScDataSheet shtdCopy;

		private ScDataRow rowd;
		private ScDataLock lokd;

		private ShFieldDisplayData shFd;

		private MainWindowModel mwModel;

		private string message;

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

		// public ScFields ScFields => sf;
		//
		// public ScMetaLock ScLockFields => sf.ScFieldsLock;
		// public ScMetaSheet ScSheetFields => sf.ScFieldsSheet;
		// public ScMetaRow ScRowFields => sf.ScFieldsRow;


		// public ScData ScData => sd;

		// public ScSheetData ScSheetData => sd.ScSheetData;

		public ScDataSheet ShtD => shtd;
		public ScDataRow RowD => rowd;
		public ScDataLock LokD => lokd;

		public static ShDebugMessages M { get; private set; }

		public string MessageBoxText
		{
			get => message;
			set
			{
				message = value;
				OnPropertyChange();
			}
		}

		public string StatusBoxText
		{
			get => message;
			set { MessageBoxText = value; }
		}

	#endregion

	#region private properties

	#endregion

	#region public methods

		public void ShowMsg()
		{
			OnPropertyChange(nameof(MessageBoxText));
		}

	#endregion

	#region private methods

		private void init()
		{
			mwModel = new MainWindowModel(this);

			shFd = new ShFieldDisplayData();

			shtd = new ScDataSheet();

			AExId.Config(RvtCommand.RvtDoc);

			shtExid = new ShtExId("Sheet ExId 01", ShtExId.ROOT);

			lokExid = new LokExId("Lock ExId 01", LokExId.PRIME);
		}

	#endregion

	#region event consuming

		private void BtnExit_OnClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

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


		// dynavalue
		private void BtnExp00_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.TestDynaValue();
		}

		// exid
		private void BtnExp03_OnClick(object sender, RoutedEventArgs e)
		{
			shtExid = null;

			M.WriteLineAligned("\nexid| before");
			mwModel.ShowExid(shtExid);

			shtExid = new ShtExId("Sheet ExId 01", ShtExId.ROOT);

			M.WriteLineAligned("\nexid| after");
			mwModel.ShowExid(shtExid);
		}

		// field information
		private void BtnExp01_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.TestFieldsLock();
			mwModel.TestFieldsRow();
			mwModel.TestFieldsSheet();
		}

		// // test sheet data
		private void BtnExp02_OnClick(object sender, RoutedEventArgs e)
		{
			shtd = mwModel.TestMakeDataSheetInitial(shtExid);

			mwModel.TestMakeDataRow3(shtExid, shtd);

			mwModel.ShowSheetData(shtd);
		}

		// test clone
		private void BtnExp02b_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.TestClone(shtd);
		}


		//
		// // test sheet data
		// private void BtnExp02b_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	shtd = mwModel.TestMakeDataSheetEmpty();
		//
		// 	mwModel.ShowSheetData(shtd);
		//
		// 	shtd = mwModel.TestMakeDataSheetInitial(shtExid);
		//
		// 	mwModel.ShowSheetData(shtd);
		//
		// }

		// test lock data
		private void BtnExp05_OnClick(object sender, RoutedEventArgs e)
		{
			// ScDataLock2 lokd2 = mwModel.TestMakeDataLock(lokExid);

			mwModel.ShowLockData(lokd);
		}


		// test begin
		// private void BtnExp04_OnClick(object sender, RoutedEventArgs e)
		// {
		// 	M.WriteLine("\n*** begin create schema ***\n");
		//
		// 	shtd = mwModel.TestMakeDataSheetInitial(shtExid);
		//
		// 	mwModel.TestMakeDataRow3(shtExid, shtd);
		//
		// 	mwModel.ShowSheetData(shtd);
		//
		// 	M.WriteLine("\ndata made\n");
		//
		// 	mwModel.CreateSheet(shtExid, shtd);
		//
		// }

		// test sheet data
		private void BtnExp06_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.MakeDataGeneric();

			// shtd = new ScDataSheet1();
			// rowd = new ScDataRow1();
			// lokd = new ScDataLock1();
			//
			// mwModel.ShowSheetData(shtd);
		}

		protected char nextLockCharL = 'A'; // A to Z
		protected char nextLockCharR = 'a'; // a to z then A to Z


		private void BtnExp07_OnClick(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < 26 * 32; i++)
			{
				M.Write($"{nextLockCharL}{nextLockCharR};  ");

				incrementCharPair();
			}
		}

		public void incrementCharPair()
		{
			if (nextLockCharR == 'z' || nextLockCharR == 'Z')
			{
				M.NewLine();
				nextLockCharR = (char) (nextLockCharR - 26);

				if (nextLockCharL == 'Z')
				{
					nextLockCharL = '@';
					nextLockCharR = '@';
					M.NewLine();
				}

				++nextLockCharL;
			}

			++nextLockCharR;
		}

		private void BtnExp08_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.KeysTests();
			// mwModel.KeysTests();
		}
	}
}