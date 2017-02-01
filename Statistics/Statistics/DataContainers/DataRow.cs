using System;

namespace Statistics.DataContainers
{
    public class DataRow
    {
        readonly DataFrame frame;
        readonly int rowIndex;

        public DataFrame Frame { get { return this.frame; } }

        public int RowIndex { get { return this.rowIndex; } }

        public DataRow(DataFrame frame, int rowIndex)
        {
            this.frame = frame;
            this.rowIndex = rowIndex;
        }

        public object this [int columnIndex]
        {
            get { return this.frame[this.rowIndex, columnIndex]; }
            set { this.frame[this.rowIndex, columnIndex] = value; }
        }

        public object this [string columnName]
        {
            get { return this.frame[columnName][this.rowIndex]; }
            set { this.frame[columnName][this.rowIndex] = value; }
        }

    }
}

