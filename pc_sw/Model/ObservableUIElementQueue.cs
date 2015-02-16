using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace pc_sw.Model
{
    public class ObservableUIElementQueue : UIElementCollection, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count { get { return _queue.Count; } }

        private ObservableQueue<UIElement> _queue;

        public bool IsReadOnly
        {
            get { return false; }
        }

        public override UIElement this[int index]
        {
            get
            {
                return this._queue.ElementAt(index);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ObservableUIElementQueue(UIElement visualParent, FrameworkElement logicalParent)
            : base(visualParent, logicalParent)
        {
            this._queue = new ObservableQueue<UIElement>(8);
            this._queue.CollectionChanged += _queue_CollectionChanged;
        }

        public void Enqueue(UIElement element)
        {
            _queue.Enqueue(element);
        }

        public UIElement Dequeue()
        {
            return _queue.Dequeue();
        }

        void _queue_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(sender, e);
            }
        }

        public override int Add(UIElement element)
        {
            _queue.Enqueue(element);

            return -1;
        }

        public override void Remove(UIElement element)
        {
            if (element == _queue.PeekFirst())
            {
                _queue.Dequeue();
            }
            else
            {
                throw new ArgumentException("Removing queued items");
            }
        }
    }
}
