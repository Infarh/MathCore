using System;

namespace MathCore.Graphs
{
    public class LambdaGraphLink<TValue, TWeight> : IGraphLink<TValue, TWeight>
    {
        private readonly IGraphNode<TValue, TWeight> _From;
        private readonly Func<TValue, TValue, TWeight> _GetWeight;
        private readonly bool _Buffered;
        private object _Weight;
        public IGraphNode<TValue, TWeight> Node { get; }

        public TWeight Weight => _Buffered
            ? (TWeight) (_Weight ??= _GetWeight(_From.Value, Node.Value))
            : _GetWeight(_From.Value, Node.Value);

        public LambdaGraphLink(IGraphNode<TValue, TWeight> From, IGraphNode<TValue, TWeight> To, Func<TValue, TValue, TWeight> GetWeight, bool Buffered = false)
        {
            _From = From;
            _GetWeight = GetWeight;
            _Buffered = Buffered;
            Node = To;
        }
    }
}