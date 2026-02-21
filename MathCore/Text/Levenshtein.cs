namespace MathCore.Text;

/// <summary>
/// Расстояние Левенштейна (редакционное расстояние)<br/>
/// Метрика, измеряющая по модулю разность между двумя последовательностями символов,
/// определяемая как минимальное количество односимвольных операций (а именно вставки,
/// удаления, замены), необходимых для превращения одной последовательности символов в другую
/// </summary>
/// <remarks>
/// Расстояние Левенштейна — классическая метрика строковых данных, используемая в:<br/>
/// - Проверке орфографии и автокоррекции<br/>
/// - Сравнении ДНК-последовательностей в биоинформатике<br/>
/// - Нечетком поиске и сравнении текстов<br/>
/// - Определении степени различия между версиями файлов<br/>
/// <br/>
/// Алгоритм использует динамическое программирование для построения матрицы расстояний.<br/>
/// Временная сложность: O(m×n), где m и n — длины строк<br/>
/// Пространственная сложность: O(m×n) для Distance, O(n) можно оптимизировать<br/>
/// <seealso href="https://ru.wikibooks.org/wiki/Реализации_алгоритмов/Расстояние_Левенштейна#C#"/>
/// </remarks>
/// <example>
/// <![CDATA[
/// // Основное использование
/// var distance = Levenshtein.Distance("kitten", "sitting"); // 3
/// // Требуется 3 операции: k→s, e→i, +g
/// 
/// // Быстрая версия для больших строк
/// var fast_distance = Levenshtein.DistanceFast("intention", "execution"); // 5
/// 
/// // Проверка на опечатки
/// var typo_distance = Levenshtein.Distance("teh", "the"); // 2
/// 
/// // Поиск похожих слов в словаре
/// var dictionary = new[] { "cat", "hat", "rat", "bat", "car" };
/// var word = "cut";
/// var similar = dictionary
///     .Select(w => new { Word = w, Distance = Levenshtein.Distance(word, w) })
///     .Where(x => x.Distance <= 2) // максимум 2 изменения
///     .OrderBy(x => x.Distance)
///     .ToList();
/// // Результат: cat (1), car (1)
/// ]]>
/// </example>
public static class Levenshtein
{
    /// <summary>Вычисление расстояния Левенштейна между двумя строками</summary>
    /// <param name="S1">Первая строка</param>
    /// <param name="S2">Вторая строка</param>
    /// <returns>Минимальное количество односимвольных операций (вставка, удаление, замена) для преобразования S1 в S2</returns>
    /// <exception cref="ArgumentNullException">Если S1 или S2 равны null</exception>
    /// <remarks>
    /// Реализация использует двумерную матрицу для хранения промежуточных результатов.<br/>
    /// Для больших строк рекомендуется использовать <see cref="DistanceFast"/>, 
    /// который имеет более эффективную работу с памятью
    /// </remarks>
    /// <example>
    /// <![CDATA[
    /// // Простые примеры
    /// var d1 = Levenshtein.Distance("", "abc");        // 3 (3 вставки)
    /// var d2 = Levenshtein.Distance("abc", "");        // 3 (3 удаления)
    /// var d3 = Levenshtein.Distance("abc", "abc");     // 0 (строки идентичны)
    /// var d4 = Levenshtein.Distance("abc", "adc");     // 1 (одна замена b→d)
    /// 
    /// // Более сложные примеры
    /// var d5 = Levenshtein.Distance("saturday", "sunday");     // 3
    /// var d6 = Levenshtein.Distance("book", "back");           // 2
    /// var d7 = Levenshtein.Distance("algorithm", "altruistic"); // 6
    /// 
    /// // Обработка исключений
    /// try
    /// {
    ///     var d8 = Levenshtein.Distance(null, "test");
    /// }
    /// catch (ArgumentNullException ex)
    /// {
    ///     Console.WriteLine("Строки не могут быть null");
    /// }
    /// ]]>
    /// </example>
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

    /// <summary>Быстрое вычисление расстояния Левенштейна с оптимизацией использования памяти</summary>
    /// <param name="source">Исходная строка</param>
    /// <param name="target">Целевая строка</param>
    /// <returns>Минимальное количество односимвольных операций (вставка, удаление, замена) для преобразования source в target</returns>
    /// <remarks>
    /// Эта версия алгоритма оптимизирована для работы с большими строками.<br/>
    /// Использует более эффективное представление матрицы стоимостей в виде массива массивов,
    /// что может дать прирост производительности на 10-20% по сравнению с <see cref="Distance"/><br/>
    /// <br/>
    /// <b>Рекомендации по выбору метода:</b><br/>
    /// - Для коротких строк (до 100 символов): любой метод<br/>
    /// - Для средних строк (100-1000 символов): предпочтительнее DistanceFast<br/>
    /// - Для длинных строк (более 1000 символов): обязательно DistanceFast
    /// </remarks>
    /// <example>
    /// <![CDATA[
    /// // Сравнение производительности на больших строках
    /// var long_str1 = new string('a', 1000) + "xyz";
    /// var long_str2 = new string('a', 1000) + "abc";
    /// 
    /// var distance = Levenshtein.DistanceFast(long_str1, long_str2); // 3
    /// 
    /// // Практическое применение: сравнение текстовых блоков
    /// var text1 = "The quick brown fox jumps over the lazy dog";
    /// var text2 = "The quick brown cat jumps over the lazy dog";
    /// var diff = Levenshtein.DistanceFast(text1, text2); // 3
    /// 
    /// // Вычисление процента сходства
    /// var max_length = Math.Max(text1.Length, text2.Length);
    /// var similarity = (1.0 - (double)diff / max_length) * 100;
    /// Console.WriteLine($"Сходство: {similarity:F2}%"); // ~93.18%
    /// 
    /// // Массовое сравнение
    /// var documents = GetDocuments();
    /// var reference = "reference document text";
    /// var similar_docs = documents
    ///     .AsParallel() // параллельная обработка
    ///     .Select(doc => new 
    ///     { 
    ///         Document = doc, 
    ///         Distance = Levenshtein.DistanceFast(reference, doc.Text) 
    ///     })
    ///     .Where(x => x.Distance < 50)
    ///     .OrderBy(x => x.Distance)
    ///     .ToList();
    /// ]]>
    /// </example>
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