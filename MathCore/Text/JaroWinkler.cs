#nullable enable
namespace MathCore.Text;

/// <summary>
/// Сходство строк по метрике Джаро — Винклера<br/>
/// Расстояние между двумя строками, определяемаое как число односимвольных
/// преобразований, которое необходимо для того, чтобы изменить одно слово в другое.
/// </summary>
/// <remarks>
///     <seealso href="https://ru.wikipedia.org/wiki/Сходство_Джаро_—_Винклера"/>
/// </remarks>
public static class JaroWinkler
{
    /// <summary>Метрика в пространстве строк на основе сходства Джаро — Винклера</summary>
    /// <param name="Str1">Сравниваемая строка</param>
    /// <param name="Str2">Сравниваемая строка</param>
    /// <param name="WeightThreshold">Порог применения модификации Winkler</param>
    /// <param name="PrefixLength">Размер префикса</param>
    /// <param name="CharComparer">Объект сравнения символов строки</param>
    /// <returns>Расстояние между в пространстве строк (0 - строки совпадают, 1 - строки не совпадают)</returns>
    public static double Distance(string Str1, string Str2, double WeightThreshold = 0.7, int PrefixLength = 4, IEqualityComparer<char>? CharComparer = null) =>
        1.0 - Proximity(Str1, Str2, WeightThreshold, PrefixLength, CharComparer);

    /// <summary>Вычисление сходства двух строк на основе метрики Джаро — Винклера</summary>
    /// <param name="Str1">Сравниваемая строка</param>
    /// <param name="Str2">Сравниваемая строка</param>
    /// <param name="WeightThreshold">Порог применения модификации Winkler</param>
    /// <param name="PrefixLength">Размер префикса</param>
    /// <param name="CharComparer">Объект сравнения символов строки</param>
    /// <returns>Значение от 0 до 1: 0 - строки не совпадают, 1 - строки совпадают</returns>
    public static double Proximity(string Str1, string Str2, double WeightThreshold = 0.7, int PrefixLength = 4, IEqualityComparer<char>? CharComparer = null)
    {
        CharComparer ??= EqualityComparer<char>.Default;

        var l1 = Str1.Length;
        var l2 = Str2.Length;
        if (l1 == 0) return l2 == 0 ? 1 : 0;

        var range = Math.Max(0, Math.Max(l1, l2) / 2 - 1);

        var str1_matched = new bool[l1];
        var str2_matched = new bool[l2];

        var matching_count = 0;
        for (var i = 0; i < l1; i++)
            for (int j = Math.Max(0, i - range), end = Math.Min(i + range + 1, l2); j < end; j++)
            {
                if (str2_matched[j] || !CharComparer.Equals(Str1[i], Str2[j])) continue;
                str1_matched[i] = true;
                str2_matched[j] = true;
                matching_count++;
                break;
            }

        if (matching_count == 0) return 0;

        var transitions_count = 0;
        var k = 0;
        for (var i = 0; i < l1; i++)
            if (str1_matched[i])
            {
                while (!str2_matched[k])
                    k++;
                if (!CharComparer.Equals(Str1[i], Str2[k]))
                    transitions_count++;
                k++;
            }

        var distance_jaro =
        (
            matching_count / (double)l1
            + matching_count / (double)l2
            + (matching_count - transitions_count / 2) / (double)matching_count
        ) / 3;

        if (distance_jaro <= WeightThreshold) return distance_jaro;

        var prefix_max_length = Math.Min(PrefixLength, Math.Min(Str1.Length, Str2.Length));
        var prefix_length = 0;
        while (prefix_length < prefix_max_length && CharComparer.Equals(Str1[prefix_length], Str2[prefix_length]))
            prefix_length++;
        return prefix_length == 0
            ? distance_jaro
            : distance_jaro + 0.1 * prefix_length * (1.0 - distance_jaro);
    }
}