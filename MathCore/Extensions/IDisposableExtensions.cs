#nullable enable
using System.Collections.Generic;

using MathCore;

using NN = MathCore.Annotations.NotNullAttribute;

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Методы-расширения для интерфейса <see cref="IDisposable"/></summary>
// ReSharper disable once InconsistentNaming
public static class IDisposableExtensions
{
    /// <summary>Представить в виде группы элементов, поддерживающих освобождение ресурсов</summary>
    /// <typeparam name="T">Тип элемента, поддерживающий <see cref="IDisposable"/></typeparam>
    /// <param name="items">Элементы, поддерживающие <see cref="IDisposable"/></param>
    /// <returns><see cref="DisposableGroup{T}"/></returns>
    public static DisposableGroup<T> AsDisposableGroup<T>(this IEnumerable<T> items) where T : IDisposable => new(items);

    /// <summary>Выполнить действие, после чего освободить ресурсы</summary>
    /// <typeparam name="T">Тип объекта, над которым требуется выполнить действие</typeparam>
    /// <param name="obj">Объект, действие над которым требуется выполнить</param>
    /// <param name="action">Выполняемое над объектом действие</param>
    public static void DisposeAfter<T>(this T obj, Action<T> action)
        where T : IDisposable
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        using (obj) action(obj);
    }

    /// <summary>Выполнить действие, после чего освободить ресурсы</summary>
    /// <typeparam name="T">Тип объекта, над которым требуется выполнить действие</typeparam>
    /// <typeparam name="TP">Тип параметра действия</typeparam>
    /// <param name="obj">Объект, действие над которым требуется выполнить</param>
    /// <param name="p">Параметр, передаваемый в действие, чтобы избежать замыкания</param>
    /// <param name="action">Выполняемое над объектом действие</param>
    public static void DisposeAfter<T, TP>(this T obj, in TP p, Action<T, TP> action)
        where T : IDisposable
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        using (obj) action(obj, p);
    }

    /// <summary>Выполнить действие, после чего освободить ресурсы</summary>
    /// <typeparam name="T">Тип объекта, над которым требуется выполнить действие</typeparam>
    /// <typeparam name="TP1">Тип параметра 1 действия</typeparam>
    /// <typeparam name="TP2">Тип параметра 2 действия</typeparam>
    /// <param name="obj">Объект, действие над которым требуется выполнить</param>
    /// <param name="p1">Параметр 1, передаваемый в действие, чтобы избежать замыкания</param>
    /// <param name="p2">Параметр 2, передаваемый в действие, чтобы избежать замыкания</param>
    /// <param name="action">Выполняемое над объектом действие</param>
    public static void DisposeAfter<T, TP1, TP2>(this T obj, in TP1 p1, in TP2 p2, Action<T, TP1, TP2> action)
        where T : IDisposable
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        using (obj) action(obj, p1, p2);
    }

    /// <summary>Выполнить действие, после чего освободить ресурсы</summary>
    /// <typeparam name="T">Тип объекта, над которым требуется выполнить действие</typeparam>
    /// <typeparam name="TP1">Тип параметра 1 действия</typeparam>
    /// <typeparam name="TP2">Тип параметра 2 действия</typeparam>
    /// <typeparam name="TP3">Тип параметра 3 действия</typeparam>
    /// <param name="obj">Объект, действие над которым требуется выполнить</param>
    /// <param name="p1">Параметр 1, передаваемый в действие, чтобы избежать замыкания</param>
    /// <param name="p2">Параметр 2, передаваемый в действие, чтобы избежать замыкания</param>
    /// <param name="p3">Параметр 3, передаваемый в действие, чтобы избежать замыкания</param>
    /// <param name="action">Выполняемое над объектом действие</param>
    public static void DisposeAfter<T, TP1, TP2, TP3>(this T obj, in TP1 p1, in TP2 p2, in TP3 p3, Action<T, TP1, TP2, TP3> action)
        where T : IDisposable
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        using (obj) action(obj, p1, p2, p3);
    }

    /// <summary>Выполнить действие и получить результат, после чего освободить ресурсы</summary>
    /// <typeparam name="T">Тип объекта, над которым требуется выполнить действие</typeparam>
    /// <typeparam name="TResult">Тип вычисляемого результата</typeparam>
    /// <param name="obj">Объект, действие над которым требуется выполнить</param>
    /// <param name="func">Функция, вычисляющая результат на основе переданного ей значения</param>
    public static TResult DisposeAfter<T, TResult>(this T obj, Func<T, TResult> func)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return func(obj);
    }

    /// <summary>Выполнить действие и получить результат, после чего освободить ресурсы</summary>
    /// <typeparam name="T">Тип объекта, над которым требуется выполнить действие</typeparam>
    /// <typeparam name="TP">Тип параметра действия</typeparam>
    /// <typeparam name="TResult">Тип вычисляемого результата</typeparam>
    /// <param name="obj">Объект, действие над которым требуется выполнить</param>
    /// <param name="p">Параметр, передаваемый в функцию, чтобы избежать замыкания</param>
    /// <param name="func">Функция, вычисляющая результат на основе переданного ей значения</param>
    public static TResult DisposeAfter<T, TP, TResult>(this T obj, in TP p, Func<T, TP, TResult> func)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return func(obj, p);
    }

    /// <summary>Выполнить действие и получить результат, после чего освободить ресурсы</summary>
    /// <typeparam name="T">Тип объекта, над которым требуется выполнить действие</typeparam>
    /// <typeparam name="TP1">Тип параметра 1 действия</typeparam>
    /// <typeparam name="TP2">Тип параметра 2 действия</typeparam>
    /// <typeparam name="TResult">Тип вычисляемого результата</typeparam>
    /// <param name="obj">Объект, действие над которым требуется выполнить</param>
    /// <param name="p1">Параметр 1, передаваемый в функцию, чтобы избежать замыкания</param>
    /// <param name="p2">Параметр 2, передаваемый в функцию, чтобы избежать замыкания</param>
    /// <param name="func">Функция, вычисляющая результат на основе переданного ей значения</param>
    public static TResult DisposeAfter<T, TP1, TP2, TResult>(this T obj, in TP1 p1, in TP2 p2, Func<T, TP1, TP2, TResult> func)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return func(obj, p1, p2);
    }

    /// <summary>Выполнить действие и получить результат, после чего освободить ресурсы</summary>
    /// <typeparam name="T">Тип объекта, над которым требуется выполнить действие</typeparam>
    /// <typeparam name="TP1">Тип параметра 1 действия</typeparam>
    /// <typeparam name="TP2">Тип параметра 2 действия</typeparam>
    /// <typeparam name="TP3">Тип параметра 3 действия</typeparam>
    /// <typeparam name="TResult">Тип вычисляемого результата</typeparam>
    /// <param name="obj">Объект, действие над которым требуется выполнить</param>
    /// <param name="p1">Параметр 1, передаваемый в функцию, чтобы избежать замыкания</param>
    /// <param name="p2">Параметр 2, передаваемый в функцию, чтобы избежать замыкания</param>
    /// <param name="p3">Параметр 3, передаваемый в функцию, чтобы избежать замыкания</param>
    /// <param name="func">Функция, вычисляющая результат на основе переданного ей значения</param>
    public static TResult DisposeAfter<T, TP1, TP2, TP3, TResult>(this T obj, in TP1 p1, in TP2 p2, in TP3 p3, Func<T, TP1, TP2, TP3, TResult> func)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return func(obj, p1, p2, p3);
    }
}