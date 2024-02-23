#nullable enable
using System.Collections;

// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions;

public class LambdaExpressionRebuilder(IEnumerable<LambdaExpressionRebuilder.Rule> NewValues)
    : ExpressionVisitorEx, IEnumerable
{
    public class Rule(Func<Expression, bool> Selector, Func<Expression, Expression> NewNode)
    {
        public Func<Expression, bool> Selector { get; set; } = Selector;
        public Func<Expression, Expression> NewNode { get; set; } = NewNode;
    }

    public class Rule<T>(Func<T, bool> Selector, Func<Expression, Expression> Node)
        : Rule(s => s is T expression && Selector(expression), Node)
        where T : Expression;

    private readonly List<Rule> _NewValues = NewValues.ToList();

    public LambdaExpressionRebuilder(params Rule[] NewValues) : this((IEnumerable<Rule>)NewValues) { }

    public override Expression? Visit(Expression? node) =>
        _NewValues.First(value => value.Selector(node)) is { } v
            ? v.NewNode(node) 
            : null;

    public void Add(Func<Expression, bool> Selector, Func<Expression, Expression> NewNode) => _NewValues.Add(new(Selector, NewNode));

    public IEnumerator GetEnumerator() => _NewValues.GetEnumerator();
}