﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;

namespace Statistics.DataContainers
{
    public class DataFrame
    {
        Dictionary<string,Series> columnFromName;
        List<Series> columns;
        int rowCount;

        public int RowCount { get { return this.rowCount; } }

        public int ColumnCount { get { return this.columns.Count; } }

        Series primaryKey;

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
                    if (object.Equals(this.primaryKey, null))
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

        public DataFrame()
        {
            this.columnFromName = new Dictionary<string, Series>();
            this.columns = new List<Series>();
            this.rowCount = 0;
        }

        public DataFrame(int rowCount)
        {
            this.columnFromName = new Dictionary<string, Series>();
            this.columns = new List<Series>();
            this.rowCount = rowCount;
        }

        public ReadOnlyCollection<Series> Columns
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
                Series newColumn = new Series(columnName, type, this.rowCount);
                columnFromName.Add(columnName, newColumn);
                columns.Add(newColumn);
            }
        }

        public void AddColumn(Series column)
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

        public DataFrame this [Series booleanSeries]
        {
            get
            {
                if (this.rowCount == booleanSeries.Count && booleanSeries.type == typeof(bool))
                {
                    DataFrame newFrame = new DataFrame();
                    foreach (Series column in columns)
                    {
                        newFrame.AddColumn(column.ColumnName, column.type);
                    }
                    for (int i = 0; i < this.rowCount; i++)
                    {
                        DataRow row = this.GetRow(i);
                        if ((bool)booleanSeries[i])
                        {
                            newFrame.AddRow(row.ItemArray);
                        }
                    }
                    return newFrame;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("booleanSeries");
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
                Series newColumn = new Series(columnNames[i], types[i], rowCapacity);
                frame.columnFromName.Add(columnNames[i], newColumn);
                frame.columns.Add(newColumn);
                for (int row = 0; row < rowCapacity; row++)
                {
                    newColumn.Add(null);
                }
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

        /*  public object Clone()
        {
            var newDataFrame = new DataFrame();
            foreach (var column in this.columns)
            {
                newDataFrame.AddColumn((Series)column.Clone());
            }
            return newDataFrame;
        }*/

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

        public DataFrame Subset(Predicate<DataRow> filter)
        {
            DataFrame newFrame = new DataFrame();
            foreach (Series column in columns)
            {
                newFrame.AddColumn(column.ColumnName, column.type);
            }
            for (int i = 0; i < this.rowCount; i++)
            {
                DataRow row = this.GetRow(i);
                if (filter(row))
                {
                    newFrame.AddRow(row.ItemArray);
                }
            }
            return newFrame;
        }

        public int IndexOfKey(object key)
        {
            if (object.Equals(this.primaryKey, null))
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
                if (string.Equals(columns[i].ColumnName, columnName))
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

        public T[] MapToClass<T>(Func<DataRow,T> converter)
        {
            T[] array = new T[this.rowCount];
            for (int i = 0; i < this.rowCount; i++)
            {
                array[i] = converter(this.GetRow(i));
            }
            return array;
        }
    }
}

