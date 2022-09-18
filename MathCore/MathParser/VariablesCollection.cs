#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MathCore.MathParser.ExpressionTrees.Nodes;

using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace MathCore.MathParser;

/// <summary>Коллекция переменных</summary>
[DebuggerDisplay("Variables count = {" + nameof(Count) + "}")]
public class VariablesCollection : IEnumerable<ExpressionVariable>
{
    /// <summary>Математическое выражение</summary>
    private readonly MathExpression _Expression;

    private readonly List<ExpressionVariable> _Variables = new();

    /// <summary>Количество переменных в коллекции</summary>
    public int Count => _Variables.Count;

    /// <summary>Итератор переменных коллекции</summary>
    /// <param name="Name">Имя переменной</param>
    /// <returns>Переменная с указанным именем</returns>
    public ExpressionVariable this[string Name]
    {
        [DST]
        get
        {
            if (Name is null) throw new ArgumentNullException(nameof(Name));
            if (string.IsNullOrEmpty(Name)) throw new ArgumentOutOfRangeException(nameof(Name));
            return (_Variables.Find(v => v.Name == Name)
                    ?? new ExpressionVariable(Name).InitializeObject(this, (v, vc) => vc!.Add(v)))
                ?? throw new InvalidOperationException();
        }
        [DST]
        set
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (Name is null) throw new ArgumentNullException(nameof(Name));
            if (string.IsNullOrEmpty(Name)) throw new ArgumentOutOfRangeException(nameof(Name));
            var old_var = _Variables.Find(v => v.Name == Name);

            if (value is LambdaExpressionVariable or EventExpressionVariable)
            {
                value.Name = Name;
                //Обойти все узлы дерева являющиеся узлами переменных у которых имя соответствует заданному
                foreach (var node in _Expression.VariableNodes.Where(n => n.Variable.Name == Name))
                    node.Variable = value; // и для каждого узла заменить переменную на указанную

                //_Expression.Tree 
                //    .OfType<VariableValueNode>()
                //    .Where(node => node.Variable.Name == Name)
                //    .Foreach(value, (node, v) => node.Variable = v);

                _Variables.Remove(old_var);
                Add(value);
            }
            else if (old_var is null)
                Add(value);
            else
                old_var.Value = value.GetValue();
        }

    }

    /// <summary>Итератор переменных коллекции</summary>
    /// <param name="i">Индекс переменной</param>
    /// <returns>Переменная с указанным индексом</returns>
    public ExpressionVariable this[int i] => _Variables[i];

    /// <summary>Перечисление всех имён переменных коллекции</summary>
    public IEnumerable<string> Names => _Variables.Select(v => v.Name);

    /// <summary>Инициализация новой коллекции переменных</summary>
    /// <param name="expression">Математическое выражение, которому принадлежит коллекция</param>
    public VariablesCollection(MathExpression expression) => _Expression = expression;

    /// <summary>Добавить переменную в коллекцию</summary>
    /// <param name="Variable">Переменная</param>
    /// <returns>Истина, если переменная была добавлена</returns>
    public bool Add(ExpressionVariable Variable)
    {
        var variable = _Variables.Find(v => v.Name == Variable.Name);
        if (variable != null) return false;
        Variable.IsConstant = false;
        _Variables.Add(Variable);
        return true;
    }

    public bool Replace(string Name, ExpressionVariable variable)
    {
        var replaced = false;
        var old_var  = _Variables.Find(v => v.Name == Name);
        if (old_var is null) return false;

        //Обойти все узлы дерева являющиеся узлами переменных у которых имя соответствует заданному
        foreach (var node in _Expression.VariableNodes.Where(n => n.Variable.Name == Name))
        {
            node.Variable = variable; // и для каждого узла заменить переменную на указанную
            replaced      = true;
        }

        //_Expression.Tree                                //Обойти все узлы дерева
        //    .OfType<VariableValueNode>()                // являющиеся узлами переменных
        //    .Where(node => node.Variable.Name == Name)  // у которых имя соответствует заданному
        //    .Foreach(node =>                            // и для каждого узла заменить переменную на указанную
        //    {
        //        node.Variable = variable;
        //        replaced = true;
        //    });

        _Variables.Remove(old_var);
        Add(variable);
        if (replaced)
            variable.Name = Name;
        return replaced;
    }

    /// <summary>Переместить переменную из коллекции переменных в коллекцию констант</summary>
    /// <param name="Variable">Перемещаемая переменная</param>
    /// <returns>Истина, если переменная была перемещена из коллекции переменных в коллекцию констант</returns>
    public bool MoveToConstCollection(string Variable) => MoveToConstCollection(this[Variable]);

    /// <summary>Переместить переменную из коллекции переменных в коллекцию констант</summary>
    /// <param name="Variable">Перемещаемая переменная</param>
    /// <returns>Истина, если переменная была перемещена из коллекции переменных в коллекцию констант</returns>
    public bool MoveToConstCollection(ExpressionVariable Variable) =>
        Exist(v => ReferenceEquals(v, Variable))
        && _Variables.Remove(Variable) && _Expression.Constants.Add(Variable);

    /// <summary>Удаление переменной из коллекции</summary>
    /// <param name="Variable">Удаляемая переменная</param>
    /// <returns>Истина, если удаление прошло успешно</returns>
    public bool Remove(ExpressionVariable Variable) =>
        !_Expression.VariableNodes.Any(n => ReferenceEquals(n.Variable, Variable))
        && _Variables.Remove(Variable);

    /// <summary>Удалить переменную из коллекции</summary>
    /// <param name="Variable">Удаляемая переменная</param>
    /// <returns>Истина, если переменная удалена успешно</returns>
    public bool RemoveFromCollection(ExpressionVariable Variable) => _Variables.Remove(Variable);

    /// <summary>Очистить коллекцию переменных</summary>
    public void ClearCollection() => _Variables.Clear();

    /// <summary>Существует ли в коллекции переменная с указанным именем</summary>
    /// <param name="Name">Искомое имя переменной</param>
    /// <returns>Истина, если в коллекции присутствует переменная с указанным именем</returns>
    public bool Exist(string Name) => Exist(v => v.Name == Name);

    /// <summary>Проверка на существование переменной в коллекции</summary>
    /// <param name="variable">Проверяемая переменная</param>
    /// <returns>Истина, если указанная переменная входит в коллекцию</returns>
    public bool Exist(ExpressionVariable variable) => _Variables.Contains(variable);

    /// <summary>Существует ли переменная в коллекции с заданным критерием поиска</summary>
    /// <param name="exist">Критерий поиска переменной</param>
    /// <returns>Истина, если найдена переменная по указанному критерию</returns>
    public bool Exist(Predicate<ExpressionVariable> exist) => _Variables.Exists(exist);

    /// <summary>Существует ли узел переменной в дереве с указанным именем</summary>
    /// <param name="Name">Искомое имя переменной</param>
    /// <returns>Истина, если указанное имя переменной существует в дереве</returns>
    public bool ExistInTree(string Name) => ExistInTree(v => v.Name == Name);

    /// <summary>Существует ли узел переменной в дереве</summary>
    /// <param name="exist">Критерий поиска</param>
    /// <returns>Истина, если найден узел по указанному критерию</returns>
    public bool ExistInTree(Func<VariableValueNode, bool> exist) =>
        _Expression.VariableNodes.Any(exist);

    /// <summary>Получить перечисление узлов переменных с указанным именем</summary>
    /// <param name="VariableName">Искомое имя переменной</param>
    /// <returns>Перечисление узлов с переменными с указанным именем</returns>
    public IEnumerable<VariableValueNode> GetTreeNodes(string VariableName) => GetTreeNodes(v => v.Name == VariableName);

    /// <summary>Получить перечисление узлов дерева с переменными</summary>
    /// <param name="selector">Метод выборки узлов</param>
    /// <returns>Перечисление узлов переменных</returns>
    public IEnumerable<VariableValueNode> GetTreeNodes(Func<VariableValueNode, bool> selector) =>
        _Expression.VariableNodes.Where(selector);

    /// <summary>Получить перечисление узлов дерева выражения, содержащих указанный тип переменных</summary>
    /// <typeparam name="TVariable">Тип переменной</typeparam>
    /// <returns>Перечисление узлов дерева с указанным типом переменных</returns>
    public IEnumerable<VariableValueNode> GetTreeNodesOf<TVariable>() where TVariable : ExpressionVariable => 
        _Expression.VariableNodes.Where(n => n.Variable is TVariable);

    /// <summary>Получить перечисление узлов дерева выражения, содержащих указанный тип переменных</summary>
    /// <typeparam name="TVariable">Тип переменной</typeparam>
    /// <param name="selector">Метод выбора узлов по содержащимся в них переменным</param>
    /// <returns>Перечисление узлов дерева с указанным типом переменных</returns>
    public IEnumerable<VariableValueNode> GetTreeNodesVOf<TVariable>(Func<TVariable, bool> selector)
        where TVariable : ExpressionVariable =>
        _Expression.VariableNodes.Where(n => n.Variable is TVariable variable && selector(variable));

    /// <summary>Получить перечисление узлов дерева выражения, содержащих указанный тип переменных</summary>
    /// <typeparam name="TVariable">Тип переменной</typeparam>
    /// <param name="selector">Метод выбора узлов</param>
    /// <returns>Перечисление узлов дерева с указанным типом переменных</returns>
    public IEnumerable<VariableValueNode> GetTreeNodesOf<TVariable>(Func<VariableValueNode, bool> selector)
        where TVariable : ExpressionVariable =>
        _Expression.VariableNodes.Where(n => n.Variable is TVariable && selector(n));

    /// <summary>Возвращает перечислитель, выполняющий перебор элементов в коллекции</summary>
    /// <returns>
    /// Интерфейс <see cref="T:System.Collections.Generic.IEnumerator`1"/>, который может использоваться для перебора элементов коллекции.
    /// </returns>
    IEnumerator<ExpressionVariable> IEnumerable<ExpressionVariable>.GetEnumerator() => _Variables.GetEnumerator();

    /// <summary>Возвращает перечислитель, который осуществляет перебор элементов коллекции</summary>
    /// <returns>
    /// Объект <see cref="T:System.Collections.IEnumerator"/>, который может использоваться для перебора элементов коллекции.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Variables).GetEnumerator();
}