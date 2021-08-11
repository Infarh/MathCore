#nullable enable
using System;

namespace MathCore.Text
{
    /// <summary>Расстояние Левенштейна</summary>
    /// <remarks>
    ///     <seealso href="https://ru.wikibooks.org/wiki/Реализации_алгоритмов/Расстояние_Левенштейна#C#"/>
    /// </remarks>
    public static class Levenshtein
    {
        public static int Distance(string S1, string S2)
        {
            if (S1 is null) throw new ArgumentNullException(nameof(S1));
            if (S2 is null) throw new ArgumentNullException(nameof(S2));

            var s1_length = S1.Length;
            var s2_length = S2.Length;

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
    }
}