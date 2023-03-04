#region + Using Directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ShExStorageN.ShSchemaFields;

#endregion

// user name: jeffs
// created:   12/4/2022 7:28:42 PM

namespace ShExStorageC.ShSchemaFields.ShScSupport
{

	// public class KeyToFieldValueConverter : IValueConverter
	// {
	// 	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	// 	{
	// 		if (!(value is Dictionary<SchemaSheetKey, ScFieldDefData<SchemaSheetKey>>)) { return null; }
	// 		if (!(parameter is SchemaSheetKey)) { return null; }
	//
	// 		Dictionary<SchemaSheetKey, ScFieldDefData<SchemaSheetKey>> val = (Dictionary<SchemaSheetKey, ScFieldDefData<SchemaSheetKey>>) value;
	// 		SchemaSheetKey key = (SchemaSheetKey) parameter;
	//
	// 		return val[key];
	// 	}
	//
	// 	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	// 	{
	// 		return null;
	// 	}
	// }
/*
	public class PassThruConverter : IValueConverter
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


	public class KeyToFieldValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Dictionary<SchemaSheetKey, ScFieldDefData1<SchemaSheetKey>>)) { return null; }
			if (!(parameter is SchemaSheetKey)) { return null; }

			Dictionary<SchemaSheetKey, ScFieldDefData1<SchemaSheetKey>> val = (Dictionary<SchemaSheetKey, ScFieldDefData1<SchemaSheetKey>>) value;
			SchemaSheetKey key = (SchemaSheetKey) parameter;

			return val[key].Value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

	public class KeyToFieldNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Dictionary<SchemaSheetKey, ScFieldDefData1<SchemaSheetKey>>)) { return null; }
			if (!(parameter is SchemaSheetKey)) { return null; }

			Dictionary<SchemaSheetKey, ScFieldDefData1<SchemaSheetKey>> val = (Dictionary<SchemaSheetKey, ScFieldDefData1<SchemaSheetKey>>) value;
			SchemaSheetKey key = (SchemaSheetKey) parameter;

			return val[key].Meta1Field.FieldName;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
*/

}
