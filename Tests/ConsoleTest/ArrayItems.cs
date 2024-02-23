namespace ConsoleTest;

public readonly ref struct ArrayItems<T>(T[] Items)
{
    public T[] Items { get; init; } = Items;

    public Enumerator GetEnumerator() => new(Items);

    public ref struct Enumerator(T[] Items)
    {
        private int _Index = -1;
        private Item _Current = default;

        public Item Current => _Current;

        public bool MoveNext()
        {
            if (_Index == Items.Length)
                return false;

            _Index++;
            _Current = _Index == Items.Length ? default : new(Items, _Index);

            return true;
        }
    }

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    public readonly ref struct Item(T[] Items, int index)
    {
        public int Index => index;

        public ref T Value => ref Items[index];
    }
}