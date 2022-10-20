using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evaluations;

/// <summary>Вычисление произведения двух вычислений</summary>
/// <typeparam name="T">Тип значения вычисления</typeparam>
public class MultiplyEvaluation<T> : BinaryFunctionOperatorEvaluation<T>
{
    /// <summary>Инициализация нового вычисления произведения</summary>
    public MultiplyEvaluation() : base(Ex.Multiply) { }

    /// <summary>Инициализация нового вычисления произведения</summary>
    /// <param name="a">Вычисление первого сомножителя</param>
    /// <param name="b">Вычисление второго сомножителя</param>
    public MultiplyEvaluation(Evaluation<T> a, Evaluation<T> b) : base(Ex.Multiply, a, b) { }
}