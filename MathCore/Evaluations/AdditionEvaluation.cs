using Ex = System.Linq.Expressions.Expression;
// ReSharper disable UnusedMember.Global

namespace MathCore.Evaluations;

/// <summary>Вычисление суммы двух вычислений</summary>
/// <typeparam name="T">Тип значения вычисления</typeparam>
public class AdditionEvaluation<T> : BinaryFunctionOperatorEvaluation<T>
{
    /// <summary>Инициализация нового вычисления суммы двух вычислений</summary>
    public AdditionEvaluation() : base(Ex.Add) { }

    /// <summary>Инициализация нового вычисления суммы двух вычислений</summary>
    /// <param name="a">Вычисление первого слагаемого</param>
    /// <param name="b">Вычисление второго слагаемого</param>
    public AdditionEvaluation(Evaluation<T> a, Evaluation<T> b) : base(Ex.Add, a, b) { }
}