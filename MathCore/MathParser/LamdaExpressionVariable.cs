using System;

namespace MathCore.MathParser
{
    /// <summary>Лямда-переменная</summary>
    /// <remarks>Значение переменной - результат вычисления лямда-функции</remarks>
    public class LamdaExpressionVariable : ExpressionVariabel
    {
        /// <summary>Функция вычисления значения переменной</summary>
        private readonly Func<double> _Value;

        /// <summary>Признак отсутствия возможности предвычисления значения</summary>
        public override bool IsPrecomputable => false;

        /// <summary>Инициализация нового экземпляра лямда-переменной</summary>
        /// <param name="Source">Лямда-функция получения значения переменной</param>
        public LamdaExpressionVariable(Func<double> Source) : this("", Source) { }

        /// <summary>Инициализация нового экземпляра лямда-переменной</summary>
        /// <param name="Name">Имя переменной</param>
        /// <param name="Source">Лямда-функция получения значения переменной</param>
        public LamdaExpressionVariable(string Name, Func<double> Source) : base(Name) => _Value = Source;

        /// <summary>Получить значение переменной</summary>
        /// <returns>Численное значение переменной</returns>
        public override double GetValue() => Value = _Value();

        /// <summary>Клонировать переменную</summary>
        /// <returns>Новый экземпляр лямда-переменной с тем же именем и клоном функции</returns>
        public override ExpressionVariabel Clone() => new LamdaExpressionVariable(Name, (Func<double>)_Value.Clone());
    }
}