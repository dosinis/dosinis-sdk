using System;
using System.Collections;
using System.Collections.Generic;

namespace DosinisSDK.Utils
{
    [Serializable]
    public class ObservableList<T> : IList<T>, IObservableList<T>
    {
        private readonly List<T> list = new List<T>();
		
        // Events
		
        public event Action OnCollectionModified;
		
        // List utils
		
        public T Find(Predicate<T> match)
        {
            return list.Find(match);
        }
		
        public List<T> FindAll(Predicate<T> match)
        {
            return list.FindAll(match);
        }

        // IList implementation
		
        public int Count => list.Count;
        public bool IsReadOnly => false;

        public T this[int index]
        {
            get => list[index];

            set
            {
                list[index] = value;
                OnCollectionModified?.Invoke();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            list.Add(item);
            OnCollectionModified?.Invoke();
        }

        public void Clear()
        {
            list.Clear();
            OnCollectionModified?.Invoke();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            bool removed = list.Remove(item);
			
            OnCollectionModified?.Invoke();

            return removed;
        }

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            list.Insert(index, item);
            OnCollectionModified?.Invoke();
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
            OnCollectionModified?.Invoke();
        }
    }
    
    public interface IObservableList<in T>
    {
        event Action OnCollectionModified;
        bool Contains(T item);
        int IndexOf(T item);
    }
}
