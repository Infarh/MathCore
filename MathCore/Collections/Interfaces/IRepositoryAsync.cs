#nullable enable
namespace MathCore.Collections.Interfaces;

/// <summary>Репозиторий с асинхронными операциями доступа к данным</summary>
/// <typeparam name="T">Тип элемента репозитория</typeparam>
public interface IRepositoryAsync<T>
{
    /// <summary>Запрос к репозиторию</summary>
    IQueryable<T> Items { get; }

    /// <summary>Число элементов в репозитории</summary>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Количество элементов в репозитории</returns>
    Task<int> CountAsync(CancellationToken Cancel = default);

    /// <summary>Получить всё содержимое репозитория</summary>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Перечисление всех элементов репозитория</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken Cancel = default);

    /// <summary>Получить элементы в заданном диапазоне</summary>
    /// <param name="Skip">Число пропускаемых элементов</param>
    /// <param name="Take">Число извлекаемых элементов</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Перечисление элементов репозитория в заданном диапазоне</returns>
    Task<IEnumerable<T>> GetAsync(int Skip, int Take, CancellationToken Cancel = default);

    /// <summary>Получить страницу элементов из репозитория</summary>
    /// <param name="Index">Индекс страницы начиная с 0</param>
    /// <param name="Size">Число элементов на страницу</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Страница с элементами</returns>
    Task<IPage<T>> GetPageAsync(int Index, int Size, CancellationToken Cancel = default);

    /// <summary>Добавить элемент в репозиторий</summary>
    /// <param name="item">Добавляемый элемент</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Добавленный элемент, если он отсутствовал, либо <c>null</c></returns>
    Task<T?> AddAsync(T item, CancellationToken Cancel = default);

    /// <summary>Обновить состояние элемента в репозитории</summary>
    /// <param name="item">Обновляемый элемент</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Обновлённый элемент, либо <c>null</c> если обновление невозможно</returns>
    Task<T?> UpdateAsync(T item, CancellationToken Cancel = default);

    /// <summary>Удаление элемента из репозитория</summary>
    /// <param name="item">Удаляемый элемент</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns>Удалённый элемент, если он присутствовал в репозитории, и <c>null</c>, если элемента в репозитории не было</returns>
    Task<T?> DeleteAsync(T item, CancellationToken Cancel = default);
}