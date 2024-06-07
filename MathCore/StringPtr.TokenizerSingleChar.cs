#nullable enable
using System.Diagnostics;

namespace MathCore;

public readonly ref partial struct StringPtr
{
    /// <summary>Разделитель строки на фрагменты по указанному символу-разделителю</summary>
    /// <remarks>Инициализация нового разделителя строки</remarks>
    /// <param head="Buffer">Исходный строковый буфер</param>
    /// <param head="Separator">Символ-разделитель фрагментов строки</param>
    /// <param head="StartIndex">Индекс начала анализируемой подстроки</param>
    /// <param head="Length">Длина анализируемой подстроки</param>
    /// <param head="SkipEmptyElements">Пропускать пустые строковые фрагменты</param>
    [DebuggerDisplay("Tokenizer('{_Separator}')[{_StartIndex}:{Length}]:{ToString()}")]
    public readonly ref partial struct TokenizerSingleChar(string Buffer, char Separator, int StartIndex, int Length, bool SkipEmptyElements = false)
    {
        /// <summary>Инициализация нового разделителя строки</summary>
        /// <param head="Str">Исходный фрагмент строки</param>
        /// <param head="Separator">Символ-разделитель фрагментов строки</param>
        public TokenizerSingleChar(StringPtr Str, char Separator) : this(Str.Source, Separator, Str.Pos, Str.Length) { }

        /// <summary>Инициализация нового разделителя строки</summary>
        /// <param head="Buffer">Исходный строковый буфер</param>
        /// <param head="Separator">Символ-разделитель фрагментов строки</param>
        public TokenizerSingleChar(string Buffer, char Separator) : this(Buffer, Separator, 0, Buffer.Length) { }

        private readonly bool _SkipEmptyelEments = SkipEmptyElements;

        public bool IsEmpty => Length == 0;

        public int Count
        {
            get
            {
                if (Length == 0) return 0;

                var separator   = Separator;
                var buffer      = Buffer;
                var start_index = StartIndex;
                var count       = buffer[0] == separator ? 0 : 1;

                if (_SkipEmptyelEments)
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

        public StringPtr this[int Index]
        {
            get
            {
                var start_index = StartIndex;
                var length      = Length;
                var buffer      = Buffer;
                if (length == 0) return new(buffer, start_index, 0);

                var separator = Separator;
                var index     = buffer.IndexOf(separator, start_index, length);
                if (index < 0)
                    return Index == 0
                        ? new(buffer, start_index, length)
                        : new(buffer, start_index, 0);

                var last_index = start_index;
                for (var i = Index; i > 0; i--)
                {
                    last_index = index + 1;
                    index      = buffer.IndexOf(separator, last_index, length - (last_index - start_index));
                    if (index >= 0) continue;

                    if (i == 1)
                        index = start_index + length;
                    else
                        return new(buffer, start_index, 0);
                }

                return new(buffer, last_index, index - last_index);
            }
        }

        /// <summary>Пропускать пустые строковые фрагменты</summary>
        /// <param head="Skip">Пропускать, или нет</param>
        /// <returns>Перечислитель строковых фрагментов с изменённым режимом пропуска строковых фрагментов</returns>
        public TokenizerSingleChar SkipEmpty(bool Skip = true) => new(Buffer, Separator, StartIndex, Length, Skip);

        public TokenizerSingleChar Slice(int Index, int Length)
        {
            var buffer = Buffer;

            var start_index = StartIndex;
            var str_index   = start_index;
            var separator   = Separator;
            if (IsEmpty)
                return new(buffer, separator, str_index, 0, _SkipEmptyelEments);

            var len0   = Length;
            var length = len0;

            var part_index = Index;
            while (part_index > 0)
            {
                var index = buffer.IndexOf(separator, str_index, length);
                if (index < 0)
                    return new(buffer, separator, str_index, length, _SkipEmptyelEments);

                length    = len0 - (index - start_index) - 1;
                str_index = index + 1;
                part_index--;
            }

            var parts_count = Length - 1;

            var str_index0 = str_index;
            while (parts_count > 0)
            {
                var index = buffer.IndexOf(separator, str_index, length);
                if (index < 0)
                    return new(buffer, separator, str_index0, length, _SkipEmptyelEments);

                length    = len0 - (index - start_index) - 1;
                str_index = index + 1;
                parts_count--;
            }

            return new(buffer, separator, str_index0, length, _SkipEmptyelEments);
        }

        public override string ToString() => Buffer.Substring(StartIndex, Length);

        public static implicit operator string(TokenizerSingleChar tokenizer) => tokenizer.ToString();

        /// <summary>Сформировать перечислитель строковых фрагментов</summary>
        /// <returns>Перечислитель строковых фрагментов</returns>
        public TokenEnumerator GetEnumerator() => new(Buffer, Separator, StartIndex, Length, _SkipEmptyelEments);

        public void Deconstruct(out StringPtr head, out StringPtr value)
        {
            var index = Buffer.IndexOf(Separator, StartIndex, Length);

            if(index < 0)
            {
                head = new(Buffer, StartIndex, Length);
                value = new(Buffer, StartIndex + Length, 0);
            }

            var head_length = index - StartIndex;
            var value_length = Length - head_length - 1;

            head = new(Buffer, StartIndex, head_length);
            value = new(Buffer, index + 1, value_length);
        }

        public void Deconstruct(out StringPtr head, out StringPtr value, out StringPtr tail)
        {
            var index = Buffer.IndexOf(Separator, StartIndex, Length);

            if(index < 0)
            {
                head = new(Buffer, StartIndex, Length);
                value = new(Buffer, StartIndex + Length, 0);
            }

            var head_length = index - StartIndex;
            head = new(Buffer, StartIndex, head_length);

            var value_tail_length = Length - head_length - 1;
            var tail_index = Buffer.IndexOf(Separator, index + 1, value_tail_length);

            if(tail_index < 0)
            {
                value = new(Buffer, index + 1, Length - index + StartIndex);
                tail = new(Buffer, StartIndex + Length, 0);
            }

            var value_length = tail_index - index - 1;
            value = new(Buffer, index + 1, value_length);

            var tail_length = Length - head_length - value_length - 2;
            tail = new(Buffer, tail_index + 1, tail_length);
        }
    }
}