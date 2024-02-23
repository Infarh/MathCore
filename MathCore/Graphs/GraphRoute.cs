#nullable enable
using System.Collections;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.Graphs;

public class GraphRoute<TValue, TWeight>(IGraphNode<TValue, TWeight>[] Nodes) : IEnumerable<IGraphNode<TValue, TWeight>>
{
    public int Length => Nodes.Length;

    public IGraphNode<TValue, TWeight> First
    {
        get
        {
            if(Nodes.Length == 0)
                throw new("Попытка доступа к первому узлу пустого пути");
            return Nodes[0];
        }
    }

    public IGraphNode<TValue, TWeight> Last
    {
        get
        {
            if(Nodes.Length == 0)
                throw new("Попытка доступа к последнему узлу пустого пути");
            return Nodes[^1];
        }
    }

    public bool IsEmpty => Nodes.Length == 0;

    public ref readonly IGraphNode<TValue, TWeight> this[int i] => ref Nodes[i];

    public GraphRoute(IEnumerable<IGraphNode<TValue, TWeight>> Collection) : this(Collection.ToArray()) { }

    public TWeight GetWeight(Func<TWeight, TWeight, TWeight> Aggregator) => this.SelectWithLastValue((last, next) => last.Links.First(l => l.Node.Equals(next)).Weight).Aggregate(Aggregator);

    /// <inheritdoc />
    public IEnumerator<IGraphNode<TValue, TWeight>> GetEnumerator() => Nodes.Cast<IGraphNode<TValue, TWeight>>().GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => Nodes.GetEnumerator();

    /// <inheritdoc />
    public override string ToString() => IsEmpty ? "Empty route" : $"{First} -[{Length}]-> {Last}";
}