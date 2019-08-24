namespace MathCore.Expressions.Complex
{
    public abstract class ComplexUnaryExpression : ComplexExpression
    {
        public ComplexExpression Value { get; private set; }

        protected ComplexUnaryExpression(ComplexExpression Value) { this.Value = Value; }
    }
}