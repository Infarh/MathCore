#nullable enable
using System.Collections;

using MathCore.Values;

// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToUsingDeclaration
// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Local
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable InvertIf
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace System.Linq;

public static partial class IEnumerableExtensions
{
    public static (double Average, double Dispersion) AverageAndDispersion(this IEnumerable<double> values)
    {
        var avg = 0d;
        var d = 0d;

        foreach(var x in values)
        {
            avg += x;
            d += x * x;
        }

        return (avg, d - avg * avg);
    }

    public static (double Average, double Sigma) AverageAndSigma(this IEnumerable<double> values)
    {
        var (avg, d) = values.AverageAndDispersion();
        return (avg, d.Sqrt());
    }

    /// <summary>Усреднение последовательности методом скользящего среднего с заданной длиной окна</summary>
    /// <param name="samples">Исходная последовательность</param>
    /// <param name="WindowLength">Длина окна выборки (должна быть больше 1)</param>
    /// <returns>Усреднённая последовательность если <paramref name="WindowLength"/> &gt; 1</returns>
    /// <exception cref="ArgumentNullException">Если отсутствует ссылка на <paramref name="samples"/></exception>
    /// <exception cref="ArgumentOutOfRangeException">При <paramref name="WindowLength"/> &lt; 1</exception>
    public static IEnumerable<double> AverageWindow(this IEnumerable<double> samples, int WindowLength)
    {
        if (samples is null) throw new ArgumentNullException(nameof(samples));
        switch (WindowLength)
        {
            case < 1: throw new ArgumentOutOfRangeException(nameof(WindowLength), WindowLength, "Длина окна должна быть больше 0");
            case 1:
                {
                    foreach (var sample in samples)
                        yield return sample;
                    yield break;
                }
        }

        var average = new AverageValue(WindowLength);

        foreach (var sample in samples)
            yield return average.AddValue(sample);
    }

    /// <summary>Усреднение последовательности методом выбора медианы в окне выборки</summary>
    /// <param name="samples">Исходная последовательность</param>
    /// <param name="WindowLength">Длина окна выборки (должна быть больше 1)</param>
    /// <returns>Усреднённая последовательность если <paramref name="WindowLength"/> &gt; 1</returns>
    /// <exception cref="ArgumentNullException">Если отсутствует ссылка на <paramref name="samples"/></exception>
    /// <exception cref="ArgumentOutOfRangeException">При <paramref name="WindowLength"/> &lt; 1</exception>
    public static IEnumerable<double> AverageMedian(this IEnumerable<double> samples, int WindowLength)
    {
        if(samples is null) throw new ArgumentNullException(nameof(samples));
        switch (WindowLength)
        {
            case < 1: throw new ArgumentOutOfRangeException(nameof(WindowLength), WindowLength, "Длина окна должна быть больше 0");
            case 1:
                {
                    foreach (var sample in samples)
                        yield return sample;
                    yield break;
                }
        }

        var buffer = new double[WindowLength];  // Отсортированный буфер для вычисления медианы
        var is_even = WindowLength % 2 == 0;    // Длина буфера является чётной
        var length05 = WindowLength / 2;        // Индекс середины в буфере
        // Создаём циклическую очередь для определения последнего элемента в буфере
        var queue = new LinearQueue<double>(WindowLength);

        var element = samples.GetEnumerator();
        try
        {
            if (!element.MoveNext())
                yield break;

            var x = element.Current;
            buffer[0] = x;
            queue.Add(buffer[0]);
            yield return buffer[0];

            if (!element.MoveNext())
                yield break;

            // Заполняем буфер
            for (var i = 1; i < WindowLength; i++)
            {
                x = element.Current;
                queue.Add(x);

                var j = 0;
                while (j < i && buffer[j] <= x)
                    j++;

                if (j == i) 
                    buffer[i] = x;
                else
                {
                    var tmp = buffer[j];
                    buffer[j] = x;
                    while (j < i - 1)
                    {
                        j++;
                        (buffer[j], tmp) = (tmp, buffer[j]);
                    }

                    buffer[j + 1] = tmp;
                }

                yield return i % 2 == 1 
                    ? (buffer[i / 2] + buffer[i / 2 + 1]) / 2 
                    : buffer[i / 2];

                if (!element.MoveNext())
                    yield break;
            }

            do
            {
                x = element.Current;
                var last_x = queue.Add(x);  // Добавляем в циклическую очередь новый элемент и обратно получаем последний элемент из очереди

                // Ищем индекс удалённого из циклического буфера последнего элемента что бы заменить его новым
                var index = 0;
                while (index < WindowLength && buffer[index] != last_x)
                    index++;

                // Если вдруг индекс не был найден, то это ошибка структуры алгоритма
                if (index == WindowLength)
                    throw new InvalidOperationException($"Предыдущий элемент {last_x} не найден в буфере [{string.Join(", ", buffer)}]");

                // Записываем в отсортированный буфер на место старого элемента новый
                buffer[index] = x;
                // Теперь буфер не отсортирован. Но в нём всего один элемент нарушает порядок
                // Можно применить простейший алгоритм сортировки (пузырьком)

                // Элемент может быть либо больше предыдущего, либо меньше - когда надо выполнять сортировку
                if (x > last_x)
                    for (var i = index; i < WindowLength - 1; i++)
                        if (buffer[i] <= buffer[i + 1]) // Если при очередной проверке порядок двух соседних элементов не нарушен, то сортировка закончина
                            break;
                        else
                            (buffer[i], buffer[i + 1]) = (buffer[i + 1], buffer[i]);
                else if (x < last_x)
                    for (var i = index; i > 0; i--)
                        if (buffer[i] >= buffer[i - 1]) // Если при очередной проверке порядок двух соседних элементов не нарушен, то сортировка закончина
                            break;
                        else
                            (buffer[i], buffer[i - 1]) = (buffer[i - 1], buffer[i]);

                yield return is_even
                    ? (buffer[length05 - 1] + buffer[length05]) / 2
                    : buffer[length05];
            }
            while (element.MoveNext());
        }
        finally
        {
            element.Dispose();
        }
    }

    public static (double Q1, double Median, double Q3) GetQ1Q3(this IEnumerable<double> samples)
    {
        var buffer = samples.ToArray();

        if (buffer is { Length: 0 })
            throw new InvalidOperationException("Последовательность не содержит элементов");

        if (buffer is { Length: 1 })
            return (double.NaN, buffer[0], double.NaN);

        Array.Sort(buffer);

        var q1 = buffer[buffer.Length / 4];
        var median = buffer[buffer.Length / 2];
        var q3 = buffer[3 * buffer.Length / 4];

        return (q1, median, q3);
    }
}
