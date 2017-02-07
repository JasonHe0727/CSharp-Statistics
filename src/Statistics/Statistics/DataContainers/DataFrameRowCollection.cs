using System;

namespace Statistics.DataContainers
{
    public class DataFrameRowCollection
    {
        readonly DataFrame frame;

        public DataFrame Frame { get { return this.frame; } }

        internal DataFrameRowCollection(DataFrame frame)
        {
            this.frame = frame;
        }

        public DataFrameRow this [int rowIndex]
        {
            get
            {
                if (rowIndex >= 0 && rowIndex < this.frame.RowCount)
                {
                    return new DataFrameRow(this.frame, rowIndex);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("rowIndex");
                }
            }
        }

        public void Add(params object[] values)
        {
            this.frame.AddRow(values);
        }

    }
}

