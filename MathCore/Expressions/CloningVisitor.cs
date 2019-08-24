// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions
{
    [Diagnostics.DebuggerStepThrough]
    public class CloningVisitor : ExpressionVisitorEx
    {
        #region Overrides of ExpressionVisitorEx

        /// <inheritdoc />
        protected override ElementInit VisitElementInitializer(ElementInit initializer) => Expression.ElementInit(initializer.AddMethod, VisitExpressionList(initializer.Arguments));

        /// <inheritdoc />
        protected override Expression VisitUnary(UnaryExpression u) => Expression.MakeUnary(u.NodeType, Visit(u.Operand), u.Type);

        /// <inheritdoc />
        protected override Expression VisitBinary(BinaryExpression b) => Expression.MakeBinary(b.NodeType, Visit(b.Left), Visit(b.Right));

        /// <inheritdoc />
        protected override Expression VisitTypeIs(TypeBinaryExpression b) => Expression.TypeIs(Visit(b.Expression), b.TypeOperand);

        /// <inheritdoc />
        protected override Expression VisitConstant(ConstantExpression c) => c.Value.ToExpression();

        /// <inheritdoc />
        protected override Expression VisitConditional(ConditionalExpression c) => Expression.Condition(Visit(c.Test), Visit(c.IfTrue), Visit(c.IfFalse));

        /// <inheritdoc />
        protected override Expression VisitParameter(ParameterExpression p) => p.Name.IsNullOrWhiteSpace() ? Expression.Parameter(p.Type) : Expression.Parameter(p.Type, p.Name);

        /// <inheritdoc />
        protected override Expression VisitMemberAccess(MemberExpression m) => Expression.MakeMemberAccess(Visit(m.Expression), m.Member);

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression m) => Expression.Call(Visit(m.Object), m.Method, VisitExpressionList(m.Arguments));

        /// <inheritdoc />
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment) => Expression.Bind(assignment.Member, Visit(assignment.Expression));

        /// <inheritdoc />
        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding) => Expression.MemberBind(binding.Member, VisitBindingList(binding.Bindings));

        /// <inheritdoc />
        protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding) => Expression.ListBind(binding.Member, VisitElementInitializerList(binding.Initializers));

        /// <inheritdoc />
        protected override Expression VisitLambda(LambdaExpression lambda) => Expression.Lambda(lambda.Type, Visit(lambda.Body), lambda.Parameters);

        /// <inheritdoc />
        protected override NewExpression VisitNew(NewExpression nex) => nex.Members != null
            ? Expression.New(nex.Constructor, VisitExpressionList(nex.Arguments), nex.Members)
            : Expression.New(nex.Constructor, VisitExpressionList(nex.Arguments));

        /// <inheritdoc />
        protected override Expression VisitMemberInit(MemberInitExpression init) => Expression.MemberInit(VisitNew(init.NewExpression), VisitBindingList(init.Bindings));

        /// <inheritdoc />
        protected override Expression VisitListInit(ListInitExpression init) => Expression.ListInit(VisitNew(init.NewExpression), VisitElementInitializerList(init.Initializers));

        /// <inheritdoc />
        protected override Expression VisitNewArray(NewArrayExpression na) => na.NodeType == ExpressionType.NewArrayInit
            ? Expression.NewArrayInit(na.Type.GetElementType(), VisitExpressionList(na.Expressions))
            : Expression.NewArrayBounds(na.Type.GetElementType(), VisitExpressionList(na.Expressions));

        /// <inheritdoc />
        protected override Expression VisitInvocation(InvocationExpression iv) => Expression.Invoke(Visit(iv.Expression), VisitExpressionList(iv.Arguments));

        #endregion
    }
}