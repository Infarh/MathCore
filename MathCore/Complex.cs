#nullable enable
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

using MathCore.Annotations;
using MathCore.Expressions.Complex;

using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable MissingAnnotation
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace MathCore
{
    /// <summary>Комплексное число</summary>
    [Serializable, TypeConverter(typeof(ComplexConverter))]
    public readonly partial struct Complex : ICloneable<Complex>, IFormattable,
                                             IEquatable<Complex>, IEquatable<float>, IEquatable<double>,
                                             IEquatable<byte>, IEquatable<sbyte>, IEquatable<short>,
                                             IEquatable<ushort>, IEquatable<int>, IEquatable<uint>,
                                             IEquatable<long>, IEquatable<ulong>, IEquatable<(double Re, double Im)>
    {
        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Метод убирает все парные символы скобок в начале и конце строки</summary>
        /// <param name="str">Очищаемая строка</param>
        private static void ClearString(ref string str)
        {
            while (str[0] == '{' && str[^1] == '}') str = str.Substring(1, str.Length - 2);
            while (str[0] == '[' && str[^1] == ']') str = str.Substring(1, str.Length - 2);
            while (str[0] == '(' && str[^1] == ')') str = str.Substring(1, str.Length - 2);
            while (str[0] == '\'' && str[^1] == '\'') str = str.Substring(1, str.Length - 2);
            while (str[0] == '"' && str[^1] == '"') str = str.Substring(1, str.Length - 2);
            if (str.IndexOf(' ') != -1) str = str.Replace(" ", string.Empty);
        }

        /// <summary>Разобрать строку в комплексное число</summary>
        /// <param name="str">Разбираемая строка</param>
        /// <returns>Комплексное число, получаемое в результате разбора строки</returns>
        /// <exception cref="ArgumentNullException">В случае если передана пустая ссылка на строку</exception>
        /// <exception cref="FormatException">В случае ошибочной строки</exception>
        public static Complex Parse(string str)
        {
            if (str is null) throw new ArgumentNullException(nameof(str));
            // Если получили пустую строку, то это ошибка преобразования
            if (string.IsNullOrWhiteSpace(str) || str.Length == 0)
                throw new FormatException("Строка имела неверный формат", new ArgumentException(str, nameof(str)));

            //Убираем все начальные и конечные скобки, ковычки и апострофы
            var old_style = str.StartsWith("(");
            ClearString(ref str);

            var values = old_style ? str.Split(';') : str.Split('+', '-'); // Делим строку по знаку + и -

            var Re = 0d; // Аккумулятор действительной части
            var Im = 0d; // Аккумулятор мнимой части

            var index = 0;
            for (var j = 0; j < values.Length; j++)
            {
                var v = values[j]; // Берём очередной элемент из массива элементов
                var v_index = str.IndexOf(values[j], index, StringComparison.Ordinal); // Ищем индекс включения текущего элемента в исходной строке
                index = v_index + v.Length; // Устанавливаем индекс на последний символ текущего элемента в основной строке для того, что бы на следующем цикле искать уже с этого места
                var sign = v_index > 1 && str[v_index - 1] == '-'; // Если мы рассматриваем не первый элемент и символ в основной строке, предшествующий текущему элементу - '-' (минус), то этот элемент носит отрицательное значение
                var is_im = false;
                if (v.IndexOf('i') != -1 || v.IndexOf('j') != -1) // Если в текущем элементе найден символ мнимой единицы
                {                                                 // то...
                    is_im = true;                                 // определяем текущий элемент, как мнимое число
                    v = v.Replace("i", string.Empty).Replace("j", string.Empty);      // удаляем символ мнимой единицы
                }
                //var val = double.Parse(v);
                if (!double.TryParse(v, out var val)) // Пытаемся преобразовать текущий элемент в число...
                    throw new FormatException("Строка имела неверный формат", new ArgumentException(str, nameof(str)));
                if (sign) val *= -1;              // Если был найден знак "минус", то это отрицательное значение 
                if (is_im)                        // Если бы найден символ мнимой единицы, то...
                    Im += val;                    // ... это мнимая часть, добавляем её в мнимый аккумулятор
                else                              // иначе...
                    Re += val;                    // ... это действительная часть, добавляем её в действительный аккумулятор.

            }                                     // Когда все компоненты числа перебраны...
            return new Complex(Re, Im);              // Формируем новое комплексное число и возвращаем его в качестве результата операции
        }

        /// <summary>Попытаться разобрать строку и преобразовать её в комплексное число</summary>
        /// <param name="str">Разбираемая строка</param>
        /// <param name="z">Число, получаемое в результате разбора строки</param>
        /// <returns>Истина, если операция разбора строки выполнена успешно</returns>
        public static bool TryParse(string? str, out Complex z)
        {
            // Если получили пустую строку, то это ошибка преобразования
            if (string.IsNullOrWhiteSpace(str) || str!.Length == 0)
            {
                z = default;
                return false;
            }

            //Убираем все начальные и конечные скобки, ковычки и апострофы
            ClearString(ref str);

            var values = str.Split('+', '-'); // Делим строку по знаку + и -

            var Re = 0d; // Аккумулятор действительной части
            var Im = 0d; // Аккумулятор мнимой части

            var index = 0;
            for (var j = 0; j < values.Length; j++)
            {
                var v = values[j]; // Берём очередной элемент из массива элементов
                var v_index = str.IndexOf(values[j], index, StringComparison.Ordinal); // Ищем индекс включения текущего элемента в исходной строке
                index = v_index + v.Length; // Устанавливаем индекс на последний символ текущего элемента в основной строке для того, что бы на следующем цикле искать уже с этого места
                var sign = v_index > 1 && str[v_index - 1] == '-'; // Если мы рассматриваем не первый элемент и символ в основной строке, предшествующий текущему элементу - '-' (минус), то этот элемент носит отрицательное значение
                var is_im = false;
                if (v.IndexOf('i') != -1 || v.IndexOf('j') != -1) // Если в текущем элементе найден символ мнимой единицы
                {                                                 // то...
                    is_im = true;                                 // определяем текущий элемент, как мнимое число
                    v = v.Replace("i", string.Empty).Replace("j", string.Empty);      // удаляем символ мнимой единицы
                }
                //var val = double.Parse(v);
                if (!double.TryParse(v, out var val)) // Пытаемся преобразовать текущий элемент в число...
                {                                     // Если не получилось, то
                    z = default;             // результат - значение по умолчанию
                    return false;                     // результат операции - ложь. Выходим.
                }                                     // Если получилось...
                if (sign) val *= -1;              // Если был найден знак "минус", то это отрицательное значение 
                if (is_im)                        // Если бы найден символ мнимой единицы, то...
                    Im += val;                    // ... это мнимая часть, добавляем её в мнимый аккумулятор
                else                              // иначе...
                    Re += val;                    // ... это действительная часть, добавляем её в действительный аккумулятор.

            }                                     // Когда все компоненты числа перебраны...
            z = new Complex(Re, Im);              // Формируем новое комплексное число и возвращаем его в качестве результата операции
            return true;                          // Операция выполнена успешно.
        }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Точность вычисления тригонометрических функций 3e-16 </summary>
        /// <remarks>Определено приближённо</remarks>
        public const double Epsilon = 3.0e-16;

        /// <summary>Логарифм комплексного аргумента</summary>
        /// <param name="Im">Комплексный аргумент</param>
        /// <returns>Значение логарифма</returns>
        public static Complex Ln(double Im)
            => new(Math.Log(Im), Math.Abs(Im) == 0 ? 0 : (Im > 0 ? Consts.pi05 : -Consts.pi05));

        ///<summary>Натуральный логогриф комплексного числа</summary>
        ///<param name="z">Комплексное число</param>
        ///<returns>Натуральный логарифм</returns>
        public static Complex Ln(in Complex z) => new(.5 * Math.Log(z._Re * z._Re + z._Im * z._Im), z.Arg);

        ///<summary>Логогриф мнимого числа по действительному основанию</summary>
        ///<param name="Im">Мнимое число</param>
        ///<param name="b">Действительное основание логарифма</param>
        ///<returns>Логарифм мнимого числа по действительному основанию</returns>
        public static Complex Log(double Im, double b) => new(
            Math.Log(Im, b),
            Im == 0
                ? 0
                : (Im > 0
                      ? Consts.pi05
                      : -Consts.pi05) * Math.Log(Math.E, b));

        /// <summary>Логарифм комплексного числа по действительному аргументу</summary>
        /// <param name="z">Комплексное число</param>
        /// <param name="b">Действительное основание логарифма</param>
        /// <returns>Логарифм комплексного числа по действительному основанию</returns>
        public static Complex Log(in Complex z, double b)
            => new(.5 * Math.Log(z._Re * z._Re + z._Im * z._Im, b), z.Arg * Math.Log(Math.E, b));

        /// <summary>Экспоненциальная форма числа Z = e^j*Arg</summary>
        /// <param name="Arg">Аргумент</param>
        /// <returns>Комплексное число в экспоненциальной форме записи</returns>
        public static Complex Exp(double Arg) => new(Math.Cos(Arg), Math.Sin(Arg));

        /// <summary>Экспоненциальная форма числа Z = Abs * e^j*Arg</summary>
        /// <param name="Abs">Модуль числа</param>
        /// <param name="Arg">Аргумент числа</param>
        /// <returns>Комплексное число в экспоненциальной форме</returns>
        public static Complex Exp(double Abs, double Arg) => new(Abs * Math.Cos(Arg), Abs * Math.Sin(Arg));

        /// <summary>Экспонента с комплексным показателем Z = e^(re + j*im) = e^re * [cos(im) + j*sin(im)]</summary>
        /// <param name="z">Комплексный показатель степени экспоненты</param>
        /// <returns>Результат вычисления комплексной экспоненты</returns>
        public static Complex Exp(in Complex z)
        {
            var (re, im) = z;
            var e = Math.Exp(re);
            return new Complex(e * Math.Cos(im), e * Math.Sin(im));
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
        public static Complex Mod(double Re, in Complex Im) => new(Re + Im.Re, Im.Im);

        /// <summary>Алгебраическая форма записи комплексного числа</summary>
        /// <param name="Re">Действительная часть числа</param>
        /// <param name="Im">Мнимая часть числа</param>
        /// <returns>Комплексное число в алгебраической форме записи</returns>
        public static Complex Mod(in Complex Re, in Complex Im) => new(Re.Re + Im.Re, Re.Im + Im.Im);

        /// <summary>Алгебраическая форма записи комплексного числа</summary>
        /// <param name="Re">Действительная часть числа</param>
        /// <param name="Im">Мнимая часть числа</param>
        /// <returns>Комплексное число в алгебраической форме записи</returns>
        public static Complex Mod(in Complex Re, double Im) => new(Re.Re, Re.Im + Im);

        /// <summary>Действительное число</summary>
        /// <param name="re">Значение действительной части числа</param>
        /// <returns>Комплексное число Re + j0</returns>
        public static Complex ReValue(double re) => new(re);

        /// <summary>Мнимое число</summary>
        /// <param name="im">Значение мнимой части числа</param>
        /// <returns>Комплексное число 0 + jIm</returns>
        public static Complex ImValue(double im) => new(0, im);

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
            if (Re is null) throw new ArgumentNullException(nameof(Re));
            if (Im is null) throw new ArgumentNullException(nameof(Im));
            if (Re.Length != Im.Length) throw new InvalidOperationException(@"Длины массивов не совпадают");

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
        public Complex Reciprocal => _Re.Equals(0d) && _Im.Equals(0d)
            ? new Complex(double.PositiveInfinity, double.PositiveInfinity)
            : i / this;

        public bool IsNaN => double.IsNaN(_Re) || double.IsNaN(_Im);

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Комплексное число</summary>
        /// <param name="Re">Действительная часть</param>
        /// <param name="Im">Мнимая часть</param>
        [DST]
        public Complex(double Re, double Im = 0)
        {
            _Re = Re;
            _Im = Im;
        }

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
            var re_str = re.ToString(CultureInfo.CurrentCulture);
            var im_str = $"{(Math.Abs(im) != 1 ? Math.Abs(im).ToString(CultureInfo.CurrentCulture) : string.Empty)}i";
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
            var re_str = re.ToString(Format);
            var im_str = $"{(Math.Abs(im) != 1 ? Math.Abs(im).ToString(Format) : string.Empty)}i";
            if (im < 0) im_str = $"-{im_str}";
            return $"{(re != 0 ? $"{re_str}{(im > 0 ? "+" : string.Empty)}" : string.Empty)}{(im != 0 ? im_str : string.Empty)}";
        }

        [DST]
        public string ToString(IFormatProvider FormatProvider)
        {
            var re = Re;
            var im = Im;
            if (re == 0 && im == 0) return "0";
            var re_str = re.ToString(FormatProvider);
            var im_str = $"{(Math.Abs(im) != 1 ? Math.Abs(im).ToString(FormatProvider) : string.Empty)}i";
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
            var re_str = re.ToString(format, FormatProvider);
            var im_str = $"{(Math.Abs(im) != 1 ? Math.Abs(im).ToString(format, FormatProvider) : string.Empty)}i";
            if (im < 0) im_str = $"-{im_str}";
            return $"{(re != 0 ? $"{re_str}{(im > 0 ? "+" : string.Empty)}" : string.Empty)}{(im != 0 ? im_str : string.Empty)}";
        }

        /// <inheritdoc />
        [DST]
        public override int GetHashCode()
        {
            unchecked
            {
                return (_Re.GetHashCode() * 0x18d) ^ _Im.GetHashCode();
            }
        }

        /// <summary>Получение клона</summary>
        /// <returns>Клон числа</returns>
        [DST]
        public Complex Clone() => new(_Re, _Im);

        /// <summary>Получение клона</summary>
        /// <returns>Клон числа</returns>
        [DST]
        object ICloneable.Clone() => Clone();


        /// <inheritdoc />
        [DST]
        public override bool Equals(object obj) => obj is Complex z && Equals(z);

        #region IEquatable Members

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое значение</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        public bool Equals(Complex other) => (IsNaN && other.IsNaN) || _Re.Equals(other._Re) && _Im.Equals(other._Im);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое число</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        bool IEquatable<Complex>.Equals(Complex other) => Equals(other);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое значение</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        public bool Equals(double other) => (double.IsNaN(other) && IsNaN) || _Re.Equals(other) && _Im.Equals(0d);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое число</param>
        /// <returns>Истина, если числа идентичны</returns>
        //[DST]
        bool IEquatable<double>.Equals(double other) => Equals(other);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое значение</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        public bool Equals(short other) => _Re.Equals(other) && _Im.Equals(0d);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое число</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        bool IEquatable<short>.Equals(short other) => Equals(other);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое значение</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        public bool Equals(ushort other) => _Re.Equals(other) && _Im.Equals(0d);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое число</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        bool IEquatable<ushort>.Equals(ushort other) => Equals(other);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое значение</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        public bool Equals(byte other) => _Re.Equals(other) && _Im.Equals(0d);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое число</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        bool IEquatable<byte>.Equals(byte other) => Equals(other);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое значение</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        public bool Equals(sbyte other) => _Re.Equals(other) && _Im.Equals(0d);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое число</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        bool IEquatable<sbyte>.Equals(sbyte other) => Equals(other);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое значение</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        public bool Equals(float other) => (float.IsNaN(other) && IsNaN) || _Re.Equals(other) && _Im.Equals(0d);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое число</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        bool IEquatable<float>.Equals(float other) => Equals(other);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое значение</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        public bool Equals(int other) => _Re.Equals(other) && _Im.Equals(0d);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое число</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        bool IEquatable<int>.Equals(int other) => Equals(other);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое значение</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        public bool Equals(uint other) => _Re.Equals(other) && _Im.Equals(0d);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое число</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        bool IEquatable<uint>.Equals(uint other) => Equals(other);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое значение</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        public bool Equals(long other) => _Re.Equals(other) && _Im.Equals(0d);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое число</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        bool IEquatable<long>.Equals(long other) => Equals(other);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое значение</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        public bool Equals(ulong other) => _Re.Equals(other) && _Im.Equals(0d);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое число</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        bool IEquatable<ulong>.Equals(ulong other) => Equals(other);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Кортеж двух вещественных чисел</param>
        /// <returns>Истина, если действительная и мнимая части идентичны</returns>
        bool IEquatable<(double Re, double Im)>.Equals((double Re, double Im) other) => 
            (IsNaN && (double.IsNaN(other.Re) || double.IsNaN(other.Im))) 
            || _Re.Equals(other.Re) && _Im.Equals(other.Im);

        #endregion

        /// <summary>Поворот вектора комплексного числа на угол</summary>
        /// <param name="w">Угол поворота вектора в комплексной плоскости</param>
        /// <returns>Комплексное число, повёрнутое на угол</returns>
        [DST]
        public Complex Rotate(double w)
        {
            var sin = Math.Sin(w);
            var cos = Math.Cos(w);
            var re = _Re;
            var im = _Im;

            return new Complex(re * cos - im * sin, re * cos + im * sin);
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
        public static Complex Sqrt(in Complex z) => z.Sqrt();

        /// <summary>Вычисление корня действительной степени</summary>
        /// <param name="x">Действительная степень корня</param>
        /// <returns>Комплексный результат вычисления корня действительной степени от комплексного числа</returns>
        [DST]
        public static Complex Sqrt(double x) => x >= 0 ? new Complex(Math.Sqrt(x)) : new Complex(0, Math.Sqrt(-x));
    }
}