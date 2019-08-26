using System;

namespace MathCore.Values
{
    [Serializable]
    public struct ByteCountValue
    {
        public static ByteDymynsionEnum GetDemention(double Value) => (ByteDymynsionEnum)(Math.Log(Value, 2) / 10);

        public double Count { get; set; }
        public ByteDymynsionEnum Demension { get; set; }


        public ByteCountValue(double Value) : this() => Count = Value;

        public ByteCountValue(double Value, ByteDymynsionEnum Demension)
            : this()
        {
            Count = Value;
            this.Demension = Demension;
        }
    }

    [Serializable]
    public enum ByteDymynsionEnum : byte { B = 0, kB = 1, MByte = 2, GB }
}
