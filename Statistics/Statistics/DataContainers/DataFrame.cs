using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;

namespace Statistics.DataContainers
{
    public class DataFrame
    {
        Dictionary<string,DataColumn> columnFromName;
        List<DataColumn> columns;
        int rowCount;

        public int RowCount { get { return this.rowCount; } }

        public int ColumnCount { get { return this.columns.Count; } }

        public DataFrame()
        {
            this.columnFromName = new Dictionary<string, DataColumn>();
            this.columns = new List<DataColumn>();
            this.rowCount = 0;
        }

        public DataFrame(int rowCount)
        {
            this.columnFromName = new Dictionary<string, DataColumn>();
            this.columns = new List<DataColumn>();
            this.rowCount = rowCount;
        }

        /*        public DataFrame(IEnumerable<DataRow> rows)
        {
            using (var iterator = rows.GetEnumerator())
            {
                if (iterator.MoveNext())
                {
                }
            }
        }*/

        public ReadOnlyCollection<DataColumn> Columns
        {
            get
            {
                return this.columns.AsReadOnly();
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

        public void AddColumn(string columnName, Type type)
        {
            if (columnFromName.ContainsKey(columnName))
            {
                string message = string.Format("Column {0} has already existed.", columnName);
                throw new ArgumentException(message, "columnName");
            }
            else
            {
                DataColumn newColumn = new DataColumn(this, columnName, type, this.rowCount);
                columnFromName.Add(columnName, newColumn);
                columns.Add(newColumn);
            }
        }

        public void AddColumn(DataColumn column)
        {
            if (this.columns.Count == 0)
            {
                this.columns.Add(column);
                this.columnFromName.Add(column.ColumnName, column);
                this.rowCount = this.columns.Count;
            }
            else
            {
                if (column.Count == this.rowCount)
                {
                    if (columnFromName.ContainsKey(column.ColumnName))
                    {
                        string message = string.Format("Column {0} has already existed.", column.ColumnName);
                        throw new ArgumentException(message, "columnName");
                    }
                    else
                    {
                        this.columns.Add(column);
                        this.columnFromName.Add(column.ColumnName, column);
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("column");
                }
            }
        }

        private void ExpandNewColumn(DataColumn newColumn)
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
            }
            else
            {
                throw new ArgumentOutOfRangeException("row");
            }
        }

        public void RemoveColumn(string columnName)
        {
            DataColumn column;
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

        public DataColumn this [string columnName]
        {
            get { return this.columnFromName[columnName]; }
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

                builder.Append(this.columns[0].ColumnName);
                for (int i = 1; i < ColumnCount; i++)
                {
                    builder.Append(',');
                    builder.Append(this.columns[i].ColumnName);
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
                DataColumn newColumn = new DataColumn(frame, columnNames[i], types[i], rowCapacity);
                frame.columnFromName.Add(columnNames[i], newColumn);
                frame.columns.Add(newColumn);
                for (int row = 0; row < rowCapacity; row++)
                {
                    newColumn.Add(null);
                }
            }

            return frame;
        }

        public static DataFrame LoadFromFile(string path, Encoding encoding)
        {
            var reader = new DataFrameReader(path, encoding);
            return reader.LoadData();
        }

        public void SaveToFile(string path, Encoding encoding)
        {
            var writer = new DataFrameWriter(this, path, encoding);
            writer.SaveData();
        }

        public object Clone()
        {
            var newDataFrame = new DataFrame();
            foreach (var column in this.columns)
            {
                newDataFrame.AddColumn((DataColumn)column.Clone());
            }
            return newDataFrame;
        }

        public DataRow GetRow(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < this.rowCount)
            {
                return new DataRow(this, rowIndex);
            }
            else
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }
        }
    }
}

