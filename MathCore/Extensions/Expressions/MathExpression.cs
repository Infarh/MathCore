using System.Linq.Expressions;

namespace MathCore.Extensions.Expressions;

/// <summary>Методы формирования математических выражений на деревьях выражений</summary>
public static class MathExpression
{
    /// <summary>Выражение модуля</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение Math.Abs(x)</returns>
    public static MethodCallExpression Abs(Expression x) => ((Func<double, double>)Math.Abs).GetCallExpression(x);
    /// <summary>Выражение синуса</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение Math.Sin(x)</returns>
    public static MethodCallExpression Sin(Expression x) => ((Func<double, double>)Math.Sin).GetCallExpression(x);
    /// <summary>Выражение арксинуса</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение Math.Asin(x)</returns>
    public static MethodCallExpression Asin(Expression x) => ((Func<double, double>)Math.Asin).GetCallExpression(x);
    /// <summary>Выражение косинуса</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение Math.Cos(x)</returns>
    public static MethodCallExpression Cos(Expression x) => ((Func<double, double>)Math.Cos).GetCallExpression(x);
    /// <summary>Выражение арккосинуса</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение Math.Acos(x)</returns>
    public static MethodCallExpression Acos(Expression x) => ((Func<double, double>)Math.Acos).GetCallExpression(x);
    /// <summary>Выражение тангенса</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение Math.Tan(x)</returns>
    public static MethodCallExpression Tan(Expression x) => ((Func<double, double>)Math.Tan).GetCallExpression(x);
    /// <summary>Выражение арктангенса</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение Math.Atan(x)</returns>
    public static MethodCallExpression Atan(Expression x) => ((Func<double, double>)Math.Atan).GetCallExpression(x);
    /// <summary>Выражение арктангенса двух аргументов</summary>
    /// <param name="y">Ордината</param>
    /// <param name="x">Абсцисса</param>
    /// <returns>Выражение Math.Atan2(y, x)</returns>
    public static MethodCallExpression Atan2(Expression y, Expression x) => ((Func<double, double, double>)Math.Atan2).GetCallExpression(x);
    /// <summary>Выражение экспоненты</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение Math.Exp(x)</returns>
    public static MethodCallExpression Exp(Expression x) => ((Func<double, double>)Math.Exp).GetCallExpression(x);
    /// <summary>Выражение e в степени</summary>
    /// <param name="x">Показатель степени</param>
    /// <returns>Выражение e^x</returns>
    public static BinaryExpression ExpPower(Expression x) => Consts.e.ToExpression().Power(x);
    /// <summary>Выражение натурального логарифма</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение Math.Log(x)</returns>
    public static MethodCallExpression Log(Expression x) => ((Func<double, double>)Math.Log).GetCallExpression(x);
    /// <summary>Выражение десятичного логарифма</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение Math.Log10(x)</returns>
    public static MethodCallExpression Log10(Expression x) => ((Func<double, double>)Math.Log10).GetCallExpression(x);
    /// <summary>Выражение логарифма по основанию</summary>
    /// <param name="a">Основание</param>
    /// <param name="b">Аргумент</param>
    /// <returns>Выражение Math.Log(a, b)</returns>
    public static MethodCallExpression Log(Expression a, Expression b) => ((Func<double, double, double>)Math.Log).GetCallExpression(a, b);
    /// <summary>Выражение степени</summary>
    /// <param name="a">Основание</param>
    /// <param name="b">Показатель</param>
    /// <returns>Выражение Math.Pow(a, b)</returns>
    public static MethodCallExpression Pow(Expression a, Expression b) => ((Func<double, double, double>)Math.Pow).GetCallExpression(a, b);
    /// <summary>Выражение знака числа</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение Math.Sign(x)</returns>
    public static MethodCallExpression Sign(Expression x) => ((Func<double, int>)Math.Sign).GetCallExpression(x);
    /// <summary>Выражение округления вниз</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение Math.Floor(x)</returns>
    public static MethodCallExpression Floor(Expression x) => ((Func<double, double>)Math.Floor).GetCallExpression(x);
    /// <summary>Выражение округления</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение Math.Round(x)</returns>
    public static MethodCallExpression Round(Expression x) => ((Func<double, double>)Math.Round).GetCallExpression(x);
    /// <summary>Выражение округления с количеством разрядов</summary>
    /// <param name="x">Операнд</param>
    /// <param name="n">Количество разрядов</param>
    /// <returns>Выражение Math.Round(x, n)</returns>
    public static MethodCallExpression Round(Expression x, Expression n) => ((Func<double, int, double>)Math.Round).GetCallExpression(x, n);
    /// <summary>Выражение отбрасывания дробной части</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение Math.Truncate(x)</returns>
    public static MethodCallExpression Truncate(Expression x) => ((Func<double, double>)Math.Truncate).GetCallExpression(x);
    /// <summary>Выражение минимума</summary>
    /// <param name="a">Первый операнд</param>
    /// <param name="b">Второй операнд</param>
    /// <returns>Выражение Math.Min(a, b)</returns>
    public static MethodCallExpression Min(Expression a, Expression b) => ((Func<double, double, double>)Math.Min).GetCallExpression(a, b);
    /// <summary>Выражение максимума</summary>
    /// <param name="a">Первый операнд</param>
    /// <param name="b">Второй операнд</param>
    /// <returns>Выражение Math.Max(a, b)</returns>
    public static MethodCallExpression Max(Expression a, Expression b) => ((Func<double, double, double>)Math.Max).GetCallExpression(a, b);
    /// <summary>Выражение квадратного корня</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение Math.Sqrt(x)</returns>
    public static MethodCallExpression Sqrt(Expression x) => ((Func<double, double>)Math.Sqrt).GetCallExpression(x);
    /// <summary>Выражение квадратного корня через степень 1/2</summary>
    /// <param name="x">Операнд</param>
    /// <returns>Выражение x^(1/2)</returns>
    public static BinaryExpression SqrtPower(Expression x) => x.Power(1.ToExpression().Divide(2));
    /// <summary>Выражение корня степени y</summary>
    /// <param name="x">Операнд</param>
    /// <param name="y">Показатель корня</param>
    /// <returns>Выражение x^(1/y)</returns>
    public static BinaryExpression SqrtPower(Expression x, Expression y) => x.Power(1.ToExpression().Divide(y));
    /// <summary>Выражение вызова делегата</summary>
    /// <param name="f">Делегат</param>
    /// <param name="args">Аргументы</param>
    /// <returns>Выражение вызова метода</returns>
    public static MethodCallExpression F(Delegate f, params IEnumerable<Expression> args) => f.GetCallExpression(args);
    /// <summary>Выражение вызова функции</summary>
    /// <param name="f">Функция</param>
    /// <param name="args">Аргументы</param>
    /// <returns>Выражение вызова метода</returns>
    public static MethodCallExpression F(Func<double, double> f, params IEnumerable<Expression> args) => f.GetCallExpression(args);
}