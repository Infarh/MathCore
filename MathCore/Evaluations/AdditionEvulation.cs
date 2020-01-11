using Ex = System.Linq.Expressions.Expression;
// ReSharper disable UnusedMember.Global

namespace MathCore.Evaluations
{
    /// <summary>Вычисление суммы двух вычислений</summary>
    /// <typeparam name="T">Тип значения вычисления</typeparam>
    public class AdditionEvulation<T> : BinaryFunctionOperatorEvulation<T>
    {
        /// <summary>Инициализация нового вычисления суммы двух вычислений</summary>
        public AdditionEvulation() : base(Ex.Add) { }

        /// <summary>Инициализация нового вычисления суммы двух вычислений</summary>
        /// <param name="a">Вычисление первого слагаемого</param>
        /// <param name="b">Вычисление второго слагаемого</param>
        public AdditionEvulation(Evulation<T> a, Evulation<T> b) : base(Ex.Add, a, b) { }
    }
}