using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.CSV
{
    public readonly struct CSVQuery : IEnumerable<CSVQueryRow>
    {
        private readonly FileInfo _File;
        private readonly int _SkipRows;
        private readonly bool _ContainsHeader;
        private readonly int _SkipRowsAfterHeader;
        private readonly char _ValuesSeparator;
        private readonly int _TakeRows;

        public CSVQuery([NotNull] FileInfo File) : this(File, 0, false, 0, ';', -1) { }

        private CSVQuery(FileInfo File, int SkipRows, bool ContainsHeader, int SkipRowsAfterHeader, char ValuesSeparator, int TakeRows)
        {
            _File = File ?? throw new ArgumentNullException(nameof(File));
            _SkipRows = SkipRows;
            _ContainsHeader = ContainsHeader;
            _SkipRowsAfterHeader = SkipRowsAfterHeader;
            _ValuesSeparator = ValuesSeparator;
            _TakeRows = TakeRows;
        }

        public CSVQuery Skip(int RowsCount) => new CSVQuery(
            _File, 
            RowsCount, 
            _ContainsHeader, 
            _SkipRowsAfterHeader,
            _ValuesSeparator,
            _TakeRows);

        public CSVQuery SkipAfterHeader(int RowsCount) => new CSVQuery(
            _File, 
            _SkipRows, 
            _ContainsHeader, 
            RowsCount,
            _ValuesSeparator,
            _TakeRows);

        public CSVQuery ReadHeader(bool IsExist = true) => new CSVQuery(
            _File, 
            _SkipRows, 
            IsExist,
            _SkipRowsAfterHeader,
            _ValuesSeparator,
            _TakeRows);

        public CSVQuery ValuesSeparator(char Separator) => new CSVQuery(
            _File,
            _SkipRows,
            _ContainsHeader,
            _SkipRowsAfterHeader,
            Separator,
            _TakeRows);

        public CSVQuery TakeRows(int RowsCount) => new CSVQuery(
            _File,
            _SkipRows,
            _ContainsHeader,
            _SkipRowsAfterHeader,
            _ValuesSeparator,
            RowsCount);

        public IEnumerator<CSVQueryRow> GetEnumerator()
        {
            using var reader = _File.OpenText();

            for (var i = 0; i < _SkipRows && !reader.EndOfStream; i++)
                reader.ReadLine();

            if(reader.EndOfStream) yield break;

            char[] separator = { _ValuesSeparator };

            SortedList<string, int> header = null;
            if (_ContainsHeader)
            {
                var header_line = reader.ReadLine();
                if(!string.IsNullOrWhiteSpace(header_line))
                {
                    var headers = header_line.Split(separator);
                    header = new SortedList<string, int>();
                    for (var i = 0; i < headers.Length; i++)
                        header[headers[i]] = i;
                }
            }

            for (var i = 0; i < _SkipRowsAfterHeader && !reader.EndOfStream; i++)
                reader.ReadLine();


            var index = 0;
            while (!reader.EndOfStream && index != _TakeRows)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var items = line.Split(separator);
                yield return new CSVQueryRow(index, items, header);
                index++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}