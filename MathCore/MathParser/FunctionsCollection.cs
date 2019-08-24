using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.MathParser
{
    /// <summary>Коллекция функций</summary>
    [DebuggerDisplay("Количество зарегистрированных функций = {Count}")]
    public class FunctionsCollection : IEnumerable<ExpressionFunction>
    {
        /// <summary>Ссылка на математическое выражение</summary>
        [NotNull]
        private readonly MathExpression _MathExpression;

        /// <summary>Список функций математического выражения</summary>
        [NotNull]
        private readonly List<ExpressionFunction> _Functions = new List<ExpressionFunction>();

        /// <summary>Имена функций</summary>
        public IEnumerable<string> Names
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);
                return _Functions.Select(v => v.Name);
            }
        }

        /// <summary>Количество используемых функций</summary>
        public int Count
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return _Functions.Count;
            }
        }

        /// <summary>Индексатор функций по имени и списку параметров</summary>
        /// <param name="Name">Имя функции</param>
        /// <param name="ArgumentsCount">Количество аргументов</param>
        /// <returns>Функция, удовлетворяющаяя заданой сигнатуре</returns>
        [NotNull]
        public ExpressionFunction this[[NotNull] string Name, int ArgumentsCount]
        {
            get
            {
                Contract.Requires(Name != null);
                Contract.Requires(ArgumentsCount >= 0);
                Contract.Ensures(Contract.Result<ExpressionFunction>() != null);
                var function = _Functions.FirstOrDefault(f => f.IsEqualSignature(Name, ArgumentsCount));
                if(function != null) return function;
                function = new ExpressionFunction(Name, new string[ArgumentsCount]);
                _Functions.Add(function);
                return function;
            }
        }

        /// <summary>Индексатор функций по имени и списку параметров</summary>
        /// <param name="Name">Имя функции</param>
        /// <param name="Arguments">Список имён аргументов</param>
        /// <returns>Функция, удовлетворяющаяя заданой сигнатуре</returns>
        [NotNull]
        public ExpressionFunction this[[NotNull] string Name, [NotNull] params string[] Arguments]
        {
            get
            {
                Contract.Requires(Name != null);
                Contract.Requires(Arguments != null);
                Contract.Ensures(Contract.Result<ExpressionFunction>() != null);
                var function = _Functions.FirstOrDefault(f => f.IsEqualSignature(Name, Arguments));
                if(function != null) return function;
                function = new ExpressionFunction(Name, Arguments);
                _Functions.Add(function);
                return function;
            }
        }

        /// <summary>Инициализация новой коллекции функций математического выражения</summary>
        /// <param name="MathExpression">Математическое выражение, на которое ссылается создаваемая коллекция</param>
        public FunctionsCollection([NotNull] MathExpression MathExpression)
        {
            Contract.Requires(MathExpression != null);
            _MathExpression = MathExpression;
        }


        /// <summary>Добавить функцию в коллекцию</summary>
        /// <param name="function">Функция</param>
        /// <returns>Истина, если функция была добавлена</returns>
        public bool Add([NotNull] ExpressionFunction function)
        {
            Contract.Requires(function != null);
            var F = _Functions.FirstOrDefault(f => f.IsEqualSignature(function.Name, function.Arguments));
            if(F != null) return false;
            _Functions.Add(function);
            return true;
        }

        /// <summary>Возвращает перечислитель, выполняющий перебор элементов в коллекции</summary>
        /// <returns>
        /// Интерфейс <see cref="T:System.Collections.Generic.IEnumerator`1"/>, который может использоваться для перебора элементов коллекции.
        /// </returns>
        [NotNull]
        IEnumerator<ExpressionFunction> IEnumerable<ExpressionFunction>.GetEnumerator() => _Functions.GetEnumerator();

        /// <summary>Возвращает перечислитель, который осуществляет перебор элементов коллекции</summary>
        /// <returns>
        /// Объект <see cref="T:System.Collections.IEnumerator"/>, который может использоваться для перебора элементов коллекции.
        /// </returns>
        [NotNull]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Functions).GetEnumerator();
    }
}