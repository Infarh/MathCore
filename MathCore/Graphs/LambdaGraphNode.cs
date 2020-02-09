using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable UnusedMember.Global

namespace MathCore.Graphs
{
    public class LambdaGraphNode<TValue, TWeight> : IGraphNode<TValue, TWeight>, IEquatable<LambdaGraphNode<TValue, TWeight>>
    {
        private readonly Func<TValue, IEnumerable<TValue>> _GetChilds;

        private readonly Func<TValue, TValue, TWeight> _GetWeight;
        private readonly bool _Buffered;
        private IGraphLink<TValue, TWeight>[] _Links;

        /// <inheritdoc />
        [NotNull]
        public IEnumerable<IGraphLink<TValue, TWeight>> Links => _Buffered
            ? _Links ??= (_GetChilds(Value) ?? Enumerable.Empty<TValue>())
               .Select(v => new LambdaGraphNode<TValue, TWeight>(v, _GetChilds, _GetWeight, _Buffered))
               .Select(to => new LambdaGraphLink<TValue, TWeight>(this, to, _GetWeight, _Buffered))
               .Cast<IGraphLink<TValue, TWeight>>().ToArray()
            : (_GetChilds(Value) ?? Enumerable.Empty<TValue>())
                .Select(v => new LambdaGraphNode<TValue, TWeight>(v, _GetChilds, _GetWeight))
                .Select(to => new LambdaGraphLink<TValue, TWeight>(this, to, _GetWeight))
            // ReSharper disable once RedundantEnumerableCastCall
           .Cast<IGraphLink<TValue, TWeight>>();

        public TValue Value { get; }

        [NotNull] public LambdaGraphNode<TValue, TWeight>[] Childs => this.Cast<LambdaGraphNode<TValue, TWeight>>().ToArray();

        public LambdaGraphNode(TValue Value, Func<TValue, IEnumerable<TValue>> GetChilds, Func<TValue, TValue, TWeight> GetWeight, bool Buffered = false)
        {
            _GetChilds = GetChilds;
            _GetWeight = GetWeight;
            _Buffered = Buffered;
            this.Value = Value;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = _GetChilds?.GetHashCode() ?? 0;
                hash = (hash * 397) ^ (_GetWeight?.GetHashCode() ?? 0);
                hash = (hash * 397) ^ EqualityComparer<TValue>.Default.GetHashCode(Value);
                return hash;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [DST]
        public static bool operator ==([CanBeNull] LambdaGraphNode<TValue, TWeight> left, [CanBeNull] LambdaGraphNode<TValue, TWeight> right) => Equals(left, right);

        [DST]
        public static bool operator !=([CanBeNull] LambdaGraphNode<TValue, TWeight> left, [CanBeNull] LambdaGraphNode<TValue, TWeight> right) => !Equals(left, right);

        public override bool Equals(object obj) => obj != null &&
                                                   (ReferenceEquals(this, obj) ||
                                                    obj.GetType() == GetType() &&
                                                    Equals((LambdaGraphNode<TValue, TWeight>)obj));

        public bool Equals(LambdaGraphNode<TValue, TWeight> other) => other != null
            && (ReferenceEquals(this, other) 
            || Equals(_GetChilds, other._GetChilds) 
            && Equals(_GetWeight, other._GetWeight) 
            && EqualityComparer<TValue>.Default.Equals(Value, other.Value));

        [DST]
        public IEnumerator<IGraphNode<TValue, TWeight>> GetEnumerator() => Links.Select(link => link.Node).GetEnumerator();

        [DST] [NotNull] public override string ToString() => $"λ[{(Value is null ? string.Empty : Value.ToString())}]";
    }

    public class LambdaGraphNode<V> : IGraphNode<V>, IEquatable<LambdaGraphNode<V>>
    {
        private readonly Func<V, IEnumerable<V>> _GetChilds;
        private readonly bool _Buffered;
        private IEnumerable<IGraphNode<V>> _Childs;

        /// <summary>Связи узла</summary>
        [NotNull]
        public IEnumerable<IGraphNode<V>> Childs => _Buffered
            ? _Childs ??= (_GetChilds(Value) ?? Enumerable.Empty<V>())
               .Select(value => new LambdaGraphNode<V>(value, _GetChilds, _Buffered))
               .Cast<IGraphNode<V>>()
               .ToArray()
            : (_GetChilds(Value) ?? Enumerable.Empty<V>()).Select(value => new LambdaGraphNode<V>(value, _GetChilds, _Buffered));

        /// <summary>Значение узла</summary>
        public V Value { get; set; }

        [NotNull] public LambdaGraphNode<V>[] ChildsArray => Childs.Cast<LambdaGraphNode<V>>().ToArray();

        public LambdaGraphNode(V Value, Func<V, IEnumerable<V>> GetChilds, bool Buffered = false)
        {
            _GetChilds = GetChilds;
            _Buffered = Buffered;
            this.Value = Value;
        }

        /// <inheritdoc />
        [DST]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        [DST]
        public IEnumerator<IGraphNode<V>> GetEnumerator() => Childs.GetEnumerator();

        /// <inheritdoc />
        [DST]
        [NotNull]
        public override string ToString() => $"λ:{Value}";

        /// <inheritdoc />
        public bool Equals(LambdaGraphNode<V> other) => other != null
                                                        && (ReferenceEquals(this, other) 
                                                            || EqualityComparer<V>.Default.Equals(Value, other.Value));

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as LambdaGraphNode<V>);

        /// <inheritdoc />
        public override int GetHashCode() => EqualityComparer<V>.Default.GetHashCode(Value);
    }
}