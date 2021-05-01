using MathCore;
//using MathCore.Vectors;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable UnusedMember.Global

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Расширения для чисел двойной точности</summary>
    public static class DoubleExtensions
    {
        public static bool EqualWithAccuracy(this double x, double y, double Accuracy = 1.1102230246251565E-15) => x.Check(y, x - y, Accuracy);

        private static bool Check(this double x, double y, double delta, double Accuracy = 1.1102230246251565E-15)
        {
            if (double.IsInfinity(x) || double.IsInfinity(y)) return x.Equals(y);
            if (double.IsNaN(x) || double.IsNaN(y)) return false;
            return Math.Abs(delta) < Math.Abs(Accuracy);
        }

        /// <summary>Модуль числа</summary>
        /// <param name="x">Действительное вещественное число</param>
        /// <returns>Модуль числа</returns>
        [DST] public static double Abs(this double x) => double.IsNaN(x) ? double.NaN : Math.Abs(x);
        [DST] public static float Abs(this float x) => float.IsNaN(x) ? float.NaN : Math.Abs(x);
        [DST] public static decimal Abs(this decimal x) => Math.Abs(x);

        [DST] public static int Abs(this int x) => Math.Abs(x);
        [DST] public static long Abs(this long x) => Math.Abs(x);
        [DST] public static short Abs(this short x) => Math.Abs(x);
        [DST] public static sbyte Abs(this sbyte x) => Math.Abs(x);

        [DST] public static double Abs(this Complex x) => x.Abs;

        /// <summary>Число по модулю</summary>
        /// <param name="x">Исходное число</param>
        /// <param name="mod">Модуль</param>
        /// <returns>Число по модулю</returns>
        [DST] public static double AbsMod(this double x, double mod) => x % mod + (x < 0 ? mod : 0);


        [DST] public static double Sign(this double x) => double.IsNaN(x) ? double.NaN : Math.Sign(x);
        [DST] public static double Round(this double x) => double.IsNaN(x) ? double.NaN : Math.Round(x);
        [DST] public static double Floor(this double x) => double.IsNaN(x) ? double.NaN : Math.Floor(x);
        [DST] public static double Truncate(this double x) => double.IsNaN(x) ? double.NaN : Math.Truncate(x);
        [DST] public static double Ceiling(this double x) => double.IsNaN(x) ? double.NaN : Math.Ceiling(x);

        [DST] public static float Sign(this float x) => float.IsNaN(x) ? float.NaN : Math.Sign(x);
        [DST] public static float Round(this float x) => float.IsNaN(x) ? float.NaN : (float)Math.Round(x);
        [DST] public static float Floor(this float x) => float.IsNaN(x) ? float.NaN : (float)Math.Floor(x);
        [DST] public static float Truncate(this float x) => float.IsNaN(x) ? float.NaN : (float)Math.Truncate(x);
        [DST] public static float Ceiling(this float x) => float.IsNaN(x) ? float.NaN : (float)Math.Ceiling(x);

        [DST]
        public static int Pow(this int x, int p)
        {
            switch (p)
            {
                case < 0: return 1 / x.Pow(-p);
                case 0: return 1;
                case 1: return x;
                case 2: return x * x;
                case 3: return x * x * x;
                case 4: return x * x * x * x;
                default:
                    var result = x;
                    for (var i = 1; i < p; i++)
                        result *= x;
                    return result;
            }
        }

        [DST]
        public static double Pow(this double x, int p)
        {
            if (x is double.NaN) return double.NaN;
            switch (p)
            {
                case < 0: return 1 / x.Pow(-p);
                case 0: return 1;
                case 1: return x;
                case 2: return x * x;
                case 3: return x * x * x;
                case 4: return x * x * x * x;
                default:
                    var result = x;
                    for (var i = 1; i < p; i++)
                        result *= x;
                    return result;
            }
        }

        [DST]
        public static Complex Pow(this Complex x, double p) =>
            x.Re is double.NaN || x.Im is double.NaN
                ? double.NaN
                : p switch
                {
                    double.NaN => Complex.NaN,
                    < 0 => 1 / x.Pow(-p),
                    0 => 1,
                    1 => x,
                    2 => x.Pow2(),
                    _ => x ^ p
                };

        [DST] public static double Pow(this double x, double p) => double.IsNaN(x) ? double.NaN : double.IsNaN(p) ? double.NaN : Math.Pow(x, p);

        [DST] public static double Pow2(this double x) => x * x;
        [DST] public static float Pow2(this float x) => x * x;
        [DST] public static int Pow2(this int value) => value * value;
        [DST] public static uint Pow2(this uint value) => value * value;
        [DST] public static long Pow2(this long value) => value * value;
        [DST] public static ulong Pow2(this ulong value) => value * value;
        [DST] public static short Pow2(this short value) => (short)(value * value);
        [DST] public static ushort Pow2(this ushort value) => (ushort)(value * value);
        [DST] public static byte Pow2(this byte value) => (byte)(value * value);
        [DST] public static byte Pow2(this sbyte value) => (byte)(value * value);

        [DST]
        public static Complex Pow2(in this Complex value)
        {
            var (a, b) = value;
            return new Complex(a * a - b * b, 2 * a * b);
        }

        /// <summary>Квадратный корень</summary>
        /// <param name="x">Число из которого извлекается квадратный корень</param>
        /// <returns>Квадратный корень числа</returns>
        [DST] public static double Sqrt(this double x) => double.IsNaN(x) ? double.NaN : Math.Sqrt(x);

        /// <summary>Квадратный корень</summary>
        /// <param name="x">Число из которого извлекается квадратный корень</param>
        /// <returns>Квадратный корень числа</returns>
        [DST] public static float Sqrt(this float x) => float.IsNaN(x) ? float.NaN : (float)Math.Sqrt(x);

        /// <summary>Является ли число целым?</summary>
        /// <param name="x">Проверяемое число</param>
        /// <returns>Истина, если число целое</returns>
        [DST] public static bool IsInt(this double x) => !double.IsNaN(x) && ((int)x - x).Equals(0d);

        /// <summary>Является ли число целым?</summary>
        /// <param name="x">Проверяемое число</param>
        /// <returns>Истина, если число целое</returns>
        [DST] public static bool IsInt(this float x) => !float.IsNaN(x) && ((int)x - x).Equals(0f);

        /// <summary>Является ли значение "не числом"?</summary>
        /// <param name="x">Проверяемое значение</param>
        /// <returns>Истина, если значение - не число</returns>
        [DST] public static bool IsNaN(this double x) => double.IsNaN(x);

        /// <summary>Является ли значение "не числом"?</summary>
        /// <param name="x">Проверяемое значение</param>
        /// <returns>Истина, если значение - не число</returns>
        [DST] public static bool IsNaN(this float x) => float.IsNaN(x);

        /// <summary>Округление числа до указанного количества знаков после запятой </summary>
        /// <param name="x">Округляемое число</param>
        /// <param name="n">Количество знаков после запятой при n >= 0 и до запятой при n меньше 0</param>
        /// <returns>Число, округлённое до указанной точности</returns>
        [DST]
        public static double Round(this double x, int n)
        {
            if (double.IsNaN(x)) return x;
            if (n >= 0) return Math.Round(x, n);

            var nn = -n;
            var b = Math.Pow(10, nn);
            return Math.Round(x / b) * b;
        }

        /// <summary>Округление числа до указанного количества знаков после запятой </summary>
        /// <param name="x">Округляемое число</param>
        /// <param name="n">Количество знаков после запятой при n >= 0 и до запятой при n меньше 0</param>
        /// <returns>Число, округлённое до указанной точности</returns>
        [DST]
        public static float Round(this float x, int n)
        {
            if (float.IsNaN(x)) return x;
            if (n >= 0) return (float)Math.Round(x, n);

            var nn = -n;
            var b = Math.Pow(10, nn);
            return (float)(Math.Round(x / b) * b);
        }

        //[DST] public static Vector3D RoundAdaptive(this Vector3D r, int n = 1) => new Vector3D(r.X.RoundAdaptive(n), r.Y.RoundAdaptive(n), r.Z.RoundAdaptive(n));


        /// <summary>Адаптивное округление</summary>
        /// <param name="x">Округляемая величина</param>
        /// <param name="n">Количество значащих разрядов</param>
        /// <returns>Число с указанным количеством значащих разрядов</returns>
        [DST]
        public static double RoundAdaptive(this double x, int n = 1)
        {
            if (x.Equals(0d)) return 0;
            if (double.IsNaN(x)) return x;
            if (double.IsInfinity(x)) return x;
            var sign = Math.Sign(x);
            x = x.Abs();
            var b = Math.Pow(10, (int)Math.Log10(x) - 1);
            return Math.Round(x / b, n - 1) * b * sign;
        }

        /// <summary>Адаптивное округление</summary>
        /// <param name="x">Округляемая величина</param>
        /// <param name="n">Количество значащих разрядов</param>
        /// <returns>Число с указанным количеством значащих разрядов</returns>
        [DST]
        public static float RoundAdaptive(this float x, int n = 1)
        {
            if (x.Equals(0f)) return 0;
            if (float.IsNaN(x)) return x;
            if (float.IsInfinity(x)) return x;
            var sign = Math.Sign(x);
            x = x.Abs();
            var b = Math.Pow(10, (int)Math.Log10(x) - 1);
            return (float)(Math.Round(x / b, n - 1) * b * sign);
        }

        /// <summary>Получить обратное число</summary>
        /// <param name="x">Инвертируемое число</param>
        /// <returns>Число, обратное к исходном</returns>
        [DST] public static double GetInverse(this double x) => 1 / x;

        /// <summary>Получить обратное число</summary>
        /// <param name="x">Инвертируемое число</param>
        /// <returns>Число, обратное к исходном</returns>
        [DST] public static float GetInverse(this float x) => 1 / x;
        
        /// <summary>Возведение в целую степень</summary>
        /// <param name="x">Действительное число</param>
        /// <param name="n">Целочисленный показатель степени</param>
        /// <returns>x^n</returns>
        [DST]
        public static double Power(this double x, int n)
        {
            if (double.IsNaN(x)) return double.NaN;
            if (x.Equals(0d)) return 0d;
            if (x.Equals(1d)) return 1d;
            if (n > 1000 || n < -1000) return Math.Pow(x, n);
            if (n < 0) return (1 / x).Power(-n);
            var result = 1.0;
            for (var i = 0; i < n; i++)
                result *= x;
            return result;
        }

        /// <summary>Возведение в целую степень</summary>
        /// <param name="x">Действительное число</param>
        /// <param name="n">Целочисленный показатель степени</param>
        /// <returns>x^n</returns>
        [DST]
        public static float Power(this float x, int n)
        {
            if (float.IsNaN(x)) return float.NaN;
            if (x.Equals(0f)) return 0f;
            if (x.Equals(1f)) return 1f;
            if (n > 1000 || n < -1000) return (float)Math.Pow(x, n);
            if (n < 0) return (1 / x).Power(-n);
            var result = 1.0;
            for (var i = 0; i < n; i++)
                result *= x;
            return (float)result;
        }

        /// <summary>Возведение числа в действительную степень</summary>
        /// <param name="x">Основание</param><param name="y">Действительный показатель степени</param>
        /// <returns>Действительное число x возведённое в степень y: x^y</returns>
        [DST] public static double Power(this double x, double y) => double.IsNaN(x) ? double.NaN : (double.IsNaN(y) ? double.NaN : Math.Pow(x, y));

        /// <summary>Возведение числа в действительную степень</summary>
        /// <param name="x">Основание</param><param name="y">Действительный показатель степени</param>
        /// <returns>Действительное число x возведённое в степень y: x^y</returns>
        [DST] public static float Power(this float x, float y) => float.IsNaN(x) ? float.NaN : (float.IsNaN(y) ? float.NaN : (float)Math.Pow(x, y));

        /// <summary>Возведение числа в комплексную степень</summary>
        /// <param name="x">Основание</param><param name="z">Комплексный показатель степень</param>
        /// <returns>Значение x^z, где x - действительное, z - комплексное</returns>
        [DST] public static Complex Power(this double x, Complex z) => x ^ z;

        /// <summary>Преобразование в децибелы по амплитуде</summary>
        /// <param name="x">Амплитудное значение 20*lg(x)</param>
        /// <returns>Значение в децибелах</returns>
        [DST] public static double In_dB(this double x) => double.IsNaN(x) ? double.NaN : 20 * Math.Log10(x);

        /// <summary>Преобразование в децибелы по мощности</summary>
        /// <param name="x">Значение мощности 10*lg(x)</param>
        /// <returns>Значение в децибелах</returns>
        [DST] public static double In_dB_byPower(this double x) => double.IsNaN(x) ? double.NaN : 10 * Math.Log10(x);

        /// <summary>Преобразование из децибелов в разы по значению (амплитуде)</summary>
        /// <param name="db">Значение в децибелах 10^(x/20)</param>
        /// <returns>Значение в разах по амплитуде</returns>
        [DST] public static double From_dB(this double db) => double.IsNaN(db) ? double.NaN : Math.Pow(10, db / 20);

        /// <summary>Преобразование из децибелов в разы по мощности</summary>
        /// <param name="db">Значение в децибелах 10^(x/10)</param>
        /// <returns>Значение в разах по мощности</returns>
        [DST] public static double From_dB_byPower(this double db) => double.IsNaN(db) ? double.NaN : Math.Pow(10, db / 10);

        /// <summary>Преобразование значения в радианы</summary>
        /// <param name="deg">Значение в градусах</param>
        /// <returns>Значение в радианах</returns>
        [DST] public static double ToRad(this double deg) => deg * Consts.Geometry.ToRad;

        /// <summary>Преобразование значения в градусы</summary>
        /// <param name="rad">Значение в радианах</param>
        /// <returns>Значение в градусах</returns>
        [DST] public static double ToDeg(this double rad) => rad * Consts.Geometry.ToDeg;

        /// <summary>Преобразование значения в радианы</summary>
        /// <param name="deg">Значение в градусах</param>
        /// <returns>Значение в радианах</returns>
        [DST] public static float ToRad(this float deg) => (float) (deg * Consts.Geometry.ToRad);

        /// <summary>Преобразование значения в градусы</summary>
        /// <param name="rad">Значение в радианах</param>
        /// <returns>Значение в градусах</returns>
        [DST] public static float ToDeg(this float rad) => (float) (rad * Consts.Geometry.ToDeg);

        public static long ToIntBits(this double x) => BitConverter.DoubleToInt64Bits(x);
        public static double ToDoubleBits(this long x) => BitConverter.Int64BitsToDouble(x);
    }
}