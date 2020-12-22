using System.Linq;

using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    /// <summary>Класс методов-расширений для интерфейса коллекции <see cref="ICollection{T}"/></summary>
    public static class ICollectionExtensions
    {
        /// <summary>Пакетное добавление элементов в коллекцию</summary>
        /// <typeparam name="T">Тип добавляемых элементов</typeparam>
        /// <param name="collection">Коллекция, в которую надо добавить перечисленных элементы</param>
        /// <param name="items">Перечисление добавляемых элементов</param>
        public static void AddItems<T>([NotNull] this ICollection<T> collection, [NotNull] IEnumerable<T> items)
        {
            switch (collection)
            {
                default:
                    items.Foreach(collection.Add);
                    break;
                case List<T> list:
                    list.AddRange(items);
                    break;
            }
        }

        /// <summary>Пакетное ленивое удаление элементов из коллекции. Элементы будут удалены лишь при перечислении результатов удаления</summary>
        /// <typeparam name="T">Тип удаляемых элементов</typeparam>
        /// <param name="collection">Коллекция, из которой требуется удалить перечисленные элементы</param>
        /// <param name="items">Перечисление удаляемых из коллекции элементов</param>
        /// <returns>Перечисление результатов удаления элементов</returns>
        [NotNull]
        public static IEnumerable<(T, bool)> RemoveItemsLazy<T>([NotNull] this ICollection<T> collection, [NotNull] IEnumerable<T> items) =>
            items.Select(item => (item, collection.Remove(item)));

        /// <summary>Пакетное удаление элементов из коллекции</summary>
        /// <typeparam name="T">Тип удаляемых элементов</typeparam>
        /// <param name="collection">Коллекция, из которой требуется удалить перечисленные элементы</param>
        /// <param name="items">Перечисление удаляемых из коллекции элементов</param>
        /// <returns>Перечисление результатов удаления элементов</returns>
        [NotNull]
        public static (T, bool)[] RemoveItemsWithResults<T>([NotNull] this ICollection<T> collection, [NotNull] IEnumerable<T> items) =>
            collection.RemoveItemsLazy(items).ToArray();

        /// <summary>Пакетное удаление элементов из коллекции</summary>
        /// <typeparam name="T">Тип удаляемых элементов</typeparam>
        /// <param name="collection">Коллекция, из которой требуется удалить перечисленные элементы</param>
        /// <param name="items">Перечисление удаляемых из коллекции элементов</param>
        /// <returns>Перечисление результатов удаления элементов</returns>
        public static void RemoveItems<T>([NotNull] this ICollection<T> collection, [NotNull] IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Remove(item);
        }
    }
}