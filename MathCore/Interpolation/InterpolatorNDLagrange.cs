using System.Drawing;
using System.Globalization;
using System.IO.Compression;

namespace MathCore.Interpolation;

public sealed class InterpolatorNDLagrange
{
    public static InterpolatorNDLagrange LoadCSV(FileInfo file, bool Header = true, char Separator = ';', bool SkipWrongLines = true)
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
            return LoadCSV(reader, Header, Separator, SkipWrongLines);
    }

    public static InterpolatorNDLagrange LoadCSV(TextReader reader, bool Header = true, char Separator = ';', bool SkipWrongLines = true)
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

        var min = new double[arguments_count];
        var max = new double[arguments_count];

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

                    min[i] = Math.Min(min[i], v);
                    max[i] = Math.Max(max[i], v);

                    i++;
                }
                else
                {
                    value = v;
                    break;
                }
            }

            if (error_line)
            {
                line_index++;
                continue;
            }

            arguments_list.Add(args);
            values_list.Add(value);

            line_index++;
        }
        while ((line = reader.ReadLine()) is not null);

        for (var i = 0; i < arguments_count; i++)
            max[i] -= min[i];

        return new(arguments_count, [.. arguments_list], [.. values_list], min, max);
    }

    private readonly int _ArgsCount;
    private readonly double[][] _Points;
    private readonly double[] _Values;
    private readonly double[] _Min;
    private readonly double[] _Range;

    public int PointsCount => _Points.Length;

    public readonly ref struct PointSelector(double[][] Points, double[] Values)
    {
        private readonly double[][] _Points = Points;

        private readonly double[] _Values = Values;

        public int Count => _Points[0].Length;

        public IReadOnlyList<double> this[int n] => _Points[n];

        public PointsEnumerator GetEnumerator() => new(_Points, _Values);
    }

    public ref struct PointsEnumerator(double[][] Points, double[] Values)
    {
        private readonly double[][] _Points = Points;

        private readonly double[] _Values = Values;

        private int _Index;

        public (IReadOnlyList<double> Argument, double Value) Current { get; private set; }

        public bool MoveNext()
        {
            if (_Index >= _Points.Length) 
                return false;

            Current = (_Points[_Index], _Values[_Index]);

            _Index++;

            return true;
        }
    }

    public PointSelector Points => new(_Points, _Values);

    public double this[params double[] X0] => GetValue(X0);

    public double this[params IReadOnlyList<double> X0] => GetValue(X0);

    private InterpolatorNDLagrange(int ArgsCount, double[][] Points, double[] Values, double[] Min, double[] Range)
    {
        _Points = Points;
        _ArgsCount = ArgsCount;
        _Values = Values;
        _Min = Min;
        _Range = Range;
    }

    public double GetValue(params IReadOnlyList<double> X0)
    {
        if (X0.Count != _ArgsCount)
            throw new InvalidOperationException($"Количество переданных аргументов {X0.Count}, а требуется {_ArgsCount}");

        var result = 0d;
        for (var i = 0; i < _Values.Length; i++)
        {
            var Xi = _Points[i];

            var p = 1d;
            for (var j = 0; p != 0 && j < _Values.Length; j++)
                if (j != i)
                {
                    var Xj = _Points[j];

                    var proj = Projection(X0, Xi, Xj);

                    p *= proj;
                }

            if (p == 0)
                continue;

            var fi = _Values[i];
            result += fi * p;
        }

        return result;
    }

    private double Projection(IReadOnlyList<double> X0, double[] Xi, double[] Xj)
    {
        var prod = 0d;
        var len0 = 0d;
        var leni = 0d;
        for (var n = 0; n < _ArgsCount; n++)
        {
            var x0 = (X0[n] - _Min[n]) / _Range[n];
            var xi = (Xi[n] - _Min[n]) / _Range[n];
            var xj = (Xj[n] - _Min[n]) / _Range[n];

            var dx0 = x0 - xj;
            var dxi = xi - xj;

            prod += dx0 * dxi;
            len0 += dx0 * dx0;
            leni += dxi * dxi;
        }

        if (prod == 0)
            return 0;

        var p = prod / (len0 * leni).Sqrt();
        //var p_1 = prod * (len0 * leni).SqrtInvFast3();

        return p;
    }
}
