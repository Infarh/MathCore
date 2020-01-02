using System;
using Ex = System.Linq.Expressions.Expression;
// ReSharper disable UnusedMember.Global

namespace MathCore.Evulations
{
    /// <summary>Вычисление бинарной опирации между двумя вычислениями</summary>
    /// <typeparam name="T">Тип значения вычисления</typeparam>
    public class BinaryFunctionOperationEvulation<T> : Evulation<T>
    {
        /// <summary>Первый операнд вычисления</summary>
        public Evulation<T> A { get; set; }

        /// <summary>Второй операнд вычисления</summary>
        public Evulation<T> B { get; set; }

        /// <summary>Метод вычисления значения вычисления</summary>
        public Func<T, T, T> Operation { get; set; }

        /// <summary>Инициализация нового бинарного вычисления</summary>
        protected BinaryFunctionOperationEvulation() { }

        /// <summary>Инициализация нового бинарного вычисления</summary>
        /// <param name="Operation">Метод вычисления результата вычисления на основе результатов вычисления значений операндов</param>
        protected BinaryFunctionOperationEvulation(Func<T, T, T> Operation) => this.Operation = Operation;

        /// <summary>Инициализация нового бинарного вычисления</summary>
        /// <param name="Operation">Метод вычисления результата вычисления на основе результатов вычисления значений операндов</param>
        /// <param name="A">Первый операнд вычисления</param>
        /// <param name="B">Второй операнд вычисления</param>
        protected BinaryFunctionOperationEvulation(Func<T, T, T> Operation, Evulation<T> A, Evulation<T> B)
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
}