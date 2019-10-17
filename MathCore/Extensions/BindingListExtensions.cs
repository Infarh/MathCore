
// ReSharper disable once CheckNamespace
namespace System.ComponentModel
{
    public static class BindingListExtensions
    {
        public static BindingList<T> Initialize<T>
        (
            this BindingList<T> list,
            int ElementsCount,
            Func<int, T> Initializer
        )
        {
            for (var i = 0; i < ElementsCount; i++)
                list.Add(Initializer(i));
            return list;
        }

        public static BindingList<T> Initialize<T, TParameter>
        (
            this BindingList<T> list,
            int ElementsCount,
            TParameter parameter,
            Func<int, TParameter, T> Initializer
        )
        {
            for (var i = 0; i < ElementsCount; i++)
                list.Add(Initializer(i, parameter));
            return list;
        }
    }
}