#region using
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

#endregion

// projname: App2
// itemname: MainWindow
// username: jeffs
// created:  1/2/2022 6:18:58 PM

namespace App2.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		#region private fields

		#endregion

		#region ctor

		public MainWindow()
		{
			InitializeComponent();
		}

		#endregion

		#region public properties

		#endregion

		#region private properties

		#endregion

		#region public methods

		#endregion

		#region private methods

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

	}
}
