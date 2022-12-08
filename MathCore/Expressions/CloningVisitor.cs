// ReSharper disable once CheckNamespace

using MathCore.Annotations;

namespace System.Linq.Expressions;

[DST]
public class CloningVisitor : ExpressionVisitorEx
{
    #region Overrides of ExpressionVisitorEx

    /// <inheritdoc />
    [NotNull]
    protected override ElementInit VisitElementInitializer([NotNull] ElementInit initializer) => Expression.ElementInit(initializer.AddMethod, VisitExpressionList(initializer.Arguments));

    /// <inheritdoc />
    [NotNull]
    protected override Expression VisitUnary([NotNull] UnaryExpression u) => Expression.MakeUnary(u.NodeType, Visit(u.Operand), u.Type);

    /// <inheritdoc />
    [NotNull]
    protected override Expression VisitBinary([NotNull] BinaryExpression b) => Expression.MakeBinary(b.NodeType, Visit(b.Left), Visit(b.Right));

    /// <inheritdoc />
    [NotNull]
    protected override Expression VisitTypeIs([NotNull] TypeBinaryExpression b) => Expression.TypeIs(Visit(b.Expression), b.TypeOperand);

    /// <inheritdoc />
    [NotNull]
    protected override Expression VisitConstant([NotNull] ConstantExpression c) => c.Value.ToExpression();

    /// <inheritdoc />
    [NotNull]
    protected override Expression VisitConditional([NotNull] ConditionalExpression c) => Expression.Condition(Visit(c.Test), Visit(c.IfTrue), Visit(c.IfFalse));

    /// <inheritdoc />
    [NotNull]
    protected override Expression VisitParameter([NotNull] ParameterExpression p) => p.Name.IsNullOrWhiteSpace() ? Expression.Parameter(p.Type) : Expression.Parameter(p.Type, p.Name);

    /// <inheritdoc />
    [NotNull]
    protected override Expression VisitMemberAccess([NotNull] MemberExpression m) => Expression.MakeMemberAccess(Visit(m.Expression), m.Member);

    /// <inheritdoc />
    [NotNull]
    protected override Expression VisitMethodCall([NotNull] MethodCallExpression m) => Expression.Call(Visit(m.Object), m.Method, VisitExpressionList(m.Arguments));

    /// <inheritdoc />
    [NotNull]
    protected override MemberAssignment VisitMemberAssignment([NotNull] MemberAssignment assignment) => Expression.Bind(assignment.Member, Visit(assignment.Expression));

    /// <inheritdoc />
    [NotNull]
    protected override MemberMemberBinding VisitMemberMemberBinding([NotNull] MemberMemberBinding binding) => Expression.MemberBind(binding.Member, VisitBindingList(binding.Bindings));

    /// <inheritdoc />
    [NotNull]
    protected override MemberListBinding VisitMemberListBinding([NotNull] MemberListBinding binding) => Expression.ListBind(binding.Member, VisitElementInitializerList(binding.Initializers));

    /// <inheritdoc />
    [NotNull]
    protected override Expression VisitLambda([NotNull] LambdaExpression lambda) => Expression.Lambda(lambda.Type, Visit(lambda.Body), lambda.Parameters);

    /// <inheritdoc />
    [NotNull]
    protected override NewExpression VisitNew([NotNull] NewExpression nex) => nex.Members != null
        ? Expression.New(nex.Constructor, VisitExpressionList(nex.Arguments), nex.Members)
        : Expression.New(nex.Constructor, VisitExpressionList(nex.Arguments));

    /// <inheritdoc />
    [NotNull]
    protected override Expression VisitMemberInit([NotNull] MemberInitExpression init) => Expression.MemberInit(VisitNew(init.NewExpression), VisitBindingList(init.Bindings));

    /// <inheritdoc />
    [NotNull]
    protected override Expression VisitListInit([NotNull] ListInitExpression init) => Expression.ListInit(VisitNew(init.NewExpression), VisitElementInitializerList(init.Initializers));

    /// <inheritdoc />
    [NotNull]
    protected override Expression VisitNewArray([NotNull] NewArrayExpression na) => na.NodeType == ExpressionType.NewArrayInit
        ? Expression.NewArrayInit(na.Type.GetElementType(), VisitExpressionList(na.Expressions))
        : Expression.NewArrayBounds(na.Type.GetElementType(), VisitExpressionList(na.Expressions));

    /// <inheritdoc />
    [NotNull]
    protected override Expression VisitInvocation([NotNull] InvocationExpression iv) => Expression.Invoke(Visit(iv.Expression), VisitExpressionList(iv.Arguments));

    #endregion
}