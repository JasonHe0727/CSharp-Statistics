using System;
using System.Collections;
using System.Collections.Generic;

namespace Statistics.DataContainers
{
    public class Series: IEnumerable
    {
        readonly string name;
        readonly ArrayList data;

        Type _type;
        bool unique = false;

        public Type type { get { return this._type; } }

        public string Name { get { return this.name; } }

        public bool Unique
        {
            get { return this.unique; }
            set
            {
                if (!this.unique && value)
                {
                    this.CheckUnique();
                }
                this.unique = value;
            }
        }

        public static readonly Series Empty = new Series();

        public bool IsEmpty
        { 
            get
            {
                return this.name == string.Empty && this._type == null && this.data == null;
            }
        }

        private Series()
        {
            this.name = string.Empty;
            this._type = null;
            this.data = null;
        }

        public Series(string name, Type type)
        {
            this.name = name;
            this._type = type;
            this.data = new ArrayList();
        }

        public Series(string name, Type type, int capacity)
        {
            this.name = name;
            this._type = type;
            this.data = new ArrayList(capacity);
        }

        public int Count { get { return this.data.Count; } }

        public static Series Create<T>(string name, ICollection<T> values)
        {
            var series = new Series(name, typeof(T), values.Count);
            foreach (var item in values)
            {
                series.data.Add(item);
            }
            return series;
        }

        internal void Add(object value)
        {
            if (value == null)
            {
                data.Add(null);
            }
            else
            {
                data.Add(Convert.ChangeType(value, this._type));
            }
        }

        internal void Remove(object value)
        {
            data.Remove(value);
        }

        internal void RemoveAt(int index)
        {
            data.RemoveAt(index);
        }


        internal void Clear()
        {
            data.Clear();
        }

        public bool Contains(object value)
        {
            return data.Contains(value);
        }

        public int IndexOf(object value)
        {
            return data.IndexOf(value);
        }

        internal void Insert(int index, object value)
        {
            data.Insert(index, value);
        }

        public object this [int index]
        {
            get{ return this.data[index]; }
            set
            {
                if (value == null)
                {
                    this.data[index] = null;
                }
                else
                {
                    this.data[index] = Convert.ChangeType(value, this._type);
                }
            }
        }

        public void CopyTo(Array array, int index)
        {
            this.data.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var item in data)
            {
                yield return item;
            }
        }

        public void ConvertAll(Type type)
        {
            this._type = type;
            for (int i = 0; i < this.Count; i++)
            {
                object value = this.data[i];
                if (value != null)
                {
                    this.data[i] = Convert.ChangeType(value, type);
                }
            }
        }

        public void Apply(Func<object,object> f)
        {
            for (int i = 0; i < this.Count; i++)
            {
                this[i] = f(this[i]);
            }
        }

        private void CheckUnique()
        {
            var table = new Hashtable();
            foreach (var item in this.data)
            {
                if (table.ContainsKey(item))
                {
                    throw new Exception(
                        string.Format("Column '{0}' contains non-unique values.", this.name));
                }
            }
        }

        public T[] As<T>()
        {
            T[] array = new T[this.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (T)this.data[i];
            }
            return array;
        }

        public void TrimToSize()
        {
            this.data.TrimToSize();
        }

        #region Operators

        public static BooleanArray operator ==(Series series, object value)
        {
            BooleanArray result = new BooleanArray(series.Count);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = object.Equals(series.data[i], value);
            }
            return result;
        }

        public static BooleanArray operator ==(object value, Series series)
        {
            return series == value;
        }

        public static BooleanArray operator !=(Series series, object value)
        {
            BooleanArray result = new BooleanArray(series.Count);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = !object.Equals(series.data[i], value);
            }
            return result;
        }

        public static BooleanArray operator !=(object value, Series series)
        {
            return series != value;
        }

        public static BooleanArray Compare(Series series, object value, Func<object,object, bool> comparer)
        {
            BooleanArray result = new BooleanArray(series.Count);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = comparer(series.data[i], value);
            }
            return result;
        }

        public static BooleanArray operator >(Series series, object value)
        {
            return Compare(series, value, (x, y) => Comparer.Default.Compare(x, y) > 0);
        }

        public static BooleanArray operator <(Series series, object value)
        {
            return Compare(series, value, (x, y) => Comparer.Default.Compare(x, y) < 0);
        }

        public static BooleanArray operator >=(Series series, object value)
        {
            return Compare(series, value, (x, y) => Comparer.Default.Compare(x, y) >= 0);
        }

        public static BooleanArray operator <=(Series series, object value)
        {
            return Compare(series, value, (x, y) => Comparer.Default.Compare(x, y) <= 0);
        }


        public static BooleanArray operator >(object value, Series series)
        {
            return series < value;
        }

        public static BooleanArray operator <(object value, Series series)
        {
            return series > value;
        }

        public static BooleanArray operator >=(object value, Series series)
        {
            return series <= value;
        }

        public static BooleanArray operator <=(object value, Series series)
        {
            return series >= value;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return object.Equals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

