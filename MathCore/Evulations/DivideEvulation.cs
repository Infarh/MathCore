using Ex = System.Linq.Expressions.Expression;
// ReSharper disable UnusedMember.Global

namespace MathCore.Evulations
{
    /// <summary>Вычисление деления двух вычислений</summary>
    /// <typeparam name="T">Тип значения вычисления</typeparam>
    public class DivideEvulation<T> : BinaryFunctionOperatorEvulation<T>
    {
        /// <summary>Инициализация нового вычисления деления</summary>
        public DivideEvulation() : base(Ex.Divide) { }

        /// <summary>Инициализация нового вычисления деления</summary>
        /// <param name="a">Вычисление делимого</param>
        /// <param name="b">Вычисление делителя</param>
        public DivideEvulation(Evulation<T> a, Evulation<T> b) : base(Ex.Divide, a, b) { }
    }
}