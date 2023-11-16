#nullable enable
// ReSharper disable ReturnTypeCanBeEnumerable.Global

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.CommandProcessor;

/// <summary>Аргумент команды</summary>
public readonly struct Argument: IEquatable<Argument>
{
    /// <summary>Имя аргумента</summary>
    public string Name { get; }

    /// <summary>Значения аргумента</summary>
    private readonly string[] _Values;

    /// <summary>Значения аргумента</summary>
    public IReadOnlyList<string> Values => _Values;

    /// <summary>Значение аргумента</summary>
    public string Value => _Values?.Length > 0 ? _Values[0] : string.Empty;

    /// <summary>Количество значений аргумента</summary>
    public int Count => _Values?.Length ?? 0;

    /// <summary>Доступ к значениям аргумента по номеру</summary>
    /// <param name="i">Номер значения</param>
    /// <returns>Значение аргумента с указанным номером</returns>
    public ref string this[int i] => ref _Values[i];

    /// <summary>Аргумент команды</summary>
    /// <param name="ArgStr">Строковое описание аргумента</param>
    /// <param name="ValueSplitter">Разделитель имени аргумента и значения</param>
    public Argument(string ArgStr, char ValueSplitter = '=')
        : this()
    {
        var arg_items = ArgStr.Split(ValueSplitter);
        Name = arg_items[0].ClearSystemSymbolsAtBeginAndEnd();
        _Values = arg_items.Skip(1)
           .Select(value => value.ClearSystemSymbolsAtBeginAndEnd())
           .Where(value => !string.IsNullOrEmpty(value))
           .ToArray();
    }

    /// <summary>Представление значения в указанном типе</summary>
    /// <typeparam name="T">Требуемый тип значения аргумента</typeparam>
    /// <returns>Значение аргумента указанного типа</returns>
    public T ValueAs<T>() => (T)Convert.ChangeType(Value, typeof(T));

    /// <summary>Попытаться получить значение аргумента команды в указанном типе <typeparamref name="T"/></summary>
    /// <param name="value">Приведённое к типу <typeparamref name="T"/> значение аргумента</param>
    /// <typeparam name="T">Требуемый тип значения аргумента</typeparam>
    /// <returns>Исключение, возникшее в процессе преобразования строки значения аргумента к типу <typeparamref name="T"/></returns>
    public bool TryGetValueAs<T>(out T value)
    {
        try
        {
            value = ValueAs<T>();
            return true;
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch
        {
            value = default;
            return false;
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    /// <summary>Попытаться получить значение аргумента команды в указанном типе <typeparamref name="T"/></summary>
    /// <param name="value">Приведённое к типу <typeparamref name="T"/> значение аргумента</param>
    /// <param name="Error">Исключение, возникшее в процессе преобразования строки значения аргумента к типу <typeparamref name="T"/></param>
    /// <typeparam name="T">Требуемый тип значения аргумента</typeparam>
    /// <returns>Истина, если преобразование выполнено успешно</returns>
    public bool TryGetValueAs<T>(out T value, out Exception? Error)
    {
        try
        {
            value = ValueAs<T>();
            Error = null;
            return true;
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception error)
        {
            value = default;
            Error = error;
            return false;
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    /// <summary>Преобразование в строку</summary>
    /// <returns>Строковое представление аргумента</returns>
    public override string ToString() => $"{Name}{(_Values is null || _Values.Length == 0 ? string.Empty : Values.ToSeparatedStr(", ").ToFormattedString("={0}"))}";

    /// <inheritdoc />
    public bool Equals(Argument other) => Equals(_Values, other._Values) && Name == other.Name;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Argument other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return ((_Values != null ? _Values.GetHashCode() : 0) * 397) ^ (Name != null ? Name.GetHashCode() : 0);
        }
    }

    /// <summary>Оператор, проверяющий равенство между двумя экземплярами <see cref="Argument"/></summary>
    /// <returns>Истина, если все поля экземпляров равны между собой</returns>
    public static bool operator ==(Argument left, Argument right) => left.Equals(right);

    /// <summary>Оператор, проверяющий неравенство между двумя экземплярами <see cref="Argument"/></summary>
    /// <returns>Истина, если хотя бы одно поле у экземпляров отличается</returns>
    public static bool operator !=(Argument left, Argument right) => !left.Equals(right);
}