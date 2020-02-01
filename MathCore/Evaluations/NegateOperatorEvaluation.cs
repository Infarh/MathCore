using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evaluations
{
    /// <summary>Вычисление отрицания значения</summary>
    /// <typeparam name="T">Тип значения параметра</typeparam>
    public class NegateOperatorEvaluation<T> : UnaryOperatorEvaluation<T, T>
    {
        /// <summary>Инициализация нового вычисления отризацания</summary>
        public NegateOperatorEvaluation() : base(Ex.Negate) { }

        /// <summary>Инициализация нового вычисления отрицания</summary>
        /// <param name="value">Вычисление значения операнда</param>
        public NegateOperatorEvaluation(Evulation<T> value) : base(Ex.Negate, value) { }
    }
}