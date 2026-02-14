using System.Globalization;
using System.IO.Compression;

namespace MathCore.Interpolation;

/// <summary>Многомерный интерполятор Лагранжа для произвольного количества переменных</summary>
/// <remarks>
/// Выполняет интерполяцию функций многих переменных на основе метода Лагранжа.
/// Поддерживает загрузку данных из CSV-файлов, включая сжатые форматы ZIP и GZIP.
/// Использует нормализацию входных данных для улучшения численной устойчивости.
/// </remarks>
/// <example>
/// <code>
/// // Загрузка данных из CSV-файла
/// var file = new FileInfo("data.csv");
/// var interpolator = InterpolatorNDLagrange.LoadCSV(file);
/// 
/// // Вычисление интерполированного значения
/// var result = interpolator[1.5, 2.3, 3.7];
/// 
/// // Или через метод
/// var value = interpolator.GetValue(new[] { 1.5, 2.3, 3.7 });
/// </code>
/// </example>
public sealed class InterpolatorNDLagrange
{
    /// <summary>Загружает данные интерполяции из CSV-файла</summary>
    /// <param name="file">Файл с данными (поддерживаются форматы .csv, .zip, .gzip)</param>
    /// <param name="Header">Пропустить первую строку как заголовок</param>
    /// <param name="Separator">Символ-разделитель столбцов</param>
    /// <param name="SkipWrongLines">Пропускать строки с ошибками формата</param>
    /// <returns>Настроенный экземпляр интерполятора</returns>
    /// <exception cref="InvalidOperationException">Архив пустой или данные отсутствуют</exception>
    /// <remarks>
    /// Формат CSV: каждая строка содержит N аргументов и одно значение функции.
    /// Для ZIP-архивов автоматически выбирается первый .csv или .txt файл.
    /// </remarks>
    /// <example>
    /// <code>
    /// var file = new FileInfo("measurements.csv");
    /// var interpolator = InterpolatorNDLagrange.LoadCSV(file, Header: true, Separator: ';');
    /// 
    /// // Для сжатых файлов
    /// var zip_file = new FileInfo("data.zip");
    /// var interpolator_zip = InterpolatorNDLagrange.LoadCSV(zip_file);
    /// </code>
    /// </example>
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

    /// <summary>Загружает данные интерполяции из текстового потока</summary>
    /// <param name="reader">Текстовый поток с CSV-данными</param>
    /// <param name="Header">Пропустить первую строку как заголовок</param>
    /// <param name="Separator">Символ-разделитель столбцов</param>
    /// <param name="SkipWrongLines">Пропускать строки с ошибками формата</param>
    /// <returns>Настроенный экземпляр интерполятора</returns>
    /// <exception cref="InvalidOperationException">Отсутствуют данные для загрузки или ошибка формата</exception>
    /// <remarks>
    /// Автоматически определяет количество аргументов по первой строке данных.
    /// Последний столбец считается значением функции, остальные - аргументами.
    /// Выполняет нормализацию данных для улучшения численной устойчивости.
    /// </remarks>
    public static InterpolatorNDLagrange LoadCSV(TextReader reader, bool Header = true, char Separator = ';', bool SkipWrongLines = true)
    {
        if (Header)
            if (reader.ReadLine() is null)
                throw new InvalidOperationException("Отсутствуют данные для загрузки");

        var arguments_list = new List<double[]>(1000);
        var values_list = new List<double>(1000);

        string? line;

        var line_index = Header ? 1 : 0;
        int arguments_count;
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

    /// <summary>Количество опорных точек интерполяции</summary>
    public int PointsCount => _Points.Length;

    /// <summary>Предоставляет доступ к опорным точкам и их значениям</summary>
    /// <remarks>Позволяет перечислять все опорные точки с их аргументами и значениями функции</remarks>
    public readonly ref struct PointSelector(double[][] Points, double[] Values)
    {
        private readonly double[][] _Points = Points;

        private readonly double[] _Values = Values;

        /// <summary>Количество аргументов в каждой точке</summary>
        public int Count => _Points[0].Length;

        /// <summary>Получает аргументы указанной опорной точки</summary>
        /// <param name="n">Индекс опорной точки</param>
        /// <returns>Массив аргументов точки</returns>
        public IReadOnlyList<double> this[int n] => _Points[n];

        /// <summary>Возвращает перечислитель для обхода всех опорных точек</summary>
        /// <returns>Перечислитель точек</returns>
        public PointsEnumerator GetEnumerator() => new(_Points, _Values);
    }

    /// <summary>Перечислитель для обхода опорных точек интерполяции</summary>
    /// <remarks>Предоставляет пары (аргументы, значение) для каждой опорной точки</remarks>
    public ref struct PointsEnumerator(double[][] Points, double[] Values)
    {
        private readonly double[][] _Points = Points;

        private readonly double[] _Values = Values;

        private int _Index;

        /// <summary>Текущая опорная точка с аргументами и значением функции</summary>
        public (IReadOnlyList<double> Argument, double Value) Current { get; private set; }

        /// <summary>Переходит к следующей опорной точке</summary>
        /// <returns>true, если переход выполнен успешно; иначе false</returns>
        public bool MoveNext()
        {
            if (_Index >= _Points.Length)
                return false;

            Current = (_Points[_Index], _Values[_Index]);

            _Index++;

            return true;
        }
    }

    /// <summary>Коллекция опорных точек интерполяции</summary>
    /// <example>
    /// <code>
    /// var interpolator = InterpolatorNDLagrange.LoadCSV(file);
    /// 
    /// // Перечисление всех точек
    /// foreach (var (args, value) in interpolator.Points)
    /// {
    ///     Console.WriteLine($"Args: [{string.Join(", ", args)}] => Value: {value}");
    /// }
    /// </code>
    /// </example>
    public PointSelector Points => new(_Points, _Values);

    /// <summary>Вычисляет интерполированное значение для заданных аргументов</summary>
    /// <param name="X0">Аргументы функции</param>
    /// <returns>Интерполированное значение</returns>
    /// <exception cref="InvalidOperationException">Количество аргументов не соответствует размерности интерполятора</exception>
    /// <example>
    /// <code>
    /// var interpolator = InterpolatorNDLagrange.LoadCSV(file);
    /// var result = interpolator[1.5, 2.3, 3.7];
    /// </code>
    /// </example>
    public double this[params double[] X0] => GetValue(X0);

    /// <summary>Вычисляет интерполированное значение для заданных аргументов</summary>
    /// <param name="X0">Аргументы функции</param>
    /// <returns>Интерполированное значение</returns>
    /// <exception cref="InvalidOperationException">Количество аргументов не соответствует размерности интерполятора</exception>
    public double this[params IReadOnlyList<double> X0] => GetValue(X0);

    private InterpolatorNDLagrange(int ArgsCount, double[][] Points, double[] Values, double[] Min, double[] Range)
    {
        _Points = Points;
        _ArgsCount = ArgsCount;
        _Values = Values;
        _Min = Min;
        _Range = Range;
    }

    /// <summary>Вычисляет интерполированное значение функции в заданной точке методом Лагранжа</summary>
    /// <param name="X0">Точка, в которой вычисляется значение функции</param>
    /// <returns>Интерполированное значение</returns>
    /// <exception cref="InvalidOperationException">Количество переданных аргументов не совпадает с размерностью интерполятора</exception>
    /// <remarks>
    /// Использует многомерный метод Лагранжа с проекцией векторов для вычисления весовых коэффициентов.
    /// Выполняет нормализацию аргументов в диапазон [0,1] для улучшения численной устойчивости.
    /// </remarks>
    /// <example>
    /// <code>
    /// var interpolator = InterpolatorNDLagrange.LoadCSV(file);
    /// 
    /// // Вычисление значения в точке (1.5, 2.3)
    /// var value = interpolator.GetValue(new[] { 1.5, 2.3 });
    /// 
    /// // Или через индексатор
    /// var value2 = interpolator[1.5, 2.3];
    /// </code>
    /// </example>
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

    /// <summary>Вычисляет проекцию вектора для расчёта весового коэффициента Лагранжа</summary>
    /// <param name="X0">Точка интерполяции</param>
    /// <param name="Xi">i-я опорная точка</param>
    /// <param name="Xj">j-я опорная точка</param>
    /// <returns>Значение проекции, нормализованное по длинам векторов</returns>
    /// <remarks>Вычисляет скалярное произведение нормализованных векторов в многомерном пространстве</remarks>
    private double Projection(IReadOnlyList<double> X0, double[] Xi, double[] Xj)
    {
        var proj = 0d;
        var len_0 = 0d;
        var len_i = 0d;
        for (var n = 0; n < _ArgsCount; n++)
        {
            var x0 = (X0[n] - _Min[n]) / _Range[n]; // Нормализация аргументов
            var xi = (Xi[n] - _Min[n]) / _Range[n];
            var xj = (Xj[n] - _Min[n]) / _Range[n];

            var dx0 = x0 - xj;
            var dxi = xi - xj;

            proj += dx0 * dxi; // Скалярное произведение
            len_0 += dx0 * dx0; // Квадрат длины первого вектора
            len_i += dxi * dxi; // Квадрат длины второго вектора
        }

        if (proj == 0)
            return 0;

        var p = proj / (len_0 * len_i).Sqrt();

        return p;
    }
}
