using System.Diagnostics;

namespace MathCore;

public readonly ref partial struct StringPtr
{
    /// <summary>Разделитель строки на фрагменты по одному символу-разделителю</summary>
    /// <param name="Buffer">Исходный строковый буфер</param>
    /// <param name="Separator">Символ-разделитель фрагментов строки</param>
    /// <param name="StartIndex">Индекс начала анализируемой подстроки</param>
    /// <param name="Length">Длина анализируемой подстроки</param>
    /// <param name="SkipEmptyElements">Пропускать пустые строковые фрагменты</param>
    /// <remarks>Создаёт разделитель без выделения новых строк</remarks>
    /// <example>
    /// <![CDATA[
    /// var tokenizer = new StringPtr("a,b").Split(',');
    /// foreach (var part in tokenizer)
    /// {
    ///     Console.WriteLine(part);
    /// }
    /// ]]>
    /// </example>
    [DebuggerDisplay("Tokenizer:{ToString()}")]
    public readonly ref partial struct TokenizerSingleChar(string Buffer, char Separator, int StartIndex, int Length, bool SkipEmptyElements = false)
    {
        /// <summary>Инициализация нового разделителя строки</summary>
        /// <param name="Str">Исходный фрагмент строки</param>
        /// <param name="Separator">Символ-разделитель фрагментов строки</param>
        /// <example>
        /// <![CDATA[
        /// var tokenizer = new StringPtr("a,b").Split(',');
        /// ]]>
        /// </example>
        public TokenizerSingleChar(StringPtr Str, char Separator) : this(Str.Source, Separator, Str.Pos, Str.Length) { }

        /// <summary>Инициализация нового разделителя строки</summary>
        /// <param name="Buffer">Исходный строковый буфер</param>
        /// <param name="Separator">Символ-разделитель фрагментов строки</param>
        /// <example>
        /// <![CDATA[
        /// var tokenizer = new StringPtr.TokenizerSingleChar("a,b", ',')
        /// ]]>
        /// </example>
        public TokenizerSingleChar(string Buffer, char Separator) : this(Buffer, Separator, 0, Buffer.Length) { }

        private readonly bool _SkipEmptyElements = SkipEmptyElements;

        /// <summary>Признак отсутствия фрагментов</summary>
        /// <example>
        /// <![CDATA[
        /// var is_empty = new StringPtr.TokenizerSingleChar("", ',').IsEmpty;
        /// ]]>
        /// </example>
        public bool IsEmpty => Length == 0;

        /// <summary>Количество фрагментов</summary>
        /// <example>
        /// <![CDATA[
        /// var count = new StringPtr("a,b").Split(',').Count;
        /// ]]>
        /// </example>
        public int Count
        {
            get
            {
                if (Length == 0) return 0;

                var separator = Separator;
                var buffer = Buffer;
                var start_index = StartIndex;
                var count = buffer[0] == separator ? 0 : 1; // учитываем первый фрагмент без разделителя

                if (_SkipEmptyElements)
                {
                    for (var (i, last_index, end) = (start_index, start_index, start_index + Length); i < end; i++)
                        if (buffer[i] == separator && i > last_index + 1)
                        {
                            count++;
                            last_index = i;
                        }
                }
                else
                    for (var (i, end) = (start_index, start_index + Length); i < end; i++)
                        if (buffer[i] == separator)
                            count++;

                return count;
            }
        }

        /// <summary>Получить фрагмент по индексу</summary>
        /// <param name="Index">Индекс фрагмента</param>
        /// <returns>Фрагмент строки</returns>
        /// <example>
        /// <![CDATA[
        /// var part = new StringPtr("a,b").Split(',')[1];
        /// ]]>
        /// </example>
        public StringPtr this[int Index]
        {
            get
            {
                var start_index = StartIndex;
                var length = Length;
                var buffer = Buffer;
                if (length == 0) return new(buffer, start_index, 0);

                var separator = Separator;
                var index = buffer.IndexOf(separator, start_index, length);
                if (index < 0)
                    return Index == 0
                        ? new(buffer, start_index, length)
                        : new(buffer, start_index, 0);

                var last_index = start_index;
                for (var i = Index; i > 0; i--)
                {
                    last_index = index + 1;
                    index = buffer.IndexOf(separator, last_index, length - (last_index - start_index));
                    if (index >= 0) continue;

                    if (i == 1)
                        index = start_index + length; // возвращаем последний фрагмент
                    else
                        return new(buffer, start_index, 0);
                }

                return new(buffer, last_index, index - last_index);
            }
        }

        /// <summary>Пропускать пустые строковые фрагменты</summary>
        /// <param name="Skip">Пропускать или нет</param>
        /// <returns>Разделитель строк с режимом пропуска</returns>
        /// <example>
        /// <![CDATA[
        /// var tokenizer = new StringPtr("a,,b").Split(',').SkipEmpty();
        /// ]]>
        /// </example>
        public TokenizerSingleChar SkipEmpty(bool Skip = true) => new(Buffer, Separator, StartIndex, Length, Skip);

        /// <summary>Получить срез фрагментов</summary>
        /// <param name="Index">Начальный индекс фрагмента</param>
        /// <param name="Length">Количество фрагментов</param>
        /// <returns>Новый разделитель для указанного диапазона</returns>
        /// <example>
        /// <![CDATA[
        /// var slice = new StringPtr("a,b,c").Split(',').Slice(1, 1);
        /// ]]>
        /// </example>
        public TokenizerSingleChar Slice(int Index, int Length)
        {
            var buffer = Buffer;

            var start_index = StartIndex;
            var str_index = start_index;
            var separator = Separator;
            if (IsEmpty)
                return new(buffer, separator, str_index, 0, _SkipEmptyElements);

            var len0 = Length;
            var length = len0;

            var part_index = Index;
            while (part_index > 0)
            {
                var index = buffer.IndexOf(separator, str_index, length);
                if (index < 0)
                    return new(buffer, separator, str_index, length, _SkipEmptyElements);

                length = len0 - (index - start_index) - 1; // смещаемся к следующему фрагменту
                str_index = index + 1;
                part_index--;
            }

            var parts_count = Length - 1;

            var str_index0 = str_index;
            while (parts_count > 0)
            {
                var index = buffer.IndexOf(separator, str_index, length);
                if (index < 0)
                    return new(buffer, separator, str_index0, length, _SkipEmptyElements);

                length = len0 - (index - start_index) - 1; // уменьшаем диапазон среза
                str_index = index + 1;
                parts_count--;
            }

            return new(buffer, separator, str_index0, length, _SkipEmptyElements);
        }

        /// <summary>Преобразовать диапазон в строку</summary>
        /// <returns>Строковое представление диапазона</returns>
        /// <example>
        /// <![CDATA[
        /// var text = new StringPtr("a,b").Split(',').ToString();
        /// ]]>
        /// </example>
        public override string ToString() => Buffer.Substring(StartIndex, Length);

        /// <summary>Оператор неявного преобразования разделителя в строку</summary>
        /// <param name="tokenizer">Разделитель строк</param>
        /// <returns>Строковое представление диапазона</returns>
        /// <example>
        /// <![CDATA[
        /// string text = new StringPtr("a,b").Split(',');
        /// ]]>
        /// </example>
        public static implicit operator string(TokenizerSingleChar tokenizer) => tokenizer.ToString();

        /// <summary>Деконструкция на начало и значение</summary>
        /// <param name="head">Начало строки</param>
        /// <param name="value">Значение строки</param>
        /// <example>
        /// <![CDATA[
        /// var (head, value) = new StringPtr("a:b").Split(':');
        /// ]]>
        /// </example>
        public void Deconstruct(out StringPtr head, out StringPtr value)
        {
            var index = Buffer.IndexOf(Separator, StartIndex, Length);

            if (index < 0)
            {
                head = new(Buffer, StartIndex, Length);
                value = new(Buffer, StartIndex + Length, 0);
                return;
            }

            var head_length = index - StartIndex;
            var value_length = Length - head_length - 1;

            head = new(Buffer, StartIndex, head_length);
            value = new(Buffer, index + 1, value_length);
        }

        /// <summary>Деконструкция на начало, значение и хвост</summary>
        /// <param name="head">Начало строки</param>
        /// <param name="value">Значение строки</param>
        /// <param name="tail">Хвост строки</param>
        /// <example>
        /// <![CDATA[
        /// var (head, value, tail) = new StringPtr("a:b:c").Split(':');
        /// ]]>
        /// </example>
        public void Deconstruct(out StringPtr head, out StringPtr value, out StringPtr tail)
        {
            var index = Buffer.IndexOf(Separator, StartIndex, Length);

            if (index < 0)
            {
                head = new(Buffer, StartIndex, Length);
                value = new(Buffer, StartIndex + Length, 0);
                tail = new(Buffer, StartIndex + Length, 0);
                return;
            }

            var head_length = index - StartIndex;
            head = new(Buffer, StartIndex, head_length);

            var value_tail_length = Length - head_length - 1;
            var tail_index = Buffer.IndexOf(Separator, index + 1, value_tail_length);

            if (tail_index < 0)
            {
                value = new(Buffer, index + 1, Length - index + StartIndex);
                tail = new(Buffer, StartIndex + Length, 0);
                return;
            }

            var value_length = tail_index - index - 1;
            value = new(Buffer, index + 1, value_length);

            var tail_length = Length - head_length - value_length - 2;
            tail = new(Buffer, tail_index + 1, tail_length);
        }

        /// <summary>Преобразовать последовательность фрагментов в массив строк</summary>
        /// <returns>Массив строковых фрагментов</returns>
        /// <example>
        /// <![CDATA[
        /// var array = new StringPtr("a,b").Split(',').ToArray();
        /// ]]>
        /// </example>
        public string[] ToArray() => [..ToList()];

        /// <summary>Преобразовать последовательность фрагментов в список строк</summary>
        /// <returns>Список строковых фрагментов</returns>
        /// <example>
        /// <![CDATA[
        /// var list = new StringPtr("a,b").Split(',').ToList();
        /// ]]>
        /// </example>
        public List<string> ToList()
        {
            var result = new List<string>();

            for (var enumerator = GetEnumerator(); enumerator.MoveNext();)
                result.Add(enumerator.Current);

            result.TrimExcess();

            return result;
        }
    }
}