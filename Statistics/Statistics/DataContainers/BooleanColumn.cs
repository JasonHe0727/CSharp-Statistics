using System;
using System.Collections.Generic;
using System.Collections;

namespace Statistics.DataContainers
{
    public class BooleanColumn:IEnumerable<bool>
    {
        private DataFrame frame;
        private List<BooleanValue> data;
        private string columnName;

        public DataFrame Frame { get { return this.frame; } }

        public Type type { get { return typeof(bool); } }

        public string ColumnName { get { return this.columnName; } }

        public BooleanColumn(DataFrame frame, string columnName)
        {
            this.columnName = columnName;
            this.frame = frame;
            this.data = new List<BooleanValue>();
        }

        public int Count { get { return this.data.Count; } }

        public void Add(object value)
        {
            if (value is bool)
            {
                data.Add((bool)value ? BooleanValue.True : BooleanValue.False);
            }
            else if (value == null)
            {
                data.Add(BooleanValue.NaN);
            }
            else
            {
                data.Add(Convert.ToBoolean(value) ? BooleanValue.True : BooleanValue.False);
            }
        }

        public void Remove(object value)
        {
            if (value is bool)
            {
                data.Remove((bool)value ? BooleanValue.True : BooleanValue.False);
            }
            else if (value == null)
            {
                data.Remove(BooleanValue.NaN);
            }
            else
            {
                data.Remove(Convert.ToBoolean(value) ? BooleanValue.True : BooleanValue.False);
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
            if (value is bool)
            {
                return  data.Contains((bool)value ? BooleanValue.True : BooleanValue.False);
            }
            else if (value == null)
            {
                return data.Contains(BooleanValue.NaN);
            }
            else
            {
                return data.Contains(Convert.ToBoolean(value) ? BooleanValue.True : BooleanValue.False);
            }
        }

        public int IndexOf(object value)
        {
            if (value is bool)
            {
                return data.IndexOf((bool)value ? BooleanValue.True : BooleanValue.False);
            }
            else if (value == null)
            {
                return data.IndexOf(BooleanValue.NaN);
            }
            else
            {
                return data.IndexOf(Convert.ToBoolean(value) ? BooleanValue.True : BooleanValue.False);
            }
        }

        public void Insert(int index, object value)
        {
            if (value is bool)
            {
                data.Insert(index, (bool)value ? BooleanValue.True : BooleanValue.False);
            }
            else if (value == null)
            {
                data.Insert(index, BooleanValue.NaN);
            }
            else
            {
                data.Insert(index, Convert.ToBoolean(value) ? BooleanValue.True : BooleanValue.False);
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
            get
            { 
                BooleanValue value = this.data[index];
                if (value == BooleanValue.True)
                {
                    return true;
                }
                else if (value == BooleanValue.False)
                {
                    return false;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value is bool)
                {
                    data[index] = (bool)value ? BooleanValue.True : BooleanValue.False;
                }
                else if (value == null)
                {
                    data[index] = BooleanValue.NaN;
                }
                else
                {
                    data[index] = Convert.ToBoolean(value) ? BooleanValue.True : BooleanValue.False;
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
                if (item == BooleanValue.True)
                {
                    yield return true;
                }
                else if (item == BooleanValue.False)
                {
                    yield return false;
                }
                else
                {
                    yield return null;
                }
            }
        }

        public IEnumerator<bool> GetEnumerator()
        {
            foreach (var item in data)
            {
                if (item == BooleanValue.True)
                {
                    yield return true;
                }
                else if (item == BooleanValue.False)
                {
                    yield return false;
                }
                else
                {
                    throw new InvalidCastException("cannot cast 'NaN' to 'bool'.");
                }
            }
        }

        private enum BooleanValue: byte
        {
            True = (byte)1,
            False = (byte)0,
            NaN = (byte)2
        }
    }


}

