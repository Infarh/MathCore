#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.Values;

/// <summary>Файл текстовых данных, разделённых запятой</summary>
public class CSV : IEnumerable<CSV.Item>
{
    /// <summary>Элемент данных</summary>
    public class Item : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>Начальное положение</summary>
        public long FileStartPos { get; }

        /// <summary>Конечное положение в файле</summary>
        public long FileEndPos { get; }

        /// <summary>Элементы заголовка</summary>
        private readonly string[] _Header;
        /// <summary>Элементы данных</summary>
        private readonly string[] _Items;

        /// <summary>Требуемый элемент данных по указанному индексу</summary>
        /// <param name="index">Индекс элемента данных</param>
        /// <returns>Элемент данных по указанному индексу</returns>
        public ref readonly string this[int index] => ref _Items[index];

        public IReadOnlyCollection<string> Header => _Header;

        /// <summary>Требуемый элемент данных по указанному имени заголовка столбца</summary>
        /// <param name="key">Имя столбца заголовка</param>
        /// <returns>Требуемый элемент данных</returns>
        public ref readonly string this[string key] => ref _Items[Array.IndexOf(_Header, key)];

        /// <summary>Количество элементов данных</summary>
        public int ItemsCount => _Items.Length;

        /// <summary>Новый элемент данных</summary>
        /// <param name="Header">Названия столбцов</param>
        /// <param name="Items">Элементы данных</param>
        /// <param name="FileStartPos">Начальное положение</param>
        /// <param name="FileEndPos">Конечное положение в файле</param>
        public Item(string[] Header, string[] Items, long FileStartPos, long FileEndPos)
        {
            this.FileStartPos = FileStartPos;
            this.FileEndPos   = FileEndPos;
            _Header           = Header;
            _Items            = Items;
        }

        /// <inheritdoc />
        public override string ToString() => string.Join(" ", _Items.Select(i => i.Trim()));

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            for (int i = 0, len = Math.Min(_Header.Length, _Items.Length); i < len; i++)
                yield return new KeyValuePair<string, string>(_Header[i], _Items[i]);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>Имя файла</summary>
    private readonly string _FileName;
    /// <summary>Символ-разделитель</summary>
    private readonly char _Separator;
    /// <summary>Количество пропускаемых первых строк файла</summary>
    private readonly int _SkipFirstLines;
    /// <summary>Считывать строку заголовков</summary>
    private readonly bool _HeaderLine;
    /// <summary>Пропускать пустые строки</summary>
    private readonly bool _SkipEmptyLines;
    /// <summary>Кодировка файла</summary>
    private readonly Encoding _Encoding;

    /// <summary>Новый файл данных, разделённых запятой</summary>
    /// <param name="FileName">Имя файла данных</param>
    /// <param name="Separator">Символ-разделитель</param>
    /// <param name="SkipFirstLines">Количество пропускаемых строк в начале файла</param>
    /// <param name="HeaderLine">Считывать ли заголовок</param>
    /// <param name="SkipEmptyLines">Пропускать пустые строки</param>
    /// <param name="Encoding">Кодировка файла (если не указана, используется <see cref="Encoding.UTF8"/>)</param>
    public CSV(string FileName, char Separator = ';', int SkipFirstLines = 0, bool HeaderLine = true, bool SkipEmptyLines = true, Encoding? Encoding = null)
    {
        _FileName       = FileName;
        _Separator      = Separator;
        _SkipFirstLines = SkipFirstLines;
        _HeaderLine     = HeaderLine;
        _SkipEmptyLines = SkipEmptyLines;
        _Encoding       = Encoding ?? Encoding.UTF8;
    }

    /// <inheritdoc />
    public IEnumerator<Item> GetEnumerator()
    {
        var       separator = _Separator;
        using var reader    = new StreamReader(new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.Read), _Encoding);
        for (var skip = _SkipFirstLines; skip > 0 && !reader.EndOfStream; skip--) 
            reader.ReadLine();

        string[]? header = null;
        if (_HeaderLine && !reader.EndOfStream)
            if (reader.ReadLine() is not { Length: > 0 } header_line)
                throw new FormatException("Ошибка формата файла - отсутствует требуемая строка заголовка");
            else
                header = header_line.Split(separator);

        while (!reader.EndOfStream)
        {
            var file_pos  = reader.BaseStream.Position;
            if (reader.ReadLine()?.Split(separator) is not { } item_line) continue;
            if (_SkipEmptyLines && item_line.All(s => s is { Length: > 0 })) continue;
            yield return new Item(header, item_line, file_pos, reader.BaseStream.Position);
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}