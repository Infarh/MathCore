#nullable enable
using System.Collections;
using System.Globalization;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace MathCore.Interpolation;

public class InterpolatorNDLinear
{
    public static InterpolatorNDLinear LoadCSV(
        FileInfo file,
        bool Header = true,
        char Separator = ';',
        bool SkipWrongLines = true,
        Func<double[], double, bool>? ValueSelector = null)
    {
        if (string.Equals(file.Extension, ".zip", StringComparison.OrdinalIgnoreCase))
        {
            using var zip = ZipFile.OpenRead(file.FullName);
            var entry = zip.Entries.Count switch
            {
                0 => throw new InvalidOperationException("Пустой архив"),
                1 => zip.Entries[0],
                _ => zip.Entries.FirstOrDefault(e => string.Equals(Path.GetExtension(e.Name), ".csv", StringComparison.OrdinalIgnoreCase))
                  ?? zip.Entries.FirstOrDefault(e => string.Equals(Path.GetExtension(e.Name), ".txt", StringComparison.OrdinalIgnoreCase))
                  ?? zip.Entries[0]
            };
            using var reader = entry.Open().GetStreamReader();
            return LoadCSV(reader, Header, Separator, SkipWrongLines);
        }

        if (string.Equals(file.Extension, ".gzip", StringComparison.OrdinalIgnoreCase))
        {
            using var file_stream = file.OpenRead();
            using var gzip_stream = new GZipStream(file_stream, CompressionMode.Decompress);
            using var reader = new StreamReader(gzip_stream);
            return LoadCSV(reader, Header, Separator, SkipWrongLines);
        }

        using (var reader = file.OpenText())
            return LoadCSV(reader, Header, Separator, SkipWrongLines, ValueSelector);
    }

    public static InterpolatorNDLinear LoadCSV(
        TextReader reader,
        bool Header = true,
        char Separator = ';',
        bool SkipWrongLines = true,
        Func<double[], double, bool>? ValueSelector = null)
    {
        if (Header)
            if (reader.ReadLine() is null)
                throw new InvalidOperationException("Отсутствуют данные для загрузки");

        var arguments_list = new List<double[]>(1000);
        var values_list = new List<double>(1000);

        string line;

        var line_index = Header ? 1 : 0;
        var arguments_count = 0;
        do
        {
            line = reader.ReadLine() ?? throw new InvalidOperationException("Отсутствуют данные для загрузки");
            line_index++;

            arguments_count = line.CountChar(Separator);
            if (!SkipWrongLines && arguments_count == 0)
                throw new InvalidOperationException("Отсутствуют данные для загрузки");
        }
        while (line.Length == 0 && arguments_count == 0);

        do
        {
            var line_ptr = line.AsStringPtr();

            var args = new double[arguments_count];
            var value = 0d;

            var error_line = false;
            var i = 0;
            foreach (var s in line_ptr.Split(Separator))
            {
                if (!s.TryParseDouble(CultureInfo.CurrentCulture, out var v) && !s.TryParseDouble(CultureInfo.InvariantCulture, out v))
                {
                    if (!SkipWrongLines)
                        throw new InvalidOperationException($"Ошибка формата файла в строке {line_index}: невозможно прочитать вещественное число из значения {i} ({s.ToString()}");

                    error_line = true;
                    break;
                }

                if (i < arguments_count)
                {
                    args[i] = v;
                    i++;
                }
                else
                {
                    value = v;
                    break;
                }
            }

            if (error_line || ValueSelector?.Invoke(args, value) == false)
            {
                line_index++;
                continue;
            }

            arguments_list.Add(args);
            values_list.Add(value);

            line_index++;
        }
        while ((line = reader.ReadLine()!) is not null);

        var nodes = new List<ValueTreeNode>();

        for (var i = 0; i < values_list.Count; i++)
        {
            var argument = arguments_list[i];
            var value = values_list[i];

            ValueTreeNode.Add(nodes, argument, value);
        }

        return new(arguments_count, nodes);
    }

    private readonly int _ArgumentsCount;
    private readonly List<ValueTreeNode> _Nodes;

    private class ValueTreeNode(double Value, List<ValueTreeNode>? Childs = null) :
        IComparable<ValueTreeNode>, IComparable<double>,
        IEnumerable<ValueTreeNode>
    {
        public ValueTreeNode? this[int ChildIndex] => Childs?[ChildIndex];

        public static void Add(List<ValueTreeNode> nodes, ArrayPtr<double> args, double value)
        {
            if (nodes is []) // если в списке нет узлов (пока ещё...)
                switch (args) // в зависимости от списка аргументов
                {
                    case [var arg]: // если аргумент один (последний)
                        nodes.Add(new(arg, [value])); // то мы добавляем в список новый узел, который будет содержать значение для этого аргумента
                        return;

                    // если аргументов много (ещё много...)
                    case [var arg, .. var tail]: // мы отделяем первый аргумент и список оставшихся аргументов
                        var sub_nodes = new List<ValueTreeNode>(); // создаём список для оставшихся аргументов
                        Add(sub_nodes, tail, value); // добавляем в этот список узлы для оставшихся аргументов
                        nodes.Add(new(arg, sub_nodes)); // добавляем в текущий список узел с текущим аргументом
                        return;
                }

            var (head_arg, tail_args) = args;
            var index = nodes.SearchBinaryValue(head_arg);

            if (index >= 0)
            {
                var node = nodes[index];
                node.Add(tail_args, value);
            }
            else
            {
                var ind = ~index;
                nodes.Insert(ind, new(head_arg, new(1) { new(value) }));
            }
        }

        public double Value { get; } = Value;

        private void Add(ArrayPtr<double> args, double value) => Add(Childs, args, value);

        public double GetValue(ArrayPtr<double> args)
        {
            if(args.Length == 0) 
                return Childs[0].Value;

            var (x, xx) = args;

            var index = Childs.SearchBinaryValue(x);

            if (index >= 0)
                return Childs[index].GetValue(xx);

            var i1 = Math.Max(0, ~index - 1);
            var i2 = i1 + 1;

            var node1 = Childs[i1];
            var node2 = Childs[i2];

            var a = node1.Value;
            var b = node2.Value;

            var kx = (x - a) / (b - a);

            var y1 = node1.GetValue(xx);
            var y2 = node2.GetValue(xx);

            var y = y1 * (1 - kx) + y2 * kx;
            return y;
        }

        public override string ToString()
        {
            var result = new StringBuilder().Append(Value);

            switch (Childs)
            {
                case null: break;

                case []:
                    result.Append("[]");
                    break;

                case { Count: > 0 }:
                    result.Append('[');
                    foreach (var child in Childs)
                        result.Append(child.Value).Append(',');
                    result.Length--;
                    result.Append(']');
                    break;
            }

            return result.ToString();
        }

        public int CompareTo(ValueTreeNode? other) => Value.CompareTo(other.NotNull().Value);
        public int CompareTo(double other) => Value.CompareTo(other);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<ValueTreeNode> GetEnumerator() => Childs.GetEnumerator();

        public static implicit operator ValueTreeNode(double value) => new(value);
        public static implicit operator double(ValueTreeNode node) => node.Value;
    }

    public double this[params double[] args] => GetValue(args);

    private InterpolatorNDLinear(int ArgumentsCount, List<ValueTreeNode> nodes)
    {
        _ArgumentsCount = ArgumentsCount;
        _Nodes = nodes;
    }

    public double GetValue(params double[] arguments)
    {
        if (arguments.Length != _ArgumentsCount)
            throw new ArgumentException($"Передано аргументов {arguments.Length}, а требуется {_ArgumentsCount}");

        var (x, xx) = arguments.ToArrayPtr();

        var index = _Nodes.SearchBinaryValue(x);

        if (index >= 0)
            return _Nodes[index].GetValue(xx);

        var i1 = Math.Max(0, ~index - 1);
        var i2 = i1 + 1;

        var node1 = _Nodes[i1];
        var node2 = _Nodes[i2];

        var a = node1.Value;
        var b = node2.Value;

        var kx = (x - a) / (b - a);

        var y1 = node1.GetValue(xx);
        var y2 = node2.GetValue(xx);

        var y = y1 * (1 - kx) + y2 * kx;
        return y;
    }
}
