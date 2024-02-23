#nullable enable
using MathCore;

// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

/// <summary>Методы расширения для интерфейса <see cref="IList{T}"/></summary>
public static class IListExtensions
{
    /// <summary>Создать генератор случных элементов</summary>
    /// <typeparam name="T">Тип элементов списка</typeparam>
    /// <param name="Items">Список элементов, на основе которого надо создать генератор</param>
    /// <param name="Random">Датчик случайных чисел</param>
    /// <returns>Генератор случайного значения из элементов списка</returns>
    public static Randomizer<T> GetRandomizer<T>(this IList<T> Items, Random? Random = null) => new(Items, Random);

    /// <summary>Ссылка на список пуста, либо список не содержит элементов</summary>
    /// <param name="list">Проверяемый список</param>
    /// <returns>Истина, если не задана ссылка на список, либо список пуст</returns>
    public static bool IsNullOrEmpty(this IList? list) => list is not { Count: > 0 };

    ///<summary>Метод расширения для инициализации списка</summary>
    ///<param name="list">Инициализируемый объект</param>
    ///<param name="Count">Требуемое число элементов</param>
    ///<param name="Initializator">Метод инициализации</param>
    ///<param name="ClearBefore">Очищать предварительно (по умолчанию)</param>
    ///<typeparam name="T">Тип элементов списка</typeparam>
    ///<returns>Инициализированный список</returns>
    [DST]
    public static IList<T>? Initialize<T>
    (
        this IList<T>? list,
        int Count,
        Func<int, T> Initializator,
        bool ClearBefore = true
    )
    {
        switch (list)
        {
            case null: return null;
            case List<T> l:
                {
                    if (ClearBefore) l.Clear();
                    for (var i = 0; i < Count; i++) l.Add(Initializator(i));
                    break;
                }
            default:
                {
                    if (ClearBefore) list.Clear();
                    for (var i = 0; i < Count; i++) list.Add(Initializator(i));
                    break;
                }
        }

        return list;
    }

    ///<summary>Метод расширения для инициализации списка</summary>
    ///<param name="list">Инициализируемый объект</param>
    ///<param name="Count">Требуемое число элементов</param>
    ///<param name="parameter">Параметр инициализации</param>
    ///<param name="Initializator">Метод инициализации</param>
    ///<param name="ClearBefore">Очищать предварительно (по умолчанию)</param>
    ///<typeparam name="T">Тип элементов списка</typeparam>
    ///<typeparam name="TParameter">Тип параметра инициализации</typeparam>
    ///<returns>Инициализированный список</returns>
    [DST]
    public static IList<T>? Initialize<T, TParameter>
    (
        this IList<T>? list,
        int Count,
        in TParameter? parameter,
        Func<int, TParameter?, T> Initializator,
        bool ClearBefore = true
    )
    {
        switch (list)
        {
            case null: return null;
            case List<T> l:
                {
                    if (ClearBefore) l.Clear();
                    for (var i = 0; i < Count; i++)
                        l.Add(Initializator(i, parameter));
                    break;
                }
            default:
                {
                    if (ClearBefore) list.Clear();
                    for (var i = 0; i < Count; i++)
                        list.Add(Initializator(i, parameter));
                    break;
                }
        }

        return list;
    }

    /// <summary>Перемешать список</summary>
    /// <param name="list">Перемешиваемый список</param>
    /// <param name="rnd">Генератор случайных чисел</param>
    /// <typeparam name="T">Тип элементов списка</typeparam>
    /// <returns>Перемешанный исходный список</returns>
    public static IList<T> Mix<T>(this IList<T> list, Random? rnd = null)
    {
        rnd ??= new();

        switch (list)
        {
            case T[] l:
                {
                    var length = l.Length - 1;
                    var temp   = l[0];
                    var index  = 0;
                    for (var i = 1; i <= length; i++)
                        l[index] = l[index = rnd.Next(length)];
                    l[index] = temp;
                    break;
                }
            case List<T> l:
                {
                    var length = l.Count - 1;
                    var temp   = l[0];
                    var index  = 0;
                    for (var i = 1; i <= length; i++)
                        l[index] = l[index = rnd.Next(length)];
                    l[index] = temp;
                    break;
                }
            default:
                {
                    var length = list.Count - 1;
                    var temp   = list[0];
                    var index  = 0;
                    for (var i = 1; i <= length; i++)
                        list[index] = list[index = rnd.Next(length)];
                    list[index] = temp;
                    break;
                }
        }
        return list;
    }
}