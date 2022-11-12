// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions;

internal class ReplaceVisitor : ExpressionVisitorEx
{
    private readonly Expression _From;
    private readonly Expression _To;

    public ReplaceVisitor(Expression from, Expression to)
    {
        _From = from;
        _To   = to;
    }

    public override Expression Visit(Expression node) => node == _From ? _To : base.Visit(node);
}