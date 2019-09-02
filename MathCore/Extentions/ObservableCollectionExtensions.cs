using System.Collections.Generic;
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Collections.ObjectModel
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>([NotNull, ItemCanBeNull] this ObservableCollection<T> collection, [ItemCanBeNull, CanBeNull]  IEnumerable<T> items)
        {
            if (items is null) return;
            foreach (var item in items)
                collection.Add(item);
        }
    }
}