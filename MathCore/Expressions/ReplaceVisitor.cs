// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions;

internal class ReplaceVisitor(Expression from, Expression to) : ExpressionVisitorEx
{
    public override Expression Visit(Expression node) => node == from ? to : base.Visit(node);
}