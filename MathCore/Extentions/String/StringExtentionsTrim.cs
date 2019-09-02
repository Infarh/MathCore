using System.Diagnostics.Contracts;

namespace System
{
    public static class StringExtentionsTrim
    {
        public static string TrimByLength(this string Str, int Length, string ReplacementPattern = "..")
        {
            Contract.Requires(Length >= 0);
            Contract.Ensures((Str is null && Contract.Result<string>() is null)
                || (Str != null && Contract.Result<string>() != null));

            if(Str is null) return null;
            if(Str.Length <= Length) return Str;
            if(Length == 0) return "";

            var dL1 = Str.Length - Length + ReplacementPattern.Length;
            var dL2 = dL1 / 2;
            dL1 = dL1 - dL2;

            var s1 = Str.Substring(0, Str.Length / 2 - dL1);
            var start = Str.Length/2 + dL2;
            var len = Str.Length - Str.Length / 2 - dL2;
            var s2 = Str.Substring(start, len);

            return string.Format("{0}{2}{1}", s1, s2, ReplacementPattern);
        }
    }
}