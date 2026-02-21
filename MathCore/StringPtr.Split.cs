namespace MathCore;

public readonly ref partial struct StringPtr
{
    /// <summary>Разделить строку на подстроки по указанному символам-разделителям</summary>
    /// <param name="Separators">Символы-разделители</param>
    /// <returns>Разделитель строки на фрагменты</returns>
    public Tokenizer Split(params char[] Separators) => new(this, Separators);

    /// <summary>Разделить строку на подстроки по указанному символам-разделителям</summary>
    /// <param name="SkipEmpty">Пропускать пустые фрагменты</param>
    /// <param name="Separators">Символы-разделители</param>
    /// <returns>Разделитель строки на фрагменты</returns>
    public Tokenizer Split(bool SkipEmpty, params char[] Separators) => SkipEmpty
        ? new Tokenizer(this, Separators).SkipEmpty(true)
        : new(this, Separators);

    /// <summary>Разделить строку на подстроки по указанному символу-разделителю</summary>
    /// <param name="Separator">Символ-разделитель</param>
    /// <returns>Разделитель строки на фрагменты</returns>
    public TokenizerSingleChar Split(char Separator) => new(this, Separator);

    /// <summary>Разделить строку на подстроки по указанному символу-разделителю</summary>
    /// <param name="SkipEmpty">Пропускать пустые фрагменты</param>
    /// <param name="Separator">Символ-разделитель</param>
    /// <returns>Разделитель строки на фрагменты</returns>
    public TokenizerSingleChar Split(bool SkipEmpty, char Separator) => SkipEmpty
        ? new TokenizerSingleChar(this, Separator).SkipEmpty()
        : new(this, Separator);

    public (string Key, int Value) SplitKeyValueInt32(char Separator)
    {
        var value = SubstringAfter(Separator).ParseInt32();
        var key = SubstringBefore(Separator).ToString();
        return (key, value);
    }

    public (string Key, double Value) SplitKeyValueDouble(char Separator)
    {
        var value = SubstringAfter(Separator).ParseDouble();
        var key = SubstringBefore(Separator).ToString();
        return (key, value);
    }

    public (string Key, double Value) SplitKeyValueDouble(char Separator, IFormatProvider format)
    {
        var value = SubstringAfter(Separator).ParseDouble(format);
        var key = SubstringBefore(Separator).ToString();
        return (key, value);
    }

    public (int First, int Last) SplitIntervalInt32(char Separator)
    {
        var first = SubstringBefore(Separator).ParseInt32();
        var last = SubstringAfter(Separator).ParseInt32();
        return (first, last);
    }

    public (double First, double Last) SplitIntervalDouble(char Separator, bool OrderMinMax = false)
    {
        var first = SubstringBefore(Separator).ParseDouble();
        var last = SubstringAfter(Separator).ParseDouble();
        return OrderMinMax ? (Math.Min(first, last), Math.Max(first, last)) : (first, last);
    }

    public (double First, double Last) SplitIntervalDouble(char Separator, IFormatProvider format, bool OrderMinMax = false)
    {
        var first = SubstringBefore(Separator).ParseDouble(format);
        var last = SubstringAfter(Separator).ParseDouble(format);
        return OrderMinMax ? (Math.Min(first, last), Math.Max(first, last)) : (first, last);
    }
}
