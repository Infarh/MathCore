#nullable enable
using MathCore.Collections.Interfaces;

namespace MathCore.Collections;

/// <summary>Репозиторий на основе коллекции</summary>
/// <typeparam name="T">Тип элемента репозитория</typeparam>
/// <remarks>Новый репозиторий</remarks>
/// <param name="Collection">Коллекция элементов репозитория</param>
/// <exception cref="ArgumentNullException"></exception>
public class Repository<T>(ICollection<T> Collection) : IRepository<T>
{
    /// <summary>Коллекция элементов репозитория</summary>
    private readonly ICollection<T> _Collection = Collection ?? throw new ArgumentNullException(nameof(Collection));

    /// <summary>Новый репозиторий</summary>
    public Repository() : this(new List<T>()) { }

    /// <summary>Запрос к репозиторию</summary>
    public virtual IQueryable<T> Items => _Collection.AsQueryable();

    /// <summary>Число элементов в репозитории</summary>
    /// <returns>Количество элементов в репозитории</returns>
    public virtual int Count() => _Collection.Count;

    /// <summary>Получить всё содержимое репозитория</summary>
    /// <returns>Перечисление всех элементов репозитория</returns>
    public virtual IEnumerable<T> GetAll() => _Collection.AsEnumerable();

    /// <summary>Получить элементы в заданном диапазоне</summary>
    /// <param name="Skip">Число пропускаемых элементов</param>
    /// <param name="Take">Число извлекаемых элементов</param>
    /// <returns>Перечисление элементов репозитория в заданном диапазоне</returns>
    public virtual IEnumerable<T> Get(int Skip, int Take)
    {
        if (Take == 0 || Skip >= _Collection.Count)
            return Enumerable.Empty<T>();

        IEnumerable<T> query = _Collection;
        if (Skip > 0)
            query = query.Skip(Skip);
        return query.Take(Take);
    }

    /// <summary>Получить страницу элементов из репозитория</summary>
    /// <param name="Index">Индекс страницы начиная с 0</param>
    /// <param name="Size">Число элементов на страницу</param>
    /// <returns>Страница с элементами</returns>
    public virtual IPage<T> GetPage(int Index, int Size)
    {
        var total_count = _Collection.Count;
        var skip_size = Index * Size;
        if (Size == 0 || skip_size >= total_count)
            return Page<T>.Empty(total_count, Index, Size);

        IEnumerable<T> query = _Collection;

        if (Index > 0)
            query = query.Skip(skip_size);

        return new Page<T>(query, Math.Min(total_count - skip_size, Size), total_count, Index, Size);
    }

    /// <summary>Добавить элемент в репозиторий</summary>
    /// <param name="item">Добавляемый элемент</param>
    /// <returns>Добавленный элемент, если он отсутствовал, либо <c>null</c></returns>
    /// <exception cref="ArgumentNullException">Если передана пустая ссылка на элемент</exception>
    public virtual T? Add(T item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        if (_Collection.Contains(item))
            return default;
        _Collection.Add(item);
        return item;
    }

    /// <summary>Обновить состояние элемента в репозитории</summary>
    /// <param name="item">Обновляемый элемент</param>
    /// <returns>Обновлённый элемент, либо <c>null</c> если обновление невозможно</returns>
    /// <exception cref="ArgumentNullException">Если передана пустая ссылка на элемент</exception>
    public virtual T? Update(T item) => item ?? throw new ArgumentNullException(nameof(item));

    /// <summary>Удаление элемента из репозитория</summary>
    /// <param name="item">Удаляемый элемент</param>
    /// <returns>Удалённый элемент, если он присутствовал в репозитории, и <c>null</c>, если элемента в репозитории не было</returns>
    /// <exception cref="ArgumentNullException">Если передана пустая ссылка на элемент</exception>
    public T? Delete(T item) =>
        item is null
            ? throw new ArgumentNullException(nameof(item))
            : _Collection.Remove(item)
                ? item
                : default;
}
