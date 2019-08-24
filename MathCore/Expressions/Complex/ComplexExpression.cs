using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace MathCore.Expressions.Complex
{
    public abstract class ComplexExpression
    {
        /* -------------------------------------------------------------------------------------------------------------------- */
        #region Static service methods

        protected static ConstantExpression Constant(object value) { return Expression.Constant((double)value); }
        protected static UnaryExpression Negate(Expression value) { return Expression.Negate(value); }
        protected static BinaryExpression Add(Expression Left, Expression Right) { return Expression.Add(Left, Right); }
        protected static BinaryExpression Subtract(Expression Left, Expression Right) { return Expression.Subtract(Left, Right); }
        protected static BinaryExpression Multiply(Expression Left, Expression Right) { return Expression.Multiply(Left, Right); }
        protected static BinaryExpression Divide(Expression Left, Expression Right) { return Expression.Divide(Left, Right); }
        protected static BinaryExpression Divide(Expression Left, double Right) { return Expression.Divide(Left, Constant(Right)); }
        protected static BinaryExpression GetPower(Expression Left, Expression Right) { return Expression.Power(Left, Right); }
        protected static BinaryExpression GetPower(double Left, Expression Right) { return Expression.Power(Constant(Left), Right); }
        protected static BinaryExpression GetPower(Expression Left, double Right) { return Expression.Power(Left, Constant(Right)); }
        protected static Expression Inverse(Expression value) { return Divide(Constant(1d), value); }

        protected static Expression GetAbs(Expression value) { return Expression.Call(((Func<double, double>)Math.Abs).Method, value); }
        protected static Expression GetSqrt(Expression value) { return Expression.Call(((Func<double, double>)Math.Sqrt).Method, value); }
        protected static Expression GetAtan2(Expression y, Expression x) { return Expression.Call(((Func<double, double, double>)Math.Atan2).Method, y, x); }
        protected static Expression GetSin(Expression value) { return Expression.Call(((Func<double, double>)Math.Sin).Method, value); }
        protected static Expression GetCos(Expression value) { return Expression.Call(((Func<double, double>)Math.Cos).Method, value); }
        protected static Expression GetLog(Expression value) { return Expression.Call(((Func<double, double>)Math.Log).Method, value); }

        #endregion

        /* -------------------------------------------------------------------------------------------------------------------- */

        #region Static public methods

        public static CopmlexConstantExpression Mod(MathCore.Complex Z) { return Mod(Z.Re, Z.Im); }
        public static CopmlexConstantExpression Mod(double Re, double Im = 0) { return new CopmlexConstantExpression(Re, Im); }
        public static CopmlexConstantExpression Mod(Expression Re) { return new CopmlexConstantExpression(Re, Constant(0d)); }
        public static CopmlexConstantExpression Mod(Expression Re, Expression Im) { return new CopmlexConstantExpression(Re, Im); }

        public static CopmlexConstantExpression Exp(double Abs, double Arg) { return Exp(Constant(Abs), Constant(Arg)); }

        public static CopmlexConstantExpression Exp(Expression Abs, Expression Arg)
        {
            return new CopmlexConstantExpression(Multiply(Abs, GetCos(Arg)), Multiply(Abs, GetSin(Arg)));
        }

        public static CopmlexConstantExpression Exp(double Arg) { return Exp(Constant(Arg)); }

        public static CopmlexConstantExpression Exp(Expression Arg) { return new CopmlexConstantExpression(GetCos(Arg), GetSin(Arg)); }

        #endregion

        /* -------------------------------------------------------------------------------------------------------------------- */

        #region Fields

        private Expression _Re;
        private Expression _Im;

        #endregion

        /* -------------------------------------------------------------------------------------------------------------------- */

        #region Properties

        public Expression Re => _Re ?? (_Re = GetRe());

        public Expression Im => _Im ?? (_Im = GetIm());

        public Expression Abs => GetSqrt(Power);

        public Expression Power => Add(GetPower(Re, 2), GetPower(Im, 2));

        public Expression Arg => GetAtan2(Im, Re);

        public ComplexExpression ComplexConjugate => new ComplexConjugateExpression(this);

        #endregion

        /* -------------------------------------------------------------------------------------------------------------------- */


        /* -------------------------------------------------------------------------------------------------------------------- */

        protected abstract Expression GetRe();
        protected abstract Expression GetIm();

        public Expression<TDelegate> Lambda<TDelegate>(params ParameterExpression[] Parameters)
        {
            var tDelegate = typeof(TDelegate);

            var tComplex = typeof(MathCore.Complex);
            var constructor = tComplex.GetConstructor(new[] { typeof(double), typeof(double) });
            Debug.Assert(constructor != null, "MathCore.Complex.ctor info != null");
            var expression = Expression.New(constructor, Re, Im);
            return Expression.Lambda<TDelegate>(expression, Parameters);
        }

        /* -------------------------------------------------------------------------------------------------------------------- */

        public static implicit operator ComplexExpression(MathCore.Complex Z) { return Mod(Z); }

        public static ComplexBinaryExpression operator +(int x, ComplexExpression y) => new ComplexAddExpression(Mod(x), y);
        public static ComplexBinaryExpression operator +(float x, ComplexExpression y) => new ComplexAddExpression(Mod(x), y);
        public static ComplexBinaryExpression operator +(double x, ComplexExpression y) => new ComplexAddExpression(Mod(x), y);
        public static ComplexBinaryExpression operator +(MathCore.Complex x, ComplexExpression y) => new ComplexAddExpression(x.Expression, y);
        public static ComplexBinaryExpression operator +(Expression x, ComplexExpression y) => new ComplexAddExpression(Mod(x), y);
        public static ComplexBinaryExpression operator +(ComplexExpression x, int y) => new ComplexAddExpression(x, Mod(y));
        public static ComplexBinaryExpression operator +(ComplexExpression x, float y) => new ComplexAddExpression(x, Mod(y));
        public static ComplexBinaryExpression operator +(ComplexExpression x, double y) => new ComplexAddExpression(x, Mod(y));
        public static ComplexBinaryExpression operator +(ComplexExpression x, MathCore.Complex y) => new ComplexAddExpression(x, y.Expression);
        public static ComplexBinaryExpression operator +(ComplexExpression x, Expression y) => new ComplexAddExpression(x, Mod(y));
        public static ComplexBinaryExpression operator +(ComplexExpression x, ComplexExpression y) => new ComplexAddExpression(x, y);


        public static ComplexBinaryExpression operator -(int x, ComplexExpression y) => new ComplexSubtractExpression(Mod(x), y);
        public static ComplexBinaryExpression operator -(float x, ComplexExpression y) => new ComplexSubtractExpression(Mod(x), y);
        public static ComplexBinaryExpression operator -(double x, ComplexExpression y) => new ComplexSubtractExpression(Mod(x), y);
        public static ComplexBinaryExpression operator -(MathCore.Complex x, ComplexExpression y) => new ComplexSubtractExpression(x.Expression, y);
        public static ComplexBinaryExpression operator -(Expression x, ComplexExpression y) => new ComplexSubtractExpression(Mod(x), y);
        public static ComplexBinaryExpression operator -(ComplexExpression x, int y) => new ComplexSubtractExpression(x, Mod(y));
        public static ComplexBinaryExpression operator -(ComplexExpression x, float y) => new ComplexSubtractExpression(x, Mod(y));
        public static ComplexBinaryExpression operator -(ComplexExpression x, double y) => new ComplexSubtractExpression(x, Mod(y));
        public static ComplexBinaryExpression operator -(ComplexExpression x, MathCore.Complex y) => new ComplexSubtractExpression(x, y.Expression);
        public static ComplexBinaryExpression operator -(ComplexExpression x, Expression y) => new ComplexSubtractExpression(x, Mod(y));
        public static ComplexBinaryExpression operator -(ComplexExpression x, ComplexExpression y) => new ComplexSubtractExpression(x, y);


        public static ComplexBinaryExpression operator *(int x, ComplexExpression y) => new ComplexMultiplyExpression(Mod(x), y);
        public static ComplexBinaryExpression operator *(float x, ComplexExpression y) => new ComplexMultiplyExpression(Mod(x), y);
        public static ComplexBinaryExpression operator *(double x, ComplexExpression y) => new ComplexMultiplyExpression(Mod(x), y);
        public static ComplexBinaryExpression operator *(MathCore.Complex x, ComplexExpression y) => new ComplexMultiplyExpression(x.Expression, y);
        public static ComplexBinaryExpression operator *(Expression x, ComplexExpression y) => new ComplexMultiplyExpression(Mod(x), y);
        public static ComplexBinaryExpression operator *(ComplexExpression x, int y) => new ComplexMultiplyExpression(x, Mod(y));
        public static ComplexBinaryExpression operator *(ComplexExpression x, float y) => new ComplexMultiplyExpression(x, Mod(y));
        public static ComplexBinaryExpression operator *(ComplexExpression x, double y) => new ComplexMultiplyExpression(x, Mod(y));
        public static ComplexBinaryExpression operator *(ComplexExpression x, MathCore.Complex y) => new ComplexMultiplyExpression(x, y.Expression);
        public static ComplexBinaryExpression operator *(ComplexExpression x, Expression y) => new ComplexMultiplyExpression(x, Mod(y));
        public static ComplexBinaryExpression operator *(ComplexExpression x, ComplexExpression y) => new ComplexMultiplyExpression(x, y);


        public static ComplexBinaryExpression operator /(int x, ComplexExpression y) => new ComplexDivideExpression(Mod(x), y);
        public static ComplexBinaryExpression operator /(float x, ComplexExpression y) => new ComplexDivideExpression(Mod(x), y);
        public static ComplexBinaryExpression operator /(double x, ComplexExpression y) => new ComplexDivideExpression(Mod(x), y);
        public static ComplexBinaryExpression operator /(MathCore.Complex x, ComplexExpression y) => new ComplexDivideExpression(x.Expression, y);
        public static ComplexBinaryExpression operator /(Expression x, ComplexExpression y) => new ComplexDivideExpression(Mod(x), y);
        public static ComplexBinaryExpression operator /(ComplexExpression x, int y) => new ComplexDivideExpression(x, Mod(y));
        public static ComplexBinaryExpression operator /(ComplexExpression x, float y) => new ComplexDivideExpression(x, Mod(y));
        public static ComplexBinaryExpression operator /(ComplexExpression x, double y) => new ComplexDivideExpression(x, Mod(y));
        public static ComplexBinaryExpression operator /(ComplexExpression x, MathCore.Complex y) => new ComplexDivideExpression(x, y.Expression);
        public static ComplexBinaryExpression operator /(ComplexExpression x, Expression y) => new ComplexDivideExpression(x, Mod(y));
        public static ComplexBinaryExpression operator /(ComplexExpression x, ComplexExpression y) => new ComplexDivideExpression(x, y);

        public static ComplexBinaryExpression operator ^(int x, ComplexExpression y) => new ComplexPowerExpression(Mod(x), y);
        public static ComplexBinaryExpression operator ^(float x, ComplexExpression y) => new ComplexPowerExpression(Mod(x), y);
        public static ComplexBinaryExpression operator ^(double x, ComplexExpression y) => new ComplexPowerExpression(Mod(x), y);
        public static ComplexBinaryExpression operator ^(MathCore.Complex x, ComplexExpression y) => new ComplexPowerExpression(x.Expression, y);
        public static ComplexBinaryExpression operator ^(Expression x, ComplexExpression y) => new ComplexPowerExpression(Mod(x), y);
        public static ComplexBinaryExpression operator ^(ComplexExpression x, int y) => new ComplexPowerExpression(x, Mod(y));
        public static ComplexBinaryExpression operator ^(ComplexExpression x, float y) => new ComplexPowerExpression(x, Mod(y));
        public static ComplexBinaryExpression operator ^(ComplexExpression x, double y) => new ComplexPowerExpression(x, Mod(y));
        public static ComplexBinaryExpression operator ^(ComplexExpression x, MathCore.Complex y) => new ComplexPowerExpression(x, y.Expression);
        public static ComplexBinaryExpression operator ^(ComplexExpression x, Expression y) => new ComplexPowerExpression(x, Mod(y));
        public static ComplexBinaryExpression operator ^(ComplexExpression x, ComplexExpression y) => new ComplexPowerExpression(x, y);


        /* -------------------------------------------------------------------------------------------------------------------- */
    }
}
