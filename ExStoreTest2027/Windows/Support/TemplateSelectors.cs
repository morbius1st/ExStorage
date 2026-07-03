
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ExStorSys;


// user name: jeffs
// created:   1/18/2026 10:28:10 PM

namespace ExStorSys
{
#region select based on FieldData<WorkBookFieldKeys>

	public class WbkDynaValueTemplateSelector : DataTemplateSelector
	{
		public required DataTemplate StringTemplate { get; set; }
		public required DataTemplate EnumTemplate { get; set; }

		public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
		{
			if (item == null) return null;

			if (item.GetType() != typeof(FieldData<WorkBookFieldKeys>)) return null;

			if (((FieldData<WorkBookFieldKeys>) item).Field!.FieldKey == WorkBookFieldKeys.PK_AD_STATUS)
			{
				return EnumTemplate;
			}

			return StringTemplate;
		}
	}

#endregion

#region select based on int

	public class ShtFamListTempSelect : DataTemplateSelector
	{
		public required DataTemplate TempWhenZero { get; set; }
		public required DataTemplate TempWhenNotZero { get; set; }

		public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
		{
			if (item == null) return null;
			
			if (item.GetType() != typeof(int)) item = 1;
			
			int count = (int) item;
			
			if (count == 0) return TempWhenZero;

			return TempWhenNotZero;
			// return TempWhenZero;
		}
	}

#endregion

#region select based on bool

	public class SelectTemplatePerBool : DataTemplateSelector
	{
		public required DataTemplate TempWhenFalse { get; set; }
		public required DataTemplate TempWhenTrue { get; set; }

		public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
		{
			bool choice;

			if (item == null || item.GetType() != typeof(bool))
			{
				choice = false;
			}
			else
			{
				choice = (bool) item;
			}
			
			if (choice) return TempWhenTrue;

			return TempWhenFalse;
		}
	}

#endregion

}
