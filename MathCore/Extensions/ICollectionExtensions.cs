using System.Linq;
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    public static class ICollectionExtensions
    {
        public static void AddItems<T>([NotNull] this ICollection<T> collection, [NotNull] IEnumerable<T> items) => items.Foreach(collection.Add);

        [NotNull]
        public static IEnumerable<bool> RemoveItems<T>([NotNull] this ICollection<T> collection, [NotNull] IEnumerable<T> items) => items.Select(collection.Remove);
    }
}