namespace MathCore.Extensions;

/// <summary>Статический класс расширения для работы с исключениями</summary>
public static class ExceptionEx
{
    /// <summary>Добавляет пару ключ-значение в коллекцию Data исключения</summary>
    /// <typeparam name="TException">Тип исключения</typeparam>
    /// <param name="exception">Исключение</param>
    /// <param name="Key">Ключ данных</param>
    /// <param name="Value">Значение данных</param>
    /// <returns>Исключение с добавленными данными</returns>
    public static TException WithData<TException>(this TException exception, object Key, object Value)
        where TException : Exception
    {
        exception.Data[Key] = Value; // Добавление данных в исключение
        return exception; // Возврат модифицированного исключения
    }
}
