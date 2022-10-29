#region using

using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using RevitSupport;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ScSupport;
using ShExStorageN.ShExStorage;
using ShExStorageN.ShSchemaFields;
using ShStudy.ShEval;
using ShStudy;

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

		private ExId exid;

		// private ScFields sf;
		// private ScData sd;

		private IScDataWorkBook<
			SchemaTableKey, 
			ScDataTable,
			ShScFieldDefData<SchemaTableKey>
			> workBook;
		
		private ScDataTable tbld;
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
		// public ScMetaTable ScTableFields => sf.ScFieldsTable;
		// public ScMetaRow ScRowFields => sf.ScFieldsRow;


		// public ScData ScData => sd;

		// public ScTableData ScTableData => sd.ScTableData;

		public ScDataTable TblD => tbld;
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

			tbld = new ScDataTable();

			exid = new ExId(RvtCommand.RvtDoc.Title);
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


		private void BtnExp00_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.TestDynaValue();
		}


		private void BtnExp01_OnClick(object sender, RoutedEventArgs e)
		{

			// mwModel.TestDynamicValue();

			mwModel.TestFieldsLock();
			
			mwModel.TestFieldsRow();
			
			mwModel.TestFieldsTable();

		}

		private void BtnExp02_OnClick(object sender, RoutedEventArgs e)
		{
			tbld = mwModel.TestDataTable(exid);
			rowd = mwModel.TestDataRow(exid, tbld);
			lokd = mwModel.TestDataLock(exid, tbld);
		}

		private void BtnExp03_OnClick(object sender, RoutedEventArgs e)
		{
			mwModel.ShowExid(exid);
		}

		// test begin
		private void BtnExp04_OnClick(object sender, RoutedEventArgs e)
		{
			// rowd = mwModel.TestMakeDataRow(exid, tbld);
			//
			// mwModel.TestBegin(exid, tbld);

			mwModel.TestRowDataAndCollectionViews(exid, tbld);

		}
	}
}