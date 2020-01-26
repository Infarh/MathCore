using System;
using System.Collections.Generic;
using MathCore.Annotations;
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedType.Global

namespace MathCore
{
    public class LamdaComparer<T> : EqualityComparer<T>
    {
        [NotNull] private readonly Func<T, T, bool> _Comparer;

        [NotNull] private readonly Func<T, int> _Hash;

        public LamdaComparer([NotNull] Func<T, T, bool> comparer, [NotNull] Func<T, int> hash)
        {
            _Comparer = comparer;
            _Hash = hash;
        }

        /// <inheritdoc />
        public override bool Equals(T x, T y) => _Comparer(x, y);

        /// <inheritdoc />
        public override int GetHashCode(T obj) => _Hash(obj);
    }
}