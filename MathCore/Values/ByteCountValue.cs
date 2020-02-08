using System;

namespace MathCore.Values
{
    [Serializable]
    public struct ByteCountValue
    {
        public static ByteDimensionEnum GetDimension(double Value) => (ByteDimensionEnum)(Math.Log(Value, 2) / 10);

        public double Count { get; set; }
        public ByteDimensionEnum Dimension { get; set; }


        public ByteCountValue(double Value) : this() => Count = Value;

        public ByteCountValue(double Value, ByteDimensionEnum Dimension)
            : this()
        {
            Count = Value;
            this.Dimension = Dimension;
        }
    }

    [Serializable]
    public enum ByteDimensionEnum : byte { B = 0, kB = 1, MByte = 2, GB }
}