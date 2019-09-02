using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;

namespace System
{
    public static class BoolExtensions
    {
        public static bool And([NotNull] this IEnumerable<bool> items) => items.Aggregate(true, (r, v) => r && v);
        public static bool Or([NotNull] this IEnumerable<bool> items) => items.Aggregate(false, (r, v) => r || v);
    }
}