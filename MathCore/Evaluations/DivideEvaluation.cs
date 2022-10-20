using Ex = System.Linq.Expressions.Expression;
// ReSharper disable UnusedMember.Global

namespace MathCore.Evaluations;

/// <summary>Вычисление деления двух вычислений</summary>
/// <typeparam name="T">Тип значения вычисления</typeparam>
public class DivideEvaluation<T> : BinaryFunctionOperatorEvaluation<T>
{
    /// <summary>Инициализация нового вычисления деления</summary>
    public DivideEvaluation() : base(Ex.Divide) { }

    /// <summary>Инициализация нового вычисления деления</summary>
    /// <param name="a">Вычисление делимого</param>
    /// <param name="b">Вычисление делителя</param>
    public DivideEvaluation(Evaluation<T> a, Evaluation<T> b) : base(Ex.Divide, a, b) { }
}