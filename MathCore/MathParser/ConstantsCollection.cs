using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable NotAccessedField.Local

namespace MathCore.MathParser
{
    /// <summary>Коллекция констант</summary>
    [System.Diagnostics.DebuggerDisplay("Колличество зафиксированных констант = {" + nameof(Count) + "}"), DST]
    public sealed class ConstantsCollection : IEnumerable<ExpressionVariabel>
    {
        /// <summary>Ссылка на выражение</summary>
        [NotNull]
        private readonly MathExpression _Expression;

        /// <summary>Элементы коллекци</summary>
        [NotNull]
        private readonly List<ExpressionVariabel> _Items = new List<ExpressionVariabel>();

        /// <summary>Количество элементов коллекции</summary>
        public int Count => _Items.Count;

        /// <summary>Итератор констант по имени</summary>
        /// <param name="Name">Имя константы</param>
        /// <returns>Константа с указанным именем</returns>
        [NotNull]
        public ExpressionVariabel this[[NotNull] string Name]
        {
            get
            {
                Contract.Requires(!string.IsNullOrWhiteSpace(Name));
                Contract.Ensures(Contract.Result<ExpressionVariabel>() != null);
                if(Name == null) throw new ArgumentNullException(nameof(Name));
                if(string.IsNullOrEmpty(Name)) throw new ArgumentOutOfRangeException(nameof(Name));
                var c = _Items.Find(v => v.Name == Name);
                if(c == null) throw new ArgumentException($"Константа с именем {Name} не найдена");
                return c;
            }
        }

        /// <summary>Инициализация новой коллекции констант</summary>
        /// <param name="Expression">Математическое выражение, которому принадлежит коллекция</param>
        public ConstantsCollection([NotNull] MathExpression Expression)
        {
            Contract.Requires(Expression != null);
            _Expression = Expression;
        }

        /// <summary>Добавить элемент в коллекцию</summary>
        /// <param name="Constant">Добавляемое значение, как константа</param>
        public bool Add([NotNull] ExpressionVariabel Constant)
        {
            Contract.Requires(Constant != null);
            if(_Items.Contains(v => v.Name == Constant.Name)) return false;
            Constant.IsConstant = true;
            _Items.Add(Constant);
            return true;
        }

        /// <summary>Получить имена констант колеекции</summary>
        /// <returns>Перечисление имён констант колеекции</returns>
        [NotNull]
        public IEnumerable<string> GetNames()
        {
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);
            return _Items.Select(v => v.Name);
        }

        /// <summary>Получить перечислитеь констант коллекци</summary>
        /// <returns>Перечислитель констант</returns>
        [NotNull]
        IEnumerator<ExpressionVariabel> IEnumerable<ExpressionVariabel>.GetEnumerator() => _Items.GetEnumerator();

        /// <summary>Получить перечислитеь констант коллекци</summary>
        /// <returns>Перечислитель констант</returns>
        [NotNull]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<ExpressionVariabel>)this).GetEnumerator();
    }
}