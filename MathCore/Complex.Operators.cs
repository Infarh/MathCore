using System;
using System.Diagnostics.Contracts;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace MathCore
{
    public partial struct Complex
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

        /// <summary>Унарный оператор разности. Возвращает число -Re-jIm</summary>
        /// <param name="z">Комплексное число</param>
        /// <returns>Возвращает -Re-jIm</returns>
        [DST] public static Complex operator -(in Complex z) => new Complex(-z.Re, -z.Im);

        /// <summary>Унарный оператор суммы. Возвращает число Re+jIm</summary>
        /// <param name="z">Комплексное число</param>
        /// <returns>Возвращает Re+jIm</returns>
        public static Complex operator +(in Complex z) => new Complex(z.Re, z.Im);

        /// <summary>Оператор комплексного сопряжения</summary>
        /// <param name="z">Комплексное число</param>
        /// <returns>Возвращает Re-jIm</returns>
        [DST] public static Complex operator !(in Complex z) => z.ComplexConjugate;

        /// <summary>Оператор сложения</summary>
        /// <param name="X">Первое слогаемое</param>
        /// <param name="Y">Второе слогаемое</param>
        /// <returns>Сумма комплексных чисел</returns>
        [DST] public static Complex operator +(in Complex X, in Complex Y) => new Complex(X.Re + Y.Re, X.Im + Y.Im);

        /// <summary>Оператор разности комплексных чисел</summary>
        /// <param name="X">Уменьшаемое комплексное число</param>
        /// <param name="Y">Вычитаемое комплексное число</param>
        /// <returns>Разность комплексных чисел</returns>
        [DST] public static Complex operator -(in Complex X, in Complex Y) => new Complex(X.Re - Y.Re, X.Im - Y.Im);

        /// <summary>Оператор сложения</summary>
        /// <param name="X">Первое слогаемое</param>
        /// <param name="Y">Второе слогаемое</param>
        /// <returns>Сумма комплексных чисел</returns>
        [DST] public static Complex operator +(double X, in Complex Y) => new Complex(X + Y.Re, Y.Im);

        /// <summary>Оператор суммы целого и комплексного числа</summary>
        /// <param name="X">Целое число</param>
        /// <param name="Y">Комплексное число</param>
        /// <returns>X+Re{Y}+jIm{Y}</returns>
        [DST] public static Complex operator +(int X, in Complex Y) => new Complex(X + Y.Re, Y.Im);

        /// <summary>Оператор суммы вещественного числа одинарной точности и комплексного числа</summary>
        /// <param name="X">Вещественное число одинарной точности число</param>
        /// <param name="Y">Комплексное число</param>
        /// <returns>X+Re{Y}+jIm{Y}</returns>
        [DST] public static Complex operator +(float X, in Complex Y) => new Complex(X + Y.Re, Y.Im);

        /// <summary>Оператор разности комплексных чисел</summary>
        /// <param name="X">Уменьшаемое комплексное число</param>
        /// <param name="Y">Вычитаемое комплексное число</param>
        /// <returns>Разность комплексных чисел</returns>
        [DST] public static Complex operator -(double X, in Complex Y) => new Complex(X - Y.Re, -Y.Im);

        /// <summary>Оператор разности целого и комплексного числа</summary>
        /// <param name="X">Целое число</param>
        /// <param name="Y">Комплексное число</param>
        /// <returns>X-Re{Y}-jIm{Y}</returns>
        [DST] public static Complex operator -(int X, in Complex Y) => new Complex(X - Y.Re, -Y.Im);

        /// <summary>Оператор разности вещественного числа одинарной точности и комплексного числа</summary>
        /// <param name="X">Вещественное число одинарной точности число</param>
        /// <param name="Y">Комплексное число</param>
        /// <returns>X-Re{Y}-jIm{Y}</returns>
        [DST] public static Complex operator -(float X, in Complex Y) => new Complex(X - Y.Re, -Y.Im);

        /// <summary>Оператор сложения</summary>
        /// <param name="X">Первое слогаемое</param>
        /// <param name="Y">Второе слогаемое</param>
        /// <returns>Сумма комплексных чисел</returns>
        [DST] public static Complex operator +(in Complex X, double Y) => new Complex(X.Re + Y, X.Im);

        /// <summary>Оператор суммы комплексного числа и целого числа</summary>
        /// <param name="X">Комплексное число</param>
        /// <param name="Y">Целое число</param>
        /// <returns>Re{X}+Y+jIm{X}</returns>
        [DST] public static Complex operator +(in Complex X, int Y) => new Complex(X.Re + Y, X.Im);

        /// <summary>Оператор суммы комплексного числа и вещественного числа одинарной точности</summary>
        /// <param name="X">Комплексное число</param>
        /// <param name="Y">Вещественное число одинарной точности</param>
        /// <returns>Re{X}+Y+jIm{X}</returns>
        [DST] public static Complex operator +(in Complex X, float Y) => new Complex(X.Re + Y, X.Im);

        /// <summary>Оператор разности комплексных чисел</summary>
        /// <param name="X">Уменьшаемое комплексное число</param>
        /// <param name="Y">Вычитаемое комплексное число</param>
        /// <returns>Разность комплексных чисел</returns>
        [DST] public static Complex operator -(in Complex X, double Y) => new Complex(X.Re - Y, X.Im);

        /// <summary>Оператор разности комплексного числа и целого числа</summary>
        /// <param name="X">Комплексное число</param>
        /// <param name="Y">Целое число</param>
        /// <returns>Re{X}-Y+jIm{X}</returns>
        [DST] public static Complex operator -(in Complex X, int Y) => new Complex(X.Re - Y, X.Im);

        /// <summary>Оператор разности комплексного числа и вещественного числа одинарной точности</summary>
        /// <param name="X">Комплексное число</param>
        /// <param name="Y">Вещественное число одинарной точности</param>
        /// <returns>Re{X}-Y+jIm{X}</returns>
        [DST] public static Complex operator -(in Complex X, float Y) => new Complex(X.Re - Y, X.Im);

        /// <summary>Оператор умножения комплексного числа на вещественное</summary>
        /// <param name="X">Комплексное число</param>
        /// <param name="Y">Вещественное число</param>
        /// <returns>Комплексное произведение</returns>
        [DST] public static Complex operator *(in Complex X, double Y) => new Complex(X.Re * Y, X.Im * Y);

        /// <summary>Оператор умножения комплексного числа на целое</summary>
        /// <param name="X">Комплексное число</param>
        /// <param name="Y">Целое число</param>
        /// <returns>Комплексное произведение</returns>
        [DST] public static Complex operator *(in Complex X, int Y) => new Complex(X.Re * Y, X.Im * Y);

        /// <summary>Оператор умножения комплексного числа на вещественное одинарной точности</summary>
        /// <param name="X">Комплексное число</param>
        /// <param name="Y">Вещественное число одинарной точности</param>
        /// <returns>Комплексное произведение</returns>
        [DST] public static Complex operator *(in Complex X, float Y) => new Complex(X.Re * Y, X.Im * Y);

        /// <summary>Оператор деления комплексного числа на вещественное</summary>
        /// <param name="X">Комплексное делимое число</param>
        /// <param name="Y">Вещественный делитель</param>
        /// <returns>Комплексное частное</returns>
        [DST]
        public static Complex operator /(in Complex X, double Y)
        {
            Contract.Requires(!Y.Equals(0d));
            return new Complex(X.Re / Y, X.Im / Y);
        }

        /// <summary>Оператор деления комплексного числа на целое</summary>
        /// <param name="X">Комплексное делимое число</param>
        /// <param name="Y">Целый делитель</param>
        /// <returns>Комплексное частное</returns>
        [DST]
        public static Complex operator /(in Complex X, int Y)
        {
            Contract.Requires(Y != 0);
            return new Complex(X.Re / Y, X.Im / Y);
        }

        /// <summary>Оператор деления комплексного числа на вещественное одинарной точности</summary>
        /// <param name="X">Комплексное делимое число</param>
        /// <param name="Y">Вещественный делитель одинарной точности</param>
        /// <returns>Комплексное частное</returns>
        [DST]
        public static Complex operator /(in Complex X, float Y)
        {
            Contract.Requires(!Y.Equals(0f));
            return new Complex(X.Re / Y, X.Im / Y);
        }

        /// <summary>Оператор произведения вещественного и комплексного числа</summary>
        /// <param name="X">Вещественное число</param>
        /// <param name="Y">Комплексное число</param>
        /// <returns>Комплексное произведение</returns>
        [DST, Pure] public static Complex operator *(double X, in Complex Y) => new Complex(Y.Re * X, Y.Im * X);

        /// <summary>Оператор произведения целого и комплексного числа</summary>
        /// <param name="X">Целое число</param>
        /// <param name="Y">Комплексное число</param>
        /// <returns>Комплексное произведение</returns>
        [DST] public static Complex operator *(int X, in Complex Y) => new Complex(Y.Re * X, Y.Im * X);

        /// <summary>Оператор произведения вещественного числа одинарной точности и комплексного числа</summary>
        /// <param name="X">Вещественное число одинарной точности</param>
        /// <param name="Y">Комплексное число</param>
        /// <returns>Комплексное произведение</returns>
        [DST] public static Complex operator *(float X, in Complex Y) => new Complex(Y.Re * X, Y.Im * X);

        /// <summary>Оператор деления вещественного и комплексного числа</summary>
        /// <param name="X">Вещественное делимое число</param>
        /// <param name="Y">Комплексный делитель</param>
        /// <returns>Комплексное частное</returns>
        [DST]
        public static Complex operator /(double X, in Complex Y)
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
        public static Complex operator /(int X, in Complex Y)
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
        public static Complex operator /(float X, in Complex Y)
        {
            var (re, im) = Y;
            var q = 1 / (re * re + im * im);
            return new Complex(X * re * q, -X * im * q);
        }

        /// <summary>Оператор произведения двух комплексных чисел</summary>
        /// <param name="X">Первый множитель</param>
        /// <param name="Y">Второй множитель</param>
        /// <returns>Комплексное произведение</returns>
        [DST] public static Complex operator *(in Complex X, in Complex Y) => new Complex(X.Re * Y.Re - X.Im * Y.Im, X.Re * Y.Im + X.Im * Y.Re);

        /// <summary>Оператор деления двух комплексных чисел</summary>
        /// <param name="X">Делимое комплексное число</param>
        /// <param name="Y">Делитель комплексного числа</param>
        /// <returns>Частное двух комплексных чисел</returns>
        [DST]
        public static Complex operator /(in Complex X, in Complex Y)
        {
            // Division : Smith's formula.
            var (x_re, x_im) = X;
            var (y_re, y_im) = Y;

            if(double.IsNaN(y_re) || double.IsNaN(y_im))
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
        [DST] public static Complex operator ^(in Complex Z, double X) => Exp(Math.Pow(Z.Abs, X), Z.Arg * X);

        /// <summary>Возведение комплексного числа в вещественную степень одинарной точности по формуле Муавра</summary>
        /// <param name="Z">Возводимое в степень комплексное число Z^X</param>
        /// <param name="X">Вещественный показатель степени одинарной точности Z^X</param>
        /// <returns>Z^X</returns> 
        [DST] public static Complex operator ^(in Complex Z, float X) => Exp(Math.Pow(Z.Abs, X), Z.Arg * X);

        /// <summary>Возведение комплексного числа в целую степень по формуле Муавра</summary>
        /// <param name="Z">Возводимое в степень комплексное число Z^X</param>
        /// <param name="X">Целый показатель степени Z^X</param>
        /// <returns>Z^X</returns> 
        [DST] public static Complex operator ^(in Complex Z, int X) => Exp(Math.Pow(Z.Abs, X), Z.Arg * X);

        /// <summary>Оператор возведения вещественного числа в комплексную степень</summary>
        /// <param name="X">Вещественное число</param>
        /// <param name="Z">Комплексная степень</param>
        /// <returns>Комплексный результат возведения вещественного числа в комплексную степень</returns>
        [DST]
        public static Complex operator ^(double X, in Complex Z)
        {
            var (re, im) = Z;
            if(X >= 0) return Exp(Math.Pow(X, re), Math.Log(X) * im);
            var lnX = Math.Log(Math.Abs(X));
            return Exp(lnX * re - Consts.pi * im, Consts.pi * re + lnX * im);
        }

        /// <summary>Оператор возведения вещественного числа одинарной точности в комплексную степень</summary>
        /// <param name="X">Вещественное число одинарной точности</param>
        /// <param name="Z">Комплексная степень</param>
        /// <returns>Комплексный результат возведения вещественного числа одинарной точности в комплексную степень</returns>
        [DST]
        public static Complex operator ^(float X, in Complex Z)
        {
            var (re, im) = Z;
            if(X >= 0) return Exp(Math.Pow(X, re), Math.Log(X) * im);
            var lnX = Math.Log(Math.Abs(X));
            return Exp(lnX * re - Consts.pi * im, Consts.pi * re + lnX * im);
        }

        /// <summary>Оператор возведения целого числа в комплексную степень</summary>
        /// <param name="X">Целое число</param>
        /// <param name="Z">Комплексная степень</param>
        /// <returns>Комплексный результат возведения целого числа в комплексную степень</returns>
        [DST]
        public static Complex operator ^(int X, in Complex Z)
        {
            var (re, im) = Z;
            if(X >= 0) return Exp(Math.Pow(X, re), Math.Log(X) * im);
            var lnX = Math.Log(Math.Abs(X));
            return Exp(lnX * re - Consts.pi * im, Consts.pi * re + lnX * im);
        }

        /// <summary>Оператор возведения комплексного числа в комплексную степень</summary>
        /// <param name="X">Комплексное основание экспоненты</param>
        /// <param name="Y">Комплексный показатель степени</param>
        /// <returns>Комплексный результат возведения комплексного числа в комплексную степень</returns>
        [DST]
        public static Complex operator ^(in Complex X, in Complex Y)
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

        [DST, NotNull]
        public static Complex[] operator +([NotNull] int[] X, in Complex Y)
        {
            var result = new Complex[X.Length];
            for(var j = 0; j < result.Length; j++) result[j] = X[j] + Y;
            return result;
        }

        [DST, NotNull]
        public static Complex[] operator +([NotNull] float[] X, in Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] + Y;
            return result;
        }

        [DST, NotNull]
        public static Complex[] operator +([NotNull] double[] X, in Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] + Y;
            return result;
        }

        [DST, NotNull]
        public static Complex[] operator -([NotNull] int[] X, in Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] - Y;
            return result;
        }

        [DST, NotNull]
        public static Complex[] operator -([NotNull] float[] X, in Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] - Y;
            return result;
        }

        [DST, NotNull]
        public static Complex[] operator -([NotNull] double[] X, in Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] - Y;
            return result;
        }

        [DST, NotNull]
        public static Complex[] operator *([NotNull] int[] X, in Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] * Y;
            return result;
        }

        [DST, NotNull]
        public static Complex[] operator *([NotNull] float[] X, in Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] * Y;
            return result;
        }

        [DST, NotNull]
        public static Complex[] operator *([NotNull] double[] X, in Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] * Y;
            return result;
        }

        [DST, NotNull]
        public static Complex[] operator /([NotNull] int[] X, in Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] / Y;
            return result;
        }

        [DST, NotNull]
        public static Complex[] operator /([NotNull] float[] X, in Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] / Y;
            return result;
        }

        [DST, NotNull]
        public static Complex[] operator /([NotNull] double[] X, in Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] / Y;
            return result;
        }

        [DST, NotNull]
        public static Complex[] operator +([NotNull] Complex[] X, in Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] + Y;
            return result;
        }

        [DST, NotNull] public static Complex[] operator -([NotNull] Complex[] X, in Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] - Y;
            return result;
        }

        [DST, NotNull]
        public static Complex[] operator *([NotNull] Complex[] X, in Complex Y)
        {
            var result = new Complex[X.Length];
            for (var j = 0; j < result.Length; j++) result[j] = X[j] * Y;
            return result;
        }

        [DST, NotNull]
        public static Complex[] operator /([NotNull] Complex[] X, in Complex Y)
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
        [DST] public static bool operator ==(in Complex X, in Complex Y) => X._Re.Equals(Y._Re) && X._Im.Equals(Y._Im);

        /// <summary>Оператор сравнения двух комплексных чисел (неравенства)</summary>
        /// <returns>Истина, если числа не равны</returns>
        [DST] public static bool operator !=(in Complex X, in Complex Y) => !X._Re.Equals(Y._Re) || !X._Im.Equals(Y._Im);

        public static bool operator ==(in Complex X, float Y) => X._Re.Equals(Y) && X._Im.Equals(0d);
        public static bool operator !=(in Complex X, float Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);

        public static bool operator ==(in Complex X, double Y) => X._Re.Equals(Y) && X._Im.Equals(0d);
        public static bool operator !=(in Complex X, double Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);

        public static bool operator ==(in Complex X, sbyte Y) => X._Re.Equals(Y) && X._Im.Equals(0d);
        public static bool operator !=(in Complex X, sbyte Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);
        public static bool operator ==(in Complex X, byte Y) => X._Re.Equals(Y) && X._Im.Equals(0d);
        public static bool operator !=(in Complex X, byte Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);

        public static bool operator ==(in Complex X, short Y) => X._Re.Equals(Y) && X._Im.Equals(0d);
        public static bool operator !=(in Complex X, short Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);
        public static bool operator ==(in Complex X, ushort Y) => X._Re.Equals(Y) && X._Im.Equals(0d);
        public static bool operator !=(in Complex X, ushort Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);

        public static bool operator ==(in Complex X, int Y) => X._Re.Equals(Y) && X._Im.Equals(0d);
        public static bool operator !=(in Complex X, int Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);
        public static bool operator ==(in Complex X, uint Y) => X._Re.Equals(Y) && X._Im.Equals(0d);
        public static bool operator !=(in Complex X, uint Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);

        public static bool operator ==(in Complex X, long Y) => X._Re.Equals(Y) && X._Im.Equals(0d);
        public static bool operator !=(in Complex X, long Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);
        public static bool operator ==(in Complex X, ulong Y) => X._Re.Equals(Y) && X._Im.Equals(0d);
        public static bool operator !=(in Complex X, ulong Y) => !X._Re.Equals(Y) || !X._Im.Equals(0d);

        #endregion

        /* -------------------------------------------------------------------------------------------- */

        #region Операторы приведения

        /// <summary>ОПератор неявного приведения к дробному типу чисел с двойной точностью</summary>
        /// <param name="Z">Приводимое комплексное число</param>
        /// <returns>Модуль комплексного числа</returns>
        public static explicit operator double(in Complex Z) => Z.Abs;

        /// <summary>Оператор неявного приведения дробного числа двойной точности к комплексному виду</summary>
        /// <param name="X">Вещественное число двойной точности</param>
        /// <returns>Комплексное число</returns>
        public static implicit operator Complex(in double X) => new Complex(X);

        /// <summary>Оператор неявного приведения целого числа к комплексному виду</summary>
        /// <param name="X">Целое число</param>
        /// <returns>Комплексное число</returns>
        public static implicit operator Complex(in int X) => new Complex(X);

        #endregion

        /* -------------------------------------------------------------------------------------------- */

    }
}