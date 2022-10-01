#nullable enable
namespace MathCore.Values;

/// <summary>Объект, обладающий значением</summary>
/// <typeparam name="T">Тип значения объекта</typeparam>
public interface IValueRead<out T>
{
    /// <summary>Значение объекта</summary>
    T Value { get; }
}