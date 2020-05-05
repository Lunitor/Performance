using System;

namespace Lunitor.DataReader.Models
{
    class Data : IComparable<Data>
    {
        public DateTime TimeStamp { get; private set; }
        public double Value { get; private set; }

        public Data(DateTime timeStamp, double value)
        {
            TimeStamp = timeStamp;
            Value = value;
        }

        public int CompareTo(Data other)
        {
            return TimeStamp.CompareTo(other.TimeStamp);
        }
    }
}
