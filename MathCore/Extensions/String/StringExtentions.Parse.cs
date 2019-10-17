using System.Globalization;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System
{
    public static class StringExtentionsParse
    {
        [DST]
        public static bool IsInt8(this string s) => byte.TryParse(s, out var i);

        [DST]
        public static bool IsInt16(this string s) => short.TryParse(s, out var i);

        [DST]
        public static bool IsInt32(this string s) => int.TryParse(s, out var i);

        [DST]
        public static bool IsInt64(this string s) => long.TryParse(s, out var i);

        [DST]
        public static bool IsShort(this string s) => short.TryParse(s, out var i);

        [DST]
        public static bool IsDouble(this string s) => double.TryParse(s, out var i);

        [DST]
        public static byte? AsInt8(this string s, byte? Default = null) => byte.TryParse(s, out var i) ? i : Default;

        [DST]
        public static short? AsInt16(this string s, short? Default = null) => short.TryParse(s, out var i) ? i : Default;

        [DST]
        public static int? AsInt32(this string s, int? Default = null) => int.TryParse(s, out var i) ? i : Default;

        [DST]
        public static long? AsInt64(this string s, long? Default = null) => long.TryParse(s, out var i) ? i : Default;

        [DST]
        public static float? AsSingle(this string s, float? Default = null) => float.TryParse(s, out var i) ? i : Default;

        [DST]
        public static double? AsDouble(this string s, double? Default = null)
        {
            var FormatInfo = NumberFormatInfo.CurrentInfo;

            s = s.Replace('.', FormatInfo.PercentDecimalSeparator[0]);

            var result = double.TryParse(s, out var i);

            return result ? i : Default;
        }

        [DST]
        public static string RemoveFromBeginEnd(this string S, int BeginCount, int EndCount) => S.Remove(0, BeginCount).RemoveFromEnd(EndCount);

        [DST]
        public static string RemoveFromEnd(this string S, int count) => S.Length <= count ? "" : S.Remove(S.Length - count, count);

        [DST]
        public static string RemoveFromEnd(this string S, int StartPos, int count) => S.Remove(S.Length - StartPos, count);
    }
}