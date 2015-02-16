using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace pc_sw.Model
{
    public class ObservableQueue<T>: INotifyCollectionChanged, IEnumerable<T>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count { get { return _count; } protected set { _count = value; } }
        public bool NotifyUpdates { get { return _notifyUpdates; } }

        protected Queue<T> Queue { get { return _queue; } }
        protected int FixedCapacity { get { return _fixedCapacity; } }

        private Queue<T> _queue;
        private readonly int _fixedCapacity;
        private int _count = 0;
        
        private bool _notifyUpdates = true;

        public ObservableQueue()
        {
            _queue = new Queue<T>();
            _fixedCapacity = Int32.MaxValue;
        }

        public ObservableQueue(Int32 fixedCapacity)
        {
            _queue = new Queue<T>(fixedCapacity);
            _fixedCapacity = fixedCapacity;
        }

        public void Clear()
        {
            _queue.Clear();
            _count = 0;
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));
        }

        public void Enqueue(T item)
        {
            if (_count >= _fixedCapacity)
            {
                T removedItem = _queue.Dequeue();
                this.OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove,
                        removedItem, 0));
            }
            else
            {
                _count++;
            }
            
            _queue.Enqueue(item);
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, item));
        }

        public T Dequeue()
        {
            if (_count > 0)
            {
                _count--;
            }

            T item = _queue.Dequeue();

            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    item, 0));
            return item;
        }

        public T PeekFirst()
        {
            return _queue.Peek();
        }

        public T PeekLast()
        {
            return _queue.ElementAt(_count - 1);
        }

        public T ElementAt(int index)
        {
            return _queue.ElementAt(index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        public void PauseUpdates()
        {
            _notifyUpdates = false;
        }

        public void ResumeUpdates()
        {
            _notifyUpdates = true;
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_notifyUpdates && this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }
    }
}
