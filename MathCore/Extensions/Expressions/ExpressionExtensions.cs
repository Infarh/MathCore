using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MathCore.Annotations;
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

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace MathCore.Extensions.Expressions
{
    public static class ExpressionExtensions
    {
        #region Types

        private sealed class SubstExpressionVisitor : ExpressionVisitorEx
        {
            #region Properties

            public pEx ParamExpressionToSubstitute { private get; set; }

            public lEx SubstExpression { private get; set; }

            #endregion

            #region Methods

            public new Ex Visit(Ex exp) => base.Visit(exp);

            protected override Ex VisitUnary([NotNull] uEx node)
                => node.Operand == ParamExpressionToSubstitute
                    ? Ex.MakeUnary(node.NodeType, SubstExpression.Body, node.Type)
                    : base.VisitUnary(node);

            protected override Ex VisitMethodCall([NotNull] mcEx node)
            {
                if (node.Arguments.Count(expr => expr == ParamExpressionToSubstitute) == 0)
                    return base.VisitMethodCall(node);
                var arguments = node.Arguments
                   .Select(arg => arg == ParamExpressionToSubstitute ? SubstExpression.Body : Visit(arg))
                    .ToList();
                return Call(node.Object, node.Method, arguments);
            }

            protected override Ex VisitBinary([NotNull] bEx node)
            {
                Ex left, right;
                var subst_left = false;
                var subst_right = false;

                if (node.Left == ParamExpressionToSubstitute)
                {
                    left = SubstExpression.Body;
                    subst_left = true;
                }
                else
                    left = node.Left;

                if (node.Right == ParamExpressionToSubstitute)
                {
                    right = SubstExpression.Body;
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

        /// <exception cref="FormatException">Количество аргументов подстановки не равно 1, или во входном выражении отсутсвуте подставляемый параметр</exception>
        [CanBeNull]
        public static lEx Substitute([NotNull] this lEx Expr, [NotNull] lEx Substitution)
        {
            var parameters = Expr.Parameters;
            var substitute_parameters = Substitution.Parameters;
            if (substitute_parameters.Count != 1)
                throw new FormatException("Количество аргументов подстановки не равно 1");
            var SubstituteParameter = substitute_parameters[0];
            if (!parameters.Contains(p => p.Name == SubstituteParameter.Name && p.Type == SubstituteParameter.Type))
                throw new FormatException("Во входном выражении отсутсвуте подставляемый параметр");

            var visitor = new SubstitutionVisitor(Substitution);
            var result = visitor.Visit(Expr);

            return result as lEx;
        }

        public static lEx Substitute<TDelegate>
        (
            [NotNull] this lEx MainEx,
            string ParameterName,
            [NotNull] Expression<TDelegate> SubstExpression
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
                    SubstExpression = SubstExpression,
                    ParamExpressionToSubstitute = main_parameter
                };

            return (lEx)visitor.Visit(Lambda(MainEx.Body, pars));
        }

        [NotNull] public static mEx GetProperty([NotNull] this Ex obj, [NotNull] string PropertyName) => Property(obj, PropertyName);

        [NotNull] public static mEx GetProperty(this Ex obj, [NotNull] PropertyInfo Info) => Property(obj, Info);

        [NotNull] public static mEx GetField([NotNull] this Ex obj, [NotNull] string FieldName) => Field(obj, FieldName);

        [NotNull] public static bEx Assign([NotNull] this Ex dest, [NotNull] Ex source) => Ex.Assign(dest, source);
        [NotNull] public static bEx Assign<T>([NotNull] this Ex dest, Ex source) => Ex.Assign(dest, source.ToExpression());

        [NotNull] public static bEx AssignTo([NotNull] this Ex source, [NotNull] Ex dest) => Ex.Assign(dest, source);

        [NotNull] public static uEx Negate([NotNull] this Ex obj) => Ex.Negate(obj);

        [NotNull] public static bEx AddAssign([NotNull] this Ex left, [NotNull] Ex right) => Ex.AddAssign(left, right);
        [NotNull] public static bEx AddAssign<T>([NotNull] this Ex left, T right) => Ex.AddAssign(left, right.ToExpression());

        private static bool IsNumeric([NotNull] this Ex ex) => ex.Type.IsNumeric();

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

        [NotNull]
        private static Ex TryConvert(this Ex a, [NotNull] ref Ex b)
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

        [NotNull]
        public static bEx AddWithConversion([NotNull] this Ex left, Ex right) =>
            !left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type
                ? left.Add(right)
                : left.TryConvert(ref right).Add(right);

        [NotNull] public static bEx Add([NotNull] this Ex left, [NotNull] Ex right) => Ex.Add(left, right);
        [NotNull] public static bEx Add([NotNull] this Ex left, Ex right, bool conversion) => conversion ? left.AddWithConversion(right) : Ex.Add(left, right);
        [NotNull] public static bEx Add([NotNull] this Ex left, int right) => left.AddWithConversion(right.ToExpression());
        [NotNull] public static bEx Add([NotNull] this Ex left, double right) => left.AddWithConversion(right.ToExpression());
        [NotNull] public static bEx Add([NotNull] this Ex left, decimal right) => left.AddWithConversion(right.ToExpression());
        [NotNull] public static bEx Add([NotNull] this Ex left, string right) => left.Add(right.ToExpression());
        [NotNull] public static bEx Add<T>([NotNull] this Ex left, T right) => Ex.Add(left, right.ToExpression());

        [CanBeNull]
        public static bEx Add<T>([CanBeNull] this Ex left, [CanBeNull] params T[] right)
        {
            var i = 0;
            Ex l;
            if (left != null) l = left;
            else if (right is null || right.Length == i) return null;
            else l = right[i++].ToExpression();
            while (i < right.Length)
                l = l.AddWithConversion(right[i++].ToExpression());
            return (bEx)l;
        }

        [NotNull] public static bEx SubtractAssign([NotNull] this Ex left, [NotNull] Ex right) => Ex.SubtractAssign(left, right);
        [NotNull] public static bEx SubtractAssign<T>([NotNull] this Ex left, T right) => Ex.SubtractAssign(left, right.ToExpression());

        [NotNull]
        public static bEx SubtractWithConversion([NotNull] this Ex left, Ex right) =>
            !left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type
                ? left.Subtract(right)
                : left.TryConvert(ref right).Subtract(right);

        [NotNull] public static bEx Subtract([NotNull] this Ex left, [NotNull] Ex right) => Ex.Subtract(left, right);
        [NotNull] public static bEx Subtract([NotNull] this Ex left, Ex right, bool conversion) => conversion ? left.SubtractWithConversion(right) : Ex.Subtract(left, right);
        [NotNull] public static bEx Subtract([NotNull] this Ex left, int right) => left.SubtractWithConversion(right.ToExpression());
        [NotNull] public static bEx Subtract([NotNull] this Ex left, double right) => left.SubtractWithConversion(right.ToExpression());
        [NotNull] public static bEx Subtract([NotNull] this Ex left, decimal right) => left.SubtractWithConversion(right.ToExpression());
        [NotNull] public static bEx Subtract<T>([NotNull] this Ex left, T right) => Ex.Subtract(left, right.ToExpression());

        [NotNull] public static bEx MultiplyAssign([NotNull] this Ex left, [NotNull] Ex right) => Ex.MultiplyAssign(left, right);
        [NotNull] public static bEx MultiplyAssign<T>([NotNull] this Ex left, T right) => Ex.MultiplyAssign(left, right.ToExpression());

        public static bEx MultiplyWithConversion([NotNull] this Ex left, Ex right) =>
            !left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type
                ? left.Multiply(right)
                : left.TryConvert(ref right).Multiply(right);

        public static bEx Multiply([NotNull] this Ex left, Ex right, bool conversion = false) => conversion ? left.MultiplyWithConversion(right) : Ex.Multiply(left, right);
        public static bEx Multiply([NotNull] this Ex left, int right) => left.MultiplyWithConversion(right.ToExpression());
        public static bEx Multiply([NotNull] this Ex left, double right) => left.MultiplyWithConversion(right.ToExpression());

        public static bEx Multiply([CanBeNull] this Ex left, [CanBeNull] params Ex[] right)
        {
            var i = 0;
            Ex l;
            if (left != null) l = left;
            else if (right is null || right.Length == i) return null;
            else l = right[i++];
            while (i < right.Length)
                l = l.MultiplyWithConversion(right[i++]);
            return (bEx)l;
        }

        public static bEx Multiply<T>([CanBeNull] this Ex left, [CanBeNull] params T[] right)
        {
            var i = 0;
            Ex l;
            if (left != null) l = left;
            else if (right is null || right.Length == i) return null;
            else l = right[i++].ToExpression();
            while (i < right.Length)
                l = l.MultiplyWithConversion(right[i++].ToExpression());
            return (bEx)l;
        }

        [NotNull] public static bEx DivideAssign([NotNull] this Ex left, [NotNull] Ex right) => Ex.DivideAssign(left, right);
        [NotNull] public static bEx DivideAssign<T>([NotNull] this Ex left, T right) => Ex.DivideAssign(left, right.ToExpression());

        [NotNull]
        public static bEx DivideWithConversion([NotNull] this Ex left, Ex right)
        {
            if (!left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type) return left.Divide(right);
            return left.TryConvert(ref right).Divide(right);
        }

        [NotNull] public static bEx Divide([NotNull] this Ex left, [NotNull] Ex right) => Ex.Divide(left, right);
        [NotNull] public static bEx Divide<T>([NotNull] this Ex left, T right) => Ex.Divide(left, right.ToExpression());
        [NotNull] public static bEx Divide([NotNull] this Ex left, Ex right, bool conversion) => conversion ? left.DivideWithConversion(right) : Ex.Divide(left, right);
        [NotNull] public static bEx Divide([NotNull] this Ex left, int right) => left.DivideWithConversion(right.ToExpression());
        [NotNull] public static bEx Divide([NotNull] this Ex left, double right) => left.DivideWithConversion(right.ToExpression());

        [NotNull] public static bEx PowerAssign([NotNull] this Ex left, [NotNull] Ex right) => Ex.PowerAssign(left, right);
        [NotNull] public static bEx PowerAssign<T>([NotNull] this Ex left, T right) => Ex.PowerAssign(left, right.ToExpression());

        [NotNull]
        public static bEx PowerWithConversion([NotNull] this Ex left, Ex right)
        {
            if (!left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type) return left.Power(right);
            return left.TryConvert(ref right).Power(right);
        }
        [NotNull]
        public static bEx PowerOfWithConversion(this Ex left, [NotNull] Ex right)
        {
            if (!left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type) return right.Power(left);
            return right.TryConvert(ref left).Power(left);
        }

        [NotNull] public static bEx Power([NotNull] this Ex left, [NotNull] Ex right) => Ex.Power(left, right);
        [NotNull] public static bEx Power<T>([NotNull] this Ex left, T right) => Ex.Power(left, right.ToExpression());
        [NotNull] public static bEx Power([NotNull] this Ex left, Ex right, bool conversion) => conversion ? left.PowerWithConversion(right) : Ex.Power(left, right);
        [NotNull] public static bEx PowerOf([NotNull] this Ex left, [NotNull] Ex right) => Ex.Power(right, left);
        [NotNull] public static bEx PowerOf<T>([NotNull] this Ex left, T right) => Ex.Power(right.ToExpression(), left);
        [NotNull] public static bEx PowerOf(this Ex left, [NotNull] Ex right, bool conversion) => conversion ? left.PowerOfWithConversion(right) : Ex.Power(right, left);
        [NotNull] public static bEx Power([NotNull] this Ex left, int right) => left.PowerWithConversion(right.ToExpression());
        [NotNull] public static bEx PowerOf(this Ex left, int right) => right.ToExpression().PowerWithConversion(left);
        [NotNull] public static bEx Power([NotNull] this Ex left, double right) => left.PowerWithConversion(right.ToExpression());
        [NotNull] public static bEx PowerOf(this Ex left, double right) => right.ToExpression().PowerWithConversion(left);

        public static mcEx Sqrt(this Ex expr) => MathExpression.Sqrt(expr);
        public static bEx SqrtPower(this Ex expr) => MathExpression.SqrtPower(expr);
        public static bEx SqrtPower(this Ex expr, Ex power) => MathExpression.SqrtPower(expr, power);
        public static bEx SqrtPower<T>(this Ex expr, T power) => MathExpression.SqrtPower(expr, power.ToExpression());

        [NotNull] public static bEx IsEqual([NotNull] this Ex left, [NotNull] Ex right) => Equal(left, right);
        [NotNull] public static bEx IsEqual<T>([NotNull] this Ex left, T right) => Equal(left, right.ToExpression());

        [NotNull] public static bEx IsNotEqual([NotNull] this Ex left, [NotNull] Ex right) => NotEqual(left, right);
        [NotNull] public static bEx IsNotEqual<T>([NotNull] this Ex left, T right) => NotEqual(left, right.ToExpression());

        [NotNull] public static bEx IsGreaterThan([NotNull] this Ex left, [NotNull] Ex right) => GreaterThan(left, right);
        [NotNull] public static bEx IsGreaterThan<T>([NotNull] this Ex left, T right) => GreaterThan(left, right.ToExpression());

        [NotNull] public static bEx IsGreaterThanOrEqual([NotNull] this Ex left, [NotNull] Ex right) => GreaterThanOrEqual(left, right);
        [NotNull] public static bEx IsGreaterThanOrEqual<T>([NotNull] this Ex left, T right) => GreaterThanOrEqual(left, right.ToExpression());

        [NotNull] public static bEx IsLessThan([NotNull] this Ex left, [NotNull] Ex right) => LessThan(left, right);
        [NotNull] public static bEx IsLessThan<T>([NotNull] this Ex left, T right) => LessThan(left, right.ToExpression());

        [NotNull] public static bEx IsLessThanOrEqual([NotNull] this Ex left, [NotNull] Ex right) => LessThanOrEqual(left, right);
        [NotNull] public static bEx IsLessThanOrEqual<T>([NotNull] this Ex left, T right) => LessThanOrEqual(left, right.ToExpression());

        [NotNull] public static bEx And([NotNull] this Ex left, [NotNull] Ex right) => Ex.And(left, right);
        [NotNull] public static bEx And<T>([NotNull] this Ex left, T right) => Ex.And(left, right.ToExpression());

        [NotNull] public static bEx AndAssign([NotNull] this Ex left, [NotNull] Ex right) => Ex.AndAssign(left, right);
        [NotNull] public static bEx AndAssign<T>([NotNull] this Ex left, T right) => Ex.AndAssign(left, right.ToExpression());

        [NotNull] public static bEx OrAssign([NotNull] this Ex left, [NotNull] Ex right) => Ex.OrAssign(left, right);
        [NotNull] public static bEx OrAssign<T>([NotNull] this Ex left, T right) => Ex.OrAssign(left, right.ToExpression());

        [NotNull] public static bEx Or([NotNull] this Ex left, [NotNull] Ex right) => Ex.Or(left, right);
        [NotNull] public static bEx Or<T>([NotNull] this Ex left, T right) => Ex.Or(left, right.ToExpression());
        [NotNull] public static uEx Not([NotNull] this Ex d) => Ex.Not(d);

        [NotNull] public static bEx AndLazy([NotNull] this Ex left, [NotNull] Ex right) => AndAlso(left, right);
        [NotNull] public static bEx AndLazy<T>([NotNull] this Ex left, T right) => AndAlso(left, right.ToExpression());

        [NotNull] public static bEx OrLazy([NotNull] this Ex left, [NotNull] Ex right) => OrElse(left, right);
        [NotNull] public static bEx OrLazy<T>([NotNull] this Ex left, T right) => OrElse(left, right.ToExpression());

        [NotNull] public static bEx Coalesce([NotNull] this Ex first, [NotNull] Ex second) => Ex.Coalesce(first, second);
        [NotNull] public static bEx Coalesce<T>([NotNull] this Ex first, T second) => Ex.Coalesce(first, second.ToExpression());

        public static bEx Coalesce([CanBeNull] this Ex left, [CanBeNull] params Ex[] right)
        {
            var i = 0;
            Ex l;
            if (left != null) l = left;
            else if (right is null || right.Length == i) return null;
            else l = right[i++];
            while (i < right.Length)
                l = l.Coalesce(right[i++]);
            return (bEx)l;
        }

        [CanBeNull]
        public static bEx Coalesce<T>([CanBeNull] this Ex left, [CanBeNull] params T[] right)
        {
            var i = 0;
            Ex l;
            if (left != null) l = left;
            else if (right is null || right.Length == i) return null;
            else l = right[i++].ToExpression();
            while (i < right.Length)
                l = l.Coalesce(right[i++]);
            return (bEx)l;
        }

        [NotNull] public static bEx XORAssign([NotNull] this Ex left, [NotNull] Ex right) => ExclusiveOrAssign(left, right);
        [NotNull] public static bEx XORAssign<T>([NotNull] this Ex left, T right) => ExclusiveOrAssign(left, right.ToExpression());

        [NotNull] public static bEx XOR([NotNull] this Ex left, [NotNull] Ex right) => ExclusiveOr(left, right);
        [NotNull] public static bEx XOR<T>([NotNull] this Ex left, T right) => ExclusiveOr(left, right.ToExpression());

        public static bEx XOR([CanBeNull] this Ex left, [CanBeNull] params Ex[] right)
        {
            var i = 0;
            Ex l;
            if (left != null) l = left;
            else if (right is null || right.Length == i) return null;
            else l = right[i++];
            while (i < right.Length)
                l = l.XOR(right[i++]);
            return (bEx)l;
        }

        [CanBeNull]
        public static bEx XOR<T>([CanBeNull] this Ex left, [CanBeNull] params T[] right)
        {
            var i = 0;
            Ex l;
            if (left != null) l = left;
            else if (right is null || right.Length == i) return null;
            else l = right[i++].ToExpression();
            while (i < right.Length)
                l = l.XOR(right[i++]);
            return (bEx)l;
        }

        [NotNull] public static bEx ModuloAssign([NotNull] this Ex left, [NotNull] Ex right) => Ex.ModuloAssign(left, right);
        [NotNull] public static bEx ModuloAssign<T>([NotNull] this Ex left, Ex right) => Ex.ModuloAssign(left, right.ToExpression());

        [NotNull] public static bEx Modulo([NotNull] this Ex left, [NotNull] Ex right) => Ex.Modulo(left, right);
        [NotNull] public static bEx Modulo<T>([NotNull] this Ex left, Ex right) => Ex.Modulo(left, right.ToExpression());

        [NotNull] public static bEx LeftShiftAssign([NotNull] this Ex left, [NotNull] Ex right) => Ex.LeftShiftAssign(left, right);
        [NotNull] public static bEx LeftShiftAssign<T>([NotNull] this Ex left, T right) => Ex.LeftShiftAssign(left, right.ToExpression());

        [NotNull] public static bEx LeftShift([NotNull] this Ex left, [NotNull] Ex right) => Ex.LeftShift(left, right);
        [NotNull] public static bEx LeftShift<T>([NotNull] this Ex left, T right) => Ex.LeftShift(left, right.ToExpression());

        [NotNull] public static bEx RightShiftAssign([NotNull] this Ex left, [NotNull] Ex right) => Ex.RightShiftAssign(left, right);
        [NotNull] public static bEx RightShiftAssign<T>([NotNull] this Ex left, T right) => Ex.RightShiftAssign(left, right.ToExpression());

        [NotNull] public static bEx RightShift([NotNull] this Ex left, [NotNull] Ex right) => Ex.RightShift(left, right);
        [NotNull] public static bEx RightShift<T>([NotNull] this Ex left, T right) => Ex.RightShift(left, right.ToExpression());

        [NotNull] public static bEx IsRefEqual([NotNull] this Ex left, [NotNull] Ex right) => ReferenceEqual(left, right);
        [NotNull] public static bEx IsRefEqual<T>([NotNull] this Ex left, T right) => ReferenceEqual(left, right.ToExpression());

        [NotNull] public static bEx IsIsRefEqual([NotNull] this Ex left, [NotNull] Ex right) => ReferenceNotEqual(left, right);
        [NotNull] public static bEx IsIsRefEqual<T>([NotNull] this Ex left, T right) => ReferenceNotEqual(left, right.ToExpression());

        [NotNull] public static Ex Condition([NotNull] this Ex Condition, [NotNull] Ex Then, [NotNull] Ex Else) => Ex.Condition(Condition, Then, Else);
        [NotNull] public static Ex ConditionWithResult<T>([NotNull] this Ex Condition, T Then, T Else) => Ex.Condition(Condition, Then.ToExpression(), Else.ToExpression());

        [NotNull] public static Ex ToNewExpression([NotNull] this Type type) => New(type.GetConstructor(Type.EmptyTypes));

        [NotNull] public static Ex ToNewExpression([NotNull] this Type type, [NotNull] params Ex[] p) => New(type.GetConstructor(p.Select(pp => pp.Type).ToArray()));
        [NotNull] public static Ex ToNewExpression<T>([NotNull] this Type type, [NotNull] params T[] p) => New(type.GetConstructor(p.Select(pp => pp.GetType()).ToArray()));

        [NotNull] public static pEx ParameterOf(this string ParameterName, [NotNull] Type type) => Parameter(type, ParameterName);

        [NotNull]
        public static mcEx GetCall([NotNull] this Ex obj, [NotNull] string method, IEnumerable<Ex> arg)
            => Call(obj, method, (arg = arg.ToArray()).Select(a => a.Type).ToArray(), (Ex[])arg);

        [NotNull]
        public static mcEx GetCall([NotNull] this Ex obj, [NotNull] string method, [NotNull] params Ex[] arg)
            => Call(obj, method, arg.Select(a => a.Type).ToArray(), arg);

        [NotNull] public static mcEx GetCall(this Ex obj, [NotNull] MethodInfo method, IEnumerable<Ex> arg) => Call(obj, method, arg);

        [NotNull] public static mcEx GetCall(this Ex obj, [NotNull] MethodInfo method, params Ex[] arg) => Call(obj, method, arg);

        [NotNull] public static mcEx GetCall(this Ex obj, [NotNull] MethodInfo method) => Call(obj, method);

        [NotNull] public static mcEx GetCall(this Ex obj, [NotNull] Delegate d, IEnumerable<Ex> arg) => obj.GetCall(d.Method, arg);

        [NotNull] public static mcEx GetCall(this Ex obj, [NotNull] Delegate d, params Ex[] arg) => obj.GetCall(d.Method, arg);

        [NotNull] public static mcEx GetCall(this Ex obj, [NotNull] Delegate d) => obj.GetCall(d.Method);

        [NotNull] public static InvocationExpression GetInvoke([NotNull] this Ex d, IEnumerable<Ex> arg) => Invoke(d, arg);

        [NotNull] public static InvocationExpression GetInvoke([NotNull] this Ex d, params Ex[] arg) => Invoke(d, arg);

        [NotNull] public static iEx ArrayAccess([NotNull] this Ex d, [NotNull] IEnumerable<Ex> arg) => Ex.ArrayAccess(d, arg);

        [NotNull] public static iEx ArrayAccess([NotNull] this Ex d, [NotNull] params Ex[] arg) => Ex.ArrayAccess(d, arg);

        [NotNull] public static mcEx ArrayIndex([NotNull] this Ex d, [NotNull] IEnumerable<Ex> arg) => Ex.ArrayIndex(d, arg);

        [NotNull] public static mcEx ArrayIndex([NotNull] this Ex d, [NotNull] params Ex[] arg) => Ex.ArrayIndex(d, arg);

        [NotNull] public static uEx ArrayLength([NotNull] this Ex d) => Ex.ArrayLength(d);
        [NotNull] public static uEx ConvertTo([NotNull] this Ex d, [NotNull] Type type) => Convert(d, type);
        [NotNull] public static uEx Increment([NotNull] this Ex d) => Ex.Increment(d);

        [NotNull] public static bEx Inverse([NotNull] this Ex expr) => 1.ToExpression().Divide(expr);

        [NotNull] public static uEx Decrement([NotNull] this Ex d) => Ex.Decrement(d);
        [NotNull] public static uEx IsTrue([NotNull] this Ex d) => Ex.IsTrue(d);
        [NotNull] public static uEx IsFalse([NotNull] this Ex d) => Ex.IsFalse(d);
        [NotNull] public static uEx Quote([NotNull] this Ex d) => Ex.Quote(d);
        [NotNull] public static uEx OnesComplement([NotNull] this Ex d) => Ex.OnesComplement(d);
        [NotNull] public static DefaultExpression Default([NotNull] this Type d) => Ex.Default(d);
        [NotNull] public static uEx PostIncrementAssign([NotNull] this Ex d) => Ex.PostIncrementAssign(d);
        [NotNull] public static uEx PreIncrementAssign([NotNull] this Ex d) => Ex.PreIncrementAssign(d);
        [NotNull] public static uEx PostDecrementAssign([NotNull] this Ex d) => Ex.PostDecrementAssign(d);
        [NotNull] public static uEx PreDecrementAssign([NotNull] this Ex d) => Ex.PreDecrementAssign(d);
        [NotNull] public static uEx Throw([NotNull] this Ex d) => Ex.Throw(d);
        [NotNull] public static uEx Throw([NotNull] this Ex d, [NotNull] Type type) => Ex.Throw(d, type);
        [NotNull] public static uEx TypeAs([NotNull] this Ex d, [NotNull] Type type) => Ex.TypeAs(d, type);
        [NotNull] public static TypeBinaryExpression TypeIs([NotNull] this Ex d, [NotNull] Type type) => Ex.TypeIs(d, type);
        [NotNull] public static uEx Unbox([NotNull] this Ex d, [NotNull] Type type) => Ex.Unbox(d, type);
        [NotNull] public static uEx UnaryPlus([NotNull] this Ex d) => Ex.UnaryPlus(d);
        [NotNull] public static uEx MakeUnary([NotNull] this Ex d, ExpressionType uType, Type type) => Ex.MakeUnary(uType, d, type);
        [NotNull] public static bEx MakeUnary([NotNull] this Ex left, [NotNull] Ex right, ExpressionType uType) => MakeBinary(uType, left, right);

        [NotNull] public static Expression<TDelegate> CreateLambda<TDelegate>([NotNull] this Ex body, params pEx[] p) => Lambda<TDelegate>(body, p);
        [NotNull] public static lEx CreateLambda([NotNull] this Ex body, params pEx[] p) => Lambda(body, p);

        public static Ex CloneExpression(this Ex expr)
        {
            var visitor = new CloningVisitor();
            return visitor.Visit(expr);
        }

        [CanBeNull]
        public static Ex[] CloneArray([CanBeNull] this Ex[] expr)
        {
            if (expr is null) return null;
            var visitor = new CloningVisitor();
            var result = new Ex[expr.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = visitor.Visit(expr[i]);
            return result;
        }
        [CanBeNull]
        public static Ex[,] CloneArray([CanBeNull] this Ex[,] expr)
        {
            if (expr is null) return null;
            var visitor = new CloningVisitor();

            var N = expr.GetLength(0);
            var M = expr.GetLength(1);
            var result = new Ex[N, M];
            for (var i = 0; i < N; i++)
                for (var j = 0; j < M; j++)
                    result[i, j] = visitor.Visit(expr[i, j]);
            return result;
        }

        #endregion

        [NotNull]
        public static Expression<Func<T, bool>> IsEqual<T>([NotNull] this Expression<Func<T, bool>> Expr) =>
            Lambda<Func<T, bool>>(Expr.Body.Not(), Expr.Parameters);

        [NotNull]
        public static Expression<Func<T, bool>> IsEqual<T, TValue>([NotNull] this Expression<Func<T, TValue>> Expr, TValue Value) =>
            Lambda<Func<T, bool>>(Expr.Body.IsEqual(Value), Expr.Parameters);

        [NotNull]
        public static Expression<Func<T, bool>> IsNotEqual<T, TValue>([NotNull] this Expression<Func<T, TValue>> Expr, TValue Value) =>
            Lambda<Func<T, bool>>(Expr.Body.IsNotEqual(Value), Expr.Parameters);

        [NotNull]
        public static Expression<Func<T, bool>> IsGreaterThan<T, TValue>([NotNull] this Expression<Func<T, TValue>> Expr, TValue Value) =>
            Lambda<Func<T, bool>>(Expr.Body.IsGreaterThan(Value), Expr.Parameters);

        [NotNull]
        public static Expression<Func<T, bool>> IsLessThan<T, TValue>([NotNull] this Expression<Func<T, TValue>> Expr, TValue Value) =>
            Lambda<Func<T, bool>>(Expr.Body.IsLessThan(Value), Expr.Parameters);

        [NotNull]
        public static Expression<Func<T, bool>> IsGreaterThanOrEqual<T, TValue>([NotNull] this Expression<Func<T, TValue>> Expr, TValue Value) =>
            Lambda<Func<T, bool>>(Expr.Body.IsGreaterThanOrEqual(Value), Expr.Parameters);

        [NotNull]
        public static Expression<Func<T, bool>> IsLessThanOrEqual<T, TValue>([NotNull] this Expression<Func<T, TValue>> Expr, TValue Value) =>
            Lambda<Func<T, bool>>(Expr.Body.IsLessThanOrEqual(Value), Expr.Parameters);

        [NotNull]
        public static Expression<Func<T, TValue>> Add<T, TValue>([NotNull] this Expression<Func<T, TValue>> Expr, TValue Value) =>
            Lambda<Func<T, TValue>>(Expr.Body.Add(Value), Expr.Parameters);

        [NotNull]
        public static Expression<Func<T, TValue>> Subtract<T, TValue>([NotNull] this Expression<Func<T, TValue>> Expr, TValue Value) =>
            Lambda<Func<T, TValue>>(Expr.Body.Subtract(Value), Expr.Parameters);

        [NotNull]
        public static Expression<Func<T, TValue>> Multiply<T, TValue>([NotNull] this Expression<Func<T, TValue>> Expr, TValue Value) =>
            Lambda<Func<T, TValue>>(Expr.Body.Multiply(Value), Expr.Parameters);

        [NotNull]
        public static Expression<Func<T, TValue>> Divide<T, TValue>([NotNull] this Expression<Func<T, TValue>> Expr, TValue Value) =>
            Lambda<Func<T, TValue>>(Expr.Body.Divide(Value), Expr.Parameters);

        [NotNull]
        public static Expression<Func<T, TValue>> Power<T, TValue>([NotNull] this Expression<Func<T, TValue>> Expr, TValue Value) =>
            Lambda<Func<T, TValue>>(Expr.Body.Power(Value), Expr.Parameters);

        public static Ex Simplify(this Ex expr)
        {
            var visitor = new ExpressionRebuilder();
            visitor.BinaryVisited += ExpressionSimplifierRules.Binary;

            return visitor.Visit(expr);
        }

        private static class ExpressionSimplifierRules
        {
            [NotNull]
            public static Ex Binary(object Sender, [NotNull] EventArgs<bEx> Args)
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

            private static bool IsNumeric(object value) =>
                value is byte
                || value is sbyte
                || value is short
                || value is ushort
                || value is int
                || value is uint
                || value is long
                || value is ulong
                || value is float
                || value is double
                || value is Complex
                || value is Vector2D
                || value is Vector3D
                ;

            private static bool IsZero(object value) =>
                value is byte && ((byte)0).Equals(value)
                || value is sbyte && ((sbyte)0).Equals(value)
                || value is short && ((short)0).Equals(value)
                || value is ushort && ((ushort)0).Equals(value)
                || value is int && 0.Equals(value)
                || value is uint && 0u.Equals(value)
                || value is long && 0L.Equals(value)
                || value is ulong && 0ul.Equals(value)
                || value is float && 0f.Equals(value)
                || value is double && 0d.Equals(value)
                || value is Complex && ((Complex)0).Equals(value)
                || value is Vector2D && ((Vector2D)0).Equals(value)
                || value is Vector3D && ((Vector3D)0).Equals(value)
                ;

            private static bool IsUnit(object value) =>
                value is byte && ((byte)1).Equals(value)
                || value is sbyte && ((sbyte)1).Equals(value)
                || value is short && ((short)1).Equals(value)
                || value is ushort && ((ushort)1).Equals(value)
                || value is int && 1.Equals(value)
                || value is uint && 1u.Equals(value)
                || value is long && 1L.Equals(value)
                || value is ulong && 1ul.Equals(value)
                || value is float && 1f.Equals(value)
                || value is double && 1d.Equals(value)
                || value is Complex && Complex.Real.Equals(value)
                ;

            #endregion

            [NotNull]
            private static Ex MultiplySimplify([NotNull] bEx expr)
            {
                //var is_checked = expr.NodeType == ExpressionType.MultiplyChecked;
                if (IsZero((expr.Left as cEx)?.Value)) return expr.Left;
                if (IsUnit((expr.Left as cEx)?.Value)) return expr.Right;

                if (IsZero((expr.Right as cEx)?.Value)) return expr.Right;
                if (IsUnit((expr.Right as cEx)?.Value)) return expr.Left;

                return MultiplyValues((expr.Left as cEx)?.Value, (expr.Right as cEx)?.Value) ?? expr;
            }

            [CanBeNull]
            private static Ex MultiplyValues(object left, object right)
            {
                if (!IsNumeric(left) || !IsNumeric(right)) return null;
                if (left is byte)
                {
                    if (right is byte) return ((byte)left * (byte)right).ToExpression();
                    if (right is sbyte) return ((byte)left * (sbyte)right).ToExpression();
                    if (right is short) return ((byte)left * (short)right).ToExpression();
                    if (right is ushort) return ((byte)left * (ushort)right).ToExpression();
                    if (right is int) return ((byte)left * (int)right).ToExpression();
                    if (right is uint) return ((byte)left * (uint)right).ToExpression();
                    if (right is long) return ((byte)left * (long)right).ToExpression();
                    if (right is ulong) return ((byte)left * (ulong)right).ToExpression();
                    if (right is float) return ((byte)left * (float)right).ToExpression();
                    if (right is double) return ((byte)left * (double)right).ToExpression();
                    if (right is Complex) return ((byte)left * (Complex)right).ToExpression();
                    if (right is Vector2D) return ((byte)left * (Vector2D)right).ToExpression();
                    return ((byte)left * (right as Vector3D?))?.ToExpression();
                }
                if (left is sbyte)
                {
                    if (right is byte) return ((sbyte)left * (byte)right).ToExpression();
                    if (right is sbyte) return ((sbyte)left * (sbyte)right).ToExpression();
                    if (right is short) return ((sbyte)left * (short)right).ToExpression();
                    if (right is ushort) return ((sbyte)left * (ushort)right).ToExpression();
                    if (right is int) return ((sbyte)left * (int)right).ToExpression();
                    if (right is uint) return ((sbyte)left * (uint)right).ToExpression();
                    if (right is long) return ((sbyte)left * (long)right).ToExpression();
                    //if (right is ulong) return ((sbyte)left * (ulong)right).ToExpression();
                    if (right is float) return ((sbyte)left * (float)right).ToExpression();
                    if (right is double) return ((sbyte)left * (double)right).ToExpression();
                    if (right is Complex) return ((sbyte)left * (Complex)right).ToExpression();
                    if (right is Vector2D) return ((sbyte)left * (Vector2D)right).ToExpression();
                    return ((sbyte)left * (right as Vector3D?))?.ToExpression();
                }
                if (left is short)
                {
                    if (right is byte) return ((short)left * (byte)right).ToExpression();
                    if (right is sbyte) return ((short)left * (sbyte)right).ToExpression();
                    if (right is short) return ((short)left * (short)right).ToExpression();
                    if (right is ushort) return ((short)left * (ushort)right).ToExpression();
                    if (right is int) return ((short)left * (int)right).ToExpression();
                    if (right is uint) return ((short)left * (uint)right).ToExpression();
                    if (right is long) return ((short)left * (long)right).ToExpression();
                    //if(right is ulong) return ((short)left * (ulong)right).ToExpression();
                    if (right is float) return ((short)left * (float)right).ToExpression();
                    if (right is double) return ((short)left * (double)right).ToExpression();
                    if (right is Complex) return ((short)left * (Complex)right).ToExpression();
                    if (right is Vector2D) return ((short)left * (Vector2D)right).ToExpression();
                    return ((short)left * (right as Vector3D?))?.ToExpression();
                }
                if (left is ushort)
                {
                    if (right is byte) return ((ushort)left * (byte)right).ToExpression();
                    if (right is sbyte) return ((ushort)left * (sbyte)right).ToExpression();
                    if (right is short) return ((ushort)left * (short)right).ToExpression();
                    if (right is ushort) return ((ushort)left * (ushort)right).ToExpression();
                    if (right is int) return ((ushort)left * (int)right).ToExpression();
                    if (right is uint) return ((ushort)left * (uint)right).ToExpression();
                    if (right is long) return ((ushort)left * (long)right).ToExpression();
                    if (right is ulong) return ((ushort)left * (ulong)right).ToExpression();
                    if (right is float) return ((ushort)left * (float)right).ToExpression();
                    if (right is double) return ((ushort)left * (double)right).ToExpression();
                    if (right is Complex) return ((ushort)left * (Complex)right).ToExpression();
                    if (right is Vector2D) return ((ushort)left * (Vector2D)right).ToExpression();
                    return ((ushort)left * (right as Vector3D?))?.ToExpression();
                }
                if (left is int)
                {
                    if (right is byte) return ((int)left * (byte)right).ToExpression();
                    if (right is sbyte) return ((int)left * (sbyte)right).ToExpression();
                    if (right is short) return ((int)left * (short)right).ToExpression();
                    if (right is ushort) return ((int)left * (ushort)right).ToExpression();
                    if (right is int) return ((int)left * (int)right).ToExpression();
                    if (right is uint) return ((int)left * (uint)right).ToExpression();
                    if (right is long) return ((int)left * (long)right).ToExpression();
                    //if(right is ulong) return ((int)left * (ulong)right).ToExpression();
                    if (right is float) return ((int)left * (float)right).ToExpression();
                    if (right is double) return ((int)left * (double)right).ToExpression();
                    if (right is Complex) return ((int)left * (Complex)right).ToExpression();
                    if (right is Vector2D) return ((int)left * (Vector2D)right).ToExpression();
                    return ((int)left * (right as Vector3D?))?.ToExpression();
                }
                if (left is uint)
                {
                    if (right is byte) return ((uint)left * (byte)right).ToExpression();
                    if (right is sbyte) return ((uint)left * (sbyte)right).ToExpression();
                    if (right is short) return ((uint)left * (short)right).ToExpression();
                    if (right is ushort) return ((uint)left * (ushort)right).ToExpression();
                    if (right is int) return ((uint)left * (int)right).ToExpression();
                    if (right is uint) return ((uint)left * (uint)right).ToExpression();
                    if (right is long) return ((uint)left * (long)right).ToExpression();
                    if (right is ulong) return ((uint)left * (ulong)right).ToExpression();
                    if (right is float) return ((uint)left * (float)right).ToExpression();
                    if (right is double) return ((uint)left * (double)right).ToExpression();
                    if (right is Complex) return ((uint)left * (Complex)right).ToExpression();
                    if (right is Vector2D) return ((uint)left * (Vector2D)right).ToExpression();
                    return ((uint)left * (right as Vector3D?))?.ToExpression();
                }
                if (left is long)
                {
                    if (right is byte) return ((long)left * (byte)right).ToExpression();
                    if (right is sbyte) return ((long)left * (sbyte)right).ToExpression();
                    if (right is short) return ((long)left * (short)right).ToExpression();
                    if (right is ushort) return ((long)left * (ushort)right).ToExpression();
                    if (right is int) return ((long)left * (int)right).ToExpression();
                    if (right is uint) return ((long)left * (uint)right).ToExpression();
                    if (right is long) return ((long)left * (long)right).ToExpression();
                    //if(right is ulong) return ((long)left * (ulong)right).ToExpression();
                    if (right is float) return ((long)left * (float)right).ToExpression();
                    if (right is double) return ((long)left * (double)right).ToExpression();
                    if (right is Complex) return ((long)left * (Complex)right).ToExpression();
                    if (right is Vector2D) return ((long)left * (Vector2D)right).ToExpression();
                    return ((long)left * (right as Vector3D?))?.ToExpression();
                }
                if (left is ulong)
                {
                    if (right is byte) return ((ulong)left * (byte)right).ToExpression();
                    //if(right is sbyte) return ((ulong)left * (sbyte)right).ToExpression();
                    //if(right is short) return ((ulong)left * (short)right).ToExpression();
                    if (right is ushort) return ((ulong)left * (ushort)right).ToExpression();
                    //if(right is int) return ((ulong)left * (int)right).ToExpression();
                    if (right is uint) return ((ulong)left * (uint)right).ToExpression();
                    //if(right is long) return ((ulong)left * (long)right).ToExpression();
                    if (right is ulong) return ((ulong)left * (ulong)right).ToExpression();
                    if (right is float) return ((ulong)left * (float)right).ToExpression();
                    if (right is double) return ((ulong)left * (double)right).ToExpression();
                    if (right is Complex) return ((ulong)left * (Complex)right).ToExpression();
                    if (right is Vector2D) return ((ulong)left * (Vector2D)right).ToExpression();
                    return ((ulong)left * (right as Vector3D?))?.ToExpression();
                }
                if (left is float)
                {
                    if (right is byte) return ((float)left * (byte)right).ToExpression();
                    if (right is sbyte) return ((float)left * (sbyte)right).ToExpression();
                    if (right is short) return ((float)left * (short)right).ToExpression();
                    if (right is ushort) return ((float)left * (ushort)right).ToExpression();
                    if (right is int) return ((float)left * (int)right).ToExpression();
                    if (right is uint) return ((float)left * (uint)right).ToExpression();
                    if (right is long) return ((float)left * (long)right).ToExpression();
                    if (right is ulong) return ((float)left * (ulong)right).ToExpression();
                    if (right is float) return ((float)left * (float)right).ToExpression();
                    if (right is double) return ((float)left * (double)right).ToExpression();
                    if (right is Complex) return ((float)left * (Complex)right).ToExpression();
                    if (right is Vector2D) return ((float)left * (Vector2D)right).ToExpression();
                    return ((float)left * (right as Vector3D?))?.ToExpression();
                }
                if (left is double)
                {
                    if (right is byte) return ((double)left * (byte)right).ToExpression();
                    if (right is sbyte) return ((double)left * (sbyte)right).ToExpression();
                    if (right is short) return ((double)left * (short)right).ToExpression();
                    if (right is ushort) return ((double)left * (ushort)right).ToExpression();
                    if (right is int) return ((double)left * (int)right).ToExpression();
                    if (right is uint) return ((double)left * (uint)right).ToExpression();
                    if (right is long) return ((double)left * (long)right).ToExpression();
                    if (right is ulong) return ((double)left * (ulong)right).ToExpression();
                    if (right is float) return ((double)left * (float)right).ToExpression();
                    if (right is double) return ((double)left * (double)right).ToExpression();
                    if (right is Complex) return ((double)left * (Complex)right).ToExpression();
                    if (right is Vector2D) return ((double)left * (Vector2D)right).ToExpression();
                    return ((double)left * (right as Vector3D?))?.ToExpression();
                }
                if (left is Complex)
                {
                    if (right is byte) return ((Complex)left * (byte)right).ToExpression();
                    if (right is sbyte) return ((Complex)left * (sbyte)right).ToExpression();
                    if (right is short) return ((Complex)left * (short)right).ToExpression();
                    if (right is ushort) return ((Complex)left * (ushort)right).ToExpression();
                    if (right is int) return ((Complex)left * (int)right).ToExpression();
                    if (right is uint) return ((Complex)left * (uint)right).ToExpression();
                    if (right is long) return ((Complex)left * (long)right).ToExpression();
                    if (right is ulong) return ((Complex)left * (ulong)right).ToExpression();
                    if (right is float) return ((Complex)left * (float)right).ToExpression();
                    if (right is double) return ((Complex)left * (double)right).ToExpression();
                    if (right is Complex) return ((Complex)left * (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Complex)left * (Vector2D)right).ToExpression();
                    return null;
                }
                if (left is Vector2D)
                {
                    if (right is byte) return ((Vector2D)left * (byte)right).ToExpression();
                    if (right is sbyte) return ((Vector2D)left * (sbyte)right).ToExpression();
                    if (right is short) return ((Vector2D)left * (short)right).ToExpression();
                    if (right is ushort) return ((Vector2D)left * (ushort)right).ToExpression();
                    if (right is int) return ((Vector2D)left * (int)right).ToExpression();
                    if (right is uint) return ((Vector2D)left * (uint)right).ToExpression();
                    if (right is long) return ((Vector2D)left * (long)right).ToExpression();
                    if (right is ulong) return ((Vector2D)left * (ulong)right).ToExpression();
                    if (right is float) return ((Vector2D)left * (float)right).ToExpression();
                    if (right is double) return ((Vector2D)left * (double)right).ToExpression();
                    //if(right is Complex) return ((Vector2D)left * (Complex)right).ToExpression();
                    if (right is Vector2D) return ((Vector2D)left * (Vector2D)right).ToExpression();
                    return null;
                }
                if (left is Vector3D)
                {
                    if (right is byte) return ((Vector3D)left * (byte)right).ToExpression();
                    if (right is sbyte) return ((Vector3D)left * (sbyte)right).ToExpression();
                    if (right is short) return ((Vector3D)left * (short)right).ToExpression();
                    if (right is ushort) return ((Vector3D)left * (ushort)right).ToExpression();
                    if (right is int) return ((Vector3D)left * (int)right).ToExpression();
                    if (right is uint) return ((Vector3D)left * (uint)right).ToExpression();
                    if (right is long) return ((Vector3D)left * (long)right).ToExpression();
                    if (right is ulong) return ((Vector3D)left * (ulong)right).ToExpression();
                    if (right is float) return ((Vector3D)left * (float)right).ToExpression();
                    if (right is double) return ((Vector3D)left * (double)right).ToExpression();
                    //if(right is Complex) return ((Vector3D)left * (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Vector3D)left * (Vector2D)right).ToExpression();
                    return ((Vector3D)left * (right as Vector3D?))?.ToExpression();
                }
                return null;
            }

            [NotNull]
            private static Ex DivideSimplify([NotNull] bEx expr)
            {
                if (IsZero((expr.Left as cEx)?.Value)) return expr.Left;
                if (IsUnit((expr.Right as cEx)?.Value)) return expr.Left;

                return DivideValues((expr.Left as cEx)?.Value, (expr.Right as cEx)?.Value) ?? expr;
            }

            [CanBeNull]
            private static Ex DivideValues(object left, object right)
            {
                if (!IsNumeric(left) || !IsNumeric(right)) return null;
                if (left is byte)
                {
                    if (IsZero(right))
                    {
                        if (right is double)
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0 ? double.PositiveInfinity : double.NegativeInfinity).ToExpression();
                        if (!(right is float)) return Ex.Throw(new DivideByZeroException().ToExpression());
                        return float.IsNaN((float)right)
                            ? float.NaN.ToExpression()
                            : ((float)right > 0 ? float.PositiveInfinity : float.NegativeInfinity).ToExpression();
                    }
                    if (right is byte) return ((byte)left / (byte)right).ToExpression();
                    if (right is sbyte) return ((byte)left / (sbyte)right).ToExpression();
                    if (right is short) return ((byte)left / (short)right).ToExpression();
                    if (right is ushort) return ((byte)left / (ushort)right).ToExpression();
                    if (right is int) return ((byte)left / (int)right).ToExpression();
                    if (right is uint) return ((byte)left / (uint)right).ToExpression();
                    if (right is long) return ((byte)left / (long)right).ToExpression();
                    if (right is ulong) return ((byte)left / (ulong)right).ToExpression();
                    if (right is float) return ((byte)left / (float)right).ToExpression();
                    if (right is double) return ((byte)left / (double)right).ToExpression();
                    if (right is Complex) return ((byte)left / (Complex)right).ToExpression();
                    if (right is Vector2D) return ((byte)left / (Vector2D)right).ToExpression();
                    return ((byte)left / (right as Vector3D?))?.ToExpression();
                }
                if (left is sbyte)
                {
                    if (IsZero(right))
                    {
                        if (right is double)
                        {
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0
                                    ? ((sbyte)left > 0 ? double.PositiveInfinity : double.NegativeInfinity)
                                    : ((sbyte)left > 0 ? double.NegativeInfinity : double.PositiveInfinity))
                                    .ToExpression();
                        }
                        if (right is float)
                        {
                            return float.IsNaN((float)right)
                                ? float.NaN.ToExpression()
                                : ((float)right > 0
                                    ? ((sbyte)left > 0 ? float.PositiveInfinity : float.NegativeInfinity)
                                    : ((sbyte)left > 0 ? float.NegativeInfinity : float.PositiveInfinity))
                                    .ToExpression();
                        }
                        return Ex.Throw(new DivideByZeroException().ToExpression());
                    }
                    if (right is byte) return ((sbyte)left / (byte)right).ToExpression();
                    if (right is sbyte) return ((sbyte)left / (sbyte)right).ToExpression();
                    if (right is short) return ((sbyte)left / (short)right).ToExpression();
                    if (right is ushort) return ((sbyte)left / (ushort)right).ToExpression();
                    if (right is int) return ((sbyte)left / (int)right).ToExpression();
                    if (right is uint) return ((sbyte)left / (uint)right).ToExpression();
                    if (right is long) return ((sbyte)left / (long)right).ToExpression();
                    //if (right is ulong) return ((sbyte)left / (ulong)right).ToExpression();
                    if (right is float) return ((sbyte)left / (float)right).ToExpression();
                    if (right is double) return ((sbyte)left / (double)right).ToExpression();
                    if (right is Complex) return ((sbyte)left / (Complex)right).ToExpression();
                    if (right is Vector2D) return ((sbyte)left / (Vector2D)right).ToExpression();
                    return ((sbyte)left / (right as Vector3D?))?.ToExpression();
                }
                if (left is short)
                {
                    if (IsZero(right))
                    {
                        if (right is double)
                        {
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0
                                    ? ((short)left > 0 ? double.PositiveInfinity : double.NegativeInfinity)
                                    : ((short)left > 0 ? double.NegativeInfinity : double.PositiveInfinity))
                                    .ToExpression();
                        }
                        if (right is float)
                        {
                            return float.IsNaN((float)right)
                                ? float.NaN.ToExpression()
                                : ((float)right > 0
                                    ? ((short)left > 0 ? float.PositiveInfinity : float.NegativeInfinity)
                                    : ((short)left > 0 ? float.NegativeInfinity : float.PositiveInfinity))
                                    .ToExpression();
                        }
                        return Ex.Throw(new DivideByZeroException().ToExpression());
                    }
                    if (right is byte) return ((short)left / (byte)right).ToExpression();
                    if (right is sbyte) return ((short)left / (sbyte)right).ToExpression();
                    if (right is short) return ((short)left / (short)right).ToExpression();
                    if (right is ushort) return ((short)left / (ushort)right).ToExpression();
                    if (right is int) return ((short)left / (int)right).ToExpression();
                    if (right is uint) return ((short)left / (uint)right).ToExpression();
                    if (right is long) return ((short)left / (long)right).ToExpression();
                    //if(right is ulong) return ((short)left / (ulong)right).ToExpression();
                    if (right is float) return ((short)left / (float)right).ToExpression();
                    if (right is double) return ((short)left / (double)right).ToExpression();
                    if (right is Complex) return ((short)left / (Complex)right).ToExpression();
                    if (right is Vector2D) return ((short)left / (Vector2D)right).ToExpression();
                    return ((short)left / (right as Vector3D?))?.ToExpression();
                }
                if (left is ushort)
                {
                    if (IsZero(right))
                    {
                        if (right is double)
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0 ? double.PositiveInfinity : double.NegativeInfinity).ToExpression();
                        if (!(right is float)) return Ex.Throw(new DivideByZeroException().ToExpression());
                        return float.IsNaN((float)right)
                            ? float.NaN.ToExpression()
                            : ((float)right > 0 ? float.PositiveInfinity : float.NegativeInfinity).ToExpression();
                    }
                    if (right is byte) return ((ushort)left / (byte)right).ToExpression();
                    if (right is sbyte) return ((ushort)left / (sbyte)right).ToExpression();
                    if (right is short) return ((ushort)left / (short)right).ToExpression();
                    if (right is ushort) return ((ushort)left / (ushort)right).ToExpression();
                    if (right is int) return ((ushort)left / (int)right).ToExpression();
                    if (right is uint) return ((ushort)left / (uint)right).ToExpression();
                    if (right is long) return ((ushort)left / (long)right).ToExpression();
                    if (right is ulong) return ((ushort)left / (ulong)right).ToExpression();
                    if (right is float) return ((ushort)left / (float)right).ToExpression();
                    if (right is double) return ((ushort)left / (double)right).ToExpression();
                    if (right is Complex) return ((ushort)left / (Complex)right).ToExpression();
                    if (right is Vector2D) return ((ushort)left / (Vector2D)right).ToExpression();
                    return ((ushort)left / (right as Vector3D?))?.ToExpression();
                }
                if (left is int)
                {
                    if (IsZero(right))
                    {
                        if (right is double)
                        {
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0
                                    ? ((int)left > 0 ? double.PositiveInfinity : double.NegativeInfinity)
                                    : ((int)left > 0 ? double.NegativeInfinity : double.PositiveInfinity))
                                    .ToExpression();
                        }
                        if (right is float)
                        {
                            return float.IsNaN((float)right)
                                ? float.NaN.ToExpression()
                                : ((float)right > 0
                                    ? ((int)left > 0 ? float.PositiveInfinity : float.NegativeInfinity)
                                    : ((int)left > 0 ? float.NegativeInfinity : float.PositiveInfinity))
                                    .ToExpression();
                        }
                        return Ex.Throw(new DivideByZeroException().ToExpression());
                    }
                    if (right is byte) return ((int)left / (byte)right).ToExpression();
                    if (right is sbyte) return ((int)left / (sbyte)right).ToExpression();
                    if (right is short) return ((int)left / (short)right).ToExpression();
                    if (right is ushort) return ((int)left / (ushort)right).ToExpression();
                    if (right is int) return ((int)left / (int)right).ToExpression();
                    if (right is uint) return ((int)left / (uint)right).ToExpression();
                    if (right is long) return ((int)left / (long)right).ToExpression();
                    //if(right is ulong) return ((int)left / (ulong)right).ToExpression();
                    if (right is float) return ((int)left / (float)right).ToExpression();
                    if (right is double) return ((int)left / (double)right).ToExpression();
                    if (right is Complex) return ((int)left / (Complex)right).ToExpression();
                    if (right is Vector2D) return ((int)left / (Vector2D)right).ToExpression();
                    return ((int)left / (right as Vector3D?))?.ToExpression();
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
                    if (right is byte) return ((uint)left / (byte)right).ToExpression();
                    if (right is sbyte) return ((uint)left / (sbyte)right).ToExpression();
                    if (right is short) return ((uint)left / (short)right).ToExpression();
                    if (right is ushort) return ((uint)left / (ushort)right).ToExpression();
                    if (right is int) return ((uint)left / (int)right).ToExpression();
                    if (right is uint) return ((uint)left / (uint)right).ToExpression();
                    if (right is long) return ((uint)left / (long)right).ToExpression();
                    if (right is ulong) return ((uint)left / (ulong)right).ToExpression();
                    if (right is float) return ((uint)left / (float)right).ToExpression();
                    if (right is double) return ((uint)left / (double)right).ToExpression();
                    if (right is Complex) return ((uint)left / (Complex)right).ToExpression();
                    if (right is Vector2D) return ((uint)left / (Vector2D)right).ToExpression();
                    return ((uint)left / (right as Vector3D?))?.ToExpression();
                }
                if (left is long)
                {
                    if (IsZero(right))
                    {
                        if (right is double)
                        {
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0
                                    ? ((long)left > 0 ? double.PositiveInfinity : double.NegativeInfinity)
                                    : ((long)left > 0 ? double.NegativeInfinity : double.PositiveInfinity))
                                    .ToExpression();
                        }
                        if (right is float)
                        {
                            return float.IsNaN((float)right)
                                ? float.NaN.ToExpression()
                                : ((float)right > 0
                                    ? ((long)left > 0 ? float.PositiveInfinity : float.NegativeInfinity)
                                    : ((long)left > 0 ? float.NegativeInfinity : float.PositiveInfinity))
                                    .ToExpression();
                        }
                        return Ex.Throw(new DivideByZeroException().ToExpression());
                    }
                    if (right is byte) return ((long)left / (byte)right).ToExpression();
                    if (right is sbyte) return ((long)left / (sbyte)right).ToExpression();
                    if (right is short) return ((long)left / (short)right).ToExpression();
                    if (right is ushort) return ((long)left / (ushort)right).ToExpression();
                    if (right is int) return ((long)left / (int)right).ToExpression();
                    if (right is uint) return ((long)left / (uint)right).ToExpression();
                    if (right is long) return ((long)left / (long)right).ToExpression();
                    //if(right is ulong) return ((long)left / (ulong)right).ToExpression();
                    if (right is float) return ((long)left / (float)right).ToExpression();
                    if (right is double) return ((long)left / (double)right).ToExpression();
                    if (right is Complex) return ((long)left / (Complex)right).ToExpression();
                    if (right is Vector2D) return ((long)left / (Vector2D)right).ToExpression();
                    return ((long)left / (right as Vector3D?))?.ToExpression();
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
                    if (right is byte) return ((ulong)left / (byte)right).ToExpression();
                    //if(right is sbyte) return ((ulong)left / (sbyte)right).ToExpression();
                    //if(right is short) return ((ulong)left / (short)right).ToExpression();
                    if (right is ushort) return ((ulong)left / (ushort)right).ToExpression();
                    //if(right is int) return ((ulong)left / (int)right).ToExpression();
                    if (right is uint) return ((ulong)left / (uint)right).ToExpression();
                    //if(right is long) return ((ulong)left / (long)right).ToExpression();
                    if (right is ulong) return ((ulong)left / (ulong)right).ToExpression();
                    if (right is float) return ((ulong)left / (float)right).ToExpression();
                    if (right is double) return ((ulong)left / (double)right).ToExpression();
                    if (right is Complex) return ((ulong)left / (Complex)right).ToExpression();
                    if (right is Vector2D) return ((ulong)left / (Vector2D)right).ToExpression();
                    return ((ulong)left / (right as Vector3D?))?.ToExpression();
                }
                if (left is float)
                {
                    if (IsZero(right))
                    {
                        if (right is double)
                        {
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0
                                    ? ((float)left > 0 ? double.PositiveInfinity : double.NegativeInfinity)
                                    : ((float)left > 0 ? double.NegativeInfinity : double.PositiveInfinity))
                                    .ToExpression();
                        }
                        if (right is float)
                        {
                            return float.IsNaN((float)right)
                                ? float.NaN.ToExpression()
                                : ((float)right > 0
                                    ? ((float)left > 0 ? float.PositiveInfinity : float.NegativeInfinity)
                                    : ((float)left > 0 ? float.NegativeInfinity : float.PositiveInfinity))
                                    .ToExpression();
                        }
                        return Ex.Throw(new DivideByZeroException().ToExpression());
                    }
                    if (right is byte) return ((float)left / (byte)right).ToExpression();
                    if (right is sbyte) return ((float)left / (sbyte)right).ToExpression();
                    if (right is short) return ((float)left / (short)right).ToExpression();
                    if (right is ushort) return ((float)left / (ushort)right).ToExpression();
                    if (right is int) return ((float)left / (int)right).ToExpression();
                    if (right is uint) return ((float)left / (uint)right).ToExpression();
                    if (right is long) return ((float)left / (long)right).ToExpression();
                    if (right is ulong) return ((float)left / (ulong)right).ToExpression();
                    if (right is float) return ((float)left / (float)right).ToExpression();
                    if (right is double) return ((float)left / (double)right).ToExpression();
                    if (right is Complex) return ((float)left / (Complex)right).ToExpression();
                    if (right is Vector2D) return ((float)left / (Vector2D)right).ToExpression();
                    return ((float)left / (right as Vector3D?))?.ToExpression();
                }
                if (left is double)
                {
                    if (IsZero(right))
                    {
                        if (right is double)
                        {
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0
                                    ? ((double)left > 0 ? double.PositiveInfinity : double.NegativeInfinity)
                                    : ((double)left > 0 ? double.NegativeInfinity : double.PositiveInfinity))
                                    .ToExpression();
                        }
                        if (right is float)
                        {
                            return float.IsNaN((float)right)
                                ? float.NaN.ToExpression()
                                : ((float)right > 0
                                    ? ((double)left > 0 ? float.PositiveInfinity : float.NegativeInfinity)
                                    : ((double)left > 0 ? float.NegativeInfinity : float.PositiveInfinity))
                                    .ToExpression();
                        }
                        return Ex.Throw(new DivideByZeroException().ToExpression());
                    }
                    if (right is byte) return ((double)left / (byte)right).ToExpression();
                    if (right is sbyte) return ((double)left / (sbyte)right).ToExpression();
                    if (right is short) return ((double)left / (short)right).ToExpression();
                    if (right is ushort) return ((double)left / (ushort)right).ToExpression();
                    if (right is int) return ((double)left / (int)right).ToExpression();
                    if (right is uint) return ((double)left / (uint)right).ToExpression();
                    if (right is long) return ((double)left / (long)right).ToExpression();
                    if (right is ulong) return ((double)left / (ulong)right).ToExpression();
                    if (right is float) return ((double)left / (float)right).ToExpression();
                    if (right is double) return ((double)left / (double)right).ToExpression();
                    if (right is Complex) return ((double)left / (Complex)right).ToExpression();
                    if (right is Vector2D) return ((double)left / (Vector2D)right).ToExpression();
                    return ((double)left / (right as Vector3D?))?.ToExpression();
                }
                if (left is Complex)
                {
                    if (IsZero(right))
                        return Ex.Throw(new DivideByZeroException().ToExpression());
                    if (right is byte) return ((Complex)left / (byte)right).ToExpression();
                    if (right is sbyte) return ((Complex)left / (sbyte)right).ToExpression();
                    if (right is short) return ((Complex)left / (short)right).ToExpression();
                    if (right is ushort) return ((Complex)left / (ushort)right).ToExpression();
                    if (right is int) return ((Complex)left / (int)right).ToExpression();
                    if (right is uint) return ((Complex)left / (uint)right).ToExpression();
                    if (right is long) return ((Complex)left / (long)right).ToExpression();
                    if (right is ulong) return ((Complex)left / (ulong)right).ToExpression();
                    if (right is float) return ((Complex)left / (float)right).ToExpression();
                    if (right is double) return ((Complex)left / (double)right).ToExpression();
                    if (right is Complex) return ((Complex)left / (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Complex)left / (Vector2D)right).ToExpression();
                    return null;
                }
                if (left is Vector2D)
                {
                    if (IsZero(right))
                        return Ex.Throw(new DivideByZeroException().ToExpression());
                    if (right is byte) return ((Vector2D)left / (byte)right).ToExpression();
                    if (right is sbyte) return ((Vector2D)left / (sbyte)right).ToExpression();
                    if (right is short) return ((Vector2D)left / (short)right).ToExpression();
                    if (right is ushort) return ((Vector2D)left / (ushort)right).ToExpression();
                    if (right is int) return ((Vector2D)left / (int)right).ToExpression();
                    if (right is uint) return ((Vector2D)left / (uint)right).ToExpression();
                    if (right is long) return ((Vector2D)left / (long)right).ToExpression();
                    if (right is ulong) return ((Vector2D)left / (ulong)right).ToExpression();
                    if (right is float) return ((Vector2D)left / (float)right).ToExpression();
                    return ((Vector2D)left / (right as double?))?.ToExpression();
                    //if(right is Complex) return ((Vector2D)left / (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Vector2D)left / (Vector2D)right).ToExpression();
                }
                if (left is Vector3D)
                {
                    if (IsZero(right))
                        return Ex.Throw(new DivideByZeroException().ToExpression());
                    if (right is byte) return ((Vector3D)left / (byte)right).ToExpression();
                    if (right is sbyte) return ((Vector3D)left / (sbyte)right).ToExpression();
                    if (right is short) return ((Vector3D)left / (short)right).ToExpression();
                    if (right is ushort) return ((Vector3D)left / (ushort)right).ToExpression();
                    if (right is int) return ((Vector3D)left / (int)right).ToExpression();
                    if (right is uint) return ((Vector3D)left / (uint)right).ToExpression();
                    if (right is long) return ((Vector3D)left / (long)right).ToExpression();
                    if (right is ulong) return ((Vector3D)left / (ulong)right).ToExpression();
                    if (right is float) return ((Vector3D)left / (float)right).ToExpression();
                    if (right is double) return ((Vector3D)left / (double)right).ToExpression();
                    //if(right is Complex) return ((Vector3D)left / (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Vector3D)left / (Vector2D)right).ToExpression();
                    //return ((Vector3D)left / (right as Vector3D?))?.ToExpression();
                }
                return null;
            }

            [NotNull]
            private static Ex AdditionSimplify([NotNull] bEx expr)
            {
                var right = expr.Right;
                var left = expr.Left;
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

            private static IEnumerable<Ex> GetOperands_Addition([CanBeNull] bEx expr)
            {
                if (expr is null || expr.NodeType != ExpressionType.Add || expr.NodeType != ExpressionType.Subtract) yield break;

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

            [CanBeNull]
            private static Ex AddValues(object left, object right)
            {
                if (!IsNumeric(left) || !IsNumeric(right)) return null;
                if (left is byte)
                {
                    if (right is byte) return ((byte)left + (byte)right).ToExpression();
                    if (right is sbyte) return ((byte)left + (sbyte)right).ToExpression();
                    if (right is short) return ((byte)left + (short)right).ToExpression();
                    if (right is ushort) return ((byte)left + (ushort)right).ToExpression();
                    if (right is int) return ((byte)left + (int)right).ToExpression();
                    if (right is uint) return ((byte)left + (uint)right).ToExpression();
                    if (right is long) return ((byte)left + (long)right).ToExpression();
                    if (right is ulong) return ((byte)left + (ulong)right).ToExpression();
                    if (right is float) return ((byte)left + (float)right).ToExpression();
                    if (right is double) return ((byte)left + (double)right).ToExpression();
                    if (right is Complex) return ((byte)left + (Complex)right).ToExpression();
                    if (right is Vector2D) return ((byte)left + (Vector2D)right).ToExpression();
                    return ((byte)left + (right as Vector3D?))?.ToExpression();
                }
                if (left is sbyte)
                {
                    if (right is byte) return ((sbyte)left + (byte)right).ToExpression();
                    if (right is sbyte) return ((sbyte)left + (sbyte)right).ToExpression();
                    if (right is short) return ((sbyte)left + (short)right).ToExpression();
                    if (right is ushort) return ((sbyte)left + (ushort)right).ToExpression();
                    if (right is int) return ((sbyte)left + (int)right).ToExpression();
                    if (right is uint) return ((sbyte)left + (uint)right).ToExpression();
                    if (right is long) return ((sbyte)left + (long)right).ToExpression();
                    //if(right is ulong) return ((sbyte)left + (ulong)right).ToExpression();
                    if (right is float) return ((sbyte)left + (float)right).ToExpression();
                    if (right is double) return ((sbyte)left + (double)right).ToExpression();
                    if (right is Complex) return ((sbyte)left + (Complex)right).ToExpression();
                    if (right is Vector2D) return ((sbyte)left + (Vector2D)right).ToExpression();
                    return ((sbyte)left + (right as Vector3D?))?.ToExpression();
                }
                if (left is short)
                {
                    if (right is byte) return ((short)left + (byte)right).ToExpression();
                    if (right is sbyte) return ((short)left + (sbyte)right).ToExpression();
                    if (right is short) return ((short)left + (short)right).ToExpression();
                    if (right is ushort) return ((short)left + (ushort)right).ToExpression();
                    if (right is int) return ((short)left + (int)right).ToExpression();
                    if (right is uint) return ((short)left + (uint)right).ToExpression();
                    if (right is long) return ((short)left + (long)right).ToExpression();
                    //if(right is ulong) return ((short)left + (ulong)right).ToExpression();
                    if (right is float) return ((short)left + (float)right).ToExpression();
                    if (right is double) return ((short)left + (double)right).ToExpression();
                    if (right is Complex) return ((short)left + (Complex)right).ToExpression();
                    if (right is Vector2D) return ((short)left + (Vector2D)right).ToExpression();
                    return ((short)left + (right as Vector3D?))?.ToExpression();
                }
                if (left is ushort)
                {
                    if (right is byte) return ((ushort)left + (byte)right).ToExpression();
                    if (right is sbyte) return ((ushort)left + (sbyte)right).ToExpression();
                    if (right is short) return ((ushort)left + (short)right).ToExpression();
                    if (right is ushort) return ((ushort)left + (ushort)right).ToExpression();
                    if (right is int) return ((ushort)left + (int)right).ToExpression();
                    if (right is uint) return ((ushort)left + (uint)right).ToExpression();
                    if (right is long) return ((ushort)left + (long)right).ToExpression();
                    if (right is ulong) return ((ushort)left + (ulong)right).ToExpression();
                    if (right is float) return ((ushort)left + (float)right).ToExpression();
                    if (right is double) return ((ushort)left + (double)right).ToExpression();
                    if (right is Complex) return ((ushort)left + (Complex)right).ToExpression();
                    if (right is Vector2D) return ((ushort)left + (Vector2D)right).ToExpression();
                    return ((ushort)left + (right as Vector3D?))?.ToExpression();
                }
                if (left is int)
                {
                    if (right is byte) return ((int)left + (byte)right).ToExpression();
                    if (right is sbyte) return ((int)left + (sbyte)right).ToExpression();
                    if (right is short) return ((int)left + (short)right).ToExpression();
                    if (right is ushort) return ((int)left + (ushort)right).ToExpression();
                    if (right is int) return ((int)left + (int)right).ToExpression();
                    if (right is uint) return ((int)left + (uint)right).ToExpression();
                    if (right is long) return ((int)left + (long)right).ToExpression();
                    //if(right is ulong) return ((int)left + (ulong)right).ToExpression();
                    if (right is float) return ((int)left + (float)right).ToExpression();
                    if (right is double) return ((int)left + (double)right).ToExpression();
                    if (right is Complex) return ((int)left + (Complex)right).ToExpression();
                    if (right is Vector2D) return ((int)left + (Vector2D)right).ToExpression();
                    return ((int)left + (right as Vector3D?))?.ToExpression();
                }
                if (left is uint)
                {
                    if (right is byte) return ((uint)left + (byte)right).ToExpression();
                    if (right is sbyte) return ((uint)left + (sbyte)right).ToExpression();
                    if (right is short) return ((uint)left + (short)right).ToExpression();
                    if (right is ushort) return ((uint)left + (ushort)right).ToExpression();
                    if (right is int) return ((uint)left + (int)right).ToExpression();
                    if (right is uint) return ((uint)left + (uint)right).ToExpression();
                    if (right is long) return ((uint)left + (long)right).ToExpression();
                    if (right is ulong) return ((uint)left + (ulong)right).ToExpression();
                    if (right is float) return ((uint)left + (float)right).ToExpression();
                    if (right is double) return ((uint)left + (double)right).ToExpression();
                    if (right is Complex) return ((uint)left + (Complex)right).ToExpression();
                    if (right is Vector2D) return ((uint)left + (Vector2D)right).ToExpression();
                    return ((uint)left + (right as Vector3D?))?.ToExpression();
                }
                if (left is long)
                {
                    if (right is byte) return ((long)left + (byte)right).ToExpression();
                    if (right is sbyte) return ((long)left + (sbyte)right).ToExpression();
                    if (right is short) return ((long)left + (short)right).ToExpression();
                    if (right is ushort) return ((long)left + (ushort)right).ToExpression();
                    if (right is int) return ((long)left + (int)right).ToExpression();
                    if (right is uint) return ((long)left + (uint)right).ToExpression();
                    if (right is long) return ((long)left + (long)right).ToExpression();
                    //if(right is ulong) return ((long)left + (ulong)right).ToExpression();
                    if (right is float) return ((long)left + (float)right).ToExpression();
                    if (right is double) return ((long)left + (double)right).ToExpression();
                    if (right is Complex) return ((long)left + (Complex)right).ToExpression();
                    if (right is Vector2D) return ((long)left + (Vector2D)right).ToExpression();
                    return ((long)left + (right as Vector3D?))?.ToExpression();
                }
                if (left is ulong)
                {
                    if (right is byte) return ((ulong)left + (byte)right).ToExpression();
                    //if(right is sbyte) return ((ulong)left + (sbyte)right).ToExpression();
                    //if(right is short) return ((ulong)left + (short)right).ToExpression();
                    if (right is ushort) return ((ulong)left + (ushort)right).ToExpression();
                    //if(right is int) return ((ulong)left + (int)right).ToExpression();
                    if (right is uint) return ((ulong)left + (uint)right).ToExpression();
                    //if(right is long) return ((ulong)left + (long)right).ToExpression();
                    if (right is ulong) return ((ulong)left + (ulong)right).ToExpression();
                    if (right is float) return ((ulong)left + (float)right).ToExpression();
                    if (right is double) return ((ulong)left + (double)right).ToExpression();
                    if (right is Complex) return ((ulong)left + (Complex)right).ToExpression();
                    if (right is Vector2D) return ((ulong)left + (Vector2D)right).ToExpression();
                    return ((ulong)left + (right as Vector3D?))?.ToExpression();
                }
                if (left is float)
                {
                    if (right is byte) return ((float)left + (byte)right).ToExpression();
                    if (right is sbyte) return ((float)left + (sbyte)right).ToExpression();
                    if (right is short) return ((float)left + (short)right).ToExpression();
                    if (right is ushort) return ((float)left + (ushort)right).ToExpression();
                    if (right is int) return ((float)left + (int)right).ToExpression();
                    if (right is uint) return ((float)left + (uint)right).ToExpression();
                    if (right is long) return ((float)left + (long)right).ToExpression();
                    if (right is ulong) return ((float)left + (ulong)right).ToExpression();
                    if (right is float) return ((float)left + (float)right).ToExpression();
                    if (right is double) return ((float)left + (double)right).ToExpression();
                    if (right is Complex) return ((float)left + (Complex)right).ToExpression();
                    if (right is Vector2D) return ((float)left + (Vector2D)right).ToExpression();
                    return ((float)left + (right as Vector3D?))?.ToExpression();
                }
                if (left is double)
                {
                    if (right is byte) return ((double)left + (byte)right).ToExpression();
                    if (right is sbyte) return ((double)left + (sbyte)right).ToExpression();
                    if (right is short) return ((double)left + (short)right).ToExpression();
                    if (right is ushort) return ((double)left + (ushort)right).ToExpression();
                    if (right is int) return ((double)left + (int)right).ToExpression();
                    if (right is uint) return ((double)left + (uint)right).ToExpression();
                    if (right is long) return ((double)left + (long)right).ToExpression();
                    if (right is ulong) return ((double)left + (ulong)right).ToExpression();
                    if (right is float) return ((double)left + (float)right).ToExpression();
                    if (right is double) return ((double)left + (double)right).ToExpression();
                    if (right is Complex) return ((double)left + (Complex)right).ToExpression();
                    if (right is Vector2D) return ((double)left + (Vector2D)right).ToExpression();
                    return ((double)left + (right as Vector3D?))?.ToExpression();
                }
                if (left is Complex)
                {
                    if (right is byte) return ((Complex)left + (byte)right).ToExpression();
                    if (right is sbyte) return ((Complex)left + (sbyte)right).ToExpression();
                    if (right is short) return ((Complex)left + (short)right).ToExpression();
                    if (right is ushort) return ((Complex)left + (ushort)right).ToExpression();
                    if (right is int) return ((Complex)left + (int)right).ToExpression();
                    if (right is uint) return ((Complex)left + (uint)right).ToExpression();
                    if (right is long) return ((Complex)left + (long)right).ToExpression();
                    if (right is ulong) return ((Complex)left + (ulong)right).ToExpression();
                    if (right is float) return ((Complex)left + (float)right).ToExpression();
                    if (right is double) return ((Complex)left + (double)right).ToExpression();
                    if (right is Complex) return ((Complex)left + (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Complex)left + (Vector2D)right).ToExpression();
                    return null;
                }
                if (left is Vector2D)
                {
                    if (right is byte) return ((Vector2D)left + (byte)right).ToExpression();
                    if (right is sbyte) return ((Vector2D)left + (sbyte)right).ToExpression();
                    if (right is short) return ((Vector2D)left + (short)right).ToExpression();
                    if (right is ushort) return ((Vector2D)left + (ushort)right).ToExpression();
                    if (right is int) return ((Vector2D)left + (int)right).ToExpression();
                    if (right is uint) return ((Vector2D)left + (uint)right).ToExpression();
                    if (right is long) return ((Vector2D)left + (long)right).ToExpression();
                    if (right is ulong) return ((Vector2D)left + (ulong)right).ToExpression();
                    if (right is float) return ((Vector2D)left + (float)right).ToExpression();
                    if (right is double) return ((Vector2D)left + (double)right).ToExpression();
                    //if(right is Complex) return ((Vector2D)left + (Complex)right).ToExpression();
                    if (right is Vector2D) return ((Vector2D)left + (Vector2D)right).ToExpression();
                    return null;
                }
                if (left is Vector3D)
                {
                    if (right is byte) return ((Vector3D)left + (byte)right).ToExpression();
                    if (right is sbyte) return ((Vector3D)left + (sbyte)right).ToExpression();
                    if (right is short) return ((Vector3D)left + (short)right).ToExpression();
                    if (right is ushort) return ((Vector3D)left + (ushort)right).ToExpression();
                    if (right is int) return ((Vector3D)left + (int)right).ToExpression();
                    if (right is uint) return ((Vector3D)left + (uint)right).ToExpression();
                    if (right is long) return ((Vector3D)left + (long)right).ToExpression();
                    if (right is ulong) return ((Vector3D)left + (ulong)right).ToExpression();
                    if (right is float) return ((Vector3D)left + (float)right).ToExpression();
                    if (right is double) return ((Vector3D)left + (double)right).ToExpression();
                    //if(right is Complex) return ((Vector3D)left + (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Vector3D)left + (Vector2D)right).ToExpression();
                    return ((Vector3D)left + (right as Vector3D?))?.ToExpression();
                }
                return null;
            }

            [NotNull]
            private static Ex subtractionSimplify([NotNull] bEx expr)
            {
                if (IsZero((expr.Left as cEx)?.Value)) return expr.Right.Negate();
                if (IsZero((expr.Right as cEx)?.Value)) return expr.Left;

                return subtractValues((expr.Left as cEx)?.Value, (expr.Right as cEx)?.Value) ?? expr;
            }

            [CanBeNull]
            private static Ex subtractValues(object left, object right)
            {
                if (!IsNumeric(left) || !IsNumeric(right)) return null;
                if (left is byte)
                {
                    if (right is byte) return ((byte)left - (byte)right).ToExpression();
                    if (right is sbyte) return ((byte)left - (sbyte)right).ToExpression();
                    if (right is short) return ((byte)left - (short)right).ToExpression();
                    if (right is ushort) return ((byte)left - (ushort)right).ToExpression();
                    if (right is int) return ((byte)left - (int)right).ToExpression();
                    if (right is uint) return ((byte)left - (uint)right).ToExpression();
                    if (right is long) return ((byte)left - (long)right).ToExpression();
                    if (right is ulong) return ((byte)left - (ulong)right).ToExpression();
                    if (right is float) return ((byte)left - (float)right).ToExpression();
                    if (right is double) return ((byte)left - (double)right).ToExpression();
                    if (right is Complex) return ((byte)left - (Complex)right).ToExpression();
                    if (right is Vector2D) return ((byte)left - (Vector2D)right).ToExpression();
                    return ((byte)left - (right as Vector3D?))?.ToExpression();
                }
                if (left is sbyte)
                {
                    if (right is byte) return ((sbyte)left - (byte)right).ToExpression();
                    if (right is sbyte) return ((sbyte)left - (sbyte)right).ToExpression();
                    if (right is short) return ((sbyte)left - (short)right).ToExpression();
                    if (right is ushort) return ((sbyte)left - (ushort)right).ToExpression();
                    if (right is int) return ((sbyte)left - (int)right).ToExpression();
                    if (right is uint) return ((sbyte)left - (uint)right).ToExpression();
                    if (right is long) return ((sbyte)left - (long)right).ToExpression();
                    //if(right is ulong) return ((sbyte)left - (ulong)right).ToExpression();
                    if (right is float) return ((sbyte)left - (float)right).ToExpression();
                    if (right is double) return ((sbyte)left - (double)right).ToExpression();
                    if (right is Complex) return ((sbyte)left - (Complex)right).ToExpression();
                    if (right is Vector2D) return ((sbyte)left - (Vector2D)right).ToExpression();
                    return ((sbyte)left - (right as Vector3D?))?.ToExpression();
                }
                if (left is short)
                {
                    if (right is byte) return ((short)left - (byte)right).ToExpression();
                    if (right is sbyte) return ((short)left - (sbyte)right).ToExpression();
                    if (right is short) return ((short)left - (short)right).ToExpression();
                    if (right is ushort) return ((short)left - (ushort)right).ToExpression();
                    if (right is int) return ((short)left - (int)right).ToExpression();
                    if (right is uint) return ((short)left - (uint)right).ToExpression();
                    if (right is long) return ((short)left - (long)right).ToExpression();
                    //if(right is ulong) return ((short)left - (ulong)right).ToExpression();
                    if (right is float) return ((short)left - (float)right).ToExpression();
                    if (right is double) return ((short)left - (double)right).ToExpression();
                    if (right is Complex) return ((short)left - (Complex)right).ToExpression();
                    if (right is Vector2D) return ((short)left - (Vector2D)right).ToExpression();
                    return ((short)left - (right as Vector3D?))?.ToExpression();
                }
                if (left is ushort)
                {
                    if (right is byte) return ((ushort)left - (byte)right).ToExpression();
                    if (right is sbyte) return ((ushort)left - (sbyte)right).ToExpression();
                    if (right is short) return ((ushort)left - (short)right).ToExpression();
                    if (right is ushort) return ((ushort)left - (ushort)right).ToExpression();
                    if (right is int) return ((ushort)left - (int)right).ToExpression();
                    if (right is uint) return ((ushort)left - (uint)right).ToExpression();
                    if (right is long) return ((ushort)left - (long)right).ToExpression();
                    if (right is ulong) return ((ushort)left - (ulong)right).ToExpression();
                    if (right is float) return ((ushort)left - (float)right).ToExpression();
                    if (right is double) return ((ushort)left - (double)right).ToExpression();
                    if (right is Complex) return ((ushort)left - (Complex)right).ToExpression();
                    if (right is Vector2D) return ((ushort)left - (Vector2D)right).ToExpression();
                    return ((ushort)left - (right as Vector3D?))?.ToExpression();
                }
                if (left is int)
                {
                    if (right is byte) return ((int)left - (byte)right).ToExpression();
                    if (right is sbyte) return ((int)left - (sbyte)right).ToExpression();
                    if (right is short) return ((int)left - (short)right).ToExpression();
                    if (right is ushort) return ((int)left - (ushort)right).ToExpression();
                    if (right is int) return ((int)left - (int)right).ToExpression();
                    if (right is uint) return ((int)left - (uint)right).ToExpression();
                    if (right is long) return ((int)left - (long)right).ToExpression();
                    //if(right is ulong) return ((int)left - (ulong)right).ToExpression();
                    if (right is float) return ((int)left - (float)right).ToExpression();
                    if (right is double) return ((int)left - (double)right).ToExpression();
                    if (right is Complex) return ((int)left - (Complex)right).ToExpression();
                    if (right is Vector2D) return ((int)left - (Vector2D)right).ToExpression();
                    return ((int)left - (right as Vector3D?))?.ToExpression();
                }
                if (left is uint)
                {
                    if (right is byte) return ((uint)left - (byte)right).ToExpression();
                    if (right is sbyte) return ((uint)left - (sbyte)right).ToExpression();
                    if (right is short) return ((uint)left - (short)right).ToExpression();
                    if (right is ushort) return ((uint)left - (ushort)right).ToExpression();
                    if (right is int) return ((uint)left - (int)right).ToExpression();
                    if (right is uint) return ((uint)left - (uint)right).ToExpression();
                    if (right is long) return ((uint)left - (long)right).ToExpression();
                    if (right is ulong) return ((uint)left - (ulong)right).ToExpression();
                    if (right is float) return ((uint)left - (float)right).ToExpression();
                    if (right is double) return ((uint)left - (double)right).ToExpression();
                    if (right is Complex) return ((uint)left - (Complex)right).ToExpression();
                    if (right is Vector2D) return ((uint)left - (Vector2D)right).ToExpression();
                    return ((uint)left - (right as Vector3D?))?.ToExpression();
                }
                if (left is long)
                {
                    if (right is byte) return ((long)left - (byte)right).ToExpression();
                    if (right is sbyte) return ((long)left - (sbyte)right).ToExpression();
                    if (right is short) return ((long)left - (short)right).ToExpression();
                    if (right is ushort) return ((long)left - (ushort)right).ToExpression();
                    if (right is int) return ((long)left - (int)right).ToExpression();
                    if (right is uint) return ((long)left - (uint)right).ToExpression();
                    if (right is long) return ((long)left - (long)right).ToExpression();
                    //if(right is ulong) return ((long)left - (ulong)right).ToExpression();
                    if (right is float) return ((long)left - (float)right).ToExpression();
                    if (right is double) return ((long)left - (double)right).ToExpression();
                    if (right is Complex) return ((long)left - (Complex)right).ToExpression();
                    if (right is Vector2D) return ((long)left - (Vector2D)right).ToExpression();
                    return ((long)left - (right as Vector3D?))?.ToExpression();
                }
                if (left is ulong)
                {
                    if (right is byte) return ((ulong)left - (byte)right).ToExpression();
                    //if(right is sbyte) return ((ulong)left - (sbyte)right).ToExpression();
                    //if(right is short) return ((ulong)left - (short)right).ToExpression();
                    if (right is ushort) return ((ulong)left - (ushort)right).ToExpression();
                    //if(right is int) return ((ulong)left - (int)right).ToExpression();
                    if (right is uint) return ((ulong)left - (uint)right).ToExpression();
                    //if(right is long) return ((ulong)left - (long)right).ToExpression();
                    if (right is ulong) return ((ulong)left - (ulong)right).ToExpression();
                    if (right is float) return ((ulong)left - (float)right).ToExpression();
                    if (right is double) return ((ulong)left - (double)right).ToExpression();
                    if (right is Complex) return ((ulong)left - (Complex)right).ToExpression();
                    if (right is Vector2D) return ((ulong)left - (Vector2D)right).ToExpression();
                    return ((ulong)left - (right as Vector3D?))?.ToExpression();
                }
                if (left is float)
                {
                    if (right is byte) return ((float)left - (byte)right).ToExpression();
                    if (right is sbyte) return ((float)left - (sbyte)right).ToExpression();
                    if (right is short) return ((float)left - (short)right).ToExpression();
                    if (right is ushort) return ((float)left - (ushort)right).ToExpression();
                    if (right is int) return ((float)left - (int)right).ToExpression();
                    if (right is uint) return ((float)left - (uint)right).ToExpression();
                    if (right is long) return ((float)left - (long)right).ToExpression();
                    if (right is ulong) return ((float)left - (ulong)right).ToExpression();
                    if (right is float) return ((float)left - (float)right).ToExpression();
                    if (right is double) return ((float)left - (double)right).ToExpression();
                    if (right is Complex) return ((float)left - (Complex)right).ToExpression();
                    if (right is Vector2D) return ((float)left - (Vector2D)right).ToExpression();
                    return ((float)left - (right as Vector3D?))?.ToExpression();
                }
                if (left is double)
                {
                    if (right is byte) return ((double)left - (byte)right).ToExpression();
                    if (right is sbyte) return ((double)left - (sbyte)right).ToExpression();
                    if (right is short) return ((double)left - (short)right).ToExpression();
                    if (right is ushort) return ((double)left - (ushort)right).ToExpression();
                    if (right is int) return ((double)left - (int)right).ToExpression();
                    if (right is uint) return ((double)left - (uint)right).ToExpression();
                    if (right is long) return ((double)left - (long)right).ToExpression();
                    if (right is ulong) return ((double)left - (ulong)right).ToExpression();
                    if (right is float) return ((double)left - (float)right).ToExpression();
                    if (right is double) return ((double)left - (double)right).ToExpression();
                    if (right is Complex) return ((double)left - (Complex)right).ToExpression();
                    if (right is Vector2D) return ((double)left - (Vector2D)right).ToExpression();
                    return ((double)left - (right as Vector3D?))?.ToExpression();
                }
                if (left is Complex)
                {
                    if (right is byte) return ((Complex)left - (byte)right).ToExpression();
                    if (right is sbyte) return ((Complex)left - (sbyte)right).ToExpression();
                    if (right is short) return ((Complex)left - (short)right).ToExpression();
                    if (right is ushort) return ((Complex)left - (ushort)right).ToExpression();
                    if (right is int) return ((Complex)left - (int)right).ToExpression();
                    if (right is uint) return ((Complex)left - (uint)right).ToExpression();
                    if (right is long) return ((Complex)left - (long)right).ToExpression();
                    if (right is ulong) return ((Complex)left - (ulong)right).ToExpression();
                    if (right is float) return ((Complex)left - (float)right).ToExpression();
                    if (right is double) return ((Complex)left - (double)right).ToExpression();
                    if (right is Complex) return ((Complex)left - (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Complex)left - (Vector2D)right).ToExpression();
                    return null;
                }
                if (left is Vector2D)
                {
                    if (right is byte) return ((Vector2D)left - (byte)right).ToExpression();
                    if (right is sbyte) return ((Vector2D)left - (sbyte)right).ToExpression();
                    if (right is short) return ((Vector2D)left - (short)right).ToExpression();
                    if (right is ushort) return ((Vector2D)left - (ushort)right).ToExpression();
                    if (right is int) return ((Vector2D)left - (int)right).ToExpression();
                    if (right is uint) return ((Vector2D)left - (uint)right).ToExpression();
                    if (right is long) return ((Vector2D)left - (long)right).ToExpression();
                    if (right is ulong) return ((Vector2D)left - (ulong)right).ToExpression();
                    if (right is float) return ((Vector2D)left - (float)right).ToExpression();
                    if (right is double) return ((Vector2D)left - (double)right).ToExpression();
                    //if(right is Complex) return ((Vector2D)left - (Complex)right).ToExpression();
                    if (right is Vector2D) return ((Vector2D)left - (Vector2D)right).ToExpression();
                    return null;
                }
                if (left is Vector3D)
                {
                    if (right is byte) return ((Vector3D)left - (byte)right).ToExpression();
                    if (right is sbyte) return ((Vector3D)left - (sbyte)right).ToExpression();
                    if (right is short) return ((Vector3D)left - (short)right).ToExpression();
                    if (right is ushort) return ((Vector3D)left - (ushort)right).ToExpression();
                    if (right is int) return ((Vector3D)left - (int)right).ToExpression();
                    if (right is uint) return ((Vector3D)left - (uint)right).ToExpression();
                    if (right is long) return ((Vector3D)left - (long)right).ToExpression();
                    if (right is ulong) return ((Vector3D)left - (ulong)right).ToExpression();
                    if (right is float) return ((Vector3D)left - (float)right).ToExpression();
                    if (right is double) return ((Vector3D)left - (double)right).ToExpression();
                    //if(right is Complex) return ((Vector3D)left - (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Vector3D)left - (Vector2D)right).ToExpression();
                    return ((Vector3D)left - (right as Vector3D?))?.ToExpression();
                }
                return null;
            }
        }
    }
}