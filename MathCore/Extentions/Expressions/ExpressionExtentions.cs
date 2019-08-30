using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MathCore.Vectors;
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

namespace MathCore.Extentions.Expressions
{
    public static class ExpressionExtentions
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

            protected override Ex VisitUnary(uEx node)
                => node.Operand == ParamExpressionToSubstitute
                    ? Ex.MakeUnary(node.NodeType, SubstExpression.Body, node.Type)
                    : base.VisitUnary(node);

            protected override Ex VisitMethodCall(mcEx node)
            {
                var lv_NumArgsToSubstituteAmongMethodArgs =
                    (from expr in node.Arguments
                     where expr == ParamExpressionToSubstitute
                     select expr).Count();
                if(lv_NumArgsToSubstituteAmongMethodArgs == 0)
                    return base.VisitMethodCall(node);
                var arguments =
                    node.Arguments.Select(arg => arg == ParamExpressionToSubstitute ? SubstExpression.Body : Visit(arg))
                        .ToList();
                return Ex.Call(node.Object, node.Method, arguments);
            }

            protected override Ex VisitBinary(bEx node)
            {
                Ex left, right;
                var lv_SubstLeft = false;
                var lv_SubstRight = false;

                if(node.Left == ParamExpressionToSubstitute)
                {
                    left = SubstExpression.Body;
                    lv_SubstLeft = true;
                }
                else
                    left = node.Left;

                if(node.Right == ParamExpressionToSubstitute)
                {
                    right = SubstExpression.Body;
                    lv_SubstRight = true;
                }
                else
                    right = node.Right;
                if(!lv_SubstLeft && !lv_SubstRight) return base.VisitBinary(node);
                if(!lv_SubstLeft)
                    left = Visit(left);
                if(!lv_SubstRight)
                    right = Visit(right);
                return Ex.MakeBinary(node.NodeType, left, right, node.IsLiftedToNull, node.Method);
            }

            #endregion
        }

        #endregion

        #region Methods

        /// <exception cref="FormatException">Количество аргументов подстановки не равно 1, или во входном выражении отсутсвуте подставляемый параметр</exception>
        public static lEx Substitute(this lEx Expr, lEx Substitution)
        {
            var lv_Parameters = Expr.Parameters;
            var lv_sParameters = Substitution.Parameters;
            if(lv_sParameters.Count != 1)
                throw new FormatException("Количество аргументов подстановки не равно 1");
            var SubstituteParameter = lv_sParameters[0];
            if(!lv_Parameters.Contains(p => p.Name == SubstituteParameter.Name && p.Type == SubstituteParameter.Type))
                throw new FormatException("Во входном выражении отсутсвуте подставляемый параметр");

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

            var lv_MainParameter = MainEx.Parameters.FirstOrDefault(p => p.Name == ParameterName);

            if(lv_MainParameter == null)
                throw new Exception($"Could not find input parameter \"{ParameterName}\" in Expression \"{MainEx}\"");

            var lv_SubstitutionParameter = SubstExpression.Parameters.FirstOrDefault(p => p.Name == ParameterName);

            if(lv_SubstitutionParameter == null)
                throw new Exception($"Could not find substitution parameter \"{ParameterName}\" in Expression \"{SubstExpression}\"");

            if(lv_SubstitutionParameter.Type != lv_MainParameter.Type)
                throw new Exception
                (
                    $"The substitute Expression return type \"{SubstExpression.Type}\" does not match the type of the substituted variable \"{ParameterName}:{lv_MainParameter.Type}\""
                );

            #endregion

            //var pars = new List<ParameterEx>();

            //pars.AddRange(MainEx.Parameters);
            var pars = MainEx.Parameters.ToList();

            var lv_IdxToSubst = pars.IndexOf(lv_MainParameter);

            pars.RemoveAt(lv_IdxToSubst);

            foreach(var lv_SubstPe in SubstExpression.Parameters)
            {
                var lv_NumParamsOfTheSameNameInMainEx = pars.Count(pe => pe.Name == lv_SubstPe.Name);
                if(lv_NumParamsOfTheSameNameInMainEx != 0)
                    throw new Exception($"Input parameter of name \"{lv_SubstPe.Name}\" already exists in the main Expression");
                pars.Insert(lv_IdxToSubst, lv_SubstPe);
                lv_IdxToSubst++;
                //continue;
                /*
                throw new Exception
                (
                    string.Format
                    (
                        "Input parameter of name \"{0}\" already exists in the main Expression",
                        substPe.Name
                    )
                );
                */
            }

            var visitor =
                new SubstExpressionVisitor
                {
                    SubstExpression = SubstExpression,
                    ParamExpressionToSubstitute = lv_MainParameter
                };

            return (lEx)visitor.Visit(Ex.Lambda(MainEx.Body, pars));
        }

        public static mEx GetProperty(this Ex obj, string PropertyName) => Ex.Property(obj, PropertyName);

        public static mEx GetProperty(this Ex obj, PropertyInfo Info) => Ex.Property(obj, Info);

        public static MemberExpression GetField(this Ex obj, string FieldName) =>
            Ex.Field(obj, FieldName);

        public static bEx Assign(this Ex dest, Ex source) => Ex.Assign(dest, source);

        public static bEx AssignTo(this Ex source, Ex dest) => Ex.Assign(dest, source);

        public static uEx Negate(this Ex obj) => Ex.Negate(obj);

        public static bEx AddAssign(this Ex left, Ex right) => Ex.AddAssign(left, right);

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
            if(ta == typeof(double)) b = b.ConvertTo(ta);
            else if(tb == typeof(double)) a = a.ConvertTo(tb);
            else if(ta == typeof(float)) b = b.ConvertTo(ta);
            else if(tb == typeof(float)) a = a.ConvertTo(tb);
            else if(ta == typeof(ulong)) b = b.ConvertTo(ta);
            else if(tb == typeof(ulong)) a = a.ConvertTo(tb);
            else if(ta == typeof(long)) b = b.ConvertTo(ta);
            else if(tb == typeof(long)) a = a.ConvertTo(tb);
            else if(ta == typeof(uint)) b = b.ConvertTo(ta);
            else if(tb == typeof(uint)) a = a.ConvertTo(tb);
            else if(ta == typeof(int)) b = b.ConvertTo(ta);
            else if(tb == typeof(int)) a = a.ConvertTo(tb);
            else if(ta == typeof(sbyte)) b = b.ConvertTo(ta);
            else if(tb == typeof(sbyte)) a = a.ConvertTo(tb);
            else if(ta == typeof(byte)) b = b.ConvertTo(ta);
            else if(tb == typeof(byte)) a = a.ConvertTo(tb);
            return a;
        }

        public static bEx AddWithConversion(this Ex left, Ex right)
        {
            if(!left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type) return left.Add(right);
            return left.TryConvert(ref right).Add(right);
        }

        public static bEx Add(this Ex left, Ex right, bool conversion = false) => conversion ? left.AddWithConversion(right) : Ex.Add(left, right);

        public static bEx Add(this Ex left, int right) => left.AddWithConversion(right.ToExpression());
        public static bEx Add(this Ex left, double right) => left.AddWithConversion(right.ToExpression());
        public static bEx Add(this Ex left, decimal right) => left.AddWithConversion(right.ToExpression());
        public static bEx Add(this Ex left, string right) => left.Add(right.ToExpression());

        public static bEx Add(this Ex left, params Ex[] right)
        {
            var i = 0;
            Ex l;
            if(left != null) l = left;
            else if(right == null || right.Length == i) return null;
            else l = right[i++];
            while(i < right.Length)
                l = l.AddWithConversion(right[i++]);
            return (bEx)l;
        }

        public static bEx SubtractAssign(this Ex left, Ex right) => Ex.SubtractAssign(left, right);

        public static bEx SubtractWithConversion(this Ex left, Ex right)
        {
            if(!left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type) return left.Subtract(right);
            return left.TryConvert(ref right).Subtract(right);
        }

        public static bEx Subtract(this Ex left, Ex right, bool conversion = false) => conversion ? left.SubtractWithConversion(right) : Ex.Subtract(left, right);
        public static bEx Subtract(this Ex left, int right) => left.SubtractWithConversion(right.ToExpression());
        public static bEx Subtract(this Ex left, double right) => left.SubtractWithConversion(right.ToExpression());
        public static bEx Subtract(this Ex left, decimal right) => left.SubtractWithConversion(right.ToExpression());

        public static bEx MultiplyAssign(this Ex left, Ex right)
            => Ex.MultiplyAssign(left, right);

        public static bEx MultiplyWithConversion(this Ex left, Ex right)
        {
            if(!left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type) return left.Multiply(right);
            return left.TryConvert(ref right).Multiply(right);
        }

        public static bEx Multiply(this Ex left, Ex right, bool conversion = false) => conversion ? left.MultiplyWithConversion(right) : Ex.Multiply(left, right);
        public static bEx Multiply(this Ex left, int right) => left.MultiplyWithConversion(right.ToExpression());
        public static bEx Multiply(this Ex left, double right) => left.MultiplyWithConversion(right.ToExpression());

        public static bEx Multiply(this Ex left, params Ex[] right)
        {
            var i = 0;
            Ex l;
            if(left != null) l = left;
            else if(right == null || right.Length == i) return null;
            else l = right[i++];
            while(i < right.Length)
                l = l.MultiplyWithConversion(right[i++]);
            return (bEx)l;
        }

        public static bEx DivideAssign(this Ex left, Ex right) => Ex.DivideAssign(left, right);

        public static bEx DivideWithConversion(this Ex left, Ex right)
        {
            if(!left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type) return left.Divide(right);
            return left.TryConvert(ref right).Divide(right);
        }

        public static bEx Divide(this Ex left, Ex right, bool conversion = false) => conversion ? left.DivideWithConversion(right) : Ex.Divide(left, right);
        public static bEx Divide(this Ex left, int right) => left.DivideWithConversion(right.ToExpression());
        public static bEx Divide(this Ex left, double right) => left.DivideWithConversion(right.ToExpression());

        public static bEx PowerAssign(this Ex left, Ex right) => Ex.PowerAssign(left, right);

        public static bEx PowerWithConversion(this Ex left, Ex right)
        {
            if(!left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type) return left.Power(right);
            return left.TryConvert(ref right).Power(right);
        }
        public static bEx PowerOfWithConversion(this Ex left, Ex right)
        {
            if(!left.IsNumeric() || !right.IsNumeric() || left.Type == right.Type) return right.Power(left);
            return right.TryConvert(ref left).Power(left);
        }

        public static bEx Power(this Ex left, Ex right, bool conversion = false) => conversion ? left.PowerWithConversion(right) : Ex.Power(left, right);
        public static bEx PowerOf(this Ex left, Ex right, bool conversion = false) => conversion ? left.PowerOfWithConversion(right) : Ex.Power(right, left);
        public static bEx Power(this Ex left, int right) => left.PowerWithConversion(right.ToExpression());
        public static bEx PowerOf(this Ex left, int right) => right.ToExpression().PowerWithConversion(left);
        public static bEx Power(this Ex left, double right) => left.PowerWithConversion(right.ToExpression());
        public static bEx PowerOf(this Ex left, double right) => right.ToExpression().PowerWithConversion(left);

        public static mcEx Sqrt(this Ex expr) => MathExpression.Sqrt(expr);
        public static bEx SqrtPower(this Ex expr) => MathExpression.SqrtPower(expr);
        public static bEx SqrtPower(this Ex expr, Ex power) => MathExpression.SqrtPower(expr, power);

        public static bEx IsEqual(this Ex left, Ex right) => Ex.Equal(left, right);

        public static bEx IsNotEqual(this Ex left, Ex right) => Ex.NotEqual(left, right);

        public static bEx IsGreaterThan(this Ex left, Ex right) => Ex.GreaterThan(left, right);

        public static bEx IsGreaterThanOrEqual(this Ex left, Ex right) => Ex.GreaterThanOrEqual(left, right);

        public static bEx IsLessThan(this Ex left, Ex right) => Ex.LessThan(left, right);

        public static bEx IsLessThanOrEqual(this Ex left, Ex right) => Ex.LessThanOrEqual(left, right);

        public static bEx And(this Ex left, Ex right) => Ex.And(left, right);

        public static bEx AndAssign(this Ex left, Ex right) => Ex.AndAssign(left, right);

        public static bEx OrAssign(this Ex left, Ex right) => Ex.OrAssign(left, right);

        public static bEx Or(this Ex left, Ex right) => Ex.Or(left, right);
        public static uEx Not(this Ex d) => Ex.Not(d);

        public static bEx AndLazy(this Ex left, Ex right) => Ex.AndAlso(left, right);

        public static bEx OrLazy(this Ex left, Ex right) => Ex.OrElse(left, right);

        public static bEx Coalesce(this Ex first, Ex second) => Ex.Coalesce(first, second);

        public static bEx Coalesce(this Ex left, params Ex[] right)
        {
            var i = 0;
            Ex l;
            if(left != null) l = left;
            else if(right == null || right.Length == i) return null;
            else l = right[i++];
            while(i < right.Length)
                l = l.Coalesce(right[i++]);
            return (bEx)l;
        }

        public static bEx XORAssign(this Ex left, Ex right) => Ex.ExclusiveOrAssign(left, right);

        public static bEx XOR(this Ex left, Ex right) => Ex.ExclusiveOr(left, right);

        public static bEx XOR(this Ex left, params Ex[] right)
        {
            var i = 0;
            Ex l;
            if(left != null) l = left;
            else if(right == null || right.Length == i) return null;
            else l = right[i++];
            while(i < right.Length)
                l = l.XOR(right[i++]);
            return (bEx)l;
        }

        public static bEx ModuloAssign(this Ex left, Ex right) => Ex.ModuloAssign(left, right);

        public static bEx Modulo(this Ex left, Ex right) => Ex.Modulo(left, right);

        public static bEx LeftShiftAssign(this Ex left, Ex right) => Ex.LeftShiftAssign(left, right);

        public static bEx LeftShift(this Ex left, Ex right) => Ex.LeftShift(left, right);

        public static bEx RightShiftAssign(this Ex left, Ex right) => Ex.RightShiftAssign(left, right);

        public static bEx RightShift(this Ex left, Ex right) => Ex.RightShift(left, right);

        public static bEx IsRefEqual(this Ex left, Ex right) => Ex.ReferenceEqual(left, right);

        public static bEx IsIsRefEqual(this Ex left, Ex right) => Ex.ReferenceNotEqual(left, right);

        public static Ex Condition(this Ex Condition, Ex Then, Ex Else) => Ex.Condition(Condition, Then, Else);

        public static Ex ToNewExpression(this Type type) => Ex.New(type.GetConstructor(Type.EmptyTypes));

        public static Ex ToNewExpression(this Type type, params Ex[] p) => Ex.New(type.GetConstructor(p.Select(pp => pp.Type).ToArray()));

        public static pEx ParameterOf(this string ParameterName, Type type) => Ex.Parameter(type, ParameterName);

        public static mcEx GetCall(this Ex obj, string method, IEnumerable<Ex> arg)
            => Ex.Call(obj, method, (arg = arg.ToArray()).Select(a => a.Type).ToArray(), (Ex[])arg);

        public static mcEx GetCall(this Ex obj, string method, params Ex[] arg)
            => Ex.Call(obj, method, arg.Select(a => a.Type).ToArray(), arg);

        public static mcEx GetCall(this Ex obj, MethodInfo method, IEnumerable<Ex> arg) => Ex.Call(obj, method, arg);

        public static mcEx GetCall(this Ex obj, MethodInfo method, params Ex[] arg) => Ex.Call(obj, method, arg);

        public static mcEx GetCall(this Ex obj, MethodInfo method) => Ex.Call(obj, method);

        public static mcEx GetCall(this Ex obj, Delegate d, IEnumerable<Ex> arg) => obj.GetCall(d.Method, arg);

        public static mcEx GetCall(this Ex obj, Delegate d, params Ex[] arg) => obj.GetCall(d.Method, arg);

        public static mcEx GetCall(this Ex obj, Delegate d) => obj.GetCall(d.Method);

        public static InvocationExpression GetInvoke(this Ex d, IEnumerable<Ex> arg) => Ex.Invoke(d, arg);

        public static InvocationExpression GetInvoke(this Ex d, params Ex[] arg) => Ex.Invoke(d, arg);

        public static iEx ArrayAccess(this Ex d, IEnumerable<Ex> arg) => Ex.ArrayAccess(d, arg);

        public static iEx ArrayAccess(this Ex d, params Ex[] arg) => Ex.ArrayAccess(d, arg);

        public static mcEx ArrayIndex(this Ex d, IEnumerable<Ex> arg) => Ex.ArrayIndex(d, arg);

        public static mcEx ArrayIndex(this Ex d, params Ex[] arg) => Ex.ArrayIndex(d, arg);

        public static uEx ArrayLength(this Ex d) => Ex.ArrayLength(d);
        public static uEx ConvertTo(this Ex d, Type type) => Ex.Convert(d, type);
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
        public static uEx MakeUnary(this Ex d, ExpressionType uType, Type type) => Ex.MakeUnary(uType, d, type);
        public static bEx MakeUnary(this Ex left, Ex right, ExpressionType uType) => Ex.MakeBinary(uType, left, right);

        public static Expression<TDelegate> CreateLambda<TDelegate>(this Ex body, params pEx[] p) => Ex.Lambda<TDelegate>(body, p);
        public static lEx CreateLambda(this Ex body, params pEx[] p) => Ex.Lambda(body, p);

        public static Ex CloneExpression(this Ex expr)
        {
            var visitor = new CloningVisitor();
            return visitor.Visit(expr);
        }

        public static Ex[] CloneArray(this Ex[] expr)
        {
            if(expr == null) return null;
            var visitor = new CloningVisitor();
            var result = new Ex[expr.Length];
            for(var i = 0; i < result.Length; i++)
                result[i] = visitor.Visit(expr[i]);
            return result;
        }
        public static Ex[,] CloneArray(this Ex[,] expr)
        {
            if(expr == null) return null;
            var visitor = new CloningVisitor();

            var N = expr.GetLength(0);
            var M = expr.GetLength(1);
            var result = new Ex[N, M];
            for(var i = 0; i < N; i++)
                for(var j = 0; j < M; j++)
                    result[i, j] = visitor.Visit(expr[i, j]);
            return result;
        }

        #endregion

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
                switch(expr.NodeType)
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
                        return SubstractionSimplify(expr);
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

            private static Ex MultiplySimplify(bEx expr)
            {
                //var is_checked = expr.NodeType == ExpressionType.MultiplyChecked;
                if(IsZero((expr.Left as cEx)?.Value)) return expr.Left;
                if(IsUnit((expr.Left as cEx)?.Value)) return expr.Right;

                if(IsZero((expr.Right as cEx)?.Value)) return expr.Right;
                if(IsUnit((expr.Right as cEx)?.Value)) return expr.Left;

                return MultiplyValues((expr.Left as cEx)?.Value, (expr.Right as cEx)?.Value) ?? expr;
            }

            private static Ex MultiplyValues(object left, object right)
            {
                if(!IsNumeric(left) || !IsNumeric(right)) return null;
                if(left is byte)
                {
                    if(right is byte) return ((byte)left * (byte)right).ToExpression();
                    if(right is sbyte) return ((byte)left * (sbyte)right).ToExpression();
                    if(right is short) return ((byte)left * (short)right).ToExpression();
                    if(right is ushort) return ((byte)left * (ushort)right).ToExpression();
                    if(right is int) return ((byte)left * (int)right).ToExpression();
                    if(right is uint) return ((byte)left * (uint)right).ToExpression();
                    if(right is long) return ((byte)left * (long)right).ToExpression();
                    if(right is ulong) return ((byte)left * (ulong)right).ToExpression();
                    if(right is float) return ((byte)left * (float)right).ToExpression();
                    if(right is double) return ((byte)left * (double)right).ToExpression();
                    if(right is Complex) return ((byte)left * (Complex)right).ToExpression();
                    if(right is Vector2D) return ((byte)left * (Vector2D)right).ToExpression();
                    return ((byte)left * (right as Vector3D?))?.ToExpression();
                }
                if(left is sbyte)
                {
                    if(right is byte) return ((sbyte)left * (byte)right).ToExpression();
                    if(right is sbyte) return ((sbyte)left * (sbyte)right).ToExpression();
                    if(right is short) return ((sbyte)left * (short)right).ToExpression();
                    if(right is ushort) return ((sbyte)left * (ushort)right).ToExpression();
                    if(right is int) return ((sbyte)left * (int)right).ToExpression();
                    if(right is uint) return ((sbyte)left * (uint)right).ToExpression();
                    if(right is long) return ((sbyte)left * (long)right).ToExpression();
                    //if (right is ulong) return ((sbyte)left * (ulong)right).ToExpression();
                    if(right is float) return ((sbyte)left * (float)right).ToExpression();
                    if(right is double) return ((sbyte)left * (double)right).ToExpression();
                    if(right is Complex) return ((sbyte)left * (Complex)right).ToExpression();
                    if(right is Vector2D) return ((sbyte)left * (Vector2D)right).ToExpression();
                    return ((sbyte)left * (right as Vector3D?))?.ToExpression();
                }
                if(left is short)
                {
                    if(right is byte) return ((short)left * (byte)right).ToExpression();
                    if(right is sbyte) return ((short)left * (sbyte)right).ToExpression();
                    if(right is short) return ((short)left * (short)right).ToExpression();
                    if(right is ushort) return ((short)left * (ushort)right).ToExpression();
                    if(right is int) return ((short)left * (int)right).ToExpression();
                    if(right is uint) return ((short)left * (uint)right).ToExpression();
                    if(right is long) return ((short)left * (long)right).ToExpression();
                    //if(right is ulong) return ((short)left * (ulong)right).ToExpression();
                    if(right is float) return ((short)left * (float)right).ToExpression();
                    if(right is double) return ((short)left * (double)right).ToExpression();
                    if(right is Complex) return ((short)left * (Complex)right).ToExpression();
                    if(right is Vector2D) return ((short)left * (Vector2D)right).ToExpression();
                    return ((short)left * (right as Vector3D?))?.ToExpression();
                }
                if(left is ushort)
                {
                    if(right is byte) return ((ushort)left * (byte)right).ToExpression();
                    if(right is sbyte) return ((ushort)left * (sbyte)right).ToExpression();
                    if(right is short) return ((ushort)left * (short)right).ToExpression();
                    if(right is ushort) return ((ushort)left * (ushort)right).ToExpression();
                    if(right is int) return ((ushort)left * (int)right).ToExpression();
                    if(right is uint) return ((ushort)left * (uint)right).ToExpression();
                    if(right is long) return ((ushort)left * (long)right).ToExpression();
                    if(right is ulong) return ((ushort)left * (ulong)right).ToExpression();
                    if(right is float) return ((ushort)left * (float)right).ToExpression();
                    if(right is double) return ((ushort)left * (double)right).ToExpression();
                    if(right is Complex) return ((ushort)left * (Complex)right).ToExpression();
                    if(right is Vector2D) return ((ushort)left * (Vector2D)right).ToExpression();
                    return ((ushort)left * (right as Vector3D?))?.ToExpression();
                }
                if(left is int)
                {
                    if(right is byte) return ((int)left * (byte)right).ToExpression();
                    if(right is sbyte) return ((int)left * (sbyte)right).ToExpression();
                    if(right is short) return ((int)left * (short)right).ToExpression();
                    if(right is ushort) return ((int)left * (ushort)right).ToExpression();
                    if(right is int) return ((int)left * (int)right).ToExpression();
                    if(right is uint) return ((int)left * (uint)right).ToExpression();
                    if(right is long) return ((int)left * (long)right).ToExpression();
                    //if(right is ulong) return ((int)left * (ulong)right).ToExpression();
                    if(right is float) return ((int)left * (float)right).ToExpression();
                    if(right is double) return ((int)left * (double)right).ToExpression();
                    if(right is Complex) return ((int)left * (Complex)right).ToExpression();
                    if(right is Vector2D) return ((int)left * (Vector2D)right).ToExpression();
                    return ((int)left * (right as Vector3D?))?.ToExpression();
                }
                if(left is uint)
                {
                    if(right is byte) return ((uint)left * (byte)right).ToExpression();
                    if(right is sbyte) return ((uint)left * (sbyte)right).ToExpression();
                    if(right is short) return ((uint)left * (short)right).ToExpression();
                    if(right is ushort) return ((uint)left * (ushort)right).ToExpression();
                    if(right is int) return ((uint)left * (int)right).ToExpression();
                    if(right is uint) return ((uint)left * (uint)right).ToExpression();
                    if(right is long) return ((uint)left * (long)right).ToExpression();
                    if(right is ulong) return ((uint)left * (ulong)right).ToExpression();
                    if(right is float) return ((uint)left * (float)right).ToExpression();
                    if(right is double) return ((uint)left * (double)right).ToExpression();
                    if(right is Complex) return ((uint)left * (Complex)right).ToExpression();
                    if(right is Vector2D) return ((uint)left * (Vector2D)right).ToExpression();
                    return ((uint)left * (right as Vector3D?))?.ToExpression();
                }
                if(left is long)
                {
                    if(right is byte) return ((long)left * (byte)right).ToExpression();
                    if(right is sbyte) return ((long)left * (sbyte)right).ToExpression();
                    if(right is short) return ((long)left * (short)right).ToExpression();
                    if(right is ushort) return ((long)left * (ushort)right).ToExpression();
                    if(right is int) return ((long)left * (int)right).ToExpression();
                    if(right is uint) return ((long)left * (uint)right).ToExpression();
                    if(right is long) return ((long)left * (long)right).ToExpression();
                    //if(right is ulong) return ((long)left * (ulong)right).ToExpression();
                    if(right is float) return ((long)left * (float)right).ToExpression();
                    if(right is double) return ((long)left * (double)right).ToExpression();
                    if(right is Complex) return ((long)left * (Complex)right).ToExpression();
                    if(right is Vector2D) return ((long)left * (Vector2D)right).ToExpression();
                    return ((long)left * (right as Vector3D?))?.ToExpression();
                }
                if(left is ulong)
                {
                    if(right is byte) return ((ulong)left * (byte)right).ToExpression();
                    //if(right is sbyte) return ((ulong)left * (sbyte)right).ToExpression();
                    //if(right is short) return ((ulong)left * (short)right).ToExpression();
                    if(right is ushort) return ((ulong)left * (ushort)right).ToExpression();
                    //if(right is int) return ((ulong)left * (int)right).ToExpression();
                    if(right is uint) return ((ulong)left * (uint)right).ToExpression();
                    //if(right is long) return ((ulong)left * (long)right).ToExpression();
                    if(right is ulong) return ((ulong)left * (ulong)right).ToExpression();
                    if(right is float) return ((ulong)left * (float)right).ToExpression();
                    if(right is double) return ((ulong)left * (double)right).ToExpression();
                    if(right is Complex) return ((ulong)left * (Complex)right).ToExpression();
                    if(right is Vector2D) return ((ulong)left * (Vector2D)right).ToExpression();
                    return ((ulong)left * (right as Vector3D?))?.ToExpression();
                }
                if(left is float)
                {
                    if(right is byte) return ((float)left * (byte)right).ToExpression();
                    if(right is sbyte) return ((float)left * (sbyte)right).ToExpression();
                    if(right is short) return ((float)left * (short)right).ToExpression();
                    if(right is ushort) return ((float)left * (ushort)right).ToExpression();
                    if(right is int) return ((float)left * (int)right).ToExpression();
                    if(right is uint) return ((float)left * (uint)right).ToExpression();
                    if(right is long) return ((float)left * (long)right).ToExpression();
                    if(right is ulong) return ((float)left * (ulong)right).ToExpression();
                    if(right is float) return ((float)left * (float)right).ToExpression();
                    if(right is double) return ((float)left * (double)right).ToExpression();
                    if(right is Complex) return ((float)left * (Complex)right).ToExpression();
                    if(right is Vector2D) return ((float)left * (Vector2D)right).ToExpression();
                    return ((float)left * (right as Vector3D?))?.ToExpression();
                }
                if(left is double)
                {
                    if(right is byte) return ((double)left * (byte)right).ToExpression();
                    if(right is sbyte) return ((double)left * (sbyte)right).ToExpression();
                    if(right is short) return ((double)left * (short)right).ToExpression();
                    if(right is ushort) return ((double)left * (ushort)right).ToExpression();
                    if(right is int) return ((double)left * (int)right).ToExpression();
                    if(right is uint) return ((double)left * (uint)right).ToExpression();
                    if(right is long) return ((double)left * (long)right).ToExpression();
                    if(right is ulong) return ((double)left * (ulong)right).ToExpression();
                    if(right is float) return ((double)left * (float)right).ToExpression();
                    if(right is double) return ((double)left * (double)right).ToExpression();
                    if(right is Complex) return ((double)left * (Complex)right).ToExpression();
                    if(right is Vector2D) return ((double)left * (Vector2D)right).ToExpression();
                    return ((double)left * (right as Vector3D?))?.ToExpression();
                }
                if(left is Complex)
                {
                    if(right is byte) return ((Complex)left * (byte)right).ToExpression();
                    if(right is sbyte) return ((Complex)left * (sbyte)right).ToExpression();
                    if(right is short) return ((Complex)left * (short)right).ToExpression();
                    if(right is ushort) return ((Complex)left * (ushort)right).ToExpression();
                    if(right is int) return ((Complex)left * (int)right).ToExpression();
                    if(right is uint) return ((Complex)left * (uint)right).ToExpression();
                    if(right is long) return ((Complex)left * (long)right).ToExpression();
                    if(right is ulong) return ((Complex)left * (ulong)right).ToExpression();
                    if(right is float) return ((Complex)left * (float)right).ToExpression();
                    if(right is double) return ((Complex)left * (double)right).ToExpression();
                    if(right is Complex) return ((Complex)left * (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Complex)left * (Vector2D)right).ToExpression();
                    return null;
                }
                if(left is Vector2D)
                {
                    if(right is byte) return ((Vector2D)left * (byte)right).ToExpression();
                    if(right is sbyte) return ((Vector2D)left * (sbyte)right).ToExpression();
                    if(right is short) return ((Vector2D)left * (short)right).ToExpression();
                    if(right is ushort) return ((Vector2D)left * (ushort)right).ToExpression();
                    if(right is int) return ((Vector2D)left * (int)right).ToExpression();
                    if(right is uint) return ((Vector2D)left * (uint)right).ToExpression();
                    if(right is long) return ((Vector2D)left * (long)right).ToExpression();
                    if(right is ulong) return ((Vector2D)left * (ulong)right).ToExpression();
                    if(right is float) return ((Vector2D)left * (float)right).ToExpression();
                    if(right is double) return ((Vector2D)left * (double)right).ToExpression();
                    //if(right is Complex) return ((Vector2D)left * (Complex)right).ToExpression();
                    if(right is Vector2D) return ((Vector2D)left * (Vector2D)right).ToExpression();
                    return null;
                }
                if(left is Vector3D)
                {
                    if(right is byte) return ((Vector3D)left * (byte)right).ToExpression();
                    if(right is sbyte) return ((Vector3D)left * (sbyte)right).ToExpression();
                    if(right is short) return ((Vector3D)left * (short)right).ToExpression();
                    if(right is ushort) return ((Vector3D)left * (ushort)right).ToExpression();
                    if(right is int) return ((Vector3D)left * (int)right).ToExpression();
                    if(right is uint) return ((Vector3D)left * (uint)right).ToExpression();
                    if(right is long) return ((Vector3D)left * (long)right).ToExpression();
                    if(right is ulong) return ((Vector3D)left * (ulong)right).ToExpression();
                    if(right is float) return ((Vector3D)left * (float)right).ToExpression();
                    if(right is double) return ((Vector3D)left * (double)right).ToExpression();
                    //if(right is Complex) return ((Vector3D)left * (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Vector3D)left * (Vector2D)right).ToExpression();
                    return ((Vector3D)left * (right as Vector3D?))?.ToExpression();
                }
                return null;
            }

            private static Ex DivideSimplify(bEx expr)
            {
                if(IsZero((expr.Left as cEx)?.Value)) return expr.Left;
                if(IsUnit((expr.Right as cEx)?.Value)) return expr.Left;

                return DivadeValues((expr.Left as cEx)?.Value, (expr.Right as cEx)?.Value) ?? expr;
            }

            private static Ex DivadeValues(object left, object right)
            {
                if(!IsNumeric(left) || !IsNumeric(right)) return null;
                if(left is byte)
                {
                    if(IsZero(right))
                    {
                        if(right is double)
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0 ? double.PositiveInfinity : double.NegativeInfinity).ToExpression();
                        if(!(right is float)) return Ex.Throw(new DivideByZeroException().ToExpression());
                        return float.IsNaN((float)right)
                            ? float.NaN.ToExpression()
                            : ((float)right > 0 ? float.PositiveInfinity : float.NegativeInfinity).ToExpression();
                    }
                    if(right is byte) return ((byte)left / (byte)right).ToExpression();
                    if(right is sbyte) return ((byte)left / (sbyte)right).ToExpression();
                    if(right is short) return ((byte)left / (short)right).ToExpression();
                    if(right is ushort) return ((byte)left / (ushort)right).ToExpression();
                    if(right is int) return ((byte)left / (int)right).ToExpression();
                    if(right is uint) return ((byte)left / (uint)right).ToExpression();
                    if(right is long) return ((byte)left / (long)right).ToExpression();
                    if(right is ulong) return ((byte)left / (ulong)right).ToExpression();
                    if(right is float) return ((byte)left / (float)right).ToExpression();
                    if(right is double) return ((byte)left / (double)right).ToExpression();
                    if(right is Complex) return ((byte)left / (Complex)right).ToExpression();
                    if(right is Vector2D) return ((byte)left / (Vector2D)right).ToExpression();
                    return ((byte)left / (right as Vector3D?))?.ToExpression();
                }
                if(left is sbyte)
                {
                    if(IsZero(right))
                    {
                        if(right is double)
                        {
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0
                                    ? ((sbyte)left > 0 ? double.PositiveInfinity : double.NegativeInfinity)
                                    : ((sbyte)left > 0 ? double.NegativeInfinity : double.PositiveInfinity))
                                    .ToExpression();
                        }
                        if(right is float)
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
                    if(right is byte) return ((sbyte)left / (byte)right).ToExpression();
                    if(right is sbyte) return ((sbyte)left / (sbyte)right).ToExpression();
                    if(right is short) return ((sbyte)left / (short)right).ToExpression();
                    if(right is ushort) return ((sbyte)left / (ushort)right).ToExpression();
                    if(right is int) return ((sbyte)left / (int)right).ToExpression();
                    if(right is uint) return ((sbyte)left / (uint)right).ToExpression();
                    if(right is long) return ((sbyte)left / (long)right).ToExpression();
                    //if (right is ulong) return ((sbyte)left / (ulong)right).ToExpression();
                    if(right is float) return ((sbyte)left / (float)right).ToExpression();
                    if(right is double) return ((sbyte)left / (double)right).ToExpression();
                    if(right is Complex) return ((sbyte)left / (Complex)right).ToExpression();
                    if(right is Vector2D) return ((sbyte)left / (Vector2D)right).ToExpression();
                    return ((sbyte)left / (right as Vector3D?))?.ToExpression();
                }
                if(left is short)
                {
                    if(IsZero(right))
                    {
                        if(right is double)
                        {
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0
                                    ? ((short)left > 0 ? double.PositiveInfinity : double.NegativeInfinity)
                                    : ((short)left > 0 ? double.NegativeInfinity : double.PositiveInfinity))
                                    .ToExpression();
                        }
                        if(right is float)
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
                    if(right is byte) return ((short)left / (byte)right).ToExpression();
                    if(right is sbyte) return ((short)left / (sbyte)right).ToExpression();
                    if(right is short) return ((short)left / (short)right).ToExpression();
                    if(right is ushort) return ((short)left / (ushort)right).ToExpression();
                    if(right is int) return ((short)left / (int)right).ToExpression();
                    if(right is uint) return ((short)left / (uint)right).ToExpression();
                    if(right is long) return ((short)left / (long)right).ToExpression();
                    //if(right is ulong) return ((short)left / (ulong)right).ToExpression();
                    if(right is float) return ((short)left / (float)right).ToExpression();
                    if(right is double) return ((short)left / (double)right).ToExpression();
                    if(right is Complex) return ((short)left / (Complex)right).ToExpression();
                    if(right is Vector2D) return ((short)left / (Vector2D)right).ToExpression();
                    return ((short)left / (right as Vector3D?))?.ToExpression();
                }
                if(left is ushort)
                {
                    if(IsZero(right))
                    {
                        if(right is double)
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0 ? double.PositiveInfinity : double.NegativeInfinity).ToExpression();
                        if(!(right is float)) return Ex.Throw(new DivideByZeroException().ToExpression());
                        return float.IsNaN((float)right)
                            ? float.NaN.ToExpression()
                            : ((float)right > 0 ? float.PositiveInfinity : float.NegativeInfinity).ToExpression();
                    }
                    if(right is byte) return ((ushort)left / (byte)right).ToExpression();
                    if(right is sbyte) return ((ushort)left / (sbyte)right).ToExpression();
                    if(right is short) return ((ushort)left / (short)right).ToExpression();
                    if(right is ushort) return ((ushort)left / (ushort)right).ToExpression();
                    if(right is int) return ((ushort)left / (int)right).ToExpression();
                    if(right is uint) return ((ushort)left / (uint)right).ToExpression();
                    if(right is long) return ((ushort)left / (long)right).ToExpression();
                    if(right is ulong) return ((ushort)left / (ulong)right).ToExpression();
                    if(right is float) return ((ushort)left / (float)right).ToExpression();
                    if(right is double) return ((ushort)left / (double)right).ToExpression();
                    if(right is Complex) return ((ushort)left / (Complex)right).ToExpression();
                    if(right is Vector2D) return ((ushort)left / (Vector2D)right).ToExpression();
                    return ((ushort)left / (right as Vector3D?))?.ToExpression();
                }
                if(left is int)
                {
                    if(IsZero(right))
                    {
                        if(right is double)
                        {
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0
                                    ? ((int)left > 0 ? double.PositiveInfinity : double.NegativeInfinity)
                                    : ((int)left > 0 ? double.NegativeInfinity : double.PositiveInfinity))
                                    .ToExpression();
                        }
                        if(right is float)
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
                    if(right is byte) return ((int)left / (byte)right).ToExpression();
                    if(right is sbyte) return ((int)left / (sbyte)right).ToExpression();
                    if(right is short) return ((int)left / (short)right).ToExpression();
                    if(right is ushort) return ((int)left / (ushort)right).ToExpression();
                    if(right is int) return ((int)left / (int)right).ToExpression();
                    if(right is uint) return ((int)left / (uint)right).ToExpression();
                    if(right is long) return ((int)left / (long)right).ToExpression();
                    //if(right is ulong) return ((int)left / (ulong)right).ToExpression();
                    if(right is float) return ((int)left / (float)right).ToExpression();
                    if(right is double) return ((int)left / (double)right).ToExpression();
                    if(right is Complex) return ((int)left / (Complex)right).ToExpression();
                    if(right is Vector2D) return ((int)left / (Vector2D)right).ToExpression();
                    return ((int)left / (right as Vector3D?))?.ToExpression();
                }
                if(left is uint)
                {
                    if(IsZero(right))
                    {
                        if(right is double)
                        {
                            if(double.IsNaN((double)right)) return double.NaN.ToExpression();
                            return ((double)right > 0 ? double.PositiveInfinity : double.NegativeInfinity).ToExpression();
                        }
                        if(right is float)
                        {
                            if(float.IsNaN((float)right)) return float.NaN.ToExpression();
                            return ((float)right > 0 ? float.PositiveInfinity : float.NegativeInfinity).ToExpression();
                        }
                        return Ex.Throw(new DivideByZeroException().ToExpression());
                    }
                    if(right is byte) return ((uint)left / (byte)right).ToExpression();
                    if(right is sbyte) return ((uint)left / (sbyte)right).ToExpression();
                    if(right is short) return ((uint)left / (short)right).ToExpression();
                    if(right is ushort) return ((uint)left / (ushort)right).ToExpression();
                    if(right is int) return ((uint)left / (int)right).ToExpression();
                    if(right is uint) return ((uint)left / (uint)right).ToExpression();
                    if(right is long) return ((uint)left / (long)right).ToExpression();
                    if(right is ulong) return ((uint)left / (ulong)right).ToExpression();
                    if(right is float) return ((uint)left / (float)right).ToExpression();
                    if(right is double) return ((uint)left / (double)right).ToExpression();
                    if(right is Complex) return ((uint)left / (Complex)right).ToExpression();
                    if(right is Vector2D) return ((uint)left / (Vector2D)right).ToExpression();
                    return ((uint)left / (right as Vector3D?))?.ToExpression();
                }
                if(left is long)
                {
                    if(IsZero(right))
                    {
                        if(right is double)
                        {
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0
                                    ? ((long)left > 0 ? double.PositiveInfinity : double.NegativeInfinity)
                                    : ((long)left > 0 ? double.NegativeInfinity : double.PositiveInfinity))
                                    .ToExpression();
                        }
                        if(right is float)
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
                    if(right is byte) return ((long)left / (byte)right).ToExpression();
                    if(right is sbyte) return ((long)left / (sbyte)right).ToExpression();
                    if(right is short) return ((long)left / (short)right).ToExpression();
                    if(right is ushort) return ((long)left / (ushort)right).ToExpression();
                    if(right is int) return ((long)left / (int)right).ToExpression();
                    if(right is uint) return ((long)left / (uint)right).ToExpression();
                    if(right is long) return ((long)left / (long)right).ToExpression();
                    //if(right is ulong) return ((long)left / (ulong)right).ToExpression();
                    if(right is float) return ((long)left / (float)right).ToExpression();
                    if(right is double) return ((long)left / (double)right).ToExpression();
                    if(right is Complex) return ((long)left / (Complex)right).ToExpression();
                    if(right is Vector2D) return ((long)left / (Vector2D)right).ToExpression();
                    return ((long)left / (right as Vector3D?))?.ToExpression();
                }
                if(left is ulong)
                {
                    if(IsZero(right))
                    {
                        if(right is double)
                        {
                            if(double.IsNaN((double)right)) return double.NaN.ToExpression();
                            return ((double)right > 0 ? double.PositiveInfinity : double.NegativeInfinity).ToExpression();
                        }
                        if(right is float)
                        {
                            if(float.IsNaN((float)right)) return float.NaN.ToExpression();
                            return ((float)right > 0 ? float.PositiveInfinity : float.NegativeInfinity).ToExpression();
                        }
                        return Ex.Throw(new DivideByZeroException().ToExpression());
                    }
                    if(right is byte) return ((ulong)left / (byte)right).ToExpression();
                    //if(right is sbyte) return ((ulong)left / (sbyte)right).ToExpression();
                    //if(right is short) return ((ulong)left / (short)right).ToExpression();
                    if(right is ushort) return ((ulong)left / (ushort)right).ToExpression();
                    //if(right is int) return ((ulong)left / (int)right).ToExpression();
                    if(right is uint) return ((ulong)left / (uint)right).ToExpression();
                    //if(right is long) return ((ulong)left / (long)right).ToExpression();
                    if(right is ulong) return ((ulong)left / (ulong)right).ToExpression();
                    if(right is float) return ((ulong)left / (float)right).ToExpression();
                    if(right is double) return ((ulong)left / (double)right).ToExpression();
                    if(right is Complex) return ((ulong)left / (Complex)right).ToExpression();
                    if(right is Vector2D) return ((ulong)left / (Vector2D)right).ToExpression();
                    return ((ulong)left / (right as Vector3D?))?.ToExpression();
                }
                if(left is float)
                {
                    if(IsZero(right))
                    {
                        if(right is double)
                        {
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0
                                    ? ((float)left > 0 ? double.PositiveInfinity : double.NegativeInfinity)
                                    : ((float)left > 0 ? double.NegativeInfinity : double.PositiveInfinity))
                                    .ToExpression();
                        }
                        if(right is float)
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
                    if(right is byte) return ((float)left / (byte)right).ToExpression();
                    if(right is sbyte) return ((float)left / (sbyte)right).ToExpression();
                    if(right is short) return ((float)left / (short)right).ToExpression();
                    if(right is ushort) return ((float)left / (ushort)right).ToExpression();
                    if(right is int) return ((float)left / (int)right).ToExpression();
                    if(right is uint) return ((float)left / (uint)right).ToExpression();
                    if(right is long) return ((float)left / (long)right).ToExpression();
                    if(right is ulong) return ((float)left / (ulong)right).ToExpression();
                    if(right is float) return ((float)left / (float)right).ToExpression();
                    if(right is double) return ((float)left / (double)right).ToExpression();
                    if(right is Complex) return ((float)left / (Complex)right).ToExpression();
                    if(right is Vector2D) return ((float)left / (Vector2D)right).ToExpression();
                    return ((float)left / (right as Vector3D?))?.ToExpression();
                }
                if(left is double)
                {
                    if(IsZero(right))
                    {
                        if(right is double)
                        {
                            return double.IsNaN((double)right)
                                ? double.NaN.ToExpression()
                                : ((double)right > 0
                                    ? ((double)left > 0 ? double.PositiveInfinity : double.NegativeInfinity)
                                    : ((double)left > 0 ? double.NegativeInfinity : double.PositiveInfinity))
                                    .ToExpression();
                        }
                        if(right is float)
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
                    if(right is byte) return ((double)left / (byte)right).ToExpression();
                    if(right is sbyte) return ((double)left / (sbyte)right).ToExpression();
                    if(right is short) return ((double)left / (short)right).ToExpression();
                    if(right is ushort) return ((double)left / (ushort)right).ToExpression();
                    if(right is int) return ((double)left / (int)right).ToExpression();
                    if(right is uint) return ((double)left / (uint)right).ToExpression();
                    if(right is long) return ((double)left / (long)right).ToExpression();
                    if(right is ulong) return ((double)left / (ulong)right).ToExpression();
                    if(right is float) return ((double)left / (float)right).ToExpression();
                    if(right is double) return ((double)left / (double)right).ToExpression();
                    if(right is Complex) return ((double)left / (Complex)right).ToExpression();
                    if(right is Vector2D) return ((double)left / (Vector2D)right).ToExpression();
                    return ((double)left / (right as Vector3D?))?.ToExpression();
                }
                if(left is Complex)
                {
                    if(IsZero(right))
                        return Ex.Throw(new DivideByZeroException().ToExpression());
                    if(right is byte) return ((Complex)left / (byte)right).ToExpression();
                    if(right is sbyte) return ((Complex)left / (sbyte)right).ToExpression();
                    if(right is short) return ((Complex)left / (short)right).ToExpression();
                    if(right is ushort) return ((Complex)left / (ushort)right).ToExpression();
                    if(right is int) return ((Complex)left / (int)right).ToExpression();
                    if(right is uint) return ((Complex)left / (uint)right).ToExpression();
                    if(right is long) return ((Complex)left / (long)right).ToExpression();
                    if(right is ulong) return ((Complex)left / (ulong)right).ToExpression();
                    if(right is float) return ((Complex)left / (float)right).ToExpression();
                    if(right is double) return ((Complex)left / (double)right).ToExpression();
                    if(right is Complex) return ((Complex)left / (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Complex)left / (Vector2D)right).ToExpression();
                    return null;
                }
                if(left is Vector2D)
                {
                    if(IsZero(right))
                        return Ex.Throw(new DivideByZeroException().ToExpression());
                    if(right is byte) return ((Vector2D)left / (byte)right).ToExpression();
                    if(right is sbyte) return ((Vector2D)left / (sbyte)right).ToExpression();
                    if(right is short) return ((Vector2D)left / (short)right).ToExpression();
                    if(right is ushort) return ((Vector2D)left / (ushort)right).ToExpression();
                    if(right is int) return ((Vector2D)left / (int)right).ToExpression();
                    if(right is uint) return ((Vector2D)left / (uint)right).ToExpression();
                    if(right is long) return ((Vector2D)left / (long)right).ToExpression();
                    if(right is ulong) return ((Vector2D)left / (ulong)right).ToExpression();
                    if(right is float) return ((Vector2D)left / (float)right).ToExpression();
                    return ((Vector2D)left / (right as double?))?.ToExpression();
                    //if(right is Complex) return ((Vector2D)left / (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Vector2D)left / (Vector2D)right).ToExpression();
                }
                if(left is Vector3D)
                {
                    if(IsZero(right))
                        return Ex.Throw(new DivideByZeroException().ToExpression());
                    if(right is byte) return ((Vector3D)left / (byte)right).ToExpression();
                    if(right is sbyte) return ((Vector3D)left / (sbyte)right).ToExpression();
                    if(right is short) return ((Vector3D)left / (short)right).ToExpression();
                    if(right is ushort) return ((Vector3D)left / (ushort)right).ToExpression();
                    if(right is int) return ((Vector3D)left / (int)right).ToExpression();
                    if(right is uint) return ((Vector3D)left / (uint)right).ToExpression();
                    if(right is long) return ((Vector3D)left / (long)right).ToExpression();
                    if(right is ulong) return ((Vector3D)left / (ulong)right).ToExpression();
                    if(right is float) return ((Vector3D)left / (float)right).ToExpression();
                    if(right is double) return ((Vector3D)left / (double)right).ToExpression();
                    //if(right is Complex) return ((Vector3D)left / (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Vector3D)left / (Vector2D)right).ToExpression();
                    //return ((Vector3D)left / (right as Vector3D?))?.ToExpression();
                }
                return null;
            }

            private static Ex AdditionSimplify(bEx expr)
            {
                var right = expr.Right;
                var left = expr.Left;
                if(IsZero((left as cEx)?.Value)) return right;
                if(IsZero((right as cEx)?.Value)) return left;

                //if(right.NodeType == ExpressionType.Add || right.NodeType == ExpressionType.Subtract)
                //{
                //    var right_operands = GetOperands_Addition(right as bEx).ToArray();
                //    var consts = right_operands.Where(e => e is cEx || e.NodeType == ExpressionType.Negate && ((uEx)e).Operand is cEx).ToList();
                //    var vars = right_operands.Except(consts).ToList();

                //    Expression sum = null;
                //    while(sum == null && consts.Count > 0)
                //        if()

                //            if(consts.Count > 1)
                //            {
                //                for(var i = 0; i < consts.Count; i++)
                //                {
                //                    var s = AddValues((sum as cEx)?.Value, (consts[i] as cEx)?.Value);
                //                    if(s == null)
                //        }
                //            }
                //}


                return AddValues((left as cEx)?.Value, (right as cEx)?.Value) ?? expr;
            }

            private static IEnumerable<Expression> GetOperands_Addition(bEx expr)
            {
                if(expr == null || expr.NodeType != ExpressionType.Add || expr.NodeType != ExpressionType.Subtract) yield break;

                var left = expr.Left;
                if(left is bEx && left.NodeType == ExpressionType.Add || left.NodeType == ExpressionType.Subtract)
                    foreach(var item in GetOperands_Addition(left as bEx))
                        yield return item;
                else
                    yield return left;

                var right = expr.Right;
                if(right is bEx && right.NodeType == ExpressionType.Add || right.NodeType == ExpressionType.Subtract)
                    if(expr.NodeType == ExpressionType.Add)
                        foreach(var item in GetOperands_Addition(left as bEx))
                            yield return item;
                    else
                        foreach(var item in GetOperands_Addition(left as bEx))
                            if(item.NodeType == ExpressionType.Negate)
                                yield return ((UnaryExpression)item).Operand;
                            else
                                yield return item.Negate();
                else
                    yield return right;
            }

            private static Ex AddValues(object left, object right)
            {
                if(!IsNumeric(left) || !IsNumeric(right)) return null;
                if(left is byte)
                {
                    if(right is byte) return ((byte)left + (byte)right).ToExpression();
                    if(right is sbyte) return ((byte)left + (sbyte)right).ToExpression();
                    if(right is short) return ((byte)left + (short)right).ToExpression();
                    if(right is ushort) return ((byte)left + (ushort)right).ToExpression();
                    if(right is int) return ((byte)left + (int)right).ToExpression();
                    if(right is uint) return ((byte)left + (uint)right).ToExpression();
                    if(right is long) return ((byte)left + (long)right).ToExpression();
                    if(right is ulong) return ((byte)left + (ulong)right).ToExpression();
                    if(right is float) return ((byte)left + (float)right).ToExpression();
                    if(right is double) return ((byte)left + (double)right).ToExpression();
                    if(right is Complex) return ((byte)left + (Complex)right).ToExpression();
                    if(right is Vector2D) return ((byte)left + (Vector2D)right).ToExpression();
                    return ((byte)left + (right as Vector3D?))?.ToExpression();
                }
                if(left is sbyte)
                {
                    if(right is byte) return ((sbyte)left + (byte)right).ToExpression();
                    if(right is sbyte) return ((sbyte)left + (sbyte)right).ToExpression();
                    if(right is short) return ((sbyte)left + (short)right).ToExpression();
                    if(right is ushort) return ((sbyte)left + (ushort)right).ToExpression();
                    if(right is int) return ((sbyte)left + (int)right).ToExpression();
                    if(right is uint) return ((sbyte)left + (uint)right).ToExpression();
                    if(right is long) return ((sbyte)left + (long)right).ToExpression();
                    //if(right is ulong) return ((sbyte)left + (ulong)right).ToExpression();
                    if(right is float) return ((sbyte)left + (float)right).ToExpression();
                    if(right is double) return ((sbyte)left + (double)right).ToExpression();
                    if(right is Complex) return ((sbyte)left + (Complex)right).ToExpression();
                    if(right is Vector2D) return ((sbyte)left + (Vector2D)right).ToExpression();
                    return ((sbyte)left + (right as Vector3D?))?.ToExpression();
                }
                if(left is short)
                {
                    if(right is byte) return ((short)left + (byte)right).ToExpression();
                    if(right is sbyte) return ((short)left + (sbyte)right).ToExpression();
                    if(right is short) return ((short)left + (short)right).ToExpression();
                    if(right is ushort) return ((short)left + (ushort)right).ToExpression();
                    if(right is int) return ((short)left + (int)right).ToExpression();
                    if(right is uint) return ((short)left + (uint)right).ToExpression();
                    if(right is long) return ((short)left + (long)right).ToExpression();
                    //if(right is ulong) return ((short)left + (ulong)right).ToExpression();
                    if(right is float) return ((short)left + (float)right).ToExpression();
                    if(right is double) return ((short)left + (double)right).ToExpression();
                    if(right is Complex) return ((short)left + (Complex)right).ToExpression();
                    if(right is Vector2D) return ((short)left + (Vector2D)right).ToExpression();
                    return ((short)left + (right as Vector3D?))?.ToExpression();
                }
                if(left is ushort)
                {
                    if(right is byte) return ((ushort)left + (byte)right).ToExpression();
                    if(right is sbyte) return ((ushort)left + (sbyte)right).ToExpression();
                    if(right is short) return ((ushort)left + (short)right).ToExpression();
                    if(right is ushort) return ((ushort)left + (ushort)right).ToExpression();
                    if(right is int) return ((ushort)left + (int)right).ToExpression();
                    if(right is uint) return ((ushort)left + (uint)right).ToExpression();
                    if(right is long) return ((ushort)left + (long)right).ToExpression();
                    if(right is ulong) return ((ushort)left + (ulong)right).ToExpression();
                    if(right is float) return ((ushort)left + (float)right).ToExpression();
                    if(right is double) return ((ushort)left + (double)right).ToExpression();
                    if(right is Complex) return ((ushort)left + (Complex)right).ToExpression();
                    if(right is Vector2D) return ((ushort)left + (Vector2D)right).ToExpression();
                    return ((ushort)left + (right as Vector3D?))?.ToExpression();
                }
                if(left is int)
                {
                    if(right is byte) return ((int)left + (byte)right).ToExpression();
                    if(right is sbyte) return ((int)left + (sbyte)right).ToExpression();
                    if(right is short) return ((int)left + (short)right).ToExpression();
                    if(right is ushort) return ((int)left + (ushort)right).ToExpression();
                    if(right is int) return ((int)left + (int)right).ToExpression();
                    if(right is uint) return ((int)left + (uint)right).ToExpression();
                    if(right is long) return ((int)left + (long)right).ToExpression();
                    //if(right is ulong) return ((int)left + (ulong)right).ToExpression();
                    if(right is float) return ((int)left + (float)right).ToExpression();
                    if(right is double) return ((int)left + (double)right).ToExpression();
                    if(right is Complex) return ((int)left + (Complex)right).ToExpression();
                    if(right is Vector2D) return ((int)left + (Vector2D)right).ToExpression();
                    return ((int)left + (right as Vector3D?))?.ToExpression();
                }
                if(left is uint)
                {
                    if(right is byte) return ((uint)left + (byte)right).ToExpression();
                    if(right is sbyte) return ((uint)left + (sbyte)right).ToExpression();
                    if(right is short) return ((uint)left + (short)right).ToExpression();
                    if(right is ushort) return ((uint)left + (ushort)right).ToExpression();
                    if(right is int) return ((uint)left + (int)right).ToExpression();
                    if(right is uint) return ((uint)left + (uint)right).ToExpression();
                    if(right is long) return ((uint)left + (long)right).ToExpression();
                    if(right is ulong) return ((uint)left + (ulong)right).ToExpression();
                    if(right is float) return ((uint)left + (float)right).ToExpression();
                    if(right is double) return ((uint)left + (double)right).ToExpression();
                    if(right is Complex) return ((uint)left + (Complex)right).ToExpression();
                    if(right is Vector2D) return ((uint)left + (Vector2D)right).ToExpression();
                    return ((uint)left + (right as Vector3D?))?.ToExpression();
                }
                if(left is long)
                {
                    if(right is byte) return ((long)left + (byte)right).ToExpression();
                    if(right is sbyte) return ((long)left + (sbyte)right).ToExpression();
                    if(right is short) return ((long)left + (short)right).ToExpression();
                    if(right is ushort) return ((long)left + (ushort)right).ToExpression();
                    if(right is int) return ((long)left + (int)right).ToExpression();
                    if(right is uint) return ((long)left + (uint)right).ToExpression();
                    if(right is long) return ((long)left + (long)right).ToExpression();
                    //if(right is ulong) return ((long)left + (ulong)right).ToExpression();
                    if(right is float) return ((long)left + (float)right).ToExpression();
                    if(right is double) return ((long)left + (double)right).ToExpression();
                    if(right is Complex) return ((long)left + (Complex)right).ToExpression();
                    if(right is Vector2D) return ((long)left + (Vector2D)right).ToExpression();
                    return ((long)left + (right as Vector3D?))?.ToExpression();
                }
                if(left is ulong)
                {
                    if(right is byte) return ((ulong)left + (byte)right).ToExpression();
                    //if(right is sbyte) return ((ulong)left + (sbyte)right).ToExpression();
                    //if(right is short) return ((ulong)left + (short)right).ToExpression();
                    if(right is ushort) return ((ulong)left + (ushort)right).ToExpression();
                    //if(right is int) return ((ulong)left + (int)right).ToExpression();
                    if(right is uint) return ((ulong)left + (uint)right).ToExpression();
                    //if(right is long) return ((ulong)left + (long)right).ToExpression();
                    if(right is ulong) return ((ulong)left + (ulong)right).ToExpression();
                    if(right is float) return ((ulong)left + (float)right).ToExpression();
                    if(right is double) return ((ulong)left + (double)right).ToExpression();
                    if(right is Complex) return ((ulong)left + (Complex)right).ToExpression();
                    if(right is Vector2D) return ((ulong)left + (Vector2D)right).ToExpression();
                    return ((ulong)left + (right as Vector3D?))?.ToExpression();
                }
                if(left is float)
                {
                    if(right is byte) return ((float)left + (byte)right).ToExpression();
                    if(right is sbyte) return ((float)left + (sbyte)right).ToExpression();
                    if(right is short) return ((float)left + (short)right).ToExpression();
                    if(right is ushort) return ((float)left + (ushort)right).ToExpression();
                    if(right is int) return ((float)left + (int)right).ToExpression();
                    if(right is uint) return ((float)left + (uint)right).ToExpression();
                    if(right is long) return ((float)left + (long)right).ToExpression();
                    if(right is ulong) return ((float)left + (ulong)right).ToExpression();
                    if(right is float) return ((float)left + (float)right).ToExpression();
                    if(right is double) return ((float)left + (double)right).ToExpression();
                    if(right is Complex) return ((float)left + (Complex)right).ToExpression();
                    if(right is Vector2D) return ((float)left + (Vector2D)right).ToExpression();
                    return ((float)left + (right as Vector3D?))?.ToExpression();
                }
                if(left is double)
                {
                    if(right is byte) return ((double)left + (byte)right).ToExpression();
                    if(right is sbyte) return ((double)left + (sbyte)right).ToExpression();
                    if(right is short) return ((double)left + (short)right).ToExpression();
                    if(right is ushort) return ((double)left + (ushort)right).ToExpression();
                    if(right is int) return ((double)left + (int)right).ToExpression();
                    if(right is uint) return ((double)left + (uint)right).ToExpression();
                    if(right is long) return ((double)left + (long)right).ToExpression();
                    if(right is ulong) return ((double)left + (ulong)right).ToExpression();
                    if(right is float) return ((double)left + (float)right).ToExpression();
                    if(right is double) return ((double)left + (double)right).ToExpression();
                    if(right is Complex) return ((double)left + (Complex)right).ToExpression();
                    if(right is Vector2D) return ((double)left + (Vector2D)right).ToExpression();
                    return ((double)left + (right as Vector3D?))?.ToExpression();
                }
                if(left is Complex)
                {
                    if(right is byte) return ((Complex)left + (byte)right).ToExpression();
                    if(right is sbyte) return ((Complex)left + (sbyte)right).ToExpression();
                    if(right is short) return ((Complex)left + (short)right).ToExpression();
                    if(right is ushort) return ((Complex)left + (ushort)right).ToExpression();
                    if(right is int) return ((Complex)left + (int)right).ToExpression();
                    if(right is uint) return ((Complex)left + (uint)right).ToExpression();
                    if(right is long) return ((Complex)left + (long)right).ToExpression();
                    if(right is ulong) return ((Complex)left + (ulong)right).ToExpression();
                    if(right is float) return ((Complex)left + (float)right).ToExpression();
                    if(right is double) return ((Complex)left + (double)right).ToExpression();
                    if(right is Complex) return ((Complex)left + (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Complex)left + (Vector2D)right).ToExpression();
                    return null;
                }
                if(left is Vector2D)
                {
                    if(right is byte) return ((Vector2D)left + (byte)right).ToExpression();
                    if(right is sbyte) return ((Vector2D)left + (sbyte)right).ToExpression();
                    if(right is short) return ((Vector2D)left + (short)right).ToExpression();
                    if(right is ushort) return ((Vector2D)left + (ushort)right).ToExpression();
                    if(right is int) return ((Vector2D)left + (int)right).ToExpression();
                    if(right is uint) return ((Vector2D)left + (uint)right).ToExpression();
                    if(right is long) return ((Vector2D)left + (long)right).ToExpression();
                    if(right is ulong) return ((Vector2D)left + (ulong)right).ToExpression();
                    if(right is float) return ((Vector2D)left + (float)right).ToExpression();
                    if(right is double) return ((Vector2D)left + (double)right).ToExpression();
                    //if(right is Complex) return ((Vector2D)left + (Complex)right).ToExpression();
                    if(right is Vector2D) return ((Vector2D)left + (Vector2D)right).ToExpression();
                    return null;
                }
                if(left is Vector3D)
                {
                    if(right is byte) return ((Vector3D)left + (byte)right).ToExpression();
                    if(right is sbyte) return ((Vector3D)left + (sbyte)right).ToExpression();
                    if(right is short) return ((Vector3D)left + (short)right).ToExpression();
                    if(right is ushort) return ((Vector3D)left + (ushort)right).ToExpression();
                    if(right is int) return ((Vector3D)left + (int)right).ToExpression();
                    if(right is uint) return ((Vector3D)left + (uint)right).ToExpression();
                    if(right is long) return ((Vector3D)left + (long)right).ToExpression();
                    if(right is ulong) return ((Vector3D)left + (ulong)right).ToExpression();
                    if(right is float) return ((Vector3D)left + (float)right).ToExpression();
                    if(right is double) return ((Vector3D)left + (double)right).ToExpression();
                    //if(right is Complex) return ((Vector3D)left + (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Vector3D)left + (Vector2D)right).ToExpression();
                    return ((Vector3D)left + (right as Vector3D?))?.ToExpression();
                }
                return null;
            }

            private static Ex SubstractionSimplify(bEx expr)
            {
                if(IsZero((expr.Left as cEx)?.Value)) return expr.Right.Negate();
                if(IsZero((expr.Right as cEx)?.Value)) return expr.Left;

                return SubstractValues((expr.Left as cEx)?.Value, (expr.Right as cEx)?.Value) ?? expr;
            }

            private static Ex SubstractValues(object left, object right)
            {
                if(!IsNumeric(left) || !IsNumeric(right)) return null;
                if(left is byte)
                {
                    if(right is byte) return ((byte)left - (byte)right).ToExpression();
                    if(right is sbyte) return ((byte)left - (sbyte)right).ToExpression();
                    if(right is short) return ((byte)left - (short)right).ToExpression();
                    if(right is ushort) return ((byte)left - (ushort)right).ToExpression();
                    if(right is int) return ((byte)left - (int)right).ToExpression();
                    if(right is uint) return ((byte)left - (uint)right).ToExpression();
                    if(right is long) return ((byte)left - (long)right).ToExpression();
                    if(right is ulong) return ((byte)left - (ulong)right).ToExpression();
                    if(right is float) return ((byte)left - (float)right).ToExpression();
                    if(right is double) return ((byte)left - (double)right).ToExpression();
                    if(right is Complex) return ((byte)left - (Complex)right).ToExpression();
                    if(right is Vector2D) return ((byte)left - (Vector2D)right).ToExpression();
                    return ((byte)left - (right as Vector3D?))?.ToExpression();
                }
                if(left is sbyte)
                {
                    if(right is byte) return ((sbyte)left - (byte)right).ToExpression();
                    if(right is sbyte) return ((sbyte)left - (sbyte)right).ToExpression();
                    if(right is short) return ((sbyte)left - (short)right).ToExpression();
                    if(right is ushort) return ((sbyte)left - (ushort)right).ToExpression();
                    if(right is int) return ((sbyte)left - (int)right).ToExpression();
                    if(right is uint) return ((sbyte)left - (uint)right).ToExpression();
                    if(right is long) return ((sbyte)left - (long)right).ToExpression();
                    //if(right is ulong) return ((sbyte)left - (ulong)right).ToExpression();
                    if(right is float) return ((sbyte)left - (float)right).ToExpression();
                    if(right is double) return ((sbyte)left - (double)right).ToExpression();
                    if(right is Complex) return ((sbyte)left - (Complex)right).ToExpression();
                    if(right is Vector2D) return ((sbyte)left - (Vector2D)right).ToExpression();
                    return ((sbyte)left - (right as Vector3D?))?.ToExpression();
                }
                if(left is short)
                {
                    if(right is byte) return ((short)left - (byte)right).ToExpression();
                    if(right is sbyte) return ((short)left - (sbyte)right).ToExpression();
                    if(right is short) return ((short)left - (short)right).ToExpression();
                    if(right is ushort) return ((short)left - (ushort)right).ToExpression();
                    if(right is int) return ((short)left - (int)right).ToExpression();
                    if(right is uint) return ((short)left - (uint)right).ToExpression();
                    if(right is long) return ((short)left - (long)right).ToExpression();
                    //if(right is ulong) return ((short)left - (ulong)right).ToExpression();
                    if(right is float) return ((short)left - (float)right).ToExpression();
                    if(right is double) return ((short)left - (double)right).ToExpression();
                    if(right is Complex) return ((short)left - (Complex)right).ToExpression();
                    if(right is Vector2D) return ((short)left - (Vector2D)right).ToExpression();
                    return ((short)left - (right as Vector3D?))?.ToExpression();
                }
                if(left is ushort)
                {
                    if(right is byte) return ((ushort)left - (byte)right).ToExpression();
                    if(right is sbyte) return ((ushort)left - (sbyte)right).ToExpression();
                    if(right is short) return ((ushort)left - (short)right).ToExpression();
                    if(right is ushort) return ((ushort)left - (ushort)right).ToExpression();
                    if(right is int) return ((ushort)left - (int)right).ToExpression();
                    if(right is uint) return ((ushort)left - (uint)right).ToExpression();
                    if(right is long) return ((ushort)left - (long)right).ToExpression();
                    if(right is ulong) return ((ushort)left - (ulong)right).ToExpression();
                    if(right is float) return ((ushort)left - (float)right).ToExpression();
                    if(right is double) return ((ushort)left - (double)right).ToExpression();
                    if(right is Complex) return ((ushort)left - (Complex)right).ToExpression();
                    if(right is Vector2D) return ((ushort)left - (Vector2D)right).ToExpression();
                    return ((ushort)left - (right as Vector3D?))?.ToExpression();
                }
                if(left is int)
                {
                    if(right is byte) return ((int)left - (byte)right).ToExpression();
                    if(right is sbyte) return ((int)left - (sbyte)right).ToExpression();
                    if(right is short) return ((int)left - (short)right).ToExpression();
                    if(right is ushort) return ((int)left - (ushort)right).ToExpression();
                    if(right is int) return ((int)left - (int)right).ToExpression();
                    if(right is uint) return ((int)left - (uint)right).ToExpression();
                    if(right is long) return ((int)left - (long)right).ToExpression();
                    //if(right is ulong) return ((int)left - (ulong)right).ToExpression();
                    if(right is float) return ((int)left - (float)right).ToExpression();
                    if(right is double) return ((int)left - (double)right).ToExpression();
                    if(right is Complex) return ((int)left - (Complex)right).ToExpression();
                    if(right is Vector2D) return ((int)left - (Vector2D)right).ToExpression();
                    return ((int)left - (right as Vector3D?))?.ToExpression();
                }
                if(left is uint)
                {
                    if(right is byte) return ((uint)left - (byte)right).ToExpression();
                    if(right is sbyte) return ((uint)left - (sbyte)right).ToExpression();
                    if(right is short) return ((uint)left - (short)right).ToExpression();
                    if(right is ushort) return ((uint)left - (ushort)right).ToExpression();
                    if(right is int) return ((uint)left - (int)right).ToExpression();
                    if(right is uint) return ((uint)left - (uint)right).ToExpression();
                    if(right is long) return ((uint)left - (long)right).ToExpression();
                    if(right is ulong) return ((uint)left - (ulong)right).ToExpression();
                    if(right is float) return ((uint)left - (float)right).ToExpression();
                    if(right is double) return ((uint)left - (double)right).ToExpression();
                    if(right is Complex) return ((uint)left - (Complex)right).ToExpression();
                    if(right is Vector2D) return ((uint)left - (Vector2D)right).ToExpression();
                    return ((uint)left - (right as Vector3D?))?.ToExpression();
                }
                if(left is long)
                {
                    if(right is byte) return ((long)left - (byte)right).ToExpression();
                    if(right is sbyte) return ((long)left - (sbyte)right).ToExpression();
                    if(right is short) return ((long)left - (short)right).ToExpression();
                    if(right is ushort) return ((long)left - (ushort)right).ToExpression();
                    if(right is int) return ((long)left - (int)right).ToExpression();
                    if(right is uint) return ((long)left - (uint)right).ToExpression();
                    if(right is long) return ((long)left - (long)right).ToExpression();
                    //if(right is ulong) return ((long)left - (ulong)right).ToExpression();
                    if(right is float) return ((long)left - (float)right).ToExpression();
                    if(right is double) return ((long)left - (double)right).ToExpression();
                    if(right is Complex) return ((long)left - (Complex)right).ToExpression();
                    if(right is Vector2D) return ((long)left - (Vector2D)right).ToExpression();
                    return ((long)left - (right as Vector3D?))?.ToExpression();
                }
                if(left is ulong)
                {
                    if(right is byte) return ((ulong)left - (byte)right).ToExpression();
                    //if(right is sbyte) return ((ulong)left - (sbyte)right).ToExpression();
                    //if(right is short) return ((ulong)left - (short)right).ToExpression();
                    if(right is ushort) return ((ulong)left - (ushort)right).ToExpression();
                    //if(right is int) return ((ulong)left - (int)right).ToExpression();
                    if(right is uint) return ((ulong)left - (uint)right).ToExpression();
                    //if(right is long) return ((ulong)left - (long)right).ToExpression();
                    if(right is ulong) return ((ulong)left - (ulong)right).ToExpression();
                    if(right is float) return ((ulong)left - (float)right).ToExpression();
                    if(right is double) return ((ulong)left - (double)right).ToExpression();
                    if(right is Complex) return ((ulong)left - (Complex)right).ToExpression();
                    if(right is Vector2D) return ((ulong)left - (Vector2D)right).ToExpression();
                    return ((ulong)left - (right as Vector3D?))?.ToExpression();
                }
                if(left is float)
                {
                    if(right is byte) return ((float)left - (byte)right).ToExpression();
                    if(right is sbyte) return ((float)left - (sbyte)right).ToExpression();
                    if(right is short) return ((float)left - (short)right).ToExpression();
                    if(right is ushort) return ((float)left - (ushort)right).ToExpression();
                    if(right is int) return ((float)left - (int)right).ToExpression();
                    if(right is uint) return ((float)left - (uint)right).ToExpression();
                    if(right is long) return ((float)left - (long)right).ToExpression();
                    if(right is ulong) return ((float)left - (ulong)right).ToExpression();
                    if(right is float) return ((float)left - (float)right).ToExpression();
                    if(right is double) return ((float)left - (double)right).ToExpression();
                    if(right is Complex) return ((float)left - (Complex)right).ToExpression();
                    if(right is Vector2D) return ((float)left - (Vector2D)right).ToExpression();
                    return ((float)left - (right as Vector3D?))?.ToExpression();
                }
                if(left is double)
                {
                    if(right is byte) return ((double)left - (byte)right).ToExpression();
                    if(right is sbyte) return ((double)left - (sbyte)right).ToExpression();
                    if(right is short) return ((double)left - (short)right).ToExpression();
                    if(right is ushort) return ((double)left - (ushort)right).ToExpression();
                    if(right is int) return ((double)left - (int)right).ToExpression();
                    if(right is uint) return ((double)left - (uint)right).ToExpression();
                    if(right is long) return ((double)left - (long)right).ToExpression();
                    if(right is ulong) return ((double)left - (ulong)right).ToExpression();
                    if(right is float) return ((double)left - (float)right).ToExpression();
                    if(right is double) return ((double)left - (double)right).ToExpression();
                    if(right is Complex) return ((double)left - (Complex)right).ToExpression();
                    if(right is Vector2D) return ((double)left - (Vector2D)right).ToExpression();
                    return ((double)left - (right as Vector3D?))?.ToExpression();
                }
                if(left is Complex)
                {
                    if(right is byte) return ((Complex)left - (byte)right).ToExpression();
                    if(right is sbyte) return ((Complex)left - (sbyte)right).ToExpression();
                    if(right is short) return ((Complex)left - (short)right).ToExpression();
                    if(right is ushort) return ((Complex)left - (ushort)right).ToExpression();
                    if(right is int) return ((Complex)left - (int)right).ToExpression();
                    if(right is uint) return ((Complex)left - (uint)right).ToExpression();
                    if(right is long) return ((Complex)left - (long)right).ToExpression();
                    if(right is ulong) return ((Complex)left - (ulong)right).ToExpression();
                    if(right is float) return ((Complex)left - (float)right).ToExpression();
                    if(right is double) return ((Complex)left - (double)right).ToExpression();
                    if(right is Complex) return ((Complex)left - (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Complex)left - (Vector2D)right).ToExpression();
                    return null;
                }
                if(left is Vector2D)
                {
                    if(right is byte) return ((Vector2D)left - (byte)right).ToExpression();
                    if(right is sbyte) return ((Vector2D)left - (sbyte)right).ToExpression();
                    if(right is short) return ((Vector2D)left - (short)right).ToExpression();
                    if(right is ushort) return ((Vector2D)left - (ushort)right).ToExpression();
                    if(right is int) return ((Vector2D)left - (int)right).ToExpression();
                    if(right is uint) return ((Vector2D)left - (uint)right).ToExpression();
                    if(right is long) return ((Vector2D)left - (long)right).ToExpression();
                    if(right is ulong) return ((Vector2D)left - (ulong)right).ToExpression();
                    if(right is float) return ((Vector2D)left - (float)right).ToExpression();
                    if(right is double) return ((Vector2D)left - (double)right).ToExpression();
                    //if(right is Complex) return ((Vector2D)left - (Complex)right).ToExpression();
                    if(right is Vector2D) return ((Vector2D)left - (Vector2D)right).ToExpression();
                    return null;
                }
                if(left is Vector3D)
                {
                    if(right is byte) return ((Vector3D)left - (byte)right).ToExpression();
                    if(right is sbyte) return ((Vector3D)left - (sbyte)right).ToExpression();
                    if(right is short) return ((Vector3D)left - (short)right).ToExpression();
                    if(right is ushort) return ((Vector3D)left - (ushort)right).ToExpression();
                    if(right is int) return ((Vector3D)left - (int)right).ToExpression();
                    if(right is uint) return ((Vector3D)left - (uint)right).ToExpression();
                    if(right is long) return ((Vector3D)left - (long)right).ToExpression();
                    if(right is ulong) return ((Vector3D)left - (ulong)right).ToExpression();
                    if(right is float) return ((Vector3D)left - (float)right).ToExpression();
                    if(right is double) return ((Vector3D)left - (double)right).ToExpression();
                    //if(right is Complex) return ((Vector3D)left - (Complex)right).ToExpression();
                    //if(right is Vector2D) return ((Vector3D)left - (Vector2D)right).ToExpression();
                    return ((Vector3D)left - (right as Vector3D?))?.ToExpression();
                }
                return null;
            }
        }
    }
}