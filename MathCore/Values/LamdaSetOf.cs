using System.Collections.Generic;
using System.Linq;

namespace MathCore.Values
{
    public class LamdaSetOf<T> : AbstractSetOf<T>
    {
        private readonly IEnumerable<T> _Enimerable;

        public override int Power => _Enimerable.Count();

        public LamdaSetOf(IEnumerable<T> enimerable) => _Enimerable = enimerable;

        public override IEnumerator<T> GetEnumerator() => _Enimerable.GetEnumerator();
    }
}