using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using MathCore;
using MathCore.Annotations;
using MathCore.Values;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using NN = MathCore.Annotations.NotNullAttribute;
using CN = MathCore.Annotations.CanBeNullAttribute;
using InN = MathCore.Annotations.ItemNotNullAttribute;
using IcN = MathCore.Annotations.ItemCanBeNullAttribute;
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToUsingDeclaration
// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Local

// ReSharper disable once CheckNamespace
namespace System.Linq
{
    /// <summary>Класс методов-расширений для интерфейса перечисления</summary>
    [PublicAPI]
    public static class IEnumerableExtensions
    {
        /// <summary>Добавить элемент в начало последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="enumerable">Исходная последовательность элементов</param>
        /// <param name="values">Добавляемый элемент</param>
        /// <returns>Результирующая последовательность элементов, в которой добавленный элемент идёт на первом месте</returns>
        [NN]
        public static IEnumerable<T> InsertBefore<T>([CN] this IEnumerable<T> enumerable, [CN] params T[] values)
        {
            if (values != null) foreach (var v in values) yield return v;
            if (enumerable != null) foreach (var v in enumerable) yield return v;
        }

        /// <summary>Добавить элемент в начало последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="enumerable">Исходная последовательность элементов</param>
        /// <param name="values">Добавляемый элемент</param>
        /// <returns>Результирующая последовательность элементов, в которой добавленный элемент идёт на первом месте</returns>
        [NN]
        public static IEnumerable<T> InsertBefore<T>([CN] this IEnumerable<T> enumerable, [CN] IEnumerable<T> values)
        {
            if (values != null) foreach (var v in values) yield return v;
            if (enumerable != null) foreach (var v in enumerable) yield return v;
        }

        public static T FirstOrDefault<T, TP>([NN] this IEnumerable<T> enumerable, TP p, Func<T, TP, bool> Selector)
        {
            foreach (var item in enumerable)
                if (Selector(item, p))
                    return item;
            return default;
        }

        public static T FirstOrDefault<T, TP1, TP2>(
            [NN] this IEnumerable<T> enumerable,
            TP1 p1, 
            TP2 p2, 
            Func<T, TP1, TP2, bool> Selector)
        {
            foreach (var item in enumerable)
                if (Selector(item, p1, p2))
                    return item;
            return default;
        }

        [NN]
        public static IEnumerable<T> SelectMany<T>([CN] this IEnumerable<IEnumerable<T>> enumerable)
        {
            if (enumerable is null) yield break;
            foreach (var items in enumerable)
                if (items != null)
                    foreach (var item in items)
                        yield return item;
        }

        private class BlockEnumerator<T> : IEnumerable<T>
        {
            [NN] private readonly IEnumerator<T> _Source;
            private readonly int _Size;

            public bool Complete { get; private set; }

            public BlockEnumerator([NN] IEnumerator<T> source, int Size)
            {
                _Source = source;
                _Size = Size;
            }

            public IEnumerator<T> GetEnumerator()
            {
                for (var i = 0; i < _Size; i++)
                {
                    if (!_Source.MoveNext()) yield break;
                    yield return _Source.Current;
                }
                Complete = true;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [NN, InN]
        public static IEnumerable<IEnumerable<T>> AsBlockEnumerable<T>([NN, IcN] this IEnumerable<T> enumerable, int BlockSize)
        {
            if (BlockSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(BlockSize), "Размер блока должен быть положительным значением");

            IEnumerator<T> enumerator = null;
            try
            {
                enumerator = enumerable.GetEnumerator();
                BlockEnumerator<T> block;
                do
                {
                    block = new BlockEnumerator<T>(enumerator, BlockSize);
                    yield return block;
                } while (block.Complete);
            }
            finally
            {
                enumerator?.Dispose();
            }
        }

        [NN]
        public static IEnumerable<T> Except<T>([NN] this IEnumerable<T> enumerable, T item)
        {
            foreach (var i in enumerable)
                if (!Equals(i, item))
                    yield return i;
        }

        public static int FirstIndexOf<T>([NN, IcN] this IEnumerable<T> enumerable, [CN] T item)
        {
            var index = -1;
            foreach (var element in enumerable)
            {
                index++;
                if (Equals(element, item)) return index;
            }
            return index;
        }

        public static int LastIndexOf<T>([NN, IcN] this IEnumerable<T> enumerable, [CN] T item)
        {
            var i = 0;
            var index = -1;
            foreach (var element in enumerable)
            {
                if (Equals(element, item)) index = i;
                i++;
            }
            return index;
        }

        public static IEnumerable Concat([CN] this IEnumerable source, [CN] IEnumerable other)
        {
            if (source != null) foreach (var src in source) yield return src;
            if (other != null) foreach (var oth in other) yield return oth;
        }

        [CN]
        public static LambdaEnumerable<TValue> GetLambdaEnumerable<TObject, TValue>(
            this TObject obj,
            Func<TObject, IEnumerable<TValue>> Creator)
            => new LambdaEnumerable<TValue>(() => Creator(obj));

        [CN]
        public static IEnumerable<T> WhereNotNull<T>([CN] this IEnumerable<T> enumerable) 
            where T : class 
            => enumerable?.Where(i => i is { });

        /// <summary>Фильтрация последовательности строк по указанному регулярному выражению</summary>
        /// <param name="strings">Последовательность строк</param>
        /// <param name="regex">Регулярное выражение-фильтр</param>
        /// <returns>Последовательность строк, удовлетворяющая регулярному выражению</returns>
        [CN]
        public static IEnumerable<string> Where(
            [CN] this IEnumerable<string> strings,
            [NN] Regex regex) 
            => strings?.Where((Func<string, bool>)regex.IsMatch);

        /// <summary>Фильтрация последовательности строк, которые не удовлетворяют регулярному выражению</summary>
        /// <param name="strings">Фильтруемая последовательность строк</param>
        /// <param name="regex">Регулярное выражение-фильтр</param>
        /// <returns>Последовательность строк, которые не удовлетворяют регулярному выражению</returns>
        [CN]
        public static IEnumerable<string> WhereNot(
            [CN] this IEnumerable<string> strings,
            [NN] Regex regex) 
            => strings?.WhereNot(regex.IsMatch);

        /// <summary>Фильтрация последовательности строк по указанному регулярному выражению</summary>
        /// <param name="strings">Последовательность строк</param>
        /// <param name="regex">Регулярное выражение-фильтр</param>
        /// <returns>Последовательность строк, удовлетворяющая регулярному выражению</returns>
        [CN]
        public static IEnumerable<string> Where(
            [CN] this IEnumerable<string> strings,
            [NN] string regex) 
            => strings?.Where(s => Regex.IsMatch(s, regex));

        /// <summary>Фильтрация последовательности строк, которые не удовлетворяют регулярному выражению</summary>
        /// <param name="strings">Фильтруемая последовательность строк</param>
        /// <param name="regex">Регулярное выражение-фильтр</param>
        /// <returns>Последовательность строк, которые не удовлетворяют регулярному выражению</returns>
        [CN]
        public static IEnumerable<string> WhereNot(
            [CN] this IEnumerable<string> strings, 
            [NN] string regex) 
            => strings?.WhereNot(s => Regex.IsMatch(s, regex));

        /// <summary>Выполняет фильтрацию последовательности значений на основе заданного предиката</summary>
        /// <returns>
        /// Объект <see cref="T:System.Collections.Generic.IEnumerable`1"/>, содержащий элементы входной последовательности, которые не удовлетворяют условию.
        /// </returns>
        /// <param name="enumerable">Объект <see cref="T:System.Collections.Generic.IEnumerable`1"/>, подлежащий фильтрации.</param>
        /// <param name="NotSelector">Функция для проверки каждого элемента на не соответствие условию.</param>
        /// <typeparam name="T">Тип элементов последовательности <paramref name="enumerable"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">Значение параметра <paramref name="enumerable"/> или <paramref name="NotSelector"/> — null.</exception>
        [CN]
        public static IEnumerable<T> WhereNot<T>(
            [CN] this IEnumerable<T> enumerable,
            [NN]Func<T, bool> NotSelector) 
            => enumerable?.Where(t => !NotSelector(t));

        /// <summary>Возвращает цепочку элементов последовательности, удовлетворяющих указанному условию</summary>
        /// <returns>
        /// Объект <see cref="T:System.Collections.Generic.IEnumerable`1"/>, содержащий элементы входной последовательности до первого элемента, который не прошел проверку.
        /// </returns>
        /// <param name="enumerable">Последовательность, из которой требуется возвратить элементы.</param>
        /// <param name="NotSelector">Функция для проверки каждого элемента на соответствие условию.</param>
        /// <typeparam name="T">Тип элементов последовательности <paramref name="enumerable"/>.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">Значение параметра <paramref name="enumerable"/> или <paramref name="NotSelector"/> — null.</exception>
        [CN]
        public static IEnumerable<T> TakeWhileNot<T>(
            [CN] this IEnumerable<T> enumerable,
            [NN]Func<T, bool> NotSelector)
            => enumerable?.TakeWhile(t => !NotSelector(t));

        /// <summary>Преобразование перечисления в массив с преобразованием элементов</summary>
        /// <typeparam name="T">Тип элементов исходного перечисления</typeparam>
        /// <typeparam name="TV">Тип элементов результирующего массива</typeparam>
        /// <param name="enumerable">Исходное перечисление</param>
        /// <param name="converter">Метод преобразования элементов</param>
        /// <returns>
        /// Если ссылка на исходное перечисление не пуста, то
        ///     Результирующий массив, состоящий из элементов исходного перечисления, преобразованных указанным методом
        /// иначе
        ///     пустая ссылка на массив
        /// </returns>
        [NN] public static TV[] ToArray<T, TV>(
            [NN] this IEnumerable<T> enumerable,
            [NN] Func<T, TV> converter)
            => enumerable.Select(converter).ToArray();

        /// <summary>Преобразование перечисления в массив с преобразованием элементов</summary>
        /// <typeparam name="T">Тип элементов исходного перечисления</typeparam>
        /// <typeparam name="TV">Тип элементов результирующего массива</typeparam>
        /// <param name="enumerable">Исходное перечисление</param>
        /// <param name="converter">Метод преобразования элементов и индекса элемента</param>
        /// <returns>
        /// Если ссылка на исходное перечисление не пуста, то
        ///     Результирующий массив, состоящий из элементов исходного перечисления, преобразованных указанным методом
        /// иначе
        ///     пустая ссылка на массив
        /// </returns>
        [NN] public static TV[] ToArray<T, TV>(
            [NN] this IEnumerable<T> enumerable, 
            [NN] Func<T, int, TV> converter) 
            => enumerable.Select(converter).ToArray();

        /// <summary>Преобразование перечисления в список с преобразованием элементов</summary>
        /// <typeparam name="TItem">Тип элементов исходного перечисления</typeparam>
        /// <typeparam name="TValue">Тип элементов результирующего списка</typeparam>
        /// <param name="enumerable">Исходное перечисление</param>
        /// <param name="converter">Метод преобразования элементов</param>
        /// <returns>
        /// Если ссылка на исходное перечисление не пуста, то
        ///     Результирующий список, состоящий из элементов исходного перечисления, преобразованных указанным методом
        /// иначе
        ///     пустая ссылка на список
        /// </returns>
        [CN]
        public static List<TValue> ToList<TItem, TValue>(
            [CN] this IEnumerable<TItem> enumerable,
            [NN] Func<TItem, TValue> converter)
            => enumerable?.Select(converter).ToList();

        [NN]
        public static Dictionary<TKey, T> ToDictionaryDistinctKeys<T, TKey>(
            [NN] this IEnumerable<T> enumerable,
            Func<T, TKey> KeySelector, 
            bool OverloadValues = false)
        {
            var dic = new Dictionary<TKey, T>();
            if (OverloadValues)
                foreach (var item in enumerable)
                {
                    var key = KeySelector(item);
                    if (dic.ContainsKey(key)) continue;
                    dic.Add(key, item);
                }
            else
                foreach (var item in enumerable)
                {
                    var key = KeySelector(item);
                    dic[key] = item;
                }
            return dic;
        }

        [NN]
        public static Dictionary<TKey, TValue> ToDictionaryDistinctKeys<T, TKey, TValue>(
            [NN] this IEnumerable<T> enumerable, 
            Func<T, TKey> KeySelector, 
            Func<T, TValue> ValueSelector,
            bool OverloadValues = false)
        {
            var dic = new Dictionary<TKey, TValue>();
            if (OverloadValues)
                foreach (var item in enumerable)
                {
                    var key = KeySelector(item);
                    if (dic.ContainsKey(key)) continue;
                    dic.Add(key, ValueSelector(item));
                }
            else
                foreach (var item in enumerable)
                {
                    var key = KeySelector(item);
                    dic[key] = ValueSelector(item);
                }
            return dic;
        }

        /// <summary>Объединение перечисления строк в единую строку с разделителем - переносом строки</summary>
        /// <param name="Lines">Перечисление строк</param>
        /// <returns>Если ссылка на перечисление пуста, то пустая ссылка на строку, иначе - объединение строк с разделителем - переносом строки</returns>
        [CN]
        public static string Aggregate([CN] this IEnumerable<string> Lines) 
            => Lines?.Aggregate(new StringBuilder(), (sb, s) => sb.AppendLine(s), sb => sb.ToString());

        /// <summary>Добавить элементы перечисления в коллекцию</summary>
        /// <typeparam name="T">Тип элемента</typeparam>
        /// <param name="source">Перечисление добавляемых элементов</param>
        /// <param name="collection">Коллекция-приёмник элементов</param>
        public static void AddTo<T>([NN] this IEnumerable<T> source, [NN] ICollection<T> collection)
        {
            foreach (var item in source) collection.Add(item);
        }

        /// <summary>Удалить перечисление элементов из коллекции</summary>
        /// <typeparam name="T">Тип элементов</typeparam>
        /// <param name="source">Перечисление удаляемых элементов</param>
        /// <param name="collection">Коллекция, из которой производится удаление</param>
        /// <returns>Перечисление результатов операций удаления для каждого из элементов исходного перечисления</returns>
        [NN]
        public static bool[] RemoveFrom<T>([NN] this IEnumerable<T> source, [NN] ICollection<T> collection)
            => source.Select(collection.Remove).ToArray();


        /// <summary>Добавить в словарь</summary>
        /// <typeparam name="TKey">Тип ключей словаря</typeparam>
        /// <typeparam name="TValue">Тип значений словаря</typeparam>
        /// <param name="collection">Коллекция элементов, добавляемых в словарь</param>
        /// <param name="dictionary">Словарь, в который добавляются элементы</param>
        /// <param name="converter">Метод определения ключа элемента</param>
        public static void AddToDictionary<TKey, TValue>
        (
            [CN] this IEnumerable<TValue> collection,
            [NN] IDictionary<TKey, TValue> dictionary,
            [NN] Func<TValue, TKey> converter
        )
        {
            if (collection is null) return;
            foreach (var value in collection)
                dictionary.Add(converter(value), value);
        }

        /// <summary>Преобразовать последовательность одних элементов в последовательность других элементов с использованием механизма конвертации</summary>
        /// <typeparam name="TItem">Тип исходных элементов</typeparam>
        /// <typeparam name="TValue">Тип элементов, в которые преобразуются исходные</typeparam>
        /// <param name="collection">Последовательность исходных элементов</param>
        /// <returns>Последовательность элементов преобразованного типа</returns>
        [CN]
        public static IEnumerable<TValue> ConvertToType<TItem, TValue>([CN] this IEnumerable<TItem> collection)
        {
            var target_type = typeof(TValue);
            var source_type = typeof(TItem);

            if (source_type == target_type) return (IEnumerable<TValue>)collection;
            if (collection is null) return null;

            Func<object, object> type_converter = null;
            var converter = source_type == typeof(object)
                        ? o => (TValue)target_type.Cast(o)
                        : (Func<TItem, TValue>)(o => (TValue)(type_converter ??= target_type.GetCasterFrom(source_type))(o));
            return collection.Select(converter);
        }

        /// <summary>Создать последовательность элементов, каждое значение в которой будет получено на основе двух значений исходной последовательности</summary>
        /// <typeparam name="TItem">Тип элементов исходной последовательности</typeparam>
        /// <typeparam name="TValue">Тип элементов последовательности конвертированных элементов</typeparam>
        /// <param name="collection">Исходная последовательность элементов</param>
        /// <param name="converter">
        /// Метод преобразования, в который передаётся исходный элемент последовательности, предыдущий элемент последовательности, 
        /// и на основе двух этих элементов, он определяет значение элемента результирующей последовательности</param>
        /// <returns>Последовательность элементов, составляемая из преобразованных элементов исходной последовательности, где метод преобразования учитывает значение предшествующего элемента</returns>
        [NN]
        public static IEnumerable<TValue> SelectWithLastValue<TItem, TValue>
        (
            [CN] this IEnumerable<TItem> collection,
            [NN] Func<TItem, TItem, TValue> converter
        )
        {
            if (collection is null) yield break;
            var first = true;
            var last = default(TItem);
            foreach (var item in collection)
            {
                if (first)
                {
                    last = item;
                    first = false;
                    continue;
                }
                yield return converter(last, item);
                last = item;
            }
        }

        /// <summary>Выполнить действие для первого элемента последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="action">Действие, выполняемое для первого элемента последовательности в момент её перечисления</param>
        /// <returns>Исходная последовательность элементов</returns>
        [NN]
        public static IEnumerable<T> AtFirst<T>([CN] this IEnumerable<T> collection, [NN] Action<T> action)
        {
            if (collection is null) yield break;
            var first = true;
            foreach (var item in collection)
            {
                if (first)
                {
                    action(item);
                    first = false;
                }
                yield return item;
            }
        }

        /// <summary>Выполнить действие для последнего элемента последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="action">Действие, выполняемое для последнего элемента в момент её перечисления. Если последовательность элементов просмотрена до конца</param>
        /// <returns>Исходная последовательность элементов</returns>
        [NN]
        public static IEnumerable<T> AtLast<T>([CN] this IEnumerable<T> collection, [NN] Action<T> action)
        {
            if (collection is null) yield break;
            var last = default(T);
            var any = false;
            foreach (var item in collection)
            {
                any = true;
                last = item;
                yield return last;
            }
            if (any) action(last);
        }

        /// <summary> Выполнить действие до начала перечисления последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="ActionBefore">Действие, выполняемое до начала перечисления элементов последовательности</param>
        /// <returns>Исходная последовательность элементов</returns>
        [NN]
        public static IEnumerable<T> Before<T>([CN] this IEnumerable<T> collection, [NN] Action ActionBefore)
        {
            if (collection is null) yield break;
            ActionBefore();
            foreach (var item in collection) yield return item;
        }

        /// <summary>Выполнение действия по завершению перечисления коллекции</summary>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <param name="collection">Коллекция элементов</param>
        /// <param name="CompleteAction">Действие, выполняемое по завершению перечисления коллекции</param>
        /// <returns>Исходная последовательность элементов</returns>
        [NN]
        public static IEnumerable<T> OnComplete<T>([CN] this IEnumerable<T> collection, [NN] Action CompleteAction)
        {
            if (collection is null) yield break;
            foreach (var item in collection) yield return item;
            CompleteAction();
        }

        /// <summary>История перечисления последовательности элементов</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        public sealed class EnumerableHistory<T> : IEnumerable<T>, IObservable<T>
        {
            /// <summary>Длина истории</summary>
            private int _HistoryLength;

            /// <summary>Список элементов в истории</summary>
            [NN]
            private readonly List<T> _Queue;

            /// <summary>Объект-наблюдения за историей</summary>
            [NN]
            private readonly SimpleObservableEx<T> _ObservableObject = new SimpleObservableEx<T>();

            /// <summary>Текущий элемент перечисления</summary>
            public T Current { get; private set; }

            /// <summary>Длина истории</summary>
            [MinValue(0)]
            public int Length { get => _HistoryLength; set { _HistoryLength = value; Check(); } }

            /// <summary>Количество элементов в истории</summary>
            [MinValue(0)]
            public int Count => _Queue.Count;

            /// <summary>Доступ к элементам истории начиная с текущего</summary>
            /// <param name="i">Индекс элемента в истории, где 0 - текущий элемент</param>
            /// <returns>Элемент истории перечисления</returns>
            public T this[[MinValue(0)] int i] => _Queue[_Queue.Count - i];

            /// <summary>Инициализация нового экземпляра <see cref="EnumerableHistory{T}"/></summary>
            /// <param name="HistoryLength">Длина истории</param>
            public EnumerableHistory([MinValue(0)] int HistoryLength)
            {
                _HistoryLength = HistoryLength;
                _Queue = new List<T>(HistoryLength);
            }

            /// <summary>Удаление лишних элементов из истории</summary>
            private void Check()
            {
                while (_Queue.Count > _HistoryLength) _Queue.RemoveAt(0);
            }

            /// <summary>Добавить элемент в историю перечисления</summary>
            /// <param name="item">Добавляемый элемент</param>
            [NN]
            public EnumerableHistory<T> Add(T item)
            {
                _Queue.Add(item);
                Current = item;
                _ObservableObject.OnNext(item);
                Check();
                return this;
            }

            /// <summary>Получить перечислитель истории элементов</summary><returns>Перечислитель истории элементов</returns>
            public IEnumerator<T> GetEnumerator() => _Queue.GetEnumerator();

            /// <summary>Получить перечислитель истории элементов</summary><returns>Перечислитель истории элементов</returns>
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            /// <summary>Подписка на изменения истории перечисления</summary>
            /// <param name="observer">Объект-подписчик, уведомляемый об изменениях в истории перечисления</param>
            /// <returns>Объект, осуществляющий возможность отписаться от уведомлений изменения истории перечисления</returns>
            public IDisposable Subscribe(IObserver<T> observer) => _ObservableObject.Subscribe(observer);
        }

        /// <summary>Преобразование исходной последовательности элементов с учётом указанного размера истории перечисления</summary>
        /// <typeparam name="TIn">Тип элементов исходной коллекции</typeparam>
        /// <typeparam name="TOut">Тип элементов результирующей коллекции</typeparam>
        /// <param name="collection">Исходная коллекция элементов</param>
        /// <param name="Selector">Метод преобразования элементов коллекции на основе истории их перечисления</param>
        /// <param name="HistoryLength">Максимальная длина истории перечисления</param>
        /// <returns>Коллекция элементов, сформированная на основе исходной с учётом истории процесса перечисления исходной коллекции</returns>
        [NN]
        public static IEnumerable<TOut> SelectWithHistory<TIn, TOut>
        (
            [NN] this IEnumerable<TIn> collection,
            [NN] Func<EnumerableHistory<TIn>, TOut> Selector,
            [MinValue(0)] int HistoryLength
        )
        {
            var History = new EnumerableHistory<TIn>(HistoryLength);
            return collection.Select(item => Selector(History.Add(item)));
        }

        /// <summary>Оценка статистических параметров перечисления</summary>
        /// <param name="collection">Перечисление значений, статистику которых требуется получить</param>
        /// <param name="Length">Размер выборки для оценки</param>
        /// <returns>Оценка статистики</returns>
        [NN]
        public static StatisticValue GetStatistic([NN] this IEnumerable<double> collection, [MinValue(0)] int Length = 0)
        {
            if (Length > 0)
                return new StatisticValue(Length)
                          .InitializeObject(collection, (sv, items) => sv.AddEnumerable(items)) 
                       ?? throw new InvalidOperationException();
            var values = collection.ToArray();
            var result = new StatisticValue(values.Length);
            result.AddEnumerable(values);
            return result;
        }

        /// <summary>Отбросить нулевые значения с конца перечисления</summary>
        /// <param name="collection">Фильтруемое перечисление</param>
        /// <returns>Перечисление чисел, в котором отсутствуют хвостовые нули</returns>
        [NN]
        public static IEnumerable<double> FilterNullValuesFromEnd([NN] this IEnumerable<double> collection)
        {
            var n = 0;
            foreach (var value in collection)
                if (value.Equals(0)) n++;
                else
                {
                    for (; n > 0; n--) yield return 0;
                    yield return value;
                }
        }

        /// <summary>Определить минимальное и максимальное значение перечисления</summary>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <param name="collection">Перечисление, минимум и максимум которого необходимо определить</param>
        /// <param name="selector">Метод преобразования объектов в вещественные числа</param>
        /// <param name="Min">Минимальный элемент перечисления</param>
        /// <param name="Max">Максимальный элемент перечисления</param>
        public static void GetMinMax<T>
        (
            [NN] this IEnumerable<T> collection,
            [NN] Func<T, double> selector,
            [CN] out T Min,
            [CN] out T Max
        )
        {
            var min = new MinValue();
            var max = new MaxValue();
            Min = default;
            Max = default;
            foreach (var value in collection)
            {
                var f = selector(value);
                if (min.AddValue(f)) Min = value;
                if (max.AddValue(f)) Max = value;
            }
        }

        /// <summary>Определить минимальное и максимальное значение перечисления</summary>
        /// <param name="collection">Перечисление, минимум и максимум которого необходимо определить</param>
        /// <param name="Min">Минимальный элемент перечисления</param>
        /// <param name="Max">Максимальный элемент перечисления</param>
        public static void GetMinMax
        (
            [NN] this IEnumerable<double> collection,
            out double Min,
            out double Max
        )
        {
            var min = new MinValue();
            var max = new MaxValue();
            Min = double.NaN;
            Max = double.NaN;
            foreach (var value in collection)
            {
                if (min.AddValue(value)) Min = value;
                if (max.AddValue(value)) Max = value;
            }
        }

        /// <summary>Определить минимальное и максимальное значение перечисления</summary>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <param name="collection">Перечисление, минимум и максимум которого необходимо определить</param>
        /// <param name="selector">Метод преобразования объектов в вещественные числа</param>
        /// <param name="Min">Минимальный элемент перечисления</param>
        /// <param name="MinIndex">Индекс минимального элемента в коллекции</param>
        /// <param name="Max">Максимальный элемент перечисления</param>
        /// <param name="MaxIndex">Индекс максимального элемента в коллекции</param>
        public static void GetMinMax<T>
        (
            [NN] this IEnumerable<T> collection,
            [NN] Func<T, double> selector,
            [CN] out T Min,
            out int MinIndex,
            [CN] out T Max,
            out int MaxIndex
        )
        {
            var min = new MinValue();
            var max = new MaxValue();
            Min = default;
            Max = default;
            MinIndex = MaxIndex = -1;
            var i = 0;
            foreach (var item in collection)
            {
                var f = selector(item);
                if (min.AddValue(f))
                {
                    Min = item;
                    MinIndex = i;
                }
                if (max.AddValue(f))
                {
                    Max = item;
                    MaxIndex = i;
                }
                i++;
            }
        }

        /// <summary>Определить минимальное и максимальное значение перечисления</summary>
        /// <param name="collection">Перечисление, минимум и максимум которого необходимо определить</param>
        /// <param name="Min">Минимальный элемент перечисления</param>
        /// <param name="MinIndex">Индекс минимального элемента в коллекции</param>
        /// <param name="Max">Максимальный элемент перечисления</param>
        /// <param name="MaxIndex">Индекс максимального элемента в коллекции</param>
        public static void GetMinMax
        (
            [NN] this IEnumerable<double> collection,
            out double Min,
            out int MinIndex,
            out double Max,
            out int MaxIndex
        )
        {
            var min = new MinValue();
            var max = new MaxValue();
            Min = double.NaN;
            Max = double.NaN;
            MinIndex = MaxIndex = -1;
            var i = 0;
            foreach (var item in collection)
            {
                if (min.AddValue(item))
                {
                    Min = item;
                    MinIndex = i;
                }
                if (max.AddValue(item))
                {
                    Max = item;
                    MaxIndex = i;
                }
                i++;
            }
        }

        /// <summary>Определение максимального элемента последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="selector">Метод преобразования элементов в вещественные числа</param>
        /// <returns>Максимальный элемент последовательности</returns>
        [CN]
        public static T GetMax<T>([NN] this IEnumerable<T> collection, [NN] Func<T, double> selector)
        {
            var max = new MaxValue();
            var result = default(T);
            foreach (var v in collection)
                if (max.AddValue(selector(v)))
                    result = v;
            return result;
        }

        /// <summary>Определение максимального элемента последовательности</summary>
        /// <param name="collection">Последовательность элементов</param>
        /// <returns>Максимальный элемент последовательности</returns>
        public static double GetMax([NN] this IEnumerable<double> collection)
        {
            var max = new MaxValue();
            var result = double.NaN;
            foreach (var v in collection)
                result = max.Add(v);
            return result;
        }

        /// <summary>Определение максимального элемента последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="selector">Метод преобразования элементов в вещественные числа</param>
        /// <param name="index">Индекс максимального элемента в последовательности</param>
        /// <returns>Максимальный элемент последовательности</returns>
        [CN]
        public static T GetMax<T>([NN] this IEnumerable<T> collection, [NN] Func<T, double> selector, out int index)
        {
            var max = new MaxValue();
            var result = default(T);
            var i = 0;
            index = -1;
            foreach (var item in collection)
            {
                if (max.AddValue(selector(item)))
                {
                    index = i;
                    result = item;
                }
                i++;
            }
            return result;
        }

        /// <summary>Определение максимального элемента последовательности</summary>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="index">Индекс максимального элемента в последовательности</param>
        /// <returns>Максимальный элемент последовательности</returns>
        public static double GetMax([NN] this IEnumerable<double> collection, out int index)
        {
            var max = new MaxValue();
            var result = double.NaN;
            var i = 0;
            index = -1;
            foreach (var item in collection)
            {
                if (max.AddValue(item))
                {
                    index = i;
                    result = item;
                }
                i++;
            }
            return result;
        }

        /// <summary>Определение минимального элемента последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="selector">Метод преобразования элементов в вещественные числа</param>
        /// <returns>Минимальный элемент последовательности</returns>
        [CN]
        public static T GetMin<T>([NN] this IEnumerable<T> collection, [NN] Func<T, double> selector)
        {
            var min = new MinValue();
            var result = default(T);
            foreach (var v in collection)
                if (min.AddValue(selector(v)))
                    result = v;
            return result;
        }

        /// <summary>Определение минимального элемента последовательности</summary>
        /// <param name="collection">Последовательность элементов</param>
        /// <returns>Минимальный элемент последовательности</returns>
        public static double GetMin([NN] this IEnumerable<double> collection)
        {
            var min = new MinValue();
            var result = double.NaN;
            foreach (var v in collection)
                result = min.Add(v);
            return result;
        }

        /// <summary>Определение минимального элемента последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="selector">Метод преобразования элементов в вещественные числа</param>
        /// <param name="index">Индекс минимального элемента в последовательности</param>
        /// <returns>Минимальный элемент последовательности</returns>
        [CN]
        public static T GetMin<T>([NN] this IEnumerable<T> collection, [NN] Func<T, double> selector, out int index)
        {
            var min = new MinValue();
            var result = default(T);
            var i = 0;
            index = -1;
            foreach (var item in collection)
            {
                if (min.AddValue(selector(item)))
                {
                    index = i;
                    result = item;
                }
                i++;
            }
            return result;
        }

        /// <summary>Определение минимального элемента последовательности</summary>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="index">Индекс минимального элемента в последовательности</param>
        /// <returns>Минимальный элемент последовательности</returns>
        public static double GetMin([NN] this IEnumerable<double> collection, out int index)
        {
            var min = new MinValue();
            var result = double.NaN;
            var i = 0;
            index = -1;
            foreach (var item in collection)
            {
                if (min.AddValue(item))
                {
                    index = i;
                    result = item;
                }
                i++;
            }
            return result;
        }

        /// <summary>Преобразование последовательности элементов в строку с указанной строкой-разделителем</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов, преобразуемая в строку</param>
        /// <param name="Separator">Строка-разделитель</param>
        /// <returns>Строка, составленная из строковых эквивалентов элементов входного перечисления, разделённых строкой-разделителем</returns>
        public static string ToSeparatedString<T>([NN] IEnumerable<T> collection, string Separator) =>
            collection
               .Select((t, i) => (s: t.ToString(), v: i == 0 ? string.Empty : Separator))
               .Aggregate(new StringBuilder(), (sb, v) => sb.AppendFormat("{0}{1}", v.s, v.v), sb => sb.ToString());

        /// <summary>Быстрое преобразование последовательности в список</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="Enum">Последовательность, преобразуемая в список</param>
        /// <returns>Список элементов последовательности</returns>
        [NN]
        public static IList<T> ToListFast<T>([NN] this IEnumerable<T> Enum) => Enum is IList<T> list ? list : Enum.ToList();

        /// <summary>Сумма последовательности комплексных чисел</summary>
        /// <param name="collection">Последовательность комплексных чисел</param>
        /// <returns>Комплексное число, являющееся суммой последовательности комплексных чисел</returns>
        [DST]
        public static Complex Sum([NN] this IEnumerable<Complex> collection) => collection.Aggregate((Z, z) => Z + z);

        [DST]
        public static Complex Sum<T>([NN] this IEnumerable<T> collection, [NN] Func<T, Complex> selector) => collection.Select(selector).Aggregate((Z, z) => Z + z);

        /// <summary>Объединить элементы коллекции</summary>
        /// <typeparam name="T">Тип элемента коллекции</typeparam>
        /// <typeparam name="TResult">Тип результата</typeparam>
        /// <param name="collection">Исходная коллекция элементов</param>
        /// <param name="Init">Исходное состояние результата объединения</param>
        /// <param name="func">Метод объединения</param>
        /// <param name="index">Индекс элемента коллекции</param>
        /// <returns>Результат объединения коллекции элементов</returns>
        [DST]
        public static TResult Aggregate<T, TResult>
        (
            [NN] this IEnumerable<T> collection,
            TResult Init,
            [NN] Func<TResult, T, int, TResult> func,
            int index = 0) =>
            collection.Aggregate(Init, (last, e) => func(last, e, index++));

        /// <summary>Проверка на наличие элемента в коллекции</summary>
        /// <typeparam name="T">Тип элемента</typeparam>
        /// <param name="collection">Проверяемая коллекция</param>
        /// <param name="selector">Метод выбора</param>
        /// <returns>Истина, если выполняется предикат хотя бы на одном элементе коллекции</returns>
        [DST]
        [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
        [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
        public static bool Contains<T>([NN] this IEnumerable<T> collection, [NN] Func<T, bool> selector)
        {
            if (collection is List<T> list1)
                for (var i = 0; i < list1.Count; i++)
                    if (selector(list1[i])) return true;

            if (collection is IList<T> i_list)
                for (var i = 0; i < i_list.Count; i++)
                    if (selector(i_list[i])) return true;

            // ReSharper disable once InvertIf
            if (collection is T[] array)
                if (array.Any(selector))
                    return true;

            return collection.Any(selector);
        }

        /// <summary>Найти элемент в перечислении, удовлетворяющий предикату</summary>
        /// <param name="collection">Перечисление элементов</param>
        /// <param name="selector">Предикат выбора</param>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <returns>Найденный элемент, либо пустая ссылка</returns>
        [DST]
        public static T Find<T>([NN] this IEnumerable<T> collection, [NN] Predicate<T> selector)
        {
            foreach (var local in collection.Where(local => selector(local))) return local;
            return default;
        }

        ///<summary>Выполнение действия для всех элементов коллекции</summary>
        ///<param name="collection">Коллекция элементов</param>
        ///<param name="Action">Выполняемое действие</param>
        ///<typeparam name="T">Тип элементов коллекции</typeparam>
        [DST]
        public static void Foreach<T>([NN] this IEnumerable<T> collection, [NN] Action<T> Action)
        {
            switch (collection)
            {
                case T[] array: for (int i = 0, count = array.Length; i < count; i++) Action(array[i]); break;
                case IList<T> list_t: for (int i = 0, count = list_t.Count; i < count; i++) Action(list_t[i]); break;
                case IList list: for (int i = 0, count = list.Count; i < count; i++) Action((T)list[i]); break;
                default: foreach (var item in collection) Action(item); break;
            }
        }

        /// <summary>Выполнение действия для всех элементов коллекции</summary>
        /// <param name="collection">Коллекция элементов</param>
        /// <param name="p">Параметр действия</param>
        /// <param name="Action">Выполняемое действие</param>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <typeparam name="TP">Тип параметра процесса перебора</typeparam>
        [DST]
        public static void Foreach<T, TP>(
            [NN] this IEnumerable<T> collection,
            [CN] TP p,
            [NN] Action<T, TP> Action)
        {
            switch (collection)
            {
                case T[] array: for (int i = 0, count = array.Length; i < count; i++) Action(array[i], p); break;
                case IList<T> list: for (int i = 0, count = list.Count; i < count; i++) Action(list[i], p); break;
                case IList list: for (int i = 0, count = list.Count; i < count; i++) Action((T)list[i], p); break;
                default: foreach (var item in collection) Action(item, p); break;
            }
        }

        /// <summary>Выполнение действия для всех элементов коллекции</summary>
        /// <param name="collection">Коллекция элементов</param>
        /// <param name="p1">Параметр 1 действия</param>
        /// <param name="p2">Параметр 2 действия</param>
        /// <param name="Action">Выполняемое действие</param>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <typeparam name="TP1">Тип параметра 1 процесса перебора</typeparam>
        /// <typeparam name="TP2">Тип параметра 2 процесса перебора</typeparam>
        [DST]
        public static void Foreach<T, TP1, TP2>(
            [NN] this IEnumerable<T> collection,
            [CN] TP1 p1,
            [CN] TP2 p2,
            [NN] Action<T, TP1, TP2> Action)
        {
            switch (collection)
            {
                case T[] array: for (int i = 0, count = array.Length; i < count; i++) Action(array[i], p1, p2); break;
                case IList<T> list: for (int i = 0, count = list.Count; i < count; i++) Action(list[i], p1, p2); break;
                case IList list: for (int i = 0, count = list.Count; i < count; i++) Action((T)list[i], p1, p2); break;
                default: foreach (var item in collection) Action(item, p1, p2); break;
            }
        }

        /// <summary>Выполнение действия для всех элементов коллекции</summary>
        /// <param name="collection">Коллекция элементов</param>
        /// <param name="p1">Параметр 1 действия</param>
        /// <param name="p2">Параметр 2 действия</param>
        /// <param name="p3">Параметр 3 действия</param>
        /// <param name="Action">Выполняемое действие</param>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <typeparam name="TP1">Тип 1 параметра процесса перебора</typeparam>
        /// <typeparam name="TP2">Тип 2 параметра процесса перебора</typeparam>
        /// <typeparam name="TP3">Тип 3 параметра процесса перебора</typeparam>
        [DST]
        public static void Foreach<T, TP1, TP2, TP3>(
            [NN] this IEnumerable<T> collection,
            [CN] TP1 p1,
            [CN] TP2 p2,
            [CN] TP3 p3,
            [NN] Action<T, TP1, TP2, TP3> Action)
        {
            switch (collection)
            {
                case T[] array: for (int i = 0, count = array.Length; i < count; i++) Action(array[i], p1, p2, p3); break;
                case IList<T> list: for (int i = 0, count = list.Count; i < count; i++) Action(list[i], p1, p2, p3); break;
                case IList list: for (int i = 0, count = list.Count; i < count; i++) Action((T)list[i], p1, p2, p3); break;
                default: foreach (var item in collection) Action(item, p1, p2, p3); break;
            }
        }

        ///<summary>Выполнение действия для всех элементов коллекции с указанием индекса элемента</summary>
        ///<param name="collection">Коллекция элементов</param>
        ///<param name="Action">Действие над элементом</param>
        ///<param name="index">Смещение индекса элемента коллекции</param>
        ///<typeparam name="T">Тип элемента коллекции</typeparam>
        [DST]
        public static void Foreach<T>([NN] this IEnumerable<T> collection, [NN] Action<T, int> Action, int index = 0)
        {
            switch (collection)
            {
                case T[] array: for (int i = 0, count = array.Length; i < count; i++) Action(array[i], index++); break;
                case IList<T> list: for (int i = 0, count = list.Count; i < count; i++) Action(list[i], index++); break;
                case IList list: for (int i = 0, count = list.Count; i < count; i++) Action((T)list[i], index++); break;
                default: foreach (var item in collection) Action(item, index++); break;
            }
        }

        /// <summary>Выполнение действия для всех элементов коллекции с указанием индекса элемента</summary>
        /// <param name="collection">Коллекция элементов</param>
        /// <param name="p">Параметр действия</param>
        /// <param name="Action">Действие над элементом</param>
        /// <param name="index">Смещение индекса элемента коллекции</param>
        /// <typeparam name="T">Тип элемента коллекции</typeparam>
        /// <typeparam name="TP">Тип параметра процесса перебора</typeparam>
        [DST]
        public static void Foreach<T, TP>(
            [NN] this IEnumerable<T> collection,
            [CN] TP p,
            [NN] Action<T, int, TP> Action,
            int index = 0)
        {
            switch (collection)
            {
                case T[] array: for (int i = 0, count = array.Length; i < count; i++) Action(array[i], index++, p); break;
                case IList<T> list: for (int i = 0, count = list.Count; i < count; i++) Action(list[i], index++, p); break;
                case IList list: for (int i = 0, count = list.Count; i < count; i++) Action((T)list[i], index++, p); break;
                default: foreach (var item in collection) Action(item, index++, p); break;
            }
        }

        /// <summary>Выполнение действия для всех элементов коллекции с указанием индекса элемента</summary>
        /// <param name="collection">Коллекция элементов</param>
        /// <param name="p1">Параметр действия 1</param>
        /// <param name="p2">Параметр действия 2</param>
        /// <param name="Action">Действие над элементом</param>
        /// <param name="index">Смещение индекса элемента коллекции</param>
        /// <typeparam name="T">Тип элемента коллекции</typeparam>
        /// <typeparam name="TP1">Тип параметра процесса перебора 1</typeparam>
        /// <typeparam name="TP2">Тип параметра процесса перебора 2</typeparam>
        [DST]
        public static void Foreach<T, TP1, TP2>(
            [NN] this IEnumerable<T> collection,
            [CN] TP1 p1,
            [CN] TP2 p2,
            [NN] Action<T, int, TP1, TP2> Action,
            int index = 0)
        {
            switch (collection)
            {
                case T[] array: for (int i = 0, count = array.Length; i < count; i++) Action(array[i], index++, p1, p2); break;
                case IList<T> list: for (int i = 0, count = list.Count; i < count; i++) Action(list[i], index++, p1, p2); break;
                case IList list: for (int i = 0, count = list.Count; i < count; i++) Action((T)list[i], index++, p1, p2); break;
                default: foreach (var item in collection) Action(item, index++, p1, p2); break;
            }
        }

        /// <summary>Выполнение действия для всех элементов коллекции с указанием индекса элемента</summary>
        /// <param name="collection">Коллекция элементов</param>
        /// <param name="p1">Параметр действия 1</param>
        /// <param name="p2">Параметр действия 2</param>
        /// <param name="p3">Параметр действия 3</param>
        /// <param name="Action">Действие над элементом</param>
        /// <param name="index">Смещение индекса элемента коллекции</param>
        /// <typeparam name="T">Тип элемента коллекции</typeparam>
        /// <typeparam name="TP1">Тип параметра процесса перебора 1</typeparam>
        /// <typeparam name="TP2">Тип параметра процесса перебора 2</typeparam>
        /// <typeparam name="TP3">Тип параметра процесса перебора 3</typeparam>
        [DST]
        public static void Foreach<T, TP1, TP2, TP3>(
            [NN] this IEnumerable<T> collection,
            [CN] TP1 p1,
            [CN] TP2 p2,
            [CN] TP3 p3,
            [NN] Action<T, int, TP1, TP2, TP3> Action,
            int index = 0)
        {
            switch (collection)
            {
                case T[] array: for (int i = 0, count = array.Length; i < count; i++) Action(array[i], index++, p1, p2, p3); break;
                case IList<T> list: for (int i = 0, count = list.Count; i < count; i++) Action(list[i], index++, p1, p2, p3); break;
                case IList list: for (int i = 0, count = list.Count; i < count; i++) Action((T)list[i], index++, p1, p2, p3); break;
                default: foreach (var item in collection) Action(item, index++, p1, p2, p3); break;
            }
        }

        /// <summary>Ленивое преобразование типов, пропускающее непреобразуемые объекты</summary>
        /// <param name="collection">Исходное перечисление объектов</param>
        /// <typeparam name="T">Тип объектов входного перечисления</typeparam>
        /// <returns>Коллекция объектов преобразованного типа</returns>
        [DST, NN]
        public static IEnumerable<T> CastLazy<T>([NN] IEnumerable collection)
        {
            var result = collection as IEnumerable<T>;
            return result ?? collection.Cast<object>().Where(item => item is T).Cast<T>();
        }

        /// <summary>Ленивое преобразование типов элементов перечисления</summary>
        /// <typeparam name="TItem">Тип элементов входной перечисления</typeparam>
        /// <typeparam name="TValue">Тип элементов перечисления, в который требуется осуществить преобразование</typeparam>
        /// <param name="collection">Исходная перечисление элементов</param>
        /// <returns>Перечисление элементов преобразованного типа</returns>
        [NN]
        public static IEnumerable<TValue> CastLazy<TItem, TValue>([NN] IEnumerable<TItem> collection) => typeof(TItem) == typeof(TValue) ? (IEnumerable<TValue>)collection : collection.OfType<TValue>();

        /// <summary>Действие, выполняемое в процессе перебора элементов для всех элементов перечисления при условии выполнения предиката</summary>
        /// <typeparam name="T">Ип элементов перечисления</typeparam>
        /// <param name="collection">Перечисление элементов, для которых надо выполнить действие</param>
        /// <param name="Predicate">Условие выполнения действия</param>
        /// <param name="Action">Действие, выполняемое для всех элементов перечисления</param>
        /// <returns>Исходное перечисление</returns>
        [NN]
        public static IEnumerable<T> ForeachLazyIf<T>
        (
            [NN] this IEnumerable<T> collection,
            [NN] Func<T, bool> Predicate,
            [CN] Action<T> Action
        ) =>
            Action is null ? collection : collection.Select(t =>
            {
                if (Predicate(t)) Action(t);
                return t;
            });

        /// <summary>Отложенное выполнение указанного действия для каждого элемента последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="Action">Выполняемое действие</param>
        /// <returns>Последовательность элементов, для элементов которой выполняется отложенное действие</returns>
        [DST, NN]
        public static IEnumerable<T> ForeachLazy<T>([NN] this IEnumerable<T> collection, [CN] Action<T> Action) =>
            Action is null ? collection : collection.Select(t =>
            {
                Action(t);
                return t;
            });

        /// <summary>Выполнение указанного действия на каждом шаге перебора последовательности после выдачи элемента</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="action">Действие, Выполняемое после выдачи элемента последовательности</param>
        /// <returns>Исходная последовательность элементов</returns>
        [NN]
        public static IEnumerable<T> ForeachLazyLast<T>([NN] this IEnumerable<T> collection, [CN] Action<T> action)
        {
            if (action is null)
                foreach (var value in collection) yield return value;
            else
                foreach (var value in collection)
                {
                    yield return value;
                    action(value);
                }
        }

        /// <summary>Отложенное выполнение действия до перебора элементов последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов</param>
        /// <param name="Action">Выполняемое действие</param>
        /// <param name="index">Начальный индекс элемента последовательности</param>
        /// <returns>Последовательность элементов, для элементов которой которой выполняется действие</returns>
        [DST, NN]
        public static IEnumerable<T> ForeachLazy<T>
        (
            [NN] this IEnumerable<T> collection,
            [CN] Action<T, int> Action,
            int index = 0
        ) =>
            Action is null ? collection : collection.Select(t =>
            {
                Action(t, index++);
                return t;
            });

        /// <summary>Пересечение последовательностей</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="Source">Исходная последовательность элементов</param>
        /// <param name="Items">Последовательность элементов, пересечение с которой вычисляется</param>
        /// <returns>Последовательность элементов, входящих как в первую, так и во вторую последовательности</returns>
        [NN]
        public static IEnumerable<T> ExistingItems<T>([NN] this IEnumerable<T> Source, [NN] IEnumerable<T> Items)
        {
            var list = Items.ToListFast();
            return Source.Where(t => list.Contains(i => Equals(i, t)));
        }

        /// <summary>Последовательность уникальных элементов</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="values">Исходная последовательность элементов</param>
        /// <param name="Comparer">Метод сравнения элементов</param>
        /// <returns>Последовательность элементов, таких, что ранее они отсутствовали во входной последовательности</returns>
        [NN]
        public static IEnumerable<T> GetUnique<T>([NN] this IEnumerable<T> values, [NN] Func<T, T, bool> Comparer)
        {
            var list = new List<T>();
            foreach (var value in values.Where(value => !list.Exists(v => Comparer(value, v))))
            {
                list.Add(value);
                yield return value;
            }
        }

        /// <summary>Последовательность уникальных элементов</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="values">Исходная последовательность элементов</param>
        /// <returns>Последовательность элементов, таких, что ранее они отсутствовали во входной последовательности</returns>
        [NN]
        public static IEnumerable<T> GetUnique<T>([NN] this IEnumerable<T> values)
        {
            var set = new HashSet<T>();
            return values.Where(v => !set.Contains(v)).ForeachLazy(v => set.Add(v));
        }

        /// <summary>Найти элементы, которые не входят во вторую последовательность</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="Source">Исходная последовательность</param>
        /// <param name="Items">Последовательность элементов, которых не должно быть в выходной последовательности</param>
        /// <returns>Последовательность элементов, которые отсутствуют во второй последовательности</returns>
        [NN]
        public static IEnumerable<T> MissingItems<T>([NN] this IEnumerable<T> Source, [NN] IEnumerable<T> Items)
        {
            var list = Items.ToListFast();
            return Source.Where(t => !list.Contains(i => Equals(i, t)));
        }

        /// <summary>Пересечение последовательностей</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="A">Первая последовательность</param>
        /// <param name="B">Вторая последовательность</param>
        /// <returns>Массив элементов, входящих и в первую и во вторую последовательности</returns>
        [NN]
        public static T[] Intersection<T>([NN] this IEnumerable<T> A, [NN] IEnumerable<T> B)
        {
            var a = A.ToListFast();
            var b = B.ToListFast();

            var result = new List<T>(a.Count + b.Count);
            result.AddRange(a.ExistingItems(b));
            result.AddRange(b.ExistingItems(a));

            return result.ToArray();
        }

        /// <summary>Последовательности элементов поэлементно равны</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="A">Первая последовательность</param>
        /// <param name="B">Вторая последовательность</param>
        /// <returns>Истина, если последовательности равны с точностью до элементов</returns>
        public static bool ItemEquals<T>([NN] this IEnumerable<T> A, [NN] IEnumerable<T> B)
        {
            using (var a = A.GetEnumerator())
            using (var b = B.GetEnumerator())
            {
                var next_a = a.MoveNext();
                var next_b = b.MoveNext();
                while (next_a && next_b)
                {
                    if (a.Current is null && b.Current is { }) return false;
                    if (a.Current is null || !a.Current.Equals(b.Current)) return false;
                    next_a = a.MoveNext();
                    next_b = b.MoveNext();
                }
                return next_a == next_b;
            }
        }

        /// <summary>Определение объектов, которые не входят в пересечение двух последовательностей</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="A">Исходная последовательность</param>
        /// <param name="B">Вторичная последовательность</param>
        /// <returns>Массив элементов, входящих либо в первую, либо во вторую последовательность</returns>
        [NN]
        public static T[] NotIntersection<T>([NN] this IEnumerable<T> A, [NN] IEnumerable<T> B)
        {
            var a = A.ToListFast();
            var b = B.ToListFast();

            var result = new List<T>(a.Count + b.Count);
            result.AddRange(a.MissingItems(b));
            result.AddRange(b.MissingItems(a));

            return result.ToArray();
        }

        /// <summary>Нахождение пересечения элементов двух последовательностей</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="A">Исходная последовательность</param>
        /// <param name="B">Вторичная последовательность</param>
        /// <param name="MissingInAFromB">Массив элементов, отсутствующих в первой последовательности</param>
        /// <param name="MissingInBFromA">Массив элементов, отсутствующих во второй последовательности</param>
        /// <param name="ExistingInAFromB">Массив элементов, присутствующих в первой последовательности</param>
        /// <param name="ExistingInBFromA">Массив элементов, присутствующих во второй последовательности</param>
        /// <param name="Intersection">Пересечение элементов</param>
        /// <param name="NotIntersection">Остаток от пересечения элементов</param>
        public static void Xor<T>
        (
            [NN] this IEnumerable<T> A,
            [NN] IEnumerable<T> B,
            [NN] out T[] MissingInAFromB,
            [NN] out T[] MissingInBFromA,
            [NN] out T[] ExistingInAFromB,
            [NN] out T[] ExistingInBFromA,
            [NN] out T[] Intersection,
            [NN] out T[] NotIntersection
        )
        {
            var a = A.ToListFast();
            var b = B.ToListFast();

            var missing_in_a_from_b_list = new List<T>(a.Count + b.Count);
            var missing_in_b_from_a_list = new List<T>(a.Count + b.Count);
            var existing_in_a_from_b_list = new List<T>(a.Count + b.Count);
            var existing_in_b_from_a_list = new List<T>(a.Count + b.Count);
            var intersection_list = new List<T>(a.Count + b.Count);
            var not_intersection_list = new List<T>(a.Count + b.Count);

            var b_existing_in_a = new bool[b.Count];
            foreach (var a_item in a)
            {
                var a_existing_in_b = false;

                for (var j = 0; j < b.Count; j++)
                {
                    var b_item = b[j];
                    if (!a_item.Equals(b_item)) continue;

                    a_existing_in_b = b_existing_in_a[j] = true;
                    break;
                }

                if (a_existing_in_b)
                {
                    existing_in_a_from_b_list.Add(a_item);
                    not_intersection_list.Add(a_item);
                }
                else
                {
                    missing_in_a_from_b_list.Add(a_item);
                    intersection_list.Add(a_item);
                }
            }

            for (var i = 0; i < b.Count; i++)
            {
                var b_item = b[i];
                if (b_existing_in_a[i])
                {
                    existing_in_b_from_a_list.Add(b_item);
                    not_intersection_list.Add(b_item);
                }
                else
                {
                    missing_in_b_from_a_list.Add(b_item);
                    intersection_list.Add(b_item);
                }
            }

            ExistingInAFromB = existing_in_a_from_b_list.ToArray();
            ExistingInBFromA = existing_in_b_from_a_list.ToArray();
            MissingInAFromB = missing_in_a_from_b_list.ToArray();
            MissingInBFromA = missing_in_b_from_a_list.ToArray();
            Intersection = intersection_list.ToArray();
            NotIntersection = not_intersection_list.ToArray();
        }

        /// <summary>Преобразовать последовательность в строку с указанной строкой-разделителем</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность объектов, преобразуемая в строку с разделителями</param>
        /// <param name="Separator">Разделитель элементов в строке</param>
        /// <returns>Строка, составленная из строковых представлений объектов последовательности, разделённых указанной строкой-разделителем</returns>
        [NN]
        // ReSharper disable once EmptyString
        public static string ToSeparatedStr<T>([NN] this IEnumerable<T> collection, [CN] string Separator = "") => 
            string.Join(Separator, collection.Select(o => o.ToString()).ToArray());

        /// <summary>Найти минимум и максимум последовательности вещественных чисел</summary>
        /// <param name="values">Последовательность вещественных чисел</param>
        /// <returns>Интервал, границы которого определяют минимум и максимум значений, которые принимала входная последовательность</returns>
        public static Interval GetMinMax([NN] this IEnumerable<double> values) => new MinMaxValue(values).Interval;

        /// <summary>Добавить элемент в конец последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Исходная последовательность элементов</param>
        /// <param name="obj">Добавляемый объект</param>
        /// <returns>Последовательность, составленная из элементов исходной последовательности и добавленного элемента</returns>
        [NN]
        public static IEnumerable<T> AppendLast<T>([CN] this IEnumerable<T> collection, [CN] T obj)
        {
            if (collection != null && !(collection is T[] array && array.Length == 0))
                foreach (var value in collection)
                    yield return value;
            if (obj is { })
                yield return obj;
        }

        /// <summary>Добавить последовательность элементов в конец последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="first_collection">Начальная последовательность элементов</param>
        /// <param name="last_collection">Завершающая последовательность элементов</param>
        /// <returns>Последовательность элементов, составленная из элементов первичной и вторичной последовательностей</returns>
        [NN]
        public static IEnumerable<T> AppendLast<T>
        (
            [CN] this IEnumerable<T> first_collection,
            [CN] IEnumerable<T> last_collection
        )
        {
            if (first_collection != null && !(first_collection is T[] first_array && first_array.Length == 0))
                foreach (var value in first_collection)
                    yield return value;
            if (last_collection is null || last_collection is T[] last_array && last_array.Length == 0) yield break;
            foreach (var value in last_collection)
                yield return value;
        }

        /// <summary>Добавить объект в начало перечисления</summary>
        /// <typeparam name="T">Тип объектов перечисления</typeparam>
        /// <param name="collection">Основное перечисление</param>
        /// <param name="obj">Объект, добавляемый в начало перечисления</param>
        /// <returns>Перечисление объектов, составленное из первого объекта и остального перечисления</returns>
        [NN]
        public static IEnumerable<T> AppendFirst<T>([CN] this IEnumerable<T> collection, [CN] T obj)
        {
            if (obj is { }) yield return obj;
            if (collection is null || collection is T[] items && items.Length == 0) yield break;
            foreach (var value in collection)
                yield return value;
        }

        /// <summary>Добавить перечисление объектов в начало основного перечисления</summary>
        /// <typeparam name="T">Тип объектов перечисления</typeparam>
        /// <param name="last_collection">Первая последовательность объектов</param>
        /// <param name="first_collection">Вторая последовательность объектов</param>
        /// <returns>Последовательность объектов, составленная из первой последовательности, за которой следует вторая последовательность</returns>
        [NN]
        public static IEnumerable<T> AppendFirst<T>
        (
            [CN] this IEnumerable<T> last_collection,
            [CN] IEnumerable<T> first_collection
        )
        {
            if (first_collection != null && !(first_collection is T[] first_array && first_array.Length == 0))
                foreach (var value in first_collection)
                    yield return value;
            if (last_collection is null || last_collection is T[] last_array && last_array.Length == 0) yield break;
            foreach (var value in last_collection)
                yield return value;
        }

        /// <summary>Вставить элемент в указанное положение в последовательности</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Последовательность элементов, в которую требуется вставить новый элемент</param>
        /// <param name="obj">Элемент, добавляемый в последовательность</param>
        /// <param name="pos">Положение в которое требуется вставить элемент</param>
        /// <returns>Последовательность элементов, в указанной позиции которой будет размещён указанный элемент</returns>
        [NN]
        public static IEnumerable<T> InsertAtPos<T>([NN] this IEnumerable<T> collection, T obj, int pos)
        {
            var i = 0;
            foreach (var value in collection)
            {
                if (i == pos) { yield return obj; i++; }
                yield return value;
                i++;
            }
        }

        /// <summary>Инверсная конкатенация перечислений</summary>
        /// <typeparam name="T">Тип элементов перечислений</typeparam>
        /// <param name="FirstCollection">Исходная последовательность, добавляемая в конец</param>
        /// <param name="SecondCollection">Вторичная последовательность, добавляемая в начало</param>
        /// <returns>Последовательность элементов, составленная из элементов вторичной последовательности и элементов первичной последовательности</returns>
        [NN]
        public static IEnumerable<T> ConcatInverted<T>
        (
            [NN] this IEnumerable<T> FirstCollection,
            [NN] IEnumerable<T> SecondCollection
        )
        {
            foreach (var value in SecondCollection) yield return value;
            foreach (var value in FirstCollection) yield return value;
        }

        /// <summary>Сумма перечисления полиномов</summary>
        /// <param name="P">Перечисление полиномов, которые надо сложить</param>
        /// <returns>Полином, являющийся суммой полиномов</returns>
        [NN]
        public static Polynom Sum([NN] this IEnumerable<Polynom> P)
        {
            Polynom result = null;
            foreach (var p in P)
                if (result is null) result = p;
                else
                    result += p;
            return result ?? new Polynom(0);
        }

        /// <summary>Произведение перечисления полиномов</summary>
        /// <param name="P">Перечисление полиномов, которые надо перемножить</param>
        /// <returns>Полином, являющийся произведением полиномов</returns>
        [NN]
        public static Polynom Multiply([NN] this IEnumerable<Polynom> P)
        {
            Polynom result = null;
            foreach (var p in P)
                if (result is null) result = p;
                else
                    result *= p;
            return result ?? new Polynom(1);
        }

        /// <summary>Проредить последовательность</summary>
        /// <typeparam name="T">Тип элементов последовательности</typeparam>
        /// <param name="collection">Прореживаемая последовательность</param>
        /// <param name="N">Размер выборки > 0</param>
        /// <param name="k">Положение в выборке (от 0 до N-1)</param>
        /// <returns>Последовательность из N-ых элементов выборки, стоящих на k-ом месте</returns>
        [NN]
        public static IEnumerable<T> Decimate<T>([NN] this IEnumerable<T> collection, int N, int k = 0)
        {
            var i = 0;
            foreach (var v in collection)
                if (i++ % N == k)
                    yield return v;
        }

        /// <summary>Получить первый и последний элементы перечисления</summary>
        /// <typeparam name="T">Тип элементов перечисления</typeparam>
        /// <param name="enumerable">Перечисление</param>
        /// <returns>Перечисление, состоящее из первого и последнего элементов исходного перечисления</returns>
        [NN]
        public static IEnumerable<T> TakeFirstAndLast<T>([NN] this IEnumerable<T> enumerable)
        {
            var last = default(T);
            var first_taken = false;
            foreach (var item in enumerable)
                if (!first_taken)
                {
                    yield return item;
                    first_taken = true;
                }
                else
                    last = item;

            if (first_taken)
                yield return last;
        }
    }
}