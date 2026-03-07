using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ExStorSys;



// user name: jeffs
// created:   12/28/2025 8:24:39 PM

namespace ExStoreTest2026.Windows
{

/* debug converters */

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

#region TestConverter

	// for debugging only
	// allows breakpoint here
	[ValueConversion(typeof(Enum), typeof(SolidColorBrush))]
	public class TestConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			SolidColorBrush answer = Brushes.Red;

			Type t1 = value?.GetType() ?? typeof(double);
			Type t2 = parameter?.GetType() ?? typeof(double);

			return answer;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Brushes.Red;
			;
		}
	}

#endregion

/* string converters */

#region pass-through converter

	// verify if the input string is "void"
	// that is, is either null of empty
	[ValueConversion(typeof(string), typeof(bool))]
	public class StringVoidToBool : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is string)) return false;

			return String.IsNullOrWhiteSpace((string)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

#endregion


/* visibility converters */

#region TypeMatch2Visibility

	// for debugging only
	// allows breakpoint here
	[ValueConversion(typeof(Type), typeof(Visibility))]
	public class TypeMatch2Visibility : IValueConverter
	{
		// value = the field type
		// parameter = the reference Type
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value == null || value is not Type) return null;
			if (parameter == null || parameter is not Type) return null;

			Type T1 = (Type) value;
			Type T2 = (Type) parameter;

			bool b8 = T1.IsAssignableTo(T2);

			return b8 ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}

#endregion


/* value translater */

// convert dynavalue (dynamic) to string for display or as an enum
// need dyanvalue, workbook or sheet field def

#region WorkbookValueTranslator

	// for debugging only
	// allows breakpoint here
	[ValueConversion(typeof(object), typeof(string))]
	public class WorkbookValueTranslator : IValueConverter
	{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value == null) return null;
			if (parameter == null || parameter is not FieldDef<WorkBookFieldKeys>) return null;

			FieldDef<WorkBookFieldKeys> fd = (FieldDef<WorkBookFieldKeys>) parameter;

			if (fd.FieldType == typeof(ActivateStatus)) return (ActivateStatus) value;

			return value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}

#endregion


/* enum converters */

#region UserSecurityEnumToStrConverter

	[ValueConversion(typeof(Enum), typeof(string))]
	public class UserSecurityEnumToStrConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value == null) return "is null";
			if (parameter == null) return "is null";

			Tuple<string, string, SolidColorBrush> info =
				ExStorConst.UsserSecurityLevelDesc[(UserSecutityLevel) value];

			if (parameter.Equals("1")) return info.Item1;

			return info.Item2;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

#endregion

#region FieldAccessToVisibilityConverter

	/// <summary>
	/// converts an enum + user security level to bool<br/>
	/// bool represents whether the field can be edited or not
	/// </summary>
	[ValueConversion(typeof(Enum), typeof(bool))]
	public class FieldAccessToVisibilityConverter : IMultiValueConverter
	{
		// enum values provided:
		// [0] = user security level
		// [1] = field edit level
		// return field edit status
		public object Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture)
		{
			// Debug.WriteLine($"*** at convert to vis | {parameter} | len {values?.Length ?? -1}");

			if (values == null || parameter == null) return false;

			if (values.Length == 0 ||
				!(values[0] is Enum) ||
				!(values[1] is Enum) ) return false;


			FieldEditStatus fes =
				SecurityMgr.ValidateFieldEditing((FieldEditLevel) values[1], (UserSecutityLevel) values[0]);

			bool b = false;
			if (parameter.Equals(1)) 
				b = fes == FieldEditStatus.FES_CAN_EDIT || fes == FieldEditStatus.FES_CAN_VIEW;
			else 
				b = fes == FieldEditStatus.FES_CAN_EDIT;

			return b;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

#endregion

#region FieldAccessToEditibilityConverter

	[ValueConversion(typeof(Enum), typeof(bool))]
	public class FieldAccessToEditibilityConverter : IMultiValueConverter
	{
		// enum values provided:
		// [0] = user security level
		// [1] = field edit level
		// return field edit status
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length == 0 ||
				!(values[0] is Enum) ||
				!(values[1] is Enum) ) return false;

			FieldEditStatus fes =
				SecurityMgr.ValidateFieldEditing((FieldEditLevel) values[1], (UserSecutityLevel) values[0]);

			return fes == FieldEditStatus.FES_CAN_EDIT;
			// return fes != FieldEditStatus.FES_NONE ? Visibility.Visible : Visibility.Collapsed;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

#endregion

#region EnumToColorConverter

	/// <summary>
	/// convert an enum into a color brush based on the data passed<br/>
	/// value = enum value to convert<br/>
	/// parameter = (is not used)
	/// </summary>
	[ValueConversion(typeof(Enum), typeof(SolidColorBrush))]
	public class EnumToColorConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			SolidColorBrush answer = Brushes.Red;

			if (!(value is Enum)) return answer;
			if (parameter != null && !(parameter is Enum)) return answer;

			int result = 0;

			if (value == null) return answer;

			// if (parameter != null && (FieldEditLevel )parameter != FieldEditLevel.FEL_LOCKED)
			// {
			// 	if ((FieldEditLevel) value <= (FieldEditLevel) parameter)
			// 	{
			// 		result = (FieldEditLevel) parameter == FieldEditLevel.FEL_DEBUG ? 3 :
			// 			(FieldEditLevel) parameter == FieldEditLevel.FEL_ADVANCED ? 2 :
			// 				(FieldEditLevel) parameter == FieldEditLevel.FEL_BASIC ? 1 : 0;
			// 	}
			// 	else
			// 	{
			// 		result = 0;
			// 	}
			//
			// }
			// else
			if (value is FieldEditLevel)
			{
				answer = ExStorConst.FieldEditLevelDesc[(FieldEditLevel) value].Item3;
			}
			else if (value is UserSecutityLevel)
			{
				answer = ExStorConst.UsserSecurityLevelDesc[(UserSecutityLevel) value].Item3;
			}
			else if (value is ActivateStatus)
			{
				answer = ExStorConst.ActiveStatusDesc[(ActivateStatus) value].Item3;
			}

			// if (result == 0) answer = Brushes.SlateGray;
			// else if (result == 1) answer = Brushes.Yellow;
			// else if (result == 2) answer = Brushes.DeepSkyBlue;
			// else if (result == 3) answer = Brushes.Chartreuse;


			return answer;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

#endregion

#region FieldEditLevelEnumToStrConverter

	[ValueConversion(typeof(Enum), typeof(string))]
	public class FieldEditLevelEnumToStrConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value == null) return "bad value";
			if (parameter == null) return "bad parameter";

			string vStr;
			FieldEditLevel vStat;
			string answer;

			try
			{
				vStr = Enum.GetName(value.GetType(), value)!;
				vStat = Enum.Parse<FieldEditLevel>(vStr);
				answer = parameter.Equals("1") ? ExStorConst.FieldEditLevelDesc[vStat].Item1 : ExStorConst.FieldEditLevelDesc[vStat].Item2;
			}
			catch
			{
				return "not converted";
			}

			return answer;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

#endregion

#region FieldStatusEnumToStrConverter

	[ValueConversion(typeof(Enum), typeof(string))]
	public class FieldStatusEnumToStrConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value == null) return "bad value";
			if (parameter == null) return "bad parameter";

			string vStr;
			FieldStatus vStat;
			string answer;

			try
			{
				vStr = Enum.GetName(value.GetType(), value)!;
				vStat = Enum.Parse<FieldStatus>(vStr);
				answer = parameter.Equals("1") ? ExStorConst.FieldStatusDesc[vStat].Item1 : ExStorConst.FieldStatusDesc[vStat].Item2;
			}
			catch
			{
				return "not converted";
			}

			return answer;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

#endregion

#region FieldStatusEnumToStrConverter

	[ValueConversion(typeof(Enum), typeof(bool))]
	public class FieldStatusEnumToBoolConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value == null || value is not FieldStatus) return true;

			FieldStatus vStat = (FieldStatus) value;

			return vStat == FieldStatus.FS_CLEAN;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

#endregion

#region FieldAccessToBool

	[ValueConversion(typeof(Enum), typeof(bool))]
	public class FieldAccessToBool : IMultiValueConverter
	{
		// enum values provided:
		// [0] = user security level eg. 100
		// [1] = field access level eg. 1000
		// result field access leve >= user secutity level eg. 1000 > 100 - ok to go
		// except if val0 = SL_NONE - return false
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length == 0 ||
				!(values[0] is Enum) ||
				!(values[1] is Enum) ) return false;

			int usr0 = (int) values[0];

			if (usr0 == (int) ExStorSys.UserSecutityLevel.SL_NONE) return false;

			int fld1 = (int) values[1];

			return fld1 >= usr0;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

#endregion


/* get field converters */

// converter to get field value from data object
// get values from sheet data object
// provide:
// value = field key enum value 
// parameter = sheet data object

#region UserSecurityEnumToStrConverter

	[ValueConversion(typeof(Enum), typeof(string))]
	public class SheetFieldEnumToStringConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (parameter == null || !(parameter is Enum)) return "bad value";
			if (value == null || !(value is Sheet)) return "bad parameter";

			string result = "not found";
			SheetFieldKeys key = (SheetFieldKeys) parameter;
			Sheet sht = (Sheet) value;

			if (key == SheetFieldKeys.RK_MGMT_FAM_COUNT)
			{
				result = sht.GetValue(SheetFieldKeys.RK_RD_FAMILY_LIST)?.AsDictStringString().Count.ToString() ?? "none";
			}
			else if (key == SheetFieldKeys.RK_OD_STATUS)
			{
				result = ExStorConst.SheetOpStatusDesc[(SheetOpStatus) (sht.GetValue(key)?.AsEnum() ?? SheetOpStatus.SS_NA)].Item2;
			}
			else result = sht.GetValue(key)?.AsString() ?? "null";

			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

#endregion

/* math converters */

#region AddConverter

	[ValueConversion(typeof(double), typeof(double))]
	public class AddConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			// if (!(value is double) ||
			// 	!(parameter is double) ) return 0.0 ;

			double v = (double) value;
			double p ;

			// if (value == null || !Double.TryParse((string) value, out v)) return 0.0;
			if (parameter == null || !Double.TryParse((string) parameter, out p)) return 0.0;
			//
			// double sum = v + p;

			return v + p;
			// return (object) (v + p);
			// return (double)(((double) value) + ((double) parameter));
		}

		public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
		{
			return 0.0;
		}
	}

#endregion

#region double multi add converter

	[ValueConversion(typeof(double), typeof(double))]
	public class MultiAddConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double sum = 0.0;

			foreach (Double value in values)
			{
				sum += value;
			}

			return sum;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new object[] { 0.0 };
		}
	}

#endregion

#region double divider converter

	[ValueConversion(typeof(double), typeof(double))]
	public class DivideConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double valu = (double) (value ?? 0.0);

			bool result = double.TryParse((string) (parameter ?? "1.0"), out double divisor);

			if (!result || divisor == 0) return valu;

			return valu / divisor;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

#endregion

/* saved converters */

// #region FieldAccessToBool
//
// 	[ValueConversion(typeof(Enum), typeof(bool))]
// 	public class GenericEnumToString<T> : IMultiValueConverter
// 	where T: Enum
// 	{
//
// 		// enum values provided:
// 		// [0] = user security level eg. 100
// 		// [1] = field access level eg. 1000
// 		// result field access leve >= user secutity level eg. 1000 > 100 - ok to go
// 		// except if val0 = SL_NONE - return false
// 		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			if (values.Length == 0 ||
// 				!(values[0] is Enum) ||
// 				!(values[1] is string) ) return "bad value";
//
// 			if (!(parameter is Dictionary<T, Tuple<string, string>>)) return "bad parameter";
//
// 			T v = (T) values[0];
//
// 			Dictionary<T, Tuple<string, string>> strValues = (Dictionary<T, Tuple<string, string>>) parameter;
//
// 			string answer = strValues[v].Item2;
//
// 			return answer;
// 		}
//
// 		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
// 		{
// 			return null;
// 		}
// 	}
//
// #endregion
}