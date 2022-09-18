#nullable enable

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Data;

/// <summary>Класс методов-расширений для <see cref="IDataRecord"/></summary>
// ReSharper disable once InconsistentNaming
public static class IDataRecordExtensions
{
    /// <summary>Извлечение поля данных из записи по индексу поля</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="record">Запись с данными</param>
    /// <param name="index">Индекс поля с данными в записи</param>
    /// <exception cref="NullReferenceException">Если запись с указанным индексом отсутствует, а тип данных не является ссылочным</exception>
    /// <returns>Значение поля, если в записи с указанным индексом оно есть, либо значение по умолчанию</returns>
    public static T? Field<T>(this IDataRecord record, int index) => 
        !record.IsDBNull(index) 
            ? (T)record.GetValue(index) 
            : typeof(T).IsCanBeNullRef() 
                ? default 
                : throw new InvalidOperationException($"Поле с индексом {index} отсутствует в записи");

    /// <summary>Извлечение поля данных из записи по имени поля</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="record">Запись с данными</param>
    /// <param name="ColumnName">Имя колонки</param>
    /// <exception cref="NullReferenceException">Если запись с указанным именем отсутствует, а тип данных не является ссылочным</exception>
    /// <returns>Значение поля, если в записи с указанным индексом оно есть, либо значение по умолчанию</returns>
    public static T? Field<T>(this IDataRecord record, string ColumnName)
    {
        var index = record.GetOrdinal(ColumnName);
        return !record.IsDBNull(index) 
            ? (T)record.GetValue(index) 
            : typeof(T).IsCanBeNullRef() 
                ? default 
                : throw new NullReferenceException();
    }
}