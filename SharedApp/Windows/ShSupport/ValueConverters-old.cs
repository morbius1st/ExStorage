using System;
using System.Globalization;
using System.Windows.Data;

namespace SharedApp.Windows.Support
{
#region pass-through converter

	// for debugging only
	// allows breakpoint here
	[ValueConversion(typeof(object), typeof(object))]
	public class PassThroughConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}

#endregion
}
