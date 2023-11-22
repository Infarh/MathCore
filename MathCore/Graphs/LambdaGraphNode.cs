#nullable enable
using System.Collections;

// ReSharper disable UnusedMember.Global

namespace MathCore.Graphs;

public class LambdaGraphNode<TValue, TWeight>(TValue Value, Func<TValue, IEnumerable<TValue>?>? GetChilds,
        Func<TValue, TValue, TWeight> GetWeight, bool Buffered = false)
    : IGraphNode<TValue, TWeight>, IEquatable<LambdaGraphNode<TValue, TWeight>>
{
    private readonly Func<TValue, IEnumerable<TValue>?>? _GetChilds = GetChilds;

    private readonly Func<TValue, TValue, TWeight> _GetWeight = GetWeight;
    private IGraphLink<TValue, TWeight>[]? _Links;

    /// <inheritdoc />
    public IEnumerable<IGraphLink<TValue, TWeight>> Links => Buffered
        ? _Links ??= (_GetChilds(Value) ?? Enumerable.Empty<TValue>())
           .Select(v => new LambdaGraphNode<TValue, TWeight>(v, _GetChilds, _GetWeight, Buffered))
           .Select(to => new LambdaGraphLink<TValue, TWeight>(this, to, _GetWeight, Buffered))
           .Cast<IGraphLink<TValue, TWeight>>().ToArray()
        : (_GetChilds(Value) ?? Enumerable.Empty<TValue>())
       .Select(v => new LambdaGraphNode<TValue, TWeight>(v, _GetChilds, _GetWeight))
       .Select(to => new LambdaGraphLink<TValue, TWeight>(this, to, _GetWeight))
       .Cast<IGraphLink<TValue, TWeight>>();

    public TValue Value { get; } = Value;

    public LambdaGraphNode<TValue, TWeight>[] Childs => this.Cast<LambdaGraphNode<TValue, TWeight>>().ToArray();

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = _GetChilds?.GetHashCode() ?? 0;
            hash = (hash * 397) ^ (_GetWeight?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ EqualityComparer<TValue>.Default.GetHashCode(Value!);
            return hash;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    [DST]
    public static bool operator ==(LambdaGraphNode<TValue, TWeight>? left, LambdaGraphNode<TValue, TWeight>? right) => Equals(left, right);

    [DST]
    public static bool operator !=(LambdaGraphNode<TValue, TWeight>? left, LambdaGraphNode<TValue, TWeight>? right) => !Equals(left, right);

    public override bool Equals(object? obj) => obj != null &&
        (ReferenceEquals(this, obj) ||
            obj.GetType() == GetType() &&
            Equals((LambdaGraphNode<TValue, TWeight>)obj));

    public bool Equals(LambdaGraphNode<TValue, TWeight>? other) => other != null
        && (ReferenceEquals(this, other) 
            || Equals(_GetChilds, other._GetChilds) 
            && Equals(_GetWeight, other._GetWeight) 
            && EqualityComparer<TValue>.Default.Equals(Value, other.Value));

    [DST]
    public IEnumerator<IGraphNode<TValue, TWeight>> GetEnumerator() => Links.Select(link => link.Node).GetEnumerator();

    [DST] public override string ToString() => $"λ[{(Value is null ? string.Empty : Value.ToString())}]";
}

public class LambdaGraphNode<V>(V Value, Func<V, IEnumerable<V>?> GetChilds, bool Buffered = false) : IGraphNode<V>, IEquatable<LambdaGraphNode<V>>
{
    private IEnumerable<IGraphNode<V>>? _Childs;

    /// <summary>Связи узла</summary>
    public IEnumerable<IGraphNode<V>> Childs => Buffered
        ? _Childs ??= (GetChilds(Value) ?? Enumerable.Empty<V>())
           .Select(value => new LambdaGraphNode<V>(value, GetChilds, Buffered))
           .Cast<IGraphNode<V>>()
           .ToArray()
        : (GetChilds(Value) ?? Enumerable.Empty<V>()).Select(value => new LambdaGraphNode<V>(value, GetChilds, Buffered));

    /// <summary>Значение узла</summary>
    public V Value { get; set; } = Value;

    public LambdaGraphNode<V>[] ChildsArray => Childs.Cast<LambdaGraphNode<V>>().ToArray();

    /// <inheritdoc />
    [DST]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [DST]
    public IEnumerator<IGraphNode<V>> GetEnumerator() => Childs.GetEnumerator();

    /// <inheritdoc />
    [DST]
    public override string ToString() => $"λ:{Value}";

    /// <inheritdoc />
    public bool Equals(LambdaGraphNode<V>? other) => other != null
        && (ReferenceEquals(this, other) 
            || EqualityComparer<V>.Default.Equals(Value, other.Value));

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as LambdaGraphNode<V>);

    /// <inheritdoc />
    public override int GetHashCode() => EqualityComparer<V>.Default.GetHashCode(Value!);
}