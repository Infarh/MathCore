using System;
using System.Collections.Generic;
using System.Linq;

namespace MathCore.Values
{
    public class LambdaSetOf<T> : AbstractSetOf<T>
    {
        private readonly IEnumerable<T> _Enumerable;

        public override int Power => _Enumerable.Count();

        public override bool Add(T Value)
        {
            var collection = _Enumerable as ICollection<T> ?? throw new NotSupportedException("Добавление элементов не поддерживается");
            if (collection.Contains(Value)) return false;
            collection.Add(Value);
            return true;
        }

        public LambdaSetOf(IEnumerable<T> enumerable) => _Enumerable = enumerable;

        public override IEnumerator<T> GetEnumerator() => _Enumerable.GetEnumerator();
    }
}