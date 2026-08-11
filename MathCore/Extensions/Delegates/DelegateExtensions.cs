using System.Linq.Expressions;
using System.Reflection;

using MathCore.Extensions.Expressions;

using Ex = System.Linq.Expressions.Expression;
using MCEx = System.Linq.Expressions.MethodCallExpression;
// ReSharper disable UnusedMember.Global
// ReSharper disable CatchAllClause

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Расширения для работы с делегатами</summary>
public static class DelegateExtensions
{
    #region Expressions

    /// <summary>Создаёт выражение вызова метода для делегата с одним аргументом</summary>
    /// <param name="d">Делегат</param>
    /// <param name="arg">Аргумент</param>
    /// <returns>Выражение вызова метода</returns>
    public static MCEx GetCallExpression(this Delegate d, Ex arg) => d.Method.GetCallExpression(arg);

    /// <summary>Создаёт выражение вызова метода для делегата с коллекцией аргументов</summary>
    /// <param name="d">Делегат</param>
    /// <param name="arg">Коллекция аргументов</param>
    /// <returns>Выражение вызова метода</returns>
    public static MCEx GetCallExpression(this Delegate d, params IEnumerable<Ex> arg) => d.Method.GetCallExpression(arg);

    /// <summary>Создаёт выражение вызова метода для метода с одним аргументом</summary>
    /// <param name="d">Метод</param>
    /// <param name="arg">Аргумент</param>
    /// <returns>Выражение вызова метода</returns>
    public static MCEx GetCallExpression(this MethodInfo d, Ex arg) => Ex.Call(d, arg);

    /// <summary>Создаёт выражение вызова метода для метода с коллекцией аргументов</summary>
    /// <param name="d">Метод</param>
    /// <param name="arg">Коллекция аргументов</param>
    /// <returns>Выражение вызова метода</returns>
    public static MCEx GetCallExpression(this MethodInfo d, params IEnumerable<Ex> arg) => Ex.Call(d, arg);

    /// <summary>Создаёт выражение вызова метода для метода с экземпляром и коллекцией аргументов</summary>
    /// <param name="d">Метод</param>
    /// <param name="instance">Экземпляр</param>
    /// <param name="arg">Коллекция аргументов</param>
    /// <returns>Выражение вызова метода</returns>
    public static MCEx GetCallExpression(this MethodInfo d, Ex instance, params IEnumerable<Ex> arg) => Ex.Call(instance, d, arg);

    /// <summary>Создаёт выражение вызова делегата</summary>
    /// <param name="d">Делегат</param>
    /// <param name="arg">Аргументы</param>
    /// <returns>Выражение вызова делегата</returns>
    public static InvocationExpression GetInvokeExpression(this Delegate d, params IEnumerable<Ex> arg) => d.ToExpression().GetInvoke(arg);

    #endregion

    #region Action

    /// <summary>Преобразует <see cref="Action"/> в <see cref="EventHandler"/></summary>
    /// <param name="action">Действие</param>
    /// <returns>Обработчик события</returns>
    public static EventHandler AsEventHandler(this Action action) => (_, _) => action();

    /// <summary>Преобразует <see cref="Action"/> в <see cref="EventHandler{T}</summary>
    /// <param name="action">Действие</param>
    /// <typeparam name="T">Тип аргументов события</typeparam>
    /// <returns>Обработчик события</returns>
    public static EventHandler<T> AsEventHandler<T>(this Action action) where T : EventArgs => (_, _) => action();

    /// <summary>Преобразует <see cref="Action"/> в <see cref="EventHandler{T}</summary>
    /// <param name="action">Действие</param>
    /// <typeparam name="T">Тип аргументов события</typeparam>
    /// <returns>Обработчик события</returns>
    public static EventHandler<EventArgs<T>> AsEventHandlerArg<T>(this Action action) => (_, _) => action();

    /// <summary>Асинхронно выполняет действие</summary>
    /// <param name="action">Действие</param>
    /// <returns>Таск выполнения</returns>
    public static Task InvokeAsync(this Action action) => Task.Factory.FromAsync(action.BeginInvoke, action.EndInvoke, null);

    /// <summary>Асинхронно выполняет действие с аргументом</summary>
    /// <param name="action">Действие</param>
    /// <param name="parameter">Параметр</param>
    /// <typeparam name="T">Тип параметра</typeparam>
    /// <returns>Таск выполнения</returns>
    public static Task InvokeAsync<T>(this Action<T> action, T parameter) => Task.Factory.FromAsync(action.BeginInvoke, action.EndInvoke, parameter, null);

    /// <summary>Оборачивает действие в try-catch блок</summary>
    /// <param name="action">Действие</param>
    /// <param name="OnException">Обработчик исключения</param>
    /// <returns>Обёрнутое действие</returns>
    public static Action TryCatch(this Action action, Action? OnException = null) =>
        () =>
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                OnException?.Invoke();
            }
        };

    /// <summary>Оборачивает действие в try-catch блок</summary>
    /// <param name="action">Действие</param>
    /// <param name="OnException">Обработчик исключения с аргументом</param>
    /// <typeparam name="T">Тип параметра действия</typeparam>
    /// <returns>Обёрнутое действие</returns>
    public static Action<T> TryCatch<T>(this Action<T> action, Action<T>? OnException = null) =>
        t =>
        {
            try
            {
                action(t);
            }
            catch (Exception)
            {
                OnException?.Invoke(t);
            }
        };

    /// <summary>Оборачивает действие в try-catch блок с перехватом конкретного исключения</summary>
    /// <param name="action">Действие</param>
    /// <param name="OnException">Обработчик исключения</param>
    /// <typeparam name="TException">Тип перехватываемого исключения</typeparam>
    /// <returns>Обёрнутое действие</returns>
    public static Action TryCatch<TException>(this Action action, Action? OnException = null)
        where TException : Exception =>
        () =>
        {
            try
            {
                action();
            }
            catch (TException)
            {
                OnException?.Invoke();
            }
        };

    /// <summary>Оборачивает действие в try-catch блок с перехватом конкретного исключения</summary>
    /// <param name="action">Действие</param>
    /// <param name="OnException">Обработчик исключения с аргументом</param>
    /// <typeparam name="T">Тип параметра действия</typeparam>
    /// <typeparam name="TException">Тип перехватываемого исключения</typeparam>
    /// <returns>Обёрнутое действие</returns>
    public static Action<T> TryCatch<T, TException>(this Action<T> action, Action<T>? OnException = null)
        where TException : Exception =>
        t =>
        {
            try
            {
                action(t);
            }
            catch (TException)
            {
                OnException?.Invoke(t);
            }
        };

    #endregion

    #region Func

    /// <summary>Оборачивает func в try-catch блок</summary>
    /// <param name="func">Функция</param>
    /// <param name="OnException">Обработчик исключения</param>
    /// <typeparam name="T">Тип результата</typeparam>
    /// <returns>Обёрнутая функция</returns>
    public static Func<T?> TryCatch<T>(this Func<T> func, Func<T>? OnException = null) =>
        () =>
        {
            try
            {
                return func();
            }
            catch (Exception)
            {
                return OnException is null ? default : OnException();
            }
        };

    /// <summary>Оборачивает func в try-catch блок</summary>
    /// <param name="func">Функция</param>
    /// <param name="OnException">Обработчик исключения</param>
    /// <typeparam name="TIn">Тип аргумента</typeparam>
    /// <typeparam name="TOut">Тип результата</typeparam>
    /// <returns>Обёрнутая функция</returns>
    public static Func<TIn, TOut?> TryCatch<TIn, TOut>(this Func<TIn, TOut> func, Func<TIn, TOut>? OnException = null) =>
        t =>
        {
            try
            {
                return func(t);
            }
            catch (Exception)
            {
                return OnException is null ? default : OnException(t);
            }
        };

    /// <summary>Оборачивает func в try-catch блок с перехватом конкретного исключения</summary>
    /// <param name="func">Функция</param>
    /// <param name="OnException">Обработчик исключения</param>
    /// <typeparam name="T">Тип результата</typeparam>
    /// <typeparam name="TException">Тип перехватываемого исключения</typeparam>
    /// <returns>Обёрнутая функция</returns>
    public static Func<T?> TryCatch<T, TException>(this Func<T> func, Func<T>? OnException = null)
        where TException : Exception =>
        () =>
        {
            try
            {
                return func();
            }
            catch (TException)
            {
                return OnException is null ? default : OnException();
            }
        };

    /// <summary>Оборачивает func в try-catch блок с перехватом конкретного исключения</summary>
    /// <param name="func">Функция</param>
    /// <param name="OnException">Обработчик исключения</param>
    /// <typeparam name="TIn">Тип аргумента</typeparam>
    /// <typeparam name="TOut">Тип результата</typeparam>
    /// <typeparam name="TException">Тип перехватываемого исключения</typeparam>
    /// <returns>Обёрнутая функция</returns>
    public static Func<TIn, TOut?> TryCatch<TIn, TOut, TException>(this Func<TIn, TOut> func,
        Func<TIn, TOut>? OnException = null)
        where TException : Exception =>
        t =>
        {
            try
            {
                return func(t);
            }
            catch (TException)
            {
                return OnException is null ? default : OnException(t);
            }
        };

    #endregion

    /// <summary>Асинхронно вызывает делегат с аргументами</summary>
    /// <param name="action">Делегат</param>
    /// <param name="parameters">Аргументы</param>
    /// <returns>Таск результата</returns>
    public static Task<object?> DynamicInvokeAsync(this Delegate action, params object[] parameters) => action.Async(parameters, (d, p) => d.DynamicInvoke(p))!;
}