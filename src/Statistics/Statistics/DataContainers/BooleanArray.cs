using System;

namespace Statistics.DataContainers
{
    public class BooleanArray
    {
        readonly bool[] array;

        internal BooleanArray(int size)
        {
            this.array = new bool[size];
        }

        public bool this [int index]
        {
            get
            {
                return this.array[index];
            }
            set
            {
                this.array[index] = value;
            }
        }

        public int Length
        {
            get
            {
                return this.array.Length;
            }
        }

        public static BooleanArray operator &(BooleanArray arrayA, BooleanArray arrayB)
        {
            if (arrayA.Length == arrayB.Length)
            {
                var result = new BooleanArray(arrayA.Length);
                for (int i = 0; i < result.array.Length; i++)
                {
                    result.array[i] = arrayA.array[i] & arrayB.array[i];
                }
                return result;
            }
            else
            {
                throw new ArgumentOutOfRangeException("arrayB");
            }
        }

        public static BooleanArray operator |(BooleanArray arrayA, BooleanArray arrayB)
        {
            if (arrayA.Length == arrayB.Length)
            {
                var result = new BooleanArray(arrayA.Length);
                for (int i = 0; i < result.array.Length; i++)
                {
                    result.array[i] = arrayA.array[i] | arrayB.array[i];
                }
                return result;
            }
            else
            {
                throw new ArgumentOutOfRangeException("arrayB");
            }
        }

        public static BooleanArray operator !(BooleanArray array)
        {
            var result = new BooleanArray(array.Length);
            for (int i = 0; i < result.array.Length; i++)
            {
                result.array[i] = !array.array[i];
            }
            return result;
        }

    }
}

