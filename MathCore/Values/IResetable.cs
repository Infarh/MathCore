namespace MathCore.Values
{
    /// <summary>Объект, позволяющий осуществлять сброс своего состояния</summary>
    public interface IResetable
    {
        /// <summary>Сброс состояния</summary>
        void Reset();
    }
}