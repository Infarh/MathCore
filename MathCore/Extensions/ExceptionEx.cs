namespace MathCore.Extensions;

/// <summary>Класс методов-расширений для работы с исключениями</summary>
public static class ExceptionEx
{
    /// <summary>Добавляет пару ключ-значение в коллекцию данных исключения и возвращает это исключение</summary>
    /// <typeparam name="TException">Тип исключения.</typeparam>
    /// <param name="exception">Исключение, в которое добавляются данные.</param>
    /// <param name="Key">Ключ данных.</param>
    /// <param name="Value">Значение данных.</param>
    /// <returns>Исключение с добавленными данными.</returns>
    public static TException WithData<TException>(this TException exception, object Key, object Value)
       where TException : Exception
    {
        exception.Data[Key] = Value;
        return exception;
    }
}
