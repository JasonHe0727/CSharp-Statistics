using System;
using System.IO;
using System.Text;

namespace Statistics.DataContainers
{
    internal class DataFrameReader:IDisposable
    {
        Stream stream;
        BinaryReader reader;

        internal DataFrameReader()
        {
        }

        void Read()
        {
            
        }

        void Dispose()
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

