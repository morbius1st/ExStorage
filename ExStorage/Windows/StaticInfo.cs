#region + Using Directives

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

// user name: jeffs
// created:   12/2/2022 9:48:51 PM

namespace ExStorage.Windows
{
	public static class StaticInfo
	{
		public static MainWindow MainWin { get; set; }
		public static MainWindowModel MainWinModel { get; set; }

		public static void UpdateMainWinProperty(string propertyName)
		{
			MainWin.UpdateProperty(propertyName);
		}


		public static string ToString()
		{
			return $"this is {nameof(StaticInfo)}";
		}
	}
}