#nullable enable
namespace MathCore.Values;

/// <summary>Объект, позволяющий осуществлять сброс своего состояния</summary>
public interface IResettable
{
    /// <summary>Сброс состояния</summary>
    void Reset();
}