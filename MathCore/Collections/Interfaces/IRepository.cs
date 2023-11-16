#nullable enable
namespace MathCore.Collections.Interfaces;

/// <summary>Репозиторий</summary>
/// <typeparam name="T">Тип элемента репозитория</typeparam>
public interface IRepository<T>
{
    /// <summary>Запрос к репозиторию</summary>
    IQueryable<T> Items { get; }

    /// <summary>Число элементов в репозитории</summary>
    /// <returns>Количество элементов в репозитории</returns>
    int Count();

    /// <summary>Получить всё содержимое репозитория</summary>
    /// <returns>Перечисление всех элементов репозитория</returns>
    IEnumerable<T> GetAll();

    /// <summary>Получить элементы в заданном диапазоне</summary>
    /// <param name="Skip">Число пропускаемых элементов</param>
    /// <param name="Take">Число извлекаемых элементов</param>
    /// <returns>Перечисление элементов репозитория в заданном диапазоне</returns>
    IEnumerable<T> Get(int Skip, int Take);

    /// <summary>Получить страницу элементов из репозитория</summary>
    /// <param name="Index">Индекс страницы начиная с 0</param>
    /// <param name="Size">Число элементов на страницу</param>
    /// <returns>Страница с элементами</returns>
    IPage<T> GetPage(int Index, int Size);

    /// <summary>Добавить элемент в репозиторий</summary>
    /// <param name="item">Добавляемый элемент</param>
    /// <returns>Добавленный элемент, если он отсутствовал, либо <c>null</c></returns>
    T? Add(T item);

    /// <summary>Обновить состояние элемента в репозитории</summary>
    /// <param name="item">Обновляемый элемент</param>
    /// <returns>Обновлённый элемент, либо <c>null</c> если обновление невозможно</returns>
    T? Update(T item);

    /// <summary>Удаление элемента из репозитория</summary>
    /// <param name="item">Удаляемый элемент</param>
    /// <returns>Удалённый элемент, если он присутствовал в репозитории, и <c>null</c>, если элемента в репозитории не было</returns>
    T? Delete(T item);
}