namespace MathCore.Expressions.Complex
{
    public abstract class ComplexBinaryExpression : ComplexExpression
    {
        public ComplexExpression Left { get; private set; }
        public ComplexExpression Right { get; private set; }

        protected ComplexBinaryExpression(ComplexExpression Left, ComplexExpression Right)
        {
            this.Left = Left;
            this.Right = Right;
        }
    }
}