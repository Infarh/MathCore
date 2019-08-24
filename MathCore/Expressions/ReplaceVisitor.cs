namespace System.Linq.Expressions
{
    class ReplaceVisitor : ExpressionVisitorEx
    {
        private readonly Expression from, to;
        public ReplaceVisitor(Expression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }

        public override Expression Visit(Expression node) => node == from ? to : base.Visit(node);
    }
}