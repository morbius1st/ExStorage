#region + Using Directives

using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;

#endregion

// user name: jeffs
// created:   12/30/2020 11:11:08 PM

namespace ExStoreTest2026.Windows
{
	public class CsCheckBoxAp : DependencyObject
	{

	#region double - checkbox box size

		public static readonly DependencyProperty CheckBoxBoxSizeProperty = DependencyProperty.RegisterAttached(
			"CheckBoxBoxSize", typeof(double), typeof(CsCheckBoxAp),
			new FrameworkPropertyMetadata(8.0, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCheckBoxBoxSize(UIElement e, double value)
		{
			e.SetValue(CheckBoxBoxSizeProperty, value);
		}

		public static double GetCheckBoxBoxSize(UIElement e)
		{
			return (double) e.GetValue(CheckBoxBoxSizeProperty);
		}

	#endregion

	#region double - checkbox box width

		public static readonly DependencyProperty CheckBoxBoxWidthProperty = DependencyProperty.RegisterAttached(
			"CheckBoxBoxWidth", typeof(double), typeof(CsCheckBoxAp),
			new FrameworkPropertyMetadata(8.0, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCheckBoxBoxWidth(UIElement e, double value)
		{
			e.SetValue(CheckBoxBoxWidthProperty, value);
		}

		public static double GetCheckBoxBoxWidth(UIElement e)
		{
			return (double) e.GetValue(CheckBoxBoxWidthProperty);
		}

	#endregion

	#region thickness = checkbox box margin

		public static readonly DependencyProperty
			CheckBoxBoxMarginProperty = DependencyProperty.RegisterAttached(
				"CheckBoxBoxMargin", typeof(Thickness),
				typeof(CsCheckBoxAp), new FrameworkPropertyMetadata(new Thickness(0),
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCheckBoxBoxMargin(UIElement e, Thickness value)
		{
			e.SetValue(CheckBoxBoxMarginProperty, value);
		}

		public static Thickness GetCheckBoxBoxMargin(UIElement e)
		{
			return (Thickness) e.GetValue(CheckBoxBoxMarginProperty);
		}

	#endregion

	#region thickness - checkbox check margin

		public static readonly DependencyProperty
			CheckBoxCheckMarginProperty = DependencyProperty.RegisterAttached(
				"CheckBoxCheckMargin", typeof(Thickness),
				typeof(CsCheckBoxAp), new FrameworkPropertyMetadata(new Thickness(0),
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCheckBoxCheckMargin(UIElement e, Thickness value)
		{
			e.SetValue(CheckBoxCheckMarginProperty, value);
		}

		public static Thickness GetCheckBoxCheckMargin(UIElement e)
		{
			return (Thickness) e.GetValue(CheckBoxCheckMarginProperty);
		}

	#endregion

	#region thickness - content margin

		public static readonly DependencyProperty
			CheckBoxContentMarginProperty = DependencyProperty.RegisterAttached(
				"CheckBoxContentMargin", typeof(Thickness),
				typeof(CsCheckBoxAp), new FrameworkPropertyMetadata(new Thickness(0),
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCheckBoxContentMargin(UIElement e, Thickness value)
		{
			e.SetValue(CheckBoxContentMarginProperty, value);
		}

		public static Thickness GetCheckBoxContentMargin(UIElement e)
		{
			return (Thickness) e.GetValue(CheckBoxContentMarginProperty);
		}

	#endregion

	#region brush - option mark fill color

		public static readonly DependencyProperty OptionMarkFillBrushProperty = DependencyProperty.RegisterAttached(
			"OptionMarkFillBrush", typeof(SolidColorBrush), typeof(CsCheckBoxAp),
			new FrameworkPropertyMetadata(Brushes.DimGray, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetOptionMarkFillBrush(UIElement e, SolidColorBrush value)
		{
			e.SetValue(OptionMarkFillBrushProperty, value);
		}

		public static SolidColorBrush GetOptionMarkFillBrush(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(OptionMarkFillBrushProperty);
		}

	#endregion

	#region brush - Indeterminant mark fill color

		public static readonly DependencyProperty IndeterminantMarkFillBrushProperty = DependencyProperty.RegisterAttached(
			"IndeterminantMarkFillBrush", typeof(SolidColorBrush), typeof(CsCheckBoxAp),
			new FrameworkPropertyMetadata(Brushes.DimGray, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetIndeterminantMarkFillBrush(UIElement e, SolidColorBrush value)
		{
			e.SetValue(IndeterminantMarkFillBrushProperty, value);
		}

		public static SolidColorBrush GetIndeterminantMarkFillBrush(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(IndeterminantMarkFillBrushProperty);
		}

	#endregion


	#region data template - toggle switch left

		public static readonly DependencyProperty
			ToggleLeftContentTemplateProperty = DependencyProperty.RegisterAttached(
				"ToggleLeftContentTemplate", typeof(DataTemplate), typeof(CsCheckBoxAp), 
				new FrameworkPropertyMetadata(new DataTemplate(),
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetToggleLeftContentTemplate(UIElement e, DataTemplate value)
		{
			e.SetValue(ToggleLeftContentTemplateProperty, value);
		}

		public static DataTemplate GetToggleLeftContentTemplate(UIElement e)
		{
			return (DataTemplate) e.GetValue(ToggleLeftContentTemplateProperty);
		}

	#endregion

	#region data template - toggle switch right

		public static readonly DependencyProperty
			ToggleRightContentTemplateProperty = DependencyProperty.RegisterAttached(
				"ToggleRightContentTemplate", typeof(DataTemplate), typeof(CsCheckBoxAp), 
				new FrameworkPropertyMetadata(new DataTemplate(),
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetToggleRightContentTemplate(UIElement e, DataTemplate value)
		{
			e.SetValue(ToggleRightContentTemplateProperty, value);
		}

		public static DataTemplate GetToggleRightContentTemplate(UIElement e)
		{
			return (DataTemplate) e.GetValue(ToggleRightContentTemplateProperty);
		}

	#endregion


	#region double - toggle switch switch width

		public static readonly DependencyProperty ToggleSwitchWidthProperty = DependencyProperty.RegisterAttached(
			"ToggleSwitchWidth", typeof(double), typeof(CsCheckBoxAp),
			new FrameworkPropertyMetadata(8.0, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetToggleSwitchWidth(UIElement e, double value)
		{
			e.SetValue(ToggleSwitchWidthProperty, value);
		}

		public static double GetToggleSwitchWidth(UIElement e)
		{
			return (double) e.GetValue(ToggleSwitchWidthProperty);
		}

	#endregion


		
	#region object - toggle switch left content

		public static readonly DependencyProperty
			ToggleSwitchLeftContentProperty = DependencyProperty.RegisterAttached(
				"ToggleSwitchLeftContent", typeof(object), typeof(CsCheckBoxAp), 
				new FrameworkPropertyMetadata(new object(),
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetToggleSwitchLeftContent(UIElement e, object value)
		{
			e.SetValue(ToggleSwitchLeftContentProperty, value);
		}

		public static object GetToggleSwitchLeftContent(UIElement e)
		{
			return (object) e.GetValue(ToggleSwitchLeftContentProperty);
		}

	#endregion

	#region corner radius - toggle switch corner radius

		public static readonly DependencyProperty
			ToggleSwitchCornerRadiusProperty = DependencyProperty.RegisterAttached(
				"ToggleSwitchCornerRadius", typeof(CornerRadius), typeof(CsCheckBoxAp), 
				new FrameworkPropertyMetadata(new CornerRadius(0),
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetToggleSwitchCornerRadius(UIElement e, CornerRadius value)
		{
			e.SetValue(ToggleSwitchCornerRadiusProperty, value);
		}

		public static CornerRadius GetToggleSwitchCornerRadius(UIElement e)
		{
			return (CornerRadius) e.GetValue(ToggleSwitchCornerRadiusProperty);
		}

	#endregion

	#region corner radius - toggle outter corner radius

		public static readonly DependencyProperty
			ToggleOutterCornerRadiusProperty = DependencyProperty.RegisterAttached(
				"ToggleOutterCornerRadius", typeof(CornerRadius), typeof(CsCheckBoxAp), 
				new FrameworkPropertyMetadata(new CornerRadius(0),
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetToggleOutterCornerRadius(UIElement e, CornerRadius value)
		{
			e.SetValue(ToggleOutterCornerRadiusProperty, value);
		}

		public static CornerRadius GetToggleOutterCornerRadius(UIElement e)
		{
			return (CornerRadius) e.GetValue(ToggleOutterCornerRadiusProperty);
		}

	#endregion

	#region corner radius - toggle inner corner radius

		public static readonly DependencyProperty
			ToggleInnerCornerRadiusProperty = DependencyProperty.RegisterAttached(
				"ToggleInnerCornerRadius", typeof(CornerRadius), typeof(CsCheckBoxAp), 
				new FrameworkPropertyMetadata(new CornerRadius(0),
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetToggleInnerCornerRadius(UIElement e, CornerRadius value)
		{
			e.SetValue(ToggleInnerCornerRadiusProperty, value);
		}

		public static CornerRadius GetToggleInnerCornerRadius(UIElement e)
		{
			return (CornerRadius) e.GetValue(ToggleInnerCornerRadiusProperty);
		}

	#endregion


	}
}