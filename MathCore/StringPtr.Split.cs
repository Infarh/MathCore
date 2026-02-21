namespace MathCore;

public readonly ref partial struct StringPtr
{
    /// <summary>Разделить строку на подстроки по указанным символам-разделителям</summary>
    /// <param name="Separators">Символы-разделители</param>
    /// <returns>Разделитель строки на фрагменты</returns>
    /// <example>
    /// <![CDATA[
    /// var tokens = new StringPtr("a,b").Split(',', ';');
    /// ]]>
    /// </example>
    public Tokenizer Split(params char[] Separators) => new(this, Separators);

    /// <summary>Разделить строку на подстроки по указанным символам-разделителям</summary>
    /// <param name="SkipEmpty">Пропускать пустые фрагменты</param>
    /// <param name="Separators">Символы-разделители</param>
    /// <returns>Разделитель строки на фрагменты</returns>
    /// <example>
    /// <![CDATA[
    /// var tokens = new StringPtr("a,,b").Split(true, ','); 
    /// ]]>
    /// </example>
    public Tokenizer Split(bool SkipEmpty, params char[] Separators) => SkipEmpty
        ? new Tokenizer(this, Separators).SkipEmpty(true)
        : new(this, Separators);

    /// <summary>Разделить строку на подстроки по указанному символу-разделителю</summary>
    /// <param name="Separator">Символ-разделитель</param>
    /// <returns>Разделитель строки на фрагменты</returns>
    /// <example>
    /// <![CDATA[
    /// var tokens = new StringPtr("a,b").Split(',');
    /// ]]>
    /// </example>
    public TokenizerSingleChar Split(char Separator) => new(this, Separator);

    /// <summary>Разделить строку на подстроки по указанному символу-разделителю</summary>
    /// <param name="SkipEmpty">Пропускать пустые фрагменты</param>
    /// <param name="Separator">Символ-разделитель</param>
    /// <returns>Разделитель строки на фрагменты</returns>
    /// <example>
    /// <![CDATA[
    /// var tokens = new StringPtr("a,,b").Split(true, ','); 
    /// ]]>
    /// </example>
    public TokenizerSingleChar Split(bool SkipEmpty, char Separator) => SkipEmpty
        ? new TokenizerSingleChar(this, Separator).SkipEmpty()
        : new(this, Separator);

    /// <summary>Разделить строку на ключ и значение</summary>
    /// <param name="Separator">Символ-разделитель</param>
    /// <returns>Ключ и целочисленное значение</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("x=10").SplitKeyValueInt32('=');
    /// ]]>
    /// </example>
    public (string Key, int Value) SplitKeyValueInt32(char Separator)
    {
        var value = SubstringAfter(Separator).ParseInt32();
        var key = SubstringBefore(Separator).ToString();
        return (key, value);
    }

    /// <summary>Разделить строку на ключ и значение</summary>
    /// <param name="Separator">Символ-разделитель</param>
    /// <returns>Ключ и вещественное значение</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("x=1.5").SplitKeyValueDouble('=');
    /// ]]>
    /// </example>
    public (string Key, double Value) SplitKeyValueDouble(char Separator)
    {
        var value = SubstringAfter(Separator).ParseDouble();
        var key = SubstringBefore(Separator).ToString();
        return (key, value);
    }

    /// <summary>Разделить строку на ключ и значение</summary>
    /// <param name="Separator">Символ-разделитель</param>
    /// <param name="format">Провайдер формата значения</param>
    /// <returns>Ключ и вещественное значение</returns>
    /// <example>
    /// <![CDATA[
    /// var result = new StringPtr("x=1,5").SplitKeyValueDouble('=', CultureInfo.GetCultureInfo("ru-RU"));
    /// ]]>
    /// </example>
    public (string Key, double Value) SplitKeyValueDouble(char Separator, IFormatProvider format)
    {
        var value = SubstringAfter(Separator).ParseDouble(format);
        var key = SubstringBefore(Separator).ToString();
        return (key, value);
    }

    /// <summary>Разделить строку на интервал целых чисел</summary>
    /// <param name="Separator">Символ-разделитель</param>
    /// <returns>Первая и последняя границы</returns>
    /// <example>
    /// <![CDATA[
    /// var interval = new StringPtr("1-10").SplitIntervalInt32('-');
    /// ]]>
    /// </example>
    public (int First, int Last) SplitIntervalInt32(char Separator)
    {
        var first = SubstringBefore(Separator).ParseInt32();
        var last = SubstringAfter(Separator).ParseInt32();
        return (first, last);
    }

    /// <summary>Разделить строку на интервал вещественных чисел</summary>
    /// <param name="Separator">Символ-разделитель</param>
    /// <param name="OrderMinMax">Отсортировать границы по возрастанию</param>
    /// <returns>Первая и последняя границы</returns>
    /// <example>
    /// <![CDATA[
    /// var interval = new StringPtr("10-1").SplitIntervalDouble('-', true);
    /// ]]>
    /// </example>
    public (double First, double Last) SplitIntervalDouble(char Separator, bool OrderMinMax = false)
    {
        var first = SubstringBefore(Separator).ParseDouble();
        var last = SubstringAfter(Separator).ParseDouble();
        return OrderMinMax ? (Math.Min(first, last), Math.Max(first, last)) : (first, last);
    }

    /// <summary>Разделить строку на интервал вещественных чисел</summary>
    /// <param name="Separator">Символ-разделитель</param>
    /// <param name="format">Провайдер формата значения</param>
    /// <param name="OrderMinMax">Отсортировать границы по возрастанию</param>
    /// <returns>Первая и последняя границы</returns>
    /// <example>
    /// <![CDATA[
    /// var interval = new StringPtr("10,5-1,5").SplitIntervalDouble('-', CultureInfo.GetCultureInfo("ru-RU"), true);
    /// ]]>
    /// </example>
    public (double First, double Last) SplitIntervalDouble(char Separator, IFormatProvider format, bool OrderMinMax = false)
    {
        var first = SubstringBefore(Separator).ParseDouble(format);
        var last = SubstringAfter(Separator).ParseDouble(format);
        return OrderMinMax ? (Math.Min(first, last), Math.Max(first, last)) : (first, last);
    }
}
