using MathCore;
//using MathCore.Vectors;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Расширения для чисел двойной точности</summary>
    public static class DoubleExtentions
    {
        [DST]
        public static double Abs(this double value) => double.IsNaN(value) ? double.NaN : Math.Abs(value);

        [DST]
        public static double Sign(this double value) => double.IsNaN(value) ? double.NaN : Math.Sign(value);

        [DST]
        public static double Round(this double value) => double.IsNaN(value) ? double.NaN : Math.Round(value);

        [DST]
        public static double Floor(this double value) => double.IsNaN(value) ? double.NaN : Math.Floor(value);

        [DST]
        public static double Truncate(this double value) => double.IsNaN(value) ? double.NaN : Math.Truncate(value);

        [DST]
        public static double Pow(this double value, double p) => double.IsNaN(value) ? double.NaN : double.IsNaN(p) ? double.NaN : Math.Pow(value, p);

        [DST]
        public static double Pow2(this double value) => value * value;

        [DST]
        public static Complex Pow2(in this Complex value)
        {
            var (a, b) = value;
            return new Complex(a * a - b * b, 2 * a * b);
        }

        /// <summary>Квадратный корень</summary>
        /// <param name="x">Число из которого извлекается квадратный корень</param>
        /// <returns>Квадратный корень числа</returns>
        [DST]
        public static double Sqrt(this double x) => double.IsNaN(x) ? double.NaN : Math.Sqrt(x);

        /// <summary>Является ли число целым?</summary>
        /// <param name="x">Проверяемое число</param>
        /// <returns>Истина, если число целое</returns>
        [DST]
        public static bool IsInt(this double x) => !double.IsNaN(x) && ((int)x - x).Equals(0d);

        /// <summary>Является ли значение "не числом"?</summary>
        /// <param name="x">Проверяемое значение</param>
        /// <returns>Истина, если значение - не число</returns>
        [DST]
        public static bool IsNaN(this double x) => double.IsNaN(x);

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

        //[DST]
        //public static Vector3D RoundAdaptive(this Vector3D r, int n = 1) => new Vector3D(r.X.RoundAdaptive(n), r.Y.RoundAdaptive(n), r.Z.RoundAdaptive(n));


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
            x = x.GetAbs();
            var b = Math.Pow(10, (int)Math.Log10(x) - 1);
            return Math.Round(x / b, n - 1) * b * sign;
        }

        /// <summary>Получить обратное число</summary>
        /// <param name="x">Инвертируемое число</param>
        /// <returns>Число, обратное к исходном</returns>
        [DST]
        public static double GetInverse(this double x) => 1 / x;

        /// <summary>Число по модулю</summary>
        /// <param name="x">Исходное число</param>
        /// <param name="mod">Модуль</param>
        /// <returns>Число по модулю</returns>
        [DST]
        public static double GetAbsMod(this double x, double mod) => x % mod + (x < 0 ? mod : 0);

        /// <summary>Модуль числа</summary>
        /// <param name="x">Действительное вещественное число</param>
        /// <returns>Модуль числа</returns>
        [DST]
        public static double GetAbs(this double x) => double.IsNaN(x) ? double.NaN : Math.Abs(x);

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

        /// <summary>Возведение числа в действительную степень</summary>
        /// <param name="x">ОСнование</param><param name="y">Действительный показатель степени</param>
        /// <returns>Действительное число x возведённое в степень y: x^y</returns>
        [DST]
        public static double Power(this double x, double y) => double.IsNaN(x) ? double.NaN : (double.IsNaN(y) ? double.NaN : Math.Pow(x, y));

        /// <summary>Возведение числа в комплексную степень</summary>
        /// <param name="x">Основание</param><param name="z">Комплексный показатель степень</param>
        /// <returns>Значение x^z, где x - действительное, z - комплексное</returns>
        [DST]
        public static Complex Power(this double x, Complex z) => x ^ z;

        /// <summary>Преобразование в децебеллы по амплитуде</summary>
        /// <param name="x">Амплитудное значение 20*lg(x)</param>
        /// <returns>Значение в децебеллах</returns>
        [DST]
        public static double In_dB(this double x) => double.IsNaN(x) ? double.NaN : 20 * Math.Log10(x);

        /// <summary>Преобразование в децебеллы по мощности</summary>
        /// <param name="x">Значение мощности 10*lg(x)</param>
        /// <returns>Значение в децебеллах</returns>
        [DST]
        public static double In_dB_byPower(this double x) => double.IsNaN(x) ? double.NaN : 10 * Math.Log10(x);

        /// <summary>Преобразование из децебеллов в разы по значению (амплитуде)</summary>
        /// <param name="db">Значение в децебеллах 10^(x/20)</param>
        /// <returns>Значение в разах по амплитуде</returns>
        [DST]
        public static double From_dB(this double db) => double.IsNaN(db) ? double.NaN : Math.Pow(10, db / 20);

        /// <summary>Преобразование из децебеллов в разы по мощности</summary>
        /// <param name="db">Значение в децебеллах 10^(x/10)</param>
        /// <returns>Значение в разах по мощности</returns>
        [DST]
        public static double From_dB_byPower(this double db) => double.IsNaN(db) ? double.NaN : Math.Pow(10, db / 10);

        /// <summary>Преобразование значения в радианы</summary>
        /// <param name="deg">Значение в градусах</param>
        /// <returns>Значение в радианах</returns>
        [DST]
        public static double ToRad(this double deg) => deg * Consts.Geometry.ToRad;

        /// <summary>Преобразование значения в градусы</summary>
        /// <param name="rad">Значение в радианах</param>
        /// <returns>Значение в градусах</returns>
        [DST]
        public static double ToDeg(this double rad) => rad * Consts.Geometry.ToDeg;
    }
}