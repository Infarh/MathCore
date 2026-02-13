using System.Linq.Expressions;

using MathCore.Vectors;

using bEx = System.Linq.Expressions.BinaryExpression;
using cEx = System.Linq.Expressions.ConstantExpression;
using Ex = System.Linq.Expressions.Expression;
using uEx = System.Linq.Expressions.UnaryExpression;
// ReSharper disable MergeCastWithTypeCheck
// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable ConvertIfStatementToSwitchStatement
// ReSharper disable InvertIf

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace MathCore.Extensions.Expressions;

public static partial class ExpressionExtensions
{
    private static class ExpressionSimplifierRules
    {
        public static Ex Binary(object Sender, EventArgs<bEx> Args)
        {
            var expr = Args.Argument;
            return expr.NodeType switch
            {
                ExpressionType.Add or ExpressionType.AddChecked => AdditionSimplify(expr),
                //case ExpressionType.And:
                //    break;
                //case ExpressionType.AndAlso:
                //    break;
                //case ExpressionType.Coalesce:
                //    break;
                ExpressionType.Divide => DivideSimplify(expr),
                //case ExpressionType.Equal:
                //    break;
                //case ExpressionType.ExclusiveOr:
                //    break;
                //case ExpressionType.GreaterThan:
                //    break;
                //case ExpressionType.GreaterThanOrEqual:
                //    break;
                //case ExpressionType.LeftShift:
                //    break;
                //case ExpressionType.LessThan:
                //    break;
                //case ExpressionType.LessThanOrEqual:
                //    break;
                //case ExpressionType.Modulo:
                //    break;
                ExpressionType.Multiply or ExpressionType.MultiplyChecked => MultiplySimplify(expr),
                //case ExpressionType.NotEqual:
                //    break;
                //case ExpressionType.Or:
                //    break;
                //case ExpressionType.OrElse:
                //    break;
                //case ExpressionType.Power:
                //    break;
                //case ExpressionType.RightShift:
                //    break;
                ExpressionType.Subtract => SubtractionSimplify(expr),
                _ => expr,
            };
        }

        #region Is...?

        private static bool IsNumeric(object? value) => value
            is byte
            or sbyte
            or short
            or ushort
            or int
            or uint
            or long
            or ulong
            or float
            or double
            or Complex
            or Vector2D
            or Vector3D;

        private static bool IsZero(object? value) =>
            value is ((byte)0) or ((sbyte)0) or ((short)0) or ((ushort)0) or 0 or 0u or 0L or 0ul or 0f or 0d
            || (value is Complex && ((Complex)0).Equals(value))
            || (value is Vector2D && ((Vector2D)0).Equals(value))
            || (value is Vector3D && ((Vector3D)0).Equals(value))
        ;

        private static bool IsUnit(object? value) =>
            value is ((byte)1) or ((sbyte)1) or ((short)1) or ((ushort)1) or 1 or 1u or 1L or 1ul or 1f or 1d
            || (value is Complex && Complex.Real.Equals(value))
        ;

        #endregion

        private static Ex MultiplySimplify(bEx expr)
        {
            //var is_checked = expr.NodeType == ExpressionType.MultiplyChecked;
            if (IsZero((expr.Left as cEx)?.Value)) return expr.Left;
            if (IsUnit((expr.Left as cEx)?.Value)) return expr.Right;

            if (IsZero((expr.Right as cEx)?.Value)) return expr.Right;
            if (IsUnit((expr.Right as cEx)?.Value)) return expr.Left;

            return MultiplyValues((expr.Left as cEx)?.Value, (expr.Right as cEx)?.Value) ?? expr;
        }

        private static Ex? MultiplyValues(object? left, object? right)
        {
            if (!IsNumeric(left) || !IsNumeric(right)) return null;
            return left switch
            {
                byte left1 => right switch
                {
                    byte b => (left1 * b).ToExpression(),
                    sbyte right1 => (left1 * right1).ToExpression(),
                    short s => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i => (left1 * i).ToExpression(),
                    uint u => (left1 * u).ToExpression(),
                    long l => (left1 * l).ToExpression(),
                    ulong right1 => (left1 * right1).ToExpression(),
                    float f => (left1 * f).ToExpression(),
                    double d => (left1 * d).ToExpression(),
                    Complex complex => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _ => (left1 * (right as Vector3D?))?.ToExpression()
                },
                sbyte left1 => right switch
                {
                    byte b => (left1 * b).ToExpression(),
                    sbyte right1 => (left1 * right1).ToExpression(),
                    short s => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i => (left1 * i).ToExpression(),
                    uint u => (left1 * u).ToExpression(),
                    long l => (left1 * l).ToExpression(),
                    //ulong right1 => (left1 * right1).ToExpression(),
                    float f => (left1 * f).ToExpression(),
                    double d => (left1 * d).ToExpression(),
                    Complex complex => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _ => (left1 * (right as Vector3D?))?.ToExpression()
                },
                short left1 => right switch
                {
                    byte b => (left1 * b).ToExpression(),
                    sbyte right1 => (left1 * right1).ToExpression(),
                    short s => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i => (left1 * i).ToExpression(),
                    uint u => (left1 * u).ToExpression(),
                    long l => (left1 * l).ToExpression(),
                    //ulong right1 => (left1 * right1).ToExpression(),
                    float f => (left1 * f).ToExpression(),
                    double d => (left1 * d).ToExpression(),
                    Complex complex => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _ => (left1 * (right as Vector3D?))?.ToExpression()
                },
                ushort left1 => right switch
                {
                    byte b => (left1 * b).ToExpression(),
                    sbyte right1 => (left1 * right1).ToExpression(),
                    short s => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i => (left1 * i).ToExpression(),
                    uint u => (left1 * u).ToExpression(),
                    long l => (left1 * l).ToExpression(),
                    ulong right1 => (left1 * right1).ToExpression(),
                    float f => (left1 * f).ToExpression(),
                    double d => (left1 * d).ToExpression(),
                    Complex complex => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _ => (left1 * (right as Vector3D?))?.ToExpression()
                },
                int left1 => right switch
                {
                    byte b => (left1 * b).ToExpression(),
                    sbyte right1 => (left1 * right1).ToExpression(),
                    short s => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i => (left1 * i).ToExpression(),
                    uint u => (left1 * u).ToExpression(),
                    long l => (left1 * l).ToExpression(),
                    //ulong right1 => (left1 * right1).ToExpression(),
                    float f => (left1 * f).ToExpression(),
                    double d => (left1 * d).ToExpression(),
                    Complex complex => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _ => (left1 * (right as Vector3D?))?.ToExpression()
                },
                uint left1 => right switch
                {
                    byte b => (left1 * b).ToExpression(),
                    sbyte right1 => (left1 * right1).ToExpression(),
                    short s => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i => (left1 * i).ToExpression(),
                    uint u => (left1 * u).ToExpression(),
                    long l => (left1 * l).ToExpression(),
                    ulong right1 => (left1 * right1).ToExpression(),
                    float f => (left1 * f).ToExpression(),
                    double d => (left1 * d).ToExpression(),
                    Complex complex => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _ => (left1 * (right as Vector3D?))?.ToExpression()
                },
                long left1 => right switch
                {
                    byte b => (left1 * b).ToExpression(),
                    sbyte right1 => (left1 * right1).ToExpression(),
                    short s => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i => (left1 * i).ToExpression(),
                    uint u => (left1 * u).ToExpression(),
                    long l => (left1 * l).ToExpression(),
                    //ulong right1 => (left1 * right1).ToExpression(),
                    float f => (left1 * f).ToExpression(),
                    double d => (left1 * d).ToExpression(),
                    Complex complex => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _ => (left1 * (right as Vector3D?))?.ToExpression()
                },
                ulong left1 => right switch
                {
                    byte b => (left1 * b).ToExpression(),
                    //sbyte right1 => (left1 * right1).ToExpression(),
                    //short s => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    //int i => (left1 * i).ToExpression(),
                    //uint u => (left1 * u).ToExpression(),
                    //long l => (left1 * l).ToExpression(),
                    ulong right1 => (left1 * right1).ToExpression(),
                    float f => (left1 * f).ToExpression(),
                    double d => (left1 * d).ToExpression(),
                    Complex complex => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _ => (left1 * (right as Vector3D?))?.ToExpression()
                },
                float left1 => right switch
                {
                    byte b => (left1 * b).ToExpression(),
                    sbyte right1 => (left1 * right1).ToExpression(),
                    short s => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i => (left1 * i).ToExpression(),
                    uint u => (left1 * u).ToExpression(),
                    long l => (left1 * l).ToExpression(),
                    ulong right1 => (left1 * right1).ToExpression(),
                    float f => (left1 * f).ToExpression(),
                    double d => (left1 * d).ToExpression(),
                    Complex complex => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _ => (left1 * (right as Vector3D?))?.ToExpression()
                },
                double left1 => right switch
                {
                    byte b => (left1 * b).ToExpression(),
                    sbyte right1 => (left1 * right1).ToExpression(),
                    short s => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i => (left1 * i).ToExpression(),
                    uint u => (left1 * u).ToExpression(),
                    long l => (left1 * l).ToExpression(),
                    ulong right1 => (left1 * right1).ToExpression(),
                    float f => (left1 * f).ToExpression(),
                    double d => (left1 * d).ToExpression(),
                    Complex complex => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _ => (left1 * (right as Vector3D?))?.ToExpression()
                },
                Complex left1 => right switch
                {
                    byte b => (left1 * b).ToExpression(),
                    sbyte right1 => (left1 * right1).ToExpression(),
                    short s => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i => (left1 * i).ToExpression(),
                    uint u => (left1 * u).ToExpression(),
                    long l => (left1 * l).ToExpression(),
                    ulong right1 => (left1 * right1).ToExpression(),
                    float f => (left1 * f).ToExpression(),
                    double d => (left1 * d).ToExpression(),
                    Complex complex => (left1 * complex).ToExpression(),
                    //Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _ => null
                },
                Vector2D left1 => right switch
                {
                    byte b => (left1 * b).ToExpression(),
                    sbyte right1 => (left1 * right1).ToExpression(),
                    short s => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i => (left1 * i).ToExpression(),
                    uint u => (left1 * u).ToExpression(),
                    long l => (left1 * l).ToExpression(),
                    ulong right1 => (left1 * right1).ToExpression(),
                    float f => (left1 * f).ToExpression(),
                    double d => (left1 * d).ToExpression(),
                    //Complex complex => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _ => null
                },
                Vector3D vector_3d => right switch
                {
                    byte b => (vector_3d * b).ToExpression(),
                    sbyte right1 => (vector_3d * right1).ToExpression(),
                    short s => (vector_3d * s).ToExpression(),
                    ushort right1 => (vector_3d * right1).ToExpression(),
                    int i => (vector_3d * i).ToExpression(),
                    uint u => (vector_3d * u).ToExpression(),
                    long l => (vector_3d * l).ToExpression(),
                    ulong right1 => (vector_3d * right1).ToExpression(),
                    float f => (vector_3d * f).ToExpression(),
                    double d => (vector_3d * d).ToExpression(),
                    Complex complex => (vector_3d * complex).ToExpression(),
                    Vector2D vector_2d => (vector_3d * vector_2d).ToExpression(),
                    _ => (vector_3d * (right as Vector3D?))?.ToExpression()
                },
                _ => null
            };
        }

        private static Ex DivideSimplify(bEx expr)
        {
            if (IsZero((expr.Left as cEx)?.Value)) return expr.Left;
            if (IsUnit((expr.Right as cEx)?.Value)) return expr.Left;

            return DivideValues((expr.Left as cEx)?.Value!, (expr.Right as cEx)?.Value!) ?? expr;
        }

        private static Ex? DivideValues(object left, object right)
        {
            if (!IsNumeric(left) || !IsNumeric(right)) return null;
            if (left is byte)
            {
                if (IsZero(right))
                {
                    return right switch
                    {
                        double.NaN => double.NaN.ToExpression(),
                        double and > 0 => double.PositiveInfinity.ToExpression(),
                        double and < 0 => double.NegativeInfinity.ToExpression(),

                        float.NaN => float.NaN.ToExpression(),
                        float and > 0 => float.PositiveInfinity.ToExpression(),
                        float and < 0 => float.NegativeInfinity.ToExpression(),

                        _ => Ex.Throw(new DivideByZeroException().ToExpression())
                    };


                    //if (right is double)
                    //    return double.IsNaN((double)right)
                    //        ? double.NaN.ToExpression()
                    //        : ((double)right > 0 ? double.PositiveInfinity : double.NegativeInfinity).ToExpression();
                    //if (right is not float) 
                    //    return Ex.Throw(new DivideByZeroException().ToExpression());
                    //return float.IsNaN((float)right)
                    //    ? float.NaN.ToExpression()
                    //    : ((float)right > 0 ? float.PositiveInfinity : float.NegativeInfinity).ToExpression();
                }

                return right switch
                {
                    byte b => ((byte)left / b).ToExpression(),
                    sbyte right1 => ((byte)left / right1).ToExpression(),
                    short s => ((byte)left / s).ToExpression(),
                    ushort right1 => ((byte)left / right1).ToExpression(),
                    int i => ((byte)left / i).ToExpression(),
                    uint u => ((byte)left / u).ToExpression(),
                    long l => ((byte)left / l).ToExpression(),
                    ulong right1 => ((byte)left / right1).ToExpression(),
                    float f => ((byte)left / f).ToExpression(),
                    double d => ((byte)left / d).ToExpression(),
                    Complex complex => ((byte)left / complex).ToExpression(),
                    Vector2D vector_2d => ((byte)left / vector_2d).ToExpression(),
                    _ => ((byte)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is sbyte)
            {
                if (IsZero(right))
                    return ((sbyte)left, right) switch
                    {
                        (_, double.NaN) => double.NaN.ToExpression(),
                        ( > 0, double and > 0) => double.PositiveInfinity.ToExpression(),
                        (_, double and > 0) => double.NegativeInfinity.ToExpression(),
                        ( > 0, double and < 0) => double.NegativeInfinity.ToExpression(),
                        (_, double and < 0) => double.PositiveInfinity.ToExpression(),

                        (_, float.NaN) => float.NaN.ToExpression(),
                        ( > 0, float and > 0) => float.PositiveInfinity.ToExpression(),
                        (_, float and > 0) => float.NegativeInfinity.ToExpression(),
                        ( > 0, float and < 0) => float.NegativeInfinity.ToExpression(),
                        (_, float and < 0) => float.PositiveInfinity.ToExpression(),

                        _ => Ex.Throw(new DivideByZeroException().ToExpression())
                    };

                return right switch
                {
                    byte b => ((sbyte)left / b).ToExpression(),
                    sbyte right1 => ((sbyte)left / right1).ToExpression(),
                    short s => ((sbyte)left / s).ToExpression(),
                    ushort right1 => ((sbyte)left / right1).ToExpression(),
                    int i => ((sbyte)left / i).ToExpression(),
                    uint u => ((sbyte)left / u).ToExpression(),
                    long l => ((sbyte)left / l).ToExpression(),
                    //ulong right1 => ((sbyte)left / right1).ToExpression(),
                    float f => ((sbyte)left / f).ToExpression(),
                    double d => ((sbyte)left / d).ToExpression(),
                    Complex complex => ((sbyte)left / complex).ToExpression(),
                    Vector2D vector_2d => ((sbyte)left / vector_2d).ToExpression(),
                    _ => ((sbyte)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is short)
            {
                if (IsZero(right))
                    return ((short)left, right) switch
                    {
                        (_, double.NaN) => double.NaN.ToExpression(),
                        ( > 0, double and > 0) => double.PositiveInfinity.ToExpression(),
                        (_, double and > 0) => double.NegativeInfinity.ToExpression(),
                        ( > 0, double and < 0) => double.NegativeInfinity.ToExpression(),
                        (_, double and < 0) => double.PositiveInfinity.ToExpression(),

                        (_, float.NaN) => float.NaN.ToExpression(),
                        ( > 0, float and > 0) => float.PositiveInfinity.ToExpression(),
                        (_, float and > 0) => float.NegativeInfinity.ToExpression(),
                        ( > 0, float and < 0) => float.NegativeInfinity.ToExpression(),
                        (_, float and < 0) => float.PositiveInfinity.ToExpression(),

                        _ => Ex.Throw(new DivideByZeroException().ToExpression())
                    };

                return right switch
                {
                    byte b => ((short)left / b).ToExpression(),
                    sbyte right1 => ((short)left / right1).ToExpression(),
                    short s => ((short)left / s).ToExpression(),
                    ushort right1 => ((short)left / right1).ToExpression(),
                    int i => ((short)left / i).ToExpression(),
                    uint u => ((short)left / u).ToExpression(),
                    long l => ((short)left / l).ToExpression(),
                    //ulong right1 => ((short)left / right1).ToExpression(),
                    float f => ((short)left / f).ToExpression(),
                    double d => ((short)left / d).ToExpression(),
                    Complex complex => ((short)left / complex).ToExpression(),
                    Vector2D vector_2d => ((short)left / vector_2d).ToExpression(),
                    _ => ((short)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is ushort)
            {
                if (IsZero(right))
                {
                    if (right is double)
                        return double.IsNaN((double)right)
                            ? double.NaN.ToExpression()
                            : ((double)right > 0 ? double.PositiveInfinity : double.NegativeInfinity).ToExpression();
                    if (right is not float) return Ex.Throw(new DivideByZeroException().ToExpression());
                    return float.IsNaN((float)right)
                        ? float.NaN.ToExpression()
                        : ((float)right > 0 ? float.PositiveInfinity : float.NegativeInfinity).ToExpression();
                }

                return right switch
                {
                    byte b => ((ushort)left / b).ToExpression(),
                    sbyte right1 => ((ushort)left / right1).ToExpression(),
                    short s => ((ushort)left / s).ToExpression(),
                    ushort right1 => ((ushort)left / right1).ToExpression(),
                    int i => ((ushort)left / i).ToExpression(),
                    uint u => ((ushort)left / u).ToExpression(),
                    long l => ((ushort)left / l).ToExpression(),
                    ulong right1 => ((ushort)left / right1).ToExpression(),
                    float f => ((ushort)left / f).ToExpression(),
                    double d => ((ushort)left / d).ToExpression(),
                    Complex complex => ((ushort)left / complex).ToExpression(),
                    Vector2D vector_2d => ((ushort)left / vector_2d).ToExpression(),
                    _ => ((ushort)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is int)
            {
                if (IsZero(right))
                    return ((int)left, right) switch
                    {
                        (_, double.NaN) => double.NaN.ToExpression(),
                        ( > 0, double and > 0) => double.PositiveInfinity.ToExpression(),
                        (_, double and > 0) => double.NegativeInfinity.ToExpression(),
                        ( > 0, double and < 0) => double.NegativeInfinity.ToExpression(),
                        (_, double and < 0) => double.PositiveInfinity.ToExpression(),

                        (_, float.NaN) => float.NaN.ToExpression(),
                        ( > 0, float and > 0) => float.PositiveInfinity.ToExpression(),
                        (_, float and > 0) => float.NegativeInfinity.ToExpression(),
                        ( > 0, float and < 0) => float.NegativeInfinity.ToExpression(),
                        (_, float and < 0) => float.PositiveInfinity.ToExpression(),

                        _ => Ex.Throw(new DivideByZeroException().ToExpression())
                    };

                return right switch
                {
                    byte b => ((int)left / b).ToExpression(),
                    sbyte right1 => ((int)left / right1).ToExpression(),
                    short s => ((int)left / s).ToExpression(),
                    ushort right1 => ((int)left / right1).ToExpression(),
                    int i => ((int)left / i).ToExpression(),
                    uint u => ((int)left / u).ToExpression(),
                    long l => ((int)left / l).ToExpression(),
                    //ulong right1 => ((int)left / right1).ToExpression(),
                    float f => ((int)left / f).ToExpression(),
                    double d => ((int)left / d).ToExpression(),
                    Complex complex => ((int)left / complex).ToExpression(),
                    Vector2D vector_2d => ((int)left / vector_2d).ToExpression(),
                    _ => ((int)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is uint)
            {
                if (IsZero(right))
                {
                    if (right is double)
                    {
                        if (double.IsNaN((double)right)) return double.NaN.ToExpression();
                        return ((double)right > 0 ? double.PositiveInfinity : double.NegativeInfinity).ToExpression();
                    }
                    if (right is float)
                    {
                        if (float.IsNaN((float)right)) return float.NaN.ToExpression();
                        return ((float)right > 0 ? float.PositiveInfinity : float.NegativeInfinity).ToExpression();
                    }
                    return Ex.Throw(new DivideByZeroException().ToExpression());
                }

                return right switch
                {
                    byte b => ((uint)left / b).ToExpression(),
                    sbyte right1 => ((uint)left / right1).ToExpression(),
                    short s => ((uint)left / s).ToExpression(),
                    ushort right1 => ((uint)left / right1).ToExpression(),
                    int i => ((uint)left / i).ToExpression(),
                    uint u => ((uint)left / u).ToExpression(),
                    long l => ((uint)left / l).ToExpression(),
                    ulong right1 => ((uint)left / right1).ToExpression(),
                    float f => ((uint)left / f).ToExpression(),
                    double d => ((uint)left / d).ToExpression(),
                    Complex complex => ((uint)left / complex).ToExpression(),
                    Vector2D vector_2d => ((uint)left / vector_2d).ToExpression(),
                    _ => ((uint)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is long)
            {
                if (IsZero(right))
                    return ((long)left, right) switch
                    {
                        (_, double.NaN) => double.NaN.ToExpression(),
                        ( > 0, double and > 0) => double.PositiveInfinity.ToExpression(),
                        (_, double and > 0) => double.NegativeInfinity.ToExpression(),
                        ( > 0, double and < 0) => double.NegativeInfinity.ToExpression(),
                        (_, double and < 0) => double.PositiveInfinity.ToExpression(),

                        (_, float.NaN) => float.NaN.ToExpression(),
                        ( > 0, float and > 0) => float.PositiveInfinity.ToExpression(),
                        (_, float and > 0) => float.NegativeInfinity.ToExpression(),
                        ( > 0, float and < 0) => float.NegativeInfinity.ToExpression(),
                        (_, float and < 0) => float.PositiveInfinity.ToExpression(),

                        _ => Ex.Throw(new DivideByZeroException().ToExpression())
                    };

                return right switch
                {
                    byte b => ((long)left / b).ToExpression(),
                    sbyte right1 => ((long)left / right1).ToExpression(),
                    short s => ((long)left / s).ToExpression(),
                    ushort right1 => ((long)left / right1).ToExpression(),
                    int i => ((long)left / i).ToExpression(),
                    uint u => ((long)left / u).ToExpression(),
                    long l => ((long)left / l).ToExpression(),
                    //ulong right1 => ((long)left / right1).ToExpression(),
                    float f => ((long)left / f).ToExpression(),
                    double d => ((long)left / d).ToExpression(),
                    Complex complex => ((long)left / complex).ToExpression(),
                    Vector2D vector_2d => ((long)left / vector_2d).ToExpression(),
                    _ => ((long)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is ulong)
            {
                if (IsZero(right))
                {
                    if (right is double)
                    {
                        if (double.IsNaN((double)right)) return double.NaN.ToExpression();
                        return ((double)right > 0 ? double.PositiveInfinity : double.NegativeInfinity).ToExpression();
                    }
                    if (right is float)
                    {
                        if (float.IsNaN((float)right)) return float.NaN.ToExpression();
                        return ((float)right > 0 ? float.PositiveInfinity : float.NegativeInfinity).ToExpression();
                    }
                    return Ex.Throw(new DivideByZeroException().ToExpression());
                }

                return right switch
                {
                    byte b => ((ulong)left / b).ToExpression(),
                    //sbyte right1 => ((ulong)left / right1).ToExpression(),
                    //short s => ((ulong)left / s).ToExpression(),
                    ushort right1 => ((ulong)left / right1).ToExpression(),
                    //int i => ((ulong)left / i).ToExpression(),
                    uint u => ((ulong)left / u).ToExpression(),
                    //long l => ((ulong)left / l).ToExpression(),
                    ulong right1 => ((ulong)left / right1).ToExpression(),
                    float f => ((ulong)left / f).ToExpression(),
                    double d => ((ulong)left / d).ToExpression(),
                    Complex complex => ((ulong)left / complex).ToExpression(),
                    Vector2D vector_2d => ((ulong)left / vector_2d).ToExpression(),
                    _ => ((ulong)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is float)
            {
                if (IsZero(right))
                    return ((float)left, right) switch
                    {
                        (_, double.NaN) => double.NaN.ToExpression(),
                        ( > 0, double and > 0) => double.PositiveInfinity.ToExpression(),
                        (_, double and > 0) => double.NegativeInfinity.ToExpression(),
                        ( > 0, double and < 0) => double.NegativeInfinity.ToExpression(),
                        (_, double and < 0) => double.PositiveInfinity.ToExpression(),

                        (_, float.NaN) => float.NaN.ToExpression(),
                        ( > 0, float and > 0) => float.PositiveInfinity.ToExpression(),
                        (_, float and > 0) => float.NegativeInfinity.ToExpression(),
                        ( > 0, float and < 0) => float.NegativeInfinity.ToExpression(),
                        (_, float and < 0) => float.PositiveInfinity.ToExpression(),

                        _ => Ex.Throw(new DivideByZeroException().ToExpression())
                    };

                return right switch
                {
                    byte b => ((float)left / b).ToExpression(),
                    sbyte right1 => ((float)left / right1).ToExpression(),
                    short s => ((float)left / s).ToExpression(),
                    ushort right1 => ((float)left / right1).ToExpression(),
                    int i => ((float)left / i).ToExpression(),
                    uint u => ((float)left / u).ToExpression(),
                    long l => ((float)left / l).ToExpression(),
                    ulong right1 => ((float)left / right1).ToExpression(),
                    float f => ((float)left / f).ToExpression(),
                    double d => ((float)left / d).ToExpression(),
                    Complex complex => ((float)left / complex).ToExpression(),
                    Vector2D vector_2d => ((float)left / vector_2d).ToExpression(),
                    _ => ((float)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is double)
            {
                if (IsZero(right))
                    return ((double)left, right) switch
                    {
                        (_, double.NaN) => double.NaN.ToExpression(),
                        ( > 0, double and > 0) => double.PositiveInfinity.ToExpression(),
                        (_, double and > 0) => double.NegativeInfinity.ToExpression(),
                        ( > 0, double and < 0) => double.NegativeInfinity.ToExpression(),
                        (_, double and < 0) => double.PositiveInfinity.ToExpression(),

                        (_, float.NaN) => float.NaN.ToExpression(),
                        ( > 0, float and > 0) => float.PositiveInfinity.ToExpression(),
                        (_, float and > 0) => float.NegativeInfinity.ToExpression(),
                        ( > 0, float and < 0) => float.NegativeInfinity.ToExpression(),
                        (_, float and < 0) => float.PositiveInfinity.ToExpression(),

                        _ => Ex.Throw(new DivideByZeroException().ToExpression())
                    };

                return right switch
                {
                    byte b => ((double)left / b).ToExpression(),
                    sbyte right1 => ((double)left / right1).ToExpression(),
                    short s => ((double)left / s).ToExpression(),
                    ushort right1 => ((double)left / right1).ToExpression(),
                    int i => ((double)left / i).ToExpression(),
                    uint u => ((double)left / u).ToExpression(),
                    long l => ((double)left / l).ToExpression(),
                    ulong right1 => ((double)left / right1).ToExpression(),
                    float f => ((double)left / f).ToExpression(),
                    double d => ((double)left / d).ToExpression(),
                    Complex complex => ((double)left / complex).ToExpression(),
                    Vector2D vector_2d => ((double)left / vector_2d).ToExpression(),
                    _ => ((double)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is Complex)
            {
                if (IsZero(right))
                    return Ex.Throw(new DivideByZeroException().ToExpression());
                return right switch
                {
                    byte b => ((Complex)left / b).ToExpression(),
                    sbyte right1 => ((Complex)left / right1).ToExpression(),
                    short s => ((Complex)left / s).ToExpression(),
                    ushort right1 => ((Complex)left / right1).ToExpression(),
                    int i => ((Complex)left / i).ToExpression(),
                    uint u => ((Complex)left / u).ToExpression(),
                    long l => ((Complex)left / l).ToExpression(),
                    ulong right1 => ((Complex)left / right1).ToExpression(),
                    float f => ((Complex)left / f).ToExpression(),
                    double d => ((Complex)left / d).ToExpression(),
                    Complex complex => ((Complex)left / complex).ToExpression(),
                    //Vector2D vector_2d => ((Complex)left / vector_2d).ToExpression(),
                    _ => null
                };
            }
            if (left is Vector2D)
            {
                if (IsZero(right))
                    return Ex.Throw(new DivideByZeroException().ToExpression());
                return right switch
                {
                    byte b => ((Vector2D)left / b).ToExpression(),
                    sbyte right1 => ((Vector2D)left / right1).ToExpression(),
                    short s => ((Vector2D)left / s).ToExpression(),
                    ushort right1 => ((Vector2D)left / right1).ToExpression(),
                    int i => ((Vector2D)left / i).ToExpression(),
                    uint u => ((Vector2D)left / u).ToExpression(),
                    long l => ((Vector2D)left / l).ToExpression(),
                    ulong right1 => ((Vector2D)left / right1).ToExpression(),
                    float f => ((Vector2D)left / f).ToExpression(),
                    //Complex complex => ((Vector2D)left / complex).ToExpression(),
                    //Vector2D vector_2d => ((Vector2D)left / vector_2d).ToExpression(),
                    _ => ((Vector2D)left / (right as double?))?.ToExpression()
                };
            }
            if (left is Vector3D)
            {
                if (IsZero(right))
                    return Ex.Throw(new DivideByZeroException().ToExpression());
                return right switch
                {
                    byte b => ((Vector3D)left / b).ToExpression(),
                    sbyte right1 => ((Vector3D)left / right1).ToExpression(),
                    short s => ((Vector3D)left / s).ToExpression(),
                    ushort right1 => ((Vector3D)left / right1).ToExpression(),
                    int i => ((Vector3D)left / i).ToExpression(),
                    uint u => ((Vector3D)left / u).ToExpression(),
                    long l => ((Vector3D)left / l).ToExpression(),
                    ulong right1 => ((Vector3D)left / right1).ToExpression(),
                    float f => ((Vector3D)left / f).ToExpression(),
                    double d => ((Vector3D)left / d).ToExpression(),
                    //Complex complex => ((Vector3D)left / complex).ToExpression(),
                    //Vector2D vector_2d => ((Vector3D)left / vector_2d).ToExpression(),
                    //_ => ((Vector3D)left / (right as Vector3D?))?.ToExpression()
                    _ => null
                };
            }
            return null;
        }

        private static Ex AdditionSimplify(bEx expr)
        {
            var right = expr.Right;
            var left = expr.Left;
            if (IsZero((left as cEx)?.Value)) return right;
            if (IsZero((right as cEx)?.Value)) return left;

            //if(right.NodeType == ExpressionType.Add || right.NodeType == ExpressionType.Subtract)
            //{
            //    var right_operands = GetOperands_Addition(right as bEx).ToArray();
            //    var consts = right_operands.Where(e => e is cEx || e.NodeType == ExpressionType.Negate && ((uEx)e).Operand is cEx).ToList();
            //    var vars = right_operands.Except(consts).ToList();

            //    Expression sum = null;
            //    while(sum is null && consts.Count > 0)
            //        if()

            //            if(consts.Count > 1)
            //            {
            //                for(var i = 0; i < consts.Count; i++)
            //                {
            //                    var s = AddValues((sum as cEx)?.Value, (consts[i] as cEx)?.Value);
            //                    if(s is null)
            //        }
            //            }
            //}


            return AddValues((left as cEx)?.Value!, (right as cEx)?.Value!) ?? expr;
        }

        private static IEnumerable<Ex> GetOperands_Addition(bEx? expr)
        {
            if (expr is null || expr.NodeType is not (ExpressionType.Add or ExpressionType.Subtract)) yield break;

            var left = expr.Left;
            if ((left is bEx && left.NodeType == ExpressionType.Add) || left.NodeType == ExpressionType.Subtract)
                foreach (var item in GetOperands_Addition(left as bEx))
                    yield return item;
            else
                yield return left;

            var right = expr.Right;
            if ((right is bEx && right.NodeType == ExpressionType.Add) || right.NodeType == ExpressionType.Subtract)
                if (expr.NodeType == ExpressionType.Add)
                    foreach (var item in GetOperands_Addition(left as bEx))
                        yield return item;
                else
                    foreach (var item in GetOperands_Addition(left as bEx))
                        if (item.NodeType == ExpressionType.Negate)
                            yield return ((uEx)item).Operand;
                        else
                            yield return item.Negate();
            else
                yield return right;
        }

        private static Ex? AddValues(object left, object right)
        {
            if (!IsNumeric(left) || !IsNumeric(right)) return null;
            return left switch
            {
                byte left1 => right switch
                {
                    byte b => (left1 + b).ToExpression(),
                    sbyte right1 => (left1 + right1).ToExpression(),
                    short s => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i => (left1 + i).ToExpression(),
                    uint u => (left1 + u).ToExpression(),
                    long l => (left1 + l).ToExpression(),
                    ulong right1 => (left1 + right1).ToExpression(),
                    float f => (left1 + f).ToExpression(),
                    double d => (left1 + d).ToExpression(),
                    Complex complex => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _ => (left1 + (right as Vector3D?))?.ToExpression()
                },
                sbyte left1 => right switch
                {
                    byte b => (left1 + b).ToExpression(),
                    sbyte right1 => (left1 + right1).ToExpression(),
                    short s => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i => (left1 + i).ToExpression(),
                    uint u => (left1 + u).ToExpression(),
                    long l => (left1 + l).ToExpression(),
                    //ulong right1 => (left1 + right1).ToExpression(),
                    float f => (left1 + f).ToExpression(),
                    double d => (left1 + d).ToExpression(),
                    Complex complex => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _ => (left1 + (right as Vector3D?))?.ToExpression()
                },
                short left1 => right switch
                {
                    byte b => (left1 + b).ToExpression(),
                    sbyte right1 => (left1 + right1).ToExpression(),
                    short s => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i => (left1 + i).ToExpression(),
                    uint u => (left1 + u).ToExpression(),
                    long l => (left1 + l).ToExpression(),
                    //ulong right1 => (left1 + right1).ToExpression(),
                    float f => (left1 + f).ToExpression(),
                    double d => (left1 + d).ToExpression(),
                    Complex complex => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _ => (left1 + (right as Vector3D?))?.ToExpression()
                },
                ushort left1 => right switch
                {
                    byte b => (left1 + b).ToExpression(),
                    sbyte right1 => (left1 + right1).ToExpression(),
                    short s => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i => (left1 + i).ToExpression(),
                    uint u => (left1 + u).ToExpression(),
                    long l => (left1 + l).ToExpression(),
                    ulong right1 => (left1 + right1).ToExpression(),
                    float f => (left1 + f).ToExpression(),
                    double d => (left1 + d).ToExpression(),
                    Complex complex => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _ => (left1 + (right as Vector3D?))?.ToExpression()
                },
                int left1 => right switch
                {
                    byte b => (left1 + b).ToExpression(),
                    sbyte right1 => (left1 + right1).ToExpression(),
                    short s => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i => (left1 + i).ToExpression(),
                    uint u => (left1 + u).ToExpression(),
                    long l => (left1 + l).ToExpression(),
                    //ulong right1 => (left1 + right1).ToExpression(),
                    float f => (left1 + f).ToExpression(),
                    double d => (left1 + d).ToExpression(),
                    Complex complex => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _ => (left1 + (right as Vector3D?))?.ToExpression()
                },
                uint left1 => right switch
                {
                    byte b => (left1 + b).ToExpression(),
                    sbyte right1 => (left1 + right1).ToExpression(),
                    short s => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i => (left1 + i).ToExpression(),
                    uint u => (left1 + u).ToExpression(),
                    long l => (left1 + l).ToExpression(),
                    ulong right1 => (left1 + right1).ToExpression(),
                    float f => (left1 + f).ToExpression(),
                    double d => (left1 + d).ToExpression(),
                    Complex complex => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _ => (left1 + (right as Vector3D?))?.ToExpression()
                },
                long left1 => right switch
                {
                    byte b => (left1 + b).ToExpression(),
                    sbyte right1 => (left1 + right1).ToExpression(),
                    short s => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i => (left1 + i).ToExpression(),
                    uint u => (left1 + u).ToExpression(),
                    long l => (left1 + l).ToExpression(),
                    //ulong right1 => (left1 + right1).ToExpression(),
                    float f => (left1 + f).ToExpression(),
                    double d => (left1 + d).ToExpression(),
                    Complex complex => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _ => (left1 + (right as Vector3D?))?.ToExpression()
                },
                ulong left1 => right switch
                {
                    byte b => (left1 + b).ToExpression(),
                    //sbyte right1 => (left1 + right1).ToExpression(),
                    //short s => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    //int i => (left1 + i).ToExpression(),
                    uint u => (left1 + u).ToExpression(),
                    //long l => (left1 + l).ToExpression(),
                    ulong right1 => (left1 + right1).ToExpression(),
                    float f => (left1 + f).ToExpression(),
                    double d => (left1 + d).ToExpression(),
                    Complex complex => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _ => (left1 + (right as Vector3D?))?.ToExpression()
                },
                float left1 => right switch
                {
                    byte b => (left1 + b).ToExpression(),
                    sbyte right1 => (left1 + right1).ToExpression(),
                    short s => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i => (left1 + i).ToExpression(),
                    uint u => (left1 + u).ToExpression(),
                    long l => (left1 + l).ToExpression(),
                    ulong right1 => (left1 + right1).ToExpression(),
                    float f => (left1 + f).ToExpression(),
                    double d => (left1 + d).ToExpression(),
                    Complex complex => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _ => (left1 + (right as Vector3D?))?.ToExpression()
                },
                double left1 => right switch
                {
                    byte b => (left1 + b).ToExpression(),
                    sbyte right1 => (left1 + right1).ToExpression(),
                    short s => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i => (left1 + i).ToExpression(),
                    uint u => (left1 + u).ToExpression(),
                    long l => (left1 + l).ToExpression(),
                    ulong right1 => (left1 + right1).ToExpression(),
                    float f => (left1 + f).ToExpression(),
                    double d => (left1 + d).ToExpression(),
                    Complex complex => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _ => (left1 + (right as Vector3D?))?.ToExpression()
                },
                Complex left1 => right switch
                {
                    byte b => (left1 + b).ToExpression(),
                    sbyte right1 => (left1 + right1).ToExpression(),
                    short s => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i => (left1 + i).ToExpression(),
                    uint u => (left1 + u).ToExpression(),
                    long l => (left1 + l).ToExpression(),
                    ulong right1 => (left1 + right1).ToExpression(),
                    float f => (left1 + f).ToExpression(),
                    double d => (left1 + d).ToExpression(),
                    Complex complex => (left1 + complex).ToExpression(),
                    //Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _ => null
                },
                Vector2D left1 => right switch
                {
                    byte b => (left1 + b).ToExpression(),
                    sbyte right1 => (left1 + right1).ToExpression(),
                    short s => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i => (left1 + i).ToExpression(),
                    uint u => (left1 + u).ToExpression(),
                    long l => (left1 + l).ToExpression(),
                    ulong right1 => (left1 + right1).ToExpression(),
                    float f => (left1 + f).ToExpression(),
                    double d => (left1 + d).ToExpression(),
                    //Complex complex => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _ => null
                },
                Vector3D vector_3d => right switch
                {
                    byte b => (vector_3d + b).ToExpression(),
                    sbyte right1 => (vector_3d + right1).ToExpression(),
                    short s => (vector_3d + s).ToExpression(),
                    ushort right1 => (vector_3d + right1).ToExpression(),
                    int i => (vector_3d + i).ToExpression(),
                    uint u => (vector_3d + u).ToExpression(),
                    long l => (vector_3d + l).ToExpression(),
                    ulong right1 => (vector_3d + right1).ToExpression(),
                    float f => (vector_3d + f).ToExpression(),
                    double d => (vector_3d + d).ToExpression(),
                    Complex complex => (vector_3d + complex).ToExpression(),
                    Vector2D vector_2d => (vector_3d + vector_2d).ToExpression(),
                    _ => (vector_3d + (right as Vector3D?))?.ToExpression()
                },
                _ => null
            };
        }

        private static Ex SubtractionSimplify(bEx expr)
        {
            if (IsZero((expr.Left as cEx)?.Value)) return expr.Right.Negate();
            if (IsZero((expr.Right as cEx)?.Value)) return expr.Left;

            return SubtractValues((expr.Left as cEx)?.Value, (expr.Right as cEx)?.Value) ?? expr;
        }

        private static Ex? SubtractValues(object? left, object? right)
        {
            if (!IsNumeric(left) || !IsNumeric(right)) return null;
            return left switch
            {
                byte left1 => right switch
                {
                    byte b => (left1 - b).ToExpression(),
                    sbyte right1 => (left1 - right1).ToExpression(),
                    short s => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i => (left1 - i).ToExpression(),
                    uint u => (left1 - u).ToExpression(),
                    long l => (left1 - l).ToExpression(),
                    ulong right1 => (left1 - right1).ToExpression(),
                    float f => (left1 - f).ToExpression(),
                    double d => (left1 - d).ToExpression(),
                    Complex complex => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _ => (left1 - (right as Vector3D?))?.ToExpression()
                },
                sbyte left1 => right switch
                {
                    byte b => (left1 - b).ToExpression(),
                    sbyte right1 => (left1 - right1).ToExpression(),
                    short s => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i => (left1 - i).ToExpression(),
                    uint u => (left1 - u).ToExpression(),
                    long l => (left1 - l).ToExpression(),
                    //ulong right1 => (left1 - right1).ToExpression(),
                    float f => (left1 - f).ToExpression(),
                    double d => (left1 - d).ToExpression(),
                    Complex complex => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _ => (left1 - (right as Vector3D?))?.ToExpression()
                },
                short left1 => right switch
                {
                    byte b => (left1 - b).ToExpression(),
                    sbyte right1 => (left1 - right1).ToExpression(),
                    short s => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i => (left1 - i).ToExpression(),
                    uint u => (left1 - u).ToExpression(),
                    long l => (left1 - l).ToExpression(),
                    //ulong right1 => (left1 - right1).ToExpression(),
                    float f => (left1 - f).ToExpression(),
                    double d => (left1 - d).ToExpression(),
                    Complex complex => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _ => (left1 - (right as Vector3D?))?.ToExpression()
                },
                ushort left1 => right switch
                {
                    byte b => (left1 - b).ToExpression(),
                    sbyte right1 => (left1 - right1).ToExpression(),
                    short s => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i => (left1 - i).ToExpression(),
                    uint u => (left1 - u).ToExpression(),
                    long l => (left1 - l).ToExpression(),
                    ulong right1 => (left1 - right1).ToExpression(),
                    float f => (left1 - f).ToExpression(),
                    double d => (left1 - d).ToExpression(),
                    Complex complex => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _ => (left1 - (right as Vector3D?))?.ToExpression()
                },
                int left1 => right switch
                {
                    byte b => (left1 - b).ToExpression(),
                    sbyte right1 => (left1 - right1).ToExpression(),
                    short s => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i => (left1 - i).ToExpression(),
                    uint u => (left1 - u).ToExpression(),
                    long l => (left1 - l).ToExpression(),
                    //ulong right1 => (left1 - right1).ToExpression(),
                    float f => (left1 - f).ToExpression(),
                    double d => (left1 - d).ToExpression(),
                    Complex complex => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _ => (left1 - (right as Vector3D?))?.ToExpression()
                },
                uint left1 => right switch
                {
                    byte b => (left1 - b).ToExpression(),
                    sbyte right1 => (left1 - right1).ToExpression(),
                    short s => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i => (left1 - i).ToExpression(),
                    uint u => (left1 - u).ToExpression(),
                    long l => (left1 - l).ToExpression(),
                    ulong right1 => (left1 - right1).ToExpression(),
                    float f => (left1 - f).ToExpression(),
                    double d => (left1 - d).ToExpression(),
                    Complex complex => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _ => (left1 - (right as Vector3D?))?.ToExpression()
                },
                long left1 => right switch
                {
                    byte b => (left1 - b).ToExpression(),
                    sbyte right1 => (left1 - right1).ToExpression(),
                    short s => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i => (left1 - i).ToExpression(),
                    uint u => (left1 - u).ToExpression(),
                    long l => (left1 - l).ToExpression(),
                    //ulong right1 => (left1 - right1).ToExpression(),
                    float f => (left1 - f).ToExpression(),
                    double d => (left1 - d).ToExpression(),
                    Complex complex => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _ => (left1 - (right as Vector3D?))?.ToExpression()
                },
                ulong left1 => right switch
                {
                    byte b => (left1 - b).ToExpression(),
                    //sbyte right1 => (left1 - right1).ToExpression(),
                    //short s => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    //int i => (left1 - i).ToExpression(),
                    uint u => (left1 - u).ToExpression(),
                    //long l => (left1 - l).ToExpression(),
                    ulong right1 => (left1 - right1).ToExpression(),
                    float f => (left1 - f).ToExpression(),
                    double d => (left1 - d).ToExpression(),
                    Complex complex => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _ => (left1 - (right as Vector3D?))?.ToExpression()
                },
                float left1 => right switch
                {
                    byte b => (left1 - b).ToExpression(),
                    sbyte right1 => (left1 - right1).ToExpression(),
                    short s => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i => (left1 - i).ToExpression(),
                    uint u => (left1 - u).ToExpression(),
                    long l => (left1 - l).ToExpression(),
                    ulong right1 => (left1 - right1).ToExpression(),
                    float f => (left1 - f).ToExpression(),
                    double d => (left1 - d).ToExpression(),
                    Complex complex => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _ => (left1 - (right as Vector3D?))?.ToExpression()
                },
                double left1 => right switch
                {
                    byte b => (left1 - b).ToExpression(),
                    sbyte right1 => (left1 - right1).ToExpression(),
                    short s => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i => (left1 - i).ToExpression(),
                    uint u => (left1 - u).ToExpression(),
                    long l => (left1 - l).ToExpression(),
                    ulong right1 => (left1 - right1).ToExpression(),
                    float f => (left1 - f).ToExpression(),
                    double d => (left1 - d).ToExpression(),
                    Complex complex => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _ => (left1 - (right as Vector3D?))?.ToExpression()
                },
                Complex left1 => right switch
                {
                    byte b => (left1 - b).ToExpression(),
                    sbyte right1 => (left1 - right1).ToExpression(),
                    short s => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i => (left1 - i).ToExpression(),
                    uint u => (left1 - u).ToExpression(),
                    long l => (left1 - l).ToExpression(),
                    ulong right1 => (left1 - right1).ToExpression(),
                    float f => (left1 - f).ToExpression(),
                    double d => (left1 - d).ToExpression(),
                    Complex complex => (left1 - complex).ToExpression(),
                    //Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _ => null
                },
                Vector2D left1 => right switch
                {
                    byte b => (left1 - b).ToExpression(),
                    sbyte right1 => (left1 - right1).ToExpression(),
                    short s => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i => (left1 - i).ToExpression(),
                    uint u => (left1 - u).ToExpression(),
                    long l => (left1 - l).ToExpression(),
                    ulong right1 => (left1 - right1).ToExpression(),
                    float f => (left1 - f).ToExpression(),
                    double d => (left1 - d).ToExpression(),
                    //Complex complex => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _ => null
                },
                Vector3D vector_3d => right switch
                {
                    byte b => (vector_3d - b).ToExpression(),
                    sbyte right1 => (vector_3d - right1).ToExpression(),
                    short s => (vector_3d - s).ToExpression(),
                    ushort right1 => (vector_3d - right1).ToExpression(),
                    int i => (vector_3d - i).ToExpression(),
                    uint u => (vector_3d - u).ToExpression(),
                    long l => (vector_3d - l).ToExpression(),
                    ulong right1 => (vector_3d - right1).ToExpression(),
                    float f => (vector_3d - f).ToExpression(),
                    double d => (vector_3d - d).ToExpression(),
                    Complex complex => (vector_3d - complex).ToExpression(),
                    Vector2D vector_2d => (vector_3d - vector_2d).ToExpression(),
                    _ => (vector_3d - (right as Vector3D?))?.ToExpression()
                },
                _ => null
            };
        }
    }
}