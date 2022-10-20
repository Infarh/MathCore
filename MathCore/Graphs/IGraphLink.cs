namespace MathCore.Graphs;

/// <summary>Связь узла</summary>
/// <typeparam name="TValue">Тип значения узла</typeparam>
/// <typeparam name="TWeight">Тип нагрузки на связь</typeparam>
public interface IGraphLink<out TValue, out TWeight>
{
    /// <summary>Связанный узел</summary>
    IGraphNode<TValue, TWeight> Node { get; }
    /// <summary>Нагрузка на связь</summary>
    TWeight Weight { get; }
}