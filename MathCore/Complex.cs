#nullable enable
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

using MathCore.Expressions.Complex;

using static System.Math;

// ReSharper disable MissingAnnotation
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace MathCore;

/// <summary>Комплексное число</summary>
[Serializable, TypeConverter(typeof(ComplexConverter))]
public readonly partial struct Complex : ICloneable<Complex>, IFormattable,
                                         IEquatable<Complex>, IEquatable<float>, IEquatable<double>,
                                         IEquatable<byte>, IEquatable<sbyte>, IEquatable<short>,
                                         IEquatable<ushort>, IEquatable<int>, IEquatable<uint>,
                                         IEquatable<long>, IEquatable<ulong>, IEquatable<(double Re, double Im)>,
                                         IEquatable<(int Re, double Im)>, IEquatable<(double Re, int Im)>
{
    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Метод убирает все парные символы скобок в начале и конце строки</summary>
    /// <param name="str">Очищаемая строка</param>
    private static StringPtr ClearStringPtr(StringPtr str)
    {
        var start_len = str.Length;
        while(start_len > 0)
        {
            while (str.IsInBracket('{', '}')) str   = str.Substring(1, str.Length - 2);
            while (str.IsInBracket('[', ']')) str   = str.Substring(1, str.Length - 2);
            while (str.IsInBracket('(', ')')) str   = str.Substring(1, str.Length - 2);
            while (str.IsInBracket('"', '"')) str   = str.Substring(1, str.Length - 2);
            while (str.IsInBracket('\'', '\'')) str = str.Substring(1, str.Length - 2);

            var len = str.Length;
            start_len = len == start_len ? 0 : len;
        }

        return str;
    }

    /// <summary>Разобрать строку в комплексное число</summary>
    /// <param name="str">Разбираемая строка</param>
    /// <returns>Комплексное число, получаемое в результате разбора строки</returns>
    /// <exception cref="ArgumentNullException">В случае если передана пустая ссылка на строку</exception>
    /// <exception cref="FormatException">В случае ошибочной строки</exception>
    public static Complex Parse(string str) =>
        TryParse(str ?? throw new ArgumentNullException(nameof(str)), out var result)
            ? result
            : throw new FormatException("Строка имела неверный формат", new ArgumentException(str, nameof(str)));


    /// <summary>Попытаться разобрать строку и преобразовать её в комплексное число</summary>
    /// <param name="str">Разбираемая строка</param>
    /// <param name="z">Число, получаемое в результате разбора строки</param>
    /// <returns>Истина, если операция разбора строки выполнена успешно</returns>
    public static bool TryParse(string? str, out Complex z)
    {
        // Если получили пустую строку, то это ошибка преобразования
        if (str is { Length: > 0 }) 
            return TryParse(new StringPtr(str), out z);

        z = default;
        return false;

    }

    public static bool TryParse(StringPtr str, out Complex z)
    {
        var str_ptr = ClearStringPtr(str);

        var values_ptr = str_ptr.Split(true, '+', '-');

        var Re = 0d; // Аккумулятор действительной части
        var Im = 0d; // Аккумулятор мнимой части

        foreach (var v in values_ptr)
        {
            if (v.Length == 0) continue;

            var    sign = v.Pos > 1 && v[-1] == '-';
            double val;

            var is_im = false;
            if (v.StartWith('i') || v.StartWith('j'))
            {
                is_im = true;
                if (!v.Substring(1).TryParseDouble(out val))
                {
                    z = default;
                    return false;
                }
            }
            else if (v.EndWith('i') || v.EndWith('j'))
            {
                is_im = true;
                if (!v.Substring(0, v.Length - 1).TryParseDouble(out val))
                {
                    z = default;
                    return false;
                }
            }
            else if (!v.TryParseDouble(out val))
            {
                z = default;
                return false;
            }

            if (is_im)
                Im += sign ? -val : val;
            else
                Re += sign ? -val : val;
        }

        z = new Complex(Re, Im);
        return true;
    }

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Точность вычисления тригонометрических функций 3e-16 </summary>
    /// <remarks>Определено приближённо</remarks>
    public const double Epsilon = 3.0e-16;

    /// <summary>Логарифм комплексного аргумента</summary>
    /// <param name="Im">Комплексный аргумент</param>
    /// <returns>Значение логарифма</returns>
    public static Complex Ln(double Im) => new
    (
        Re: Math.Log(Im), 
        Im: Abs(Im) == 0 ? 0 : (Im > 0 ? Consts.pi05 : -Consts.pi05)
    );

    ///<summary>Натуральный логогриф комплексного числа</summary>
    ///<param name="z">Комплексное число</param>
    ///<returns>Натуральный логарифм</returns>
    public static Complex Ln(Complex z) => new
    (
        Re: .5 * Math.Log(z._Re * z._Re + z._Im * z._Im),
        Im: z.Arg
    );

    ///<summary>Логогриф мнимого числа по действительному основанию</summary>
    ///<param name="Im">Мнимое число</param>
    ///<param name="b">Действительное основание логарифма</param>
    ///<returns>Логарифм мнимого числа по действительному основанию</returns>
    public static Complex Log(double Im, double b) => new
    (
        Re: Math.Log(Im, b),
        Im: Im switch
        {
            > 0 => +Consts.pi05 * Math.Log(E, b),
            < 0 => -Consts.pi05 * Math.Log(E, b),
            _   => 0
        }
    );

    /// <summary>Логарифм комплексного числа по действительному аргументу</summary>
    /// <param name="z">Комплексное число</param>
    /// <param name="b">Действительное основание логарифма</param>
    /// <returns>Логарифм комплексного числа по действительному основанию</returns>
    public static Complex Log(Complex z, double b) => new
    (
        Re: 0.5   * Math.Log(z._Re * z._Re + z._Im * z._Im, b),
        Im: z.Arg * Math.Log(E, b)
    );

    /// <summary>Экспоненциальная форма числа Z = e^j*Arg</summary>
    /// <param name="Arg">Аргумент</param>
    /// <returns>Комплексное число в экспоненциальной форме записи</returns>
    public static Complex Exp(double Arg) => new(Cos(Arg), Sin(Arg));

    /// <summary>Экспоненциальная форма числа Z = Abs * e^j*Arg</summary>
    /// <param name="Abs">Модуль числа</param>
    /// <param name="Arg">Аргумент числа</param>
    /// <returns>Комплексное число в экспоненциальной форме</returns>
    public static Complex Exp(double Abs, double Arg) => new(Abs * Cos(Arg), Abs * Sin(Arg));

    /// <summary>Экспонента с комплексным показателем Z = e^(re + j*im) = e^re * [cos(im) + j*sin(im)]</summary>
    /// <param name="z">Комплексный показатель степени экспоненты</param>
    /// <returns>Результат вычисления комплексной экспоненты</returns>
    public static Complex Exp(Complex z)
    {
        var (re, im) = z;
        var e = Math.Exp(re);
        return new Complex(e * Cos(im), e * Sin(im));
    }

    /// <summary>Алгебраическая форма записи комплексного числа</summary>
    /// <param name="Re">Действительная часть числа</param>
    /// <param name="Im">Мнимая часть числа</param>
    /// <returns>Комплексное число в алгебраической форме записи</returns>
    public static Complex Mod(double Re, double Im = 0) => new(Re, Im);

    /// <summary>Алгебраическая форма записи комплексного числа</summary>
    /// <param name="Re">Действительная часть числа</param>
    /// <param name="Im">Мнимая часть числа</param>
    /// <returns>Комплексное число в алгебраической форме записи</returns>
    public static Complex Mod(double Re, Complex Im) => new(Re + Im.Re, Im.Im);

    /// <summary>Алгебраическая форма записи комплексного числа</summary>
    /// <param name="Re">Действительная часть числа</param>
    /// <param name="Im">Мнимая часть числа</param>
    /// <returns>Комплексное число в алгебраической форме записи</returns>
    public static Complex Mod(Complex Re, Complex Im) => new(Re.Re + Im.Re, Re.Im + Im.Im);

    /// <summary>Алгебраическая форма записи комплексного числа</summary>
    /// <param name="Re">Действительная часть числа</param>
    /// <param name="Im">Мнимая часть числа</param>
    /// <returns>Комплексное число в алгебраической форме записи</returns>
    public static Complex Mod(Complex Re, double Im) => new(Re.Re, Re.Im + Im);

    /// <summary>Действительное число</summary>
    /// <param name="re">Значение действительной части числа</param>
    /// <returns>Комплексное число Re + j0</returns>
    public static Complex ReValue(double re) => new(re);

    /// <summary>Мнимое число</summary>
    /// <param name="im">Значение мнимой части числа</param>
    /// <returns>Комплексное число 0 + jIm</returns>
    public static Complex ImValue(double im) => new(0, im);

    /// <summary>Комплексно-сопряжённые значения</summary>
    /// <returns>Пара комплексно-сопряжённых чисел</returns>
    public (Complex Z, Complex Zconj) Conjugate() => (this, ComplexConjugate);

    /// <summary>Комплексно-сопряжённые значения</summary>
    /// <param name="Re">Действительная часть</param>
    /// <param name="Im">Мнимая часть</param>
    /// <returns>Пара комплексно-сопряжённых чисел</returns>
    public static (Complex Z, Complex Zconj) Conjugate(double Re, double Im) =>
    (
        Z:     new(Re, Im),
        Zconj: new(Re, -Im)
    );

    /// <summary>Комплексно-сопряжённые значения</summary>
    /// <param name="ExpPower">Показатель степени комплексно-сопряжённой пары</param>
    /// <returns>Пара комплексно-сопряжённых чисел</returns>
    public static (Complex Z, Complex Zconj) Conjugate(double ExpPower) =>
        Conjugate(Cos(ExpPower), Sin(ExpPower));

    /// <summary>Комплексно-сопряжённые значения</summary>
    /// <param name="Abs">Модуль комплексно-сопряжённой пары</param>
    /// <param name="ExpPower">Показатель степени комплексно-сопряжённой пары</param>
    /// <returns>Пара комплексно-сопряжённых чисел</returns>
    public static (Complex Z, Complex Zconj) ConjugateExp(double Abs, double ExpPower) => Conjugate
    (
        Re: Abs * Cos(ExpPower),
        Im: Abs * Sin(ExpPower)
    );

    /// <summary>Вычисление синуса и косинуса аргумента</summary>
    /// <param name="arg">Аргумент функции</param>
    public static (double Sin, double Cos) SinCos(double arg) =>
    (
        Sin(arg),
        Cos(arg)
    );

    /// <summary>Вычисление синуса и косинуса аргумента</summary>
    /// <param name="arg">Аргумент функции</param>
    public static (double Sin, double Cos) SinCos(double arg, double abs) =>
    (
        abs * Sin(arg),
        abs * Cos(arg)
    );

    /// <summary>Вычисление синуса и косинуса аргумента</summary>
    /// <param name="arg">Аргумент функции</param>
    public static (Complex Sin, Complex Cos) SinCos(Complex arg) =>
    (
        Trigonometry.Sin(arg),
        Trigonometry.Cos(arg)
    );

    /// <summary>Действительное "комплексное" число</summary>
    public static readonly Complex Real = new(1);

    /// <summary>Не-число</summary>
    public static readonly Complex NaN = new(double.NaN, double.NaN);

    /// <summary>Мнимая единица</summary>
    public static readonly Complex i = new(0, 1);

    /// <summary>Ноль</summary>
    public static readonly Complex Zero = new();

    /// <summary>Создать массив комплексных чисел</summary>
    /// <param name="Re">Массив действительных чисел</param>
    /// <param name="Im">Массив мнимых чисел</param>
    /// <returns>Массив комплексных чисел</returns>
    /// <exception cref="ArgumentNullException"><paramref name="Re"/> or <paramref name="Im"/> is <see langword="null"/></exception>
    /// <exception cref="InvalidOperationException">Длины массивов не совпадают</exception>
    public static Complex[] CreateArray(double[] Re, double[] Im)
    {
        if (Re.NotNull().Length != Im.NotNull().Length)
            throw new InvalidOperationException(@"Длины массивов не совпадают");

        var result = new Complex[Re.Length];
        for (var j = 0; j < result.Length; j++)
            result[j] = new Complex(Re[j], Im[j]);
        return result;
    }

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Действительная часть</summary>
    private readonly double _Re;

    /// <summary>Мнимая часть</summary>
    private readonly double _Im;

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Преобразование комплексного числа в выражение</summary>
    public ComplexConstantExpression Expression => ComplexExpression.Mod(_Re, _Im);

    /// <summary>Действительная часть</summary>
    public double Re { get => _Re; init => _Re = value; }

    /// <summary>Мнимая часть</summary>
    public double Im { get => _Im; init => _Im = value; }

    /// <summary>X * X^* = Re(X)^2 + Im(X)^2</summary>
    [XmlIgnore]
    public double Power => _Re * _Re + _Im * _Im;

    /// <summary>Модуль</summary>
    [XmlIgnore]
    public double Abs => Numeric.Radius(_Re, Im);

    /// <summary>Аргумент</summary>
    [XmlIgnore]
    public double Arg => Numeric.Angle(_Re, _Im);

    /// <summary>Комплексно сопряжённое число</summary>
    [XmlIgnore]
    public Complex ComplexConjugate => new(Re, -Im);

    /// <summary>Обратное значение 1/Z</summary>
    public Complex Reciprocal => _Re == 0 && _Im == 0
        ? new Complex(double.PositiveInfinity, double.PositiveInfinity)
        : i / this;

    public bool IsNaN => double.IsNaN(_Re) || double.IsNaN(_Im);

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Комплексное число</summary>
    /// <param name="Re">Действительная часть</param>
    /// <param name="Im">Мнимая часть</param>
    [DST]
    public Complex(double Re, double Im = 0) => (_Re, _Im) = (Re, Im);

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Представление комплексного числа в текстовой экспоненциальной форме</summary>
    /// <returns>Текстовое экспоненциальное представление комплексного числа</returns>
    [DST]
    public string ToString_Exponent()
    {
        var abs = Abs;
        var arg = Arg;
        return abs == 1
            ? arg == 0 ? "1" : $"e{{i{Arg}}}"
            : arg == 0 ? abs.ToString(CultureInfo.CurrentCulture) : $"{Abs}e{{i{Arg}}}";
    }

    /// <summary>Представление комплексного числа в текстовой экспоненциальной форме с нормировкой аргумента к значению pi</summary>
    /// <returns>Текстовое экспоненциальное представление комплексного числа с нормировкой аргумента к значению pi</returns>
    [DST]
    public string ToString_Exponent_pi()
    {
        var abs = Abs;
        var arg = Arg / Consts.pi;
        return abs == 1
            ? arg == 0 ? "1" : $"e{{i{Arg}pi}}"
            : arg == 0 ? abs.ToString(CultureInfo.CurrentCulture) : $"{Abs}e{{i{Arg}pi}}";
    }

    /// <summary>Представление комплексного числа в текстовой экспоненциальной форме с нормировкой аргумента в градусах</summary>
    /// <returns>Текстовое экспоненциальное представление комплексного числа с нормировкой аргумента в градусах</returns>
    [DST]
    public string ToString_Exponent_Deg()
    {
        var abs = Abs;
        var arg = Arg.ToDeg();
        return abs == 1
            ? arg == 0 ? "1" : $"e{{i{Arg}deg}}"
            : arg == 0 ? abs.ToString(CultureInfo.CurrentCulture) : $"{Abs}e{{i{Arg}deg}}";
    }

    /// <summary>Строковый эквивалент</summary>
    [DST]
    public override string ToString()
    {
        var re = Re;
        var im = Im;
        if (re == 0 && im == 0) return "0";
        var re_str         = re.ToString(CultureInfo.CurrentCulture);
        var im_str         = $"{(Abs(im) != 1 ? Abs(im).ToString(CultureInfo.CurrentCulture) : string.Empty)}i";
        if (im < 0) im_str = $"-{im_str}";
        return $"{(re != 0 ? $"{re_str}{(im > 0 ? "+" : string.Empty)}" : string.Empty)}{(im != 0 ? im_str : string.Empty)}";
    }

    /// <summary>Преобразование в строковый формат</summary>
    /// <param name="Format">Формат преобразования</param>
    /// <returns>Строковое представление</returns>
    [DST]
    public string ToString(string Format)
    {
        var re = Re;
        var im = Im;
        if (re == 0 && im == 0) return "0";
        var re_str         = re.ToString(Format);
        var im_str         = $"{(Abs(im) != 1 ? Abs(im).ToString(Format) : string.Empty)}i";
        if (im < 0) im_str = $"-{im_str}";
        return $"{(re != 0 ? $"{re_str}{(im > 0 ? "+" : string.Empty)}" : string.Empty)}{(im != 0 ? im_str : string.Empty)}";
    }

    [DST]
    public string ToString(IFormatProvider FormatProvider)
    {
        var re = Re;
        var im = Im;
        if (re == 0 && im == 0) return "0";
        var re_str         = re.ToString(FormatProvider);
        var im_str         = $"{(Abs(im) != 1 ? Abs(im).ToString(FormatProvider) : string.Empty)}i";
        if (im < 0) im_str = $"-{im_str}";
        return $"{(re != 0 ? $"{re_str}{(im > 0 ? "+" : string.Empty)}" : string.Empty)}{(im != 0 ? im_str : string.Empty)}";
    }

    /// <inheritdoc />
    [DST]
    public string ToString(string format, IFormatProvider FormatProvider)
    {
        var re = Re;
        var im = Im;
        if (re == 0 && im == 0) return "0";
        var re_str         = re.ToString(format, FormatProvider);
        var im_str         = $"{(Abs(im) != 1 ? Abs(im).ToString(format, FormatProvider) : string.Empty)}i";
        if (im < 0) im_str = $"-{im_str}";
        return $"{(re != 0 ? $"{re_str}{(im > 0 ? "+" : string.Empty)}" : string.Empty)}{(im != 0 ? im_str : string.Empty)}";
    }

    /// <inheritdoc />
    [DST]
    public override int GetHashCode() => unchecked((_Re.GetHashCode() * 0x18d) ^ _Im.GetHashCode());

    /// <summary>Получение клона</summary>
    /// <returns>Клон числа</returns>
    [DST] public Complex Clone() => new(_Re, _Im);

    /// <summary>Получение клона</summary>
    /// <returns>Клон числа</returns>
    [DST] object ICloneable.Clone() => Clone();


    /// <inheritdoc />
    [DST] public override bool Equals(object obj) => obj is Complex z && Equals(z);

    #region IEquatable Members

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое значение</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] public bool Equals(Complex other) => (IsNaN && other.IsNaN) || _Re == other._Re && _Im == other._Im;

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое число</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] bool IEquatable<Complex>.Equals(Complex other) => Equals(other);

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое значение</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] public bool Equals((double Re, double Im) other) => (IsNaN && (double.IsNaN(other.Re) || double.IsNaN(other.Im))) || _Re == other.Re && _Im == other.Im;

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое число</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] bool IEquatable<(double Re, double Im)>.Equals((double Re, double Im) other) => Equals(other);

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое значение</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] public bool Equals((int Re, double Im) other) => (IsNaN && double.IsNaN(other.Im)) || _Re == other.Re && _Im == other.Im;

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое число</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] bool IEquatable<(int Re, double Im)>.Equals((int Re, double Im) other) => Equals(other);

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое значение</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] public bool Equals((double Re, int Im) other) => (IsNaN && double.IsNaN(other.Re)) || _Re == other.Re && _Im == other.Im;

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое число</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] bool IEquatable<(double Re, int Im)>.Equals((double Re, int Im) other) => Equals(other);

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое значение</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] public bool Equals(double other) => (double.IsNaN(other) && IsNaN) || _Re == other && _Im == 0d;

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое число</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] bool IEquatable<double>.Equals(double other) => Equals(other);

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое значение</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] public bool Equals(short other) => _Re == other && _Im == 0d;

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое число</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] bool IEquatable<short>.Equals(short other) => Equals(other);

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое значение</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] public bool Equals(ushort other) => _Re == other && _Im == 0d;

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое число</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] bool IEquatable<ushort>.Equals(ushort other) => Equals(other);

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое значение</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] public bool Equals(byte other) => _Re == other && _Im == 0d;

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое число</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] bool IEquatable<byte>.Equals(byte other) => Equals(other);

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое значение</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] public bool Equals(sbyte other) => _Re == other && _Im == 0d;

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое число</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] bool IEquatable<sbyte>.Equals(sbyte other) => Equals(other);

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое значение</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] public bool Equals(float other) => (float.IsNaN(other) && IsNaN) || _Re == other && _Im == 0d;

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое число</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] bool IEquatable<float>.Equals(float other) => Equals(other);

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое значение</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] public bool Equals(int other) => _Re == other && _Im == 0d;

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое число</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] bool IEquatable<int>.Equals(int other) => Equals(other);

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое значение</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] public bool Equals(uint other) => _Re == other && _Im == 0d;

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое число</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] bool IEquatable<uint>.Equals(uint other) => Equals(other);

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое значение</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] public bool Equals(long other) => _Re == other && _Im == 0d;

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое число</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] bool IEquatable<long>.Equals(long other) => Equals(other);

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое значение</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] public bool Equals(ulong other) => _Re == other && _Im == 0d;

    /// <summary>Проверка на идентичность</summary>
    /// <param name="other">Проверяемое число</param>
    /// <returns>Истина, если числа идентичны</returns>
    [DST] bool IEquatable<ulong>.Equals(ulong other) => Equals(other);

    #endregion

    /// <summary>Поворот вектора комплексного числа на угол</summary>
    /// <param name="w">Угол поворота вектора в комплексной плоскости</param>
    /// <returns>Комплексное число, повёрнутое на угол</returns>
    [DST]
    public Complex Rotate(double w)
    {
        var sin = Sin(w);
        var cos = Cos(w);
        var re  = _Re;
        var im  = _Im;

        return new(re * cos - im * sin, re * cos + im * sin);
    }

    /// <summary>Округление числа</summary>
    /// <param name="DigitsCount">Число разрядов</param>
    /// <returns>Округлённое число</returns>
    [DST]
    public Complex Round(int DigitsCount = 0) => new(Math.Round(_Re, DigitsCount), Math.Round(_Im, DigitsCount));

    /// <summary>Вычисление квадратного корня числа</summary>
    /// <returns>Квадратный корень числа</returns>
    [DST]
    public Complex Sqrt() => this ^ 0.5;

    /// <summary>Вычисление корня комплексной степени</summary>
    /// <param name="z">Комплексная степень корня</param>
    /// <returns>Комплексный результат вычисления корня комплексной степени от комплексного числа</returns>
    [DST]
    public static Complex Sqrt(Complex z) => z.Sqrt();

    /// <summary>Вычисление корня действительной степени</summary>
    /// <param name="x">Действительная степень корня</param>
    /// <returns>Комплексный результат вычисления корня действительной степени от комплексного числа</returns>
    [DST]
    public static Complex Sqrt(double x) => x >= 0 ? new Complex(Math.Sqrt(x)) : new Complex(0, Math.Sqrt(-x));
}