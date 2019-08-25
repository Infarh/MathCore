using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using MathCore.Annotations;
using MathCore.Expressions.Complex;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable MissingAnnotation

namespace MathCore
{
    /// <summary>Комплексное число</summary>
    [Serializable]
    [TypeConverter(typeof(ComplexConverter))]
    public partial struct Complex : ICloneable<Complex>, IFormattable, IEquatable<Complex>,
        IEquatable<float>, IEquatable<double>,
        IEquatable<byte>, IEquatable<sbyte>,
        IEquatable<short>, IEquatable<ushort>,
        IEquatable<int>, IEquatable<uint>,
        IEquatable<long>, IEquatable<ulong>
    {
        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Метод убирает все парные символы скобок в начале и конце строки</summary>
        /// <param name="str">Очищаемая строка</param>
        private static void ClearString([NotNull] ref string str)
        {
            while (str[0] == '{' && str[str.Length - 1] == '}') str = str.Substring(1, str.Length - 2);
            while (str[0] == '[' && str[str.Length - 1] == ']') str = str.Substring(1, str.Length - 2);
            while (str[0] == '(' && str[str.Length - 1] == ')') str = str.Substring(1, str.Length - 2);
            while (str[0] == '\'' && str[str.Length - 1] == '\'') str = str.Substring(1, str.Length - 2);
            while (str[0] == '"' && str[str.Length - 1] == '"') str = str.Substring(1, str.Length - 2);
            if(str.IndexOf(' ') != -1) str = str.Replace(" ", "");
        }

        /// <summary>Разобрать строку в комплексное число</summary>
        /// <param name="str">Разбираемая строка</param>
        /// <returns>Комплексное число, получаемое в результате разбора строки</returns>
        /// <exception cref="ArgumentNullException">В случае если передана пустая ссылка на строку</exception>
        /// <exception cref="FormatException">В случае ошибочной строки</exception>
        public static Complex Parse([NotNull] string str)
        {
            if(str is null) throw new ArgumentNullException(nameof(str));
            // Если получили пустую строку, то это ошибка преобразования
            if (string.IsNullOrWhiteSpace(str) || str.Length == 0)
                throw new FormatException("Строка имела неверный формат", new ArgumentException(str, nameof(str)));

            //Убираем все начальные и конечные скобки, ковычки и апострофы
            var old_style = str.StartsWith("(");
            ClearString(ref str);

            var values = old_style ? str.Split(';') :str.Split('+', '-'); // Делим строку по знаку + и -

            var Re = 0d; // Аккумулятор действительной части
            var Im = 0d; // Аккумулятор мнимой части

            var index = 0;
            for (var i = 0; i < values.Length; i++)
            {
                var v = values[i]; // Берём очередной элемент из массива элементов
                var v_index = str.IndexOf(values[i], index, StringComparison.Ordinal); // Ищем индекс включения текущего элемента в исходной строке
                index = v_index + v.Length; // Устанавливаем индекс на последний символ текущего элемента в основной строке для того, что бы на следующем цикле искать уже с этого места
                var sign = v_index > 1 && str[v_index - 1] == '-'; // Если мы рассматриваем не первый элемент и символ в основной строке, предшествующий текущему элементу - '-' (минус), то этот элемент носит отрицательное значение
                var is_im = false;
                if (v.IndexOf('i') != -1 || v.IndexOf('j') != -1) // Если в текущем элементе найден символ мнимой единицы
                {                                                 // то...
                    is_im = true;                                 // определяем текущий элемент, как мнимое число
                    v = v.Replace("i", "").Replace("j", "");      // удаляем символ мнимой единицы
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
        public static bool TryParse([CanBeNull] string str, out Complex z)
        {
            // Если получили пустую строку, то это ошибка преобразования
            if (string.IsNullOrWhiteSpace(str) || str.Length == 0)
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
            for (var i = 0; i < values.Length; i++)
            {
                var v = values[i]; // Берём очередной элемент из массива элементов
                var v_index = str.IndexOf(values[i], index, StringComparison.Ordinal); // Ищем индекс включения текущего элемента в исходной строке
                index = v_index + v.Length; // Устанавливаем индекс на последний символ текущего элемента в основной строке для того, что бы на следующем цикле искать уже с этого места
                var sign = v_index > 1 && str[v_index - 1] == '-'; // Если мы рассматриваем не первый элемент и символ в основной строке, предшествующий текущему элементу - '-' (минус), то этот элемент носит отрицательное значение
                var is_im = false;
                if (v.IndexOf('i') != -1 || v.IndexOf('j') != -1) // Если в текущем элементе найден символ мнимой единицы
                {                                                 // то...
                    is_im = true;                                 // определяем текущий элемент, как мнимое число
                    v = v.Replace("i", "").Replace("j", "");      // удаляем символ мнимой единицы
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
            => new Complex(Math.Log(Im), Math.Abs(Im).Equals(0d) ? 0 : (Im > 0 ? Consts.pi05 : -Consts.pi05));

        ///<summary>НАтуральный логорифм комплексного числа</summary>
        ///<param name="z">Комплексное число</param>
        ///<returns>Натуральный логорифм</returns>
        public static Complex Ln(in Complex z) => new Complex(.5 * Math.Log(z._Re * z._Re + z._Im * z._Im), z.Arg);

        ///<summary>Логорифм мномого числа по действительному основанию</summary>
        ///<param name="Im">Мнимое число</param>
        ///<param name="b">Действительное основание логорифма</param>
        ///<returns>Логорифм мнимого числа по действительному основанию</returns>
        public static Complex Log(double Im, double b) => new Complex(
            Math.Log(Im, b),
            Math.Abs(Im) < double.Epsilon
                ? 0
                : (Im > 0
                      ? Consts.pi05
                      : -Consts.pi05) * Math.Log(Math.E, b));

        /// <summary>Логарифм комплексного числа по действительному аргументу</summary>
        /// <param name="z">Комплексное число</param>
        /// <param name="b">Действительное основание логорифма</param>
        /// <returns>Логорифм комплексного числа по действительному основанию</returns>
        public static Complex Log(in Complex z, double b)
            => new Complex(.5 * Math.Log(z._Re * z._Re + z._Im * z._Im, b), z.Arg * Math.Log(Math.E, b));

        /// <summary>Экспоненциальная форма числа Z = e^j*Arg</summary>
        /// <param name="Arg">Аргумент</param>
        /// <returns>Комплексное число в экспоненциальной форме записи</returns>
        public static Complex Exp(double Arg) => new Complex(Math.Cos(Arg), Math.Sin(Arg));

        /// <summary>Экспоненциальная форма числа Z = Abs * e^j*Arg</summary>
        /// <param name="Abs">Модуль числа</param>
        /// <param name="Arg">Аргумент числа</param>
        /// <returns>Комплексное число в экспоненциальной форме</returns>
        public static Complex Exp(double Abs, double Arg) => new Complex(Abs * Math.Cos(Arg), Abs * Math.Sin(Arg));

        /// <summary>Экспонента с комплексным показателем Z = e^(re + j*im) = e^re * [cos(im) + j*sin(im)]</summary>
        /// <param name="z">Комплексный показатель степени экспоненты</param>
        /// <returns>Результат вычисления комплексной экспоненты</returns>
        public static Complex Exp(in Complex z)
        {
            var e = Math.Exp(z.Re);
            return new Complex(e * Math.Cos(z.Im), e * Math.Sin(z.Im));
        }

        ///// <summary>Алгебраическая форма записи комплексного числа</summary>
        ///// <param name="Re">Действительная часть числа</param>
        ///// <returns>Комплексное число в алгебраической форме записи</returns>
        //public static Complex Mod(double Re) { return new Complex(Re); }

        /// <summary>Алгебраическая форма записи комплексного числа</summary>
        /// <param name="Re">Действительная часть числа</param>
        /// <param name="Im">Мнимиая часть числа</param>
        /// <returns>Комплексное число в алгебраической форме записи</returns>
        public static Complex Mod(double Re, double Im = 0) => new Complex(Re, Im);

        /// <summary>Алгебраическая форма записи комплексного числа</summary>
        /// <param name="Re">Действительная часть числа</param>
        /// <param name="Im">Мнимиая часть числа</param>
        /// <returns>Комплексное число в алгебраической форме записи</returns>
        public static Complex Mod(double Re, in Complex Im) => new Complex(Re + Im.Re, Im.Im);

        /// <summary>Алгебраическая форма записи комплексного числа</summary>
        /// <param name="Re">Действительная часть числа</param>
        /// <param name="Im">Мнимиая часть числа</param>
        /// <returns>Комплексное число в алгебраической форме записи</returns>
        public static Complex Mod(in Complex Re, in Complex Im) => new Complex(Re.Re + Im.Re, Re.Im + Im.Im);

        /// <summary>Алгебраическая форма записи комплексного числа</summary>
        /// <param name="Re">Действительная часть числа</param>
        /// <param name="Im">Мнимиая часть числа</param>
        /// <returns>Комплексное число в алгебраической форме записи</returns>
        public static Complex Mod(in Complex Re, double Im) => new Complex(Re.Re, Re.Im + Im);

        /// <summary>Действительное "комплексное" число</summary>
        public static readonly Complex Real = new Complex(1);

        /// <summary>Не-число</summary>
        public static readonly Complex NaN = new Complex(double.NaN, double.NaN);

        /// <summary>Мнимая единица</summary>
        public static readonly Complex i = new Complex(0, 1);

        /// <summary>Создать массив комплексных чисел</summary>
        /// <param name="Re">Массив действительных чисел</param>
        /// <param name="Im">Массив мнимых чисел</param>
        /// <returns>Массив комплексных чисел</returns>
        /// <exception cref="ArgumentNullException"><paramref name="Re"/> or <paramref name="Im"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">Длины массивов не совпадают</exception>
        [NotNull]
        public static Complex[] CreateArray([NotNull] double[] Re, [NotNull] double[] Im)
        {
            if (Re is null) throw new ArgumentNullException(nameof(Re));
            if (Im is null) throw new ArgumentNullException(nameof(Im));
            if (Re.Length != Im.Length) throw new InvalidOperationException(@"Длины массивов не совпадают");

            var result = new Complex[Re.Length];
            for (var j = 0; j < result.Length; j++) result[j] = new Complex(Re[j], Im[j]);
            return result;
        }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Действительная часть</summary>
        private readonly double _Re;

        /// <summary>Мнимая часть</summary>
        private readonly double _Im;

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Преобразование комплексного числа в выражение</summary>
        public CopmlexConstantExpression Expression => ComplexExpression.Mod(_Re, _Im);

        /// <summary>Действительная часть</summary>
        public double Re => _Re;

        /// <summary>Мнимая часть</summary>
        public double Im => _Im;

        /// <summary>X * X^* = Re(X)^2 + Im(X)^2</summary>
        [XmlIgnore]
        public double Power => _Re * _Re + _Im * _Im;

        /// <summary>Модуль</summary>
        [XmlIgnore]
        public double Abs
        {
            get
            {
                if (double.IsInfinity(_Re) || double.IsInfinity(_Im))
                    return double.PositiveInfinity;

                // |value| == sqrt(a^2 + b^2)
                // sqrt(a^2 + b^2) == a/a * sqrt(a^2 + b^2) = a * sqrt(a^2/a^2 + b^2/a^2) = a * sqrt(1 + b^2/a^2)
                // Using the above we can factor out the square of the larger component to dodge overflow.


                var re = Math.Abs(_Re);
                var im = Math.Abs(_Im);

                if (re > im)
                {
                    var ir = im / re;
                    return re * Math.Sqrt(1d + ir * ir);
                }
                if (im.Equals(0.0))
                    return re; // re is either 0.0 or NaN
                var r = re / im;
                return im * Math.Sqrt(1d + r * r);
            }
        }

        /// <summary>Аргумент</summary>
        [XmlIgnore]
        public double Arg => _Re.Equals(0)
                    ? (_Im.Equals(0) // Re == 0
                        ? 0 //  Im == 0 => 0
                        : Math.Sign(_Im) * Consts.pi05) //  Im != 0 => pi/2 * sign(Im)
                    : (_Im.Equals(0) // Re != 0
                        ? (Math.Sign(_Re) > 0
                            ? 0
                            : Consts.pi)
                        : Math.Atan2(_Im, _Re)); //  Im != 0 => atan(Im/Re)

        /// <summary>Комплексно сопряжённое число</summary>
        [XmlIgnore]
        public Complex ComplexConjugate => new Complex(Re, -Im);

        /// <summary>Обратное значение 1/Z</summary>
        public Complex Reciprocal => _Re.Equals(0d) && _Im.Equals(0d)
            ? new Complex(double.PositiveInfinity, double.PositiveInfinity)
            : i / this;

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

        /// <summary>Представление комплексного числа в текстовой экспонетциальной форме</summary>
        /// <returns>Текстовое экспонетциальное представление комплексного числа</returns>
        [NotNull]
        [DST]
        public string ToString_Exponent()
        {
            var abs = Abs;
            var arg = Arg;
            return abs.Equals(1)
                ? arg.Equals(0) ? "1" : $"e{{i{Arg}}}"
                : arg.Equals(0) ? abs.ToString(CultureInfo.CurrentCulture) : $"{Abs}e{{i{Arg}}}";
        }

        /// <summary>Представление комплексного числа в текстовой экспонетциальной форме с нормировкой аргумента к значению pi</summary>
        /// <returns>Текстовое экспонетциальное представление комплексного числа с нормировкой аргумента к значению pi</returns>
        [NotNull]
        [DST]
        public string ToString_Exponent_pi()
        {
            var abs = Abs;
            var arg = Arg / Consts.pi;
            return abs.Equals(1)
                ? arg.Equals(0) ? "1" : $"e{{i{Arg}pi}}"
                : arg.Equals(0) ? abs.ToString(CultureInfo.CurrentCulture) : $"{Abs}e{{i{Arg}pi}}";
        }

        /// <summary>Представление комплексного числа в текстовой экспонетциальной форме с нормировкой аргумента в градусах</summary>
        /// <returns>Текстовое экспонетциальное представление комплексного числа с нормировкой аргумента в градусах</returns>
        [NotNull]
        [DST]
        public string ToString_Exponent_Deg()
        {
            var abs = Abs;
            var arg = Arg.ToDeg();
            return abs.Equals(1)
                ? arg.Equals(0) ? "1" : $"e{{i{Arg}deg}}"
                : arg.Equals(0) ? abs.ToString(CultureInfo.CurrentCulture) : $"{Abs}e{{i{Arg}deg}}";
        }

        /// <summary>Строковый эквивалент</summary>
        /// <returns>Строковый эквивалент</returns>
        [DST]
        public override string ToString()
        {
            if (Math.Abs(Re) < double.Epsilon && Math.Abs(Im) < double.Epsilon) return "0";
            var re = Re.ToString(CultureInfo.CurrentCulture);
            var im = $"{(Math.Abs(Math.Abs(Im) - 1) > double.Epsilon ? Math.Abs(Im).ToString(CultureInfo.CurrentCulture) : "")}i";
            if (Im < 0) im = $"-{im}";
            return $"{(Math.Abs(Re) > double.Epsilon ? $"{re}{(Im > 0 ? "+" : "")}" : "")}{(Math.Abs(Im) > double.Epsilon ? im : "")}";
        }

        /// <summary>Преобразование в строковый формат</summary>
        /// <param name="Format">Формат преобразования</param>
        /// <returns>Строковое представление</returns>
        [DST]
        public string ToString(string Format)
        {
            if (Math.Abs(Re) < double.Epsilon && Math.Abs(Im) < double.Epsilon) return "0";
            var re = Re.ToString(Format);
            var im = $"{(Math.Abs(Math.Abs(Im) - 1) > double.Epsilon ? Math.Abs(Im).ToString(Format) : "")}i";
            if (Im < 0) im = $"-{im}";
            return $"{(Math.Abs(Re) > double.Epsilon ? $"{re}{(Im > 0 ? "+" : "")}" : "")}{(Math.Abs(Im) > double.Epsilon ? im : "")}";
        }

        /// <inheritdoc />
        [DST]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (Math.Abs(Re) < double.Epsilon && Math.Abs(Im) < double.Epsilon) return "0";
            var re = Re.ToString(format, formatProvider);
            var im = $"{(Math.Abs(Math.Abs(Im) - 1) > double.Epsilon ? Math.Abs(Im).ToString(format, formatProvider) : "")}i";
            if (Im < 0) im = $"-{im}";
            return $"{(Math.Abs(Re) > double.Epsilon ? $"{re}{(Im > 0 ? "+" : "")}" : "")}{(Math.Abs(Im) > double.Epsilon ? im : "")}";
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
        public Complex Clone() => new Complex(_Re, _Im);

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
        public bool Equals(Complex other) => _Re.Equals(other._Re) && _Im.Equals(other._Im);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое число</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        bool IEquatable<Complex>.Equals(Complex other) => Equals(other);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое значение</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
        public bool Equals(double other) => _Re.Equals(other) && _Im.Equals(0d);

        /// <summary>Проверка на идентичность</summary>
        /// <param name="other">Проверяемое число</param>
        /// <returns>Истина, если числа идентичны</returns>
        [DST]
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
        public bool Equals(float other) => _Re.Equals(other) && _Im.Equals(0d);

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
        public Complex Round(int DigitsCount = 0) => new Complex(Math.Round(_Re, DigitsCount), Math.Round(_Im, DigitsCount));

        /// <summary>Вычисление квадратного корня числа</summary>
        /// <returns>Квадратный корень числа</returns>
        public Complex Sqrt() => this ^ 0.5;

        public static Complex Sqrt(in Complex z) => z.Sqrt();

        public static Complex Sqrt(double x) => x >= 0 ? new Complex(Math.Sqrt(x)) : new Complex(0, Math.Sqrt(-x));
    }
}