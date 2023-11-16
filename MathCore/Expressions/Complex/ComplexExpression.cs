using System.Diagnostics;
using System.Linq.Expressions;

using MathCore.Annotations;

namespace MathCore.Expressions.Complex;

[PublicAPI]
public abstract class ComplexExpression
{
    /* -------------------------------------------------------------------------------------------------------------------- */
    #region Static service methods

    [NotNull]
    protected static ConstantExpression Constant([NotNull] object value) => Expression.Constant((double)value);
    [NotNull]
    protected static UnaryExpression Negate([NotNull] Expression value) => Expression.Negate(value);
    [NotNull]
    protected static BinaryExpression Add([NotNull] Expression Left, [NotNull] Expression Right) => Expression.Add(Left, Right);
    [NotNull]
    protected static BinaryExpression Subtract([NotNull] Expression Left, [NotNull] Expression Right) => Expression.Subtract(Left, Right);
    [NotNull]
    protected static BinaryExpression Multiply([NotNull] Expression Left, [NotNull] Expression Right) => Expression.Multiply(Left, Right);
    [NotNull]
    protected static BinaryExpression Divide([NotNull] Expression Left, [NotNull] Expression Right) => Expression.Divide(Left, Right);
    [NotNull]
    protected static BinaryExpression Divide([NotNull] Expression Left, double Right) => Expression.Divide(Left, Constant(Right));
    [NotNull]
    protected static BinaryExpression GetPower([NotNull] Expression Left, [NotNull] Expression Right) => Expression.Power(Left, Right);
    [NotNull]
    protected static BinaryExpression GetPower(double Left, [NotNull] Expression Right) => Expression.Power(Constant(Left), Right);
    [NotNull]
    protected static BinaryExpression GetPower([NotNull] Expression Left, double Right) => Expression.Power(Left, Constant(Right));
    [NotNull, PublicAPI]
    protected static Expression Inverse([NotNull] Expression value) => Divide(Constant(1d), value);

    [NotNull, PublicAPI]
    protected static Expression GetAbs([NotNull] Expression value) => Expression.Call(((Func<double, double>)Math.Abs).Method, value);
    [NotNull]
    protected static Expression GetSqrt([NotNull] Expression value) => Expression.Call(((Func<double, double>)Math.Sqrt).Method, value);
    [NotNull]
    protected static Expression GetAtan2([NotNull] Expression y, [NotNull] Expression x) => Expression.Call(((Func<double, double, double>)Math.Atan2).Method, y, x);
    [NotNull]
    protected static Expression GetSin([NotNull] Expression value) => Expression.Call(((Func<double, double>)Math.Sin).Method, value);
    [NotNull]
    protected static Expression GetCos([NotNull] Expression value) => Expression.Call(((Func<double, double>)Math.Cos).Method, value);
    [NotNull]
    protected static Expression GetLog([NotNull] Expression value) => Expression.Call(((Func<double, double>)Math.Log).Method, value);

    #endregion

    /* -------------------------------------------------------------------------------------------------------------------- */

    #region Static public methods

    [NotNull, PublicAPI]
    public static ComplexConstantExpression Mod(MathCore.Complex Z) => Mod(Z.Re, Z.Im);
    [NotNull]
    public static ComplexConstantExpression Mod(double Re, double Im = 0) => new(Re, Im);
    [NotNull]
    public static ComplexConstantExpression Mod(Expression Re) => new(Re, Constant(0d));
    [NotNull, PublicAPI]
    public static ComplexConstantExpression Mod(Expression Re, Expression Im) => new(Re, Im);

    [NotNull, PublicAPI]
    public static ComplexConstantExpression Exp(double Abs, double Arg) => Exp(Constant(Abs), Constant(Arg));

    [NotNull]
    public static ComplexConstantExpression Exp([NotNull] Expression Abs, [NotNull] Expression Arg) => new(Multiply(Abs, GetCos(Arg)), Multiply(Abs, GetSin(Arg)));

    [NotNull, PublicAPI]
    public static ComplexConstantExpression Exp(double Arg) => Exp(Constant(Arg));

    [NotNull]
    public static ComplexConstantExpression Exp([NotNull] Expression Arg) => new(GetCos(Arg), GetSin(Arg));

    #endregion

    /* -------------------------------------------------------------------------------------------------------------------- */

    #region Fields

    private Expression _Re;
    private Expression _Im;

    #endregion

    /* -------------------------------------------------------------------------------------------------------------------- */

    #region Properties

    public Expression Re => _Re ??= GetRe();

    public Expression Im => _Im ??= GetIm();

    [NotNull] public Expression Abs => GetSqrt(Power);

    [NotNull] public Expression Power => Add(GetPower(Re, 2), GetPower(Im, 2));

    [NotNull] public Expression Arg => GetAtan2(Im, Re);

    [PublicAPI]
    [NotNull] public ComplexExpression ComplexConjugate => new ComplexConjugateExpression(this);

    #endregion

    /* -------------------------------------------------------------------------------------------------------------------- */


    /* -------------------------------------------------------------------------------------------------------------------- */

    [NotNull] protected abstract Expression GetRe();
    [NotNull] protected abstract Expression GetIm();

    [NotNull, PublicAPI]
    public Expression<TDelegate> Lambda<TDelegate>(params ParameterExpression[] Parameters)
    {
        var t_complex   = typeof(MathCore.Complex);
        var constructor = t_complex.GetConstructor(new[] { typeof(double), typeof(double) });
        Debug.Assert(constructor != null, "MathCore.Complex.ctor info != null");
        var expression = Expression.New(constructor, Re, Im);
        return Expression.Lambda<TDelegate>(expression, Parameters);
    }

    /* -------------------------------------------------------------------------------------------------------------------- */

    public static implicit operator ComplexExpression(MathCore.Complex Z) => Mod(Z);

    [NotNull] public static ComplexBinaryExpression operator +(int x, ComplexExpression y) => new ComplexAddExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator +(float x, ComplexExpression y) => new ComplexAddExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator +(double x, ComplexExpression y) => new ComplexAddExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator +(MathCore.Complex x, ComplexExpression y) => new ComplexAddExpression(x.Expression, y);
    [NotNull] public static ComplexBinaryExpression operator +(Expression x, ComplexExpression y) => new ComplexAddExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator +(ComplexExpression x, int y) => new ComplexAddExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator +(ComplexExpression x, float y) => new ComplexAddExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator +(ComplexExpression x, double y) => new ComplexAddExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator +(ComplexExpression x, MathCore.Complex y) => new ComplexAddExpression(x, y.Expression);
    [NotNull] public static ComplexBinaryExpression operator +(ComplexExpression x, Expression y) => new ComplexAddExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator +(ComplexExpression x, ComplexExpression y) => new ComplexAddExpression(x, y);


    [NotNull] public static ComplexBinaryExpression operator -(int x, ComplexExpression y) => new ComplexSubtractExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator -(float x, ComplexExpression y) => new ComplexSubtractExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator -(double x, ComplexExpression y) => new ComplexSubtractExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator -(MathCore.Complex x, ComplexExpression y) => new ComplexSubtractExpression(x.Expression, y);
    [NotNull] public static ComplexBinaryExpression operator -(Expression x, ComplexExpression y) => new ComplexSubtractExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator -(ComplexExpression x, int y) => new ComplexSubtractExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator -(ComplexExpression x, float y) => new ComplexSubtractExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator -(ComplexExpression x, double y) => new ComplexSubtractExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator -(ComplexExpression x, MathCore.Complex y) => new ComplexSubtractExpression(x, y.Expression);
    [NotNull] public static ComplexBinaryExpression operator -(ComplexExpression x, Expression y) => new ComplexSubtractExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator -(ComplexExpression x, ComplexExpression y) => new ComplexSubtractExpression(x, y);


    [NotNull] public static ComplexBinaryExpression operator *(int x, ComplexExpression y) => new ComplexMultiplyExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator *(float x, ComplexExpression y) => new ComplexMultiplyExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator *(double x, ComplexExpression y) => new ComplexMultiplyExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator *(MathCore.Complex x, ComplexExpression y) => new ComplexMultiplyExpression(x.Expression, y);
    [NotNull] public static ComplexBinaryExpression operator *(Expression x, ComplexExpression y) => new ComplexMultiplyExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator *(ComplexExpression x, int y) => new ComplexMultiplyExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator *(ComplexExpression x, float y) => new ComplexMultiplyExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator *(ComplexExpression x, double y) => new ComplexMultiplyExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator *(ComplexExpression x, MathCore.Complex y) => new ComplexMultiplyExpression(x, y.Expression);
    [NotNull] public static ComplexBinaryExpression operator *(ComplexExpression x, Expression y) => new ComplexMultiplyExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator *(ComplexExpression x, ComplexExpression y) => new ComplexMultiplyExpression(x, y);


    [NotNull] public static ComplexBinaryExpression operator /(int x, ComplexExpression y) => new ComplexDivideExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator /(float x, ComplexExpression y) => new ComplexDivideExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator /(double x, ComplexExpression y) => new ComplexDivideExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator /(MathCore.Complex x, ComplexExpression y) => new ComplexDivideExpression(x.Expression, y);
    [NotNull] public static ComplexBinaryExpression operator /(Expression x, ComplexExpression y) => new ComplexDivideExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator /(ComplexExpression x, int y) => new ComplexDivideExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator /(ComplexExpression x, float y) => new ComplexDivideExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator /(ComplexExpression x, double y) => new ComplexDivideExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator /(ComplexExpression x, MathCore.Complex y) => new ComplexDivideExpression(x, y.Expression);
    [NotNull] public static ComplexBinaryExpression operator /(ComplexExpression x, Expression y) => new ComplexDivideExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator /(ComplexExpression x, ComplexExpression y) => new ComplexDivideExpression(x, y);

    [NotNull] public static ComplexBinaryExpression operator ^(int x, ComplexExpression y) => new ComplexPowerExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator ^(float x, ComplexExpression y) => new ComplexPowerExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator ^(double x, ComplexExpression y) => new ComplexPowerExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator ^(MathCore.Complex x, ComplexExpression y) => new ComplexPowerExpression(x.Expression, y);
    [NotNull] public static ComplexBinaryExpression operator ^(Expression x, ComplexExpression y) => new ComplexPowerExpression(Mod(x), y);
    [NotNull] public static ComplexBinaryExpression operator ^(ComplexExpression x, int y) => new ComplexPowerExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator ^(ComplexExpression x, float y) => new ComplexPowerExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator ^(ComplexExpression x, double y) => new ComplexPowerExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator ^(ComplexExpression x, MathCore.Complex y) => new ComplexPowerExpression(x, y.Expression);
    [NotNull] public static ComplexBinaryExpression operator ^(ComplexExpression x, Expression y) => new ComplexPowerExpression(x, Mod(y));
    [NotNull] public static ComplexBinaryExpression operator ^(ComplexExpression x, ComplexExpression y) => new ComplexPowerExpression(x, y);


    /* -------------------------------------------------------------------------------------------------------------------- */
}