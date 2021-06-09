using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.Classes
{
    public abstract class AbstractList<T> : IList<T>
    {
        protected List<T> inner = new List<T>();
        public virtual T this[int index]
        {
            get
            {
                return this.inner[index];
            }
            set
            {
                this.inner[index] = value;
            }
        }
        public virtual T First
        {
            get
            {
                return (this.Count > 0) ? this[0] : default(T);
            }
        }
        public virtual T Last
        {
            get
            {
                return (this.Count > 0) ? this[this.Count - 1] : default(T);
            }
        }
        public virtual int Count
        {
            get
            {
                return this.inner.Count;
            }
        }

        public virtual bool IsReadOnly

        {
            get
            {
                return false;
            }
        }

        public virtual void Add(T item)
        {
            this.inner.Add(item);
        }

        public virtual void Clear()
        {
            this.inner.Clear();
        }

        public virtual bool Contains(T item)
        {
            return this.inner.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            this.inner.CopyTo(array, arrayIndex);
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return this.inner.GetEnumerator();
        }

        public virtual int IndexOf(T item)
        {
            return this.inner.IndexOf(item);
        }

        public virtual void Insert(int index, T item)
        {
            this.inner.Insert(index, item);
        }

        public virtual bool Remove(T item)
        {
            return this.inner.Remove(item);
        }

        public virtual void RemoveAt(int index)
        {
            this.inner.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {

            return this.GetEnumerator();
        }
        public virtual void ForEach(Action<T> action)
        {
            this.inner.ForEach(action);
        }
        public virtual T Find(Predicate<T> match)
        {
           return this.inner.Find(match);
        }
        public virtual List<T> FindAll(Predicate<T> match)
        {
            return this.inner.FindAll(match);
        }
        public virtual bool Any()
        {
            return this.Count > 0;
        }
        public virtual bool Any(Predicate<T> predicate)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (predicate(this[i])) return true;
            }
            return false;
        }
        public virtual bool All(Predicate<T> predicate)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (!predicate(this[i])) return false;
            }
            return true;
        }
        
    }
}
