using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class DateTimeExtensions
    {
        [NotNull] public static byte[] ToBitArray(this DateTime Time) => BitConverter.GetBytes(Time.ToBinary());

        public static DateTime FromBinary(byte[] Data, int Offset) => DateTime.FromBinary(BitConverter.ToInt64(Data, Offset));
    }
}