using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CodeStock.App.Controls
{
    public class PhoneSlider : Slider
    {
        public PhoneSlider()
        {
            SizeChanged += PhoneSlider_SizeChanged;
        }

        private void PhoneSlider_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                Rect clipRect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
                if (Orientation == Orientation.Horizontal)
                {
                    clipRect.X -= 12;
                    clipRect.Width += 24;
                    object margin = Resources["PhoneHorizontalMargin"];
                    if (margin != null)
                    {
                        Margin = (Thickness)margin;
                    }
                }
                else
                {
                    clipRect.Y -= 12;
                    clipRect.Height += 24;
                    object margin = Resources["PhoneVerticalMargin"];
                    if (margin != null)
                    {
                        Margin = (Thickness)margin;
                    }
                }

                this.Clip = new RectangleGeometry() { Rect = clipRect };
            }
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            var val = Math.Round(newValue * 2) / 2;
            if (val != newValue) Value = val;
        }
    }
}
