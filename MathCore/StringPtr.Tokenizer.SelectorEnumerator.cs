using System.Runtime.InteropServices;

namespace MathCore;

public readonly ref partial struct StringPtr
{
    public readonly ref partial struct Tokenizer
    {
        /// <summary>Создать перечислитель с преобразованием фрагментов</summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <param name="selector">Функция преобразования фрагмента</param>
        /// <returns>Перечислитель с преобразованием</returns>
        /// <example>
        /// <![CDATA[
        /// var values = new StringPtr("1,2").Split(',').Select(p => p.ParseInt32());
        /// ]]>
        /// </example>
        public TokenizerSelector<T> Select<T>(Selector<T> selector) => new(_Buffer, Separators, StartIndex, _Length, _SkipEmpty, selector);
    }

    /// <summary>Перечислитель фрагментов с преобразованием в заданный тип</summary>
    /// <typeparam name="T">Целевой тип</typeparam>
    /// <param name="Buffer">Исходный строковый буфер</param>
    /// <param name="Separators">Символы-разделители</param>
    /// <param name="StartIndex">Индекс начала</param>
    /// <param name="Length">Длина анализируемой подстроки</param>
    /// <param name="SkipEmpty">Пропускать пустые фрагменты</param>
    /// <param name="selector">Функция преобразования фрагмента</param>
    /// <example>
    /// <![CDATA[
    /// var values = new StringPtr("1,2").Split(',').Select(p => p.ParseInt32());
    /// ]]>
    /// </example>
    [StructLayout(LayoutKind.Auto)]
    public readonly ref struct TokenizerSelector<T>(string Buffer, char[] Separators, int StartIndex, int Length, bool SkipEmpty, Selector<T> selector)
    {
        /// <summary>Получить перечислитель</summary>
        /// <returns>Перечислитель с преобразованием</returns>
        /// <example>
        /// <![CDATA[
        /// var enumerator = new StringPtr("1,2").Split(',').Select(p => p.ParseInt32()).GetEnumerator();
        /// ]]>
        /// </example>
        public SelectorEnumerator GetEnumerator() => new(Buffer, Separators, StartIndex, Length, SkipEmpty, selector);

        /// <summary>Перечислитель с преобразованием фрагментов</summary>
        /// <param name="Buffer">Исходный строковый буфер</param>
        /// <param name="Separators">Символы-разделители</param>
        /// <param name="StartIndex">Индекс начала</param>
        /// <param name="Length">Длина анализируемой подстроки</param>
        /// <param name="SkipEmpty">Пропускать пустые фрагменты</param>
        /// <param name="selector">Функция преобразования фрагмента</param>
        /// <example>
        /// <![CDATA[
        /// var enumerator = new StringPtr("1,2").Split(',').Select(p => p.ParseInt32()).GetEnumerator();
        /// ]]>
        /// </example>
        public ref struct SelectorEnumerator(string Buffer, char[] Separators, int StartIndex, int Length, bool SkipEmpty, Selector<T> selector)
        {
            private readonly int _StartIndex = StartIndex;

            /// <summary>Текущая позиция в исходной строке</summary>
            private int _CurrentPos = StartIndex;

            /// <summary>Текущее значение</summary>
            public T? Current { get; private set; }

            /// <summary>Переместиться к следующему значению</summary>
            /// <returns>Истина, если следующее значение найдено</returns>
            /// <example>
            /// <![CDATA[
            /// var e = new StringPtr("1,2").Split(',').Select(p => p.ParseInt32()).GetEnumerator();
            /// while (e.MoveNext())
            /// {
            ///     Console.WriteLine(e.Current);
            /// }
            /// ]]>
            /// </example>
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

                var str = Buffer;
                var pos = _CurrentPos;
                var end_pos = _StartIndex + Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, Separators, pos, end_pos); // Получаем следующий фрагмент
                    if (ptr.Pos == end_pos)
                    {
                        Current = selector(ptr);
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && SkipEmpty);

                Current = selector(ptr);
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return true;

                static StringPtr GetNext(string Str, char[] Separators, int StartIndex, int EndIndex)
                {
                    if (StartIndex >= EndIndex) return new(Str, EndIndex, 0);

                    var index = NextIndex(Str, Separators, StartIndex, EndIndex);
                    return index < 0
                        ? new(Str, StartIndex, EndIndex - StartIndex)
                        : new(Str, StartIndex, index - StartIndex);

                    static int NextIndex(string Str, char[] Separators, int StartIndex, int EndIndex)
                    {
                        var str_length = Str.Length;
                        var separators_length = Separators.Length;
                        for (var i = StartIndex; i < EndIndex && i < str_length; i++)
                        {
                            var c = Str[i];
                            for (var j = 0; j < separators_length; j++)
                                if (Separators[j] == c)
                                    return i;
                        }

                        return -1;
                    }
                }
            }
        }
    }
}
