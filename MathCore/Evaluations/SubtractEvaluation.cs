using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evaluations;

/// <summary>Вычисление разности двух вычислений</summary>
/// <typeparam name="T">Тип значения вычисления</typeparam>
public class SubtractEvaluation<T> : BinaryFunctionOperatorEvaluation<T>
{
    /// <summary>Инициализация нового вычисления разности</summary>
    public SubtractEvaluation() : base(Ex.Subtract) { }

    /// <summary>Инициализация нового вычисления разности</summary>
    /// <param name="a">Вычисление первого сомножителя</param>
    /// <param name="b">Вычисление второго сомножителя</param>
    public SubtractEvaluation(Evaluation<T> a, Evaluation<T> b) : base(Ex.Subtract, a, b) { }
}