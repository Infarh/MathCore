using System.Runtime.InteropServices;

namespace MathCore;

public readonly ref partial struct StringPtr
{
    /// <summary>Разделитель строки на фрагменты по указанным символам</summary>
    /// <param name="Buffer">Исходный строковый буфер</param>
    /// <param name="Separators">Символы-разделители фрагментов строки</param>
    /// <param name="StartIndex">Индекс начала анализируемой подстроки</param>
    /// <param name="Length">Длина анализируемой подстроки</param>
    /// <param name="SkipEmpty">Пропускать пустые строковые фрагменты</param>
    /// <example>
    /// <![CDATA[
    /// var tokenizer = new StringPtr("a,b").Split(',');
    /// foreach (var part in tokenizer)
    /// {
    ///     Console.WriteLine(part);
    /// }
    /// ]]>
    /// </example>
    [StructLayout(LayoutKind.Auto)]
    public readonly ref partial struct Tokenizer(string Buffer, char[] Separators, int StartIndex, int Length, bool SkipEmpty = false)
    {
        /// <summary>Строковый буфер</summary>
        private readonly string _Buffer = Buffer;

        /// <summary>Длина подстроки для анализа</summary>
        private readonly int _Length = Length;

        /// <summary>Пропускать пустые фрагменты</summary>
        private readonly bool _SkipEmpty = SkipEmpty;

        /// <summary>Инициализация нового разделителя строки</summary>
        /// <param name="Str">Исходный фрагмент строки</param>
        /// <param name="Separators">Символы-разделители фрагментов строки</param>
        /// <example>
        /// <![CDATA[
        /// var tokenizer = new StringPtr("a,b").Split(',');
        /// ]]>
        /// </example>
        public Tokenizer(StringPtr Str, char[] Separators) : this(Str.Source, Separators, Str.Pos, Str.Length) { }

        /// <summary>Инициализация нового разделителя строки</summary>
        /// <param name="Buffer">Исходный строковый буфер</param>
        /// <param name="Separators">Символы-разделители фрагментов строки</param>
        /// <example>
        /// <![CDATA[
        /// var tokenizer = new StringPtr.Tokenizer("a,b", [',']);
        /// ]]>
        /// </example>
        public Tokenizer(string Buffer, char[] Separators) : this(Buffer, Separators, 0, Buffer.Length) { }

        /// <summary>Пропускать пустые строковые фрагменты</summary>
        /// <param name="Skip">Пропускать, или нет</param>
        /// <returns>Разделитель строк с заданным режимом пропуска</returns>
        /// <example>
        /// <![CDATA[
        /// var tokenizer = new StringPtr("a,,b").Split(',').SkipEmpty(true);
        /// ]]>
        /// </example>
        public Tokenizer SkipEmpty(bool Skip) => new(_Buffer, Separators, StartIndex, _Length, Skip);

        /// <summary>Преобразовать последовательность фрагментов в массив строк</summary>
        /// <returns>Массив строковых фрагментов</returns>
        /// <example>
        /// <![CDATA[
        /// var array = new StringPtr("a,b").Split(',').ToArray();
        /// ]]>
        /// </example>
        public string[] ToArray() => [.. ToList()];

        /// <summary>Преобразовать последовательность фрагментов в список строк</summary>
        /// <returns>Список строковых фрагментов</returns>
        /// <example>
        /// <![CDATA[
        /// var list = new StringPtr("a,b").Split(',').ToList();
        /// ]]>
        /// </example>
        public List<string> ToList()
        {
            var result = new List<string>();

            for (var enumerator = GetEnumerator(); enumerator.MoveNext();)
                result.Add(enumerator.Current);

            result.TrimExcess();

            return result;
        }
    }
}