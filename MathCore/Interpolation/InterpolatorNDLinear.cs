#nullable enable
using System.Collections;
using System.Globalization;
using System.IO.Compression;
using System.Text;

namespace MathCore.Interpolation;

/// <summary>Класс для линейной интерполяции многомерных данных</summary>
public class InterpolatorNDLinear
{
    /// <summary>Загрузить интерполятор из CSV файла</summary>
    /// <param name="FilePath">Путь к файлу</param>
    /// <param name="Header">Присутствует ли заголовок</param>
    /// <param name="Separator">Символ-разделитель</param>
    /// <param name="SkipWrongLines">Пропускать ли некорректные строки</param>
    /// <param name="ValueSelector">Функция для выбора значений</param>
    /// <returns>Экземпляр <see cref="InterpolatorNDLinear"/></returns>
    public static InterpolatorNDLinear LoadCSV(
        string FilePath,
        bool Header = true,
        char Separator = ';',
        bool SkipWrongLines = true,
        Func<double[], double, bool>? ValueSelector = null)
        => LoadCSV(new FileInfo(FilePath), Header, Separator, SkipWrongLines, ValueSelector);

    /// <summary>Загрузить интерполятор из CSV файла</summary>
    /// <param name="file">Файл</param>
    /// <param name="Header">Присутствует ли заголовок</param>
    /// <param name="Separator">Символ-разделитель</param>
    /// <param name="SkipWrongLines">Пропускать ли некорректные строки</param>
    /// <param name="ValueSelector">Функция для выбора значений</param>
    /// <returns>Экземпляр <see cref="InterpolatorNDLinear"/></returns>
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
                _ => zip.Entries.FirstOrDefault(e => Path.GetExtension(e.Name).EqualsInvariantIgnoreCase(".csv"))
                  ?? zip.Entries.FirstOrDefault(e => Path.GetExtension(e.Name).EqualsInvariantIgnoreCase(".txt"))
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

    /// <summary>Загрузить интерполятор из CSV файла</summary>
    /// <param name="reader">Читатель текстовых данных</param>
    /// <param name="Header">Присутствует ли заголовок</param>
    /// <param name="Separator">Символ-разделитель</param>
    /// <param name="SkipWrongLines">Пропускать ли некорректные строки</param>
    /// <param name="ValueSelector">Функция для выбора значений</param>
    /// <returns>Экземпляр <see cref="InterpolatorNDLinear"/></returns>
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

        var culture = CultureInfo.GetCultureInfo("ru-ru");

        do
        {
            var line_ptr = line.AsStringPtr();

            var args = new double[arguments_count];
            var value = 0d;

            var error_line = false;
            var i = 0;
            foreach (var s in line_ptr.Split(Separator))
            {
                if (!double.TryParse(s, NumberStyles.Any, culture, out var v))
                {
                    if (!SkipWrongLines)
                        throw new InvalidOperationException($"Ошибка формата файла в строке {line_index}: невозможно прочитать вещественное число из значения {i} ({s.ToString()}");

                    error_line = true;
                    break;
                }

                if (i < arguments_count)
                    args[i++] = v;
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

    /// <summary>Внутренний класс для представления узла дерева значений</summary>
    private class ValueTreeNode(double Value, List<ValueTreeNode>? Childs = null) :
        IComparable<ValueTreeNode>, IComparable<double>,
        IEnumerable<ValueTreeNode>
    {
        /// <summary>Индексатор для доступа к дочерним узлам</summary>
        /// <param name="ChildIndex">Индекс дочернего узла</param>
        public ValueTreeNode? this[int ChildIndex] => Childs?[ChildIndex];

        /// <summary>Добавить узел в дерево значений</summary>
        /// <param name="nodes">Список узлов</param>
        /// <param name="args">Аргументы</param>
        /// <param name="value">Значение</param>
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
                nodes[index].Add(tail_args, value);
            else if (tail_args.Length > 0)
            {
                var node = new ValueTreeNode(head_arg, []);
                nodes.Insert(~index, node);
                node.Add(tail_args, value);
            }
            else
                nodes.Insert(~index, new(head_arg, [new(value)]));
        }

        /// <summary>Значение узла</summary>
        public double Value { get; } = Value;

        /// <summary>Добавить узел в дерево значений</summary>
        /// <param name="args">Аргументы</param>
        /// <param name="value">Значение</param>
        private void Add(ArrayPtr<double> args, double value) => Add(Childs, args, value);

        /// <summary>Получить значение по аргументам</summary>
        /// <param name="args">Аргументы</param>
        /// <returns>Значение</returns>
        public double GetValue(ArrayPtr<double> args)
        {
            var childs = Childs;
            if (args.Length == 0 || childs is null)
                return childs?[0].Value ?? double.NaN;

            var (x, xx) = args;

            var index = childs.SearchBinaryValue(x);

            if (index >= 0)
                return childs[index].GetValue(xx);

            var i1 = Math.Max(0, ~index - 1);
            var i2 = Math.Min(childs.Count - 1, i1 + 1);

            var node1 = childs[i1];
            var node2 = childs[i2];

            var a = node1.Value;
            var b = node2.Value;

            if (a == b) return node1.GetValue(xx);

            var kx = (x - a) / (b - a);

            var y1 = node1.GetValue(xx);
            var y2 = node2.GetValue(xx);

            var y = y1 * (1 - kx) + y2 * kx;
            return y;
        }

        /// <summary>Преобразовать узел в строку</summary>
        /// <returns>Строковое представление узла</returns>
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

        /// <summary>Сравнить узел с другим узлом</summary>
        /// <param name="other">Другой узел</param>
        /// <returns>Результат сравнения</returns>
        public int CompareTo(ValueTreeNode? other) => Value.CompareTo(other.NotNull().Value);

        /// <summary>Сравнить узел с числом</summary>
        /// <param name="other">Число</param>
        /// <returns>Результат сравнения</returns>
        public int CompareTo(double other) => Value.CompareTo(other);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Получить перечислитель для дочерних узлов</summary>
        /// <returns>Перечислитель</returns>
        public IEnumerator<ValueTreeNode> GetEnumerator() => Childs.GetEnumerator();

        public static implicit operator ValueTreeNode(double value) => new(value);
        public static implicit operator double(ValueTreeNode node) => node.Value;
    }

    /// <summary>Индексатор для получения значения по аргументам</summary>
    /// <param name="args">Аргументы</param>
    public double this[params double[] args] => GetValue(args);

    /// <summary>Конструктор интерполятора</summary>
    /// <param name="ArgumentsCount">Количество аргументов</param>
    /// <param name="nodes">Список узлов</param>
    private InterpolatorNDLinear(int ArgumentsCount, List<ValueTreeNode> nodes)
    {
        _ArgumentsCount = ArgumentsCount;
        _Nodes = nodes;
    }

    /// <summary>Получить значение по аргументам</summary>
    /// <param name="arguments">Аргументы</param>
    /// <returns>Значение</returns>
    public double GetValue(params double[] arguments)
    {
        if (arguments.Length != _ArgumentsCount)
            throw new ArgumentException($"Передано аргументов {arguments.Length}, а требуется {_ArgumentsCount}");

        var (x, xx) = arguments.ToArrayPtr();

        var nodes = _Nodes;
        var index = nodes.SearchBinaryValue(x);

        if (index >= 0)
            return nodes[index].GetValue(xx);

        var i1 = Math.Max(0, ~index - 1);
        var i2 = Math.Min(nodes.Count - 1, i1 + 1);

        var node1 = nodes[i1];
        var node2 = nodes[i2];

        var a = node1.Value;
        var b = node2.Value;

        if (a == b) return node1.GetValue(xx);

        var kx = (x - a) / (b - a);

        var y1 = node1.GetValue(xx);
        var y2 = node2.GetValue(xx);

        var y = y1 * (1 - kx) + y2 * kx;
        return y;
    }
}
