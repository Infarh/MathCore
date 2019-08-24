using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evulations
{
    /// <summary>¬ычисление произведени€ двух вычислений</summary>
    /// <typeparam name="T">“ип значени€ вычислени€</typeparam>
    public class MultiplyEvulation<T> : BinaryFunctionOperatorEvulation<T>
    {
        /// <summary>»нициализаци€ нового вычислени€ произведени€</summary>
        public MultiplyEvulation() : base(Ex.Multiply) { }

        /// <summary>»нициализаци€ нового вычислени€ произведени€</summary>
        /// <param name="a">¬ычисление первого сомножител€</param>
        /// <param name="b">¬ычисление второго сомножител€</param>
        public MultiplyEvulation(Evulation<T> a, Evulation<T> b) : base(Ex.Multiply, a, b) { }
    }
}