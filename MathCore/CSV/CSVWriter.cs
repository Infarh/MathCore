using System;
using System.Collections.Generic;
using MathCore.Annotations;

namespace MathCore.CSV
{
    public struct CSVWriter<T>
    {
        private readonly IEnumerable<T> _Items;

        public CSVWriter([NotNull] IEnumerable<T> items) => _Items = items ?? throw new ArgumentNullException(nameof(items));
    }
}