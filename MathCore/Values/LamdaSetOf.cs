using System;
using System.Collections.Generic;
using System.Linq;

namespace MathCore.Values
{
    public class LamdaSetOf<T> : AbstractSetOf<T>
    {
        private readonly IEnumerable<T> _Enimerable;

        public override int Power => _Enimerable.Count();

        public override bool Add(T Value)
        {
            var collection = _Enimerable as ICollection<T> ?? throw new NotSupportedException("Добавление элементов не поддерживается");
            if (collection.Contains(Value)) return false;
            collection.Add(Value);
            return true;
        }

        public LamdaSetOf(IEnumerable<T> enimerable) => _Enimerable = enimerable;

        public override IEnumerator<T> GetEnumerator() => _Enimerable.GetEnumerator();
    }
}