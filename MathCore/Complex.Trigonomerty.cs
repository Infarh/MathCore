﻿using System.Diagnostics.CodeAnalysis;
// ReSharper disable UnusedType.Global


namespace MathCore;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public readonly partial struct Complex
{
    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Тригонометрические функции комплексного переменного</summary>
    public static class Trigonometry
    {
        /// <summary>Синус</summary>
        /// <param name="z">Комплексный аргумент</param>
        /// <returns>Синус комплексного аргумента</returns>
        public static Complex Sin(Complex z)
        {
            var (re, im) = z;
            if (im == 0) 
                return new(Math.Sin(re));

            var eb     = Math.Exp(im);
            var eb_inv = 1 / eb;
            return new
            (
                Re: .5 * Math.Sin(re) * (eb_inv + eb),
                Im: .5 * Math.Cos(re) * (eb - eb_inv)
            );
        }

        /// <summary>Косинус</summary>
        /// <param name="z">Комплексный аргумент</param>
        /// <returns>Косинус комплексного аргумента</returns>
        public static Complex Cos(Complex z)
        {
            var (re, im) = z;
            if (im == 0) 
                return new(Math.Cos(re));

            var eb     = Math.Exp(im);
            var eb_inv = 1 / eb;
            return new
            (
                Re: .5 * Math.Cos(re) * (eb_inv + eb),
                Im: .5 * Math.Sin(re) * (eb_inv - eb)
            );
        }

        public static (Complex Sin, Complex Cos) SinCos(Complex z)
        {
            var (re, im) = z;
            if (im == 0)
                return (new(Math.Sin(re)), new(Math.Cos(re)));

            var eb = Math.Exp(im);
            var eb_inv = 1 / eb;

            var sin = new Complex
            (
                Re: .5 * Math.Sin(re) * (eb_inv + eb),
                Im: .5 * Math.Cos(re) * (eb - eb_inv)
            );

            var cos = new Complex
            (
                Re: .5 * Math.Cos(re) * (eb_inv + eb),
                Im: .5 * Math.Sin(re) * (eb_inv - eb)
            );

            return (sin, cos);
        }

        /// <summary>Тангенс</summary>
        /// <param name="z">Комплексный аргумент</param>
        /// <returns>Тангенс комплексного аргумента</returns>
        public static Complex Tg(Complex z)
        {
            var (re, im) = z;
            if (im == 0) 
                return new(Math.Sin(re) / Math.Cos(re));

            re *= 2;
            im *= 2;

            var sin_2_re         = Math.Sin(re);
            var sinh_2_im        = Math.Sinh(im);
            var cos2_re_cosh2_im = Math.Cos(re) + Math.Cosh(im);

            return new
            (
                Re: sin_2_re / cos2_re_cosh2_im,
                Im: sinh_2_im / cos2_re_cosh2_im
            );
        }

        /// <summary>Котангенс</summary>
        /// <param name="z">Комплексный аргумент</param>
        /// <returns>Котангенс комплексного аргумента</returns>
        public static Complex Ctg(Complex z)
        {
            var (re, im) = z;
            if (im == 0) 
                return new(Math.Sin(re) / Math.Cos(re));

            var sin_re = Math.Sin(re);
            var cos_re = Math.Cos(re);
            im *= 2;
            re *= 2;
            var sin2_im          = Math.Sinh(im);
            var cos2_re_cosh2_im = Math.Cos(re) - Math.Cosh(im);

            return new
            (
                Re: 2 * cos_re * sin_re / cos2_re_cosh2_im,
                Im: sin2_im / cos2_re_cosh2_im
            );
        }

        /// <summary>Арксинус комплексного переменного</summary>
        /// <param name="z">Комплексный аргумент</param>
        /// <returns>Арксинус комплексного аргумента</returns>
        public static Complex Asin(Complex z)
        {
            var (sqrt_re, sqrt_im) = Sqrt(1 - z.Pow2());
            var (ln_re, ln_im)     = Ln(new Complex(sqrt_re - z.Im, sqrt_im + z.Re));
            return new(ln_im, -ln_re);
        }

        /// <summary>Арккосинус комплексного переменного</summary>
        /// <param name="z">Комплексный аргумент</param>
        /// <returns>Арккосинус комплексного аргумента</returns>
        public static Complex Acos(Complex z)
        {
            var (sqrt_re, sqrt_im) = Sqrt(z.Pow2() - 1);
            var (ln_re, ln_im)     = Ln(new Complex(sqrt_re + z.Re, sqrt_im + z.Im));
            return new(ln_im, -ln_re);
        }

        /// <summary>Арктангенс комплексного переменного</summary>
        /// <param name="z">Комплексный аргумент</param>
        /// <returns>Арктангенс комплексного аргумента</returns>
        public static Complex Atan(Complex z)
        {
            var iz = i * z;
            return i * (Ln(1 - iz) - Ln(1 + iz)) / new Complex(2d);
        }

        /// <summary>Арккатангенс комплексного переменного</summary>
        /// <param name="z">Комплексный аргумент</param>
        /// <returns>Арккатангенс комплексного аргумента</returns>
        public static Complex Arctg(Complex z)
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
            public static Complex Sh(Complex z)
            {
                var (re, im) = z;
                if (im == 0) 
                    return Math.Sinh(re);

                var sinh_re = Math.Sinh(re);
                var cosh_re = Math.Cosh(re);
                var sin_im  = Math.Sin(im);
                var cos_im  = Math.Cos(im);

                return new
                (
                    Re: sinh_re * cos_im,
                    Im: cosh_re * sin_im
                );
            }

            /// <summary>Гиперболический косинус</summary>
            /// <param name="z">Комплексный аргумент</param>
            /// <returns>Гиперболический косинус комплексного аргумента</returns>
            public static Complex Ch(Complex z)
            {
                var (re, im) = z;
                if (im == 0) 
                    return Math.Cosh(re);

                var sinh_re = Math.Sinh(re);
                var cosh_re = Math.Cosh(re);
                var sin_im  = Math.Sin(im);
                var cos_im  = Math.Cos(im);

                return new
                (
                    Re: cosh_re * cos_im,
                    Im: sinh_re * sin_im
                );
            }

            public static (Complex Sh, Complex Ch) ShCh(Complex z)
            {
                var (re, im) = z;
                if (im == 0)
                    return (new(Math.Sinh(re)), new(Math.Cosh(re)));

                var exp = Math.Exp(re);

                var sin = Math.Sin(im) * 0.5;
                var cos = Math.Cos(im) * 0.5;

                var e0 = new Complex(cos * exp, +sin * exp);
                var e1 = new Complex(cos / exp, -sin / exp);

                return (e0 - e1, e0 + e1);
            }

            /// <summary>Гиперболический тангенс</summary>
            /// <param name="z">Комплексный аргумент</param>
            /// <returns>Гиперболический тангенс комплексного аргумента</returns>
            public static Complex Tgh(Complex z)
            {
                var (re, im) = z;
                if (im == 0) 
                    return Math.Tanh(re);

                var cosh_re = Math.Cosh(re);
                var sinh_re = Math.Sinh(re);
                im *= 2;
                re *= 2;
                var sin2_im          = Math.Sin(im);
                var cos2_im_cosh2_re = Math.Cos(2 * im) + Math.Cosh(2 * re);

                return new
                (
                    Re: 2 * cosh_re * sinh_re / cos2_im_cosh2_re,
                    Im: sin2_im / cos2_im_cosh2_re
                );
            }

            /// <summary>Гиперболический котангенс</summary>
            /// <param name="z">Комплексный аргумент</param>
            /// <returns>Гиперболический котангенс комплексного аргумента</returns>
            public static Complex Ctgh(Complex z)
            {
                var (re, im) = z;
                if (im == 0) 
                    return Math.Tanh(re);

                var cosh_re = Math.Cosh(re);
                var sinh_re = Math.Sinh(re);
                im *= 2;
                re *= 2;
                var sin2_im          = Math.Sin(im);
                var cos2_im_cosh2_re = Math.Cos(2 * im) - Math.Cosh(2 * re);

                return new
                (
                    Re: 2 * cosh_re * sinh_re / cos2_im_cosh2_re,
                    Im: sin2_im / cos2_im_cosh2_re
                );
            }
        }
    }
}