

namespace MathCore
{
    /// <summary>Объект индексируемый только для чтения значений</summary>
    /// <typeparam name="TIndex">Тип индекса</typeparam>
    /// <typeparam name="TValue">Тип элементов</typeparam>
    public interface IIndexableRead<in TIndex, out TValue>
    {
        /// <summary>Индексатор объекта только для чтения</summary>
        /// <param name="index">Индекс</param>
        TValue this[TIndex index] { get; }
    }

    /// <summary>Объект индексируемый только для чтения значений с двумя индексами</summary>
    /// <typeparam name="TIndex1">Тип первого индекса</typeparam>
    /// <typeparam name="TIndex2">Тип второго индекса</typeparam>
    /// <typeparam name="TValue">Тип значений</typeparam>
    public interface IIndexableRead<in TIndex1, in TIndex2, out TValue>
    {
        /// <summary>Индексатор объекта только для чтения</summary>
        /// <param name="index1">Первый индекс</param>
        /// <param name="index2">Второй индекс</param>
        TValue this[TIndex1 index1, TIndex2 index2] { get; }
    }

    /// <summary>Объект индексируемый только для записи значений</summary>
    /// <typeparam name="TIndex">Тип индекса</typeparam>
    /// <typeparam name="TValue">Тип элементов</typeparam>
    public interface IIndexableWrite<in TIndex, in TValue>
    {
        /// <summary>Индексатор объекта только для записи</summary><param name="index">Индекс</param>
        TValue this[TIndex index] { set; }
    }

    /// <summary>Объект индексируемый только для записи значений с двумя индексами</summary>
    /// <typeparam name="TIndex1">Тип первого индекса</typeparam>
    /// <typeparam name="TIndex2">Тип второго индекса</typeparam>
    /// <typeparam name="TValue">Тип значений</typeparam>
    public interface IIndexableWrite<in TIndex1, in TIndex2, in TValue>
    {
        /// <summary>Индексатор объекта только для записи</summary>
        /// <param name="index1">Первый индекс</param><param name="index2">Второй индекс</param>
        TValue this[TIndex1 index1, TIndex2 index2] { set; }
    }

    /// <summary>Объект индексируемый</summary>
    /// <typeparam name="TIndex">Тип индекса</typeparam><typeparam name="TValue">Тип элементов</typeparam>
    public interface IIndexable<in TIndex, TValue> : IIndexableRead<TIndex, TValue>, IIndexableWrite<TIndex, TValue>
    {
        /// <summary>Индексатор объекта</summary><param name="index">Индекс</param>
        new TValue this[TIndex index] { get; set; }
    }

    /// <summary>Объект индексируемый с двумя параметрами индекса</summary>
    /// <typeparam name="TIndex1">Тип первого индекса</typeparam>
    /// <typeparam name="TIndex2">Тип второго индекса</typeparam>
    /// <typeparam name="TValue">Тип элементов</typeparam>
    public interface IIndexable<in TIndex1, in TIndex2, TValue> :
        IIndexableRead<TIndex1, TIndex2, TValue>,
        IIndexableWrite<TIndex1, TIndex2, TValue>
    {
        /// <summary>Индексатор объекта</summary>
        /// <param name="index1">Первый индекс</param><param name="index2">Второй индекс</param>
        new TValue this[TIndex1 index1, TIndex2 index2] { get; set; }
    }

    /// <summary>Объект индексируемый с целочисленным индексом</summary>
    /// <typeparam name="TValue">Тип элементов</typeparam>
    public interface IIndexable<TValue> : IIndexable<int, TValue>, ICountable { }
}