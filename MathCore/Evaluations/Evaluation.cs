using System;
using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evaluations;

/// <summary>Вычисление</summary>
public abstract class Evaluation
{
    /// <summary>Получить выражение вычисления</summary>
    /// <returns>Возвращает выражение, соответствующее данному вычислению</returns>
    public abstract Ex GetExpression();

    /// <summary>Оператор неявного преобразования вычисления в выражение</summary>
    /// <param name="Evaluation">Преобразуемое вычисление</param>
    public static implicit operator Ex(Evaluation Evaluation) => Evaluation.GetExpression();
}

/// <summary>Вычисление с результатом типа <typeparamref name="T"/></summary>
/// <typeparam name="T">Тип результата вычисления</typeparam>
public abstract class Evaluation<T> : Evaluation
{
    /// <summary>Получить значение</summary>
    /// <returns></returns>
    public abstract T GetValue();

    /// <summary>Получить выражение вычисления</summary>
    /// <returns>Выражение, определяющие вычисление</returns>
    public override Ex GetExpression() => ((Func<T>) GetValue).GetCallExpression(this.ToExpression());
    //public override Ex GetExpression() => Ex.Call(Ex.Constant(this), ((Func<T>)GetValue).Method);

    /// <summary>Оператор неявного преобразования вычисления в выражение</summary>
    /// <param name="Evaluation">Преобразуемое вычисление</param>
    public static implicit operator T(Evaluation<T> Evaluation) => Evaluation.GetValue();

    /// <summary>Оператор сложения двух вычислений</summary>
    /// <param name="x">Первое слагаемое</param>
    /// <param name="y">Второе слагаемое</param>
    /// <returns>Вычисление суммы двух вычислений</returns>
    public static AdditionEvaluation<T> operator +(Evaluation<T> x, Evaluation<T> y) => new(x, y);

    /// <summary>Вычисление разности двух вычислений</summary>
    /// <param name="x">Уменьшаемое</param>
    /// <param name="y">Вычитаемое</param>
    /// <returns>Вычисление разности двух вычислений</returns>
    public static SubtractEvaluation<T> operator -(Evaluation<T> x, Evaluation<T> y) => new(x, y);

    /// <summary>Оператор получения отрицательного значения на основе вычисления</summary>
    /// <param name="x">Вычисление значения оператора</param>
    /// <returns>Оператор получения отрицательного значения</returns>
    public static NegateOperatorEvaluation<T> operator -(Evaluation<T> x) => new(x);

    /// <summary>Вычисление произведения двух вычислений</summary>
    /// <param name="x">Первый сомножитель</param>
    /// <param name="y">Второй сомножитель</param>
    /// <returns>Вычисление произведения двух вычислений</returns>
    public static MultiplyEvaluation<T> operator *(Evaluation<T> x, Evaluation<T> y) => new(x, y);

    /// <summary>Вычисление частного двух вычислений</summary>
    /// <param name="x">Делимое</param>
    /// <param name="y">Делитель</param>
    /// <returns>Вычисление частного двух вычислений</returns>
    public static DivideEvaluation<T> operator /(Evaluation<T> x, Evaluation<T> y) => new(x, y);
}