#nullable enable
using System.Collections.Generic;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Data;

/// <summary>Класс методов-расширений для <see cref="IDataReader"/></summary>
// ReSharper disable once InconsistentNaming
public static class IDataReaderExtensions
{
    /// <summary>Прочитать источник данных до конца</summary>
    /// <typeparam name="T">Тип читаемых данных</typeparam>
    /// <param name="Reader">Объект чтения данных</param>
    /// <param name="Read">Метод преобразования читаемых данных из <see cref="IDataRecord"/> в <typeparamref name="T"/></param>
    /// <returns>Список прочитанных данных</returns>
    public static List<T> ReadToEnd<T>(this IDataReader Reader, Func<IDataRecord, T> Read)
    {
        var items = new List<T>();

        while(Reader.Read())
            items.Add(Read(Reader));

        return items;
    }

    /// <summary>Представление <see cref="IDataReader"/> в виде перечисление значений</summary>
    /// <param name="Reader">Объект чтения данных</param>
    /// <param name="Read">Метод преобразования читаемых данных из <see cref="IDataRecord"/> в <typeparamref name="T"/></param>
    /// <typeparam name="T">Тип результата (элементы данных)</typeparam>
    /// <returns>Перечисление читаемых данных</returns>
    public static IEnumerable<T> AsEnumerable<T>(this IDataReader Reader, Func<IDataRecord, T> Read)
    {
        while(Reader.Read()) yield return Read(Reader);
    }
}