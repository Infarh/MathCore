using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable ReturnTypeCanBeEnumerable.Global

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.CommandProcessor;

/// <summary>Команда</summary>
public readonly struct Command : IEquatable<Command>
{
    /// <summary>Имя команды</summary>
    public string Name { get; }

    /// <summary>Параметр команды</summary>
    public string Parameter { get; }

    /// <summary>Массив аргументов команды</summary>
    private readonly Argument[] _Argument;

    /// <summary>Массив аргументов команды</summary>
    public IReadOnlyList<Argument> Argument => _Argument;

    /// <summary>Команда</summary>
    /// <param name="CommandStr">Строковое представление команды</param>
    /// <param name="ParameterSplitter">Разделитель имени и параметра команды</param>
    /// <param name="ArgSplitter">Разделитель аргументов команды</param>
    /// <param name="ValueSplitter">Разделитель имени аргумента и его значения</param>
    public Command([NotNull] string CommandStr, char ParameterSplitter = ':', char ArgSplitter = ' ', char ValueSplitter = '=')
    {
        var items      = CommandStr.Split(ArgSplitter);
        var name_items = items[0].Split(ParameterSplitter);
        Name      = name_items[0];
        Parameter = name_items.Length > 1 ? name_items[1] : null;

        _Argument = items.Skip(1).Where(ArgStr => !string.IsNullOrEmpty(ArgStr))
           .Select(ArgStr => new Argument(ArgStr, ValueSplitter))
           .Where(arg => !string.IsNullOrEmpty(arg.Name))
           .ToArray();
    }

    /// <summary>Преобразование в строку</summary>
    /// <returns>Строковое представление команды</returns>
    [NotNull]
    public override string ToString() => 
        $"{Name}{(Parameter is null ? string.Empty : Parameter.ToFormattedString("({0})"))}{(_Argument is null || _Argument.Length == 0 ? string.Empty : _Argument.ToSeparatedStr(" ").ToFormattedString(" {0}"))}";

    /// <inheritdoc />
    public bool Equals(Command other) => Equals(_Argument, other._Argument) && Name == other.Name && Parameter == other.Parameter;

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is Command other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash_code = _Argument != null ? _Argument.GetHashCode() : 0;
        unchecked
        {
            hash_code = (hash_code * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            hash_code = (hash_code * 397) ^ (Parameter != null ? Parameter.GetHashCode() : 0);
            return hash_code;
        }
    }

    /// <summary>Оператор, проверяющий равенство между двумя экземплярами <see cref="Command"/></summary>
    /// <returns>Истина, если все поля экземпляров равны между собой</returns>
    public static bool operator ==(Command left, Command right) => left.Equals(right);

    /// <summary>Оператор, проверяющий неравенство между двумя экземплярами <see cref="Command"/></summary>
    /// <returns>Истина, если хотя бы одно поле у экземпляров отличается</returns>
    public static bool operator !=(Command left, Command right) => !left.Equals(right);
}