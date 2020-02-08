using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable NotAccessedField.Local

namespace MathCore.MathParser
{
    /// <summary>Коллекция констант</summary>
    [System.Diagnostics.DebuggerDisplay("Количество зафиксированных констант = {" + nameof(Count) + "}"), DST]
    public sealed class ConstantsCollection : IEnumerable<ExpressionVariable>
    {
        /// <summary>Ссылка на выражение</summary>
        [NotNull]
        private readonly MathExpression _Expression;

        /// <summary>Элементы коллекции</summary>
        [NotNull]
        private readonly List<ExpressionVariable> _Items = new List<ExpressionVariable>();

        /// <summary>Количество элементов коллекции</summary>
        public int Count => _Items.Count;

        /// <summary>Итератор констант по имени</summary>
        /// <param name="Name">Имя константы</param>
        /// <returns>Константа с указанным именем</returns>
        [NotNull]
        public ExpressionVariable this[[NotNull] string Name]
        {
            get
            {
                if(Name is null) throw new ArgumentNullException(nameof(Name));
                if(string.IsNullOrEmpty(Name)) throw new ArgumentOutOfRangeException(nameof(Name));
                var c = _Items.Find(v => v.Name == Name);
                if(c is null) throw new ArgumentException($"Константа с именем {Name} не найдена");
                return c;
            }
        }

        /// <summary>Инициализация новой коллекции констант</summary>
        /// <param name="Expression">Математическое выражение, которому принадлежит коллекция</param>
        public ConstantsCollection([NotNull] MathExpression Expression) => _Expression = Expression;

        /// <summary>Добавить элемент в коллекцию</summary>
        /// <param name="Constant">Добавляемое значение, как константа</param>
        public bool Add([NotNull] ExpressionVariable Constant)
        {
            if(_Items.Contains(v => v.Name == Constant.Name)) return false;
            Constant.IsConstant = true;
            _Items.Add(Constant);
            return true;
        }

        /// <summary>Получить имена констант коллекции</summary>
        /// <returns>Перечисление имён констант коллекции</returns>
        [NotNull]
        public IEnumerable<string> GetNames() => _Items.Select(v => v.Name);

        /// <summary>Получить перечислитеь констант коллекции</summary>
        /// <returns>Перечислитель констант</returns>
        [NotNull]
        IEnumerator<ExpressionVariable> IEnumerable<ExpressionVariable>.GetEnumerator() => _Items.GetEnumerator();

        /// <summary>Получить перечислитеь констант коллекции</summary>
        /// <returns>Перечислитель констант</returns>
        [NotNull]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<ExpressionVariable>)this).GetEnumerator();
    }
}