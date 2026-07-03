
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


// user name: jeffs
// created:   6/27/2026 6:10:10 AM

namespace Windows.Support
{
	public class VisualStatesAP : DependencyObject
	{
	#region Generic Brush A

		public static readonly DependencyProperty GenericBrushAProperty = DependencyProperty.RegisterAttached(
			"GenericBrushA", typeof(SolidColorBrush), typeof(VisualStatesAP),
			new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetGenericBrushA(UIElement element, SolidColorBrush value)
		{
			element.SetValue(GenericBrushAProperty, value);
		}

		public static SolidColorBrush GetGenericBrushA(UIElement element)
		{
			return (SolidColorBrush) element.GetValue(GenericBrushAProperty);
		}

	#endregion

	#region Generic Brush B

		public static readonly DependencyProperty GenericBrushBProperty = DependencyProperty.RegisterAttached(
			"GenericBrushB", typeof(SolidColorBrush), typeof(VisualStatesAP),
			new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetGenericBrushB(UIElement element, SolidColorBrush value)
		{
			element.SetValue(GenericBrushBProperty, value);
		}

		public static SolidColorBrush GetGenericBrushB(UIElement element)
		{
			return (SolidColorBrush) element.GetValue(GenericBrushBProperty);
		}

	#endregion

	#region Generic Brush C

		public static readonly DependencyProperty GenericBrushCProperty = DependencyProperty.RegisterAttached(
			"GenericBrushC", typeof(SolidColorBrush), typeof(VisualStatesAP),
			new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetGenericBrushC(UIElement element, SolidColorBrush value)
		{
			element.SetValue(GenericBrushCProperty, value);
		}

		public static SolidColorBrush GetGenericBrushC(UIElement element)
		{
			return (SolidColorBrush) element.GetValue(GenericBrushCProperty);
		}

	#endregion

	}
}
