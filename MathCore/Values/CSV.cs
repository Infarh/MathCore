#nullable enable
using System.Collections;
using System.Text;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.Values;

/// <summary>Файл текстовых данных, разделённых запятой</summary>
/// <remarks>Новый файл данных, разделённых запятой</remarks>
/// <param name="FileName">Имя файла данных</param>
/// <param name="Separator">Символ-разделитель</param>
/// <param name="SkipFirstLines">Количество пропускаемых строк в начале файла</param>
/// <param name="HeaderLine">Считывать ли заголовок</param>
/// <param name="SkipEmptyLines">Пропускать пустые строки</param>
/// <param name="Encoding">Кодировка файла (если не указана, используется <see cref="Encoding.UTF8"/>)</param>
public class CSV(
    string FileName,
    char Separator = ';',
    int SkipFirstLines = 0,
    bool HeaderLine = true,
    bool SkipEmptyLines = true,
    Encoding? Encoding = null) 
    : IEnumerable<CSV.Item>
{
    /// <summary>Элемент данных</summary>
    /// <remarks>Новый элемент данных</remarks>
    /// <param name="Header">Названия столбцов</param>
    /// <param name="Items">Элементы данных</param>
    /// <param name="FileStartPos">Начальное положение</param>
    /// <param name="FileEndPos">Конечное положение в файле</param>
    public class Item(string[] Header, string[] Items, long FileStartPos, long FileEndPos) : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>Начальное положение</summary>
        public long FileStartPos { get; } = FileStartPos;

        /// <summary>Конечное положение в файле</summary>
        public long FileEndPos { get; } = FileEndPos;

        /// <summary>Элементы заголовка</summary>
        private readonly string[] _Header = Header;

        /// <summary>Требуемый элемент данных по указанному индексу</summary>
        /// <param name="index">Индекс элемента данных</param>
        /// <returns>Элемент данных по указанному индексу</returns>
        public ref readonly string this[int index] => ref Items[index];

        public IReadOnlyCollection<string> Header => _Header;

        /// <summary>Требуемый элемент данных по указанному имени заголовка столбца</summary>
        /// <param name="key">Имя столбца заголовка</param>
        /// <returns>Требуемый элемент данных</returns>
        public ref readonly string this[string key] => ref Items[Array.IndexOf(_Header, key)];

        /// <summary>Количество элементов данных</summary>
        public int ItemsCount => Items.Length;

        /// <inheritdoc />
        public override string ToString() => string.Join(" ", Items.Select(i => i.Trim()));

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            for (int i = 0, len = Math.Min(_Header.Length, Items.Length); i < len; i++)
                yield return new(_Header[i], Items[i]);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>Кодировка файла</summary>
    private readonly Encoding _Encoding = Encoding ?? Encoding.UTF8;

    /// <inheritdoc />
    public IEnumerator<Item> GetEnumerator()
    {
        var       separator = Separator;
        using var reader    = new StreamReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read), _Encoding);
        for (var skip = SkipFirstLines; skip > 0 && !reader.EndOfStream; skip--) 
            reader.ReadLine();

        string[]? header = null;
        if (HeaderLine && !reader.EndOfStream)
            if (reader.ReadLine() is not { Length: > 0 } header_line)
                throw new FormatException("Ошибка формата файла - отсутствует требуемая строка заголовка");
            else
                header = header_line.Split(separator);

        while (!reader.EndOfStream)
        {
            var file_pos  = reader.BaseStream.Position;
            if (reader.ReadLine()?.Split(separator) is not { } item_line) continue;
            if (SkipEmptyLines && item_line.All(s => s is { Length: > 0 })) continue;
            yield return new(header, item_line, file_pos, reader.BaseStream.Position);
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}