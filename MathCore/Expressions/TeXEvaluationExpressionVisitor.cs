using System.Collections.Generic;

namespace System.Linq.Expressions
{
    /// <summary> ласс "посетител€" дл€ "подстановки" актуальных значний в дерево выражени€</summary>
    public class TeXEvaluationExpressionVisitor : ExpressionVisitor
    {
        // ¬спомогательный класс дл€ хранени€ значени€ и типа свойств
        private sealed class TypeValuePair
        {
            public object Value { get; set; }
            public Type Type { get; set; }
        }

        // ћапа дл€ хранени€ значени€ и типа свойств по имени свойства
        private readonly Dictionary<string, TypeValuePair> _MemberProperties;

        //  онструктор принимает выражение и объект, значени€ свойств которого будут подставлены
        // в заданное выражение
        public TeXEvaluationExpressionVisitor(Expression expression, object memberObject)
        {
            // ѕолучаю все свойства переданного объекта
            var member_props = memberObject.GetType().GetProperties();

            // » получаю ассоциативный массив типа свойства и значени€ по имени свойства
            _MemberProperties = member_props.ToDictionary(pi => pi.Name,
                pi => new TypeValuePair
                {
                    Value = pi.GetValue(memberObject, null),
                    Type = pi.PropertyType
                });

            ConvertedExpression = Visit(expression);
        }

        // "ќбновленное" выражение с "подставленными" значени€ми свойств
        public Expression ConvertedExpression { get; private set; }

        // «амен€ем обращение к члену на соответствующее значение
        protected override Expression VisitMember(MemberExpression memberExpression)
        {
            // ѕробуем найти значение члена с указанным именем
            if(_MemberProperties.TryGetValue(memberExpression.Member.Name, out var type_value_pair))
                // » замен€ем его на соответствующее константное выражение
                return Expression.Constant(value: type_value_pair.Value, type: type_value_pair.Type);
            return memberExpression;
        }
    }
}