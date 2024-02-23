namespace MathCore.Graphs;

public class LambdaGraphLink<TValue, TWeight>(
    IGraphNode<TValue, TWeight> From,
    IGraphNode<TValue, TWeight> To,
    Func<TValue, TValue, TWeight> GetWeight,
    bool Buffered = false)
    : IGraphLink<TValue, TWeight>
{
    private object _Weight;
    public IGraphNode<TValue, TWeight> Node { get; } = To;

    public TWeight Weight => Buffered
        ? (TWeight) (_Weight ??= GetWeight(From.Value, Node.Value))
        : GetWeight(From.Value, Node.Value);
}