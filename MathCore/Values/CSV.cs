using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathCore.Values
{
    /// <summary>Файл текстовых данных, разделённых запятой</summary>
    public class CSV : IEnumerable<CSV.Item>
    {
        /// <summary>Элемент данных</summary>
        public class Item : IEnumerable<KeyValuePair<string, string>>
        {
            /// <summary>Элементы заголовка</summary>
            private readonly ReadOnlyCollection<string> _Header;
            /// <summary>Элементы данных</summary>
            private readonly string[] _Items;

            /// <summary>Требуемый элемент данных по указанному индексу</summary>
            /// <param name="index">Индекс элемента данных</param>
            /// <returns>Элемент данных по указанному индексу</returns>
            public string this[int index] => _Items[index];
            public ReadOnlyCollection<string> Header => _Header;
            /// <summary>Требуемый элемент данных по указнному имени заголовка столбца</summary>
            /// <param name="key">Имя столбца зоголовка</param>
            /// <returns>Требуемый элемент данных</returns>
            public string this[string key] => _Items[_Header.IndexOf(key)];

            /// <summary>Количество элементов данных</summary>
            public int ItemsCount => _Items.Length;

            /// <summary>Новый элемент данных</summary>
            /// <param name="Header">Названия столбцов</param>
            /// <param name="Items">Элементы данных</param>
            public Item(ReadOnlyCollection<string> Header, string[] Items)
            {
                _Header = Header;
                _Items = Items;
            }

            /// <inheritdoc />
            public override string ToString() => string.Join(" ", _Items.Select(i => i.Trim()));

            /// <inheritdoc />
            public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            {
                for (int i = 0, len = Math.Min(_Header.Count, _Items.Length); i < len; i++)
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
        /// <param name="Encoding">Кодировка файла (если не указана, используется <see cref="Encoding.UTF8S"/>)</param>
        public CSV(string FileName, char Separator = ';', int SkipFirstLines = 0, bool HeaderLine = true, bool SkipEmptyLines = true, Encoding Encoding = null)
        {
            _FileName = FileName;
            _Separator = Separator;
            _SkipFirstLines = SkipFirstLines;
            _HeaderLine = HeaderLine;
            _SkipEmptyLines = SkipEmptyLines;
            _Encoding = Encoding ?? Encoding.UTF8;
        }

        /// <inheritdoc />
        public IEnumerator<Item> GetEnumerator()
        {
            var separator = _Separator;
            using (var reader = new StreamReader(new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.Read), _Encoding))
            {
                for (var skip = _SkipFirstLines; skip > 0 && !reader.EndOfStream; skip--) reader.ReadLine();

                ReadOnlyCollection<string> header = null;
                if (_HeaderLine && !reader.EndOfStream)
                {
                    var header_line = reader.ReadLine();
                    if (string.IsNullOrEmpty(header_line))
                        throw new FormatException("Ошибка формата файла - отсутствует требуемая строка заголовка");
                    header = new List<string>(header_line?.Split(separator) ?? throw new InvalidOperationException()).AsReadOnly();
                }

                while (!reader.EndOfStream)
                {
                    var item_line = reader.ReadLine()?.Split(separator);
                    if (item_line is null) continue;
                    if (_SkipEmptyLines && item_line.All(string.IsNullOrWhiteSpace)) continue;
                    yield return new Item(header, item_line);
                }
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
