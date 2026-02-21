namespace MathCore;

public readonly ref partial struct StringPtr
{
    /// <summary>Оператор проверки на равенство фрагмента строки со строкой</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если фрагмент строки посимвольно равен указанной строке</returns>
    public static bool operator ==(StringPtr ptr, string str) => ptr.Equals(str);

    /// <summary>Оператор проверки на неравенство фрагмента строки со строкой</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если фрагмент строки посимвольно неравен указанной строке</returns>
    public static bool operator !=(StringPtr ptr, string str) => !(ptr == str);

    /// <summary>Оператор проверки на равенство фрагмента строки со строкой</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если фрагмент строки посимвольно равен указанной строке</returns>
    public static bool operator ==(string str, StringPtr ptr) => ptr.Equals(str);

    /// <summary>Оператор проверки на неравенство фрагмента строки со строкой</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если фрагмент строки посимвольно неравен указанной строке</returns>
    public static bool operator !=(string str, StringPtr ptr) => !(ptr == str);

    /// <summary>Оператор проверки на равенство фрагмента строки с целым числом</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="x">Целое число</param>
    /// <returns>Истина, если подстрока соответствует числу</returns>
    /// <example>
    /// <![CDATA[
    /// var ok = new StringPtr("42") == 42;
    /// ]]>
    /// </example>
    public static bool operator ==(StringPtr ptr, int x) => ptr.Equals(x);

    /// <summary>Оператор проверки на равенство целого числа с фрагментом строки</summary>
    /// <param name="x">Целое число</param>
    /// <param name="ptr">Фрагмент строки</param>
    /// <returns>Истина, если подстрока соответствует числу</returns>
    /// <example>
    /// <![CDATA[
    /// var ok = 42 == new StringPtr("42");
    /// ]]>
    /// </example>
    public static bool operator ==(int x, StringPtr ptr) => ptr == x;

    /// <summary>Оператор проверки на неравенство фрагмента строки с целым числом</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="x">Целое число</param>
    /// <returns>Истина, если подстрока не соответствует числу</returns>
    /// <example>
    /// <![CDATA[
    /// var ok = new StringPtr("10") != 11;
    /// ]]>
    /// </example>
    public static bool operator !=(StringPtr ptr, int x) => !(ptr == x);

    /// <summary>Оператор проверки на неравенство целого числа с фрагментом строки</summary>
    /// <param name="x">Целое число</param>
    /// <param name="ptr">Фрагмент строки</param>
    /// <returns>Истина, если подстрока не соответствует числу</returns>
    /// <example>
    /// <![CDATA[
    /// var ok = 11 != new StringPtr("10");
    /// ]]>
    /// </example>
    public static bool operator !=(int x, StringPtr ptr) => !(ptr == x);

    /// <summary>Оператор проверки на равенство фрагмента строки с вещественным числом</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="x">Вещественное число</param>
    /// <returns>Истина, если подстрока соответствует числу</returns>
    /// <example>
    /// <![CDATA[
    /// var ok = new StringPtr("3.5") == 3.5;
    /// ]]>
    /// </example>
    public static bool operator ==(StringPtr ptr, double x) => ptr.Equals(x);

    /// <summary>Оператор проверки на равенство вещественного числа с фрагментом строки</summary>
    /// <param name="x">Вещественное число</param>
    /// <param name="ptr">Фрагмент строки</param>
    /// <returns>Истина, если подстрока соответствует числу</returns>
    /// <example>
    /// <![CDATA[
    /// var ok = 3.5 == new StringPtr("3.5");
    /// ]]>
    /// </example>
    public static bool operator ==(double x, StringPtr ptr) => ptr == x;

    /// <summary>Оператор проверки на неравенство фрагмента строки с вещественным числом</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="x">Вещественное число</param>
    /// <returns>Истина, если подстрока не соответствует числу</returns>
    /// <example>
    /// <![CDATA[
    /// var ok = new StringPtr("3.5") != 3.6;
    /// ]]>
    /// </example>
    public static bool operator !=(StringPtr ptr, double x) => !(ptr == x);

    /// <summary>Оператор проверки на неравенство вещественного числа с фрагментом строки</summary>
    /// <param name="x">Вещественное число</param>
    /// <param name="ptr">Фрагмент строки</param>
    /// <returns>Истина, если подстрока не соответствует числу</returns>
    /// <example>
    /// <![CDATA[
    /// var ok = 3.6 != new StringPtr("3.5");
    /// ]]>
    /// </example>
    public static bool operator !=(double x, StringPtr ptr) => !(ptr == x);

    /// <summary>Оператор проверки на равенство фрагмента строки с символом</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="c">Символ</param>
    /// <returns>Истина, если подстрока равна символу</returns>
    /// <example>
    /// <![CDATA[
    /// var ok = new StringPtr("A") == 'A';
    /// ]]>
    /// </example>
    public static bool operator ==(StringPtr ptr, char c) => ptr.Equals(c);

    /// <summary>Оператор проверки на равенство символа с фрагментом строки</summary>
    /// <param name="c">Символ</param>
    /// <param name="ptr">Фрагмент строки</param>
    /// <returns>Истина, если подстрока равна символу</returns>
    /// <example>
    /// <![CDATA[
    /// var ok = 'A' == new StringPtr("A");
    /// ]]>
    /// </example>
    public static bool operator ==(char c, StringPtr ptr) => ptr.Equals(c);

    /// <summary>Оператор проверки на неравенство фрагмента строки с символом</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="c">Символ</param>
    /// <returns>Истина, если подстрока не равна символу</returns>
    /// <example>
    /// <![CDATA[
    /// var ok = new StringPtr("A") != 'B';
    /// ]]>
    /// </example>
    public static bool operator !=(StringPtr ptr, char c) => !(ptr == c);

    /// <summary>Оператор проверки на неравенство символа с фрагментом строки</summary>
    /// <param name="c">Символ</param>
    /// <param name="ptr">Фрагмент строки</param>
    /// <returns>Истина, если подстрока не равна символу</returns>
    /// <example>
    /// <![CDATA[
    /// var ok = 'B' != new StringPtr("A");
    /// ]]>
    /// </example>
    public static bool operator !=(char c, StringPtr ptr) => !(c == ptr);

    /// <summary>Оператор порядка "больше", сравнивающий фрагмент строки со строкой</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если фрагмент строки больше, чем указанная строка</returns>
    public static bool operator >(StringPtr ptr, string str) => string.Compare(ptr.Source, ptr.Pos, str, 0, ptr.Length) > 0;

    /// <summary>Оператор порядка "меньше", сравнивающий фрагмент строки со строкой</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если фрагмент строки меньше, чем указанная строка</returns>
    public static bool operator <(StringPtr ptr, string str) => string.Compare(ptr.Source, ptr.Pos, str, 0, ptr.Length) < 0;

    /// <summary>Оператор порядка "больше", сравнивающий строку с фрагментом строки</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если строка больше, чем указанный фрагмент строки</returns>
    public static bool operator >(string str, StringPtr ptr) => ptr < str;

    /// <summary>Оператор порядка "меньше", сравнивающий строку с фрагментом строки</summary>
    /// <param name="ptr">Фрагмент строки</param>
    /// <param name="str">Строка</param>
    /// <returns>Истина, если строка меньше, чем указанный фрагмент строки</returns>
    public static bool operator <(string str, StringPtr ptr) => ptr > str;

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Оператор неявного преобразования строки во фрагмент строки</summary>
    /// <param name="Source">Исходная строка</param>
    /// <example>
    /// <![CDATA[
    /// StringPtr ptr = "value";
    /// ]]>
    /// </example>
    public static implicit operator StringPtr(string Source) => new(Source);

    /// <summary>Оператор неявного преобразования фрагмента строки в строку</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    /// <example>
    /// <![CDATA[
    /// string value = new StringPtr("value");
    /// ]]>
    /// </example>
    public static implicit operator string(StringPtr Ptr) => Ptr.ToString();

    /// <summary>Оператор явного преобразования фрагмента строки в целое число</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    /// <returns>Целое число</returns>
    /// <example>
    /// <![CDATA[
    /// var value = (int)new StringPtr("42");
    /// ]]>
    /// </example>
    public static explicit operator int(StringPtr Ptr) => Ptr.ParseInt32();

    /// <summary>Оператор явного преобразования фрагмента строки в целое число</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    /// <returns>Целое число или <c>null</c></returns>
    /// <example>
    /// <![CDATA[
    /// int? value = (int?)new StringPtr("42");
    /// ]]>
    /// </example>
    public static explicit operator int?(StringPtr Ptr) => Ptr.ParseInt32();

    /// <summary>Оператор явного преобразования фрагмента строки в вещественное число</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    /// <returns>Вещественное число</returns>
    /// <example>
    /// <![CDATA[
    /// var value = (double)new StringPtr("3.5");
    /// ]]>
    /// </example>
    public static explicit operator double(StringPtr Ptr) => Ptr.ParseDouble();

    /// <summary>Оператор явного преобразования фрагмента строки в вещественное число</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    /// <returns>Вещественное число или <c>null</c></returns>
    /// <example>
    /// <![CDATA[
    /// double? value = (double?)new StringPtr("3.5");
    /// ]]>
    /// </example>
    public static explicit operator double?(StringPtr Ptr) => Ptr.TryParseDouble();

    /// <summary>Оператор явного преобразования фрагмента строки в булево значение</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    /// <returns>Булево значение</returns>
    /// <example>
    /// <![CDATA[
    /// var value = (bool)new StringPtr("true");
    /// ]]>
    /// </example>
    public static explicit operator bool(StringPtr Ptr) => (bool?)Ptr ?? throw new FormatException("Строка имела неверный формат");

    /// <summary>Оператор явного преобразования фрагмента строки в булево значение</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    /// <returns>Булево значение или <c>null</c></returns>
    /// <example>
    /// <![CDATA[
    /// bool? value = (bool?)new StringPtr("true");
    /// ]]>
    /// </example>
    public static explicit operator bool?(StringPtr Ptr)
    {
        var ptr = Ptr.Trim();
        if (ptr.Equals(bool.TrueString, StringComparison.InvariantCulture))
            return true;
        if (ptr.Equals(bool.FalseString, StringComparison.InvariantCulture))
            return false;
        return null;
    }
}
