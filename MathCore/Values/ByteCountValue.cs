#nullable enable
namespace MathCore.Values;

[Serializable]
public struct ByteCountValue(double Value, ByteDimensionEnum Dimension)
{
    public ByteCountValue(double Value) : this(Value, default) { }

    public static ByteDimensionEnum GetDimension(double Value) => (ByteDimensionEnum)(Math.Log(Value, 2) / 10);

    public double Count { get; set; } = Value;

    public ByteDimensionEnum Dimension { get; set; } = Dimension;
}

[Serializable]
public enum ByteDimensionEnum : byte { B = 0, kB = 1, MByte = 2, GB }