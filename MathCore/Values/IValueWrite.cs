#nullable enable
namespace MathCore.Values;

/// <summary>Объект, позволяющий определять значение</summary>
/// <typeparam name="T">Тип значений объекта</typeparam>
public interface IValueWrite<in T>
{
    /// <summary>Значение объекта</summary>
    T Value { set; }
}