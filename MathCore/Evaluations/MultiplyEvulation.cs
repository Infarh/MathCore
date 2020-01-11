using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evaluations
{
    /// <summary>Вычисление произведения двух вычислений</summary>
    /// <typeparam name="T">Тип значения вычисления</typeparam>
    public class MultiplyEvulation<T> : BinaryFunctionOperatorEvulation<T>
    {
        /// <summary>Инициализация нового вычисления произведения</summary>
        public MultiplyEvulation() : base(Ex.Multiply) { }

        /// <summary>Инициализация нового вычисления произведения</summary>
        /// <param name="a">Вычисление первого сомножителя</param>
        /// <param name="b">Вычисление второго сомножителя</param>
        public MultiplyEvulation(Evulation<T> a, Evulation<T> b) : base(Ex.Multiply, a, b) { }
    }
}