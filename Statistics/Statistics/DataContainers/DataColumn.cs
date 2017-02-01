using System;
using System.Collections;

namespace Statistics.DataContainers
{
    public class DataColumn:ICollection, IEnumerable,ICloneable
    {
        private DataFrame frame;
        private ArrayList data;
        private Type _type;
        private string columnName;

        public DataFrame Frame { get { return this.frame; } }

        public Type type { get { return this._type; } }

        public string ColumnName { get { return this.columnName; } }

        public DataColumn(DataFrame frame, string columnName, Type type)
        {
            this.columnName = columnName;
            this.frame = frame;
            this._type = type;
            this.data = new ArrayList();
        }

        public DataColumn(DataFrame frame, string columnName, Type type, int capacity)
        {
            this.columnName = columnName;
            this.frame = frame;
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

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this.data.SyncRoot;
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

        public object Clone()
        {
            var newColumn = new DataColumn(this.frame, this.columnName, this.type, this.Count);
            foreach (var item in this.data)
            {
                newColumn.data.Add(item);
            }
            return newColumn;
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
    }
}

