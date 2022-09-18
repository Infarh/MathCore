namespace MathCore.Algorithms.Numbers;

public static class Quick
{
    public static void Test()
    {

        double[,] vv =
        {
            { 2, 7, 99, 106, 41 },
            { 18, 41, 110, 112, 113 },
            { 18, 41, 110, 112, 113 },
            { 110, 110, 111, 112, 115 },
            { 0, 3, 115, 120, 190 },
            { 3, 4, 116, 101, 65 },
        };

        var values = new List<double>();
        for (var i = 0; i < vv.GetLength(0); i++)
            for (var j = 0; j < vv.GetLength(1); j++)
                values.Add(vv[i, j]);

        var mediane = Quick.QuickSelectMedian(values, Quick.SelectorRandom());
    }

    public static Func<IReadOnlyList<double>, double> SelectorHalfDiv() => v => v.Count switch
    {
        0 => throw new InvalidOperationException("В списке нет элементов"),
        1 => v[0], 
        2 => v[0], 
        3 => v[1],
        _ => v[v.Count / 2]
    };

    public static Func<IReadOnlyList<double>, double> SelectorRandom(Random rnd = null)
    {
        rnd ??= new();
        return v => v.Count switch
        {
            0 => throw new InvalidOperationException("В списке нет элементов"),
            1 => v[0],
            _ => v[rnd.Next(v.Count)]
        };
    }

    public static double QuickSelectMedian(IReadOnlyList<double> Elements, Func<IReadOnlyList<double>, double> PivotSelector)
    {
        if (Elements.Count % 2 == 1)
            return QuickSelect(Elements, Elements.Count / 2, PivotSelector);
        return 0.5 * (
            QuickSelect(Elements, Elements.Count / 2 - 1, PivotSelector) +
            QuickSelect(Elements, Elements.Count / 2, PivotSelector));

    }

    static (List<double> low, int PivotCount, List<double> high, double Pivot) GetItems(IReadOnlyList<double> Elements, double Pivot)
    {
        var lows = new List<double>(Elements.Count);
        var highs = new List<double>(Elements.Count);
        var pivots_count = 0;

        foreach (var element in Elements)
            if (element < Pivot)
                lows.Add(element);
            else if (element > Pivot)
                highs.Add(element);
            else
                pivots_count++;

        lows.TrimExcess();
        highs.TrimExcess();
        return (lows, pivots_count, highs, Pivot);
    }

    public static double QuickSelect(IReadOnlyList<double> Elements, int Index, Func<IReadOnlyList<double>, double> PivotSelector)
    {
        if (Elements.Count == 1)
            return Index == 0
                ? Elements[Index]
                : throw new ArgumentOutOfRangeException(nameof(Index), Index, "Индекс превышает границу списка элементов")
                {
                    Data = { { nameof(Elements) + ".Count", Elements.Count } }
                };

        var (lows, pivots_count, highs, pivot) = GetItems(Elements, PivotSelector(Elements));

        if (Index < lows.Count)
            return QuickSelect(lows, Index, PivotSelector);
        
        if (Index < lows.Count + pivots_count)
            return pivot;
        
        return QuickSelect(highs, Index - lows.Count - pivots_count, PivotSelector);
    }
}