using System.Collections.Generic;

namespace MathCore.Trees
{
    public interface ITree<TValue> : IIndexable<ITree<TValue>>, IEnumerable<ITree<TValue>>
    {
        int Depth { get; }

        TValue Value { get; set; }
    }
}