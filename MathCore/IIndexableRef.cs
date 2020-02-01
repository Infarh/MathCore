namespace MathCore
{
    /// <summary>Объект, индексируемый целочисленным индексом по ссылке</summary>
    /// <typeparam name="TValue">Тип элементов</typeparam>
    public interface IIndexableRef<TValue>
    {
        /// <summary>Ссылка на значение, доступное внутри объекта по индексу</summary>
        /// <param name="index">Индекс значения</param>
        ref TValue this[int index] { get; }
    }

    /// <summary>Объект, индексируемый целочисленным индексом по ссылке только для чтения</summary>
    /// <typeparam name="TValue">Тип элементов</typeparam>
    public interface IIndexableReadonlyRef<TValue>
    {
        /// <summary>Ссылка только для чтения на значение, доступное внутри объекта по индексу</summary>
        /// <param name="index">Индекс значения</param>
        ref readonly TValue this[int index] { get; }
    }
}