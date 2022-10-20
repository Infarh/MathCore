﻿#nullable enable
namespace MathCore;

public readonly ref partial struct StringPtr
{
    /// <summary>Разделитель строки на фрагменты по указанному символам-разделителям</summary>
    public readonly ref partial struct Tokenizer
    {
        /// <summary>Строковый буфер</summary>
        private readonly string _Buffer;

        /// <summary>Символы-разделители</summary>
        private readonly char[] _Separators;

        /// <summary>Начальное положение в буфере</summary>
        private readonly int _StartIndex;

        /// <summary>Длина подстроки для анализа</summary>
        private readonly int _Length;

        /// <summary>Пропускать пустые фрагменты</summary>
        private readonly bool _SkipEmpty;

        /// <summary>Инициализация нового разделителя строки</summary>
        /// <param name="Str">Исходный фрагмент строки</param>
        /// <param name="Separators">Символы-разделители фрагментов строки</param>
        public Tokenizer(StringPtr Str, char[] Separators) : this(Str.Source, Separators, Str.Pos, Str.Length) { }

        /// <summary>Инициализация нового разделителя строки</summary>
        /// <param name="Buffer">Исходный строковый буфер</param>
        /// <param name="Separators">Символы-разделители фрагментов строки</param>
        public Tokenizer(string Buffer, char[] Separators) : this(Buffer, Separators, 0, Buffer.Length) { }

        /// <summary>Инициализация нового разделителя строки</summary>
        /// <param name="Buffer">Исходный строковый буфер</param>
        /// <param name="Separators">Символы-разделители фрагментов строки</param>
        /// <param name="StartIndex">Индекс начала анализируемой подстроки</param>
        /// <param name="Length">Длина анализируемой подстроки</param>
        /// <param name="SkipEmpty">Пропускать пустые строковые фрагменты</param>
        public Tokenizer(string Buffer, char[] Separators, int StartIndex, int Length, bool SkipEmpty = false)
        {
            _Buffer     = Buffer;
            _Separators = Separators;
            _StartIndex = StartIndex;
            _Length     = Length;
            _SkipEmpty  = SkipEmpty;
        }

        /// <summary>Пропускать пустые строковые фрагменты</summary>
        /// <param name="Skip">Пропускать, или нет</param>
        /// <returns>Перечислитель строковых фрагментов с изменённым режимом пропуска строковых фрагментов</returns>
        public Tokenizer SkipEmpty(bool Skip) => new(_Buffer, _Separators, _StartIndex, _Length, Skip);

        /// <summary>Сформировать перечислитель строковых фрагментов</summary>
        /// <returns>Перечислитель строковых фрагментов</returns>
        public TokenEnumerator GetEnumerator() => new(_Buffer, _Separators, _StartIndex, _Length, _SkipEmpty);
    }
}