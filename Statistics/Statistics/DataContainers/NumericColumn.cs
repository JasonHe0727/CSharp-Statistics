using System;
using System.Collections;
using System.Collections.Generic;

namespace Statistics.DataContainers
{
    public class NumericColumn:IEnumerable<double>
    {
        private DataFrame frame;
        private List<double> data;
        private string columnName;

        public DataFrame Frame { get { return this.frame; } }

        public Type type { get { return typeof(double); } }

        public string ColumnName { get { return this.columnName; } }

        public NumericColumn(DataFrame frame, string columnName)
        {
            this.columnName = columnName;
            this.frame = frame;
            this.data = new List<double>();
        }

        public int Count { get { return this.data.Count; } }

        public void Add(object value)
        {
            if (value is double)
            {
                data.Add((double)value);
            }
            else if (value == null)
            {
                data.Add(double.NaN);
            }
            else
            {
                data.Add(Convert.ToDouble(value));
            }
        }

        public void Remove(object value)
        {
            if (value is double)
            {
                data.Remove((double)value);
            }
            else if (value == null)
            {
                data.Remove(double.NaN);
            }
            else
            {
                data.Remove(Convert.ToDouble(value));
            }
        }

        public void RemoveAt(int index)
        {
            data.RemoveAt(index);
        }


        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(object value)
        {
            if (value is double)
            {
                return  data.Contains((double)value);
            }
            else if (value == null)
            {
                return data.Contains(double.NaN);
            }
            else
            {
                return data.Contains(Convert.ToDouble(value));
            }
        }

        public int IndexOf(object value)
        {
            if (value is double)
            {
                return data.IndexOf((double)value);
            }
            else if (value == null)
            {
                return data.IndexOf(double.NaN);
            }
            else
            {
                return data.IndexOf(Convert.ToDouble(value));
            }
        }

        public void Insert(int index, object value)
        {
            if (value is double)
            {
                data.Insert(index, (double)value);
            }
            else if (value == null)
            {
                data.Insert(index, double.NaN);
            }
            else
            {
                data.Insert(index, Convert.ToDouble(value));
            }
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
                if (value is double)
                {
                    data[index] = (double)value;
                }
                else if (value == null)
                {
                    data[index] = double.NaN;
                }
                else
                {
                    data[index] = Convert.ToDouble(value);
                }
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (var item in data)
            {
                yield return item;
            }
        }

        public IEnumerator<double> GetEnumerator()
        {
            foreach (var item in data)
            {
                yield return item;
            }
        }

    }
}

