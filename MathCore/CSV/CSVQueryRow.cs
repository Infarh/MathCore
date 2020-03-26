using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MathCore.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.CSV
{
    public readonly struct CSVQueryRow
    {
        private readonly string[] _Items;
        private readonly IDictionary<string, int> _Header;

        public int ItemsCount => _Items.Length;

        public int Index { get; }

        public IReadOnlyDictionary<string, int> Headers => new ReadOnlyDictionary<string, int>(_Header);

        public ref string this[int ValueIndex] => ref _Items[ValueIndex];

        public ref string this[[NotNull] string ValueName] => ref _Items[_Header[ValueName]];

        public CSVQueryRow(int Index, string[] Items, IDictionary<string, int> Header)
        {
            this.Index = Index;
            _Items = Items;
            _Header = Header;
        }

        public T ValueAs<T>(int ValueIndex) => (T)Convert.ChangeType(this[ValueIndex], typeof(T));

        public T ValueAs<T>([NotNull] string ValueName) => ValueAs<T>(_Header[ValueName]);
    }
}