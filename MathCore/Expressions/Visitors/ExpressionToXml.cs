#nullable enable
using System;
using System.Linq.Expressions;
using System.Text;

namespace MathCore.Expressions.Visitors;

public sealed class ExpressionToXml : ExpressionVisitor
{
    //----------------------------------------------------------------------------------------//
    // Закрытые поля
    //----------------------------------------------------------------------------------------//
    // Сюда мы будет сохранять "пройденные" части выражения
    private readonly StringBuilder _Result = new();

    //----------------------------------------------------------------------------------------//
    // Конструкторы
    //----------------------------------------------------------------------------------------//

    // Конструктор принимает выражение, которое будет преобразовано в формат TeX-а
    public ExpressionToXml(Expression expression) => Visit(expression);

    // Лямбда-выражение анализируется несколько по-иному, поскольку нам нужно только тело
    // выражения, без первого параметра
    public ExpressionToXml(LambdaExpression expression) => Visit(expression.Body);


    //----------------------------------------------------------------------------------------//
    // Открытые интерфейс
    //----------------------------------------------------------------------------------------//
    // Изменяем сгенерированную строку в зависимости от типа знака "умножения"
    public string GenerateTeXExpression(string ExpressionName, MultiplicationSign MultiplicationSign = MultiplicationSign.Asterisk) =>
        GenerateTeXExpressionImpl(ExpressionName, MultiplicationSign);

    public string GenerateTeXExpression(MultiplicationSign MultiplicationSign = MultiplicationSign.Asterisk) =>
        GenerateTeXExpressionImpl(null, MultiplicationSign);


    //----------------------------------------------------------------------------------------//
    // Переопределенные виртуальные методы базового класса ExpressionVisitor
    //----------------------------------------------------------------------------------------//
    // Методы, переопределенные от класса ExpressionVisitor
    protected override Expression VisitUnary(UnaryExpression node)
    {
        if (node.NodeType == ExpressionType.Negate)
            _Result.Append('-');
        return base.VisitUnary(node);
    }

    protected override Expression VisitBinary(BinaryExpression node) => IsInfix(node.NodeType) ? VisitInfixBinary(node) : VisitPrefixBinary(node);

    protected override Expression VisitMember(MemberExpression node)
    {
        var full_name   = node.Member.Name;
        var point_index = full_name.LastIndexOf('.');
        if (point_index != -1) return node;

        var name    = full_name[(point_index + 1)..];
        _Result.Append(name);
        return node;
    }

    public override Expression? Visit(Expression node)
    {
        if (node is not ConstantExpression constant) return base.Visit(node);
        _Result.Append(constant.Value);
        return Expression.Constant(constant.Value, constant.Type);
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        var full_name   = node.Name;
        var point_index = full_name.LastIndexOf('.');
        if (point_index != -1) return node;

        var name = full_name[(point_index + 1)..];
        _Result.Append(name);
        return node;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        // Это очень простая реализация поиска определенных методов.
        // Для промышленного кода мы можем добавить кеширование или что-то
        // подобное, здесь же этого совершенно достаточно

        if (node.Method is not { DeclaringType.FullName: "System.Math", Name: "Pow" })
            return base.Visit(node) ?? throw new InvalidOperationException();

        Visit(node.Arguments[0]);
        _Result.AppendFormat("^{{{0}}}", node.Arguments[1]);
        return node;
    }

    //----------------------------------------------------------------------------------------//
    // Вспомогательные закрытые методы
    //----------------------------------------------------------------------------------------//

    // Метод возвращает true, если выражение нужно оборачивать в скобки: ()
    private static bool RequiresPrecedence(ExpressionType NodeType) => NodeType switch
    {
        ExpressionType.Add => true,
        ExpressionType.Subtract => true,
        _ => false
    };

    // Оператор деления несколько отличается от всех остальных операторов с двумя аргументами,
    // поскольку он требует другого порядка аргументов
    private static bool IsInfix(ExpressionType NodeType) => NodeType != ExpressionType.Divide;

    // Большинство операторов требуют аргументы в следующем порядке:
    // {arg1} op {arg2}
    private Expression VisitInfixBinary(BinaryExpression node)
    {
        var requires_precedence = RequiresPrecedence(node.NodeType);
        if (requires_precedence) _Result.Append("(");

        Visit(node.Left);

        _Result.Append(node.NodeType switch
        {
            ExpressionType.Multiply => "*",
            ExpressionType.Add      => "+",
            ExpressionType.Subtract => "-",
            _                       => throw new NotSupportedException($"The binary operator '{node.NodeType}' is not supported")
        });

        Visit(node.Right);

        if (requires_precedence) _Result.Append(")");
        return node;
    }


    // ReSharper disable once CommentTypo
    /// <summary>
    /// Оператор деления \fract требует иного порядка аргументов:
    /// \frac{arg1}{arg2} 
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private Expression VisitPrefixBinary(BinaryExpression node)
    {
        // Для деления (x + 2) на 3, мы должны получить следующее выражение
        // \frac{x + 2}{3}
        switch (node.NodeType)
        {
            case ExpressionType.Divide:
                _Result.Append(@"\frac");
                break;
            default:
                throw new InvalidOperationException($"Unknown prefix BinaryExpression {node.Type}");
        }

        _Result.Append('{');
        Visit(node.Left);
        _Result.Append('}');

        _Result.Append('{');
        Visit(node.Right);
        _Result.Append('}');
        return node;
    }

    // Метод, реализующий получение строкового представления полученного выражения
    private string GenerateTeXExpressionImpl(string? ExpressionName, MultiplicationSign Sign)
    {
        switch (Sign)
        {
            case MultiplicationSign.Times:
                // ReSharper disable once StringLiteralTypo
                _Result.Replace("*", @" \times ");
                break;
            case MultiplicationSign.None:
                _Result.Replace("*", string.Empty);
                break;
        }

        var tex_expression = _Result.ToString();

        // Полученное выражение содержит избыточные круглые скобки, которые следует убрать
        if (tex_expression.Length > 0)
            if (tex_expression[0] == '(' && tex_expression[^1] == ')')
                tex_expression = tex_expression[1..^1];

        // Добавляем "имя выражения" если оно указано
        return !string.IsNullOrEmpty(ExpressionName)
            ? $"{{{ExpressionName}}}{{=}}{tex_expression}"
            : tex_expression;
    }
}