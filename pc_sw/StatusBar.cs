using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using pc_sw.Model;
using System.Collections.Specialized;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Threading;

namespace pc_sw
{
    //[ContentProperty("ChildQueue")]
    public class StatusBar : Panel
    {
        public static readonly DependencyProperty ItemColorProperty =
            DependencyProperty.Register("ItemColor", typeof(Brush), typeof(StatusBar),
            new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x96, 0x40))));

        public static readonly DependencyProperty DiscardTimeProperty =
            DependencyProperty.Register("DiscardTime", typeof(Int32), typeof(StatusBar),
            new FrameworkPropertyMetadata(3, DiscardTimeChanged));

        public static readonly DependencyProperty StatusMessageQueueProperty =
            DependencyProperty.Register("StatusMessageQueue", typeof(ObservableQueue<String>), typeof(StatusBar),
            new FrameworkPropertyMetadata(new ObservableQueue<String>(), MessageQueueChanged));

        private const Double _childSeparatorWidth = 2.0;

        private Queue<StatusItem> _itemQueue;
        private Queue<UIElement> _elementsToRemove;
        private bool _itemRemovalOngoing = false;
        private StatusVisualization _currentStatus;
        private DispatcherTimer _cleanupTimer;

        public Brush ItemColor
        {
            get { return (Brush)GetValue(ItemColorProperty); }
            set { SetValue(ItemColorProperty, value); }
        }

        public Int32 DiscardTime
        {
            get { return (Int32)GetValue(DiscardTimeProperty); }
            set { SetValue(DiscardTimeProperty, value); }
        }

        public ObservableQueue<String> StatusMessageQueue
        {
            get { return (ObservableQueue<String>)GetValue(StatusMessageQueueProperty); }
            set { SetValue(StatusMessageQueueProperty, value); }
        }

        public StatusBar()
        {
            this.StatusMessageQueue.CollectionChanged += MessageQueue_CollectionChanged;
            _elementsToRemove = new Queue<UIElement>();
            _itemQueue = new Queue<StatusItem>();

            _cleanupTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) };
            _cleanupTimer.Tick += _cleanupTimer_Tick;
            _cleanupTimer.Start();
        }

        void _cleanupTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < _itemQueue.Count; i++)
            {
                var item = _itemQueue.Peek();

                if (item.Visual != _currentStatus &&
                    item.TimeToRemoval <= 0)
                {
                    RemoveStatusAnimation(_itemQueue.Dequeue().Visual);
                }
                else
                {
                    break;
                }
            }

            foreach (var item in _itemQueue)
            {
                if (item.Visual != _currentStatus)
                {
                    item.TimeToRemoval--;
                }
            }
        }

        private static void DiscardTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Int32 deltaTime = ((Int32)e.NewValue) - ((Int32)e.OldValue);

            foreach (var item in ((StatusBar)d)._itemQueue)
            {
                item.TimeToRemoval += deltaTime;
            }
        }

        private static void MessageQueueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ((ObservableQueue<String>)e.OldValue).CollectionChanged -= ((StatusBar)d).MessageQueue_CollectionChanged;
            }

            if (e.NewValue != null)
            {
                ((ObservableQueue<String>)e.NewValue).CollectionChanged += ((StatusBar)d).MessageQueue_CollectionChanged;
            }
        }

        void MessageQueue_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (_currentStatus != null)
                    {
                        _currentStatus.Current = false;
                    }
                    StatusVisualization newStatus = new StatusVisualization(this.ItemColor)
                        { Text = (String)e.NewItems[0] };

                    _itemQueue.Enqueue(new StatusItem(newStatus, DiscardTime));
                    this.Children.Add(newStatus);
                    
                    _currentStatus = newStatus;
                    _currentStatus.Current = true;
                    NewStatusAnimation(newStatus);
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();
                case NotifyCollectionChangedAction.Remove:
                    RemoveStatusAnimation(_itemQueue.Dequeue().Visual);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException();
                case NotifyCollectionChangedAction.Reset:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        private void NewStatusAnimation(StatusVisualization element)
        {
            DoubleAnimation animation = new DoubleAnimation(
                1.0, 0.0, new Duration(TimeSpan.FromSeconds(0.5)));
            animation.EasingFunction = new CubicEase();

            element.BeginAnimation(StatusVisualization.OffsetProperty, animation);
        }

        private void RemoveStatusAnimation(StatusVisualization element)
        {
            DoubleAnimation animation = new DoubleAnimation(
                0.0, -1.0, new Duration(TimeSpan.FromSeconds(0.4)));
            animation.Completed += animation_Completed;
            _elementsToRemove.Enqueue(element);

            element.BeginAnimation(StatusVisualization.OffsetProperty, animation);
        }

        void animation_Completed(object sender, EventArgs e)
        {
            this.Children.IndexOf(new UIElement());
            this.Children.Remove(_elementsToRemove.Dequeue());
            _itemRemovalOngoing = false;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize = new Size();

            if (this.InternalChildren != null)
            {
                foreach (StatusVisualization child in this.InternalChildren)
                {
                    if (child != null)
                    {
                        child.Measure(availableSize);

                        desiredSize.Height = Math.Max(desiredSize.Height, child.DesiredSize.Height);
                        desiredSize.Width += child.DesiredSize.Width;
                    }
                }

                desiredSize.Width += Math.Max(this.Children.Count - 1, 0) * _childSeparatorWidth;
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Double childBaseOffset = 0.0;

            if (this.InternalChildren != null)
            {
                foreach (StatusVisualization child in this.InternalChildren)
                {
                    Double childOffsetProp = child.Offset;

                    if (!Double.IsNaN(childOffsetProp))
                    {
                        Double childAdditionalOffset;

                        if (childOffsetProp > 0)
                        {
                            childAdditionalOffset = childOffsetProp * (finalSize.Width - childBaseOffset);
                        }
                        else
                        {
                            childAdditionalOffset = childOffsetProp * (child.DesiredSize.Width + childBaseOffset);
                        }

                        if (childBaseOffset + child.DesiredSize.Width > finalSize.Width &&
                            _itemRemovalOngoing == false)
                        {
                            this.StatusMessageQueue.Dequeue();
                            _itemRemovalOngoing = true;
                        }

                        child.Arrange(new Rect(new Point(childBaseOffset + childAdditionalOffset, 0),
                            new Size(child.DesiredSize.Width, finalSize.Height)));
                        childBaseOffset += childAdditionalOffset + child.DesiredSize.Width + _childSeparatorWidth;
                    }
                    else
                    {
                        child.Arrange(new Rect(new Point(childBaseOffset, 0),
                            new Size(child.DesiredSize.Width, finalSize.Height)));
                        childBaseOffset += child.DesiredSize.Width + _childSeparatorWidth;
                    }
                }
            }
            return finalSize;
        }
    }
}
