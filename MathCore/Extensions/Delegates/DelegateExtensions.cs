using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MathCore.Annotations;
using MathCore.Extensions.Expressions;
using MCEx = System.Linq.Expressions.MethodCallExpression;
using Ex = System.Linq.Expressions.Expression;
// ReSharper disable UnusedMember.Global
// ReSharper disable CatchAllClause

// ReSharper disable once CheckNamespace
namespace System
{
    public static class DelegateExtensions
    {
        #region Expressions

        [NotNull] public static MCEx GetCallExpression([NotNull] this Delegate d, [NotNull] Ex arg) => d.Method.GetCallExpression(arg);
        [NotNull] public static MCEx GetCallExpression([NotNull] this Delegate d, IEnumerable<Ex> arg) => d.Method.GetCallExpression(arg);
        [NotNull] public static MCEx GetCallExpression([NotNull] this Delegate d, params Ex[] arg) => d.Method.GetCallExpression(arg);

        [NotNull] public static MCEx GetCallExpression([NotNull] this MethodInfo d, [NotNull] Ex arg) => Ex.Call(d, arg);
        [NotNull] public static MCEx GetCallExpression([NotNull] this MethodInfo d, IEnumerable<Ex> arg) => Ex.Call(d, arg);
        [NotNull] public static MCEx GetCallExpression([NotNull] this MethodInfo d, params Ex[] arg) => Ex.Call(d, arg);
        [NotNull] public static MCEx GetCallExpression([NotNull] this MethodInfo d, Ex instance, params Ex[] arg) => Ex.Call(instance, d, arg);

        [NotNull] public static InvocationExpression GetInvokeExpression(this Delegate d, IEnumerable<Ex> arg) => d.ToExpression().GetInvoke(arg);
        [NotNull] public static InvocationExpression GetInvokeExpression(this Delegate d, params Ex[] arg) => d.ToExpression().GetInvoke(arg);

        #endregion

        #region Action

        [NotNull] public static Task InvokeAsync([NotNull] this Action action) => Task.Factory.FromAsync(action.BeginInvoke, action.EndInvoke, null);
        [NotNull] public static Task InvokeAsync<T>([NotNull] this Action<T> action, T parameter) => Task.Factory.FromAsync(action.BeginInvoke, action.EndInvoke, parameter, null);

        [NotNull]
        public static Action TryCatch(this Action action, Action OnException = null) =>
            () =>
            {
                try
                {
                    action();
                } catch(Exception)
                {
                    OnException?.Invoke();
                }
            };

        [NotNull]
        public static Action<T> TryCatch<T>(this Action<T> action, Action<T> OnException = null) =>
            t =>
            {
                try
                {
                    action(t);
                } catch(Exception)
                {
                    OnException?.Invoke(t);
                }
            };

        [NotNull]
        public static Action TryCatch<TException>(this Action action, Action OnException = null)
            where TException : Exception =>
            () =>
            {
                try
                {
                    action();
                } catch(TException)
                {
                    OnException?.Invoke();
                }
            };

        [NotNull]
        public static Action<T> TryCatch<T, TException>(this Action<T> action, Action<T> OnException = null)
            where TException : Exception =>
            t =>
            {
                try
                {
                    action(t);
                } catch(TException)
                {
                    OnException?.Invoke(t);
                }
            };

        #endregion

        #region Func

        [NotNull]
        public static Func<T> TryCatch<T>(this Func<T> func, Func<T> OnException = null) =>
            () =>
            {
                try
                {
                    return func();
                } catch(Exception)
                {
                    return OnException is null ? default : OnException();
                }
            };

        [NotNull]
        public static Func<TIn, TOut> TryCatch<TIn, TOut>(this Func<TIn, TOut> func, Func<TIn, TOut> OnException = null) =>
            t =>
            {
                try
                {
                    return func(t);
                } catch(Exception)
                {
                    return OnException is null ? default : OnException(t);
                }
            };

        [NotNull]
        public static Func<T> TryCatch<T, TException>(this Func<T> func, Func<T> OnException = null)
            where TException : Exception =>
            () =>
            {
                try
                {
                    return func();
                } catch(TException)
                {
                    return OnException is null ? default : OnException();
                }
            };

        [NotNull]
        public static Func<TIn, TOut> TryCatch<TIn, TOut, TException>(this Func<TIn, TOut> func,
            Func<TIn, TOut> OnException = null)
            where TException : Exception =>
            t =>
            {
                try
                {
                    return func(t);
                } catch(TException)
                {
                    return OnException is null ? default : OnException(t);
                }
            };

        #endregion

    }
}