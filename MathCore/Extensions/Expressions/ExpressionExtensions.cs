#nullable enable
using System.Linq.Expressions;
using System.Reflection;

using MathCore.Vectors;

using static System.Linq.Expressions.Expression;

using bEx = System.Linq.Expressions.BinaryExpression;
using cEx = System.Linq.Expressions.ConstantExpression;
using Ex = System.Linq.Expressions.Expression;
using iEx = System.Linq.Expressions.IndexExpression;
using lEx = System.Linq.Expressions.LambdaExpression;
using mcEx = System.Linq.Expressions.MethodCallExpression;
using mEx = System.Linq.Expressions.MemberExpression;
using pEx = System.Linq.Expressions.ParameterExpression;
using uEx = System.Linq.Expressions.UnaryExpression;
// ReSharper disable MergeCastWithTypeCheck
// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable ConvertIfStatementToSwitchStatement
// ReSharper disable InvertIf

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace MathCore.Extensions.Expressions;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Стиль", "IDE0046:Преобразовать в условное выражение", Justification = "<Ожидание>")]
public static class ExpressionExtensions
{
    #region Types

    private sealed class SubstExpressionVisitor : ExpressionVisitorEx
    {
        #region Properties

        public pEx ParamExpressionToSubstitute { private get; init; } = null!;

        public lEx SubstExpression { private get; init; } = null!;

        #endregion

        #region Methods

        public new Ex Visit(Ex exp) => base.Visit(exp);

        protected override Ex VisitUnary(uEx node)
            => node.Operand == ParamExpressionToSubstitute
                ? Ex.MakeUnary(node.NodeType, SubstExpression.Body, node.Type)
                : base.VisitUnary(node);

        protected override Ex VisitMethodCall(mcEx node)
        {
            if (node.Arguments.Count(expr => expr == ParamExpressionToSubstitute) == 0)
                return base.VisitMethodCall(node);
            var arguments = node.Arguments
               .Select(arg => arg == ParamExpressionToSubstitute ? SubstExpression.Body : Visit(arg))
               .ToList();
            return Call(node.Object, node.Method, arguments);
        }

        protected override Ex VisitBinary(bEx node)
        {
            Ex  left, right;
            var subst_left  = false;
            var subst_right = false;

            if (node.Left == ParamExpressionToSubstitute)
            {
                left       = SubstExpression.Body;
                subst_left = true;
            }
            else
                left = node.Left;

            if (node.Right == ParamExpressionToSubstitute)
            {
                right       = SubstExpression.Body;
                subst_right = true;
            }
            else
                right = node.Right;
            if (!subst_left && !subst_right) return base.VisitBinary(node);
            if (!subst_left)
                left = Visit(left);
            if (!subst_right)
                right = Visit(right);
            return MakeBinary(node.NodeType, left, right, node.IsLiftedToNull, node.Method);
        }

        #endregion
    }

    #endregion

    #region Methods

    /// <exception cref="FormatException">Количество аргументов подстановки не равно 1, или во входном выражении отсутствие подставляемый параметр</exception>
    public static lEx? Substitute(this lEx Expr, lEx Substitution)
    {
        var parameters            = Expr.Parameters;
        var substitute_parameters = Substitution.Parameters;
        if (substitute_parameters.Count != 1)
            throw new FormatException("Количество аргументов подстановки не равно 1");
        var substitute_parameter = substitute_parameters[0];
        if (!parameters.Contains(p => p.Name == substitute_parameter.Name && p.Type == substitute_parameter.Type))
            throw new FormatException("Во входном выражении отсутствие подставляемый параметр");

        var visitor = new SubstitutionVisitor(Substitution);
        var result  = visitor.Visit(Expr);

        return result as lEx;
    }

    public static lEx Substitute<TDelegate>
    (
        this lEx MainEx,
        string ParameterName,
        Expression<TDelegate> SubstExpression
    )
    {
        #region Проверка параметров

        var main_parameter = MainEx.Parameters.FirstOrDefault(p => p.Name == ParameterName);

        if (main_parameter is null)
            throw new Exception($"Could not find input parameter \"{ParameterName}\" in Expression \"{MainEx}\"");

        var substitution_parameter = SubstExpression.Parameters.FirstOrDefault(p => p.Name == ParameterName);

        if (substitution_parameter is null)
            throw new Exception($"Could not find substitution parameter \"{ParameterName}\" in Expression \"{SubstExpression}\"");

        if (substitution_parameter.Type != main_parameter.Type)
            throw new Exception
            (
                $"The substitute Expression return type \"{SubstExpression.Type}\" does not match the type of the substituted variable \"{ParameterName}:{main_parameter.Type}\""
            );

        #endregion

        var pars = MainEx.Parameters.ToList();

        var idx_to_subst = pars.IndexOf(main_parameter);

        pars.RemoveAt(idx_to_subst);

        foreach (var subst_pe in SubstExpression.Parameters)
        {
            if (pars.Count(pe => pe.Name == subst_pe.Name) != 0)
                throw new Exception($"Input parameter of name \"{subst_pe.Name}\" already exists in the main Expression");
            pars.Insert(idx_to_subst, subst_pe);
            idx_to_subst++;
        }

        var visitor =
            new SubstExpressionVisitor
            {
                SubstExpression             = SubstExpression,
                ParamExpressionToSubstitute = main_parameter
            };

        return (lEx)visitor.Visit(Lambda(MainEx.Body, pars));
    }

    public static NewExpression NewExpression(this ConstructorInfo constructor) => New(constructor);

    public static NewExpression NewExpression(this ConstructorInfo constructor, IEnumerable<Ex> arguments) => New(constructor, arguments);

    public static mEx GetProperty(this Ex obj, PropertyInfo Info) => Property(obj, Info);

    public static mEx GetProperty(this Ex obj, string PropertyName) => Property(obj, PropertyName);

    public static mEx GetField(this Ex obj, FieldInfo Info) => Field(obj, Info);
    public static mEx GetField(this Ex obj, string FieldName) => Field(obj, FieldName);

    public static bEx Assign(this Ex dest, Ex source) => Ex.Assign(dest, source);
    public static bEx Assign<T>(this Ex dest, T source) => Ex.Assign(dest, source as Ex ?? source.ToExpression());

    public static bEx AssignTo(this Ex source, Ex dest) => Ex.Assign(dest, source);

    public static uEx Negate(this Ex obj) => Ex.Negate(obj);

    public static bEx AddAssign(this Ex left, Ex right) => Ex.AddAssign(left, right);
    public static bEx AddAssign<T>(this Ex left, T right) => Ex.AddAssign(left, right as Ex ?? right.ToExpression());

    private static bool IsNumeric(this Ex ex) => ex.Type.IsNumeric();

    private static bool IsNumeric(this Type type) =>
        type == typeof(double)
        || type == typeof(float)
        || type == typeof(byte)
        || type == typeof(sbyte)
        || type == typeof(short)
        || type == typeof(ushort)
        || type == typeof(int)
        || type == typeof(uint)
        || type == typeof(long)
        || type == typeof(ulong);

    private static Ex TryConvert(this Ex a, ref Ex b)
    {
        var ta = a.Type;
        var tb = b.Type;

        if (ta == typeof(double)) b      = b.ConvertTo(ta);
        else if (tb == typeof(double)) a = a.ConvertTo(tb);
        else if (ta == typeof(float)) b  = b.ConvertTo(ta);
        else if (tb == typeof(float)) a  = a.ConvertTo(tb);
        else if (ta == typeof(ulong)) b  = b.ConvertTo(ta);
        else if (tb == typeof(ulong)) a  = a.ConvertTo(tb);
        else if (ta == typeof(long)) b   = b.ConvertTo(ta);
        else if (tb == typeof(long)) a   = a.ConvertTo(tb);
        else if (ta == typeof(uint)) b   = b.ConvertTo(ta);
        else if (tb == typeof(uint)) a   = a.ConvertTo(tb);
        else if (ta == typeof(int)) b    = b.ConvertTo(ta);
        else if (tb == typeof(int)) a    = a.ConvertTo(tb);
        else if (ta == typeof(sbyte)) b  = b.ConvertTo(ta);
        else if (tb == typeof(sbyte)) a  = a.ConvertTo(tb);
        else if (ta == typeof(byte)) b   = b.ConvertTo(ta);
        else if (tb == typeof(byte)) a   = a.ConvertTo(tb);
        return a;
    }

    public static bEx AddWithConversion(this Ex left, Ex right) =>
        !left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type
            ? left.Add(right)
            : left.TryConvert(ref right).Add(right);

    public static bEx Add(this Ex left, Ex right) => Ex.Add(left, right);
    public static bEx Add(this Ex left, Ex right, bool conversion) => conversion ? left.AddWithConversion(right) : Ex.Add(left, right);
    public static bEx Add(this Ex left, int right) => left.AddWithConversion(right.ToExpression());
    public static bEx Add(this Ex left, double right) => left.AddWithConversion(right.ToExpression());
    public static bEx Add(this Ex left, decimal right) => left.AddWithConversion(right.ToExpression());
    public static bEx Add(this Ex left, string right) => left.Add(right.ToExpression());
    public static bEx Add<T>(this Ex left, T right) => Ex.Add(left, right as Ex ?? right.ToExpression());

    public static bEx? Add<T>(this Ex? left, params T[]? right)
    {
        var i = 0;
        Ex  l;
        if (left != null) l = left;
        else if (right is null || right.Length == i) return null;
        else l = right[i++].ToExpression();
        while (i < right?.Length)
            l = l.AddWithConversion(right[i++].ToExpression());
        return (bEx)l;
    }

    public static bEx SubtractAssign(this Ex left, Ex right) => Ex.SubtractAssign(left, right);

    public static bEx SubtractAssign<T>(this Ex left, T right) => Ex.SubtractAssign(left, right as Ex ?? right.ToExpression());

    public static bEx SubtractWithConversion(this Ex left, Ex right) =>
        !left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type
            ? left.Subtract(right)
            : left.TryConvert(ref right).Subtract(right);

    public static bEx Subtract(this Ex left, Ex right) => Ex.Subtract(left, right);
    public static bEx Subtract(this Ex left, Ex right, bool conversion) => conversion ? left.SubtractWithConversion(right) : Ex.Subtract(left, right);
    public static bEx Subtract(this Ex left, int right) => left.SubtractWithConversion(right.ToExpression());
    public static bEx Subtract(this Ex left, double right) => left.SubtractWithConversion(right.ToExpression());
    public static bEx Subtract(this Ex left, decimal right) => left.SubtractWithConversion(right.ToExpression());
    public static bEx Subtract<T>(this Ex left, T right) => Ex.Subtract(left, right as Ex ?? right.ToExpression());

    public static bEx MultiplyAssign(this Ex left, Ex right) => Ex.MultiplyAssign(left, right);
    public static bEx MultiplyAssign<T>(this Ex left, T right) => Ex.MultiplyAssign(left, right as Ex ?? right.ToExpression());

    public static bEx MultiplyWithConversion(this Ex left, Ex right) =>
        !left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type
            ? left.Multiply(right)
            : left.TryConvert(ref right).Multiply(right);

    public static bEx Multiply(this Ex left, Ex right, bool conversion = false) => conversion ? left.MultiplyWithConversion(right) : Ex.Multiply(left, right);
    public static bEx Multiply(this Ex left, int right) => left.MultiplyWithConversion(right.ToExpression());
    public static bEx Multiply(this Ex left, double right) => left.MultiplyWithConversion(right.ToExpression());

    public static bEx? Multiply(this Ex? left, params Ex[]? right)
    {
        Ex  l;
        if (left != null) l = left;
        else if (right is not { Length: > 0 }) return null;
        else l = right[1];

        var i = 1;
        while (i < right?.Length)
            l = l.MultiplyWithConversion(right[i++]);
        return (bEx)l;
    }

    public static bEx? Multiply<T>(this Ex? left, params T[]? right)
    {
        Ex l;
        if (left != null) l = left;
        else if (right is not { Length: > 0 }) return null;
        else l = right[1] as Ex ?? right[1].ToExpression();

        var i = 1;
        while (i < right?.Length)
        {
            var v = right[i++];
            l = l.MultiplyWithConversion(v as Ex ?? v.ToExpression());
        }
        return (bEx)l;
    }

    public static bEx DivideAssign(this Ex left, Ex right) => Ex.DivideAssign(left, right);
    public static bEx DivideAssign<T>(this Ex left, T right) => Ex.DivideAssign(left, right as Ex ?? right.ToExpression());

    public static bEx DivideWithConversion(this Ex left, Ex right)
    {
        if (!left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type) return left.Divide(right);
        return left.TryConvert(ref right).Divide(right);
    }

    public static bEx Divide(this Ex left, Ex right) => Ex.Divide(left, right);
    public static bEx Divide<T>(this Ex left, T right) => Ex.Divide(left, right as Ex ?? right.ToExpression());
    public static bEx Divide(this Ex left, Ex right, bool conversion) => conversion ? left.DivideWithConversion(right) : Ex.Divide(left, right);
    public static bEx Divide(this Ex left, int right) => left.DivideWithConversion(right.ToExpression());
    public static bEx Divide(this Ex left, double right) => left.DivideWithConversion(right.ToExpression());

    public static bEx PowerAssign(this Ex left, Ex right) => Ex.PowerAssign(left, right);
    public static bEx PowerAssign<T>(this Ex left, T right) => Ex.PowerAssign(left, right as Ex ?? right.ToExpression());

    public static bEx PowerWithConversion(this Ex left, Ex right)
    {
        if (!left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type) return left.Power(right);
        return left.TryConvert(ref right).Power(right);
    }
    public static bEx PowerOfWithConversion(this Ex left, Ex right)
    {
        if (!left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type) return right.Power(left);
        return right.TryConvert(ref left).Power(left);
    }

    public static bEx Power(this Ex left, Ex right) => Ex.Power(left, right);
    public static bEx Power<T>(this Ex left, T right) => Ex.Power(left, right as Ex ?? right.ToExpression());
    public static bEx Power(this Ex left, Ex right, bool conversion) => conversion ? left.PowerWithConversion(right) : Ex.Power(left, right);
    public static bEx PowerOf(this Ex left, Ex right) => Ex.Power(right, left);
    public static bEx PowerOf<T>(this Ex left, T right) => Ex.Power(right as Ex ?? right.ToExpression(), left);
    public static bEx PowerOf(this Ex left, Ex right, bool conversion) => conversion ? left.PowerOfWithConversion(right) : Ex.Power(right, left);
    public static bEx Power(this Ex left, int right) => left.PowerWithConversion(right.ToExpression());
    public static bEx PowerOf(this Ex left, int right) => right.ToExpression().PowerWithConversion(left);
    public static bEx Power(this Ex left, double right) => left.PowerWithConversion(right.ToExpression());
    public static bEx PowerOf(this Ex left, double right) => right.ToExpression().PowerWithConversion(left);

    public static mcEx Sqrt(this Ex expr) => MathExpression.Sqrt(expr);
    public static bEx SqrtPower(this Ex expr) => MathExpression.SqrtPower(expr);
    public static bEx SqrtPower(this Ex expr, Ex power) => MathExpression.SqrtPower(expr, power);
    public static bEx SqrtPower<T>(this Ex expr, T power) => MathExpression.SqrtPower(expr, power as Ex ?? power.ToExpression());

    public static bEx IsEqual(this Ex left, Ex right) => Equal(left, right);
    public static bEx IsEqual<T>(this Ex left, T right) => Equal(left, right as Ex ?? right.ToExpression());

    public static bEx IsNotEqual(this Ex left, Ex right) => NotEqual(left, right);
    public static bEx IsNotEqual<T>(this Ex left, T right) => NotEqual(left, right as Ex ?? right.ToExpression());

    public static bEx IsGreaterThan(this Ex left, Ex right) => GreaterThan(left, right);
    public static bEx IsGreaterThan<T>(this Ex left, T right) => GreaterThan(left, right as Ex ?? right.ToExpression());

    public static bEx IsGreaterThanOrEqual(this Ex left, Ex right) => GreaterThanOrEqual(left, right);
    public static bEx IsGreaterThanOrEqual<T>(this Ex left, T right) => GreaterThanOrEqual(left, right as Ex ?? right.ToExpression());

    public static bEx IsLessThan(this Ex left, Ex right) => LessThan(left, right);
    public static bEx IsLessThan<T>(this Ex left, T right) => LessThan(left, right as Ex ?? right.ToExpression());

    public static bEx IsLessThanOrEqual(this Ex left, Ex right) => LessThanOrEqual(left, right);
    public static bEx IsLessThanOrEqual<T>(this Ex left, T right) => LessThanOrEqual(left, right as Ex ?? right.ToExpression());

    public static bEx And(this Ex left, Ex right) => Ex.And(left, right);
    public static bEx And<T>(this Ex left, T right) => Ex.And(left, right as Ex ?? right.ToExpression());

    public static bEx AndAssign(this Ex left, Ex right) => Ex.AndAssign(left, right);
    public static bEx AndAssign<T>(this Ex left, T right) => Ex.AndAssign(left, right as Ex ?? right.ToExpression());

    public static bEx OrAssign(this Ex left, Ex right) => Ex.OrAssign(left, right);
    public static bEx OrAssign<T>(this Ex left, T right) => Ex.OrAssign(left, right as Ex ?? right.ToExpression());

    public static bEx Or(this Ex left, Ex right) => Ex.Or(left, right);
    public static bEx Or<T>(this Ex left, T right) => Ex.Or(left, right as Ex ?? right.ToExpression());
    public static uEx Not(this Ex d) => Ex.Not(d);

    public static bEx AndLazy(this Ex left, Ex right) => AndAlso(left, right);
    public static bEx AndLazy<T>(this Ex left, T right) => AndAlso(left, right as Ex ?? right.ToExpression());

    public static bEx OrLazy(this Ex left, Ex right) => OrElse(left, right);
    public static bEx OrLazy<T>(this Ex left, T right) => OrElse(left, right as Ex ?? right.ToExpression());

    public static bEx Coalesce(this Ex first, Ex second) => Ex.Coalesce(first, second);
    public static bEx Coalesce<T>(this Ex first, T second) => Ex.Coalesce(first, second as Ex ?? second.ToExpression());

    public static bEx? Coalesce(this Ex? left, params Ex[]? right)
    {
        var i = 0;
        Ex  l;
        if (left != null) l = left;
        else if (right is null || right.Length == i) return null;
        else l = right[i++];
        while (i < right?.Length)
            l = l.Coalesce(right[i++]);
        return (bEx)l;
    }

    public static bEx? Coalesce<T>(this Ex? left, params T[]? right)
    {
        var i = 0;
        Ex  l;
        if (left != null) l = left;
        else if (right is null || right.Length == i) return null;
        else
        {
            var v = right[i++];
            l = v as Ex ?? v.ToExpression();
        }

        while (i < right?.Length) l = l.Coalesce(right[i++]);

        return (bEx)l;
    }

    public static bEx XORAssign(this Ex left, Ex right) => ExclusiveOrAssign(left, right);
    public static bEx XORAssign<T>(this Ex left, T right) => ExclusiveOrAssign(left, right as Ex ?? right.ToExpression());

    public static bEx XOR(this Ex left, Ex right) => ExclusiveOr(left, right);
    public static bEx XOR<T>(this Ex left, T right) => ExclusiveOr(left, right as Ex ?? right.ToExpression());

    public static bEx? XOR(this Ex? left, params Ex[]? right)
    {
        if (left is null) return null;
        if (right is not { Length: > 0 }) return null;

        var i = 0;
        var l = left;

        while (i < right.Length)
            l = l.XOR(right[i++]);

        return (bEx)l;
    }

    public static bEx? XOR<T>(this Ex? left, params T[]? right)
    {
        if (left is null) return null;
        if (right is not { Length: > 0 }) return null;

        var i = 0;
        var l = left;
        while (i < right.Length)
            l = l.XOR(right[i++]);

        return (bEx)l;
    }

    public static bEx ModuloAssign(this Ex left, Ex right) => Ex.ModuloAssign(left, right);
    public static bEx ModuloAssign<T>(this Ex left, T right) => Ex.ModuloAssign(left, right as Ex ?? right.ToExpression());

    public static bEx Modulo(this Ex left, Ex right) => Ex.Modulo(left, right);
    public static bEx Modulo<T>(this Ex left, T right) => Ex.Modulo(left, right as Ex ?? right.ToExpression());

    public static bEx LeftShiftAssign(this Ex left, Ex right) => Ex.LeftShiftAssign(left, right);
    public static bEx LeftShiftAssign<T>(this Ex left, T right) => Ex.LeftShiftAssign(left, right as Ex ?? right.ToExpression());

    public static bEx LeftShift(this Ex left, Ex right) => Ex.LeftShift(left, right);
    public static bEx LeftShift<T>(this Ex left, T right) => Ex.LeftShift(left, right as Ex ?? right.ToExpression());

    public static bEx RightShiftAssign(this Ex left, Ex right) => Ex.RightShiftAssign(left, right);
    public static bEx RightShiftAssign<T>(this Ex left, T right) => Ex.RightShiftAssign(left, right as Ex ?? right.ToExpression());

    public static bEx RightShift(this Ex left, Ex right) => Ex.RightShift(left, right);
    public static bEx RightShift<T>(this Ex left, T right) => Ex.RightShift(left, right as Ex ?? right.ToExpression());

    public static bEx IsRefEqual(this Ex left, Ex right) => ReferenceEqual(left, right);
    public static bEx IsRefEqual<T>(this Ex left, T right) => ReferenceEqual(left, right as Ex ?? right.ToExpression());

    public static bEx IsIsRefEqual(this Ex left, Ex right) => ReferenceNotEqual(left, right);
    public static bEx IsIsRefEqual<T>(this Ex left, T right) => ReferenceNotEqual(left, right as Ex ?? right.ToExpression());

    public static Ex Condition(this Ex Condition, Ex Then, Ex Else) => Ex.Condition(Condition, Then, Else);
    public static Ex ConditionWithResult<T>(this Ex Condition, T Then, T Else) => Ex.Condition(Condition, Then as Ex ?? Then.ToExpression(), Else as Ex ?? Else.ToExpression());

    public static Ex ToNewExpression(this Type type) => New(type.GetConstructor(Type.EmptyTypes) ?? throw new InvalidOperationException());

    public static Ex ToNewExpression(this Type type, params Ex[] p) => New(type.GetConstructor(p.Select(pp => pp.Type).ToArray()) ?? throw new InvalidOperationException("Конструктор не найден"));
    public static Ex ToNewExpression<T>(this Type type, params T[] p) => 
        New(type.GetConstructor(p.Select(pp => pp!.GetType()).ToArray()) 
            ?? throw new InvalidOperationException("Конструктор не найден"));

    public static pEx ParameterOf(this string ParameterName, Type type) => Parameter(type, ParameterName);
    public static pEx ParameterOf<T>(this string ParameterName) => Parameter(typeof(T), ParameterName);

    public static mcEx GetCall(this Ex obj, string method, IEnumerable<Ex> arg)
        => Call(obj, method, (arg = arg.ToArray()).Select(a => a.Type).ToArray(), (Ex[])arg);

    public static mcEx GetCall(this Ex obj, string method, params Ex[] arg)
        => Call(obj, method, arg.Select(a => a.Type).ToArray(), arg);

    public static mcEx GetCall(this Ex obj, MethodInfo method, IEnumerable<Ex> arg) => Call(obj, method, arg);

    public static mcEx GetCall(this Ex obj, MethodInfo method, params Ex[] arg) => Call(obj, method, arg);

    public static mcEx GetCall(this Ex obj, MethodInfo method) => Call(obj, method);

    public static mcEx GetCall(this Ex obj, Delegate d, IEnumerable<Ex> arg) => obj.GetCall(d.Method, arg);

    public static mcEx GetCall(this Ex obj, Delegate d, params Ex[] arg) => obj.GetCall(d.Method, arg);

    public static mcEx GetCall(this Ex obj, Delegate d) => obj.GetCall(d.Method);

    public static InvocationExpression GetInvoke(this Ex d, IEnumerable<Ex> arg) => Invoke(d, arg);

    public static InvocationExpression GetInvoke(this Ex d, params Ex[] arg) => Invoke(d, arg);

    public static iEx ArrayAccess(this Ex d, IEnumerable<Ex> arg) => Ex.ArrayAccess(d, arg);

    public static iEx ArrayAccess(this Ex d, params Ex[] arg) => Ex.ArrayAccess(d, arg);

    public static mcEx ArrayIndex(this Ex d, IEnumerable<Ex> arg) => Ex.ArrayIndex(d, arg);

    public static mcEx ArrayIndex(this Ex d, params Ex[] arg) => Ex.ArrayIndex(d, arg);

    public static uEx ArrayLength(this Ex d) => Ex.ArrayLength(d);
    public static uEx ConvertTo(this Ex d, Type type) => Convert(d, type);
    public static uEx ConvertTo<T>(this Ex d) => Convert(d, typeof(T));
    public static uEx Increment(this Ex d) => Ex.Increment(d);

    public static bEx Inverse(this Ex expr) => 1.ToExpression().Divide(expr);

    public static uEx Decrement(this Ex d) => Ex.Decrement(d);
    public static uEx IsTrue(this Ex d) => Ex.IsTrue(d);
    public static uEx IsFalse(this Ex d) => Ex.IsFalse(d);
    public static uEx Quote(this Ex d) => Ex.Quote(d);
    public static uEx OnesComplement(this Ex d) => Ex.OnesComplement(d);
    public static DefaultExpression Default(this Type d) => Ex.Default(d);
    public static uEx PostIncrementAssign(this Ex d) => Ex.PostIncrementAssign(d);
    public static uEx PreIncrementAssign(this Ex d) => Ex.PreIncrementAssign(d);
    public static uEx PostDecrementAssign(this Ex d) => Ex.PostDecrementAssign(d);
    public static uEx PreDecrementAssign(this Ex d) => Ex.PreDecrementAssign(d);
    public static uEx Throw(this Ex d) => Ex.Throw(d);
    public static uEx Throw(this Ex d, Type type) => Ex.Throw(d, type);
    public static uEx TypeAs(this Ex d, Type type) => Ex.TypeAs(d, type);
    public static TypeBinaryExpression TypeIs(this Ex d, Type type) => Ex.TypeIs(d, type);
    public static uEx Unbox(this Ex d, Type type) => Ex.Unbox(d, type);
    public static uEx UnaryPlus(this Ex d) => Ex.UnaryPlus(d);
    public static uEx MakeUnary(this Ex d, ExpressionType UType, Type type) => Ex.MakeUnary(UType, d, type);
    public static bEx MakeUnary(this Ex left, Ex right, ExpressionType UType) => MakeBinary(UType, left, right);

    public static Expression<TDelegate> CreateLambda<TDelegate>(this Ex body, params pEx[] p) => Lambda<TDelegate>(body, p);
    public static lEx CreateLambda(this Ex body, params pEx[] p) => Lambda(body, p);
    public static lEx CreateLambda(this Ex body, Type DelegateType, params pEx[] p) => Lambda(DelegateType, body, p);

    public static TDelegate CompileTo<TDelegate>(this Ex body, params pEx[] p) => body.CreateLambda<TDelegate>(p).Compile();

    public static Ex CloneExpression(this Ex expr)
    {
        var visitor = new CloningVisitor();
        return visitor.Visit(expr);
    }

    public static Ex[]? CloneArray(this Ex[]? expr)
    {
        if (expr is null) return null;
        var visitor = new CloningVisitor();
        var result  = new Ex[expr.Length];
        for (var i = 0; i < result.Length; i++)
            result[i] = visitor.Visit(expr[i]);
        return result;
    }
    public static Ex[,]? CloneArray(this Ex[,]? expr)
    {
        if (expr is null) return null;
        var visitor = new CloningVisitor();

        var n      = expr.GetLength(0);
        var m      = expr.GetLength(1);
        var result = new Ex[n, m];
        for (var i = 0; i < n; i++)
            for (var j = 0; j < m; j++)
                result[i, j] = visitor.Visit(expr[i, j]);
        return result;
    }

    #endregion

    public static Expression<Func<T, bool>> IsEqual<T>(this Expression<Func<T, bool>> Expr) =>
        Lambda<Func<T, bool>>(Expr.Body.Not(), Expr.Parameters);

    public static Expression<Func<T, bool>> IsEqual<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, bool>>(Expr.Body.IsEqual(Value), Expr.Parameters);

    public static Expression<Func<T, bool>> IsNotEqual<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, bool>>(Expr.Body.IsNotEqual(Value), Expr.Parameters);

    public static Expression<Func<T, bool>> IsGreaterThan<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, bool>>(Expr.Body.IsGreaterThan(Value), Expr.Parameters);

    public static Expression<Func<T, bool>> IsLessThan<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, bool>>(Expr.Body.IsLessThan(Value), Expr.Parameters);

    public static Expression<Func<T, bool>> IsGreaterThanOrEqual<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, bool>>(Expr.Body.IsGreaterThanOrEqual(Value), Expr.Parameters);

    public static Expression<Func<T, bool>> IsLessThanOrEqual<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, bool>>(Expr.Body.IsLessThanOrEqual(Value), Expr.Parameters);

    public static Expression<Func<T, TValue>> Add<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, TValue>>(Expr.Body.Add(Value), Expr.Parameters);

    public static Expression<Func<T, TValue>> Subtract<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, TValue>>(Expr.Body.Subtract(Value), Expr.Parameters);

    public static Expression<Func<T, TValue>> Multiply<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, TValue>>(Expr.Body.Multiply(Value), Expr.Parameters);

    public static Expression<Func<T, TValue>> Divide<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, TValue>>(Expr.Body.Divide(Value), Expr.Parameters);

    public static Expression<Func<T, TValue>> Power<T, TValue>(this Expression<Func<T, TValue>> Expr, TValue Value) =>
        Lambda<Func<T, TValue>>(Expr.Body.Power(Value), Expr.Parameters);

    public static Ex Simplify(this Ex expr)
    {
        var visitor = new ExpressionRebuilder();
        visitor.BinaryVisited += ExpressionSimplifierRules.Binary;

        return visitor.Visit(expr);
    }

    private static class ExpressionSimplifierRules
    {
        public static Ex Binary(object Sender, EventArgs<bEx> Args)
        {
            var expr = Args.Argument;
            switch (expr.NodeType)
            {
                default:
                    return expr;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return AdditionSimplify(expr);
                //case ExpressionType.And:
                //    break;
                //case ExpressionType.AndAlso:
                //    break;
                //case ExpressionType.Coalesce:
                //    break;
                case ExpressionType.Divide:
                    return DivideSimplify(expr);
                //case ExpressionType.Equal:
                //    break;
                //case ExpressionType.ExclusiveOr:
                //    break;
                //case ExpressionType.GreaterThan:
                //    break;
                //case ExpressionType.GreaterThanOrEqual:
                //    break;
                //case ExpressionType.LeftShift:
                //    break;
                //case ExpressionType.LessThan:
                //    break;
                //case ExpressionType.LessThanOrEqual:
                //    break;
                //case ExpressionType.Modulo:
                //    break;
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return MultiplySimplify(expr);
                //case ExpressionType.NotEqual:
                //    break;
                //case ExpressionType.Or:
                //    break;
                //case ExpressionType.OrElse:
                //    break;
                //case ExpressionType.Power:
                //    break;
                //case ExpressionType.RightShift:
                //    break;
                case ExpressionType.Subtract:
                    return subtractionSimplify(expr);
                //case ExpressionType.SubtractChecked:
                //    break;
                //case ExpressionType.TypeIs:
                //    break;
                //case ExpressionType.Assign:
                //    break;
            }
        }

        #region Is...?

        private static bool IsNumeric(object value) => value 
            is byte 
            or sbyte 
            or short 
            or ushort 
            or int 
            or uint 
            or long 
            or ulong 
            or float 
            or double 
            or Complex 
            or Vector2D 
            or Vector3D;

        private static bool IsZero(object value) =>
            value is ((byte)0) or ((sbyte)0) or ((short)0) or ((ushort)0) or 0 or 0u or 0L or 0ul or 0f or 0d 
            || value is Complex && ((Complex)0).Equals(value) 
            || value is Vector2D && ((Vector2D)0).Equals(value) 
            || value is Vector3D && ((Vector3D)0).Equals(value)
        ;

        private static bool IsUnit(object value) =>
            value is ((byte)1) or ((sbyte)1) or ((short)1) or ((ushort)1) or 1 or 1u or 1L or 1ul or 1f or 1d 
            || value is Complex && Complex.Real.Equals(value)
        ;

        #endregion

        private static Ex MultiplySimplify(bEx expr)
        {
            //var is_checked = expr.NodeType == ExpressionType.MultiplyChecked;
            if (IsZero((expr.Left as cEx)?.Value)) return expr.Left;
            if (IsUnit((expr.Left as cEx)?.Value)) return expr.Right;

            if (IsZero((expr.Right as cEx)?.Value)) return expr.Right;
            if (IsUnit((expr.Right as cEx)?.Value)) return expr.Left;

            return MultiplyValues((expr.Left as cEx)?.Value, (expr.Right as cEx)?.Value) ?? expr;
        }

        private static Ex? MultiplyValues(object left, object right)
        {
            if (!IsNumeric(left) || !IsNumeric(right)) return null;
            return left switch
            {
                byte left1 => right switch
                {
                    byte b             => (left1 * b).ToExpression(),
                    sbyte right1       => (left1 * right1).ToExpression(),
                    short s            => (left1 * s).ToExpression(),
                    ushort right1      => (left1 * right1).ToExpression(),
                    int i              => (left1 * i).ToExpression(),
                    uint u             => (left1 * u).ToExpression(),
                    long l             => (left1 * l).ToExpression(),
                    ulong right1       => (left1 * right1).ToExpression(),
                    float f            => (left1 * f).ToExpression(),
                    double d           => (left1 * d).ToExpression(),
                    Complex complex    => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _                  => (left1 * (right as Vector3D?))?.ToExpression()
                },
                sbyte left1 => right switch
                {
                    byte b        => (left1 * b).ToExpression(),
                    sbyte right1  => (left1 * right1).ToExpression(),
                    short s       => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i         => (left1 * i).ToExpression(),
                    uint u        => (left1 * u).ToExpression(),
                    long l        => (left1 * l).ToExpression(),
                    //ulong right1 => (left1 * right1).ToExpression(),
                    float f            => (left1 * f).ToExpression(),
                    double d           => (left1 * d).ToExpression(),
                    Complex complex    => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _                  => (left1 * (right as Vector3D?))?.ToExpression()
                },
                short left1 => right switch
                {
                    byte b        => (left1 * b).ToExpression(),
                    sbyte right1  => (left1 * right1).ToExpression(),
                    short s       => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i         => (left1 * i).ToExpression(),
                    uint u        => (left1 * u).ToExpression(),
                    long l        => (left1 * l).ToExpression(),
                    //ulong right1 => (left1 * right1).ToExpression(),
                    float f            => (left1 * f).ToExpression(),
                    double d           => (left1 * d).ToExpression(),
                    Complex complex    => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _                  => (left1 * (right as Vector3D?))?.ToExpression()
                },
                ushort left1 => right switch
                {
                    byte b             => (left1 * b).ToExpression(),
                    sbyte right1       => (left1 * right1).ToExpression(),
                    short s            => (left1 * s).ToExpression(),
                    ushort right1      => (left1 * right1).ToExpression(),
                    int i              => (left1 * i).ToExpression(),
                    uint u             => (left1 * u).ToExpression(),
                    long l             => (left1 * l).ToExpression(),
                    ulong right1       => (left1 * right1).ToExpression(),
                    float f            => (left1 * f).ToExpression(),
                    double d           => (left1 * d).ToExpression(),
                    Complex complex    => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _                  => (left1 * (right as Vector3D?))?.ToExpression()
                },
                int left1 => right switch
                {
                    byte b        => (left1 * b).ToExpression(),
                    sbyte right1  => (left1 * right1).ToExpression(),
                    short s       => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i         => (left1 * i).ToExpression(),
                    uint u        => (left1 * u).ToExpression(),
                    long l        => (left1 * l).ToExpression(),
                    //ulong right1 => (left1 * right1).ToExpression(),
                    float f            => (left1 * f).ToExpression(),
                    double d           => (left1 * d).ToExpression(),
                    Complex complex    => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _                  => (left1 * (right as Vector3D?))?.ToExpression()
                },
                uint left1 => right switch
                {
                    byte b             => (left1 * b).ToExpression(),
                    sbyte right1       => (left1 * right1).ToExpression(),
                    short s            => (left1 * s).ToExpression(),
                    ushort right1      => (left1 * right1).ToExpression(),
                    int i              => (left1 * i).ToExpression(),
                    uint u             => (left1 * u).ToExpression(),
                    long l             => (left1 * l).ToExpression(),
                    ulong right1       => (left1 * right1).ToExpression(),
                    float f            => (left1 * f).ToExpression(),
                    double d           => (left1 * d).ToExpression(),
                    Complex complex    => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _                  => (left1 * (right as Vector3D?))?.ToExpression()
                },
                long left1 => right switch
                {
                    byte b        => (left1 * b).ToExpression(),
                    sbyte right1  => (left1 * right1).ToExpression(),
                    short s       => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i         => (left1 * i).ToExpression(),
                    uint u        => (left1 * u).ToExpression(),
                    long l        => (left1 * l).ToExpression(),
                    //ulong right1 => (left1 * right1).ToExpression(),
                    float f            => (left1 * f).ToExpression(),
                    double d           => (left1 * d).ToExpression(),
                    Complex complex    => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _                  => (left1 * (right as Vector3D?))?.ToExpression()
                },
                ulong left1 => right switch
                {
                    byte b => (left1 * b).ToExpression(),
                    //sbyte right1 => (left1 * right1).ToExpression(),
                    //short s => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    //int i => (left1 * i).ToExpression(),
                    //uint u => (left1 * u).ToExpression(),
                    //long l => (left1 * l).ToExpression(),
                    ulong right1       => (left1 * right1).ToExpression(),
                    float f            => (left1 * f).ToExpression(),
                    double d           => (left1 * d).ToExpression(),
                    Complex complex    => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _                  => (left1 * (right as Vector3D?))?.ToExpression()
                },
                float left1 => right switch
                {
                    byte b             => (left1 * b).ToExpression(),
                    sbyte right1       => (left1 * right1).ToExpression(),
                    short s            => (left1 * s).ToExpression(),
                    ushort right1      => (left1 * right1).ToExpression(),
                    int i              => (left1 * i).ToExpression(),
                    uint u             => (left1 * u).ToExpression(),
                    long l             => (left1 * l).ToExpression(),
                    ulong right1       => (left1 * right1).ToExpression(),
                    float f            => (left1 * f).ToExpression(),
                    double d           => (left1 * d).ToExpression(),
                    Complex complex    => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _                  => (left1 * (right as Vector3D?))?.ToExpression()
                },
                double left1 => right switch
                {
                    byte b             => (left1 * b).ToExpression(),
                    sbyte right1       => (left1 * right1).ToExpression(),
                    short s            => (left1 * s).ToExpression(),
                    ushort right1      => (left1 * right1).ToExpression(),
                    int i              => (left1 * i).ToExpression(),
                    uint u             => (left1 * u).ToExpression(),
                    long l             => (left1 * l).ToExpression(),
                    ulong right1       => (left1 * right1).ToExpression(),
                    float f            => (left1 * f).ToExpression(),
                    double d           => (left1 * d).ToExpression(),
                    Complex complex    => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _                  => (left1 * (right as Vector3D?))?.ToExpression()
                },
                Complex left1 => right switch
                {
                    byte b          => (left1 * b).ToExpression(),
                    sbyte right1    => (left1 * right1).ToExpression(),
                    short s         => (left1 * s).ToExpression(),
                    ushort right1   => (left1 * right1).ToExpression(),
                    int i           => (left1 * i).ToExpression(),
                    uint u          => (left1 * u).ToExpression(),
                    long l          => (left1 * l).ToExpression(),
                    ulong right1    => (left1 * right1).ToExpression(),
                    float f         => (left1 * f).ToExpression(),
                    double d        => (left1 * d).ToExpression(),
                    Complex complex => (left1 * complex).ToExpression(),
                    //Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _ => null
                },
                Vector2D left1 => right switch
                {
                    byte b        => (left1 * b).ToExpression(),
                    sbyte right1  => (left1 * right1).ToExpression(),
                    short s       => (left1 * s).ToExpression(),
                    ushort right1 => (left1 * right1).ToExpression(),
                    int i         => (left1 * i).ToExpression(),
                    uint u        => (left1 * u).ToExpression(),
                    long l        => (left1 * l).ToExpression(),
                    ulong right1  => (left1 * right1).ToExpression(),
                    float f       => (left1 * f).ToExpression(),
                    double d      => (left1 * d).ToExpression(),
                    //Complex complex => (left1 * complex).ToExpression(),
                    Vector2D vector_2d => (left1 * vector_2d).ToExpression(),
                    _                  => null
                },
                Vector3D vector_3d => right switch
                {
                    byte b             => (vector_3d * b).ToExpression(),
                    sbyte right1       => (vector_3d * right1).ToExpression(),
                    short s            => (vector_3d * s).ToExpression(),
                    ushort right1      => (vector_3d * right1).ToExpression(),
                    int i              => (vector_3d * i).ToExpression(),
                    uint u             => (vector_3d * u).ToExpression(),
                    long l             => (vector_3d * l).ToExpression(),
                    ulong right1       => (vector_3d * right1).ToExpression(),
                    float f            => (vector_3d * f).ToExpression(),
                    double d           => (vector_3d * d).ToExpression(),
                    Complex complex    => (vector_3d * complex).ToExpression(),
                    Vector2D vector_2d => (vector_3d * vector_2d).ToExpression(),
                    _                  => (vector_3d * (right as Vector3D?))?.ToExpression()
                },
                _ => null
            };
        }

        private static Ex DivideSimplify(bEx expr)
        {
            if (IsZero((expr.Left as cEx)?.Value)) return expr.Left;
            if (IsUnit((expr.Right as cEx)?.Value)) return expr.Left;

            return DivideValues((expr.Left as cEx)?.Value, (expr.Right as cEx)?.Value) ?? expr;
        }

        private static Ex? DivideValues(object left, object right)
        {
            if (!IsNumeric(left) || !IsNumeric(right)) return null;
            if (left is byte)
            {
                if (IsZero(right))
                {
                    return right switch
                    {
                        double.NaN     => double.NaN.ToExpression(),
                        double and > 0 => double.PositiveInfinity.ToExpression(),
                        double and < 0 => double.NegativeInfinity.ToExpression(),

                        float.NaN      => float.NaN.ToExpression(),
                        float and > 0  => float.PositiveInfinity.ToExpression(),
                        float and < 0  => float.NegativeInfinity.ToExpression(),

                        _              => Ex.Throw(new DivideByZeroException().ToExpression())
                    };


                    //if (right is double)
                    //    return double.IsNaN((double)right)
                    //        ? double.NaN.ToExpression()
                    //        : ((double)right > 0 ? double.PositiveInfinity : double.NegativeInfinity).ToExpression();
                    //if (right is not float) 
                    //    return Ex.Throw(new DivideByZeroException().ToExpression());
                    //return float.IsNaN((float)right)
                    //    ? float.NaN.ToExpression()
                    //    : ((float)right > 0 ? float.PositiveInfinity : float.NegativeInfinity).ToExpression();
                }

                return right switch
                {
                    byte b             => ((byte)left / b).ToExpression(),
                    sbyte right1       => ((byte)left / right1).ToExpression(),
                    short s            => ((byte)left / s).ToExpression(),
                    ushort right1      => ((byte)left / right1).ToExpression(),
                    int i              => ((byte)left / i).ToExpression(),
                    uint u             => ((byte)left / u).ToExpression(),
                    long l             => ((byte)left / l).ToExpression(),
                    ulong right1       => ((byte)left / right1).ToExpression(),
                    float f            => ((byte)left / f).ToExpression(),
                    double d           => ((byte)left / d).ToExpression(),
                    Complex complex    => ((byte)left / complex).ToExpression(),
                    Vector2D vector_2d => ((byte)left / vector_2d).ToExpression(),
                    _                  => ((byte)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is sbyte)
            {
                if (IsZero(right))
                    return ((sbyte)left, right) switch
                    {
                        (_,  double.NaN)     => double.NaN.ToExpression(),
                        (>0, double and > 0) => double.PositiveInfinity.ToExpression(),
                        (_,  double and > 0) => double.NegativeInfinity.ToExpression(),
                        (>0, double and < 0) => double.NegativeInfinity.ToExpression(),
                        (_,  double and < 0) => double.PositiveInfinity.ToExpression(),

                        (_,  float.NaN)     => float.NaN.ToExpression(),
                        (>0, float and > 0) => float.PositiveInfinity.ToExpression(),
                        (_,  float and > 0) => float.NegativeInfinity.ToExpression(),
                        (>0, float and < 0) => float.NegativeInfinity.ToExpression(),
                        (_,  float and < 0) => float.PositiveInfinity.ToExpression(),

                        _ => Ex.Throw(new DivideByZeroException().ToExpression())
                    };

                return right switch
                {
                    byte b        => ((sbyte)left / b).ToExpression(),
                    sbyte right1  => ((sbyte)left / right1).ToExpression(),
                    short s       => ((sbyte)left / s).ToExpression(),
                    ushort right1 => ((sbyte)left / right1).ToExpression(),
                    int i         => ((sbyte)left / i).ToExpression(),
                    uint u        => ((sbyte)left / u).ToExpression(),
                    long l        => ((sbyte)left / l).ToExpression(),
                    //ulong right1 => ((sbyte)left / right1).ToExpression(),
                    float f            => ((sbyte)left / f).ToExpression(),
                    double d           => ((sbyte)left / d).ToExpression(),
                    Complex complex    => ((sbyte)left / complex).ToExpression(),
                    Vector2D vector_2d => ((sbyte)left / vector_2d).ToExpression(),
                    _                  => ((sbyte)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is short)
            {
                if (IsZero(right))
                    return ((short)left, right) switch
                    {
                        (_, double.NaN)        => double.NaN.ToExpression(),
                        ( > 0, double and > 0) => double.PositiveInfinity.ToExpression(),
                        (_, double and > 0)    => double.NegativeInfinity.ToExpression(),
                        ( > 0, double and < 0) => double.NegativeInfinity.ToExpression(),
                        (_, double and < 0)    => double.PositiveInfinity.ToExpression(),

                        (_, float.NaN)        => float.NaN.ToExpression(),
                        ( > 0, float and > 0) => float.PositiveInfinity.ToExpression(),
                        (_, float and > 0)    => float.NegativeInfinity.ToExpression(),
                        ( > 0, float and < 0) => float.NegativeInfinity.ToExpression(),
                        (_, float and < 0)    => float.PositiveInfinity.ToExpression(),

                        _ => Ex.Throw(new DivideByZeroException().ToExpression())
                    };

                return right switch
                {
                    byte b        => ((short)left / b).ToExpression(),
                    sbyte right1  => ((short)left / right1).ToExpression(),
                    short s       => ((short)left / s).ToExpression(),
                    ushort right1 => ((short)left / right1).ToExpression(),
                    int i         => ((short)left / i).ToExpression(),
                    uint u        => ((short)left / u).ToExpression(),
                    long l        => ((short)left / l).ToExpression(),
                    //ulong right1 => ((short)left / right1).ToExpression(),
                    float f            => ((short)left / f).ToExpression(),
                    double d           => ((short)left / d).ToExpression(),
                    Complex complex    => ((short)left / complex).ToExpression(),
                    Vector2D vector_2d => ((short)left / vector_2d).ToExpression(),
                    _                  => ((short)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is ushort)
            {
                if (IsZero(right))
                {
                    if (right is double)
                        return double.IsNaN((double)right)
                            ? double.NaN.ToExpression()
                            : ((double)right > 0 ? double.PositiveInfinity : double.NegativeInfinity).ToExpression();
                    if (right is not float) return Ex.Throw(new DivideByZeroException().ToExpression());
                    return float.IsNaN((float)right)
                        ? float.NaN.ToExpression()
                        : ((float)right > 0 ? float.PositiveInfinity : float.NegativeInfinity).ToExpression();
                }

                return right switch
                {
                    byte b             => ((ushort)left / b).ToExpression(),
                    sbyte right1       => ((ushort)left / right1).ToExpression(),
                    short s            => ((ushort)left / s).ToExpression(),
                    ushort right1      => ((ushort)left / right1).ToExpression(),
                    int i              => ((ushort)left / i).ToExpression(),
                    uint u             => ((ushort)left / u).ToExpression(),
                    long l             => ((ushort)left / l).ToExpression(),
                    ulong right1       => ((ushort)left / right1).ToExpression(),
                    float f            => ((ushort)left / f).ToExpression(),
                    double d           => ((ushort)left / d).ToExpression(),
                    Complex complex    => ((ushort)left / complex).ToExpression(),
                    Vector2D vector_2d => ((ushort)left / vector_2d).ToExpression(),
                    _                  => ((ushort)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is int)
            {
                if (IsZero(right))
                    return ((int)left, right) switch
                    {
                        (_, double.NaN)        => double.NaN.ToExpression(),
                        ( > 0, double and > 0) => double.PositiveInfinity.ToExpression(),
                        (_, double and > 0)    => double.NegativeInfinity.ToExpression(),
                        ( > 0, double and < 0) => double.NegativeInfinity.ToExpression(),
                        (_, double and < 0)    => double.PositiveInfinity.ToExpression(),

                        (_, float.NaN)        => float.NaN.ToExpression(),
                        ( > 0, float and > 0) => float.PositiveInfinity.ToExpression(),
                        (_, float and > 0)    => float.NegativeInfinity.ToExpression(),
                        ( > 0, float and < 0) => float.NegativeInfinity.ToExpression(),
                        (_, float and < 0)    => float.PositiveInfinity.ToExpression(),

                        _ => Ex.Throw(new DivideByZeroException().ToExpression())
                    };

                return right switch
                {
                    byte b        => ((int)left / b).ToExpression(),
                    sbyte right1  => ((int)left / right1).ToExpression(),
                    short s       => ((int)left / s).ToExpression(),
                    ushort right1 => ((int)left / right1).ToExpression(),
                    int i         => ((int)left / i).ToExpression(),
                    uint u        => ((int)left / u).ToExpression(),
                    long l        => ((int)left / l).ToExpression(),
                    //ulong right1 => ((int)left / right1).ToExpression(),
                    float f            => ((int)left / f).ToExpression(),
                    double d           => ((int)left / d).ToExpression(),
                    Complex complex    => ((int)left / complex).ToExpression(),
                    Vector2D vector_2d => ((int)left / vector_2d).ToExpression(),
                    _                  => ((int)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is uint)
            {
                if (IsZero(right))
                {
                    if (right is double)
                    {
                        if (double.IsNaN((double)right)) return double.NaN.ToExpression();
                        return ((double)right > 0 ? double.PositiveInfinity : double.NegativeInfinity).ToExpression();
                    }
                    if (right is float)
                    {
                        if (float.IsNaN((float)right)) return float.NaN.ToExpression();
                        return ((float)right > 0 ? float.PositiveInfinity : float.NegativeInfinity).ToExpression();
                    }
                    return Ex.Throw(new DivideByZeroException().ToExpression());
                }

                return right switch
                {
                    byte b             => ((uint)left / b).ToExpression(),
                    sbyte right1       => ((uint)left / right1).ToExpression(),
                    short s            => ((uint)left / s).ToExpression(),
                    ushort right1      => ((uint)left / right1).ToExpression(),
                    int i              => ((uint)left / i).ToExpression(),
                    uint u             => ((uint)left / u).ToExpression(),
                    long l             => ((uint)left / l).ToExpression(),
                    ulong right1       => ((uint)left / right1).ToExpression(),
                    float f            => ((uint)left / f).ToExpression(),
                    double d           => ((uint)left / d).ToExpression(),
                    Complex complex    => ((uint)left / complex).ToExpression(),
                    Vector2D vector_2d => ((uint)left / vector_2d).ToExpression(),
                    _                  => ((uint)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is long)
            {
                if (IsZero(right))
                    return ((long)left, right) switch
                    {
                        (_, double.NaN)        => double.NaN.ToExpression(),
                        ( > 0, double and > 0) => double.PositiveInfinity.ToExpression(),
                        (_, double and > 0)    => double.NegativeInfinity.ToExpression(),
                        ( > 0, double and < 0) => double.NegativeInfinity.ToExpression(),
                        (_, double and < 0)    => double.PositiveInfinity.ToExpression(),

                        (_, float.NaN)        => float.NaN.ToExpression(),
                        ( > 0, float and > 0) => float.PositiveInfinity.ToExpression(),
                        (_, float and > 0)    => float.NegativeInfinity.ToExpression(),
                        ( > 0, float and < 0) => float.NegativeInfinity.ToExpression(),
                        (_, float and < 0)    => float.PositiveInfinity.ToExpression(),

                        _ => Ex.Throw(new DivideByZeroException().ToExpression())
                    };

                return right switch
                {
                    byte b        => ((long)left / b).ToExpression(),
                    sbyte right1  => ((long)left / right1).ToExpression(),
                    short s       => ((long)left / s).ToExpression(),
                    ushort right1 => ((long)left / right1).ToExpression(),
                    int i         => ((long)left / i).ToExpression(),
                    uint u        => ((long)left / u).ToExpression(),
                    long l        => ((long)left / l).ToExpression(),
                    //ulong right1 => ((long)left / right1).ToExpression(),
                    float f            => ((long)left / f).ToExpression(),
                    double d           => ((long)left / d).ToExpression(),
                    Complex complex    => ((long)left / complex).ToExpression(),
                    Vector2D vector_2d => ((long)left / vector_2d).ToExpression(),
                    _                  => ((long)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is ulong)
            {
                if (IsZero(right))
                {
                    if (right is double)
                    {
                        if (double.IsNaN((double)right)) return double.NaN.ToExpression();
                        return ((double)right > 0 ? double.PositiveInfinity : double.NegativeInfinity).ToExpression();
                    }
                    if (right is float)
                    {
                        if (float.IsNaN((float)right)) return float.NaN.ToExpression();
                        return ((float)right > 0 ? float.PositiveInfinity : float.NegativeInfinity).ToExpression();
                    }
                    return Ex.Throw(new DivideByZeroException().ToExpression());
                }

                return right switch
                {
                    byte b => ((ulong)left / b).ToExpression(),
                    //sbyte right1 => ((ulong)left / right1).ToExpression(),
                    //short s => ((ulong)left / s).ToExpression(),
                    ushort right1 => ((ulong)left / right1).ToExpression(),
                    //int i => ((ulong)left / i).ToExpression(),
                    uint u => ((ulong)left / u).ToExpression(),
                    //long l => ((ulong)left / l).ToExpression(),
                    ulong right1       => ((ulong)left / right1).ToExpression(),
                    float f            => ((ulong)left / f).ToExpression(),
                    double d           => ((ulong)left / d).ToExpression(),
                    Complex complex    => ((ulong)left / complex).ToExpression(),
                    Vector2D vector_2d => ((ulong)left / vector_2d).ToExpression(),
                    _                  => ((ulong)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is float)
            {
                if (IsZero(right))
                    return ((float)left, right) switch
                    {
                        (_, double.NaN)        => double.NaN.ToExpression(),
                        ( > 0, double and > 0) => double.PositiveInfinity.ToExpression(),
                        (_, double and > 0)    => double.NegativeInfinity.ToExpression(),
                        ( > 0, double and < 0) => double.NegativeInfinity.ToExpression(),
                        (_, double and < 0)    => double.PositiveInfinity.ToExpression(),

                        (_, float.NaN)        => float.NaN.ToExpression(),
                        ( > 0, float and > 0) => float.PositiveInfinity.ToExpression(),
                        (_, float and > 0)    => float.NegativeInfinity.ToExpression(),
                        ( > 0, float and < 0) => float.NegativeInfinity.ToExpression(),
                        (_, float and < 0)    => float.PositiveInfinity.ToExpression(),

                        _ => Ex.Throw(new DivideByZeroException().ToExpression())
                    };

                return right switch
                {
                    byte b             => ((float)left / b).ToExpression(),
                    sbyte right1       => ((float)left / right1).ToExpression(),
                    short s            => ((float)left / s).ToExpression(),
                    ushort right1      => ((float)left / right1).ToExpression(),
                    int i              => ((float)left / i).ToExpression(),
                    uint u             => ((float)left / u).ToExpression(),
                    long l             => ((float)left / l).ToExpression(),
                    ulong right1       => ((float)left / right1).ToExpression(),
                    float f            => ((float)left / f).ToExpression(),
                    double d           => ((float)left / d).ToExpression(),
                    Complex complex    => ((float)left / complex).ToExpression(),
                    Vector2D vector_2d => ((float)left / vector_2d).ToExpression(),
                    _                  => ((float)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is double)
            {
                if (IsZero(right))
                    return ((double)left, right) switch
                    {
                        (_, double.NaN)        => double.NaN.ToExpression(),
                        ( > 0, double and > 0) => double.PositiveInfinity.ToExpression(),
                        (_, double and > 0)    => double.NegativeInfinity.ToExpression(),
                        ( > 0, double and < 0) => double.NegativeInfinity.ToExpression(),
                        (_, double and < 0)    => double.PositiveInfinity.ToExpression(),

                        (_, float.NaN)        => float.NaN.ToExpression(),
                        ( > 0, float and > 0) => float.PositiveInfinity.ToExpression(),
                        (_, float and > 0)    => float.NegativeInfinity.ToExpression(),
                        ( > 0, float and < 0) => float.NegativeInfinity.ToExpression(),
                        (_, float and < 0)    => float.PositiveInfinity.ToExpression(),

                        _ => Ex.Throw(new DivideByZeroException().ToExpression())
                    };

                return right switch
                {
                    byte b             => ((double)left / b).ToExpression(),
                    sbyte right1       => ((double)left / right1).ToExpression(),
                    short s            => ((double)left / s).ToExpression(),
                    ushort right1      => ((double)left / right1).ToExpression(),
                    int i              => ((double)left / i).ToExpression(),
                    uint u             => ((double)left / u).ToExpression(),
                    long l             => ((double)left / l).ToExpression(),
                    ulong right1       => ((double)left / right1).ToExpression(),
                    float f            => ((double)left / f).ToExpression(),
                    double d           => ((double)left / d).ToExpression(),
                    Complex complex    => ((double)left / complex).ToExpression(),
                    Vector2D vector_2d => ((double)left / vector_2d).ToExpression(),
                    _                  => ((double)left / (right as Vector3D?))?.ToExpression()
                };
            }
            if (left is Complex)
            {
                if (IsZero(right))
                    return Ex.Throw(new DivideByZeroException().ToExpression());
                return right switch
                {
                    byte b          => ((Complex)left / b).ToExpression(),
                    sbyte right1    => ((Complex)left / right1).ToExpression(),
                    short s         => ((Complex)left / s).ToExpression(),
                    ushort right1   => ((Complex)left / right1).ToExpression(),
                    int i           => ((Complex)left / i).ToExpression(),
                    uint u          => ((Complex)left / u).ToExpression(),
                    long l          => ((Complex)left / l).ToExpression(),
                    ulong right1    => ((Complex)left / right1).ToExpression(),
                    float f         => ((Complex)left / f).ToExpression(),
                    double d        => ((Complex)left / d).ToExpression(),
                    Complex complex => ((Complex)left / complex).ToExpression(),
                    //Vector2D vector_2d => ((Complex)left / vector_2d).ToExpression(),
                    _ => null
                };
            }
            if (left is Vector2D)
            {
                if (IsZero(right))
                    return Ex.Throw(new DivideByZeroException().ToExpression());
                return right switch
                {
                    byte b        => ((Vector2D)left / b).ToExpression(),
                    sbyte right1  => ((Vector2D)left / right1).ToExpression(),
                    short s       => ((Vector2D)left / s).ToExpression(),
                    ushort right1 => ((Vector2D)left / right1).ToExpression(),
                    int i         => ((Vector2D)left / i).ToExpression(),
                    uint u        => ((Vector2D)left / u).ToExpression(),
                    long l        => ((Vector2D)left / l).ToExpression(),
                    ulong right1  => ((Vector2D)left / right1).ToExpression(),
                    float f       => ((Vector2D)left / f).ToExpression(),
                    //Complex complex => ((Vector2D)left / complex).ToExpression(),
                    //Vector2D vector_2d => ((Vector2D)left / vector_2d).ToExpression(),
                    _ => ((Vector2D)left / (right as double?))?.ToExpression()
                };
            }
            if (left is Vector3D)
            {
                if (IsZero(right))
                    return Ex.Throw(new DivideByZeroException().ToExpression());
                return right switch
                {
                    byte b        => ((Vector3D)left / b).ToExpression(),
                    sbyte right1  => ((Vector3D)left / right1).ToExpression(),
                    short s       => ((Vector3D)left / s).ToExpression(),
                    ushort right1 => ((Vector3D)left / right1).ToExpression(),
                    int i         => ((Vector3D)left / i).ToExpression(),
                    uint u        => ((Vector3D)left / u).ToExpression(),
                    long l        => ((Vector3D)left / l).ToExpression(),
                    ulong right1  => ((Vector3D)left / right1).ToExpression(),
                    float f       => ((Vector3D)left / f).ToExpression(),
                    double d      => ((Vector3D)left / d).ToExpression(),
                    //Complex complex => ((Vector3D)left / complex).ToExpression(),
                    //Vector2D vector_2d => ((Vector3D)left / vector_2d).ToExpression(),
                    //_ => ((Vector3D)left / (right as Vector3D?))?.ToExpression()
                    _ => null
                };
            }
            return null;
        }

        private static Ex AdditionSimplify(bEx expr)
        {
            var right = expr.Right;
            var left  = expr.Left;
            if (IsZero((left as cEx)?.Value)) return right;
            if (IsZero((right as cEx)?.Value)) return left;

            //if(right.NodeType == ExpressionType.Add || right.NodeType == ExpressionType.Subtract)
            //{
            //    var right_operands = GetOperands_Addition(right as bEx).ToArray();
            //    var consts = right_operands.Where(e => e is cEx || e.NodeType == ExpressionType.Negate && ((uEx)e).Operand is cEx).ToList();
            //    var vars = right_operands.Except(consts).ToList();

            //    Expression sum = null;
            //    while(sum is null && consts.Count > 0)
            //        if()

            //            if(consts.Count > 1)
            //            {
            //                for(var i = 0; i < consts.Count; i++)
            //                {
            //                    var s = AddValues((sum as cEx)?.Value, (consts[i] as cEx)?.Value);
            //                    if(s is null)
            //        }
            //            }
            //}


            return AddValues((left as cEx)?.Value, (right as cEx)?.Value) ?? expr;
        }

        private static IEnumerable<Ex> GetOperands_Addition(bEx? expr)
        {
            if (expr is null || expr.NodeType is not (ExpressionType.Add and ExpressionType.Subtract)) yield break;

            var left = expr.Left;
            if (left is bEx && left.NodeType == ExpressionType.Add || left.NodeType == ExpressionType.Subtract)
                foreach (var item in GetOperands_Addition(left as bEx))
                    yield return item;
            else
                yield return left;

            var right = expr.Right;
            if (right is bEx && right.NodeType == ExpressionType.Add || right.NodeType == ExpressionType.Subtract)
                if (expr.NodeType == ExpressionType.Add)
                    foreach (var item in GetOperands_Addition(left as bEx))
                        yield return item;
                else
                    foreach (var item in GetOperands_Addition(left as bEx))
                        if (item.NodeType == ExpressionType.Negate)
                            yield return ((uEx)item).Operand;
                        else
                            yield return item.Negate();
            else
                yield return right;
        }

        private static Ex? AddValues(object left, object right)
        {
            if (!IsNumeric(left) || !IsNumeric(right)) return null;
            return left switch
            {
                byte left1 => right switch
                {
                    byte b             => (left1 + b).ToExpression(),
                    sbyte right1       => (left1 + right1).ToExpression(),
                    short s            => (left1 + s).ToExpression(),
                    ushort right1      => (left1 + right1).ToExpression(),
                    int i              => (left1 + i).ToExpression(),
                    uint u             => (left1 + u).ToExpression(),
                    long l             => (left1 + l).ToExpression(),
                    ulong right1       => (left1 + right1).ToExpression(),
                    float f            => (left1 + f).ToExpression(),
                    double d           => (left1 + d).ToExpression(),
                    Complex complex    => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _                  => (left1 + (right as Vector3D?))?.ToExpression()
                },
                sbyte left1 => right switch
                {
                    byte b        => (left1 + b).ToExpression(),
                    sbyte right1  => (left1 + right1).ToExpression(),
                    short s       => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i         => (left1 + i).ToExpression(),
                    uint u        => (left1 + u).ToExpression(),
                    long l        => (left1 + l).ToExpression(),
                    //ulong right1 => (left1 + right1).ToExpression(),
                    float f            => (left1 + f).ToExpression(),
                    double d           => (left1 + d).ToExpression(),
                    Complex complex    => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _                  => (left1 + (right as Vector3D?))?.ToExpression()
                },
                short left1 => right switch
                {
                    byte b        => (left1 + b).ToExpression(),
                    sbyte right1  => (left1 + right1).ToExpression(),
                    short s       => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i         => (left1 + i).ToExpression(),
                    uint u        => (left1 + u).ToExpression(),
                    long l        => (left1 + l).ToExpression(),
                    //ulong right1 => (left1 + right1).ToExpression(),
                    float f            => (left1 + f).ToExpression(),
                    double d           => (left1 + d).ToExpression(),
                    Complex complex    => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _                  => (left1 + (right as Vector3D?))?.ToExpression()
                },
                ushort left1 => right switch
                {
                    byte b             => (left1 + b).ToExpression(),
                    sbyte right1       => (left1 + right1).ToExpression(),
                    short s            => (left1 + s).ToExpression(),
                    ushort right1      => (left1 + right1).ToExpression(),
                    int i              => (left1 + i).ToExpression(),
                    uint u             => (left1 + u).ToExpression(),
                    long l             => (left1 + l).ToExpression(),
                    ulong right1       => (left1 + right1).ToExpression(),
                    float f            => (left1 + f).ToExpression(),
                    double d           => (left1 + d).ToExpression(),
                    Complex complex    => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _                  => (left1 + (right as Vector3D?))?.ToExpression()
                },
                int left1 => right switch
                {
                    byte b        => (left1 + b).ToExpression(),
                    sbyte right1  => (left1 + right1).ToExpression(),
                    short s       => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i         => (left1 + i).ToExpression(),
                    uint u        => (left1 + u).ToExpression(),
                    long l        => (left1 + l).ToExpression(),
                    //ulong right1 => (left1 + right1).ToExpression(),
                    float f            => (left1 + f).ToExpression(),
                    double d           => (left1 + d).ToExpression(),
                    Complex complex    => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _                  => (left1 + (right as Vector3D?))?.ToExpression()
                },
                uint left1 => right switch
                {
                    byte b             => (left1 + b).ToExpression(),
                    sbyte right1       => (left1 + right1).ToExpression(),
                    short s            => (left1 + s).ToExpression(),
                    ushort right1      => (left1 + right1).ToExpression(),
                    int i              => (left1 + i).ToExpression(),
                    uint u             => (left1 + u).ToExpression(),
                    long l             => (left1 + l).ToExpression(),
                    ulong right1       => (left1 + right1).ToExpression(),
                    float f            => (left1 + f).ToExpression(),
                    double d           => (left1 + d).ToExpression(),
                    Complex complex    => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _                  => (left1 + (right as Vector3D?))?.ToExpression()
                },
                long left1 => right switch
                {
                    byte b        => (left1 + b).ToExpression(),
                    sbyte right1  => (left1 + right1).ToExpression(),
                    short s       => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i         => (left1 + i).ToExpression(),
                    uint u        => (left1 + u).ToExpression(),
                    long l        => (left1 + l).ToExpression(),
                    //ulong right1 => (left1 + right1).ToExpression(),
                    float f            => (left1 + f).ToExpression(),
                    double d           => (left1 + d).ToExpression(),
                    Complex complex    => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _                  => (left1 + (right as Vector3D?))?.ToExpression()
                },
                ulong left1 => right switch
                {
                    byte b => (left1 + b).ToExpression(),
                    //sbyte right1 => (left1 + right1).ToExpression(),
                    //short s => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    //int i => (left1 + i).ToExpression(),
                    uint u => (left1 + u).ToExpression(),
                    //long l => (left1 + l).ToExpression(),
                    ulong right1       => (left1 + right1).ToExpression(),
                    float f            => (left1 + f).ToExpression(),
                    double d           => (left1 + d).ToExpression(),
                    Complex complex    => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _                  => (left1 + (right as Vector3D?))?.ToExpression()
                },
                float left1 => right switch
                {
                    byte b             => (left1 + b).ToExpression(),
                    sbyte right1       => (left1 + right1).ToExpression(),
                    short s            => (left1 + s).ToExpression(),
                    ushort right1      => (left1 + right1).ToExpression(),
                    int i              => (left1 + i).ToExpression(),
                    uint u             => (left1 + u).ToExpression(),
                    long l             => (left1 + l).ToExpression(),
                    ulong right1       => (left1 + right1).ToExpression(),
                    float f            => (left1 + f).ToExpression(),
                    double d           => (left1 + d).ToExpression(),
                    Complex complex    => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _                  => (left1 + (right as Vector3D?))?.ToExpression()
                },
                double left1 => right switch
                {
                    byte b             => (left1 + b).ToExpression(),
                    sbyte right1       => (left1 + right1).ToExpression(),
                    short s            => (left1 + s).ToExpression(),
                    ushort right1      => (left1 + right1).ToExpression(),
                    int i              => (left1 + i).ToExpression(),
                    uint u             => (left1 + u).ToExpression(),
                    long l             => (left1 + l).ToExpression(),
                    ulong right1       => (left1 + right1).ToExpression(),
                    float f            => (left1 + f).ToExpression(),
                    double d           => (left1 + d).ToExpression(),
                    Complex complex    => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _                  => (left1 + (right as Vector3D?))?.ToExpression()
                },
                Complex left1 => right switch
                {
                    byte b          => (left1 + b).ToExpression(),
                    sbyte right1    => (left1 + right1).ToExpression(),
                    short s         => (left1 + s).ToExpression(),
                    ushort right1   => (left1 + right1).ToExpression(),
                    int i           => (left1 + i).ToExpression(),
                    uint u          => (left1 + u).ToExpression(),
                    long l          => (left1 + l).ToExpression(),
                    ulong right1    => (left1 + right1).ToExpression(),
                    float f         => (left1 + f).ToExpression(),
                    double d        => (left1 + d).ToExpression(),
                    Complex complex => (left1 + complex).ToExpression(),
                    //Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _ => null
                },
                Vector2D left1 => right switch
                {
                    byte b        => (left1 + b).ToExpression(),
                    sbyte right1  => (left1 + right1).ToExpression(),
                    short s       => (left1 + s).ToExpression(),
                    ushort right1 => (left1 + right1).ToExpression(),
                    int i         => (left1 + i).ToExpression(),
                    uint u        => (left1 + u).ToExpression(),
                    long l        => (left1 + l).ToExpression(),
                    ulong right1  => (left1 + right1).ToExpression(),
                    float f       => (left1 + f).ToExpression(),
                    double d      => (left1 + d).ToExpression(),
                    //Complex complex => (left1 + complex).ToExpression(),
                    Vector2D vector_2d => (left1 + vector_2d).ToExpression(),
                    _                  => null
                },
                Vector3D vector_3d => right switch
                {
                    byte b             => (vector_3d + b).ToExpression(),
                    sbyte right1       => (vector_3d + right1).ToExpression(),
                    short s            => (vector_3d + s).ToExpression(),
                    ushort right1      => (vector_3d + right1).ToExpression(),
                    int i              => (vector_3d + i).ToExpression(),
                    uint u             => (vector_3d + u).ToExpression(),
                    long l             => (vector_3d + l).ToExpression(),
                    ulong right1       => (vector_3d + right1).ToExpression(),
                    float f            => (vector_3d + f).ToExpression(),
                    double d           => (vector_3d + d).ToExpression(),
                    Complex complex    => (vector_3d + complex).ToExpression(),
                    Vector2D vector_2d => (vector_3d + vector_2d).ToExpression(),
                    _                  => (vector_3d + (right as Vector3D?))?.ToExpression()
                },
                _ => null
            };
        }

        private static Ex subtractionSimplify(bEx expr)
        {
            if (IsZero((expr.Left as cEx)?.Value)) return expr.Right.Negate();
            if (IsZero((expr.Right as cEx)?.Value)) return expr.Left;

            return subtractValues((expr.Left as cEx)?.Value, (expr.Right as cEx)?.Value) ?? expr;
        }

        private static Ex? subtractValues(object left, object right)
        {
            if (!IsNumeric(left) || !IsNumeric(right)) return null;
            return left switch
            {
                byte left1 => right switch
                {
                    byte b             => (left1 - b).ToExpression(),
                    sbyte right1       => (left1 - right1).ToExpression(),
                    short s            => (left1 - s).ToExpression(),
                    ushort right1      => (left1 - right1).ToExpression(),
                    int i              => (left1 - i).ToExpression(),
                    uint u             => (left1 - u).ToExpression(),
                    long l             => (left1 - l).ToExpression(),
                    ulong right1       => (left1 - right1).ToExpression(),
                    float f            => (left1 - f).ToExpression(),
                    double d           => (left1 - d).ToExpression(),
                    Complex complex    => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _                  => (left1 - (right as Vector3D?))?.ToExpression()
                },
                sbyte left1 => right switch
                {
                    byte b        => (left1 - b).ToExpression(),
                    sbyte right1  => (left1 - right1).ToExpression(),
                    short s       => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i         => (left1 - i).ToExpression(),
                    uint u        => (left1 - u).ToExpression(),
                    long l        => (left1 - l).ToExpression(),
                    //ulong right1 => (left1 - right1).ToExpression(),
                    float f            => (left1 - f).ToExpression(),
                    double d           => (left1 - d).ToExpression(),
                    Complex complex    => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _                  => (left1 - (right as Vector3D?))?.ToExpression()
                },
                short left1 => right switch
                {
                    byte b        => (left1 - b).ToExpression(),
                    sbyte right1  => (left1 - right1).ToExpression(),
                    short s       => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i         => (left1 - i).ToExpression(),
                    uint u        => (left1 - u).ToExpression(),
                    long l        => (left1 - l).ToExpression(),
                    //ulong right1 => (left1 - right1).ToExpression(),
                    float f            => (left1 - f).ToExpression(),
                    double d           => (left1 - d).ToExpression(),
                    Complex complex    => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _                  => (left1 - (right as Vector3D?))?.ToExpression()
                },
                ushort left1 => right switch
                {
                    byte b             => (left1 - b).ToExpression(),
                    sbyte right1       => (left1 - right1).ToExpression(),
                    short s            => (left1 - s).ToExpression(),
                    ushort right1      => (left1 - right1).ToExpression(),
                    int i              => (left1 - i).ToExpression(),
                    uint u             => (left1 - u).ToExpression(),
                    long l             => (left1 - l).ToExpression(),
                    ulong right1       => (left1 - right1).ToExpression(),
                    float f            => (left1 - f).ToExpression(),
                    double d           => (left1 - d).ToExpression(),
                    Complex complex    => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _                  => (left1 - (right as Vector3D?))?.ToExpression()
                },
                int left1 => right switch
                {
                    byte b        => (left1 - b).ToExpression(),
                    sbyte right1  => (left1 - right1).ToExpression(),
                    short s       => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i         => (left1 - i).ToExpression(),
                    uint u        => (left1 - u).ToExpression(),
                    long l        => (left1 - l).ToExpression(),
                    //ulong right1 => (left1 - right1).ToExpression(),
                    float f            => (left1 - f).ToExpression(),
                    double d           => (left1 - d).ToExpression(),
                    Complex complex    => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _                  => (left1 - (right as Vector3D?))?.ToExpression()
                },
                uint left1 => right switch
                {
                    byte b             => (left1 - b).ToExpression(),
                    sbyte right1       => (left1 - right1).ToExpression(),
                    short s            => (left1 - s).ToExpression(),
                    ushort right1      => (left1 - right1).ToExpression(),
                    int i              => (left1 - i).ToExpression(),
                    uint u             => (left1 - u).ToExpression(),
                    long l             => (left1 - l).ToExpression(),
                    ulong right1       => (left1 - right1).ToExpression(),
                    float f            => (left1 - f).ToExpression(),
                    double d           => (left1 - d).ToExpression(),
                    Complex complex    => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _                  => (left1 - (right as Vector3D?))?.ToExpression()
                },
                long left1 => right switch
                {
                    byte b        => (left1 - b).ToExpression(),
                    sbyte right1  => (left1 - right1).ToExpression(),
                    short s       => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i         => (left1 - i).ToExpression(),
                    uint u        => (left1 - u).ToExpression(),
                    long l        => (left1 - l).ToExpression(),
                    //ulong right1 => (left1 - right1).ToExpression(),
                    float f            => (left1 - f).ToExpression(),
                    double d           => (left1 - d).ToExpression(),
                    Complex complex    => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _                  => (left1 - (right as Vector3D?))?.ToExpression()
                },
                ulong left1 => right switch
                {
                    byte b => (left1 - b).ToExpression(),
                    //sbyte right1 => (left1 - right1).ToExpression(),
                    //short s => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    //int i => (left1 - i).ToExpression(),
                    uint u => (left1 - u).ToExpression(),
                    //long l => (left1 - l).ToExpression(),
                    ulong right1       => (left1 - right1).ToExpression(),
                    float f            => (left1 - f).ToExpression(),
                    double d           => (left1 - d).ToExpression(),
                    Complex complex    => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _                  => (left1 - (right as Vector3D?))?.ToExpression()
                },
                float left1 => right switch
                {
                    byte b             => (left1 - b).ToExpression(),
                    sbyte right1       => (left1 - right1).ToExpression(),
                    short s            => (left1 - s).ToExpression(),
                    ushort right1      => (left1 - right1).ToExpression(),
                    int i              => (left1 - i).ToExpression(),
                    uint u             => (left1 - u).ToExpression(),
                    long l             => (left1 - l).ToExpression(),
                    ulong right1       => (left1 - right1).ToExpression(),
                    float f            => (left1 - f).ToExpression(),
                    double d           => (left1 - d).ToExpression(),
                    Complex complex    => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _                  => (left1 - (right as Vector3D?))?.ToExpression()
                },
                double left1 => right switch
                {
                    byte b             => (left1 - b).ToExpression(),
                    sbyte right1       => (left1 - right1).ToExpression(),
                    short s            => (left1 - s).ToExpression(),
                    ushort right1      => (left1 - right1).ToExpression(),
                    int i              => (left1 - i).ToExpression(),
                    uint u             => (left1 - u).ToExpression(),
                    long l             => (left1 - l).ToExpression(),
                    ulong right1       => (left1 - right1).ToExpression(),
                    float f            => (left1 - f).ToExpression(),
                    double d           => (left1 - d).ToExpression(),
                    Complex complex    => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _                  => (left1 - (right as Vector3D?))?.ToExpression()
                },
                Complex left1 => right switch
                {
                    byte b          => (left1 - b).ToExpression(),
                    sbyte right1    => (left1 - right1).ToExpression(),
                    short s         => (left1 - s).ToExpression(),
                    ushort right1   => (left1 - right1).ToExpression(),
                    int i           => (left1 - i).ToExpression(),
                    uint u          => (left1 - u).ToExpression(),
                    long l          => (left1 - l).ToExpression(),
                    ulong right1    => (left1 - right1).ToExpression(),
                    float f         => (left1 - f).ToExpression(),
                    double d        => (left1 - d).ToExpression(),
                    Complex complex => (left1 - complex).ToExpression(),
                    //Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _ => null
                },
                Vector2D left1 => right switch
                {
                    byte b        => (left1 - b).ToExpression(),
                    sbyte right1  => (left1 - right1).ToExpression(),
                    short s       => (left1 - s).ToExpression(),
                    ushort right1 => (left1 - right1).ToExpression(),
                    int i         => (left1 - i).ToExpression(),
                    uint u        => (left1 - u).ToExpression(),
                    long l        => (left1 - l).ToExpression(),
                    ulong right1  => (left1 - right1).ToExpression(),
                    float f       => (left1 - f).ToExpression(),
                    double d      => (left1 - d).ToExpression(),
                    //Complex complex => (left1 - complex).ToExpression(),
                    Vector2D vector_2d => (left1 - vector_2d).ToExpression(),
                    _                  => null
                },
                Vector3D vector_3d => right switch
                {
                    byte b             => (vector_3d - b).ToExpression(),
                    sbyte right1       => (vector_3d - right1).ToExpression(),
                    short s            => (vector_3d - s).ToExpression(),
                    ushort right1      => (vector_3d - right1).ToExpression(),
                    int i              => (vector_3d - i).ToExpression(),
                    uint u             => (vector_3d - u).ToExpression(),
                    long l             => (vector_3d - l).ToExpression(),
                    ulong right1       => (vector_3d - right1).ToExpression(),
                    float f            => (vector_3d - f).ToExpression(),
                    double d           => (vector_3d - d).ToExpression(),
                    Complex complex    => (vector_3d - complex).ToExpression(),
                    Vector2D vector_2d => (vector_3d - vector_2d).ToExpression(),
                    _                  => (vector_3d - (right as Vector3D?))?.ToExpression()
                },
                _ => null
            };
        }
    }

    public static MethodCallExpression GetAbs(this Ex x) => Call(((Func<double, double>)Math.Abs).Method, x);
}