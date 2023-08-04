#nullable enable
using System.Linq.Expressions;

// ReSharper disable ConvertToAutoPropertyWhenPossible

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.MathParser.ExpressionTrees.Nodes;

/// <summary>Функциональный узел</summary>
public class FunctionalNode : ComputedNode
{
    /// <summary>Имя узла</summary>
    public string Name { get; }

    /// <summary>Выражение параметров</summary>
    private readonly MathExpression _ParametersExpression = new("Param");

    /// <summary>Выражение ядра функции</summary>
    private readonly MathExpression _CoreExpression = new(nameof(Core));

    /// <summary>Выражение параметров</summary>
    public MathExpression Parameters => _ParametersExpression;

    /// <summary>Выражение ядра функции</summary>
    public MathExpression Core => _CoreExpression;

    /// <summary>Оператор</summary>
    public Functional Operator { get; set; } = null!;

    /// <summary>Инициализация нового экземпляра <see cref="FunctionNode"/></summary>
    /// <param name="Name">Имя функционала</param>
    [DST]
    public FunctionalNode(string Name) => this.Name = Name;

    /// <summary>Инициализация нового экземпляра <see cref="FunctionNode"/></summary>
    /// <param name="term">Блок определения функционала</param>
    /// <param name="Parser">Парсер выражения, конструирующий дерево</param>
    /// <param name="Expression">Выражение, в которое будет включён создаваемый узел</param>
    internal FunctionalNode(FunctionalTerm term, ExpressionParser Parser, MathExpression Expression)
        : this(term.Name)
    {
        // Расшифровка блока ядра функции
        _CoreExpression.Tree = new ExpressionTree(term.Block.GetSubTree(Parser, _CoreExpression));
        // Расшифровка блока параметров
        _ParametersExpression.Tree = new ExpressionTree(term.Parameters.GetSubTree(Parser, _ParametersExpression));

        Parser.ProcessVariables(_CoreExpression);
        Parser.ProcessVariables(_ParametersExpression);

        Parser.ProcessFunctions(_CoreExpression);
        Parser.ProcessFunctions(_ParametersExpression);

        // Добавление переменных в перечень блока параметров
        _ParametersExpression.Tree
           .Where(n => n is VariableValueNode) // проходим по всем узлам с переменными
           .Cast<VariableValueNode>()
           .Where(v => !v.Variable.IsConstant)
           .Foreach(_ParametersExpression.Variable, _CoreExpression.Variable, 
                (v, expr_vars, core_vars) =>
                {
                    expr_vars.RemoveFromCollection(v.Variable);
                    expr_vars.Add(v.Variable = core_vars[v.Variable.Name]);
                });

        //Запрос к парсеру о операторе
        Operator = ExpressionParser.GetFunctional(term.Name);
        //Инициализация оператора
        Operator.Initialize(_ParametersExpression, _CoreExpression, Parser, Expression);
    }

    /// <inheritdoc />
    public override IEnumerable<ExpressionVariable> GetVariables()
    {
        ExpressionVariable? iterator    = null;
        var                params_node = _ParametersExpression.Tree.Root;
        if(params_node is EqualityOperatorNode && params_node.Left is VariableValueNode node)
            iterator = node.Variable;
        return _CoreExpression.Tree.Root.GetVariables().Where(v => v != iterator);
    }

    /// <summary>Вычислить значение поддерева</summary>
    /// <returns>Численное значение поддерева</returns>
    [DST]
    public override double Compute() => Operator.GetValue(_ParametersExpression, _CoreExpression);

    /// <summary>Скомпилировать в выражение</summary>
    /// <returns>Скомпилированное выражение System.Linq.Expressions</returns>
    [DST]
    public override Expression Compile() => Operator.Compile(_ParametersExpression, _CoreExpression);

    /// <summary>Скомпилировать в выражение</summary>
    /// <param name="Args">Массив параметров</param>
    /// <returns>Скомпилированное выражение System.Linq.Expressions</returns>
    [DST]
    public override Expression Compile(params ParameterExpression[] Args) => Operator.Compile(_ParametersExpression, _CoreExpression, Args);

    /// <summary>Преобразование узла в строку</summary>
    /// <returns>Строковое представление узла</returns>
    public override string ToString() => $"{Operator.Name}[{_ParametersExpression.Tree.Root}]{{{_CoreExpression.Tree.Root}}}";

    /// <summary>Клонирование поддерева</summary>
    /// <returns>Клон поддерева</returns>
    public override ExpressionTreeNode Clone() => throw new NotImplementedException();

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing) return;
        _ParametersExpression.Dispose();
        _CoreExpression.Dispose();
    }
}