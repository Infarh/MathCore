namespace MathCore.Text;

/// <summary>
/// Сходство строк по метрике Джаро — Винклера<br/>
/// Расстояние между двумя строками, определяемое как число односимвольных
/// преобразований, которое необходимо для того, чтобы изменить одно слово в другое.
/// </summary>
/// <remarks>
/// Метрика Джаро — Винклера — это мера расстояния между строками, широко используемая 
/// для обнаружения дубликатов в записях. Она особенно эффективна для коротких строк 
/// и учитывает совпадение начальных символов (префикса).<br/>
/// <seealso href="https://ru.wikipedia.org/wiki/Сходство_Джаро_—_Винклера"/>
/// </remarks>
/// <example>
/// <![CDATA[
/// // Вычисление расстояния между строками
/// var distance = JaroWinkler.Distance("martha", "marhta");
/// Console.WriteLine($"Расстояние: {distance}"); // ~0.04 - строки очень похожи
/// 
/// // Вычисление сходства
/// var proximity = JaroWinkler.Proximity("dixon", "dicksonx");
/// Console.WriteLine($"Сходство: {proximity}"); // ~0.767
/// 
/// // Сравнение с учетом регистра
/// var case_sensitive = JaroWinkler.Proximity("Hello", "hello"); // ~0.867
/// 
/// // Сравнение без учета регистра
/// var case_insensitive = JaroWinkler.Proximity(
///     "Hello", 
///     "hello", 
///     CharComparer: StringComparer.OrdinalIgnoreCase
/// ); // 1.0 - полное совпадение
/// ]]>
/// </example>
public static class JaroWinkler
{
    /// <summary>Метрика в пространстве строк на основе сходства Джаро — Винклера</summary>
    /// <param name="Str1">Первая сравниваемая строка</param>
    /// <param name="Str2">Вторая сравниваемая строка</param>
    /// <param name="WeightThreshold">Порог применения модификации Winkler (по умолчанию 0.7). При сходстве Джаро выше этого порога применяется модификация Винклера, учитывающая совпадение префикса</param>
    /// <param name="PrefixLength">Максимальный размер префикса для модификации Винклера (по умолчанию 4 символа)</param>
    /// <param name="CharComparer">Объект сравнения символов строки. Позволяет настроить чувствительность к регистру и другие правила сравнения</param>
    /// <returns>Расстояние между строками в диапазоне [0..1]: 0 — строки идентичны, 1 — строки полностью различны</returns>
    /// <example>
    /// <![CDATA[
    /// // Простое сравнение
    /// var d1 = JaroWinkler.Distance("kitten", "sitting"); // ~0.253
    /// 
    /// // Одинаковые строки
    /// var d2 = JaroWinkler.Distance("test", "test"); // 0.0
    /// 
    /// // Полностью разные строки
    /// var d3 = JaroWinkler.Distance("abc", "xyz"); // 1.0
    /// 
    /// // Настройка порога модификации Винклера
    /// var d4 = JaroWinkler.Distance("martha", "marhta", WeightThreshold: 0.9);
    /// ]]>
    /// </example>
    public static double Distance(string Str1, string Str2, double WeightThreshold = 0.7, int PrefixLength = 4, IEqualityComparer<char>? CharComparer = null) =>
        1.0 - Proximity(Str1, Str2, WeightThreshold, PrefixLength, CharComparer);

    /// <summary>Вычисление сходства двух строк на основе метрики Джаро — Винклера</summary>
    /// <param name="Str1">Первая сравниваемая строка</param>
    /// <param name="Str2">Вторая сравниваемая строка</param>
    /// <param name="WeightThreshold">Порог применения модификации Winkler (по умолчанию 0.7). При сходстве Джаро выше этого порога применяется модификация Винклера, учитывающая совпадение префикса</param>
    /// <param name="PrefixLength">Максимальный размер префикса для модификации Винклера (по умолчанию 4 символа)</param>
    /// <param name="CharComparer">Объект сравнения символов строки. Позволяет настроить чувствительность к регистру и другие правила сравнения</param>
    /// <returns>Сходство строк в диапазоне [0..1]: 0 — строки полностью различны, 1 — строки идентичны</returns>
    /// <remarks>
    /// Алгоритм работает в три этапа:<br/>
    /// 1. Определяет совпадающие символы с учетом максимального допустимого расстояния<br/>
    /// 2. Вычисляет количество транспозиций (перестановок символов)<br/>
    /// 3. Если сходство Джаро превышает WeightThreshold, применяет модификацию Винклера, увеличивающую вес совпадающего префикса<br/>
    /// <br/>
    /// Метрика особенно полезна для:<br/>
    /// - Поиска дубликатов в базах данных<br/>
    /// - Исправления опечаток<br/>
    /// - Нечеткого поиска по тексту<br/>
    /// - Сравнения имен и фамилий
    /// </remarks>
    /// <example>
    /// <![CDATA[
    /// // Базовое использование
    /// var p1 = JaroWinkler.Proximity("martha", "marhta"); // ~0.961
    /// var p2 = JaroWinkler.Proximity("dwayne", "duane");  // ~0.840
    /// 
    /// // Сравнение с чувствительностью к регистру
    /// var p3 = JaroWinkler.Proximity("Test", "test"); // ~0.933
    /// 
    /// // Сравнение без учета регистра
    /// var p4 = JaroWinkler.Proximity(
    ///     "Test", 
    ///     "test",
    ///     CharComparer: StringComparer.OrdinalIgnoreCase
    /// ); // 1.0
    /// 
    /// // Настройка длины префикса
    /// var p5 = JaroWinkler.Proximity(
    ///     "ABCDEFGH", 
    ///     "ABCDXYZ",
    ///     PrefixLength: 6 // учитывать до 6 символов префикса
    /// );
    /// 
    /// // Использование для поиска похожих записей
    /// var names = new[] { "John Smith", "Jon Smith", "Jane Smith", "Bob Jones" };
    /// var target = "John Smyth";
    /// var similar = names
    ///     .Select(name => new { Name = name, Score = JaroWinkler.Proximity(target, name) })
    ///     .Where(x => x.Score > 0.8) // порог сходства
    ///     .OrderByDescending(x => x.Score)
    ///     .ToList();
    /// // Результат: Jon Smith (0.957), John Smith (0.948)
    /// ]]>
    /// </example>
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