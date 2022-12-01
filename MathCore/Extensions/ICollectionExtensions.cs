#nullable enable
using System.Linq;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

/// <summary>Класс методов-расширений для интерфейса коллекции <see cref="ICollection{T}"/></summary>
// ReSharper disable once UnusedType.Global
// ReSharper disable once InconsistentNaming
public static class ICollectionExtensions
{
    /// <summary>Пакетное добавление элементов в коллекцию</summary>
    /// <typeparam name="T">Тип добавляемых элементов</typeparam>
    /// <param name="collection">Коллекция, в которую надо добавить перечисленных элементы</param>
    /// <param name="items">Перечисление добавляемых элементов</param>
    public static void AddItems<T>(this ICollection<T> collection, IEnumerable<T>? items)
    {
        switch (collection)
        {
            default:
                switch (items)
                {
                    case null: break;
                    case T[] array:
                        foreach (var item in array)
                            collection.Add(item);
                        break;
                    default:
                        items.Foreach(collection, (item, c) => c.Add(item));
                        break;
                }
                break;

            case T[]: throw new NotSupportedException("Массив не поддерживает операций добавления элементов");

            case List<T> list when items is not null:
                list.AddRange(items);
                break;
        }
    }

    /// <summary>Пакетное ленивое удаление элементов из коллекции. Элементы будут удалены лишь при перечислении результатов удаления</summary>
    /// <typeparam name="T">Тип удаляемых элементов</typeparam>
    /// <param name="collection">Коллекция, из которой требуется удалить перечисленные элементы</param>
    /// <param name="items">Перечисление удаляемых из коллекции элементов</param>
    /// <returns>Перечисление результатов удаления элементов</returns>
    public static IEnumerable<(T, bool)> RemoveItemsLazy<T>(this ICollection<T> collection, IEnumerable<T> items) =>
        items.Select(item => (item, collection.Remove(item)));

    /// <summary>Пакетное удаление элементов из коллекции</summary>
    /// <typeparam name="T">Тип удаляемых элементов</typeparam>
    /// <param name="collection">Коллекция, из которой требуется удалить перечисленные элементы</param>
    /// <param name="items">Перечисление удаляемых из коллекции элементов</param>
    /// <returns>Перечисление результатов удаления элементов</returns>
    public static (T, bool)[] RemoveItemsWithResults<T>(this ICollection<T> collection, IEnumerable<T> items) =>
        collection.RemoveItemsLazy(items).ToArray();

    /// <summary>Пакетное удаление элементов из коллекции</summary>
    /// <typeparam name="T">Тип удаляемых элементов</typeparam>
    /// <param name="collection">Коллекция, из которой требуется удалить перечисленные элементы</param>
    /// <param name="items">Перечисление удаляемых из коллекции элементов</param>
    /// <returns>Перечисление результатов удаления элементов</returns>
    public static void RemoveItems<T>(this ICollection<T> collection, IEnumerable<T>? items)
    {
        switch (items)
        {
            case null: break;

            default:
                foreach (var item in items)
                    collection.Remove(item);
                break;

            case T[] list:
                foreach (var item in list)
                    collection.Remove(item);
                break;

            case List<T> list:
                foreach (var item in list)
                    collection.Remove(item);
                break;
        }
    }
}