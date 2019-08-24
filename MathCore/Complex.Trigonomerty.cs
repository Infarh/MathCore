using System;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable InconsistentNaming

namespace MathCore
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public readonly partial struct Complex
    {
        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Тригонометрические функции комплексного переменного</summary>
        public static class Trigonomerty
        {
            /// <summary>Синус</summary>
            /// <param name="z">Комплексный аргумент</param>
            /// <returns>Синус комплексного аргумента</returns>
            public static Complex Sin(in Complex z)
            {
                var (re, im) = z;
                if (im.Equals(0)) return new Complex(Math.Sin(re));
                var eb = Math.Exp(im);
                var eb_inv = 1 / eb;

                return new Complex(.5 * Math.Sin(re) * (eb_inv + eb), .5 * Math.Cos(re) * (eb - eb_inv));
            }

            /// <summary>Косинус</summary>
            /// <param name="z">Комплексный аргумент</param>
            /// <returns>Косинус комплексного аргумента</returns>
            public static Complex Cos(in Complex z)
            {
                var (re, im) = z;
                if (im.Equals(0)) return new Complex(Math.Cos(re));
                var eb = Math.Exp(im);
                var eb_inv = 1 / eb;

                return new Complex(.5 * Math.Cos(re) * (eb_inv + eb), .5 * Math.Sin(re) * (eb_inv - eb));
            }

            /// <summary>Тангенс</summary>
            /// <param name="z">Комплексный аргумент</param>
            /// <returns>Тангенс комплексного аргумента</returns>
            public static Complex tg(in Complex z)
            {
                var (re, im) = z;
                if (im.Equals(0d)) return new Complex(Math.Sin(re) / Math.Cos(re));

                re *= 2;
                im *= 2;

                var sin2Re = Math.Sin(re);
                var sinh2Im = Math.Sinh(im);
                var cos2Re_cosh2Im = Math.Cos(re) + Math.Cosh(im);

                return new Complex(sin2Re / cos2Re_cosh2Im, sinh2Im / cos2Re_cosh2Im);
            }

            /// <summary>Котангенс</summary>
            /// <param name="z">Комплексный аргумент</param>
            /// <returns>Котангенс комплексного аргумента</returns>
            public static Complex ctg(in Complex z)
            {
                var (re, im) = z;
                if (im.Equals(0d)) return new Complex(Math.Sin(re) / Math.Cos(re));

                var sinRe = Math.Sin(re);
                var cosRe = Math.Cos(re);
                im *= 2;
                re *= 2;
                var sin2Im = Math.Sinh(im);
                var cos2Re_cosh2Im = Math.Cos(re) - Math.Cosh(im);

                return new Complex(2 * cosRe * sinRe / cos2Re_cosh2Im, sin2Im / cos2Re_cosh2Im);
            }

            public static Complex Asin(in Complex z)
            {
                var (sqrt_re, sqrt_im) = Sqrt(1 - z.Pow2());
                var (ln_re, ln_im) = Ln(new Complex(sqrt_re - z.Im, sqrt_im + z.Re));
                return new Complex(ln_im, -ln_re);
            }

            public static Complex Acos(in Complex z)
            {
                var (sqrt_re, sqrt_im) = Sqrt(z.Pow2() - 1);
                var (ln_re, ln_im) = Ln(new Complex(sqrt_re + z.Re, sqrt_im + z.Im));
                return new Complex(ln_im, -ln_re);
            }

            /// <summary>Арктангенс комплексного переменного</summary>
            /// <param name="z">Комплексный аргумент</param>
            /// <returns>Арктангенс комплексного аргумента</returns>
            public static Complex Atan(in Complex z)
            {
                var iz = i * z;
                return i * (Ln(1 - iz) - Ln(1 + iz)) / new Complex(2d);
            }

            public static Complex Atctg(in Complex z)
            {
                var iz = i * z;
                return i * (Ln(iz + 1) - Ln(iz - 1)) / new Complex(2d);
            }

            /// <summary>Гиперболические функции</summary>
            public static class Hyperbolic
            {
                /// <summary>Гиперболический синус</summary>
                /// <param name="z">Комплексный аргумент</param>
                /// <returns>Гиперболический синус комплексного аргумента</returns>
                public static Complex Sh(in Complex z)
                {
                    var (re, im) = z;
                    if (im.Equals(0)) return Math.Sinh(re);
                    var sinhRe = Math.Sinh(re);
                    var coshRe = Math.Cosh(re);
                    var sinIm = Math.Sin(im);
                    var cosIm = Math.Cos(im);

                    return new Complex(sinhRe * cosIm, coshRe * sinIm);
                }

                /// <summary>Гиперболический косинус</summary>
                /// <param name="z">Комплексный аргумент</param>
                /// <returns>Гиперболический косинус комплексного аргумента</returns>
                public static Complex Ch(in Complex z)
                {
                    var (re, im) = z;
                    if (im.Equals(0)) return Math.Cosh(re);
                    var sinhRe = Math.Sinh(re);
                    var coshRe = Math.Cosh(re);
                    var sinIm = Math.Sin(im);
                    var cosIm = Math.Cos(im);

                    return new Complex(coshRe * cosIm, sinhRe * sinIm);
                }

                /// <summary>Гиперболический тангенс</summary>
                /// <param name="z">Комплексный аргумент</param>
                /// <returns>Гиперболический тангенс комплексного аргумента</returns>
                public static Complex tgh(in Complex z)
                {
                    var (re, im) = z;
                    if (im.Equals(0)) return Math.Tanh(re);

                    var coshRe = Math.Cosh(re);
                    var sinhRe = Math.Sinh(re);
                    im *= 2;
                    re *= 2;
                    var sin2Im = Math.Sin(im);
                    var cos2Im_cosh2Re = Math.Cos(2 * im) + Math.Cosh(2 * re);

                    return new Complex(2 * coshRe * sinhRe / cos2Im_cosh2Re, sin2Im / cos2Im_cosh2Re);
                }

                /// <summary>Гиперболический котангенс</summary>
                /// <param name="z">Комплексный аргумент</param>
                /// <returns>Гиперболический котангенс комплексного аргумента</returns>
                public static Complex ctgh(in Complex z)
                {
                    var (re, im) = z;
                    if (im.Equals(0)) return Math.Tanh(re);

                    var coshRe = Math.Cosh(re);
                    var sinhRe = Math.Sinh(re);
                    im *= 2;
                    re *= 2;
                    var sin2Im = Math.Sin(im);
                    var cos2Im_cosh2Re = Math.Cos(2 * im) - Math.Cosh(2 * re);

                    return new Complex(2 * coshRe * sinhRe / cos2Im_cosh2Re, sin2Im / cos2Im_cosh2Re);
                }
            }
        }

    }
}
