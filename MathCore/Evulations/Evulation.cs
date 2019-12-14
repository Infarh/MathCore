using System;
using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evulations
{
    /// <summary>Вычисление</summary>
    public abstract class Evulation
    {
        /// <summary>Получить выражение вычисления</summary>
        /// <returns>Возвращает выражение, соответствующее данному вычислению</returns>
        public abstract Ex GetExpression();

        /// <summary>Оператор неявного преобразования вычисления в выражение</summary>
        /// <param name="evulation">Преобразвуемое вычисление</param>
        public static implicit operator Ex(Evulation evulation) => evulation.GetExpression();
    }

    /// <summary>Вычисление с результатом типа <typeparamref name="T"/></summary>
    /// <typeparam name="T">Тип результата вычисления</typeparam>
    public abstract class Evulation<T> : Evulation
    {
        /// <summary>Получить значение</summary>
        /// <returns></returns>
        public abstract T GetValue();

        /// <summary>Получить выражение вычисления</summary>
        /// <returns>Выражение, определяющие вычисление</returns>
        public override Ex GetExpression() => ((Func<T>) GetValue).GetCallExpression(this.ToExpression());
        //public override Ex GetExpression() => Ex.Call(Ex.Constant(this), ((Func<T>)GetValue).Method);

        /// <summary>Оператор неявного преобразования вычисления в выражение</summary>
        /// <param name="evulation">Преобразвуемое вычисление</param>
        public static implicit operator T(Evulation<T> evulation) => evulation.GetValue();

        /// <summary>Оператор сложения двух вычислений</summary>
        /// <param name="x">Первое слагаемое</param>
        /// <param name="y">Второе слагаемое</param>
        /// <returns>Вычисление суммы двух вычислений</returns>
        public static AdditionEvulation<T> operator +(Evulation<T> x, Evulation<T> y) => new AdditionEvulation<T>(x, y);

        /// <summary>Вычисление разности двух вычислений</summary>
        /// <param name="x">Уменьшаемое</param>
        /// <param name="y">Вычитаемое</param>
        /// <returns>Вычисление разности двух вычислений</returns>
        public static SubtractEvulation<T> operator -(Evulation<T> x, Evulation<T> y) => new SubtractEvulation<T>(x, y);

        /// <summary>Оператор получения отрицательного значения на основе вычисления</summary>
        /// <param name="x">Вычисление значения оператора</param>
        /// <returns>Оператор получения отрицательного значения</returns>
        public static NegateOperatorEvulation<T> operator -(Evulation<T> x) => new NegateOperatorEvulation<T>(x);

        /// <summary>Вычисление произведения двух вычислений</summary>
        /// <param name="x">Первый сомножитель</param>
        /// <param name="y">Второй сомножитель</param>
        /// <returns>Вычисление произведения двух вычислений</returns>
        public static MultiplyEvulation<T> operator *(Evulation<T> x, Evulation<T> y) => new MultiplyEvulation<T>(x, y);

        /// <summary>Вычисление частного двух вычислений</summary>
        /// <param name="x">Делимое</param>
        /// <param name="y">Делитель</param>
        /// <returns>Вычисление частного двух вычислений</returns>
        public static DivideEvulation<T> operator /(Evulation<T> x, Evulation<T> y) => new DivideEvulation<T>(x, y);
    }
}