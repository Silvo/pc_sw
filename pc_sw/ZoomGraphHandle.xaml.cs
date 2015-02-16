using System;
using System.Windows;
using System.Windows.Controls;

namespace pc_sw
{
    /// <summary>
    /// Description for ZoomGraphHandle.
    /// </summary>
    public partial class ZoomGraphHandle : UserControl
    {
        public static readonly DependencyProperty LabelMarginProperty =
            DependencyProperty.Register("LabelMargin", typeof(Thickness),
            typeof(ZoomGraphHandle), new PropertyMetadata(new Thickness(-15, -4, 0, 0)));

        public static readonly DependencyProperty LabelContentProperty =
            DependencyProperty.Register("LabelContent", typeof(Object),
            typeof(ZoomGraphHandle), new PropertyMetadata("15.1"));

        public static readonly DependencyProperty LineHeightProperty =
            DependencyProperty.Register("LineHeight", typeof(Double),
            typeof(ZoomGraphHandle), new PropertyMetadata(20.0));

        public Double LineHeight
        {
            get { return (Double)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        public Object LabelContent
        {
            get { return (Object)GetValue(LabelContentProperty); }
            set { SetValue(LabelContentProperty, value); }
        }

        public Thickness LabelMargin
        {
            get { return (Thickness)GetValue(LabelMarginProperty); }
            set { SetValue(LabelMarginProperty, value); }
        }

        public ZoomGraphHandle()
        {
            InitializeComponent();
        }

        private void hostHandle_Loaded(object sender, RoutedEventArgs e)
        {
            LineHeight = this.ActualHeight - 20;
        }

        private void hostHandle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LineHeight = this.ActualHeight - 20;
        }
    }
}