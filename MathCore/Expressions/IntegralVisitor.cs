using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Linq.Expressions
{
    [NotImplemented]
    internal class IntegralVisitor : ExpressionVisitorEx
    {
        private readonly Dictionary<int, ParameterExpression> _Parameters = new Dictionary<int, ParameterExpression>();

        private ParameterExpression Parameter
        {
            get
            {
                var id = Thread.CurrentThread.ManagedThreadId;
                return _Parameters[id];
            }
            set
            {
                var id = Thread.CurrentThread.ManagedThreadId;
                if(value is null)
                    _Parameters.Remove(id);
                else
                    _Parameters.Add(id, value);
            }
        }

        private static bool CheckNumType(Type type) =>
            type == typeof(double)
            || type == typeof(int)
            || type == typeof(short)
            || type == typeof(long)
            || type == typeof(float);

        private static void CheckValueType(Type type)
        {
            if(!CheckNumType(type)) throw new NotSupportedException($"Неподдерживаемый тип данных {type}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sAdd(Expression a, Expression b) => sAdd(Expression.Add(a, b));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sAdd(double a, Expression b) => sAdd(Expression.Add(Expression.Constant(a), b));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sAdd(Expression a, double b) => sAdd(Expression.Add(a, Expression.Constant(b)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sInc(Expression a) => sAdd(a, 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sSubtract(Expression a, Expression b) => sAdd(Expression.Subtract(a, b));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sSubtract(double a, Expression b) => sAdd(Expression.Subtract(Expression.Constant(a), b));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sSubtract(Expression a, double b) => sAdd(Expression.Subtract(a, Expression.Constant(b)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sDec(Expression a) => sSubtract(a, 1);
        private static Expression sAdd(BinaryExpression b)
        {
            var l = b.Left as ConstantExpression;
            var r = b.Right as ConstantExpression;
            if(l is null && r is null) return b;
            if(l != null && r != null)
                return b.NodeType == ExpressionType.Add
                    ? Expression.Constant((double)l.Value + (double)r.Value)
                    : Expression.Constant((double)l.Value - (double)r.Value);
            if(l != null && l.Value.Equals(0.0))
                return b.NodeType == ExpressionType.Add
                    ? b.Right
                    : Expression.MakeUnary(ExpressionType.Negate, b.Right, b.Right.Type);
            if(r != null && r.Value.Equals(0.0))
                return b.Left;
            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sMultiply(Expression a, Expression b) => sMultiply(Expression.Multiply(a, b));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sMultiply(double a, Expression b) => sMultiply(Expression.Multiply(Expression.Constant(a), b));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sMultiply(Expression a, double b) => sMultiply(Expression.Multiply(a, Expression.Constant(b)));
        private static Expression sMultiply(BinaryExpression b)
        {
            var l = b.Left as ConstantExpression;
            var r = b.Right as ConstantExpression;
            if(l is null && r is null) return b;
            if(l != null && r != null)
                return Expression.Constant((double)l.Value * (double)r.Value);
            if(l?.Value.Equals(0.0) == true) return l;
            if(l?.Value.Equals(1.0) == true) return b.Right;
            if(r?.Value.Equals(0.0) == true) return r;
            if(r?.Value.Equals(1.0) == true) return b.Left;
            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sDivide(Expression a, Expression b) => sDivide(Expression.Divide(a, b));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sDivide(double a, Expression b) => sDivide(Expression.Divide(Expression.Constant(a), b));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sDivide(Expression a, double b) => sDivide(Expression.Divide(a, Expression.Constant(b)));
        private static Expression sDivide(BinaryExpression b)
        {
            var l = b.Left as ConstantExpression;
            var r = b.Right as ConstantExpression;
            if(l is null && r is null) return b;
            if(l != null && r != null) return Expression.Constant((double)l.Value / (double)r.Value);
            if(l?.Value.Equals(0.0) == true) return l;
            if(l?.Value.Equals(1.0) == true) return b;
            if(r?.Value.Equals(0.0) == true) return Expression.Constant(double.PositiveInfinity);
            if(r?.Value.Equals(1.0) == true) return b.Left;
            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sPower(Expression a, Expression b) => sPower(Expression.Power(a, b));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression sPower(Expression a, double b) => sPower(Expression.Power(a, Expression.Constant(b)));
        private static Expression sPower(BinaryExpression b)
        {
            var l = b.Left as ConstantExpression;
            var r = b.Right as ConstantExpression;
            if(l is null && r is null) return b;
            if(l != null && r != null) return Expression.Constant(Math.Pow((double)l.Value, (double)r.Value));
            if(l != null && (l.Value.Equals(0.0) || l.Value.Equals(1.0))) return l;
            if(r?.Value.Equals(0.0) == true) return Expression.Constant(1.0);
            if(r?.Value.Equals(1.0) == true) return b.Left;
            return b;
        }

        private Expression MathMethod(string Name, params Expression[] p) =>
            p.All(P => P is ConstantExpression)
                ? (Expression) Expression.Constant(typeof(Math).GetMethod(Name, p.Select(P => P.Type).ToArray())
                   .Invoke(null, p.Cast<ConstantExpression>().Select(P => P.Value).ToArray()))
                : Expression.Call(typeof(Math), Name, null, p);

        public Expression Visit(LambdaExpression exp, double constant) => Visit(exp, Expression.Constant(constant));
        public Expression Visit(LambdaExpression exp, Expression constant)
        {
            exp = (LambdaExpression)base.Visit(exp);
            return Expression.Lambda(exp.Type, sAdd(exp.Body, constant), exp.Parameters);
        }

        protected override Expression VisitLambda(LambdaExpression lambda)
        {
            Parameter = lambda.Parameters[0];
            var expr = base.VisitLambda(lambda);
            Parameter = null;
            return expr;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            CheckValueType(c.Type);
            return Parameter;
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            CheckValueType(p.Type);
            return sPower(p, 2);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            var NodeType = b.NodeType;
            switch(NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    {
                        var expr = base.VisitBinary(b);
                        return expr is BinaryExpression binary_expression
                            ? sAdd(binary_expression)
                            : expr;
                    }

                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    {
                        var left = b.Left;
                        var right = b.Right;
                        if(left is ConstantExpression) return sMultiply(left, Visit(right));
                        if(right is ConstantExpression) return sMultiply(Visit(left), right);

                        throw new NotImplementedException();
                    }

                case ExpressionType.Divide:
                    {
                        var x = b.Left;
                        var y = b.Right;
                        if(x is ConstantExpression constant && y is ParameterExpression)
                        {
                            var I = sDivide(1, MathMethod("Log", MathMethod("Abs", y)));
                            if((double)constant.Value != 1.0)
                                I = sMultiply(constant, I);
                            return I;
                        }

                        throw new NotImplementedException();
                    }
                case ExpressionType.Power:
                    {
                        var x = b.Left;
                        var y = b.Right;
                        if(x is ParameterExpression && y is ConstantExpression)
                        {
                            y = sInc(y);
                            return sDivide(sPower(x, y), y);
                        }
                        if(x is ConstantExpression && y is ParameterExpression)
                            return sDivide(b, MathMethod("Log", x));
                        throw new NotImplementedException();
                    }
                default:
                    throw new NotSupportedException("Неподдерживаемый тип операции");
            }
        }

        protected Expression VisitMathMethodCall(MethodCallExpression m)
        {
            switch(m.Method.Name)
            {
                case "Pow":
                    return Visit(sPower(Expression.Power(m.Arguments[0], m.Arguments[1])));
                case "Sin":
                    return Expression.Negate(MathMethod("Cos", m.Arguments[0]));
                case "Cos":
                    return MathMethod("Sin", m.Arguments[0]);
                //case "Tan":
                //    {
                //        var x = m.Arguments[0];
                //        return sMultiply(Visit(x), sDivide(1, sPower(MathMethod("Cos", x), 2)));
                //    }
                //case "Asin":
                //    {
                //        var x = m.Arguments[0];
                //        return sMultiply(Visit(x), sDivide(1, MathMethod("Sqrt", sSubtract(1, sPower(x, 2)))));
                //    }
                //case "Acos":
                //    {
                //        var x = m.Arguments[0];
                //        return Expression.Negate(sMultiply(Visit(x), sDivide(1, MathMethod("Sqrt", sSubtract(1, sPower(x, 2))))));
                //    }
                //case "Atan":
                //    {
                //        var x = m.Arguments[0];
                //        return sMultiply(Visit(x), sDivide(1, sAdd(1, sPower(x, 2))));
                //    }
                //case "Sinh":
                //    {
                //        var x = m.Arguments[0];
                //        return sMultiply(Visit(x), MathMethod("Cosh", x));
                //        //return sMultiply(dx, Expression.Call(typeof(Math), "Cos", null, x));
                //    }
                //case "Cosh":
                //    {
                //        var x = m.Arguments[0];
                //        return sMultiply(Visit(x), MathMethod("Sinh", x));
                //    }
                //case "Tanh":
                //    {
                //        var x = m.Arguments[0];
                //        return sMultiply(Visit(x), sDivide(1, sPower(sDivide(1, MathMethod("Tanh", x)), 2)));
                //    }
                //case "Abs":
                //    {
                //        var x = m.Arguments[0];
                //        var condition = Expression.Condition
                //            (
                //                Expression.Equal(x, Expression.Constant(0.0)),
                //                Expression.Constant(0.0),
                //                Expression.Convert(MathMethod("Sign", x), typeof(double))
                //            );
                //        return sMultiply(Visit(x), condition);
                //    }
                //case "Sign":
                //    {
                //        var x = m.Arguments[0];
                //        var condition = Expression.Condition
                //            (
                //                Expression.Equal(x, Expression.Constant(0.0)),
                //                Expression.Constant(double.PositiveInfinity),
                //                Expression.Constant(0.0)
                //            );
                //        return sMultiply(Visit(x), condition);
                //    }
                //case "Sqrt":
                //    return Visit(sPower(m.Arguments[0],
                //        Expression.Divide(Expression.Constant(1.0), Expression.Constant(2.0))));
                //case "Exp":
                //    {
                //        var x = m.Arguments[0];
                //        return sMultiply(Visit(x), MathMethod("Exp", x));
                //    }
                //case "Log":
                //    {
                //        var x = m.Arguments[0];
                //        if(m.Arguments.Count > 1)
                //        {
                //            var a = m.Arguments[1];
                //            var expr = sDivide(MathMethod("Log", x), MathMethod("Log", a));
                //            return Visit(expr);
                //        }
                //        var dx = Visit(x);
                //        return sDivide(dx, x);
                //    }
                //case "Log10":
                //    {
                //        var x = m.Arguments[0];
                //        var expr = MathMethod("Log", x, Expression.Constant(10.0));
                //        return Visit(expr);
                //    }
                default:
                    throw new NotSupportedException();
            }
            //Math.
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            //var result = OnMethodDifferential(m);
            //if(result != null) return result;

            var method = m.Method;
            if(method.DeclaringType == typeof(Math))
                return VisitMathMethodCall(m);
            throw new NotSupportedException();
            //return base.VisitMethodCall(m);
        }
    }
}