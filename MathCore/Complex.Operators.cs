using System;

using MathCore.Annotations;

using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace MathCore
{
    public readonly partial struct Complex
    {
        /// <summary>Деконструктор комплексного числа</summary>
        /// <param name="re">Действительная часть</param>
        /// <param name="im">Мнимая часть</param>
        public void Deconstruct(out double re, out double im)
        {
            re = _Re;
            im = _Im;
        }

        #region Операторы математики

        /// <summary>Унарный оператор суммы. Возвращает число Re+jIm</summary>
        /// <param name="z">Комплексное число</param>
        /// <returns>Возвращает Re+jIm</returns>
        public static Complex operator +(Complex z) => new(z.Re, z.Im);

        /// <summary>Унарный оператор разности. Возвращает число -Re-jIm</summary>
        /// <param name="z">Комплексное число</param>
        /// <returns>Возвращает -Re-jIm</returns>
        [DST] public static Complex operator -(Complex z) => new(-z.Re, -z.Im);

        /// <summary>Оператор комплексного сопряжения</summary>
        /// <param name="z">Комплексное число</param>
        /// <returns>Возвращает Re-jIm</returns>
        [DST] public static Complex operator !(Complex z) => z.ComplexConjugate;

        /// <summary>Оператор сложения</summary>
        /// <param name="X">Первое слагаемое</param>
        /// <param name="Y">Второе слагаемое</param>
        /// <returns>Сумма комплексных чисел</returns>
        [DST] public static Complex operator +(Complex X, Complex Y) => new(X.Re + Y.Re, X.Im + Y.Im);

        /// <summary>Оператор разности комплексных чисел</summary>
        /// <param name="X">Уменьшаемое комплексное число</param>
        /// <param name="Y">Вычитаемое комплексное число</param>
        /// <returns>Разность комплексных чисел</returns>
        [DST] public static Complex operator -(Complex X, Complex Y) => new(X.Re - Y.Re, X.Im - Y.Im);

        /// <summary>Оператор сложения</summary>
        /// <param name="X">Первое слагаемое</param>
        /// <param name="Y">Второе слагаемое</param>
        /// <returns>Сумма комплексных чисел</returns>
        [DST] public static Complex operator +(double X, Complex Y) => new(X + Y.Re, Y.Im);

        /// <summary>Оператор суммы целого и комплексного числа</summary>
        /// <param name="X">Целое число</param>
        /// <param name="Y">Комплексное число</param>
        /// <returns>X+Re{Y}+jIm{Y}</returns>
        [DST] public static Complex operator +(int X, Complex Y) => new(X + Y.Re, Y.Im);

        /// <summary>Оператор суммы вещественного числа одинарной точности и комплексного числа</summary>
        /// <param name="X">Вещественное число одинарной точности число</param>
        /// <param name="Y">Комплексное число</param>
        /// <returns>X+Re{Y}+jIm{Y}</returns>
        [DST] public static Complex operator +(float X, Complex Y) => new(X + Y.Re, Y.Im);

        /// <summary>Оператор разности комплексных чисел</summary>
        /// <param name="X">Уменьшаемое комплексное число</param>
        /// <param name="Y">Вычитаемое комплексное число</param>
        /// <returns>Разность комплексных чисел</returns>
        [DST] public static Complex operator -(double X, Complex Y) => new(X - Y.Re, -Y.Im);

        /// <summary>Оператор разности целого и комплексного числа</summary>
        /// <param name="X">Целое число</param>
        /// <param name="Y">Комплексное число</param>
        /// <returns>X-Re{Y}-jIm{Y}</returns>
        [DST] public static Complex operator -(int X, Complex Y) => new(X - Y.Re, -Y.Im);

        /// <summary>Оператор разности вещественного числа одинарной точности и комплексного числа</summary>
        /// <param name="X">Вещественное число одинарной точности число</param>
        /// <param name="Y">Комплексное число</param>
        /// <returns>X-Re{Y}-jIm{Y}</returns>
        [DST] public static Complex operator -(float X, Complex Y) => new(X - Y.Re, -Y.Im);

        /// <summary>Оператор сложения</summary>
        /// <param name="X">Первое слагаемое</param>
        /// <param name="Y">Второе слагаемое</param>
        /// <returns>Сумма комплексных чисел</returns>
        [DST] public static Complex operator +(Complex X, double Y) => new(X.Re + Y, X.Im);

        /// <summary>Оператор суммы комплексного числа и целого числа</summary>
        /// <param name="X">Комплексное число</param>
        /// <param name="Y">Целое число</param>
        /// <returns>Re{X}+Y+jIm{X}</returns>
        [DST] public static Complex operator +(Complex X, int Y) => new(X.Re + Y, X.Im);

        /// <summary>Оператор суммы комплексного числа и вещественного числа одинарной точности</summary>
        /// <param name="X">Комплексное число</param>
        /// <param name="Y">Вещественное число одинарной точности</param>
        /// <returns>Re{X}+Y+jIm{X}</returns>
        [DST] public static Complex operator +(Complex X, float Y) => new(X.Re + Y, X.Im);

        /// <summary>Оператор разности комплексных чисел</summary>
        /// <param name="X">Уменьшаемое комплексное число</param>
        /// <param name="Y">Вычитаемое комплексное число</param>
        /// <returns>Разность комплексных чисел</returns>
        [DST] public static Complex operator -(Complex X, double Y) => new(X.Re - Y, X.Im);

        /// <summary>Оператор разности комплексного числа и целого числа</summary>
        /// <param name="X">Комплексное число</param>
        /// <param name="Y">Целое число</param>
        /// <returns>Re{X}-Y+jIm{X}</returns>
        [DST] public static Complex operator -(Complex X, int Y) => new(X.Re - Y, X.Im);

        /// <summary>Оператор разности комплексного числа и вещественного числа одинарной точности</summary>
        /// <param name="X">Комплексное число</param>
        /// <param name="Y">Вещественное число одинарной точности</param>
        /// <returns>Re{X}-Y+jIm{X}</returns>
        [DST] public static Complex operator -(Complex X, float Y) => new(X.Re - Y, X.Im);

        /// <summary>Оператор умножения комплексного числа на вещественное</summary>
        /// <param name="X">Комплексное число</param>
        /// <param name="Y">Вещественное число</param>
        /// <returns>Комплексное произведение</returns>
        [DST] public static Complex operator *(Complex X, double Y) => new(X.Re * Y, X.Im * Y);

        /// <summary>Оператор умножения комплексного числа на целое</summary>
        /// <param name="X">Комплексное число</param>
        /// <param name="Y">Целое число</param>
        /// <returns>Комплексное произведение</returns>
        [DST] public static Complex operator *(Complex X, int Y) => new(X.Re * Y, X.Im * Y);

        /// <summary>Оператор умножения комплексного числа на вещественное одинарной точности</summary>
        /// <param name="X">Комплексное число</param>
        /// <param name="Y">Вещественное число одинарной точности</param>
        /// <returns>Комплексное произведение</returns>
        [DST] public static Complex operator *(Complex X, float Y) => new(X.Re * Y, X.Im * Y);

        /// <summary>Оператор деления комплексного числа на вещественное</summary>
        /// <param name="X">Комплексное делимое число</param>
        /// <param name="Y">Вещественный делитель</param>
        /// <returns>Комплексное частное</returns>
        [DST] public static Complex operator /(Complex X, double Y) => new(X.Re / Y, X.Im / Y);

        /// <summary>Оператор деления комплексного числа на целое</summary>
        /// <param name="X">Комплексное делимое число</param>
        /// <param name="Y">Целый делитель</param>
        /// <returns>Комплексное частное</returns>
        [DST] public static Complex operator /(Complex X, int Y) => new(X.Re / Y, X.Im / Y);

        /// <summary>Оператор деления комплексного числа на вещественное одинарной точности</summary>
        /// <param name="X">Комплексное делимое число</param>
        /// <param name="Y">Вещественный делитель одинарной точности</param>
        /// <returns>Комплексное частное</returns>
        [DST] public static Complex operator /(Complex X, float Y) => new(X.Re / Y, X.Im / Y);

        /// <summary>Оператор произведения вещественного и комплексного числа</summary>
        /// <param name="X">Вещественное число</param>
        /// <param name="Y">Комплексное число</param>
        /// <returns>Комплексное произведение</returns>
        [DST] public static Complex operator *(double X, Complex Y) => new(Y.Re * X, Y.Im * X);

        /// <summary>Оператор произведения целого и комплексного числа</summary>
        /// <param name="X">Целое число</param>
        /// <param name="Y">Комплексное число</param>
        /// <returns>Комплексное произведение</returns>
        [DST] public static Complex operator *(int X, Complex Y) => new(Y.Re * X, Y.Im * X);

        /// <summary>Оператор произведения вещественного числа одинарной точности и комплексного числа</summary>
        /// <param name="X">Вещественное число одинарной точности</param>
        /// <param name="Y">Комплексное число</param>
        /// <returns>Комплексное произведение</returns>
        [DST] public static Complex operator *(float X, Complex Y) => new(Y.Re * X, Y.Im * X);

        /// <summary>Оператор деления вещественного и комплексного числа</summary>
        /// <param name="X">Вещественное делимое число</param>
        /// <param name="Y">Комплексный делитель</param>
        /// <returns>Комплексное частное</returns>
        [DST]
        public static Complex operator /(double X, Complex Y)
        {
            var (re, im) = Y;
            var q = 1 / (re * re + im * im);
            re = X * re * q;
            im = -X * im * q;
            return new Complex(re, im);
        }

        /// <summary>Оператор деления целого и комплексного числа</summary>
        /// <param name="X">Целого делимое число</param>
        /// <param name="Y">Комплексный делитель</param>
        /// <returns>Комплексное частное</returns>
        [DST]
        public static Complex operator /(int X, Complex Y)
        {
            var (re, im) = Y;
            var q = 1 / (re * re + im * im);
            return new Complex(X * re * q, -X * im * q);
        }
        /// <summary>Оператор деления вещественного числа одинарной точности и комплексного числа</summary>
        /// <param name="X">Вещественное делимое число одинарной точности</param>
        /// <param name="Y">Комплексный делитель</param>
        /// <returns>Комплексное частное</returns>
        [DST]
        public static Complex operator /(float X, Complex Y)
        {
            var (re, im) = Y;
            var q = 1 / (re * re + im * im);
            return new Complex(X * re * q, -X * im * q);
        }

        /// <summary>Оператор произведения двух комплексных чисел</summary>
        /// <param name="X">Первый множитель</param>
        /// <param name="Y">Второй множитель</param>
        /// <returns>Комплексное произведение</returns>
        [DST] public static Complex operator *(Complex X, Complex Y) => new(X.Re * Y.Re - X.Im * Y.Im, X.Re * Y.Im + X.Im * Y.Re);

        /// <summary>Оператор деления двух комплексных чисел</summary>
        /// <param name="X">Делимое комплексное число</param>
        /// <param name="Y">Делитель комплексного числа</param>
        /// <returns>Частное двух комплексных чисел</returns>
        [DST]
        public static Complex operator /(Complex X, Complex Y)
        {
            // Division : Smith's formula.
            var (x_re, x_im) = X;
            var (y_re, y_im) = Y;

            if (double.IsNaN(y_re) || double.IsNaN(y_im))
                return new Complex(double.NaN, double.NaN);

            if (Math.Abs(y_im) < Math.Abs(y_re))
            {
                var doc = y_im / y_re;
                return new Complex((x_re + x_im * doc) / (y_re + y_im * doc), (x_im - x_re * doc) / (y_re + y_im * doc));
            }

            var cod = y_re / y_im;
            return new Complex((x_im + x_re * cod) / (y_im + y_re * cod), (-x_re + x_im * cod) / (y_im + y_re * cod));
        }

        //public static Complex operator /(Complex X, Complex Y)
        //{
        //    var re_x = X.Re;
        //    var im_x = X.Im;
        //    var re_y = Y.Re;
        //    var im_y = Y.Im;
        //    var q = 1 / (re_y * re_y + im_y * im_y);
        //    return new Complex((re_x * re_y + im_x * im_y) * q, (im_x * re_y - re_x * im_y) * q);
        //}

        /// <summary>Возведение комплексного числа в вещественную степень по формуле Муавра</summary>
        /// <param name="Z">Возводимое в степень комплексное число Z^X</param>
        /// <param name="X">Вещественный показатель степени Z^X</param>
        /// <returns>Z^X</returns>       
        [DST] public static Complex operator ^(Complex Z, double X) => Exp(Math.Pow(Z.Abs, X), Z.Arg * X);

        /// <summary>Возведение комплексного числа в вещественную степень одинарной точности по формуле Муавра</summary>
        /// <param name="Z">Возводимое в степень комплексное число Z^X</param>
        /// <param name="X">Вещественный показатель степени одинарной точности Z^X</param>
        /// <returns>Z^X</returns> 
        [DST] public static Complex operator ^(Complex Z, float X) => Exp(Math.Pow(Z.Abs, X), Z.Arg * X);

        /// <summary>Возведение комплексного числа в целую степень по формуле Муавра</summary>
        /// <param name="Z">Возводимое в степень комплексное число Z^X</param>
        /// <param name="X">Целый показатель степени Z^X</param>
        /// <returns>Z^X</returns> 
        [DST] public static Complex operator ^(Complex Z, int X) => Exp(Math.Pow(Z.Abs, X), Z.Arg * X);

        /// <summary>Оператор возведения вещественного числа в комплексную степень</summary>
        /// <param name="X">Вещественное число</param>
        /// <param name="Z">Комплексная степень</param>
        /// <returns>Комплексный результат возведения вещественного числа в комплексную степень</returns>
        [DST]
        public static Complex operator ^(double X, Complex Z)
        {
            var (re, im) = Z;
            if (X >= 0) return Exp(Math.Pow(X, re), Math.Log(X) * im);
            var ln_x = Math.Log(Math.Abs(X));
            return Exp(ln_x * re - Consts.pi * im, Consts.pi * re + ln_x * im);
        }

        /// <summary>Оператор возведения вещественного числа одинарной точности в комплексную степень</summary>
        /// <param name="X">Вещественное число одинарной точности</param>
        /// <param name="Z">Комплексная степень</param>
        /// <returns>Комплексный результат возведения вещественного числа одинарной точности в комплексную степень</returns>
        [DST]
        public static Complex operator ^(float X, Complex Z)
        {
            var (re, im) = Z;
            if (X >= 0) return Exp(Math.Pow(X, re), Math.Log(X) * im);
            var ln_x = Math.Log(Math.Abs(X));
            return Exp(ln_x * re - Consts.pi * im, Consts.pi * re + ln_x * im);
        }

        /// <summary>Оператор возведения целого числа в комплексную степень</summary>
        /// <param name="X">Целое число</param>
        /// <param name="Z">Комплексная степень</param>
        /// <returns>Комплексный результат возведения целого числа в комплексную степень</returns>
        [DST]
        public static Complex operator ^(int X, Complex Z)
        {
            var (re, im) = Z;
            if (X >= 0) return Exp(Math.Pow(X, re), Math.Log(X) * im);
            var ln_x = Math.Log(Math.Abs(X));
            return Exp(ln_x * re - Consts.pi * im, Consts.pi * re + ln_x * im);
        }

        /// <summary>Оператор возведения комплексного числа в комплексную степень</summary>
        /// <param name="X">Комплексное основание экспоненты</param>
        /// <param name="Y">Комплексный показатель степени</param>
        /// <returns>Комплексный результат возведения комплексного числа в комплексную степень</returns>
        [DST]
        public static Complex operator ^(Complex X, Complex Y)
        {
            var r = X.Abs;
            var arg = X.Arg;

            var (re, im) = Y;
            var R = Math.Pow(r, re) * Math.Pow(Math.E, -arg * im);
            var Arg = arg * re + Math.Log(r) * im;

            return Exp(R, Arg);
        }
        #endregion

        /* -------------------------------------------------------------------------------------------- */

        #region Операторы математики с массивами

        /// <summary>Оператор, прибавляющий комплексное число к массиву целых чисел</summary>
        /// <param name="X">Массив целых чисел</param>
        /// <param name="Y">Прибавляемое комплексное число</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементной суммы целых чисел исходного массива и указанного комплексного числа</returns>
        [DST, NotNull]
        public static Complex[] operator +([NotNull] in int[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] + Y;
            return result;
        }

        /// <summary>Оператор, прибавляющий комплексное число к массиву действительных чисел одинарной точности</summary>
        /// <param name="X">Массив действительных чисел одинарной точности</param>
        /// <param name="Y">Прибавляемое комплексное число</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементной суммы вещественных чисел одинарной точности исходного массива и указанного комплексного числа</returns>
        [DST, NotNull]
        public static Complex[] operator +([NotNull] in float[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] + Y;
            return result;
        }

        /// <summary>Оператор, прибавляющий комплексное число к массиву действительных чисел</summary>
        /// <param name="X">Массив действительных чисел</param>
        /// <param name="Y">Прибавляемое комплексное число</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементной суммы вещественных чисел исходного массива и указанного комплексного числа</returns>
        [DST, NotNull]
        public static Complex[] operator +([NotNull] in double[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] + Y;
            return result;
        }

        /// <summary>Оператор, вычитающий комплексное число из массива целых чисел</summary>
        /// <param name="X">Массив целых чисел</param>
        /// <param name="Y">Вычитаемое комплексное число</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементной разности целых чисел исходного массива и указанного комплексного числа</returns>
        [DST, NotNull]
        public static Complex[] operator -([NotNull] in int[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] - Y;
            return result;
        }

        /// <summary>Оператор, вычитающий комплексное число из массива вещественных чисел одинарной точности</summary>
        /// <param name="X">Массив действительных чисел</param>
        /// <param name="Y">Вычитаемое комплексное число</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементной разности вещественных чисел одинарной точности исходного массива и указанного комплексного числа</returns>
        [DST, NotNull]
        public static Complex[] operator -([NotNull] in float[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] - Y;
            return result;
        }

        /// <summary>Оператор, вычитающий комплексное число из массива вещественных чисел</summary>
        /// <param name="X">Массив действительных чисел</param>
        /// <param name="Y">Вычитаемое комплексное число</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементной разности вещественных чисел исходного массива и указанного комплексного числа</returns>
        [DST, NotNull]
        public static Complex[] operator -([NotNull] in double[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] - Y;
            return result;
        }

        /// <summary>Оператор, умножающий комплексное число на массив целых чисел</summary>
        /// <param name="X">Массив целых чисел</param>
        /// <param name="Y">Комплексный множитель</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементного произведения целых чисел исходного массива и указанного комплексного числа</returns>
        [DST, NotNull]
        public static Complex[] operator *([NotNull] in int[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] * Y;
            return result;
        }

        /// <summary>Оператор, умножающий комплексное число на массив вещественных чисел одинарной точности</summary>
        /// <param name="X">Массив вещественных чисел одинарной точности</param>
        /// <param name="Y">Комплексный множитель</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементного произведения вещественных чисел одинарной точности исходного массива и указанного комплексного числа</returns>
        [DST, NotNull]
        public static Complex[] operator *([NotNull] in float[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] * Y;
            return result;
        }

        /// <summary>Оператор, умножающий комплексное число на массив вещественных чисел</summary>
        /// <param name="X">Массив вещественных чисел</param>
        /// <param name="Y">Комплексный множитель</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементного произведения вещественных чисел исходного массива и указанного комплексного числа</returns>
        [DST, NotNull]
        public static Complex[] operator *([NotNull] in double[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] * Y;
            return result;
        }

        /// <summary>Оператор поэлементного деления массива целых чисел на комплексное число</summary>
        /// <param name="X">Массив целых чисел</param>
        /// <param name="Y">Комплексный делитель</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементного деления целых чисел исходного массива на указанное комплексное число</returns>
        [DST, NotNull]
        public static Complex[] operator /([NotNull] in int[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] / Y;
            return result;
        }

        /// <summary>Оператор поэлементного деления массива вещественных чисел одинарной точности на комплексное число</summary>
        /// <param name="X">Массив вещественных чисел одинарной точности</param>
        /// <param name="Y">Комплексный делитель</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементного деления вещественных чисел одинарной точности исходного массива на указанное комплексное число</returns>
        [DST, NotNull]
        public static Complex[] operator /([NotNull] in float[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] / Y;
            return result;
        }

        /// <summary>Оператор поэлементного деления массива вещественных чисел на комплексное число</summary>
        /// <param name="X">Массив вещественных чисел</param>
        /// <param name="Y">Комплексный делитель</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементного деления вещественных чисел исходного массива на указанное комплексное число</returns>
        [DST, NotNull]
        public static Complex[] operator /([NotNull] in double[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] / Y;
            return result;
        }

        /// <summary>Оператор поэлементной суммы массива комплексных чисел и комплексного числа</summary>
        /// <param name="X">Массив комплексных чисел</param>
        /// <param name="Y">Прибавляемое комплексное число</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементной суммы комплексных чисел исходного массива и указанного комплексного числа</returns>
        [DST, NotNull]
        public static Complex[] operator +([NotNull] Complex[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] + Y;
            return result;
        }

        /// <summary>Оператор поэлементной разности массива комплексных чисел и комплексного числа</summary>
        /// <param name="X">Массив комплексных чисел</param>
        /// <param name="Y">Вычитаемое комплексное число</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементной разности комплексных чисел исходного массива и указанного комплексного числа</returns>
        [DST, NotNull]
        public static Complex[] operator -([NotNull] Complex[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] - Y;
            return result;
        }

        /// <summary>Оператор поэлементного произведения массива комплексных чисел и комплексного числа</summary>
        /// <param name="X">Массив комплексных чисел</param>
        /// <param name="Y">Комплексных множитель</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементного произведения комплексных чисел исходного массива и указанного комплексного числа</returns>
        [DST, NotNull]
        public static Complex[] operator *([NotNull] Complex[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] * Y;
            return result;
        }

        /// <summary>Оператор поэлементного частного массива комплексных чисел и комплексного числа</summary>
        /// <param name="X">Массив комплексных чисел</param>
        /// <param name="Y">Комплексных делитель</param>
        /// <returns>Массив комплексных чисел, являющийся результатом поэлементного частного комплексных чисел исходного массива и указанного комплексного числа</returns>
        [DST, NotNull]
        public static Complex[] operator /([NotNull] Complex[] X, Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] / Y;
            return result;
        }

        #endregion

        /* -------------------------------------------------------------------------------------------- */

        #region Отношения эквивалентности

        /// <summary>Оператор сравнения двух комплексных чисел (равенства)</summary>
        /// <returns>Истина, если числа равны между собой</returns>
        [DST] public static bool operator ==(Complex X, Complex Y) => X._Re.Equals(Y._Re) && X._Im.Equals(Y._Im);

        /// <summary>Оператор сравнения двух комплексных чисел (неравенства)</summary>
        /// <returns>Истина, если числа не равны</returns>
        [DST] public static bool operator !=(Complex X, Complex Y) => !X._Re.Equals(Y._Re) || !X._Im.Equals(Y._Im);

        /// <summary>Оператор равенства комплексного числа вещественному числу одинарной точности</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое вещественное число одинарной точности</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> равна <paramref name="Y"/>, а мнимая часть равна 0</returns>
        public static bool operator ==(Complex X, float Y) => X._Re.Equals(Y) && X._Im.Equals(0d);

        /// <summary>Оператор неравенства комплексного числа вещественному числу одинарной точности</summary>
        /// <param name="X">Сравниваемое комплексное число одинарной точности</param>
        /// <param name="Y">Сравниваемое вещественное число</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> не равна <paramref name="Y"/>, либо мнимая часть не равна 0</returns>
        public static bool operator !=(Complex X, float Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);

        /// <summary>Оператор равенства комплексного числа вещественному числу</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое вещественное число</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> равна <paramref name="Y"/>, а мнимая часть равна 0</returns>
        public static bool operator ==(Complex X, double Y) => X._Re.Equals(Y) && X._Im.Equals(0d);
        
        /// <summary>Оператор неравенства комплексного числа вещественному числу</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое вещественное число</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> не равна <paramref name="Y"/>, либо мнимая часть не равна 0</returns>
        public static bool operator !=(Complex X, double Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);

        /// <summary>Оператор равенства комплексного числа целому числу (1 байт со знаком)</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число (1 байт со знаком)</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> равна <paramref name="Y"/>, а мнимая часть равна 0</returns>
        public static bool operator ==(Complex X, sbyte Y) => X._Re.Equals(Y) && X._Im.Equals(0d);
        
        /// <summary>Оператор неравенства комплексного числа целому числу (1 байт со знаком)</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число (1 байт со знаком)</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> не равна <paramref name="Y"/>, либо мнимая часть не равна 0</returns>
        public static bool operator !=(Complex X, sbyte Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);
        
        /// <summary>Оператор равенства комплексного числа целому числу (1 байт без знака)</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число (1 байт без знака)</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> равна <paramref name="Y"/>, а мнимая часть равна 0</returns>
        public static bool operator ==(Complex X, byte Y) => X._Re.Equals(Y) && X._Im.Equals(0d);
        
        /// <summary>Оператор неравенства комплексного числа целому числу (1 байт без знака)</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число (1 байт без знака)</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> не равна <paramref name="Y"/>, либо мнимая часть не равна 0</returns>
        public static bool operator !=(Complex X, byte Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);

        /// <summary>Оператор равенства комплексного числа целому числу (2 байта со знаком)</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число (2 байта со знаком)</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> равна <paramref name="Y"/>, а мнимая часть равна 0</returns>
        public static bool operator ==(Complex X, short Y) => X._Re.Equals(Y) && X._Im.Equals(0d);

        /// <summary>Оператор неравенства комплексного числа целому числу (2 байта со знаком)</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число (2 байта со знаком)</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> не равна <paramref name="Y"/>, либо мнимая часть не равна 0</returns>
        public static bool operator !=(Complex X, short Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);

        /// <summary>Оператор равенства комплексного числа целому числу (2 байта без знака)</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число (2 байта без знака)</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> равна <paramref name="Y"/>, а мнимая часть равна 0</returns>
        public static bool operator ==(Complex X, ushort Y) => X._Re.Equals(Y) && X._Im.Equals(0d);

        /// <summary>Оператор неравенства комплексного числа целому числу (2 байта без знака)</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число (2 байта без знака)</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> не равна <paramref name="Y"/>, либо мнимая часть не равна 0</returns>
        public static bool operator !=(Complex X, ushort Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);

        /// <summary>Оператор равенства комплексного числа целому числу</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> равна <paramref name="Y"/>, а мнимая часть равна 0</returns>
        public static bool operator ==(Complex X, int Y) => X._Re.Equals(Y) && X._Im.Equals(0d);

        /// <summary>Оператор неравенства комплексного числа целому числу</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> не равна <paramref name="Y"/>, либо мнимая часть не равна 0</returns>
        public static bool operator !=(Complex X, int Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);

        /// <summary>Оператор равенства комплексного числа целому числу без знака</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число без знака</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> равна <paramref name="Y"/>, а мнимая часть равна 0</returns>
        public static bool operator ==(Complex X, uint Y) => X._Re.Equals(Y) && X._Im.Equals(0d);

        /// <summary>Оператор неравенства комплексного числа целому числу без знака</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число без знака</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> не равна <paramref name="Y"/>, либо мнимая часть не равна 0</returns>
        public static bool operator !=(Complex X, uint Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);

        /// <summary>Оператор неравенства комплексного числа целому числу (8 байт со знаком)</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число (8 байт со знаком)</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> не равна <paramref name="Y"/>, либо мнимая часть не равна 0</returns>
        public static bool operator ==(Complex X, long Y) => X._Re.Equals(Y) && X._Im.Equals(0d);

        /// <summary>Оператор равенства комплексного числа целому числу (8 байт со знаком)</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число (8 байт со знаком)</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> равна <paramref name="Y"/>, а мнимая часть равна 0</returns>
        public static bool operator !=(Complex X, long Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);

        /// <summary>Оператор неравенства комплексного числа целому числу (8 байт без знака)</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число (8 байт без знака)</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> не равна <paramref name="Y"/>, либо мнимая часть не равна 0</returns>
        public static bool operator ==(Complex X, ulong Y) => X._Re.Equals(Y) && X._Im.Equals(0d);

        /// <summary>Оператор равенства комплексного числа целому числу (8 байт без знака)</summary>
        /// <param name="X">Сравниваемое комплексное число</param>
        /// <param name="Y">Сравниваемое целое число (8 байт без знака)</param>
        /// <returns>Истина, если реальная часть <paramref name="X"/> равна <paramref name="Y"/>, а мнимая часть равна 0</returns>
        public static bool operator !=(Complex X, ulong Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);

        #endregion

        /* -------------------------------------------------------------------------------------------- */

        #region Операторы приведения

        /// <summary>Оператор неявного приведения к дробному типу чисел с двойной точностью</summary>
        /// <param name="Z">Приводимое комплексное число</param>
        /// <returns>Модуль комплексного числа</returns>
        public static explicit operator double(Complex Z) => Z.Abs;

        /// <summary>Оператор неявного приведения дробного числа двойной точности к комплексному виду</summary>
        /// <param name="X">Вещественное число двойной точности</param>
        /// <returns>Комплексное число</returns>
        public static implicit operator Complex(in double X) => new(X);

        /// <summary>Оператор неявного приведения целого числа к комплексному виду</summary>
        /// <param name="X">Целое число</param>
        /// <returns>Комплексное число</returns>
        public static implicit operator Complex(in int X) => new(X);

        /// <summary>Оператор неявного приведения кортежа, состоящего из двух вещественных чисел в комплексное число</summary>
        /// <param name="Z">Кортеж из двух вещественных чисел - действительной и мнимой части</param>
        /// <returns>Комплексное число</returns>
        public static implicit operator Complex((double Re, double Im) Z) => new(Z.Re, Z.Im);

        /// <summary>Оператор неявного приведения комплексного числа в кортеж, состоящий из двух вещественных чисел</summary>
        /// <param name="Z">Комплексное число</param>
        /// <returns>Кортеж из двух вещественных чисел - действительной и мнимой части</returns>
        public static implicit operator (double Re, double Im)(Complex Z) => (Z._Re, Z._Im);

        public static implicit operator (Complex Z, Complex Zconj)(Complex Z) => Z.Conjugate();

        #endregion

        /* -------------------------------------------------------------------------------------------- */

    }
}