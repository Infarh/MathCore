using System.Collections.Generic;
using System.Reflection;
using MathCore.Extensions.Expressions;
// ReSharper disable UnusedType.Global
// ReSharper disable AnnotateNotNullTypeMember

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions
{
    public class Method<TResult>
    {
        public static Expression<Func<TObject, TResult>> GetInvokerExpression<TObject>(MethodInfo method)
        {
            var p = "obj".ParameterOf(typeof(TObject));
            return p.GetCall(method).CreateLambda<Func<TObject, TResult>>(p);
        }

        private static readonly Dictionary<MethodInfo, Delegate> __InvokersDictionary =
            new Dictionary<MethodInfo, Delegate>();

        public static Func<TObject, TResult> GetInvoker<TObject>(MethodInfo method) =>
            (Func<TObject, TResult>)__InvokersDictionary.GetValueOrAddNew(method,
                m => GetInvokerExpression<TObject>(m).Compile());
    }
}