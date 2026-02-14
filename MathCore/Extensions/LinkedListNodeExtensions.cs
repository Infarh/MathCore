
// ReSharper disable UnusedMethodReturnValue.Global

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

/// <summary>Предоставляет методы-расширения для LinkedListNode</summary>
public static class LinkedListNodeExtensions
{
    /// <summary>Добавляет новый элемент перед указанным узлом</summary>
    /// <param name="node">Узел, перед которым будет добавлен новый элемент</param>
    /// <param name="value">Значение нового элемента</param>
    /// <returns>Новый узел, добавленный перед указанным</returns>
    public static LinkedListNode<T> AddBefore<T>(this LinkedListNode<T> node, T value) => node.List!.AddBefore(node, value);

    /// <summary>Добавляет новый элемент после указанного узла</summary>
    /// <param name="node">Узел, после которого будет добавлен новый элемент</param>
    /// <param name="value">Значение нового элемента</param>
    /// <returns>Новый узел, добавленный после указанного</returns>
    public static LinkedListNode<T> AddAfter<T>(this LinkedListNode<T> node, T value) => node.List!.AddAfter(node, value);

    /// <summary>Возвращает индекс узла в связном списке</summary>
    /// <param name="node">Узел, индекс которого требуется определить</param>
    /// <returns>Индекс узла</returns>
    public static int GetIndex<T>(this LinkedListNode<T> node) => node.AsEnumerable(n => n!.Previous).TakeWhile(n => n is not null).Count() - 1;

    /// <summary>Возвращает последовательность следующих узлов</summary>
    /// <param name="node">Исходный узел</param>
    /// <returns>Последовательность следующих узлов</returns>
    public static IEnumerable<LinkedListNode<T>> GetNextNodes<T>(this LinkedListNode<T> node) => node.AsEnumerable(n => n!.Next).TakeWhile(n => n is not null).Skip(1)!;

    /// <summary>Возвращает последовательность предыдущих узлов</summary>
    /// <param name="node">Исходный узел</param>
    /// <returns>Последовательность предыдущих узлов</returns>
    public static IEnumerable<LinkedListNode<T>> GetPreviousNodes<T>(this LinkedListNode<T> node) => node.AsEnumerable(n => n!.Previous).TakeWhile(n => n is not null).Skip(1)!;
}