using System.Collections;
using System.Collections.Generic;
using MathCore.Annotations;

// ReSharper disable ClassCanBeSealed.Global

namespace MathCore.MathParser
{
    /// <summary>Коллекция функционалов</summary>
    public class FunctionalsCollection : IEnumerable<Functional>
    {
        /// <summary>Список функционалов</summary>
        [NotNull]
        private readonly List<Functional> _Operators = new List<Functional>();

        /// <summary>Ссылка на математическое выражение, с которым связана поллекция</summary>
        [NotNull]
        private readonly MathExpression _Expression;

        /// <summary>Количество функционалов в коллекции</summary>
        public int Count => _Operators.Count;

        /// <summary>Инициализация новой коллекции функционалов</summary>
        /// <param name="Expression">Математическое выражение, на которое ссылается коллекция</param>
        public FunctionalsCollection([NotNull] MathExpression Expression) => _Expression = Expression;

        /// <summary>Добавить функционал в коллекцию</summary>
        /// <param name="Operator">Добавляемый функционал</param>
        /// <returns>Истина, если добавление прошло успешно</returns>
        public bool Add([NotNull] Functional Operator)
        {
            if(_Operators.Exists(o => o.Name == Operator.Name)) return false;
            _Operators.Add(Operator);
            return true;
        }

        IEnumerator<Functional> IEnumerable<Functional>.GetEnumerator() => _Operators.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Operators).GetEnumerator();

        public override string ToString() => $"Complex operator collection > count = {Count}";
    }
}