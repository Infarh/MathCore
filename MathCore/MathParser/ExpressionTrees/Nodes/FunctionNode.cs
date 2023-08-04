#nullable enable
using System.Linq.Expressions;

namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Узел дерева выражения, содержащий функцию</summary>
public class FunctionNode : ComputedNode
{
    /// <summary>Имя функции</summary>
    public string Name { get; } = null!;

    /// <summary>Массив имён аргументов функции</summary>
    public string[] ArgumentsNames => Arguments.Select(a => a.Key).ToArray();

    /// <summary>Перечисление аргументов функции</summary>
    public IEnumerable<KeyValuePair<string, ExpressionTreeNode>> Arguments => GetFunctionArgumentNodes(this);

    /// <summary>Функция узла</summary>
    public ExpressionFunction Function { get; set; } = null!;

    /// <summary>Инициализация нового функционального узла</summary>
    internal FunctionNode() { }

    /// <summary>Инициализация нового функционального узла</summary>
    /// <param name="Name">Имя функции</param>
    internal FunctionNode(string Name) => this.Name = Name;

    /// <summary>Инициализация нового функционального узла</summary>
    /// <param name="Name">Имя функции</param>
    internal FunctionNode(StringNode Name) : this(Name.Value) { }

    /// <summary>Инициализация нового функционального узла</summary>
    /// <param name="Term">Выражение функции</param>
    /// <param name="Parser">Парсер выражения</param>
    /// <param name="Expression">Математическое выражение</param>
    internal FunctionNode(FunctionTerm Term, ExpressionParser Parser, MathExpression Expression)
        : this(Term.Name)
    {

        var arg = Term.Block.GetSubTree(Parser, Expression);
        if (arg is not FunctionArgumentNode)
            arg = arg switch
            {
                FunctionArgumentNameNode name => new FunctionArgumentNode(name),
                VariableValueNode             => new FunctionArgumentNode(null, arg),
                VariantOperatorNode when arg.Left is VariableValueNode => new FunctionArgumentNode(
                    ((VariableValueNode) arg.Left).Name, arg.Right),
                _ => new FunctionArgumentNode(null, arg)
            };
        Right    = arg;
        Function = Expression.Functions[Name, ArgumentsNames];
    }

    /// <summary>Вычисление значения узла</summary>
    /// <returns>Значение функции</returns>
    public override double Compute() => Function.GetValue(Arguments.Select(k => ((ComputedNode)k.Value).Compute()).ToArray());

    /// <summary>Получить перечисление аргументов функции</summary>
    /// <param name="FunctionNode">Узел функции</param>
    /// <returns>Перечисление аргументов функции</returns>
    private static IEnumerable<KeyValuePair<string, ExpressionTreeNode>> GetFunctionArgumentNodes(ExpressionTreeNode FunctionNode) => 
        FunctionNode.Right is FunctionArgumentNode node
            ? FunctionArgumentNode.EnumArguments(node)
            : throw new FormatException();

    /// <summary>Компиляция узла</summary>
    /// <returns>Скомпилированное выражение узла</returns>
    public override Expression Compile() =>
        Expression.Call(Function.Delegate.Target != null ? Expression.Constant(Function.Delegate.Target) : null,
            Function.Delegate.Method,
            Arguments.Select(a => ((ComputedNode)a.Value).Compile()));

    /// <summary>Компиляция узла</summary>
    /// <param name="Args">Список параметров выражения</param>
    /// <returns>Скомпилированное выражение узла</returns>
    public override Expression Compile(params ParameterExpression[] Args) =>
        Expression.Call(Function.Delegate.Target != null ? Expression.Constant(Function.Delegate.Target) : null,
            Function.Delegate.Method,
            Arguments.Select(a => ((ComputedNode)a.Value).Compile(Args)));

    /// <summary>Клонирование узла</summary>
    /// <returns>Клон узла</returns>
    public override ExpressionTreeNode Clone() => new FunctionNode(Name)
    {
        Left     = Left?.Clone(),
        Right    = Right?.Clone(),
        Function = Function.Clone()
    };

    /// <summary>Строковое представление узла</summary>
    /// <returns>Строковое представление узла</returns>
    public override string ToString() => $"{Name}({Arguments.Select(v => string.IsNullOrEmpty(v.Key) ? v.Value.ToString() : $"{v.Key}:{v.Value.ToString()}").ToSeparatedStr(", ")})";
}