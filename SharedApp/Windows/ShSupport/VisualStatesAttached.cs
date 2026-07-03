#region + Using Directives

using System.Windows;
using System.Windows.Media;

#endregion

// user name: jeffs
// created:   12/30/2020 11:11:08 PM

namespace Windows.Support
{
	public class VisualStatesAttached : DependencyObject
	{
	#region Generic Brush A

		public static readonly DependencyProperty GenericBrushAProperty = DependencyProperty.RegisterAttached(
			"GenericBrushA", typeof(SolidColorBrush), typeof(VisualStatesAttached), new PropertyMetadata(Brushes.White));

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
			"GenericBrushB", typeof(SolidColorBrush), typeof(VisualStatesAttached), new PropertyMetadata(Brushes.White));

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
			"GenericBrushC", typeof(SolidColorBrush), typeof(VisualStatesAttached), new PropertyMetadata(Brushes.White));

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