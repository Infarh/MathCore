// ReSharper disable once CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
namespace System.ComponentModel
{
    /// <summary>Класс методов-расширений для <see cref="BindingList{T}"/></summary>
    public static class BindingListExtensions
    {
        /// <summary>Инициализация нового экземпляра <see cref="BindingList{T}"/> с помощью функции</summary>
        /// <typeparam name="T">Тип элементов списка</typeparam>
        /// <param name="list">Инициализируемый <see cref="BindingList{T}"/></param>
        /// <param name="ElementsCount">Число добавляемых элементов</param>
        /// <param name="Initializer">Функция генерации новых элементов списка</param>
        /// <returns>Инициализируемый <see cref="BindingList{T}"/></returns>
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

        /// <summary>Инициализация нового экземпляра <see cref="BindingList{T}"/> с помощью функции</summary>
        /// <typeparam name="T">Тип элементов списка</typeparam>
        /// <typeparam name="TParameter">Тип параметра инициализации</typeparam>
        /// <param name="list">Инициализируемый <see cref="BindingList{T}"/></param>
        /// <param name="ElementsCount">Число добавляемых элементов</param>
        /// <param name="parameter">Параметр процесса инициализации, передаваемый в функцию</param>
        /// <param name="Initializer">Функция генерации новых элементов списка</param>
        /// <returns>Инициализируемый <see cref="BindingList{T}"/></returns>
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