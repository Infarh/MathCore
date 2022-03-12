namespace ConsoleTest;

public readonly ref struct ArrayItems<T>
{
    public T[] Items { get; init; }

    public ArrayItems(T[] Items) => this.Items = Items;

    public Enumerator GetEnumerator() => new(Items);

    public ref struct Enumerator
    {
        private int _Index;
        private readonly T[] _Items;
        private Item _Current;

        public Item Current => _Current;

        public Enumerator(T[] Items)
        {
            _Index = -1;
            _Items = Items;
            _Current = default;
        }

        public bool MoveNext()
        {
            if (_Index == _Items.Length)
                return false;

            _Index++;
            _Current = _Index == _Items.Length ? default : new(_Items, _Index);

            return true;
        }
    }

    public readonly ref struct Item
    {
        private readonly int _Index;
        private readonly T[] _Items;

        public int Index => _Index;

        public ref T Value => ref _Items[Index];

        public Item(T[] Items, int Index)
        {
            _Items = Items;
            _Index = Index;
        }
    }
}