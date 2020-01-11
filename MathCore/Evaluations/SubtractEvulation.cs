using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evaluations
{
    /// <summary>Вычисление разности двух вычислений</summary>
    /// <typeparam name="T">Тип значения вычисления</typeparam>
    public class SubtractEvulation<T> : BinaryFunctionOperatorEvulation<T>
    {
        /// <summary>Инициализация нового вычисления разности</summary>
        public SubtractEvulation() : base(Ex.Subtract) { }

        /// <summary>Инициализация нового вычисления разности</summary>
        /// <param name="a">Вычисление первого сомножителя</param>
        /// <param name="b">Вычисление второго сомножителя</param>
        public SubtractEvulation(Evulation<T> a, Evulation<T> b) : base(Ex.Subtract, a, b) { }
    }
}