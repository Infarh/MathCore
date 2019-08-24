using System.Diagnostics;
using MathCore.Annotations;

namespace System.Collections.Generic
{
    /// <summary>Методы расширения для интерфейса списка</summary>
    public static class IListExtentions
    {
        public static bool IsNullOrEmpty<T>(this IList<T> list) => list == null || list.Count == 0;

        ///<summary>Метод расширения для инициализации списка</summary>
        ///<param name="list">Инициализируемый объект</param>
        ///<param name="Count">Требуемое число элементов</param>
        ///<param name="Initializator">Метод инициализации</param>
        ///<param name="ClearBefore">Очищать предварительно (по умолчанию)</param>
        ///<typeparam name="T">Тип элементов списка</typeparam>
        ///<returns>Инициализированный список</returns>
        [DebuggerStepThrough, CanBeNull]
        public static IList<T> Initialize<T>
        (
            [CanBeNull] this IList<T> list, 
            int Count, 
            [NotNull] Func<int, T> Initializator, 
            bool ClearBefore = true
        )
        {
            if (list == null) return null;
            if(list is List<T> l)
            {
                if(ClearBefore) l.Clear();
                for(var i = 0; i < Count; i++) l.Add(Initializator(i));
            }
            else
            {
                if(ClearBefore) list.Clear();
                for(var i = 0; i < Count; i++) list.Add(Initializator(i));
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
        [DebuggerStepThrough, CanBeNull]
        public static IList<T> Initialize<T, TParameter>
        (
            [CanBeNull] this IList<T> list,
            int Count,
            [CanBeNull] TParameter parameter,
            [NotNull] Func<int, TParameter, T> Initializator,
            bool ClearBefore = true
        )
        {
            if(list == null) return null;
            if(list is List<T> l)
            {
                if(ClearBefore) l.Clear();
                for(var i = 0; i < Count; i++)
                    l.Add(Initializator(i, parameter));
            }
            else
            {
                if(ClearBefore) list.Clear();
                for(var i = 0; i < Count; i++)
                    list.Add(Initializator(i, parameter));
            }
            return list;
        }

        /// <summary>Перемешать список</summary>
        /// <param name="list">Перемешиваемый список</param>
        /// <typeparam name="T">Тип элементов списка</typeparam>
        /// <returns>Перемешанный исходный список</returns>
        public static IList<T> Mix<T>(this IList<T> list)
        {
            var rnd = new Random();

            if(list is List<T> l)
            {
                var length = l.Count - 1;
                var temp = l[0];
                var index = 0;
                for(var i = 1; i <= length; i++)
                    l[index] = l[index = rnd.Next(length)];
                l[index] = temp;
            }
            else
            {
                var length = list.Count - 1;
                var temp = list[0];
                var index = 0;
                for(var i = 1; i <= length; i++)
                    list[index] = list[index = rnd.Next(length)];
                list[index] = temp;
            }
            return list;
        }
    }
}
