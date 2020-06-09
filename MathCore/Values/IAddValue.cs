namespace MathCore.Values
{
    /// <summary>Объект, позволяющий добавлять в себя значения</summary>
    /// <typeparam name="T">Тип значений</typeparam>
    public interface IAddValue<T> : IValue<T>
    {
        /// <summary>Добавить значение</summary>
        /// <param name="value">Добавляемое значение</param>
        /// <returns>Результирующее значение</returns>
        double AddValue(double value);
    }
}