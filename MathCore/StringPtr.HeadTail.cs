namespace MathCore;

public readonly ref partial struct StringPtr
{
    /// <summary>Разделённые фрагменты строки</summary>
    /// <param name="Head">Начало строки</param>
    /// <param name="Tail">Хвост строки</param>
    public readonly ref struct HeadTail(StringPtr Head, StringPtr Tail)
    {
        /// <summary>Начало строки</summary>
        public StringPtr Head { get; } = Head;

        /// <summary>Хвост строки</summary>
        public StringPtr Tail { get; } = Tail;

        public void Deconstruct(out StringPtr head, out StringPtr tail)
        {
            head = Head;
            tail = Tail;
        }
    }

    /// <summary>Разделённые фрагменты строки на название, значение и хвост</summary>
    /// <param name="Name">Название</param>
    /// <param name="Value">Значение</param>
    /// <param name="Tail">ОСтавшаяся часть строки</param>
    public readonly ref struct NameValueTail(StringPtr Name, StringPtr Value, StringPtr Tail)
    {
        public NameValueTail(HeadTail value) : this(value.Head, value.Tail, value.Tail.Substring(value.Tail.Length, 0)) { }

        /// <summary>Название</summary>
        public StringPtr Name { get; } = Name;

        /// <summary>Значение</summary>
        public StringPtr Value { get; } = Value;

        /// <summary>Хвост строки</summary>
        public StringPtr Tail { get; } = Tail;

        public void Deconstruct(out HeadTail value, out StringPtr tail)
        {
            value = new(Name, Value);
            tail = Tail;
        }

        public void Deconstruct(out StringPtr name, out StringPtr value, out StringPtr tail)
        {
            name = Name;
            value = Value;
            tail = Tail;
        }
    }

    /// <summary>Разделить строку на начало и хвост по указанному символу-разделителю</summary>
    /// <param name="Separator">Символ-разделитель</param>
    /// <returns>Разделённые фрагменты строки</returns>
    public HeadTail GetHeadTail(char Separator = ':')
    {
        var str = Trim();
        if (str.IsEmpty) return new(str, str);

        var name = str.SubstringBefore(Separator);

        if (name.Length == str.Length)
            return new(name, str.Substring(str.Length, 0));

        var value = str.Substring(name.Length + 1);

        return new(name.TrimEnd(), value.TrimStart());
    }

    /// <summary>Разделить строку на имя параметра, значение и оставшуюся часть</summary>
    /// <param name="ValueSeparator">Разделитель имени и значения параметра</param>
    /// <param name="TailSeparator">Разделитель групп параметров</param>
    /// <returns>Разделённая строка</returns>
    public NameValueTail GetNameValueTail(char ValueSeparator = ':', char TailSeparator = ' ')
    {
        var str = Trim();
        if (str.IsEmpty) return new(str, str, str);

        var name = str.SubstringBefore(ValueSeparator);

        if (name.Length == str.Length)
            return new(name, str.Substring(str.Length, 0), str.Substring(str.Length, 0));

        var value_tail = str.Substring(name.Length + 1).TrimStart(TailSeparator);

        var value = value_tail.SubstringBefore(TailSeparator);

        if (value.Length == value_tail.Length)
            return new(name.TrimEnd(), value, value_tail.Substring(value_tail.Length, 0).TrimStart());

        var tail = value_tail.Substring(value.Length + 1);

        return new(name.TrimEnd(), value.Trim(), tail.TrimStart());
    }
}
