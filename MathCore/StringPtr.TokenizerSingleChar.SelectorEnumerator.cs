namespace MathCore;

public readonly ref partial struct StringPtr
{
    public readonly ref partial struct TokenizerSingleChar
    {
        public TokenizerSingleCharSelector<T> Select<T>(Selector<T> selector) => new(Buffer, Separator, StartIndex, Length, _SkipEmptyElements, selector);
    }

    public readonly ref struct TokenizerSingleCharSelector<T>(
        string Buffer,
        char Separator,
        int StartIndex,
        int Length,
        bool SkipEmpty,
        Selector<T> Selector)
    {
        public SelectorEnumerator GetEnumerator() => new(Buffer, Separator, StartIndex, Length, SkipEmpty, Selector);

        public ref struct SelectorEnumerator(string Buffer, char Separator, int StartIndex, int Length, bool SkipEmpty, Selector<T> Selector)
        {
            private readonly int _StartIndex = StartIndex;

            /// <summary>Текущая позиция в исходной строке</summary>
            private int _CurrentPos = StartIndex;

            public T Current { get; private set; }

            public bool MoveNext()
            {
                switch (Length - (_CurrentPos - _StartIndex))
                {
                    case < 0: return false;
                    case 0 when SkipEmpty: return false;
                    case 0:
                        Current = default;
                        _CurrentPos++;
                        return true;
                }

                var pos = _CurrentPos;
                var end_pos = _StartIndex + Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(Buffer, Separator, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current = Selector(ptr);
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && SkipEmpty);

                Current = Selector(ptr);
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return true;

                static StringPtr GetNext(string Str, char Separator, int StartIndex, int EndIndex)
                {
                    if (StartIndex >= EndIndex) return new(Str, EndIndex, 0);

                    var index = NextIndex(Str, Separator, StartIndex, EndIndex);
                    return index < 0
                        ? new(Str, StartIndex, EndIndex - StartIndex)
                        : new(Str, StartIndex, index - StartIndex);

                    static int NextIndex(string Str, char Separator, int StartIndex, int EndIndex)
                    {
                        var str_length = Str.Length;
                        for (var i = StartIndex; i < EndIndex && i < str_length; i++)
                            if (Separator == Str[i])
                                return i;

                        return -1;
                    }
                }
            }
        }
    }
}
