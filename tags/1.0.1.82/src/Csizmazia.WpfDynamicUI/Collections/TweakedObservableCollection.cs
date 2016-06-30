using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Csizmazia.Tracing;

namespace Csizmazia.Collections
{
    public class TweakedObservableCollection<T> : ObservableCollection<T>
    {
        private readonly Tracer<TweakedObservableCollection<T>> Tracer = Tracer<TweakedObservableCollection<T>>.Instance;
        private bool _isChangeNotificationEnabled;

        public TweakedObservableCollection()
        {
        }

        public TweakedObservableCollection(IEnumerable<T> items)
            : base(items)
        {
        }

        public TweakedObservableCollection(List<T> items)
            : base(items)
        {
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            using (new MethodTracer<TweakedObservableCollection<T>>("AddRange"))
            {
                Tracer.Verbose(() => "disabling change notification");
                _isChangeNotificationEnabled = false;

                Tracer.Verbose(() => "adding items to collection");
                foreach (T item in items)
                {
                    Add(item);
                }

                Tracer.Verbose(() => "enabling change notification");
                _isChangeNotificationEnabled = true;

                Tracer.Verbose(() => "raising changed events");
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            using (new MethodTracer<TweakedObservableCollection<T>>("RemoveRange"))
            {
                Tracer.Verbose(() => "disabling change notification");
                _isChangeNotificationEnabled = false;

                Tracer.Verbose(() => "removing items from collection");
                foreach (T item in items)
                {
                    Remove(item);
                }

                Tracer.Verbose(() => "enabling change notification");
                _isChangeNotificationEnabled = true;

                Tracer.Verbose(() => "raising changed events");
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }


        public void ChangingItems(IEnumerable<T> itemsToRemove, IEnumerable<T> itemsToAdd)
        {
            if (itemsToRemove == null) throw new ArgumentNullException("itemsToRemove");
            if (itemsToAdd == null) throw new ArgumentNullException("itemsToAdd");

            using (new MethodTracer<TweakedObservableCollection<T>>("ChangingItems"))
            {
                T[] toRemove = itemsToRemove.ToArray();
                T[] toAdd = itemsToAdd.ToArray();

                Tracer.Verbose(() => "disabling change notification");
                _isChangeNotificationEnabled = false;

                Tracer.Verbose(() => "removing old items from collection");

                foreach (T item in toRemove)
                {
                    Remove(item);
                }

                Tracer.Verbose(() => "adding new items to collection");
                foreach (T item in toAdd)
                {
                    Add(item);
                }


                Tracer.Verbose(() => "enabling change notification");
                _isChangeNotificationEnabled = true;

                Tracer.Verbose(() => "raising changed events");
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            Tracer.Verbose(() => "checking for change notification enabled");
            if (_isChangeNotificationEnabled)
            {
                Tracer.Verbose(() => string.Format("notifying collectionchanged ({0})", e.Action));
                base.OnCollectionChanged(e);
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            Tracer.Verbose(() => "checking for change notification enabled");
            if (_isChangeNotificationEnabled)
            {
                Tracer.Verbose(() => string.Format("notifying PropertyChanged '{0}'", e.PropertyName));
                base.OnPropertyChanged(e);
            }
        }
    }
}