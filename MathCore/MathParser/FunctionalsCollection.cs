using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        public FunctionalsCollection([NotNull] MathExpression Expression)
        {
            Contract.Requires(Expression != null);
            _Expression = Expression;
        }

        /// <summary>Добавить функционал в коллекцию</summary>
        /// <param name="Operator">Добавляемый функционал</param>
        /// <returns>Истина, если добавление прошло успешно</returns>
        public bool Add([NotNull] Functional Operator)
        {
            Contract.Requires(Operator != null);
            if(_Operators.Exists(o => o.Name == Operator.Name)) return false;
            _Operators.Add(Operator);
            return true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        [NotNull]
        IEnumerator<Functional> IEnumerable<Functional>.GetEnumerator()
        {
            Contract.Ensures(Contract.Result<IEnumerator<Functional>>() != null);
            return _Operators.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        [NotNull]
        IEnumerator IEnumerable.GetEnumerator()
        {
            Contract.Ensures(Contract.Result<IEnumerator>() != null);
            return ((IEnumerable)_Operators).GetEnumerator();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        [NotNull]
        public override string ToString()
        {
            Contract.Ensures(Contract.Result<string>() != null);
            return $"Complex operator collection > count = {Count}";
        }

        [ContractInvariantMethod]
        private void ObjectInvariant() => Contract.Invariant(_Expression != null);
    }
}