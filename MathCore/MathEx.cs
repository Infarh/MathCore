using System;

namespace MathCore;

/// <summary>Расширенный класс математических функций</summary>
public static class MathEx
{
    /// <summary>Секанс</summary>
    public static double Sec(double x)
    {
        var mod = (x - Consts.pi05) % Math.PI;
        return Math.Abs(mod) > double.Epsilon
            ? 1 / Math.Cos(x)
            : mod < 0
                ? double.PositiveInfinity
                : double.NegativeInfinity;
    }

    /// <summary>Секанс</summary>
    public static Complex Sec(Complex z) => 1 / Complex.Trigonometry.Cos(z);

    /// <summary>Косеканс</summary>
    public static double Cosec(double x)
    {
        var mod = x % Math.PI;
        return Math.Abs(mod) > double.Epsilon
            ? 1 / Math.Sin(x)
            : mod < 0
                ? double.PositiveInfinity
                : double.NegativeInfinity;
    }

    /// <summary>Косеканс</summary>
    public static Complex Cosec(Complex x) => 1 / Complex.Trigonometry.Sin(x);

    /// <summary>Тангенс</summary>
    public static double Tg(double z) => Math.Tan(z);

    /// <summary>Тангенс</summary>
    public static Complex Tg(Complex z) => Complex.Trigonometry.Tg(z);

    /// <summary>Котангенс</summary>
    public static double Ctg(double x) => 1 / Math.Tan(x);

    /// <summary>Котангенс</summary>
    public static Complex Ctg(Complex z) => 1 / Tg(z);

    /// <summary>Арккотангенс</summary>
    public static double Actg(double x) => Consts.pi05 - Math.Atan(x);

    /// <summary>Арккотангенс</summary>
    public static Complex Actg(Complex z) => Consts.pi05 - Complex.Trigonometry.Atan(z);

    /// <summary>Арксинус</summary>
    public static double Asin(double x) => Math.Atan(x / Math.Sqrt(1 - x * x));

    /// <summary>Арксинус</summary>
    public static Complex Asin(Complex z) => Complex.Trigonometry.Atan(z / Complex.Sqrt(1 - z.Pow2()));

    /// <summary>Арккосинус</summary>
    public static double Acos(double x) => Math.Atan(-x / Math.Sqrt(1 - x * x)) + Consts.pi05; // + 2 * Consts.pi025; // + 2 * Math.Atan(1);

    /// <summary>Арккосинус</summary>
    public static Complex Acos(Complex z) => Complex.Trigonometry.Atan(-z / Complex.Sqrt(1 - z.Pow2())) + Consts.pi05;

    /// <summary>Арксеканс</summary>
    public static double Asec(double x) => Consts.pi05 - Acosec(x);

    ///// <summary>Арксеканс</summary>
    //public static Complex Asec(Complex z) => Consts.pi05 - Acosec(z);

    /// <summary>Арккосеканс</summary>
    public static double Acosec(double x) => Math.Atan(Math.Sign(x) / Math.Sqrt(x * x - 1));

    ///// <summary>Арккосеканс</summary>
    //public static Complex Acosec(Complex z) => Complex.Trigonometry.Atan(Math.Sign(z) / Complex.Sqrt(z * z - 1));

    /// <summary>Гиперболические функции</summary>
    public static class Hyperbolic
    {
        /// <summary>Гиперболический синус</summary>
        public static double Sinh(double x)
        {
            var exp = Math.Exp(x);
            return (exp - 1 / exp) / 2;
        }

        /// <summary>Гиперболический синус</summary>
        public static Complex Sinh(Complex z)
        {
            var exp = Complex.Exp(z);
            return (exp - 1 / exp) / 2;
        }

        /// <summary>Гиперболический косинус</summary>
        public static double Cosh(double x)
        {
            var exp = Math.Exp(x);
            return (exp + 1 / exp) / 2;
        }

        /// <summary>Гиперболический косинус</summary>
        public static Complex Cosh(Complex z)
        {
            var exp = Complex.Exp(z);
            return (exp + 1 / exp) / 2;
        }

        /// <summary>Гиперболический тангенс</summary>
        public static double Tgh(double x)
        {
            var exp = Math.Exp(x);
            var exp_inv = 1 / exp;
            return (exp - exp_inv) / (exp + exp_inv);
        }

        /// <summary>Гиперболический тангенс</summary>
        public static Complex Tgh(Complex z)
        {
            var exp = Complex.Exp(z);
            var exp_inv = 1 / exp;
            return (exp - exp_inv) / (exp + exp_inv);
        }

        /// <summary>Гиперболический котангенс</summary>
        public static double Ctgh(double x) => 1 / Tgh(x);

        /// <summary>Гиперболический котангенс</summary>
        public static Complex Ctgh(Complex z) => 1 / Tgh(z);

        /// <summary>Гиперболический секанс</summary>
        public static double Sech(double x) => 1 / Cosh(x);

        /// <summary>Гиперболический секанс</summary>
        public static Complex Sech(Complex z) => 1 / Cosh(z);

        /// <summary>Гиперболический косеканс</summary>
        public static double Cosech(double x) => 1 / Sinh(x);

        /// <summary>Гиперболический косеканс</summary>
        public static Complex Cosech(Complex z) => 1 / Sinh(z);

        /// <summary>Гиперболический арксинус</summary>
        public static double Asinh(double x) => Math.Log(x + Math.Sqrt(x * x + 1));

        /// <summary>Гиперболический арксинус</summary>
        public static Complex Asinh(Complex z) => Complex.Ln(z + Complex.Sqrt(z.Pow2() + 1));

        /// <summary>Гиперболический арккосинус</summary>
        public static double Acosh(double x) => Math.Log(x + Math.Sqrt(x * x - 1));

        /// <summary>Гиперболический арккосинус</summary>
        public static Complex Acosh(Complex z) => Complex.Ln(z + Complex.Sqrt(z.Pow2() - 1));

        /// <summary>Гиперболический арктангенс</summary>
        public static double Atanh(double x) => Math.Log((1 + x) / (1 - x)) / 2;

        /// <summary>Гиперболический арктангенс</summary>
        public static Complex Atanh(Complex z) => Complex.Ln((1 + z) / (1 - z)) / 2;

        /// <summary>Гиперболический арккотангенс</summary>
        public static double Acotanh(double x) => Math.Log((1 - x) / (1 + x)) / 2;

        /// <summary>Гиперболический арккотангенс</summary>
        public static Complex Acotanh(Complex z) => Complex.Ln((1 - z) / (1 + z)) / 2;

        /// <summary>Гиперболический арксеканс</summary>
        public static double Asech(double x) => Math.Log((Math.Sqrt(1 - x * x) + 1) / 2);

        /// <summary>Гиперболический арксеканс</summary>
        public static Complex Asech(Complex z) => Complex.Ln((Complex.Sqrt(1 - z.Pow2()) + 1) / 2);

        /// <summary>Гиперболический арккосеканс</summary>
        public static double Acosech(double x) => Math.Log((Math.Sign(x) * Math.Sqrt(x * x) + 1) / x);

        ///// <summary>Гиперболический арккосеканс</summary>
        //public static Complex Acosech(Complex z) => Complex.Ln((Math.Sign(z) * Complex.Sqrt(z.Pow2() + 1)) / z);
    }
}
