using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

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
                        builder.Append(this.columns[col][row].ToString());
                    }
                    builder.AppendLine();
                }

                return builder.ToString();
            }
        }


        internal static DataFrame CreatEmptyFrame(string[] columnNames, Type[] types, int rowCapacity)
        {
            var frame = new DataFrame();
            for (int i = 0; i < columnNames.Length; i++)
            {
                DataColumn newColumn = new DataColumn(frame, columnNames[i], types[i], rowCapacity);
                frame.columnFromName.Add(columnNames[i], newColumn);
                frame.columns.Add(newColumn);
            }
            return frame;
        }

    }
}

