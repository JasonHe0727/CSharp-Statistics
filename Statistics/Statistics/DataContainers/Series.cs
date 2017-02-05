using System;
using System.Collections;

namespace Statistics.DataContainers
{
    public class Series:IEnumerable
    {
        private ArrayList data;
        private Type _type;
        private string columnName;
        private bool unique = false;

        public Type type { get { return this._type; } }

        public string ColumnName { get { return this.columnName; } }

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

        public Series(string columnName, Type type)
        {
            this.columnName = columnName;
            this._type = type;
            this.data = new ArrayList();
        }

        public Series(int capacity, Type type)
        {
            this.columnName = "Anonymous_Column";
            this._type = type;
            this.data = new ArrayList(capacity);
        }

        public Series(string columnName, Type type, int capacity)
        {
            this.columnName = columnName;
            this._type = type;
            this.data = new ArrayList(capacity);
        }


        public int Count { get { return this.data.Count; } }

        internal int Add(object value)
        {
            if (value == null)
            {
                return data.Add(null);
            }
            else
            {
                return data.Add(Convert.ChangeType(value, this._type));
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

        public bool IsFixedSize
        {
            get{ return false; }
        }

        public bool IsReadOnly
        {
            get{ return false; }
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

        /*public object Clone()
        {
            Series ne
            var newColumn = new Series(this.frame, this.columnName, this.type, this.Count);
            foreach (var item in this.data)
            {
                newColumn.data.Add(item);
            }
            return newColumn;
        }*/

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
                        string.Format("Column '{0}' contains non-unique values.", this.columnName));
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

        public static Series operator ==(Series series, object value)
        {
            Series result = new Series(series.Count, typeof(bool));
            foreach (object item in series.data)
            {
                result.data.Add(object.Equals(item, value));
            }
            return result;
        }

        public static Series operator ==(object value, Series series)
        {
            return series == value;
        }

        public static Series operator !=(Series series, object value)
        {
            Series result = new Series(series.Count, typeof(bool));
            foreach (object item in series.data)
            {
                result.data.Add(!object.Equals(item, value));
            }
            return result;
        }

        public static Series operator !=(object value, Series series)
        {
            return series != value;
        }

        public static Series operator >(Series series, object value)
        {
            value = Convert.ChangeType(value, series.type);
            Series result = new Series(series.Count, typeof(bool));
            foreach (object item in series.data)
            {
                result.data.Add(Comparer.Default.Compare(item, value) > 0);
            }
            return result;
        }

        public static Series operator <(Series series, object value)
        {
            value = Convert.ChangeType(value, series.type);
            Series result = new Series(series.Count, typeof(bool));
            foreach (object item in series.data)
            {
                result.data.Add(Comparer.Default.Compare(item, value) < 0);
            }
            return result;
        }

        public static Series operator >=(Series series, object value)
        {
            value = Convert.ChangeType(value, series.type);
            Series result = new Series(series.Count, typeof(bool));
            foreach (object item in series.data)
            {
                result.data.Add(Comparer.Default.Compare(item, value) >= 0);
            }
            return result;
        }

        public static Series operator <=(Series series, object value)
        {
            value = Convert.ChangeType(value, series.type);
            Series result = new Series(series.Count, typeof(bool));
            foreach (object item in series.data)
            {
                result.data.Add(Comparer.Default.Compare(item, value) <= 0);
            }
            return result;
        }

        public static Series operator &(Series seriesA, Series seriesB)
        {
            if (seriesA.Count == seriesB.Count)
            {
                if (seriesA.type == typeof(bool) && seriesB.type == typeof(bool))
                {
                    Series result = new Series(seriesA.Count, typeof(bool));
                    for (int i = 0; i < seriesA.Count; i++)
                    {
                        result.data.Add((bool)seriesA.data[i] & (bool)seriesB.data[i]);
                    }
                    return result;
                }
                else
                {
                    throw new ArgumentException("The type of series data must be boolean when using operator '&'.");
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("seriesB", "The count of seriesB must be the same as seriesA.");
            }
        }


        public static Series operator |(Series seriesA, Series seriesB)
        {
            if (seriesA.Count == seriesB.Count)
            {
                if (seriesA.type == typeof(bool) && seriesB.type == typeof(bool))
                {
                    Series result = new Series(seriesA.Count, typeof(bool));
                    for (int i = 0; i < seriesA.Count; i++)
                    {
                        result.data.Add((bool)seriesA.data[i] | (bool)seriesB.data[i]);
                    }
                    return result;
                }
                else
                {
                    throw new ArgumentException("The type of series data must be boolean when using operator '|'.");
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("seriesB", "The count of seriesB must be the same as seriesA.");
            }
        }

        public override bool Equals(object obj)
        {
            return object.Equals(this, obj);
        }
    }
}

