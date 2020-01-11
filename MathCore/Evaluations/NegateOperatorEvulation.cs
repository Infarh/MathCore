using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evaluations
{
    /// <summary>Вычисление отрицания значения</summary>
    /// <typeparam name="T">Тип значения параметра</typeparam>
    public class NegateOperatorEvulation<T> : UnaryOperatorEvulation<T, T>
    {
        /// <summary>Инициализация нового вычисления отризацания</summary>
        public NegateOperatorEvulation() : base(Ex.Negate) { }

        /// <summary>Инициализация нового вычисления отрицания</summary>
        /// <param name="value">Вычисление значения операнда</param>
        public NegateOperatorEvulation(Evulation<T> value) : base(Ex.Negate, value) { }
    }
}