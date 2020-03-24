using System;
using System.Collections.Generic;

namespace MathCore.CSV
{
    public readonly struct CSVQueryRow
    {
        private readonly string[] _Items;
        private readonly IReadOnlyDictionary<string, int> _Header;

        public int ItemsCount => _Items.Length;

        public int Index { get; }

        public ref string this[int ValueIndex] => ref _Items[ValueIndex];

        public ref string this[string ValueName] => ref _Items[_Header[ValueName]];

        public CSVQueryRow(int Index, string[] Items, IReadOnlyDictionary<string, int> Header)
        {
            this.Index = Index;
            _Items = Items;
            _Header = Header;
        }

        public T ValueAs<T>(int ValueIndex) => (T)Convert.ChangeType(this[ValueIndex], typeof(T));

        public T ValueAs<T>(string ValueName) => ValueAs<T>(_Header[ValueName]);
    }
}