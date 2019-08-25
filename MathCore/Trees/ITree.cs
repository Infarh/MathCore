using System.Collections.Generic;

namespace MathCore.Trees
{
    //[ContractClass(typeof(ContractClassForITree<>))]
    public interface ITree<TValue> : IIndexable<ITree<TValue>>, IEnumerable<ITree<TValue>>
    {
        int Depth { get; }

        TValue Value { get; set; }
    }

    //[ContractClassFor(typeof(ITree<>))]
    //internal abstract class ContractClassForITree<TValue> : ITree<TValue>
    //{
    //    public int Count { get { Contract.Ensures(Contract.Result<int>() >= 0); return default(int); } }

    //    public int Depth { get { Contract.Ensures(Contract.Result<int>() >= 0); return default(int); } }

    //    public TValue Value { get { return default(TValue); } set { } }

    //    /// <summary>Индексатор объекта</summary><param name="index">Индекс</param>
    //    public ITree<TValue> this[int index]
    //    {
    //        get
    //        {
    //            Contract.Requires(index >= 0);
    //            Contract.Requires(index < Count);
    //            return default(ITree<TValue>);
    //        }
    //        set
    //        {
    //            Contract.Requires(index >= 0);
    //            Contract.Requires(index < Count);
    //        }
    //    }

    //    /// <summary>Возвращает перечислитель, выполняющий перебор элементов в коллекции.</summary>
    //    /// <returns>
    //    /// Интерфейс <see cref="T:System.Collections.Generic.IEnumerator`1"/>, который может использоваться для перебора элементов коллекции.
    //    /// </returns>
    //    /// <filterpriority>1</filterpriority>
    //    public IEnumerator<ITree<TValue>> GetEnumerator() { return Enumerable.Empty<ITree<TValue>>().GetEnumerator(); }

    //    /// <summary>Возвращает перечислитель, который осуществляет перебор элементов коллекции.</summary>
    //    /// <returns>
    //    /// Объект <see cref="T:System.Collections.IEnumerator"/>, который может использоваться для перебора элементов коллекции.
    //    /// </returns>
    //    /// <filterpriority>2</filterpriority>
    //    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    //}
}
