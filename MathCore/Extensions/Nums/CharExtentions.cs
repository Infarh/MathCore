
using System.Diagnostics;

namespace System
{
    public static class CharExtentions
    {
        [DebuggerStepThrough]
        public static bool IsDigit(this char c) => char.IsDigit(c);//            for(var i = 0; i < 10; i++)//                if(c == i.ToString()[0]) return true;//            return false;

        public static int ToDigit(this char c) => int.Parse(new string(c, 1));
    }
}
