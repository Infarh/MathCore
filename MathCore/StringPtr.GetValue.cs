namespace MathCore;

public readonly ref partial struct StringPtr
{
    /// <summary>Получить имя из пары ключ=значение</summary>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Подстрока, содержащая имя (ключ) из пары ключ-значение</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("key=42");
    /// var name = ptr.GetName('=');
    /// ]]>
    /// </example>
    public StringPtr GetName(char Separator = '=') => SubstringBefore(Separator);

    /// <summary>Получить значение из пары ключ=значение</summary>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Подстрока, содержащая значение из пары ключ-значение</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("key=42");
    /// var value = ptr.GetValueString('=');
    /// ]]>
    /// </example>
    public StringPtr GetValueString(char Separator = '=') => SubstringAfter(Separator);

    /// <summary>Получить вещественное значение из пары ключ=значение</summary>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Вещественное значение из пары ключ-значение</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("x=3.5");
    /// var value = ptr.GetValueDouble('=');
    /// ]]>
    /// </example>
    public double GetValueDouble(char Separator = '=') => GetValueString(Separator).ParseDouble();

    /// <summary>Получить вещественное значение из пары ключ=значение</summary>
    /// <param name="Provider">Провайдер формата значения</param>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Вещественное значение из пары ключ-значение</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("x=3,5");
    /// var value = ptr.GetValueDouble(CultureInfo.GetCultureInfo("ru-RU"), '=');
    /// ]]>
    /// </example>
    public double GetValueDouble(IFormatProvider Provider, char Separator = '=') => GetValueString(Separator).ParseDouble(Provider);

    /// <summary>Получить целочисленное значение из пары ключ=значение</summary>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Целочисленное значение из пары ключ-значение</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("x=12");
    /// var value = ptr.GetValueInt32('=');
    /// ]]>
    /// </example>
    public int GetValueInt32(char Separator = '=') => GetValueString(Separator).ParseInt32();

    /// <summary>Попытаться получить вещественное значение из пары ключ=значение</summary>
    /// <param name="value">Вещественное значение из пары ключ-значение</param>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Истина, если преобразование подстроки значения в вещественное значение выполнено успешно</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("x=3.5");
    /// var ok = ptr.TryGetValueDouble(out var value, '=');
    /// ]]>
    /// </example>
    public bool TryGetValueDouble(out double value, char Separator = '=') => GetValueString(Separator).TryParseDouble(out value);

    /// <summary>Попытаться получить вещественное значение из пары ключ=значение</summary>
    /// <param name="value">Вещественное значение из пары ключ-значение</param>
    /// <param name="Provider">Провайдер формата значения</param>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Истина, если преобразование подстроки значения в вещественное значение выполнено успешно</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("x=3,5");
    /// var ok = ptr.TryGetValueDouble(CultureInfo.GetCultureInfo("ru-RU"), out var value, '=');
    /// ]]>
    /// </example>
    public bool TryGetValueDouble(IFormatProvider Provider, out double value, char Separator = '=') => GetValueString(Separator).TryParseDouble(Provider, out value);

    /// <summary>Попытаться получить целочисленное значение из пары ключ=значение</summary>
    /// <param name="value">Целочисленное значение из пары ключ-значение</param>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Истина, если преобразование подстроки значения в целочисленное значение выполнено успешно</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("x=12");
    /// var ok = ptr.TryGetValueInt32(out var value, '=');
    /// ]]>
    /// </example>
    public bool TryGetValueInt32(out int value, char Separator = '=') => GetValueString(Separator).TryParseInt32(out value);

    /// <summary>Попытаться получить булево значение из пары ключ=значение</summary>
    /// <param name="value">Булево значение из пары ключ-значение</param>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Истина, если преобразование подстроки значения в булево значение выполнено успешно</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("flag=true");
    /// var ok = ptr.TryGetValueBool(out var value, '=');
    /// ]]>
    /// </example>
    public bool TryGetValueBool(out bool value, char Separator = '=')
    {
        var str = GetValueString(Separator);
        value = false;

        if (str.IsEmpty) return false;

        if (!str.Equals("true", StringComparison.OrdinalIgnoreCase))
            return str.Equals("false", StringComparison.OrdinalIgnoreCase);

        value = true;
        return true;
    }

    /// <summary>Получить булево значение из пары ключ=значение</summary>
    /// <param name="Separator">Символ-разделитель пары ключ-значение</param>
    /// <returns>Булево значение из пары ключ-значение</returns>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("flag=true");
    /// var value = ptr.GetValueBool('=');
    /// ]]>
    /// </example>
    public bool GetValueBool(char Separator = '=') => TryGetValueBool(out var value, Separator)
        ? value
        : throw new FormatException("Строка имела неверный формат");
}
