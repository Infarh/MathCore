using System.Linq.Expressions;
using System.Reflection;

using static System.Linq.Expressions.Expression;

using bEx = System.Linq.Expressions.BinaryExpression;
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
public static partial class ExpressionExtensions
{

    #region Types

    #endregion

    #region Methods

    /// <exception cref="FormatException">Количество аргументов подстановки не равно 1, или во входном выражении отсутствие подставляемый параметр</exception>
    public static lEx? Substitute(this lEx Expr, lEx Substitution)
    {
        var parameters = Expr.Parameters;
        var substitute_parameters = Substitution.Parameters;
        if (substitute_parameters.Count != 1)
            throw new FormatException("Количество аргументов подстановки не равно 1");
        var substitute_parameter = substitute_parameters[0];
        if (!parameters.Contains(p => p.Name == substitute_parameter.Name && p.Type == substitute_parameter.Type))
            throw new FormatException("Во входном выражении отсутствие подставляемый параметр");

        var visitor = new SubstitutionVisitor(Substitution);
        var result = visitor.Visit(Expr);

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
            throw new($"Could not find input parameter \"{ParameterName}\" in Expression \"{MainEx}\"");

        var substitution_parameter = SubstExpression.Parameters.FirstOrDefault(p => p.Name == ParameterName);

        if (substitution_parameter is null)
            throw new($"Could not find substitution parameter \"{ParameterName}\" in Expression \"{SubstExpression}\"");

        if (substitution_parameter.Type != main_parameter.Type)
            throw new(
                $"The substitute Expression return type \"{SubstExpression.Type}\" does not match the type of the substituted variable \"{ParameterName}:{main_parameter.Type}\""
            );

        #endregion

        var pars = MainEx.Parameters.ToList();

        var idx_to_subst = pars.IndexOf(main_parameter);

        pars.RemoveAt(idx_to_subst);

        foreach (var subst_pe in SubstExpression.Parameters)
        {
            if (pars.Count(pe => pe.Name == subst_pe.Name) != 0)
                throw new($"Input parameter of name \"{subst_pe.Name}\" already exists in the main Expression");
            pars.Insert(idx_to_subst, subst_pe);
            idx_to_subst++;
        }

        var visitor =
            new SubstExpressionVisitor
            {
                SubstExpression = SubstExpression,
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

        if (ta == typeof(double)) b = b.ConvertTo(ta);
        else if (tb == typeof(double)) a = a.ConvertTo(tb);
        else if (ta == typeof(float)) b = b.ConvertTo(ta);
        else if (tb == typeof(float)) a = a.ConvertTo(tb);
        else if (ta == typeof(ulong)) b = b.ConvertTo(ta);
        else if (tb == typeof(ulong)) a = a.ConvertTo(tb);
        else if (ta == typeof(long)) b = b.ConvertTo(ta);
        else if (tb == typeof(long)) a = a.ConvertTo(tb);
        else if (ta == typeof(uint)) b = b.ConvertTo(ta);
        else if (tb == typeof(uint)) a = a.ConvertTo(tb);
        else if (ta == typeof(int)) b = b.ConvertTo(ta);
        else if (tb == typeof(int)) a = a.ConvertTo(tb);
        else if (ta == typeof(sbyte)) b = b.ConvertTo(ta);
        else if (tb == typeof(sbyte)) a = a.ConvertTo(tb);
        else if (ta == typeof(byte)) b = b.ConvertTo(ta);
        else if (tb == typeof(byte)) a = a.ConvertTo(tb);
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

    public static bEx? Add<T>(this Ex? left, params IReadOnlyList<T>? right)
    {
        var i = 0;
        Ex l;
        if (left != null) l = left;
        else if (right is null || right.Count == i) return null;
        else l = right[i++].ToExpression();
        while (i < right?.Count)
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
            ? left.Mult(right)
            : left.TryConvert(ref right).Mult(right);

    public static bEx Multiply(this Ex left, Ex right, bool conversion = false) => conversion ? left.MultiplyWithConversion(right) : Ex.Multiply(left, right);
    public static bEx Multiply(this Ex left, int right) => left.MultiplyWithConversion(right.ToExpression());
    public static bEx Multiply(this Ex left, double right) => left.MultiplyWithConversion(right.ToExpression());

    public static bEx? Multiply(this Ex? left, params IReadOnlyList<Ex>? right)
    {
        Ex l;
        if (left != null) l = left;
        else if (right is not { Count: > 0 }) return null;
        else l = right[1];

        var i = 1;
        while (i < right?.Count)
            l = l.MultiplyWithConversion(right[i++]);
        return (bEx)l;
    }

    public static bEx Mult<T>(this Ex left, T right) => Ex.Multiply(left, right as Ex ?? right.ToExpression());

    public static bEx Mult<T>(this Ex left, Ex right) => Ex.Multiply(left, right);

    public static bEx? Multiply<T>(this Ex? left, params IReadOnlyList<T>? right)
    {
        Ex l;
        if (left != null) l = left;
        else if (right is not { Count: > 0 }) return null;
        else l = right[1] as Ex ?? right[1].ToExpression();

        var i = 1;
        while (i < right?.Count)
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

    public static bEx? Coalesce(this Ex? left, params IReadOnlyList<Ex>? right)
    {
        var i = 0;
        Ex l;
        if (left != null) l = left;
        else if (right is null || right.Count == i) return null;
        else l = right[i++];
        while (i < right?.Count)
            l = l.Coalesce(right[i++]);
        return (bEx)l;
    }

    public static bEx? Coalesce<T>(this Ex? left, params IReadOnlyList<T>? right)
    {
        var i = 0;
        Ex l;
        if (left != null) l = left;
        else if (right is null || right.Count == i) return null;
        else
        {
            var v = right[i++];
            l = v as Ex ?? v.ToExpression();
        }

        while (i < right?.Count) l = l.Coalesce(right[i++]);

        return (bEx)l;
    }

    public static bEx XORAssign(this Ex left, Ex right) => ExclusiveOrAssign(left, right);
    public static bEx XORAssign<T>(this Ex left, T right) => ExclusiveOrAssign(left, right as Ex ?? right.ToExpression());

    public static bEx XOR(this Ex left, Ex right) => ExclusiveOr(left, right);
    public static bEx XOR<T>(this Ex left, T right) => ExclusiveOr(left, right as Ex ?? right.ToExpression());

    public static bEx? XOR(this Ex? left, params IReadOnlyList<Ex>? right)
    {
        if (left is null) return null;
        if (right is not { Count: > 0 }) return null;

        var i = 0;
        var l = left;

        while (i < right.Count)
            l = l.XOR(right[i++]);

        return (bEx)l;
    }

    public static bEx? XOR<T>(this Ex? left, params IReadOnlyList<T>? right)
    {
        if (left is null) return null;
        if (right is not { Count: > 0 }) return null;

        var i = 0;
        var l = left;
        while (i < right.Count)
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

    public static Ex ToNewExpression(this Type type, params IEnumerable<Ex> p) =>
        New(type.GetConstructor([.. p.Select(pp => pp.Type)]) ?? throw new InvalidOperationException("Конструктор не найден"));

    public static Ex ToNewExpression<T>(this Type type, params IEnumerable<T> p) =>
        New(type.GetConstructor([.. p.Select(pp => pp!.GetType())])
            ?? throw new InvalidOperationException("Конструктор не найден"));

    public static pEx ParameterOf(this string ParameterName, Type type) => Parameter(type, ParameterName);
    public static pEx ParameterOf<T>(this string ParameterName) => Parameter(typeof(T), ParameterName);

    public static mcEx GetCall(this Ex obj, string method, IEnumerable<Ex> arg)
        => Call(obj, method, [.. (arg = [.. arg]).Select(a => a.Type)], (Ex[])arg);

    public static mcEx GetCall(this Ex obj, string method, params Ex[] arg)
        => Call(obj, method, [.. arg.Select(a => a.Type)], arg);

    public static mcEx GetCall(this Ex obj, MethodInfo method, params IEnumerable<Ex> arg) => Call(obj, method, arg);

    public static mcEx GetCall(this Ex obj, MethodInfo method) => Call(obj, method);

    public static mcEx GetCall(this Ex obj, Delegate d, IEnumerable<Ex> arg) => obj.GetCall(d.Method, arg);

    public static mcEx GetCall(this Ex obj, Delegate d, params Ex[] arg) => obj.GetCall(d.Method, arg);

    public static mcEx GetCall(this Ex obj, Delegate d) => obj.GetCall(d.Method);

    public static InvocationExpression GetInvoke(this Ex d, params IEnumerable<Ex> arg) => Invoke(d, arg);

    public static iEx ArrayAccess(this Ex d, params IEnumerable<Ex> arg) => Ex.ArrayAccess(d, arg);

    public static mcEx ArrayIndex(this Ex d, params IEnumerable<Ex> arg) => Ex.ArrayIndex(d, arg);

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

    public static Expression<TDelegate> CreateLambda<TDelegate>(this Ex body, params IEnumerable<pEx> p) => Lambda<TDelegate>(body, p);
    public static lEx CreateLambda(this Ex body, params pEx[] p) => Lambda(body, p);
    public static lEx CreateLambda(this Ex body, Type DelegateType, params IEnumerable<pEx> p) => Lambda(DelegateType, body, p);

    public static TDelegate CompileTo<TDelegate>(this Ex body, params IEnumerable<pEx> p) => body.CreateLambda<TDelegate>(p).Compile();

    public static Ex CloneExpression(this Ex expr)
    {
        var visitor = new CloningVisitor();
        return visitor.Visit(expr).NotNull();
    }

    public static Ex[]? CloneArray(this Ex[]? expr)
    {
        if (expr is null) return null;
        var visitor = new CloningVisitor();
        var result = new Ex[expr.Length];
        for (var i = 0; i < result.Length; i++)
            result[i] = visitor.Visit(expr[i]).NotNull();
        return result;
    }
    public static Ex[,]? CloneArray(this Ex[,]? expr)
    {
        if (expr is null) return null;
        var visitor = new CloningVisitor();

        var n = expr.GetLength(0);
        var m = expr.GetLength(1);
        var result = new Ex[n, m];
        for (var i = 0; i < n; i++)
            for (var j = 0; j < m; j++)
                result[i, j] = visitor.Visit(expr[i, j]).NotNull();
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

    public static MethodCallExpression GetAbs(this Ex x) => Call(((Func<double, double>)Math.Abs).Method, x);
}