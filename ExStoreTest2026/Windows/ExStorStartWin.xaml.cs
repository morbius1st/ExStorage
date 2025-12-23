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

namespace ExStoreTest2026.Windows
{
	/// <summary>
	/// Interaction logic for ExStorStartWin.xaml
	/// </summary>
	public partial class ExStorStartWin : Window, INotifyPropertyChanged
	{
		private string message;

		public ExStorStartWin()
		{
			InitializeComponent();
		}

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





		public event PropertyChangedEventHandler PropertyChanged;

		[DebuggerStepThrough]
		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

		private void BtnExit_OnClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
