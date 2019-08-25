using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace System
{
    public static class CharExtentions
    {
        [DST] public static bool IsDigit(this char c) => char.IsDigit(c);

        private const int __IndexOf0 = (int)'0';
        [DST]
        public static int ToDigit(this char c)
        {
            if (!char.IsDigit(c)) throw new InvalidOperationException($"Символ \'{c}\' не является цифрой");
            return (int)c - __IndexOf0;
        }
    }
}
