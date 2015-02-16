using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace pc_sw
{
    public class StatusVisualization : TextBlock
    {
        private readonly Brush _originalBackground;

        public static readonly DependencyProperty CurrentProperty =
            DependencyProperty.Register("Current", typeof(bool), typeof(StatusVisualization),
            new FrameworkPropertyMetadata(false, SelectedChanged));

        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(Double), typeof(StatusVisualization),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public bool Current
        {
            get { return (bool)GetValue(CurrentProperty); }
            set { SetValue(CurrentProperty, value); }
        }

        public Double Offset
        {
            get { return (Double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public StatusVisualization(Brush background)
        {
            this.Foreground = Brushes.White;
            this.Background = background;
            this._originalBackground = background;


            this.Padding = new Thickness(15, 6, 15, 6);
            this.FontSize = 14;
            this.FontWeight = FontWeights.SemiBold;
            this.FontFamily = new FontFamily("Segoe UI");
            this.TextAlignment = System.Windows.TextAlignment.Center;
        }

        private static void SelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StatusVisualization sv = ((StatusVisualization)d);
            sv.Background = new SolidColorBrush(((SolidColorBrush)sv.Background).Color);
            sv.Foreground = new SolidColorBrush(((SolidColorBrush)sv.Foreground).Color);

            ColorAnimation ca;
            ColorAnimation cb;

            if (((bool)e.NewValue) == true)
            {
                /*
                ca = new ColorAnimation(
                    ((SolidColorBrush)sv.Background).Color,
                    ((SolidColorBrush)sv._originalBackground).Color,
                    new Duration(TimeSpan.FromSeconds(0.5)));
                ca.EasingFunction = new SineEase();

                cb = new ColorAnimation(
                    ((SolidColorBrush)sv.Foreground).Color,
                    ((SolidColorBrush)sv.GetDeselectedBrush(sv.Foreground)).Color,
                    new Duration(TimeSpan.FromSeconds(0.5)));
                cb.EasingFunction = new SineEase();*/
            }
            else
            {
                ca = new ColorAnimation(
                    ((SolidColorBrush)sv._originalBackground).Color,
                    ((SolidColorBrush)sv.GetDeselectedBrush(sv._originalBackground)).Color,
                    new Duration(TimeSpan.FromSeconds(0.5)));
                ca.EasingFunction = new SineEase();

                cb = new ColorAnimation(
                    ((SolidColorBrush)sv.Foreground).Color,
                    ((SolidColorBrush)sv.GetDeselectedBrush(sv.Foreground)).Color,
                    new Duration(TimeSpan.FromSeconds(0.5)));
                cb.EasingFunction = new SineEase();
                sv.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
                sv.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, cb);
            }
        }

        private Brush GetDeselectedBrush(Brush selectedBrush)
        {
            Brush newBrush;

            if (selectedBrush.GetType() == typeof(SolidColorBrush))
            {
                Color c = ((SolidColorBrush)selectedBrush).Color;
                newBrush = new SolidColorBrush();
                Int32 R = c.R, G = c.G, B = c.B;

                ((SolidColorBrush)newBrush).Color = Color.FromArgb(
                    c.A,
                    (Byte)Math.Min((R * 0.75 + 128), 255),
                    (Byte)Math.Min((G * 0.75 + 128), 255),
                    (Byte)Math.Min((B * 0.75 + 128), 255));
            }
            else if (selectedBrush.GetType() == typeof(LinearGradientBrush))
            {
                newBrush = ((LinearGradientBrush)selectedBrush).Clone();

                foreach (var gs in ((LinearGradientBrush)newBrush).GradientStops)
                {
                    Int32 R = gs.Color.R, G = gs.Color.G, B = gs.Color.B;

                    gs.Color = Color.FromArgb(
                        gs.Color.A,
                        (Byte)Math.Min((R + R * 0.25), 255),
                        (Byte)Math.Min((G + G * 0.25), 255),
                        (Byte)Math.Min((B + B * 0.25), 255));
                }
            }
            else
            {
                newBrush = Brushes.Bisque;
            }

            return newBrush;
        }
    }
}
