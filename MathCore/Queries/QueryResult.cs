#nullable enable
using System.Collections;
using System.Linq.Expressions;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable UnusedMember.Global

namespace MathCore.Queries;

/// <summary>Результат выполнения запроса с разбиением на страницы, упорядочиванием и фильтрацией</summary>
/// <typeparam name="T">Тип элементов запроса</typeparam>
public class QueryResult<T> : IPagedQueryable<T>
{
    /// <summary>Сформированный запрос</summary>
    private readonly IQueryable<T> _Query;

    public IQueryable<T> SourceQuery { get; set; }

    /// <summary>Число элементов исходного запроса</summary>
    private readonly int _TotalItemsCount;

    /// <summary>Параметры запроса</summary>
    public QueryOptions Options { get; }

    /// <summary>Номер страницы (начиная с нуля)</summary>
    public int Page => Options?.Page ?? 0;

    /// <summary>Размер страницы (по умолчанию 10)</summary>
    public int PageSize => Options?.Size ?? 10;

    /// <summary>Число страниц исходного запроса</summary>
    public int PagesCount => (int)Math.Ceiling(_TotalItemsCount / (double)PageSize);

    /// <summary>Существует ли предыдущая страница?</summary>
    public bool HasPreviousPage => Page > 0;

    /// <summary>Существует ли следующая страница?</summary>
    public bool HasNextPage => PagesCount - Page > 1;

    public QueryResult(IQueryable<T> query, QueryOptions? Options = null)
    {
        SourceQuery                = query;
        (_Query, _TotalItemsCount) = Options is null ? (query, query.Count()) : (this.Options = Options).Items(query);
    }

    /// <summary>Получить результат запроса для предыдущей страницы</summary>
    /// <returns>Результата запроса для предыдущей страницы</returns>
    public QueryResult<T>? GetPreviousPage() => HasPreviousPage
        ? new QueryResult<T>(SourceQuery, new()
        {
            Page              = Page - 1,
            Size              = PageSize,
            OrderProperty     = Options?.OrderProperty,
            OrderByDescending = Options?.OrderByDescending ?? false,
            SearchProperty    = Options?.SearchProperty,
            SearchTerm        = Options?.SearchTerm
        })
        : null;

    /// <summary>Получить результат запроса для следующей страницы</summary>
    /// <returns>Результата запроса для следующей страницы</returns>
    public QueryResult<T>? GetNextPage() => HasNextPage
        ? new QueryResult<T>(SourceQuery, new()
        {
            Page              = Page + 1,
            Size              = PageSize,
            OrderProperty     = Options?.OrderProperty,
            OrderByDescending = Options?.OrderByDescending ?? false,
            SearchProperty    = Options?.SearchProperty,
            SearchTerm        = Options?.SearchTerm
        })
        : null;

    /// <inheritdoc />
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => _Query.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Query).GetEnumerator();

    /// <inheritdoc />
    Type IQueryable.ElementType => _Query.ElementType;

    /// <inheritdoc />
    Expression IQueryable.Expression => _Query.Expression;

    /// <inheritdoc />
    IQueryProvider IQueryable.Provider => _Query.Provider;
}