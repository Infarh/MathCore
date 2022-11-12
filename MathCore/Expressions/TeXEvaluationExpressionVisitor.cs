﻿using System.Collections.Generic;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable VirtualMemberCallInConstructor

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions;

/// <summary>Класс "посетителя" для "подстановки" актуальных значений в дерево выражения</summary>
public class TeXEvaluationExpressionVisitor : ExpressionVisitor
{
    // Вспомогательный класс для хранения значения и типа свойств
    private sealed class TypeValuePair
    {
        public object Value { get; init; }
        public Type Type { get; init; }
    }

    // Словарь для хранения значения и типа свойств по имени свойства
    private readonly Dictionary<string, TypeValuePair> _MemberProperties;

    // Конструктор принимает выражение и объект, значения свойств которого будут подставлены
    // в заданное выражение
    public TeXEvaluationExpressionVisitor(Expression expression, [NotNull] object MemberObject)
    {
        // Получаю все свойства переданного объекта
        var member_props = MemberObject.GetType().GetProperties();

        // И получаю ассоциативный массив типа свойства и значения по имени свойства
        _MemberProperties = member_props.ToDictionary(pi => pi.Name,
            pi => new TypeValuePair
            {
                Value = pi.GetValue(MemberObject, null),
                Type  = pi.PropertyType
            });

        ConvertedExpression = Visit(expression);
    }

    // "Обновленное" выражение с "подставленными" значениями свойств
    public Expression ConvertedExpression { get; }

    // Заменяем обращение к члену на соответствующее значение
    protected override Expression VisitMember(MemberExpression MemberExpression)
    {
        // Пробуем найти значение члена с указанным именем
        if(_MemberProperties.TryGetValue(MemberExpression.Member.Name, out var type_value_pair))
            // И заменяем его на соответствующее константное выражение
            return Expression.Constant(value: type_value_pair.Value, type: type_value_pair.Type);
        return MemberExpression;
    }
}