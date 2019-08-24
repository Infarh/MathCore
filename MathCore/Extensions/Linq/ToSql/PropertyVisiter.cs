//using System;
//using System.Linq.Expressions;

//namespace System.Linq.ToSQL
//{
//    //http://jslcore.codeplex.com/SourceControl/changeset/view/8699#180670
//    public class PropertyVisiter : ExpressionVisitorEx
//    {
//        protected override Expression VisitMember(MemberExpression node)
//        {
//            var memberInfo = node.Member;
//            var customAttributes = memberInfo.GetCustomAttributes(typeof(ExpressionPropertyAttribute), true);
//            if(customAttributes != null)
//            {
//                if(customAttributes.Length > 0)
//                {
//                    var jPropertyAttribute = (ExpressionPropertyAttribute)customAttributes[0];
//                    var propertyName = jPropertyAttribute.PropertyName;
//                    var subPropertyName = jPropertyAttribute.SubPropertyName;

//                    var returnExpression = Expression.Property(node.Expression, propertyName);

//                    if(subPropertyName != null)
//                    {
//                        returnExpression = Expression.Property(returnExpression, subPropertyName);
//                    }

//                    return Visit(returnExpression);
//                }
//            }

//            return node;
//        }
//    }
//}