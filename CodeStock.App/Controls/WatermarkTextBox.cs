using System.Windows;
using System.Windows.Controls;

namespace CodeStock.App.Controls
{
	public class WatermarkedTextBox : TextBox
	{
		ContentControl WatermarkContent;
		public static readonly DependencyProperty WatermarkProperty =
	  DependencyProperty.Register("Watermark", typeof(object), typeof(WatermarkedTextBox), new PropertyMetadata(OnWatermarkPropertyChanged));

		public static readonly DependencyProperty WatermarkStyleProperty =
	  DependencyProperty.Register("WatermarkStyle", typeof(Style), typeof(WatermarkedTextBox), null);

		public Style WatermarkStyle
		{
			get { return base.GetValue(WatermarkStyleProperty) as Style; }
			set { base.SetValue(WatermarkStyleProperty, value); }
		}

		public object Watermark
		{
			get { return base.GetValue(WatermarkProperty) as object; }
			set { base.SetValue(WatermarkProperty, value); }
		}

		public WatermarkedTextBox()
		{
            // typeof wasn't working at least not when in same project and not using generic.xaml but custom resourcedictionary merged in
			DefaultStyleKey = typeof(WatermarkedTextBox);
            //DefaultStyleKey = "WatermarkedTextBoxStyle";
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.WatermarkContent = this.GetTemplateChild("watermarkContent") as ContentControl;
			if(WatermarkContent != null)
			{
			  DetermineWatermarkContentVisibility();
			}
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			if (WatermarkContent != null && string.IsNullOrEmpty(this.Text))
			{
				this.WatermarkContent.Visibility = Visibility.Collapsed;
			}
			base.OnGotFocus(e);
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			if (WatermarkContent != null && string.IsNullOrEmpty(this.Text))
			{
				this.WatermarkContent.Visibility = Visibility.Visible;
			}
			base.OnLostFocus(e);
		}

		private static void OnWatermarkPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			WatermarkedTextBox watermarkTextBox = sender as WatermarkedTextBox;
			if(watermarkTextBox != null && watermarkTextBox.WatermarkContent !=null)
			{
			  watermarkTextBox.DetermineWatermarkContentVisibility();
			}
		}

		private void DetermineWatermarkContentVisibility()
		{
			if (string.IsNullOrEmpty(this.Text))
			{
				this.WatermarkContent.Visibility = Visibility.Visible;
			}
			else
			{
				this.WatermarkContent.Visibility = Visibility.Collapsed;
			}
		}
	}
}
