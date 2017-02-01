using System;
using System.IO;
using System.Text;

namespace Statistics.DataContainers
{
    internal class DataFrameWriter:IDisposable
    {
        DataFrame frame;
        readonly Stream stream;
        readonly BinaryWriter writer;

        internal DataFrameWriter(DataFrame frame, string path, Encoding encoding)
        {
            this.frame = frame;
            this.stream = File.OpenWrite(path);
            this.writer = new BinaryWriter(stream, encoding);
        }

        internal void SaveData()
        {
            WriteInfo();
            WriteAll();
        }

        void WriteInfo()
        {
            this.writer.Write(frame.RowCount);
            this.writer.Write(frame.ColumnCount);

            for (int i = 0; i < frame.ColumnCount; i++)
            {
                this.writer.Write(this.frame.Columns[i].ColumnName);
            }

            for (int i = 0; i < frame.ColumnCount; i++)
            {
                this.writer.Write(this.frame.Columns[i].type.FullName);
            }
        }

        void WriteAll()
        {
            Type[] types = new Type[frame.ColumnCount];
            for (int col = 0; col < frame.ColumnCount; col++)
            {
                types[col] = frame.Columns[col].type;
            }

            Action<object>[] converters = new Action<object>[frame.ColumnCount];

            for (int col = 0; col < frame.ColumnCount; col++)
            {
                converters[col] = GetConverter(types[col]);
            }

            for (int row = 0; row < frame.RowCount; row++)
            {
                for (int col = 0; col < frame.ColumnCount; col++)
                {
                    object value = frame[row, col];
                    if (WriteFlag(value))
                    {
                        converters[col](value);
                    }
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
            if (this.writer != null)
            {
                this.writer.Dispose();
            }
        }


    }

    internal enum DataType:byte
    {
        Null = 0,
        NotNull = 1,
    }
}

