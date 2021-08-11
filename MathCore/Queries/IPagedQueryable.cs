using System.Linq;

// ReSharper disable UnusedMemberInSuper.Global

namespace MathCore.Queries
{
    /// <summary>Страница запроса</summary>
    /// <typeparam name="T">Тип элементов запроса</typeparam>
    public interface IPagedQueryable<out T> : IQueryable<T>
    {
        /// <summary>Исходный запрос</summary>
        IQueryable<T> SourceQuery { get; }

        /// <summary>Номер страницы</summary>
        int Page { get; }

        /// <summary>Число элементов на странице</summary>
        int PageSize { get; }

        /// <summary>Общее число страниц</summary>
        int PagesCount { get; }
    }
}