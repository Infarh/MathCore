using System;

namespace MathCore.MathParser
{
    /// <summary>лямбда-переменная</summary>
    /// <remarks>Значение переменной - результат вычисления лямбда-функции</remarks>
    public class LambdaExpressionVariable : ExpressionVariable
    {
        /// <summary>Функция вычисления значения переменной</summary>
        private readonly Func<double> _Value;

        /// <summary>Признак отсутствия возможности предвычисления значения</summary>
        public override bool IsPrecomputable => false;

        /// <summary>Инициализация нового экземпляра лямбда-переменной</summary>
        /// <param name="Source">лямбда-функция получения значения переменной</param>
        public LambdaExpressionVariable(Func<double> Source) : this(string.Empty, Source) { }

        /// <summary>Инициализация нового экземпляра лямбда-переменной</summary>
        /// <param name="Name">Имя переменной</param>
        /// <param name="Source">лямбда-функция получения значения переменной</param>
        public LambdaExpressionVariable(string Name, Func<double> Source) : base(Name) => _Value = Source;

        /// <summary>Получить значение переменной</summary>
        /// <returns>Численное значение переменной</returns>
        public override double GetValue() => Value = _Value();

        /// <summary>Клонировать переменную</summary>
        /// <returns>Новый экземпляр лямбда-переменной с тем же именем и клоном функции</returns>
        public override ExpressionVariable Clone() => new LambdaExpressionVariable(Name, (Func<double>)_Value.Clone());
    }
}