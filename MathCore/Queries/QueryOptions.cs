using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MathCore.Annotations;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace MathCore.Queries
{
    /// <summary>Параметры запроса</summary>
    public class QueryOptions
    {
        /// <summary>Метод упорядочивания запроса по возрастанию</summary>
        [NotNull]
        private static readonly MethodInfo __OrderBy = typeof(Queryable)
           .GetMethods()
           .Single(method => method.Name == nameof(Queryable.OrderBy)
                             && method.IsGenericMethodDefinition
                             && method.GetGenericArguments().Length == 2
                             && method.GetParameters().Length == 2);

        /// <summary>Метод упорядочивания запроса по убыванию</summary>
        [NotNull]
        private static readonly MethodInfo __OrderByDescending = typeof(Queryable)
           .GetMethods()
           .Single(method => method.Name == nameof(Queryable.OrderByDescending)
                             && method.IsGenericMethodDefinition
                             && method.GetGenericArguments().Length == 2
                             && method.GetParameters().Length == 2);

        /// <summary>Получить метод упорядочивания для заданного типа данных и типа ключа упорядочивания</summary>
        /// <param name="ItemsType">Тип упорядочиваемых элементов</param>
        /// <param name="SortingKeyType">Тип ключа, по которому должна выполняться сортировка значений</param>
        /// <param name="IsDescending">Упорядочивать ли по убыванию?</param>
        /// <returns>Информация о методе, выполняющем упорядочивание</returns>
        [NotNull]
        private static MethodInfo GetMethodInfo([NotNull] Type ItemsType, [NotNull] Type SortingKeyType, bool IsDescending) =>
            (IsDescending ? __OrderByDescending : __OrderBy).MakeGenericMethod(ItemsType, SortingKeyType);

        /// <summary>Пулл методов упорядочивания элементов</summary>
        [NotNull] private static readonly ConcurrentDictionary<(Type Source, Type Result, bool Descending), Delegate> __Selectors = new();

        /// <summary>Получить метод, формирующий запрос с упорядочиванием элементов</summary>
        /// <typeparam name="TItems">Тип упорядочиваемых элементов</typeparam>
        /// <param name="SortingKeyType">Тип ключа, по которому должна выполняться сортировка значений</param>
        /// <param name="IsDescending">Упорядочивать ли по убыванию?</param>
        /// <returns>Метод, выполняющем упорядочивание</returns>
        private static Delegate GetMethod<TItems>(Type SortingKeyType, bool IsDescending) =>
            __Selectors.GetOrAdd((typeof(TItems), SortingKeyType, IsDescending), info =>
            {
                var (source, result, descending) = info;
                var method = GetMethodInfo(source, result, descending);
                var delegate_type = typeof(Func<,,>)
                   .MakeGenericType(
                        typeof(IQueryable<TItems>),
                        typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(typeof(TItems), result)),
                        typeof(IQueryable<TItems>));
                return method.CreateDelegate(delegate_type);
            });

        /// <summary>Номер страницы (начиная с нуля)</summary>
        public int Page { get; set; } = 0;

        /// <summary>Число элементов на странице (по умолчанию 10)</summary>
        public int Size { get; set; } = 10;

        /// <summary>Свойство, по которому выполняется упорядочивание</summary>
        public string OrderProperty { get; set; }

        /// <summary>Выполнять ли упорядочивание по убыванию?</summary>
        public bool OrderByDescending { get; set; }

        /// <summary>Свойство, используемое для поиска</summary>
        public string SearchProperty { get; set; }

        /// <summary>Искомое значение</summary>
        public string SearchTerm { get; set; }

        /// <summary>Сформировать отфильтрованный и упорядоченный запрос</summary>
        /// <typeparam name="T">Тип элементов запроса</typeparam>
        /// <param name="query">Исходный запрос</param>
        /// <returns>Кортеж, содержащий упорядоченный и отфильтрованный запрос, а также число элементов исходного запроса</returns>
        internal (IQueryable<T> Query, int TotalCount) Items<T>(IQueryable<T> query)
        {
            var items = Order(Search(query));
            return (items.Skip(Page * Size).Take(Size), items.Count());
        }

        /// <summary>Сформировать выражение <see cref="Expression"/> для доступа к указанному свойству</summary>
        /// <param name="x">Объект, свойство которого требуется получить</param>
        /// <param name="PropertyQuery">Строка с определением свойства</param>
        /// <returns>Выражение, определяющее доступ указанному свойству</returns>
        private static Expression GetProperty(Expression x, [NotNull] string PropertyQuery) => PropertyQuery
           .Split('.')
           .Aggregate(x, Expression.Property);

        /// <summary>Метод, осуществляющий упорядочивание элементов запроса</summary>
        /// <typeparam name="T">Тип элементов запроса</typeparam>
        /// <param name="query">Исходный запрос</param>
        /// <returns>Упорядоченный запрос</returns>
        private IQueryable<T> Order<T>(IQueryable<T> query)
        {
            if (string.IsNullOrWhiteSpace(OrderProperty)) return query;

            var x = Expression.Parameter(typeof(T), "x");
            var value = GetProperty(x, OrderProperty);
            var criteria = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(T), value.Type), value, x);

            //return GetMethodInfo(typeof(T), value.Type, OrderByDescending)
            //   .Invoke(null, new object[] { query, criteria })
            //    as IQueryable<T>;
            return (IQueryable<T>)GetMethod<T>(value.Type, OrderByDescending).DynamicInvoke(query, criteria);
        }

        /// <summary>Метод, осуществляющий фильтрацию элементов запроса</summary>
        /// <typeparam name="T">Тип элементов запроса</typeparam>
        /// <param name="query">Исходный запрос</param>
        /// <returns>Отфильрованный запрос</returns>
        private IQueryable<T> Search<T>(IQueryable<T> query)
        {
            if (string.IsNullOrWhiteSpace(SearchProperty) || string.IsNullOrEmpty(SearchTerm)) return query;

            var x = Expression.Parameter(typeof(T), "x");

            var value = GetProperty(x, SearchProperty);
            if (value.Type != typeof(string))
                value = Expression.Call(value, "ToString", Type.EmptyTypes);

            var body = Expression.Call(value, "Contains", Type.EmptyTypes, Expression.Constant(SearchTerm));
            var condition = Expression.Lambda<Func<T, bool>>(body, x);

            return query.Where(condition);
        }
    }
}