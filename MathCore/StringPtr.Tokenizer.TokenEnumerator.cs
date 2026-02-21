namespace MathCore;

public readonly ref partial struct StringPtr
{
    public readonly ref partial struct Tokenizer
    {
        /// <summary>Сформировать перечислитель строковых фрагментов</summary>
        /// <returns>Перечислитель строковых фрагментов</returns>
        /// <example>
        /// <![CDATA[
        /// var enumerator = new StringPtr("a,b").Split(',').GetEnumerator();
        /// ]]>
        /// </example>
        public TokenEnumerator GetEnumerator() => new(_Buffer, Separators, StartIndex, _Length, _SkipEmpty);

        /// <summary>Перечислитель строковых фрагментов</summary>
        /// <param name="Buffer">Исходный строковый буфер</param>
        /// <param name="Separators">Символы-разделители</param>
        /// <param name="StartIndex">Начальное положение в строковом буфере</param>
        /// <param name="Length">Длина подстроки для анализа</param>
        /// <param name="SkipEmpty">Пропускать пустые фрагменты</param>
        /// <example>
        /// <![CDATA[
        /// var enumerator = new StringPtr("a,b").Split(',').GetEnumerator();
        /// ]]>
        /// </example>
        public ref struct TokenEnumerator(string Buffer, char[] Separators, int StartIndex, int Length, bool SkipEmpty)
        {
            private readonly int _StartIndex = StartIndex;

            /// <summary>Текущая позиция в исходной строке</summary>
            private int _CurrentPos = StartIndex;

            /// <summary>Текущий фрагмент строки</summary>
            public StringPtr Current { get; private set; } = default;

            /// <summary>Перемещение к следующему фрагменту</summary>
            /// <returns>Истина, если перемещение выполнено успешно</returns>
            /// <example>
            /// <![CDATA[
            /// var enumerator = new StringPtr("a,b").Split(',').GetEnumerator();
            /// while (enumerator.MoveNext())
            /// {
            ///     Console.WriteLine(enumerator.Current);
            /// }
            /// ]]>
            /// </example>
            public bool MoveNext()
            {
                switch (Length - (_CurrentPos - _StartIndex))
                {
                    case < 0:              return false;
                    case 0 when SkipEmpty: return false;
                    case 0:
                        Current = new(Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return true;
                }

                var str     = Buffer;
                var pos     = _CurrentPos;
                var end_pos = _StartIndex + Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, Separators, pos, end_pos); // Находим следующий фрагмент
                    if (ptr.Pos == end_pos)
                    {
                        Current     = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && SkipEmpty);

                Current     = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return true;
            }

            /// <summary>Переместиться к следующему фрагменту, либо сгенерировать исключение</summary>
            /// <returns>Следующий фрагмент строки</returns>
            /// <exception cref="InvalidOperationException">Возникает при отсутствии следующего фрагмента</exception>
            /// <example>
            /// <![CDATA[
            /// var part = new StringPtr("a,b").Split(',').GetEnumerator().MoveNextOrThrow();
            /// ]]>
            /// </example>
            public StringPtr MoveNextOrThrow() => MoveNext()
                ? Current
                : throw new InvalidOperationException(
                    $"Невозможно получить следующий фрагмент строки после разделителя {string.Join(",", Separators.Select(c => $"'{c}'"))}");

            /// <summary>Переместиться к следующему фрагменту, либо сгенерировать исключение</summary>
            /// <typeparam name="TException">Генерируемое исключение</typeparam>
            /// <returns>Следующий фрагмент строки</returns>
            /// <example>
            /// <![CDATA[
            /// var part = new StringPtr("a,b").Split(',').GetEnumerator().MoveNextOrThrow<InvalidOperationException>();
            /// ]]>
            /// </example>
            public StringPtr MoveNextOrThrow<TException>() where TException : Exception, new() => MoveNext()
                ? Current
                : throw new TException();

            /// <summary>Найти следующую подстроку</summary>
            /// <param name="Str">Исходный строковый буфер</param>
            /// <param name="Separators">Символы-разделители</param>
            /// <param name="StartIndex">Индекс символа, с которого начинается поиск</param>
            /// <param name="EndIndex">Индекс символа, на котором должен закончиться поиск</param>
            /// <returns>Найденная подстрока</returns>
            private static StringPtr GetNext(string Str, char[] Separators, int StartIndex, int EndIndex)
            {
                if (StartIndex >= EndIndex) return new(Str, EndIndex, 0);

                var index = NextIndex(Str, Separators, StartIndex, EndIndex);
                return index < 0
                    ? new(Str, StartIndex, EndIndex - StartIndex)
                    : new(Str, StartIndex, index - StartIndex);
            }

            /// <summary>Индекс следующего разделителя в строке в заданном диапазоне</summary>
            /// <param name="Str">Исходный строковый буфер</param>
            /// <param name="Separators">Символы-разделители</param>
            /// <param name="StartIndex">Индекс символа, с которого начинается поиск</param>
            /// <param name="EndIndex">Индекс символа, на котором должен закончиться поиск</param>
            /// <returns>Индекс искомого символа в подстроке, либо -1, если его найдено не было</returns>
            private static int NextIndex(string Str, char[] Separators, int StartIndex, int EndIndex)
            {
                var str_length        = Str.Length;
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

            /// <summary>Попытаться преобразовать следующую подстроку в вещественное число</summary>
            /// <param name="value">Результат преобразования, либо <see cref="double.NaN"/></param>
            /// <returns>Истина, если преобразование выполнено успешно</returns>
            /// <example>
            /// <![CDATA[
            /// var enumerator = new StringPtr("1.1,2.2").Split(',').GetEnumerator();
            /// var ok = enumerator.TryParseNextDouble(out var value);
            /// ]]>
            /// </example>
            public bool TryParseNextDouble(out double value)
            {
                value = double.NaN;
                switch (Length - (_CurrentPos - _StartIndex))
                {
                    case < 0:               return false;
                    case 0 when SkipEmpty: return false;
                    case 0:
                        Current = new(Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return Current.TryParseDouble(out value);
                }

                var str     = Buffer;
                var pos     = _CurrentPos;
                var end_pos = _StartIndex + Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, Separators, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current     = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && SkipEmpty);

                Current     = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return ptr.TryParseDouble(out value);
            }

            /// <summary>Попытаться преобразовать следующую подстроку в вещественное число</summary>
            /// <param name="provider">Формат представления вещественного числа</param>
            /// <param name="value">Результат преобразования, либо <see cref="double.NaN"/></param>
            /// <returns>Истина, если преобразование выполнено успешно</returns>
            /// <example>
            /// <![CDATA[
            /// var enumerator = new StringPtr("1,1;2,2").Split(';').GetEnumerator();
            /// var ok = enumerator.TryParseNextDouble(CultureInfo.GetCultureInfo("ru-RU"), out var value);
            /// ]]>
            /// </example>
            public bool TryParseNextDouble(IFormatProvider provider, out double value)
            {
                value = double.NaN;
                switch (Length - (_CurrentPos - _StartIndex))
                {
                    case < 0:               return false;
                    case 0 when SkipEmpty: return false;
                    case 0:
                        Current = new(Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return Current.TryParseDouble(provider, out value);
                }

                var str     = Buffer;
                var pos     = _CurrentPos;
                var end_pos = _StartIndex + Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, Separators, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current     = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && SkipEmpty);

                Current     = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return ptr.TryParseDouble(provider, out value);
            }

            /// <summary>Попытаться преобразовать следующую подстроку в целое число</summary>
            /// <param name="value">Результат преобразования, либо 0</param>
            /// <returns>Истина, если преобразование выполнено успешно</returns>
            /// <example>
            /// <![CDATA[
            /// var enumerator = new StringPtr("1,2").Split(',').GetEnumerator();
            /// var ok = enumerator.TryParseNextAsInt32(out var value);
            /// ]]>
            /// </example>
            public bool TryParseNextAsInt32(out int value)
            {
                value = 0;
                switch (Length - (_CurrentPos - _StartIndex))
                {
                    case < 0:               return false;
                    case 0 when SkipEmpty: return false;
                    case 0:
                        Current = new(Buffer, _CurrentPos, 0);
                        _CurrentPos++;
                        return Current.TryParseInt32(out value);
                }

                var str     = Buffer;
                var pos     = _CurrentPos;
                var end_pos = _StartIndex + Length;

                StringPtr ptr;
                do
                {
                    ptr = GetNext(str, Separators, pos, end_pos);
                    if (ptr.Pos == end_pos)
                    {
                        Current     = ptr;
                        _CurrentPos = end_pos;
                        return true;
                    }

                    pos += Math.Max(1, ptr.Length);
                }
                while (ptr.Length == 0 && SkipEmpty);

                Current     = ptr;
                _CurrentPos = ptr.Pos + ptr.Length + 1;
                return ptr.TryParseInt32(out value);
            }

            /// <summary>Попытаться преобразовать следующую подстроку в <see cref="bool"/> значение</summary>
            /// <param name="value">Результат преобразования, либо <c>false</c></param>
            /// <returns>Истина, если преобразование выполнено успешно</returns>
            /// <example>
            /// <![CDATA[
            /// var enumerator = new StringPtr("true,false").Split(',').GetEnumerator();
            /// var ok = enumerator.TryParseNextAsBool(out var value);
            /// ]]>
            /// </example>
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

            /// <summary>Оператор неявного преобразования перечислителя в целое число</summary>
            /// <param name="Enumerator">Перечислитель фрагментов строки</param>
            /// <returns>Преобразованное значение</returns>
            /// <example>
            /// <![CDATA[
            /// var enumerator = new StringPtr("10").Split(',').GetEnumerator();
            /// enumerator.MoveNext();
            /// var value = (int)enumerator;
            /// ]]>
            /// </example>
            public static implicit operator int(TokenEnumerator Enumerator) => Enumerator.Current.ParseInt32();

            /// <summary>Оператор неявного преобразования перечислителя в вещественное число</summary>
            /// <param name="Enumerator">Перечислитель фрагментов строки</param>
            /// <returns>Преобразованное значение</returns>
            /// <example>
            /// <![CDATA[
            /// var enumerator = new StringPtr("1.5").Split(',').GetEnumerator();
            /// enumerator.MoveNext();
            /// var value = (double)enumerator;
            /// ]]>
            /// </example>
            public static implicit operator double(TokenEnumerator Enumerator) => Enumerator.Current.ParseDouble();
        }
    }
}