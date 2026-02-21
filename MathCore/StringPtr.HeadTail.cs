namespace MathCore;

public readonly ref partial struct StringPtr
{
    /// <summary>Разделённые фрагменты строки</summary>
    /// <param name="Head">Начало строки</param>
    /// <param name="Tail">Хвост строки</param>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("name:value");
    /// var parts = ptr.GetHeadTail(':');
    /// var head = parts.Head;
    /// var tail = parts.Tail;
    /// ]]>
    /// </example>
    public readonly ref struct HeadTail(StringPtr Head, StringPtr Tail)
    {
        /// <summary>Начало строки</summary>
        public StringPtr Head { get; } = Head;

        /// <summary>Хвост строки</summary>
        public StringPtr Tail { get; } = Tail;

        /// <summary>Деконструкция результата на начало и хвост</summary>
        /// <param name="head">Начало строки</param>
        /// <param name="tail">Хвост строки</param>
        /// <example>
        /// <![CDATA[
        /// var (head, tail) = new StringPtr("a:b").GetHeadTail(':');
        /// ]]>
        /// </example>
        public void Deconstruct(out StringPtr head, out StringPtr tail)
        {
            head = Head;
            tail = Tail;
        }
    }

    /// <summary>Разделённые фрагменты строки на название, значение и хвост</summary>
    /// <param name="Name">Название</param>
    /// <param name="Value">Значение</param>
    /// <param name="Tail">Оставшаяся часть строки</param>
    /// <example>
    /// <![CDATA[
    /// var ptr = new StringPtr("key:value tail");
    /// var parts = ptr.GetNameValueTail(':', ' ');
    /// var name = parts.Name;
    /// var value = parts.Value;
    /// var tail = parts.Tail;
    /// ]]>
    /// </example>
    public readonly ref struct NameValueTail(StringPtr Name, StringPtr Value, StringPtr Tail)
    {
        /// <summary>Создать из пары начало-хвост</summary>
        /// <param name="value">Пара начало-хвост</param>
        /// <example>
        /// <![CDATA[
        /// var head_tail = new StringPtr("a:b").GetHeadTail(':');
        /// var named = new StringPtr.NameValueTail(head_tail);
        /// ]]>
        /// </example>
        public NameValueTail(HeadTail value) : this(value.Head, value.Tail, value.Tail.Substring(value.Tail.Length, 0)) { }

        /// <summary>Название</summary>
        public StringPtr Name { get; } = Name;

        /// <summary>Значение</summary>
        public StringPtr Value { get; } = Value;

        /// <summary>Хвост строки</summary>
        public StringPtr Tail { get; } = Tail;

        /// <summary>Деконструкция результата на пару и хвост</summary>
        /// <param name="value">Пара начало-значение</param>
        /// <param name="tail">Хвост строки</param>
        /// <example>
        /// <![CDATA[
        /// var (value, tail) = new StringPtr("a:b c").GetNameValueTail(':', ' ');
        /// ]]>
        /// </example>
        public void Deconstruct(out HeadTail value, out StringPtr tail)
        {
            value = new(Name, Value);
            tail = Tail;
        }

        /// <summary>Деконструкция результата на имя, значение и хвост</summary>
        /// <param name="name">Название</param>
        /// <param name="value">Значение</param>
        /// <param name="tail">Хвост строки</param>
        /// <example>
        /// <![CDATA[
        /// var (name, value, tail) = new StringPtr("a:b c").GetNameValueTail(':', ' ');
        /// ]]>
        /// </example>
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
    /// <example>
    /// <![CDATA[
    /// var parts = new StringPtr("a:b").GetHeadTail(':');
    /// ]]>
    /// </example>
    public HeadTail GetHeadTail(char Separator)
    {
        var index = IndexOf(Separator);
        if (index < 0)
            return new(this, new(Source, Pos + Length, 0));

        var head = Substring(0, index);
        var tail = Substring(index + 1);
        return new(head, tail);
    }

    /// <summary>Разделить строку на название, значение и хвост</summary>
    /// <param name="NameValueSeparator">Символ-разделитель имени и значения</param>
    /// <param name="TailSeparator">Символ-разделитель хвоста</param>
    /// <returns>Разделённые фрагменты строки</returns>
    /// <example>
    /// <![CDATA[
    /// var parts = new StringPtr("key:value tail").GetNameValueTail(':', ' ');
    /// ]]>
    /// </example>
    public NameValueTail GetNameValueTail(char NameValueSeparator, char TailSeparator)
    {
        var name_index = IndexOf(NameValueSeparator);
        if (name_index < 0)
            return new(new(this, new(Source, Pos + Length, 0)));

        var name = Substring(0, name_index);
        var value_tail = Substring(name_index + 1);
        var tail_index = value_tail.IndexOf(TailSeparator);
        if (tail_index < 0)
            return new(name, value_tail, new(Source, Pos + Length, 0));

        var value = value_tail.Substring(0, tail_index);
        var tail = value_tail.Substring(tail_index + 1);
        return new(name, value, tail);
    }
}
