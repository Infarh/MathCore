using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.MathParser
{
    /// <summary>Коллекция функций</summary>
    [DebuggerDisplay("Количество зарегистрированных функций = {" + nameof(Count) + "}")]
    public class FunctionsCollection : IEnumerable<ExpressionFunction>
    {
        /// <summary>Список функций математического выражения</summary>
        [NotNull]
        private readonly List<ExpressionFunction> _Functions = new List<ExpressionFunction>();

        /// <summary>Имена функций</summary>
        [NotNull]
        public IEnumerable<string> Names => _Functions.Select(v => v.Name);

        /// <summary>Количество используемых функций</summary>
        public int Count => _Functions.Count;

        /// <summary>Индексатор функций по имени и списку параметров</summary>
        /// <param name="Name">Имя функции</param>
        /// <param name="ArgumentsCount">Количество аргументов</param>
        /// <returns>Функция, удовлетворяющая загаданной сигнатуре</returns>
        [NotNull]
        public ExpressionFunction this[[NotNull] string Name, int ArgumentsCount]
        {
            get
            {
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
        /// <returns>Функция, удовлетворяющая загаданной сигнатуре</returns>
        [NotNull]
        public ExpressionFunction this[[NotNull] string Name, [NotNull] params string[] Arguments]
        {
            get
            {
                var function = _Functions.FirstOrDefault(f => f.IsEqualSignature(Name, Arguments));
                if(function != null) return function;
                function = new ExpressionFunction(Name, Arguments);
                _Functions.Add(function);
                return function;
            }
        }

        /// <summary>Добавить функцию в коллекцию</summary>
        /// <param name="function">Функция</param>
        /// <returns>Истина, если функция была добавлена</returns>
        public bool Add([NotNull] ExpressionFunction function)
        {
            var F = _Functions.FirstOrDefault(f => f.IsEqualSignature(function.Name, function.Arguments));
            if(F != null) return false;
            _Functions.Add(function);
            return true;
        }

        /// <inheritdoc />
        IEnumerator<ExpressionFunction> IEnumerable<ExpressionFunction>.GetEnumerator() => _Functions.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Functions).GetEnumerator();
    }
}