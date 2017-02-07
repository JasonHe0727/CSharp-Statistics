using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;

namespace Statistics.DataContainers
{
    public class DataFrame
    {
        readonly Dictionary<string,Series> columnFromName;
        readonly List<Series> columns;

        int rowCount;
        Series primaryKey = Series.Empty;

        public int RowCount { get { return this.rowCount; } }

        public int ColumnCount { get { return this.columns.Count; } }

        public Series PrimaryKey
        { 
            get
            {
                return this.primaryKey;
            }
            set
            {
                if (this.columns.Contains(value))
                {
                    value.Unique = true;
                    if (this.primaryKey.IsEmpty)
                    {
                        this.primaryKey = value;
                    }
                    else
                    {
                        this.primaryKey.Unique = false;
                        this.primaryKey = value;
                    }
                }
                else
                {
                    throw new ArgumentException("PrimaryKey");
                }
            }
        }

        public bool HasColumn(string columnName)
        {
            return this.columnFromName.ContainsKey(columnName);
        }

        public DataFrame()
        {
            this.columnFromName = new Dictionary<string, Series>();
            this.columns = new List<Series>();
            this.rowCount = 0;
            this.rows = new DataFrameRowCollection(this);
        }

        private DataFrame(int rowCount)
        {
            this.columnFromName = new Dictionary<string, Series>();
            this.columns = new List<Series>();
            this.rowCount = rowCount;
            this.rows = new DataFrameRowCollection(this);
        }

        public ReadOnlyCollection<Series> Columns
        {
            get
            {
                return this.columns.AsReadOnly();
            }
        }

        public ReadOnlyDictionary<string, Series> GetColumnDictionary
        {
            get
            {
                return new ReadOnlyDictionary<string,Series>(this.columnFromName);
            }
        }

        public readonly DataFrameRowCollection rows;

        public DataFrameRowCollection Rows
        {
            get
            {
                return this.rows;
            }
        }

        public void AddRow(params object[] values)
        {
            if (values.Length != columns.Count)
            {
                throw new ArgumentOutOfRangeException("values");
            }
            else
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    columns[i].Add(values[i]);
                }
                rowCount++;
            }
        }

        private void ImportRowUnchecked(DataFrameRow row)
        {
            for (int i = 0; i < this.ColumnCount; i++)
            {
                this.columns[i].Add(row[i]);
            }
            this.rowCount++;
        }

        public void AddColumn(string columnName, Type type)
        {
            if (HasColumn(columnName))
            {
                string message = string.Format("Column {0} has already existed.", columnName);
                throw new ArgumentException(message, "columnName");
            }
            else
            {
                Series newColumn = new Series(columnName, type, this.rowCount);
                ExpandNewColumn(newColumn);
                columnFromName.Add(columnName, newColumn);
                columns.Add(newColumn);
            }
        }

        public void AddColumn(Series column)
        {
            if (this.columns.Count == 0)
            {
                this.columns.Add(column);
                this.columnFromName.Add(column.Name, column);
                this.rowCount = this.columns.Count;
            }
            else
            {
                if (column.Count == this.rowCount)
                {
                    if (HasColumn(column.Name))
                    {
                        string message = string.Format("Column {0} has already existed.", column.Name);
                        throw new ArgumentException(message, "columnName");
                    }
                    else
                    {
                        this.columns.Add(column);
                        this.columnFromName.Add(column.Name, column);
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("column");
                }
            }
        }

        private void ExpandNewColumn(Series newColumn)
        {
            for (int i = 0; i < rowCount; i++)
            {
                newColumn.Add(null);
            }
        }

        public void RemoveRow(int row)
        {
            if (row > 0 && row < rowCount)
            {
                for (int i = 0; i < ColumnCount; i++)
                {
                    this.columns[i].RemoveAt(row);
                }
                this.rowCount--;
            }
            else
            {
                throw new ArgumentOutOfRangeException("row");
            }
        }

        public void RemoveColumn(string columnName)
        {
            Series column;
            if (this.columnFromName.TryGetValue(columnName, out column))
            {
                this.columnFromName.Remove(columnName);
                this.columns.Remove(column);
            }
            else
            {
                throw new ArgumentOutOfRangeException("columnName");
            }
        }

        public object this [int row, int col]
        {
            get
            {
                return this.columns[col][row];
            }
            set
            {
                this.columns[col][row] = value;
            }
        }

        public Series this [string columnName]
        {
            get { return this.columnFromName[columnName]; }
        }

        public DataFrame this [BooleanArray booleanArray]
        {
            get
            {
                if (this.rowCount == booleanArray.Length)
                {
                    DataFrame newFrame = this.Clone();
                    for (int i = 0; i < this.rowCount; i++)
                    {
                        if (booleanArray[i])
                        {
                            DataFrameRow row = this.rows[i];
                            newFrame.ImportRowUnchecked(row);
                        }
                    }
                    return newFrame;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("booleanArray");
                }
            }
        }

        public override string ToString()
        {
            if (ColumnCount == 0)
            {
                return string.Empty;
            }
            else
            {
                StringBuilder builder = new StringBuilder();

                builder.Append(this.columns[0].Name);
                for (int i = 1; i < ColumnCount; i++)
                {
                    builder.Append(',');
                    builder.Append(this.columns[i].Name);
                }
                builder.AppendLine();

                for (int row = 0; row < RowCount; row++)
                {
                    builder.Append(this.columns[0][row].ToString());
                    for (int col = 1; col < ColumnCount; col++)
                    {
                        builder.Append(',');
                        builder.Append(Convert.ToString(this.columns[col][row]));
                    }
                    builder.AppendLine();
                }

                return builder.ToString();
            }
        }

        internal static DataFrame CreatEmptyFrame(string[] columnNames, Type[] types, int rowCapacity)
        {
            var frame = new DataFrame(rowCapacity);
            for (int i = 0; i < columnNames.Length; i++)
            {
                Series newColumn = new Series(columnNames[i], types[i], rowCapacity);
                frame.columnFromName.Add(columnNames[i], newColumn);
                frame.columns.Add(newColumn);

                frame.ExpandNewColumn(newColumn);
            }

            return frame;
        }

        public static DataFrame LoadFromBinaryFile(string path)
        {
            return LoadFromBinaryFile(path, Encoding.UTF8);
        }

        public static DataFrame LoadFromBinaryFile(string path, Encoding encoding)
        {
            var reader = new DataFrameReader(path, encoding);
            return reader.LoadData();
        }

        public void SaveToBinaryFile(string path)
        {
            this.SaveToBinaryFile(path, Encoding.UTF8);
        }

        public void SaveToBinaryFile(string path, Encoding encoding)
        {
            var writer = new DataFrameWriter(this, path, encoding);
            writer.SaveData();
        }

        public DataFrame Subset(Predicate<DataFrameRow> filter)
        {
            DataFrame newFrame = this.Clone();

            for (int i = 0; i < this.rowCount; i++)
            {
                DataFrameRow row = this.rows[i];
                if (filter(row))
                {
                    newFrame.ImportRowUnchecked(row);
                }
            }
            return newFrame;
        }

        public DataFrame Clone()
        {
            DataFrame newFrame = new DataFrame();

            foreach (Series column in columns)
            {
                newFrame.AddColumn(column.Name, column.type);
            }

            return newFrame;
        }

        public int IndexOfKey(object key)
        {
            if (this.primaryKey.IsEmpty)
            {
                throw new Exception("DataFrame does not have a primary key.");
            }
            else
            {
                return this.primaryKey.IndexOf(key);
            }
        }

        public int IndexOfColumn(string columnName)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                if (string.Equals(columns[i].Name, columnName))
                {
                    return i;
                }
            }
            return (-1);
        }


        public void Clear()
        {
            for (int i = 0; i < this.columns.Count; i++)
            {
                this.columns[i].Clear();
            }
            this.rowCount = 0;
        }

        public void TrimToSize()
        {
            for (int i = 0; i < this.columns.Count; i++)
            {
                this.columns[i].TrimToSize();
            }
        }

        public T[] MapToClass<T>(Func<DataFrameRow,T> converter)
        {
            T[] array = new T[this.rowCount];
            for (int i = 0; i < this.rowCount; i++)
            {
                array[i] = converter(this.rows[i]);
            }
            return array;
        }
    }
}

