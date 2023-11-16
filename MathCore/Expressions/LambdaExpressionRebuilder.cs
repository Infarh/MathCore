using System.Collections;

using MathCore.Annotations;

// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions;

public class LambdaExpressionRebuilder : ExpressionVisitorEx, IEnumerable
{
    public class Rule
    {
        public Func<Expression, bool> Selector { get; set; }
        public Func<Expression, Expression> NewNode { get; set; }

        public Rule(Func<Expression, bool> Selector, Func<Expression, Expression> NewNode)
        {
            this.Selector = Selector;
            this.NewNode  = NewNode;
        }
    }

    public class Rule<T> : Rule where T : Expression
    {
        public Rule(Func<T, bool> Selector, Func<Expression, Expression> Node) : base(s => s is T expression && Selector(expression), Node) { }
    }

    private readonly List<Rule> _NewValues;

    public LambdaExpressionRebuilder([NotNull] params Rule[] NewValues) : this((IEnumerable<Rule>)NewValues) { }
    public LambdaExpressionRebuilder([NotNull] IEnumerable<Rule> NewValues) => _NewValues = NewValues.ToList();

    public override Expression Visit(Expression node)
    {
        var V = _NewValues.First(v => v.Selector(node));
        return V is null ? base.Visit(node) : V.NewNode(node);
    }

    public void Add(Func<Expression, bool> Selector, Func<Expression, Expression> NewNode) => _NewValues.Add(new Rule(Selector, NewNode));

    public IEnumerator GetEnumerator() => _NewValues.GetEnumerator();
}