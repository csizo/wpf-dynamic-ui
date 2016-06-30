using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Csizmazia.Collections
{
    public class WeakList<T> : IList<T>
    {
        private readonly List<WeakReference> innerList;

        public WeakList()
        {
            innerList = new List<WeakReference>();
        }

        public WeakList(int capacity)
        {
            innerList = new List<WeakReference>(capacity);
        }

        public WeakList(IEnumerable<T> items)
        {
            innerList = new List<WeakReference>(items.Select(item => new WeakReference(item)));
        }

        #region IList<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            var copy = new T[innerList.Count];
            innerList.Select(w => (T) w.Target).ToList().CopyTo(copy);

            return copy.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            innerList.Add(new WeakReference(item));
        }

        public void Clear()
        {
            innerList.Clear();
        }

        public bool Contains(T item)
        {
            return innerList.Any(w => item.Equals(w.Target));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
        }

        public bool Remove(T item)
        {
            int count = innerList.RemoveAll(w => item.Equals(w.Target));
            return count > 0;
        }

        public int Count
        {
            get { return innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int IndexOf(T item)
        {
            return innerList.FindIndex(w => item.Equals(w.Target));
        }

        public void Insert(int index, T item)
        {
            innerList.Insert(index, new WeakReference(item));
        }

        public void RemoveAt(int index)
        {
            innerList.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return (T) innerList[index].Target; }
            set { innerList[index] = new WeakReference(value); }
        }

        #endregion

        public void Clear(bool nullOnly)
        {
            if (nullOnly)
            {
                innerList.RemoveAll(w => w.Target == null);
            }
            else
            {
                Clear();
            }
        }
    }
}