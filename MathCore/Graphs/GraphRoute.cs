using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MathCore.Graphs
{
    public class GraphRoute<TValue, TWeight> : IEnumerable<IGraphNode<TValue, TWeight>>
    {
        private readonly IGraphNode<TValue, TWeight>[] _Nodes;

        public int Length => _Nodes.Length;

        public IGraphNode<TValue, TWeight> First
        {
            get
            {
                if(_Nodes.Length == 0)
                    throw new Exception("ѕопытка доступа к первому узлу пустого пути");
                return _Nodes[0];
            }
        }

        public IGraphNode<TValue, TWeight> Last
        {
            get
            {
                if(_Nodes.Length == 0)
                    throw new Exception("ѕопытка доступа к последнему узлу пустого пути");
                return _Nodes[_Nodes.Length - 1];
            }
        }

        public bool IsEmpty => _Nodes.Length == 0;

        public IGraphNode<TValue, TWeight> this[int i] => _Nodes[i];

        public GraphRoute(IGraphNode<TValue, TWeight>[] Nodes) => _Nodes = Nodes;

        public GraphRoute(IEnumerable<IGraphNode<TValue, TWeight>> Collection) => _Nodes = Collection.ToArray();

        public TWeight GetWeight(Func<TWeight, TWeight, TWeight> Aggregator) => this.SelectWithLastValue((last, next) => last.Links.First(l => l.Node.Equals(next)).Weight).Aggregate(Aggregator);

        public IEnumerator<IGraphNode<TValue, TWeight>> GetEnumerator() => _Nodes.Cast<IGraphNode<TValue, TWeight>>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _Nodes.GetEnumerator();


        public override string ToString() => IsEmpty ? "Empty rouhe" : $"{First} -[{Length}]-> {Last}";
    }
}