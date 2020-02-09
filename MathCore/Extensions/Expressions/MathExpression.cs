using System;
using System.Linq.Expressions;

namespace MathCore.Extensions.Expressions
{
    public static class MathExpression
    {
        public static MethodCallExpression Abs(Expression x) => ((Func<double, double>)Math.Abs).GetCallExpression(x);
        public static MethodCallExpression Sin(Expression x) => ((Func<double, double>)Math.Sin).GetCallExpression(x);
        public static MethodCallExpression Asin(Expression x) => ((Func<double, double>)Math.Asin).GetCallExpression(x);
        public static MethodCallExpression Cos(Expression x) => ((Func<double, double>)Math.Cos).GetCallExpression(x);
        public static MethodCallExpression Acos(Expression x) => ((Func<double, double>)Math.Acos).GetCallExpression(x);
        public static MethodCallExpression Tan(Expression x) => ((Func<double, double>)Math.Tan).GetCallExpression(x);
        public static MethodCallExpression Atan(Expression x) => ((Func<double, double>)Math.Atan).GetCallExpression(x);
        public static MethodCallExpression Atan2(Expression y, Expression x) => ((Func<double, double, double>)Math.Atan2).GetCallExpression(x);
        public static MethodCallExpression Exp(Expression x) => ((Func<double, double>)Math.Exp).GetCallExpression(x);
        public static BinaryExpression ExpPower(Expression x) => Consts.e.ToExpression().Power(x);
        public static MethodCallExpression Log(Expression x) => ((Func<double, double>)Math.Log).GetCallExpression(x);
        public static MethodCallExpression Log10(Expression x) => ((Func<double, double>)Math.Log10).GetCallExpression(x);
        public static MethodCallExpression Log(Expression a, Expression b) => ((Func<double, double, double>)Math.Log).GetCallExpression(a, b);
        public static MethodCallExpression Pow(Expression a, Expression b) => ((Func<double, double, double>)Math.Pow).GetCallExpression(a, b);
        public static MethodCallExpression Sign(Expression x) => ((Func<double, int>)Math.Sign).GetCallExpression(x);
        public static MethodCallExpression Floor(Expression x) => ((Func<double, double>)Math.Floor).GetCallExpression(x);
        public static MethodCallExpression Round(Expression x) => ((Func<double, double>)Math.Round).GetCallExpression(x);
        public static MethodCallExpression Round(Expression x, Expression n) => ((Func<double, int, double>)Math.Round).GetCallExpression(x, n);
        public static MethodCallExpression Truncate(Expression x) => ((Func<double, double>)Math.Truncate).GetCallExpression(x);
        public static MethodCallExpression Min(Expression a, Expression b) => ((Func<double, double, double>)Math.Min).GetCallExpression(a, b);
        public static MethodCallExpression Max(Expression a, Expression b) => ((Func<double, double, double>)Math.Max).GetCallExpression(a, b);
        public static MethodCallExpression Sqrt(Expression x) => ((Func<double, double>)Math.Sqrt).GetCallExpression(x);
        public static BinaryExpression SqrtPower(Expression x) => x.Power(1.ToExpression().Divide(2));
        public static BinaryExpression SqrtPower(Expression x, Expression y) => x.Power(1.ToExpression().Divide(y));
        public static MethodCallExpression F(Delegate f, params Expression[] args) => f.GetCallExpression(args);
        public static MethodCallExpression F(Func<double, double> f, params Expression[] args) => f.GetCallExpression(args);
    }
}