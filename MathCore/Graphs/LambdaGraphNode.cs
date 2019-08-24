using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MathCore.Graphs
{
    public class LambdaGraphNode<TValue, TWeight> : IGraphNode<TValue, TWeight>, IEquatable<LambdaGraphNode<TValue, TWeight>>
    {
        private readonly Func<TValue, IEnumerable<TValue>> _GetChields;

        private readonly Func<TValue, TValue, TWeight> _GetWeight;
        private readonly bool _Buffered;
        private IGraphLink<TValue, TWeight>[] _Links;

        /// <inheritdoc />
        public IEnumerable<IGraphLink<TValue, TWeight>> Links => _Buffered
            ? (_Links ?? (_Links = (_GetChields(Value) ?? Enumerable.Empty<TValue>())
                .Select(v => new LambdaGraphNode<TValue, TWeight>(v, _GetChields, _GetWeight, _Buffered))
                .Select(to => new LambdaGraphLink<TValue, TWeight>(this, to, _GetWeight, _Buffered))
                .Cast<IGraphLink<TValue, TWeight>>().ToArray()))
            : (_GetChields(Value) ?? Enumerable.Empty<TValue>())
                .Select(v => new LambdaGraphNode<TValue, TWeight>(v, _GetChields, _GetWeight))
                .Select(to => new LambdaGraphLink<TValue, TWeight>(this, to, _GetWeight))
                .Cast<IGraphLink<TValue, TWeight>>();

        public TValue Value { get; }

        public LambdaGraphNode<TValue, TWeight>[] Childs => this.Cast<LambdaGraphNode<TValue, TWeight>>().ToArray();

        public LambdaGraphNode(TValue Value, Func<TValue, IEnumerable<TValue>> GetChields, Func<TValue, TValue, TWeight> GetWeight, bool Buffered = false)
        {
            _GetChields = GetChields;
            _GetWeight = GetWeight;
            _Buffered = Buffered;
            this.Value = Value;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = _GetChields?.GetHashCode() ?? 0;
                hash = (hash * 397) ^ (_GetWeight?.GetHashCode() ?? 0);
                hash = (hash * 397) ^ EqualityComparer<TValue>.Default.GetHashCode(Value);
                return hash;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [DebuggerStepThrough]
        public static bool operator ==(LambdaGraphNode<TValue, TWeight> left, LambdaGraphNode<TValue, TWeight> right) => Equals(left, right);

        [DebuggerStepThrough]
        public static bool operator !=(LambdaGraphNode<TValue, TWeight> left, LambdaGraphNode<TValue, TWeight> right) => !Equals(left, right);

        public override bool Equals(object obj) => !ReferenceEquals(null, obj) &&
                                                   (ReferenceEquals(this, obj) ||
                                                    obj.GetType() == GetType() &&
                                                    Equals((LambdaGraphNode<TValue, TWeight>)obj));

        public bool Equals(LambdaGraphNode<TValue, TWeight> other) => !ReferenceEquals(null, other) 
            && (ReferenceEquals(this, other) 
            || Equals(_GetChields, other._GetChields) 
            && Equals(_GetWeight, other._GetWeight) 
            && EqualityComparer<TValue>.Default.Equals(Value, other.Value));

        [DebuggerStepThrough]
        public IEnumerator<IGraphNode<TValue, TWeight>> GetEnumerator() => Links.Select(link => link.Node).GetEnumerator();

        [DebuggerStepThrough]
        public override string ToString() => $"λ[{(ReferenceEquals(Value, null) ? "" : Value.ToString())}]";
    }

    public class LambdaGraphNode<V> : IGraphNode<V>, IEquatable<LambdaGraphNode<V>>
    {
        private readonly Func<V, IEnumerable<V>> _GetChields;
        private readonly bool _Buffered;
        private IEnumerable<IGraphNode<V>> _Childs;

        /// <summary>Связи узла</summary>
        public IEnumerable<IGraphNode<V>> Childs => _Buffered
            ? _Childs ?? (_Childs = (_GetChields(Value) ?? Enumerable.Empty<V>())
                  .Select(value => new LambdaGraphNode<V>(value, _GetChields, _Buffered))
                  .Cast<IGraphNode<V>>()
                  .ToArray())
            : (_GetChields(Value) ?? Enumerable.Empty<V>()).Select(value => new LambdaGraphNode<V>(value, _GetChields, _Buffered));

        /// <summary>Значение узла</summary>
        public V Value { get; set; }

        public LambdaGraphNode<V>[] ChildsArray => Childs.Cast<LambdaGraphNode<V>>().ToArray();

        public LambdaGraphNode(V Value, Func<V, IEnumerable<V>> GetChields, bool Buffered = false)
        {
            _GetChields = GetChields;
            _Buffered = Buffered;
            this.Value = Value;
        }

        /// <inheritdoc />
        [DebuggerStepThrough]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        [DebuggerStepThrough]
        public IEnumerator<IGraphNode<V>> GetEnumerator() => Childs.GetEnumerator();

        /// <inheritdoc />
        [DebuggerStepThrough]
        public override string ToString() => $"λ:{Value}";

        /// <inheritdoc />
        public bool Equals(LambdaGraphNode<V> other) => !ReferenceEquals(null, other) 
                                                        && (ReferenceEquals(this, other) 
                                                            || EqualityComparer<V>.Default.Equals(Value, other.Value));

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as LambdaGraphNode<V>);

        /// <inheritdoc />
        public override int GetHashCode() => EqualityComparer<V>.Default.GetHashCode(Value);
    }
}