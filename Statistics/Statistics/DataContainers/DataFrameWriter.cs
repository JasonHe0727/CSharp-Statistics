using System;
using System.IO;
using System.Text;

namespace Statistics.DataContainers
{
    internal class DataFrameWriter:IDisposable
    {
        DataFrame frame;
        Stream stream;
        BinaryWriter writer;

        internal DataFrameWriter(DataFrame frame, string path, Encoding encoding)
        {
            this.frame = frame;
            this.stream = File.OpenWrite(path);
            this.writer = new BinaryWriter(stream, encoding);
        }

        internal void WriteInfo()
        {
            this.writer.Write(frame.RowCount);
            this.writer.Write(frame.ColumnCount);
            for (int i = 0; i < frame.ColumnCount; i++)
            {
                this.writer.Write(this.frame.Columns[i].ColumnName);
            }
        }

        internal void WriteAll()
        {
            for (int col = 0; col < frame.ColumnCount; col++)
            {
                WriteColumn(frame.Columns[col]);
            }
        }

        void WriteColumn(DataColumn column)
        {
            Type type = column.type;
            Action<object> converter = GetConverter(type);
            for (int row = 0; row < column.Count; row++)
            {
                object value = column[row];
                if (WriteFlag(value))
                {
                    converter(value);
                }
            }
        }

        Action<object> GetConverter(Type type)
        {
            switch (type.Name)
            {
                case "Int32":
                    return i => this.writer.Write((int)i);
                case "Int64":
                    return i => this.writer.Write((long)i);
                case "Boolean":
                    return i => this.writer.Write((bool)i);
                case "Single":
                    return i => this.writer.Write((float)i);
                case "Double":
                    return i => this.writer.Write((double)i);
                case "Char":
                    return i => this.writer.Write((char)i);
                case "String":
                    return i => this.writer.Write((string)i);
                default:
                    return i => this.writer.Write(i.ToString());
            }
        }

        bool WriteFlag(object value)
        {
            if (value == null)
            {
                this.writer.Write((byte)DataType.Null);
                return false;
            }
            else
            {
                this.writer.Write((byte)DataType.NotNull);
                return true;
            }
        }

        public void Dispose()
        {
            if (this.stream != null)
            {
                this.stream.Dispose();
            }
            if (this.writer!= null)
            {
                this.writer.Dispose();
            }
        }

        private enum DataType:byte
        {
            Null = 0,
            NotNull = 1,
        }
    }
}

