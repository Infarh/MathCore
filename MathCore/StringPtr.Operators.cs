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

    public static bool operator ==(StringPtr ptr, int x) => ptr.Equals(x);

    public static bool operator ==(int x, StringPtr ptr) => ptr == x;

    public static bool operator !=(StringPtr ptr, int x) => !(ptr == x);
    public static bool operator !=(int x, StringPtr ptr) => !(ptr == x);

    public static bool operator ==(StringPtr ptr, double x) => ptr.Equals(x);

    public static bool operator ==(double x, StringPtr ptr) => ptr == x;

    public static bool operator !=(StringPtr ptr, double x) => !(ptr == x);
    public static bool operator !=(double x, StringPtr ptr) => !(ptr == x);

    public static bool operator ==(StringPtr ptr, char c) => ptr.Equals(c);
    public static bool operator ==(char c, StringPtr ptr) => ptr.Equals(c);
    public static bool operator !=(StringPtr ptr, char c) => !(ptr == c);
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
    public static implicit operator StringPtr(string Source) => new(Source);

    /// <summary>Оператор неявного преобразования фрагмента строки в строку</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    public static implicit operator string(StringPtr Ptr) => Ptr.ToString();

    /// <summary>Оператор явного преобразования фрагмента строки в целое число</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    public static explicit operator int(StringPtr Ptr) => Ptr.ParseInt32();
    public static explicit operator int?(StringPtr Ptr) => Ptr.ParseInt32();

    /// <summary>Оператор явного преобразования фрагмента строки в вещественное число</summary>
    /// <param name="Ptr">Фрагмент строки</param>
    public static explicit operator double(StringPtr Ptr) => Ptr.ParseDouble();
    public static explicit operator double?(StringPtr Ptr) => Ptr.TryParseDouble();

    public static explicit operator bool(StringPtr Ptr) => (bool?)Ptr ?? throw new FormatException("Строка имела неверный формат");

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
