using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
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

        public static MCEx GetCallExpression(this Delegate d, Ex arg) => d.Method.GetCallExpression(arg);
        public static MCEx GetCallExpression(this Delegate d, IEnumerable<Ex> arg) => d.Method.GetCallExpression(arg);
        public static MCEx GetCallExpression(this Delegate d, params Ex[] arg) => d.Method.GetCallExpression(arg);

        public static MCEx GetCallExpression(this MethodInfo d, Ex arg) => Ex.Call(d, arg);
        public static MCEx GetCallExpression(this MethodInfo d, IEnumerable<Ex> arg) => Ex.Call(d, arg);
        public static MCEx GetCallExpression(this MethodInfo d, params Ex[] arg) => Ex.Call(d, arg);
        public static MCEx GetCallExpression(this MethodInfo d, Ex instance, params Ex[] arg) => Ex.Call(instance, d, arg);

        public static InvocationExpression GetInvokeExpression(this Delegate d, IEnumerable<Ex> arg) => d.ToExpression().GetInvoke(arg);
        public static InvocationExpression GetInvokeExpression(this Delegate d, params Ex[] arg) => d.ToExpression().GetInvoke(arg);

        #endregion

        #region Action

        public static Task InvokeAsync(this Action action) => Task.Factory.FromAsync(action.BeginInvoke, action.EndInvoke, null);
        public static Task InvokeAsync<T>(this Action<T> action, T parameter) => Task.Factory.FromAsync(action.BeginInvoke, action.EndInvoke, parameter, null);

        public static Action TryCatch(this Action action, Action OnException = null)
        {
            return () =>
            {
                try
                {
                    action();
                } catch(Exception)
                {
                    OnException?.Invoke();
                }
            };
        }

        public static Action<T> TryCatch<T>(this Action<T> action, Action<T> OnException = null)
        {
            return t =>
            {
                try
                {
                    action(t);
                } catch(Exception)
                {
                    OnException?.Invoke(t);
                }
            };
        }

        public static Action TryCatch<TException>(this Action action, Action OnException = null)
            where TException : Exception
        {
            return () =>
            {
                try
                {
                    action();
                } catch(TException)
                {
                    OnException?.Invoke();
                }
            };
        }

        public static Action<T> TryCatch<T, TException>(this Action<T> action, Action<T> OnException = null)
            where TException : Exception
        {
            return t =>
            {
                try
                {
                    action(t);
                } catch(TException)
                {
                    OnException?.Invoke(t);
                }
            };
        }

        #endregion

        #region Func

        public static Func<T> TryCatch<T>(this Func<T> func, Func<T> OnException = null)
        {
            return () =>
            {
                try
                {
                    return func();
                } catch(Exception)
                {
                    return OnException is null ? default : OnException();
                }
            };
        }

        public static Func<TIn, TOut> TryCatch<TIn, TOut>(this Func<TIn, TOut> func, Func<TIn, TOut> OnException = null)
        {
            return t =>
            {
                try
                {
                    return func(t);
                } catch(Exception)
                {
                    return OnException is null ? default : OnException(t);
                }
            };
        }

        public static Func<T> TryCatch<T, TException>(this Func<T> func, Func<T> OnException = null)
            where TException : Exception
        {
            return () =>
            {
                try
                {
                    return func();
                } catch(TException)
                {
                    return OnException is null ? default : OnException();
                }
            };
        }

        public static Func<TIn, TOut> TryCatch<TIn, TOut, TException>(this Func<TIn, TOut> func,
            Func<TIn, TOut> OnException = null)
            where TException : Exception
        {
            return t =>
            {
                try
                {
                    return func(t);
                } catch(TException)
                {
                    return OnException is null ? default : OnException(t);
                }
            };
        }

        #endregion

    }
}