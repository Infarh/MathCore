using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathCore.Annotations;
using MathCore.Extensions.Expressions;

// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleNamedExpression

namespace MathCore.CSV
{
    public readonly struct CSVWriter<T>
    {
        private readonly IEnumerable<T> _Items;

        private readonly char _Separator;
        private readonly IDictionary<string, (Func<T, object> Selector, int Index)> _Selectors;
        private readonly bool _WriteHeaders;

        [NotNull]
        private static IEnumerable<KeyValuePair<string, (Func<T, object> Selector, int Index)>> GetOrdered(
            [CanBeNull] IEnumerable<KeyValuePair<string, (Func<T, object> Selector, int Index)>> Selectors) =>
            Selectors?.OrderBy(s => s.Value.Index).ThenBy(s => s.Key)
               ?? Enumerable.Empty<KeyValuePair<string, (Func<T, object> Selector, int Index)>>();

        [NotNull] public IEnumerable<string> Headers => GetOrdered(_Selectors).Select(s => s.Key);

        public CSVWriter([NotNull] IEnumerable<T> items)
            : this(
                items: items,
                Separator: ',',
                WriteHeaders: true,
                Selectors: null)
        { }

        private CSVWriter(
            [NotNull] IEnumerable<T> items,
            char Separator,
            bool WriteHeaders,
            [CanBeNull] IDictionary<string, (Func<T, object> Selector, int Index)> Selectors
            )
        {
            _Items = items ?? throw new ArgumentNullException(nameof(items));
            _Separator = Separator;
            _WriteHeaders = WriteHeaders;
            _Selectors = Selectors;
        }

        [NotNull]
        private static IDictionary<string, (Func<T, object> Selector, int Index)> CreateHeaders()
        {
            var type = typeof(T);
            var properties = type.GetProperties().Where(p => p.CanRead).Select((p, i) => (p, i));
            var selectors = new Dictionary<string, (Func<T, object> Selector, int Index)>();

            var item = Expression.Parameter(type, "item");
            foreach (var (property, index) in properties)
            {
                var selector_expr = property.PropertyType.IsValueType
                    ? item.GetProperty(property).ConvertTo<object>()
                    : (Expression)item.GetProperty(property);

                selectors[property.Name] = (selector_expr.CompileTo<Func<T, object>>(item), index);
            }

            return selectors;
        }

        public CSVWriter<T> Separator(char separator) =>
            new CSVWriter<T>(
                _Items,
                separator,
                _WriteHeaders,
                _Selectors
            );

        public CSVWriter<T> WriteHeader(bool write = true) =>
            new CSVWriter<T>(
                _Items,
                _Separator,
                write,
                _Selectors
            );

        public CSVWriter<T> AddDefaultHeaders() =>
            new CSVWriter<T>(
                _Items,
                _Separator,
                _WriteHeaders,
                MergeSelectors(_Selectors, CreateHeaders())
                );

        [CanBeNull]
        private static IDictionary<string, (Func<T, object> Selector, int Index)> MergeSelectors(
            [CanBeNull] IDictionary<string, (Func<T, object> Selector, int Index)> Source,
            [CanBeNull] IDictionary<string, (Func<T, object> Selector, int Index)> Destination = null)
        {
            if (Source is null)
                return Destination;

            var result = new Dictionary<string, (Func<T, object> Selector, int Index)>(Source);
            if (Destination is null) return result;

            foreach (var (header, selector) in Destination)
                result[header] = selector;

            return result;
        }

        [NotNull]
        private static IDictionary<string, (Func<T, object> Selector, int Index)> MergeSelectors(
            [CanBeNull] IDictionary<string, (Func<T, object> Selector, int Index)> Source,
            string Name, Func<T, object> Selector)
        {
            var result = Source is null
                ? new Dictionary<string, (Func<T, object> Selector, int Index)>()
                : new Dictionary<string, (Func<T, object> Selector, int Index)>(Source);

            var index = result.Count == 0 ? 0 : result.Max(s => s.Value.Index) + 1;
            result[Name] = (Selector, index);

            return result;
        }

        public CSVWriter<T> AddColumn(string Name, Func<T, object> Selector) =>
            new CSVWriter<T>(
                _Items,
                _Separator,
                _WriteHeaders,
                MergeSelectors(_Selectors, Name, Selector)
            );

        public CSVWriter<T> RemoveColumn(string Name)
        {
            var columns = _Selectors;
            if (columns?.ContainsKey(Name) == true)
            {
                columns = new Dictionary<string, (Func<T, object> Selector, int Index)>(columns);
                columns.Remove(Name);
            }
            return new CSVWriter<T>(
                _Items,
                _Separator,
                _WriteHeaders,
                columns
            );
        }

        public CSVWriter<T> RemoveColumn(int index)
        {
            var columns = _Selectors;
            if (columns != null)
            {
                var i = 0;
                foreach (var (header, _) in columns)
                {
                    if (i == index)
                    {
                        columns = new Dictionary<string, (Func<T, object> Selector, int Index)>(columns);
                        columns.Remove(header);
                        break;
                    }
                    i++;
                }
            }
            return new CSVWriter<T>(
                _Items,
                _Separator,
                _WriteHeaders,
                columns
            );
        }

        #region Write

        public void WriteTo([NotNull] string FileName, [CanBeNull] Encoding encoding = null)
        {
            using var file_stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
            WriteTo(file_stream, encoding);
        }

        public async Task WriteToAsync([NotNull] string FileName, [CanBeNull] Encoding encoding = null, CancellationToken Cancel = default)
        {
            using var file_stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
            await WriteToAsync(file_stream, encoding, Cancel).ConfigureAwait(false);
        }

        public void WriteTo([NotNull] FileInfo File) => WriteTo(File.CreateText());
        public async Task WriteToAsync([NotNull] FileInfo File, CancellationToken Cancel = default) =>
            await WriteToAsync(File.CreateText(), Cancel).ConfigureAwait(false);

        public void WriteTo([NotNull] Stream stream, [CanBeNull] Encoding encoding = null) =>
            WriteTo(new StreamWriter(stream, encoding ?? Encoding.UTF8, 1024, true));

        public async Task WriteToAsync([NotNull] Stream stream, [CanBeNull] Encoding encoding = null, CancellationToken Cancel = default) =>
            await WriteToAsync(new StreamWriter(stream, encoding ?? Encoding.UTF8, 1024, true), Cancel).ConfigureAwait(false);

        public void WriteTo([NotNull] TextWriter writer)
        {
            var selectors_key = GetOrdered(_Selectors ?? CreateHeaders()).ToArray();

            var values = selectors_key.Select(s => s.Key).ToArray();
            var selectors = selectors_key.Select(s => s.Value.Selector).ToArray();

            var separator = new string(_Separator, 1);
            writer.WriteLine(string.Join(separator, values));

            foreach (var item in _Items)
            {
                for (var i = 0; i < values.Length; i++) 
                    values[i] = Convert.ToString(selectors[i](item));
                writer.WriteLine(string.Join(separator, values));
            }
        }

        public async Task WriteToAsync([NotNull] TextWriter writer, CancellationToken Cancel = default)
        {
            Cancel.ThrowIfCancellationRequested();
            var selectors_key = GetOrdered(_Selectors ?? CreateHeaders()).ToArray();

            var values = selectors_key.Select(s => s.Key).ToArray();
            var selectors = selectors_key.Select(s => s.Value.Selector).ToArray();

            var separator = new string(_Separator, 1);
            Cancel.ThrowIfCancellationRequested();
            await writer.WriteLineAsync(string.Join(separator, values)).ConfigureAwait(false);

            Cancel.ThrowIfCancellationRequested();
            foreach (var item in _Items)
            {
                Cancel.ThrowIfCancellationRequested();
                for (var i = 0; i < values.Length; i++)
                    values[i] = Convert.ToString(selectors[i](item));

                Cancel.ThrowIfCancellationRequested();
                await writer.WriteLineAsync(string.Join(separator, values)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}