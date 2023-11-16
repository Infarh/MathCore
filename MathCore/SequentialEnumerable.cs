#nullable enable
using System.Collections;

namespace MathCore;

/// <summary>Итератор выбирает последовательно одно из значений нескольких перечислений в соответствии с указанным объектом сравнения</summary>
/// <typeparam name="T">Тип элементов коллекций</typeparam>
public class SequentialEnumerable<T> : IEnumerable<T>
{
    private readonly IComparer<T> _Comparer;
    private readonly IEnumerable<T>[] _Enumerables;

    public SequentialEnumerable(IEnumerable<IEnumerable<T>> Enumerables) : this(Enumerables.ToArray()) { }
    public SequentialEnumerable(params IEnumerable<T>[] Enumerables) : this(Comparer<T>.Default, Enumerables) { }

    public SequentialEnumerable(IComparer<T> Comparer, IEnumerable<IEnumerable<T>> Enumerables) : this(Comparer, Enumerables.ToArray()) { }

    public SequentialEnumerable(IComparer<T> Comparer, params IEnumerable<T>[] Enumerables)
    {
        _Comparer    = Comparer ?? throw new ArgumentNullException(nameof(Comparer));
        _Enumerables = Enumerables ?? throw new ArgumentNullException(nameof(Enumerables));
    }

    public IEnumerator<T> GetEnumerator()
    {
        using var enumerators = _Enumerables.Select(e => e.GetEnumerator()).AsDisposableGroup();
        var       count       = enumerators.Count;
        if (count == 0) yield break;

        var process_enumerators = enumerators.Items.ToArray();

        var compare_results = new int[count];

        do
        {
            for (var i = 0; i < count; i++)
                if (compare_results[i] == 0 && !process_enumerators[i]?.MoveNext() is true)
                    process_enumerators[i] = null;

            for (var i = 0; i < count; i++) compare_results[i] = 1;

            var index = 0;
            while (index < count && process_enumerators[index] is null) index++;
            if (index == count)
                yield break;

            compare_results[index] = 0;
            var value = process_enumerators[index]!.Current;
            for (var i = index + 1; i < count; i++)
            {
                if (process_enumerators[i] is null) continue;

                var current_value  = process_enumerators[i]!.Current;
                var compare_result = _Comparer.Compare(value, current_value);
                if (compare_result > 0)
                {
                    compare_results[i] = 0;
                    index              = i;
                    for (var j = i - 1; j >= 0; j--)
                        compare_results[j] = 1;
                    value = current_value;
                }
                else
                    compare_results[i] = compare_result;
            }

            yield return value;
        }
        while (true);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}