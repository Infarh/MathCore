#nullable enable
namespace MathCore.Text;

/// <summary>
/// Расстояние Левенштейна (редакционное расстояние)<br/>
/// Метрика, измеряющая по модулю разность между двумя последовательностями символов,
/// определяемая как минимальное количество односимвольных операций (а именно вставки,
/// удаления, замены), необходимых для превращения одной последовательности символов в другую
/// </summary>
/// <remarks>
///     <seealso href="https://ru.wikibooks.org/wiki/Реализации_алгоритмов/Расстояние_Левенштейна#C#"/>
/// </remarks>
public static class Levenshtein
{
    public static int Distance(string S1, string S2)
    {
        if (S1 is not { Length: var s1_length }) throw new ArgumentNullException(nameof(S1));
        if (S2 is not { Length: var s2_length }) throw new ArgumentNullException(nameof(S2));

        var results = new int[s1_length + 1, s2_length + 1];

        for (var i = 0; i <= s1_length; i++) results[i, 0] = i;
        for (var j = 0; j <= s2_length; j++) results[0, j] = j;

        for (var i = 1; i <= s1_length; i++)
            for (var j = 1; j <= s2_length; j++)
                results[i, j] = Math.Min(
                    Math.Min(
                        results[i - 1, j] + 1,
                        results[i, j - 1] + 1),
                    S1[i - 1] == S2[j - 1] 
                        ? results[i - 1, j - 1]
                        : results[i - 1, j - 1] + 1);

        return results[s1_length, s2_length];
    }

    public static int DistanceFast(string source, string target)
    {
        var cost_matrix = new int[source.Length + 1][];
        for (var i = 0; i < cost_matrix.Length; i++)
            cost_matrix[i] = new int[target.Length + 1];
        //var cost_matrix = Enumerable
        //   .Range(0, source.Length + 1)
        //   .Select(_ => new int[target.Length + 1])
        //   .ToArray();

        for (var i = 1; i <= source.Length; ++i) 
            cost_matrix[i][0] = i;

        for (var i = 1; i <= target.Length; ++i) 
            cost_matrix[0][i] = i;

        for (var i = 1; i <= source.Length; ++i)
            for (var j = 1; j <= target.Length; ++j)
            {
                var insert = cost_matrix[i][j - 1] + 1;
                var delete = cost_matrix[i - 1][j] + 1;
                var edit   = cost_matrix[i - 1][j - 1] + (source[i - 1] == target[j - 1] ? 0 : 1);

                cost_matrix[i][j] = Math.Min(Math.Min(insert, delete), edit);
            }

        return cost_matrix[source.Length][target.Length];
    }
}