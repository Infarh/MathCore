using System.Linq.Expressions;

using static System.Linq.Expressions.Expression;

using bEx = System.Linq.Expressions.BinaryExpression;
using Ex = System.Linq.Expressions.Expression;
using lEx = System.Linq.Expressions.LambdaExpression;
using mcEx = System.Linq.Expressions.MethodCallExpression;
using pEx = System.Linq.Expressions.ParameterExpression;
using uEx = System.Linq.Expressions.UnaryExpression;
// ReSharper disable MergeCastWithTypeCheck
// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable ConvertIfStatementToSwitchStatement
// ReSharper disable InvertIf

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace MathCore.Extensions.Expressions;

public static partial class ExpressionExtensions
{
    private sealed class SubstExpressionVisitor : ExpressionVisitorEx
    {
        #region Properties

        public pEx ParamExpressionToSubstitute { private get; init; } = null!;

        public lEx SubstExpression { private get; init; } = null!;

        #endregion

        #region Methods

        public new Ex? Visit(Ex exp) => base.Visit(exp);

        protected override Ex VisitUnary(uEx node)
            => node.Operand == ParamExpressionToSubstitute
                ? Ex.MakeUnary(node.NodeType, SubstExpression.Body, node.Type)
                : base.VisitUnary(node);

        protected override Ex VisitMethodCall(mcEx node)
        {
            if (!node.Arguments.Any(expr => expr == ParamExpressionToSubstitute))
                return base.VisitMethodCall(node);

            var arguments = node.Arguments
               .Select(arg => arg == ParamExpressionToSubstitute ? SubstExpression.Body : Visit(arg)!);

            return Call(node.Object, node.Method, [.. arguments!]);
        }

        protected override Ex VisitBinary(bEx node)
        {
            Ex left, right;
            var subst_left = false;
            var subst_right = false;

            if (node.Left != ParamExpressionToSubstitute)
                left = node.Left;
            else
            {
                left = SubstExpression.Body;
                subst_left = true;
            }

            if (node.Right != ParamExpressionToSubstitute)
                right = node.Right;
            else
            {
                right = SubstExpression.Body;
                subst_right = true;
            }

            if (!subst_left && !subst_right)
                return base.VisitBinary(node);

            if (!subst_left)
                left = Visit(left)!;
            if (!subst_right)
                right = Visit(right)!;

            return MakeBinary(node.NodeType, left, right, node.IsLiftedToNull, node.Method);
        }

        #endregion
    }
}