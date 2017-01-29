using System;
using System.IO;
using System.Text;

namespace Statistics.DataContainers
{
    internal class DataFrameReader:IDisposable
    {
        Stream stream;
        BinaryReader reader;
        DataFrame frame;

        internal DataFrameReader(string path, Encoding encoding)
        {
            this.stream = File.OpenRead(path);
            this.reader = new BinaryReader(stream, encoding);
        }

        void ReadInfo()
        {
            int nRows = this.reader.ReadInt32();
            int nCols = this.reader.ReadInt32();
            string[] columnNames = new string[nCols];
            Type[] types = new Type[nCols];
            for (int i = 0; i < nCols; i++)
            {
                columnNames[i] = this.reader.ReadString();
            }
            for (int i = 0; i < nCols; i++)
            {
                types[i] = Type.GetType(this.reader.ReadString());
            }
            this.frame = DataFrame.CreatEmptyFrame(columnNames, types, nRows);
        }

        void ReadAll()
        {
            for (int col = 0; col < frame.ColumnCount; col++)
            {
                DataColumn column = this.frame.Columns[col];
                Func<object> converter = GetConverter(column.type);
                for (int row = 0; row < frame.RowCount; row++)
                {
                    if (ReadType())
                    {
                        column.Add(converter());
                    }
                    else
                    {
                        column.Add(null);
                    }
                }
            }
        }

        bool ReadType()
        {
            DataType type = (DataType)this.reader.ReadByte();
            return type != DataType.Null;
        }

        Func<object> GetConverter(Type type)
        {
            switch (type.Name)
            {
                case "Int32":
                    return () => this.reader.ReadInt32();
                case "Int64":
                    return () => this.reader.ReadInt64();
                case "Boolean":
                    return () => this.reader.ReadBoolean();
                case "Single":
                    return () => this.reader.ReadSingle();
                case "Double":
                    return () => this.reader.ReadDouble();
                case "Char":
                    return () => this.reader.ReadChar();
                case "String":
                    return () => this.reader.ReadString();
                default:
                    return () => Convert.ChangeType(this.reader.ReadString(), type);
            }
        }

        public void Dispose()
        {
            if (this.stream != null)
            {
                this.stream.Dispose();
            }
            if (this.reader != null)
            {
                this.reader.Dispose();
            }
        }
    }
}

