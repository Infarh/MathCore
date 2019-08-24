using System.Diagnostics.Contracts;

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
            Contract.Requires(Initializer != null);
            Contract.Requires(list != null);
            Contract.Requires(ElementsCount >= 0);
            Contract.Ensures(Contract.Result<BindingList<T>>() == list);

            for(var i = 0; i < ElementsCount; i++)
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
            Contract.Requires(Initializer != null);
            Contract.Requires(list != null);
            Contract.Requires(ElementsCount >= 0);
            Contract.Ensures(Contract.Result<BindingList<T>>() == list);

            for(var i = 0; i < ElementsCount; i++)
                list.Add(Initializer(i, parameter));
            return list;
        }
    }
}