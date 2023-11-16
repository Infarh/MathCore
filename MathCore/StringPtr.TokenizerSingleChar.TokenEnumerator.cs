#nullable enable
namespace MathCore;

public readonly ref partial struct StringPtr
{
    public readonly ref partial struct TokenizerSingleChar
    {
        /// <summary>Перечислитель строковых фрагментов</summary>
        public ref struct TokenEnumerator
        {
            /// <summary>Строковый буфер</summary>
            private readonly string _Buffer;

            /// <summary>Символ-разделитель</summary>
            private readonly char _Separator;

            /// <summary>Начальное положение в буфере</summary>
            private readonly int _StartIndex;

            /// <summary>Длина подстроки для анализа</summary>
            private readonly int _Length;

            /// <summary>Пропускать пустые фрагменты</summary>
            private readonly bool _SkipEmpty;

            /// <summary>Текущая позиция в исходной строке</summary>
            private int _CurrentPos;

            /// <summary>Инициализация нового перечислителя строковых фрагментов</summary>
            /// <param name="Buffer">Исходный строковый буфер</param>
            /// <param name="Separator">Символ-разделитель</param>
            /// <param name="StartIndex">Начальное положение в строковом буфере</param>
            /// <param name="Length">Длина подстроки для анализа</param>
            /// <param name="SkipEmpty">Пропускать пустые фрагменты</param>
            public TokenEnumerator(string Buffer, char Separator, int StartIndex, int Length, bool SkipEmpty)
            {
                _Buffer     = Buffer;
                _Separator  = Separator;
                _StartIndex = StartIndex;
                _CurrentPos = StartIndex;
                _Length     = Length;
                _SkipEmpty  = SkipEmpty;

                Current = default;
            }

            /// <summary>Текущий фрагмент строки</summary>
            public StringPtr Current { get; private set; }

            /// <summary>Перемещение к следующему фрагменту</summary>
            /// <returns>Истина, если перемещение выполнено успешно</returns>
            public bool MoveNext()
            {
                switch (_Length - (_CurrentPos - _StartIndex))
                {
                    case < 0:               return false;
                    case 0 when _SkipEmpty: return false;
                    case 0:
                        Current = new(_Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return true;
                }

                var str     = _Buffer;
                var pos     = _CurrentPos;
                var end_pos = _StartIndex + _Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, _Separator, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current     = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && _SkipEmpty);

                Current     = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return true;
            }

            /// <summary>Переместиться к следующему фрагменту, либо сгенерировать исключение в случае отсутствия такой возможности</summary>
            /// <returns>Следующий фрагмент строки</returns>
            /// <exception cref="InvalidOperationException">Возникает в случае отсутствия возможности выделить следующий фрагмент строки</exception>
            public StringPtr MoveNextOrThrow() => MoveNext()
                ? Current
                : throw new InvalidOperationException($"Невозможно получить следующий фрагмент строки после разделителя {_Separator}");

            /// <summary>Переместиться к следующему фрагменту, либо сгенерировать исключение в случае отсутствия такой возможности</summary>
            /// <typeparam name="TException">Генерируемое исключение в случае отсутствия возможности перемещения к следующей подстроке</typeparam>
            /// <returns>Следующий фрагмент строки</returns>
            public StringPtr MoveNextOrThrow<TException>() where TException : Exception, new() => MoveNext()
                ? Current
                : throw new TException();

            /// <summary>Найти следующую подстроку</summary>
            /// <param name="Str">Исходный строковый буфер</param>
            /// <param name="Separator">Символ-разделитель</param>
            /// <param name="StartIndex">Индекс символа, с которого начинается поиск</param>
            /// <param name="EndIndex">Индекс символа, на котором должен закончиться поиск</param>
            /// <returns>Найденная подстрока</returns>
            private static StringPtr GetNext(string Str, char Separator, int StartIndex, int EndIndex)
            {
                if (StartIndex >= EndIndex) return new(Str, EndIndex, 0);

                var index = NextIndex(Str, Separator, StartIndex, EndIndex);
                return index < 0
                    ? new(Str, StartIndex, EndIndex - StartIndex)
                    : new(Str, StartIndex, index - StartIndex);
            }

            /// <summary>Индекс следующего разделителя в строке в заданном диапазоне</summary>
            /// <param name="Str">Исходный строковый буфер</param>
            /// <param name="Separator">Символ-разделитель</param>
            /// <param name="StartIndex">Индекс символа, с которого начинается поиск</param>
            /// <param name="EndIndex">Индекс символа, на котором должен закончиться поиск</param>
            /// <returns>Индекс искомого символа в подстроке, либо -1, если его найдено не было</returns>
            private static int NextIndex(string Str, char Separator, int StartIndex, int EndIndex)
            {
                var str_length = Str.Length;
                for (var i = StartIndex; i < EndIndex && i < str_length; i++)
                    if (Separator == Str[i])
                        return i;

                return -1;
            }

            /// <summary>Попытаться преобразовать следующую подстроку в вещественное число</summary>
            /// <param name="value">Результат преобразования, либо <see cref="double.NaN"/>, если подстрока имеет неверный формат, лио отсутствует</param>
            /// <returns>Истина, если преобразование подстроки в вещественное число выполнено успешно</returns>
            public bool TryParseNextDouble(out double value)
            {
                value = double.NaN;
                switch (_Length - (_CurrentPos - _StartIndex))
                {
                    case < 0:               return false;
                    case 0 when _SkipEmpty: return false;
                    case 0:
                        Current = new(_Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return Current.TryParseDouble(out value);
                }

                var str     = _Buffer;
                var pos     = _CurrentPos;
                var end_pos = _StartIndex + _Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, _Separator, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current     = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && _SkipEmpty);

                Current     = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return ptr.TryParseDouble(out value);
            }

            public double? ParseNextDouble() => TryParseNextDouble(out var v) ? v : null;

            public double ParseNextDoubleOrThrow() => TryParseNextDouble(out var v)
                ? v
                : throw new FormatException("Строка имела неверный формат");

            /// <summary>Попытаться преобразовать следующую подстроку в вещественное число</summary>
            /// <param name="provider">Формат представления вещественного числа</param>
            /// <param name="value">Результат преобразования, либо <see cref="double.NaN"/>, если подстрока имеет неверный формат, лио отсутствует</param>
            /// <returns>Истина, если преобразование подстроки в вещественное число выполнено успешно</returns>
            public bool TryParseNextDouble(IFormatProvider provider, out double value)
            {
                value = double.NaN;
                switch (_Length - (_CurrentPos - _StartIndex))
                {
                    case < 0:               return false;
                    case 0 when _SkipEmpty: return false;
                    case 0:
                        Current = new(_Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return Current.TryParseDouble(provider, out value);
                }

                var str     = _Buffer;
                var pos     = _CurrentPos;
                var end_pos = _StartIndex + _Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, _Separator, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current     = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && _SkipEmpty);

                Current     = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return ptr.TryParseDouble(provider, out value);
            }

            public double? ParseNextDouble(IFormatProvider Provider) => TryParseNextDouble(Provider, out var v) ? v : null;

            public double ParseNextDoubleOrThrow(IFormatProvider Provider) => TryParseNextDouble(Provider, out var v)
                ? v
                : throw new FormatException("Строка имела неверный формат");

            /// <summary>Попытаться преобразовать следующую подстроку в целое число</summary>
            /// <param name="value">Результат преобразования, либо 0, если подстрока имеет неверный формат, лио отсутствует</param>
            /// <returns>Истина, если преобразование подстроки в целое число выполнено успешно</returns>
            public bool TryParseNextInt32(out int value)
            {
                value = 0;
                switch (_Length - (_CurrentPos - _StartIndex))
                {
                    case < 0:               return false;
                    case 0 when _SkipEmpty: return false;
                    case 0:
                        Current = new(_Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return Current.TryParseInt32(out value);
                }

                var str     = _Buffer;
                var pos     = _CurrentPos;
                var end_pos = _StartIndex + _Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, _Separator, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current     = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && _SkipEmpty);

                Current     = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return ptr.TryParseInt32(out value);
            }

            /// <summary>Попытаться преобразовать следующую подстроку в <see cref="bool"/> значение</summary>
            /// <param name="value">Результат преобразования, либо <c>false</c>, если подстрока имеет неверный формат, лио отсутствует</param>
            /// <returns>Истина, если преобразование подстроки в <see cref="bool"/> значение выполнено успешно</returns>
            public bool TryParseNextAsBool(out bool value)
            {
                value = default;
                if (!MoveNext()) return false;

                if (Current.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    value = true;
                    return true;
                }

                if (Current.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    value = false;
                    return true;
                }

                return false;
            }

            /// <summary>Оператор неявного преобразования перечислителя фрагментов строки в целое число</summary>
            /// <param name="Enumerator">Перечислитель фрагментов строки</param>
            public static implicit operator int(TokenEnumerator Enumerator) => Enumerator.Current.ParseInt32();

            /// <summary>Оператор неявного преобразования перечислителя фрагментов строки в вещественное число</summary>
            /// <param name="Enumerator">Перечислитель фрагментов строки</param>
            public static implicit operator double(TokenEnumerator Enumerator) => Enumerator.Current.ParseDouble();
        }
    }
}