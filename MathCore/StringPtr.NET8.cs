#if NET8_0_OR_GREATER

namespace MathCore;

public readonly ref partial struct StringPtr
{
    /// <summary>Представление подстроки в виде <see cref="ReadOnlySpan{T}"/></summary>
    /// <returns>Диапазон символов подстроки</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("abc");
    /// var span = ptr.Span;
    /// ]]>
    /// </example>
    public ReadOnlySpan<char> Span => Source.AsSpan(Pos, Length);

    /// <summary>Представление подстроки в виде <see cref="ReadOnlyMemory{T}"/></summary>
    /// <returns>Буфер подстроки</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("abc");
    /// var memory = ptr.Memory;
    /// ]]>
    /// </example>
    public ReadOnlyMemory<char> Memory => Source.AsMemory(Pos, Length);

    /// <summary>Получить подстроку по диапазону</summary>
    /// <param name="range">Диапазон в рамках подстроки</param>
    /// <returns>Подстрока по заданному диапазону</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("abcdef");
    /// var part = ptr[1..4];
    /// ]]>
    /// </example>
    public StringPtr this[Range range]
    {
        get
        {
            var start_index = range.Start;
            var end_index = range.End;


            var start = start_index.IsFromEnd ? Length - start_index.Value : start_index.Value;
            var end = end_index.IsFromEnd ? Length - end_index.Value : end_index.Value;

            var length = end - start;
            return this[start, length];
        }
    }

    /// <summary>Получить символ по индексу</summary>
    /// <param name="index">Индекс в подстроке</param>
    /// <returns>Символ по указанному индексу</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("abc");
    /// var value = ptr[^1];
    /// ]]>
    /// </example>
    public char this[Index index] => this[index.Value];
}

#endif