using Ex = System.Linq.Expressions.Expression;
// ReSharper disable UnusedMember.Global

namespace MathCore.Evaluations;

/// <summary>Вычисление бинарной операции между двумя вычислениями</summary>
/// <typeparam name="T">Тип значения вычисления</typeparam>
public class BinaryFunctionOperationEvaluation<T> : Evaluation<T>
{
    /// <summary>Первый операнд вычисления</summary>
    public Evaluation<T> A { get; set; }

    /// <summary>Второй операнд вычисления</summary>
    public Evaluation<T> B { get; set; }

    /// <summary>Метод вычисления значения вычисления</summary>
    public Func<T, T, T> Operation { get; set; }

    /// <summary>Инициализация нового бинарного вычисления</summary>
    protected BinaryFunctionOperationEvaluation() { }

    /// <summary>Инициализация нового бинарного вычисления</summary>
    /// <param name="Operation">Метод вычисления результата вычисления на основе результатов вычисления значений операндов</param>
    protected BinaryFunctionOperationEvaluation(Func<T, T, T> Operation) => this.Operation = Operation;

    /// <summary>Инициализация нового бинарного вычисления</summary>
    /// <param name="Operation">Метод вычисления результата вычисления на основе результатов вычисления значений операндов</param>
    /// <param name="A">Первый операнд вычисления</param>
    /// <param name="B">Второй операнд вычисления</param>
    protected BinaryFunctionOperationEvaluation(Func<T, T, T> Operation, Evaluation<T> A, Evaluation<T> B)
        : this(Operation)
    {
        this.A = A;
        this.B = B;
    }

    /// <inheritdoc />
    public override T GetValue() => Operation(A.GetValue(), B.GetValue());

    /// <inheritdoc />
    public override Ex GetExpression() => Ex.Call(Operation.Target?.ToExpression(), Operation.Method, A.GetExpression(), B.GetExpression());
}