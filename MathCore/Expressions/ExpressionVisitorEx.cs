using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions;

/// <summary>Посетитель деревьев выражений</summary>
public abstract class ExpressionVisitorEx
{
    /// <summary>Посещает узел выражения</summary>
    /// <param name="Node">Посещаемый узел</param>
    /// <returns>Посещённое выражение</returns>
    [return: NotNullIfNotNull(nameof(Node))]
    public virtual Expression? Visit(Expression? Node) =>
        Node is null
            ? null
            : Node.NodeType switch
            {
                ExpressionType.Negate => VisitUnary((UnaryExpression)Node),
                ExpressionType.NegateChecked => VisitUnary((UnaryExpression)Node),
                ExpressionType.Not => VisitUnary((UnaryExpression)Node),
                ExpressionType.Convert => VisitUnary((UnaryExpression)Node),
                ExpressionType.ConvertChecked => VisitUnary((UnaryExpression)Node),
                ExpressionType.ArrayLength => VisitUnary((UnaryExpression)Node),
                ExpressionType.Quote => VisitUnary((UnaryExpression)Node),
                ExpressionType.TypeAs => VisitUnary((UnaryExpression)Node),
                ExpressionType.Add => VisitBinary((BinaryExpression)Node),
                ExpressionType.AddChecked => VisitBinary((BinaryExpression)Node),
                ExpressionType.Subtract => VisitBinary((BinaryExpression)Node),
                ExpressionType.SubtractChecked => VisitBinary((BinaryExpression)Node),
                ExpressionType.Multiply => VisitBinary((BinaryExpression)Node),
                ExpressionType.MultiplyChecked => VisitBinary((BinaryExpression)Node),
                ExpressionType.Divide => VisitBinary((BinaryExpression)Node),
                ExpressionType.Modulo => VisitBinary((BinaryExpression)Node),
                ExpressionType.And => VisitBinary((BinaryExpression)Node),
                ExpressionType.AndAlso => VisitBinary((BinaryExpression)Node),
                ExpressionType.Or => VisitBinary((BinaryExpression)Node),
                ExpressionType.OrElse => VisitBinary((BinaryExpression)Node),
                ExpressionType.LessThan => VisitBinary((BinaryExpression)Node),
                ExpressionType.LessThanOrEqual => VisitBinary((BinaryExpression)Node),
                ExpressionType.GreaterThan => VisitBinary((BinaryExpression)Node),
                ExpressionType.GreaterThanOrEqual => VisitBinary((BinaryExpression)Node),
                ExpressionType.Equal => VisitBinary((BinaryExpression)Node),
                ExpressionType.NotEqual => VisitBinary((BinaryExpression)Node),
                ExpressionType.Coalesce => VisitBinary((BinaryExpression)Node),
                ExpressionType.ArrayIndex => VisitBinary((BinaryExpression)Node),
                ExpressionType.RightShift => VisitBinary((BinaryExpression)Node),
                ExpressionType.LeftShift => VisitBinary((BinaryExpression)Node),
                ExpressionType.ExclusiveOr => VisitBinary((BinaryExpression)Node),
                ExpressionType.Power => VisitBinary((BinaryExpression)Node),
                ExpressionType.TypeIs => VisitTypeIs((TypeBinaryExpression)Node),
                ExpressionType.Conditional => VisitConditional((ConditionalExpression)Node),
                ExpressionType.Constant => VisitConstant((ConstantExpression)Node),
                ExpressionType.Parameter => VisitParameter((ParameterExpression)Node),
                ExpressionType.MemberAccess => VisitMemberAccess((MemberExpression)Node),
                ExpressionType.Call => VisitMethodCall((MethodCallExpression)Node),
                ExpressionType.Lambda => VisitLambda((LambdaExpression)Node),
                ExpressionType.New => VisitNew((NewExpression)Node),
                ExpressionType.NewArrayInit => VisitNewArray((NewArrayExpression)Node),
                ExpressionType.NewArrayBounds => VisitNewArray((NewArrayExpression)Node),
                ExpressionType.Invoke => VisitInvocation((InvocationExpression)Node),
                ExpressionType.MemberInit => VisitMemberInit((MemberInitExpression)Node),
                ExpressionType.ListInit => VisitListInit((ListInitExpression)Node),
                //ExpressionType.AddAssign => expr,
                //ExpressionType.AddAssignChecked => expr,
                //ExpressionType.AndAssign => expr,
                //ExpressionType.Assign => expr,
                //ExpressionType.Block => expr,
                //ExpressionType.DebugInfo => expr,
                //ExpressionType.Decrement => expr,
                //ExpressionType.Default => expr,
                //ExpressionType.DivideAssign => expr,
                //ExpressionType.Dynamic => expr,
                //ExpressionType.ExclusiveOrAssign => expr,
                //ExpressionType.Extension => expr,
                //ExpressionType.Goto => expr,
                //ExpressionType.Increment => expr,
                //ExpressionType.Index => expr,
                //ExpressionType.IsFalse => expr,
                //ExpressionType.IsTrue => expr,
                //ExpressionType.Label => expr,
                //ExpressionType.LeftShiftAssign => expr,
                //ExpressionType.Loop => expr,
                //ExpressionType.ModuloAssign => expr,
                //ExpressionType.MultiplyAssign => expr,
                //ExpressionType.MultiplyAssignChecked => expr,
                //ExpressionType.OnesComplement => expr,
                //ExpressionType.OrAssign => expr,
                //ExpressionType.PostDecrementAssign => expr,
                //ExpressionType.PostIncrementAssign => expr,
                //ExpressionType.PowerAssign => expr,
                //ExpressionType.PreDecrementAssign => expr,
                //ExpressionType.PreIncrementAssign => expr,
                //ExpressionType.RightShiftAssign => expr,
                //ExpressionType.RuntimeVariables => expr,
                //ExpressionType.SubtractAssign => expr,
                //ExpressionType.SubtractAssignChecked => expr,
                //ExpressionType.Switch => expr,
                //ExpressionType.Throw => expr,
                //ExpressionType.Try => expr,
                //ExpressionType.TypeEqual => expr,
                //ExpressionType.UnaryPlus => expr,
                //ExpressionType.Unbox => expr,
                _ => throw new($"Unhandled expression type: '{Node.NodeType}'")
            };

    /// <summary>Посещает привязку члена</summary>
    /// <param name="binding">Привязка</param>
    /// <returns>Привязка</returns>
    protected virtual MemberBinding VisitBinding(MemberBinding binding) =>
        binding.BindingType switch
        {
            MemberBindingType.Assignment => VisitMemberAssignment((MemberAssignment)binding),
            MemberBindingType.MemberBinding => VisitMemberMemberBinding((MemberMemberBinding)binding),
            MemberBindingType.ListBinding => VisitMemberListBinding((MemberListBinding)binding),
            _ => throw new($"Unhandled binding type '{binding.BindingType}'")
        };

    /// <summary>Посещает инициализатор элемента</summary>
    /// <param name="initializer">Инициализатор</param>
    /// <returns>Инициализатор</returns>
    protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
    {
        var arguments = VisitExpressionList(initializer.Arguments);
        return arguments != initializer.Arguments ? Expression.ElementInit(initializer.AddMethod, arguments) : initializer;
    }

    /// <summary>Посещает унарное выражение</summary>
    /// <param name="u">Унарное выражение</param>
    /// <returns>Выражение</returns>
    protected virtual Expression VisitUnary(UnaryExpression u)
    {
        var operand = Visit(u.Operand);
        return operand != u.Operand ? Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method) : u;
    }

    /// <summary>Посещает бинарное выражение</summary>
    /// <param name="b">Бинарное выражение</param>
    /// <returns>Выражение</returns>
    protected virtual Expression VisitBinary(BinaryExpression b)
    {
        var left = Visit(b.Left);
        var right = Visit(b.Right);
        var conversion = Visit(b.Conversion);
        return left != b.Left || right != b.Right || conversion != b.Conversion
            ? b.NodeType == ExpressionType.Coalesce && b.Conversion != null
                ? Expression.Coalesce(left, right, conversion as LambdaExpression)
                : Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method)
            : b;
    }

    /// <summary>Посещает выражение проверки типа</summary>
    /// <param name="b">Выражение проверки типа</param>
    /// <returns>Выражение</returns>
    protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
    {
        var expr = Visit(b.Expression);
        return expr != b.Expression ? Expression.TypeIs(expr, b.TypeOperand) : b;
    }

    /// <summary>Посещает константу</summary>
    /// <param name="c">Константа</param>
    /// <returns>Константа</returns>
    protected virtual Expression VisitConstant(ConstantExpression c) => c;

    /// <summary>Посещает условное выражение</summary>
    /// <param name="c">Условное выражение</param>
    /// <returns>Выражение</returns>
    protected virtual Expression VisitConditional(ConditionalExpression c)
    {
        var test = Visit(c.Test);
        var if_true = Visit(c.IfTrue);
        var if_false = Visit(c.IfFalse);
        return test != c.Test || if_true != c.IfTrue || if_false != c.IfFalse
            ? Expression.Condition(test, if_true, if_false)
            : c;
    }

    /// <summary>Посещает параметр</summary>
    /// <param name="p">Параметр</param>
    /// <returns>Параметр</returns>
    protected virtual Expression VisitParameter(ParameterExpression p) => p;

    /// <summary>Посещает доступ к члену</summary>
    /// <param name="m">Выражение доступа к члену</param>
    /// <returns>Выражение</returns>
    protected virtual Expression VisitMemberAccess(MemberExpression m)
    {
        var exp = Visit(m.Expression);
        return exp != m.Expression ? Expression.MakeMemberAccess(exp, m.Member) : m;
    }

    /// <summary>Посещает вызов метода</summary>
    /// <param name="m">Выражение вызова метода</param>
    /// <returns>Выражение</returns>
    protected virtual Expression VisitMethodCall(MethodCallExpression m)
    {
        var obj = Visit(m.Object);
        IEnumerable<Expression> args = VisitExpressionList(m.Arguments);
        return obj != m.Object || !ReferenceEquals(args, m.Arguments) ? Expression.Call(obj, m.Method, args) : m;
    }

    /// <summary>Посещает список выражений</summary>
    /// <param name="original">Исходный список</param>
    /// <returns>Список выражений</returns>
    protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
    {
        List<Expression>? list = null;
        for (int i = 0, n = original.Count; i < n; i++)
        {
            var p = Visit(original[i]).NotNull();
            if (list != null)
                list.Add(p);
            else if (p != original[i])
            {
                list = new(n);
                for (var j = 0; j < i; j++)
                    list.Add(original[j]);
                list.Add(p);
            }
        }
        return list?.AsReadOnly() ?? original;
    }

    /// <summary>Посещает присваивание члена</summary>
    /// <param name="assignment">Присваивание</param>
    /// <returns>Присваивание</returns>
    protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
    {
        var e = Visit(assignment.Expression);
        return e != assignment.Expression ? Expression.Bind(assignment.Member, e) : assignment;
    }

    /// <summary>Посещает привязку члена-члена</summary>
    /// <param name="binding">Привязка</param>
    /// <returns>Привязка</returns>
    protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
    {
        var bindings = VisitBindingList(binding.Bindings);
        return !Equals(bindings, binding.Bindings) ? Expression.MemberBind(binding.Member, bindings) : binding;
    }

    /// <summary>Посещает привязку списка члена</summary>
    /// <param name="binding">Привязка</param>
    /// <returns>Привязка</returns>
    protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
    {
        var initializers = VisitElementInitializerList(binding.Initializers);
        return !Equals(initializers, binding.Initializers) ? Expression.ListBind(binding.Member, initializers) : binding;
    }

    /// <summary>Посещает список привязок членов</summary>
    /// <param name="original">Исходный список</param>
    /// <returns>Список привязок</returns>
    protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
    {
        List<MemberBinding>? list = null;
        for (int i = 0, n = original.Count; i < n; i++)
        {
            var b = VisitBinding(original[i]);
            if (list != null)
                list.Add(b);
            else if (b != original[i])
            {
                list = new(n);
                for (var j = 0; j < i; j++)
                    list.Add(original[j]);
                list.Add(b);
            }
        }

        return list is null
            ? original
            : list;
    }

    /// <summary>Посещает список инициализаторов элементов</summary>
    /// <param name="original">Исходный список</param>
    /// <returns>Список инициализаторов</returns>
    protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
    {
        List<ElementInit>? list = null;
        for (int i = 0, n = original.Count; i < n; i++)
        {
            var init = VisitElementInitializer(original[i]);
            if (list != null)
                list.Add(init);
            else if (init != original[i])
            {
                list = new(n);
                for (var j = 0; j < i; j++)
                    list.Add(original[j]);
                list.Add(init);
            }
        }
        return list != null ? list : original;
    }

    /// <summary>Посещает лямбда-выражение</summary>
    /// <param name="lambda">Лямбда-выражение</param>
    /// <returns>Выражение</returns>
    protected virtual Expression VisitLambda(LambdaExpression lambda)
    {
        var body = Visit(lambda.Body);
        return body != lambda.Body
            ? Expression.Lambda(lambda.Type, body, lambda.Parameters)
            : lambda;
    }

    /// <summary>Посещает выражение конструирования</summary>
    /// <param name="nex">Выражение конструирования</param>
    /// <returns>Выражение</returns>
    protected virtual NewExpression VisitNew(NewExpression nex)
    {
        var args = VisitExpressionList(nex.Arguments);
        return args != nex.Arguments
            ? (nex.Members != null
                ? Expression.New(nex.Constructor!, args, nex.Members)
                : Expression.New(nex.Constructor!, args))
            : nex;
    }

    /// <summary>Посещает инициализацию члена</summary>
    /// <param name="init">Выражение инициализации члена</param>
    /// <returns>Выражение</returns>
    protected virtual Expression VisitMemberInit(MemberInitExpression init)
    {
        var n = VisitNew(init.NewExpression);
        var bindings = VisitBindingList(init.Bindings);
        return n != init.NewExpression || !Equals(bindings, init.Bindings) ? Expression.MemberInit(n, bindings) : init;
    }

    /// <summary>Посещает инициализацию списка</summary>
    /// <param name="init">Выражение инициализации списка</param>
    /// <returns>Выражение</returns>
    protected virtual Expression VisitListInit(ListInitExpression init)
    {
        var n = VisitNew(init.NewExpression);
        var initializers = VisitElementInitializerList(init.Initializers);
        return n != init.NewExpression || !Equals(initializers, init.Initializers)
            ? Expression.ListInit(n, initializers)
            : init;
    }

    /// <summary>Посещает выражение нового массива</summary>
    /// <param name="na">Выражение массива</param>
    /// <returns>Выражение</returns>
    protected virtual Expression VisitNewArray(NewArrayExpression na)
    {
        var expr = VisitExpressionList(na.Expressions);
        return expr != na.Expressions
            ? (na.NodeType == ExpressionType.NewArrayInit
                ? Expression.NewArrayInit(na.Type.GetElementType()!, expr)
                : Expression.NewArrayBounds(na.Type.GetElementType()!, expr))
            : na;
    }

    /// <summary>Посещает выражение вызова</summary>
    /// <param name="iv">Выражение вызова</param>
    /// <returns>Выражение</returns>
    protected virtual Expression VisitInvocation(InvocationExpression iv)
    {
        var args = VisitExpressionList(iv.Arguments);
        var expr = Visit(iv.Expression);
        return args != iv.Arguments || expr != iv.Expression ? Expression.Invoke(expr, args) : iv;
    }
}