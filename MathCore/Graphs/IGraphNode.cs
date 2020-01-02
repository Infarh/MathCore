using System.Collections.Generic;

namespace MathCore.Graphs
{
    /// <summary>Узел графа</summary>
    /// <typeparam name="TValue">Тип значения узла</typeparam>
    /// <typeparam name="TWeight">Тип нагрузки на связь</typeparam>
    public interface IGraphNode<out TValue, out TWeight> : IEnumerable<IGraphNode<TValue, TWeight>>
    {
        /// <summary>Связи узла</summary>
        IEnumerable<IGraphLink<TValue, TWeight>> Links { get; }

        /// <summary>Значение узла</summary>
        TValue Value { get; }
    }

    public interface IGraphNode<out TValue> : IEnumerable<IGraphNode<TValue>>
    {
        /// <summary>Связи узла</summary>
        IEnumerable<IGraphNode<TValue>> Childs { get; }

        /// <summary>Значение узла</summary>
        TValue Value { get; }
    }
}