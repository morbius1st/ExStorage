using ExStoreTest2025.Support;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ShExStorageC.ShSchemaFields;
using ShExStorageC.ShSchemaFields.ShScSupport;
using ShExStorageN.ShExStorage;

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

			Msgs.Mw = this;

			exid = ExId.GetInstance(Command.RvtDoc);

			mwm = new MainWindowModel(exid);
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

		private void Btn_Test3_OnClick(object sender, RoutedEventArgs e)
		{
			
		}

	}
}