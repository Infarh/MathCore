using System.Collections.Generic;

namespace System.Linq.Expressions
{
    /// <summary>Класс "посетителя" для "подстановки" актуальных значний в дерево выражения</summary>
    public class TeXEvaluationExpressionVisitor : ExpressionVisitor
    {
        // Вспомогательный класс для хранения значения и типа свойств
        private sealed class TypeValuePair
        {
            public object Value { get; set; }
            public Type Type { get; set; }
        }

        // Мапа для хранения значения и типа свойств по имени свойства
        private readonly Dictionary<string, TypeValuePair> _MemberProperties;

        // Конструктор принимает выражение и объект, значения свойств которого будут подставлены
        // в заданное выражение
        public TeXEvaluationExpressionVisitor(Expression expression, object memberObject)
        {
            // Получаю все свойства переданного объекта
            var member_props = memberObject.GetType().GetProperties();

            // И получаю ассоциативный массив типа свойства и значения по имени свойства
            _MemberProperties = member_props.ToDictionary(pi => pi.Name,
                pi => new TypeValuePair
                {
                    Value = pi.GetValue(memberObject, null),
                    Type = pi.PropertyType
                });

            ConvertedExpression = Visit(expression);
        }

        // "Обновленное" выражение с "подставленными" значениями свойств
        public Expression ConvertedExpression { get; private set; }

        // Заменяем обращение к члену на соответствующее значение
        protected override Expression VisitMember(MemberExpression memberExpression)
        {
            // Пробуем найти значение члена с указанным именем
            if(_MemberProperties.TryGetValue(memberExpression.Member.Name, out var type_value_pair))
                // И заменяем его на соответствующее константное выражение
                return Expression.Constant(value: type_value_pair.Value, type: type_value_pair.Type);
            return memberExpression;
        }
    }
}