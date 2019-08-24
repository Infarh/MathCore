using System.Diagnostics.Contracts;

namespace System
{
    public static class DateTimeExtensions
    {
        public static byte[] ToBitArray(this DateTime Time) => BitConverter.GetBytes(Time.ToBinary());

        public static DateTime FromBinary(byte[] Data, int Offset)
        {
            Contract.Requires(Data != null);
            Contract.Requires(Offset >= 0);
            Contract.Requires(Offset <= Data.Length - 8);
            return DateTime.FromBinary(BitConverter.ToInt64(Data, Offset));
        }
    }
}
