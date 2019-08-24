using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using MathCore.Annotations;

namespace MathCore.MathParser
{
    /// <summary>Переменная математического выражения</summary>
    public class ExpressionVariabel : ExpressionItem, ICloneable<ExpressionVariabel>
    {
        /// <summary>Значение переменной</summary>
        private double _Value;
        /// <summary>Является ли константой?</summary>
        private bool _IsConstant;

        /// <summary>Значение переменной</summary>
        public virtual double Value { get => _Value; set => Set(ref _Value, value); }

        /// <summary>Признак возможности предвычисления значения</summary>
        public virtual bool IsPrecomputable => true;

        /// <summary>Является ли константой?</summary>
        public bool IsConstant { get => _IsConstant; set => Set(ref _IsConstant, value); }

        /// <summary>Метод извлечения значения</summary>
        /// <returns>Численное значение переменной</returns>
        public override double GetValue() => Value;

        /// <summary>Инициализация нового экземпляра переменной</summary>
        /// <param name="Name">Имя переменной</param>
        public ExpressionVariabel(string Name) : base(Name) { }

        /// <summary>Клонирование переменной</summary>
        /// <returns>Новый экземпляр переменной с тем же имененм и тем же значением</returns>
        public virtual ExpressionVariabel Clone() => new ExpressionVariabel(Name) { Value = Value };

        /// <summary>Преобразование в строку</summary>
        /// <returns>Строковое представление переменной</returns>
        public override string ToString() => $"{Name}={_Value}";

        object ICloneable.Clone() => Clone();

        /// <summary>Оператор неявного привидения вещественного числа к типу переменной</summary>
        /// <param name="x">Вещественное число</param>
        /// <returns>Безымянная переменная, хранящая указанное число</returns>
        public static implicit operator ExpressionVariabel(double x) =>
            new ExpressionVariabel("") { _Value = x };

        /// <summary>Оператор неявного привидения к типу вещественного числа</summary>
        /// <param name="variable">Приводимая переменная</param>
        /// <returns>Значение переменной</returns>
        public static implicit operator double(ExpressionVariabel variable) =>
            variable.GetValue();
    }
}