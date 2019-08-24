using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using MathCore.Annotations;
using MathCore.MathParser.ExpressionTrees.Nodes;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace MathCore.MathParser
{
    /// <summary>Коллекция переменных</summary>
    [DebuggerDisplay("Variabels count = {" + nameof(Count) + "}")]
    public class VariabelsCollection : IEnumerable<ExpressionVariabel>
    {
        /// <summary>Математическое выражение</summary>
        private readonly MathExpression _Expression;

        [NotNull]
        private readonly List<ExpressionVariabel> _Variabels = new List<ExpressionVariabel>();

        /// <summary>Количество переменных в коллекции</summary>
        public int Count => _Variabels.Count;

        /// <summary>Итератор переменных коллекции</summary>
        /// <param name="Name">Имя переменной</param>
        /// <returns>Переменная с указанным именем</returns>
        [NotNull]
        public ExpressionVariabel this[[NotNull] string Name]
        {
            [DST]
            get
            {
                if(Name == null) throw new ArgumentNullException(nameof(Name));
                if(string.IsNullOrEmpty(Name)) throw new ArgumentOutOfRangeException(nameof(Name));
                Contract.EndContractBlock();
                return (_Variabels.Find(v => v.Name == Name)
                       ?? new ExpressionVariabel(Name).InitializeObject(v => Add(v))) 
                       ?? throw new InvalidOperationException();
            }
            [DST]
            set
            {
                if(value == null) throw new ArgumentNullException(nameof(value));
                if(Name == null) throw new ArgumentNullException(nameof(Name));
                if(string.IsNullOrEmpty(Name)) throw new ArgumentOutOfRangeException(nameof(Name));
                Contract.EndContractBlock();
                var old_var = _Variabels.Find(v => v.Name == Name);

                if(value is LamdaExpressionVariable || value is EventExpressionVariable)
                {
                    value.Name = Name;
                    _Expression.Tree //Обойти все узлы дерева
                                      // являющиеся узлами переменных
                                .Where(node => node is VariableValueNode)
                                .Cast<VariableValueNode>()
                                // у которых имя соответствует заданному
                                .Where(node => node.Variable.Name == Name)
                                // и для каждого узла заменить переменную на указанную
                                .Foreach(node => node.Variable = value);
                    _Variabels.Remove(old_var);
                    Add(value);
                }
                else if(old_var == null)
                    Add(value);
                else
                    old_var.Value = value.GetValue();
            }

        }

        /// <summary>Итератор переменных коллекции</summary>
        /// <param name="i">Индекс переменной</param>
        /// <returns>Переменная с указанным индексом</returns>
        [NotNull]
        public ExpressionVariabel this[int i]
        {
            get
            {
                Contract.Requires(i >= 0);
                Contract.Requires(i < Count);
                Contract.Ensures(Contract.Result<ExpressionVariabel>() != null);
                return _Variabels[i];
            }
        }

        /// <summary>Перечисление всех имён переменных колеекции</summary>
        [NotNull]
        public IEnumerable<string> Names
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);
                return _Variabels.Select(v => v.Name);
            }
        }

        /// <summary>Инициализация новой коллекции переменных</summary>
        /// <param name="expression">Математическое выражение, которому принадлежит коллекция</param>
        public VariabelsCollection([NotNull] MathExpression expression)
        {
            Contract.Requires(expression != null);
            _Expression = expression;
        }

        /// <summary>Добавить переменную в коллекцию</summary>
        /// <param name="Variable">Переменная</param>
        /// <returns>Истина, если переменная была добавлена</returns>
        public bool Add([NotNull] ExpressionVariabel Variable)
        {
            Contract.Requires(Variable != null);
            Contract.Ensures(Contract.OldValue(Count) == Count + 1);
            var variable = _Variabels.Find(v => v.Name == Variable.Name);
            if(variable != null) return false;
            Variable.IsConstant = false;
            _Variabels.Add(Variable);
            return true;
        }

        public bool Replace([NotNull] string Name, [NotNull] ExpressionVariabel variable)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(Name));
            Contract.Requires(variable != null);

            var replaced = false;
            var old_var = _Variabels.Find(v => v.Name == Name);
            if(old_var == null) return false;


            _Expression.Tree //Обойти все узлы дерева
                              // являющиеся узлами переменных
                                .Where(node => node is VariableValueNode)
                                .Cast<VariableValueNode>()
                                // у которых имя соответствует заданному
                                .Where(node => node.Variable.Name == Name)
                                // и для каждого узла заменить переменную на указанную
                                .Foreach(node =>
                                {
                                    node.Variable = variable;
                                    replaced = true;
                                });
            _Variabels.Remove(old_var);
            Add(variable);
            if(replaced)
                variable.Name = Name;
            return replaced;
        }

        /// <summary>Переместить переменную из коллекции переменных в коллекцию констант</summary>
        /// <param name="Variable">Перемещаемая переменная</param>
        /// <returns>Истина, если переменная была перемещена из коллекции переменных в коллекцию констант</returns>
        public bool MoveToConstCollection([NotNull] string Variable)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(Variable));
            return MoveToConstCollection(this[Variable]);
        }

        /// <summary>Переместить переменную из коллекции переменных в коллекцию констант</summary>
        /// <param name="Variable">Перемещаемая переменная</param>
        /// <returns>Истина, если переменная была перемещена из коллекции переменных в коллекцию констант</returns>
        public bool MoveToConstCollection([NotNull] ExpressionVariabel Variable)
        {
            Contract.Requires(Variable != null);
            return Exist(v => ReferenceEquals(v, Variable))
                   && _Variabels.Remove(Variable) && _Expression.Constants.Add(Variable);
        }

        /// <summary>Удаление переменной из коллекции</summary>
        /// <param name="Variable">Удаляемая переменная</param>
        /// <returns>Истина, если удаление прошло успешно</returns>
        public bool Remove([NotNull] ExpressionVariabel Variable)
        {
            Contract.Requires(Variable != null);
            return !_Expression.Tree
                        .Where(n => n is VariableValueNode)
                        .Any(n => ReferenceEquals(((VariableValueNode)n).Variable, Variable)) 
                    && _Variabels.Remove(Variable);
        }

        /// <summary>Удалить переменную из коллекции</summary>
        /// <param name="Variable">Удаляемая переменная</param>
        /// <returns>Истина, если переменная удалена успешно</returns>
        public bool RemoveFromCollection([NotNull] ExpressionVariabel Variable)
        {
            Contract.Requires(Variable != null);
            return _Variabels.Remove(Variable);
        }

        /// <summary>Очистить коллекцию переменных</summary>
        public void ClearCollection()
        {
            Contract.Ensures(Count == 0);
            _Variabels.Clear();
        }

        /// <summary>Существует ли в коллекции переменная с указанным имененм</summary>
        /// <param name="Name">Искомое имя переменной</param>
        /// <returns>Истина, если в коллекции пристутствует переменная с указанным именем</returns>
        public bool Exist([NotNull] string Name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(Name));
            return Exist(v => v.Name == Name);
        }

        /// <summary>Проверка на существование переменной в коллекции</summary>
        /// <param name="variable">Проверяемая переменная</param>
        /// <returns>Истина, если указанная переменная входит в коллекцию</returns>
        public bool Exist([NotNull] ExpressionVariabel variable)
        {
            Contract.Requires(variable != null);
            return _Variabels.Contains(variable);
        }

        /// <summary>Существует ли переменная в коллекции с заданным критерием поиска</summary>
        /// <param name="exist">Критерий поиска переменной</param>
        /// <returns>Истина, если найдена переменная по указанному критерию</returns>
        public bool Exist([NotNull] Predicate<ExpressionVariabel> exist)
        {
            Contract.Requires(exist != null);
            return _Variabels.Exists(exist);
        }

        /// <summary>Существует ли узел переменной в дереве с указанным именем</summary>
        /// <param name="Name">Искомое имя переменной</param>
        /// <returns>Истина, если указанное имя переменной существует в дереве</returns>
        public bool ExistInTree([NotNull] string Name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(Name));
            return ExistInTree(v => v.Name == Name);
        }

        /// <summary>Существует ли узел переменной в дереве</summary>
        /// <param name="exist">Критерий поиска</param>
        /// <returns>Истина, если найден узел по указанному критерию</returns>
        public bool ExistInTree([NotNull] Func<VariableValueNode, bool> exist)
        {
            Contract.Requires(exist != null);
            return _Expression.Tree
                .Where(n => n is VariableValueNode)
                .Cast<VariableValueNode>()
                .Any(exist);
        }

        /// <summary>Получить перечисление узлов переменных с указанным именем</summary>
        /// <param name="VariableName">Искомое имя переменной</param>
        /// <returns>Перечисление узлов с переменными с указанным именем</returns>
        [NotNull]
        public IEnumerable<VariableValueNode> GetTreeNodes([NotNull] string VariableName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(VariableName));
            Contract.Ensures(Contract.Result<IEnumerable<VariableValueNode>>() != null);
            return GetTreeNodes(v => v.Name == VariableName);
        }

        /// <summary>Получить перечисление узлов дерева с переменными</summary>
        /// <param name="selector">Метод выборки узлов</param>
        /// <returns>Перечисление узлов переменных</returns>
        [NotNull]
        public IEnumerable<VariableValueNode> GetTreeNodes([NotNull] Func<VariableValueNode, bool> selector)
        {
            Contract.Requires(selector != null);
            Contract.Ensures(Contract.Result<IEnumerable<VariableValueNode>>() != null);
            return _Expression.Tree
                .Where(n => n is VariableValueNode)
                .Cast<VariableValueNode>()
                .Where(selector);
        }

        /// <summary>Получить перечисление узлов дерева выражения, содержащих указанный тип переменных</summary>
        /// <typeparam name="TVariable">Тип переменной</typeparam>
        /// <returns>Перечисление узлов дерева с указанным типом переменных</returns>
        [NotNull]
        public IEnumerable<VariableValueNode> GetTreeNodesOf<TVariable>()
            where TVariable : ExpressionVariabel
        {
            Contract.Ensures(Contract.Result<IEnumerable<VariableValueNode>>() != null);
            return _Expression.Tree
                .Where(n => n is VariableValueNode)
                .Cast<VariableValueNode>()
                .Where(n => n.Variable is TVariable);
        }

        /// <summary>Получить перечисление узлов дерева выражения, содержащих указанный тип переменных</summary>
        /// <typeparam name="TVariable">Тип переменной</typeparam>
        /// <param name="selector">Метод выбора узлов по содержащимся в них переменным</param>
        /// <returns>Перечисление узлов дерева с указанным типом переменных</returns>
        [NotNull]
        public IEnumerable<VariableValueNode> GetTreeNodesVOf<TVariable>([NotNull] Func<TVariable, bool> selector)
            where TVariable : ExpressionVariabel
        {
            Contract.Requires(selector != null);
            Contract.Ensures(Contract.Result<IEnumerable<VariableValueNode>>() != null);
            return _Expression.Tree
                .Where(n => n is VariableValueNode)
                .Cast<VariableValueNode>()
                .Where(n => n.Variable is TVariable)
                .Where(n => selector((TVariable)n.Variable));
        }

        /// <summary>Получить перечисление узлов дерева выражения, содержащих указанный тип переменных</summary>
        /// <typeparam name="TVariable">Тип переменной</typeparam>
        /// <param name="selector">Метод выбора узлов</param>
        /// <returns>Перечисление узлов дерева с указанным типом переменных</returns>
        [NotNull]
        public IEnumerable<VariableValueNode> GetTreeNodesOf<TVariable>([NotNull] Func<VariableValueNode, bool> selector)
            where TVariable : ExpressionVariabel
        {
            Contract.Requires(selector != null);
            Contract.Ensures(Contract.Result<IEnumerable<VariableValueNode>>() != null);
            return _Expression.Tree
                .Where(n => n is VariableValueNode)
                .Cast<VariableValueNode>()
                .Where(n => n.Variable is TVariable)
                .Where(selector);
        }

        /// <summary>Возвращает перечислитель, выполняющий перебор элементов в коллекции</summary>
        /// <returns>
        /// Интерфейс <see cref="T:System.Collections.Generic.IEnumerator`1"/>, который может использоваться для перебора элементов коллекции.
        /// </returns>
        [NotNull]
        IEnumerator<ExpressionVariabel> IEnumerable<ExpressionVariabel>.GetEnumerator() => _Variabels.GetEnumerator();

        /// <summary>Возвращает перечислитель, который осуществляет перебор элементов коллекции</summary>
        /// <returns>
        /// Объект <see cref="T:System.Collections.IEnumerator"/>, который может использоваться для перебора элементов коллекции.
        /// </returns>
        [NotNull]
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Variabels).GetEnumerator();
    }
}