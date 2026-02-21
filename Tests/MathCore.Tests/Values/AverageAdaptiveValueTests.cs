using MathCore.Values;

namespace MathCore.Tests.Values;

/// <summary>Тесты адаптивного скользящего среднего с обнаружением скачков статистики</summary>
/// <remarks>
/// Набор тестов проверяет корректность работы алгоритма адаптивного усреднения:
/// <list type="bullet">
///     <item>Базовые тесты конструкторов и свойств</item>
///     <item>Стационарные процессы с малыми флуктуациями</item>
///     <item>Обнаружение резких скачков статистики</item>
///     <item>Реальные сценарии: чтение файлов, индикаторы приборов</item>
///     <item>Стресс-тестирование с множественными переходами</item>
/// </list>
/// </remarks>
[TestClass]
public class AverageAdaptiveValueTests
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    public void Ctor_Default_Test()
    {
        var average = new AverageAdaptiveValue();

        Assert.AreEqual(0.1, average.BaseFactor);
        Assert.AreEqual(0.5, average.FastFactor);
        Assert.AreEqual(3.0, average.JumpThreshold);
        Assert.AreEqual(5, average.MinSamplesForDetection);
        Assert.AreEqual(0, average.ValuesCount);
        Assert.IsTrue(double.IsNaN(average.StartValue));
    }

    [TestMethod]
    public void Ctor_WithStartValue_Test()
    {
        const double start_value = 100.0;

        var average = new AverageAdaptiveValue(StartValue: start_value);

        Assert.AreEqual(start_value, average.Value);
        Assert.AreEqual(start_value, average.StartValue);
        Assert.AreEqual(1, average.ValuesCount);
    }

    [TestMethod]
    public void Ctor_WithCustomParameters_Test()
    {
        var average = new AverageAdaptiveValue(
            BaseFactor: 0.15,
            FastFactor: 0.6,
            JumpThreshold: 2.5,
            MinSamplesForDetection: 10);

        Assert.AreEqual(0.15, average.BaseFactor);
        Assert.AreEqual(0.6, average.FastFactor);
        Assert.AreEqual(2.5, average.JumpThreshold);
        Assert.AreEqual(10, average.MinSamplesForDetection);
    }

    [TestMethod]
    public void Ctor_InvalidBaseFactor_ThrowsException() => Assert.That
        .Method(() => new AverageAdaptiveValue(BaseFactor: 1.5))
        .Throw<ArgumentOutOfRangeException>();

    [TestMethod]
    public void Ctor_InvalidFastFactor_ThrowsException() => Assert.That
        .Method(() => new AverageAdaptiveValue(FastFactor: -0.1))
        .Throw<ArgumentOutOfRangeException>();

    [TestMethod]
    public void AddValue_FirstValue_InitializesCorrectly()
    {
        var average = new AverageAdaptiveValue();
        const double first_value = 50.0;

        var result = average.AddValue(first_value);

        Assert.AreEqual(first_value, result);
        Assert.AreEqual(first_value, average.Value);
        Assert.AreEqual(1, average.ValuesCount);
    }

    [TestMethod]
    public void Reset_RestoresInitialState()
    {
        var average = new AverageAdaptiveValue(StartValue: 100.0);
        average.AddValue(150.0);
        average.AddValue(200.0);

        average.Reset();

        Assert.AreEqual(100.0, average.Value);
        Assert.AreEqual(1, average.ValuesCount);
    }

    /// <summary>Тест усреднения стационарного процесса с малыми флуктуациями</summary>
    /// <remarks>
    /// Сценарий: поток данных со стабильным математическим ожиданием (100) и случайным шумом (±5).
    /// Алгоритм должен плавно усреднить флуктуации, используя медленный BaseFactor.
    /// Проверяется стабильность среднего и корректность расчёта стандартного отклонения.
    /// </remarks>
    [TestMethod]
    public void StationaryProcess_SmoothAveraging()
    {
        // Инициализация с параметрами для стационарного процесса
        var average = new AverageAdaptiveValue(
            BaseFactor: 0.1,               // Медленное сглаживание для стабильного процесса
            FastFactor: 0.5,               // Резерв для обнаружения скачков
            JumpThreshold: 3.0,            // Порог обнаружения = 3 стандартных отклонения
            MinSamplesForDetection: 5);    // Начинаем обнаруживать скачки после 5 точек

        var random = new Random(42);
        const double mean = 100.0;
        const double noise_amplitude = 5.0;

        TestContext.WriteLine("=== Тест стационарного процесса ===");
        TestContext.WriteLine("Параметры: среднее=100, амплитуда шума=±5");
        TestContext.WriteLine($"{"Итерация",-10} {"Входное",-12} {"Среднее",-12} {"σ",-10}");
        TestContext.WriteLine(new string('-', 50));

        // Подаём 20 значений с нормальными флуктуациями
        for (var i = 0; i < 20; i++)
        {
            // Генерация случайного шума в диапазоне [-5, +5]
            var noise = (random.NextDouble() - 0.5) * 2 * noise_amplitude;
            var value = mean + noise;

            average.AddValue(value);

            TestContext.WriteLine(
                $"{i + 1,-10} {value,-12:F2} {average.Value,-12:F2} {average.StandardDeviation,-10:F2}");
        }

        // Проверка, что среднее близко к истинному среднему
        Assert.AreEqual(mean, average.Value, 10.0, "Среднее должно быть близко к 100");

        // Проверка, что стандартное отклонение соответствует ожидаемому
        Assert.IsGreaterThan(0, average.StandardDeviation, "Стандартное отклонение должно быть положительным");
    }

    /// <summary>Тест обнаружения резкого скачка статистики процесса</summary>
    /// <remarks>
    /// Сценарий: процесс меняет своё математическое ожидание со 100 на 200 на 15-й итерации.
    /// Алгоритм должен обнаружить скачок (отклонение > 3σ) и переключиться на быстрый FastFactor
    /// для ускоренной адаптации к новому уровню. Это ключевой тест адаптивности алгоритма.
    /// </remarks>
    [TestMethod]
    public void JumpDetection_FastAdaptation()
    {
        // Инициализация с чувствительным порогом обнаружения
        var average = new AverageAdaptiveValue(
            BaseFactor: 0.1,               // Медленное сглаживание в норме
            FastFactor: 0.6,               // Быстрая адаптация при скачке
            JumpThreshold: 2.5,            // Снижен порог для более чувствительного обнаружения
            MinSamplesForDetection: 5);    // Небольшое окно для быстрого старта обнаружения

        var random = new Random(42);
        const double initial_mean = 100.0;  // Начальное среднее
        const double jump_mean = 200.0;     // Новое среднее после скачка
        const double noise_amplitude = 5.0;

        TestContext.WriteLine("=== Тест обнаружения скачка ===");
        TestContext.WriteLine("Параметры: начальное среднее=100, скачок до 200 на итерации 15");
        TestContext.WriteLine($"{"Итерация",-10} {"Входное",-12} {"Среднее",-12} {"σ",-10} {"Δ/σ",-10}");
        TestContext.WriteLine(new string('-', 60));

        for (var i = 0; i < 30; i++)
        {
            var noise = (random.NextDouble() - 0.5) * 2 * noise_amplitude;

            // Резкая смена среднего на 15-й итерации
            var current_mean = i < 15 ? initial_mean : jump_mean;
            var value = current_mean + noise;

            // Сохраняем текущие значения для расчёта нормализованного отклонения
            var old_value = average.Value;
            var old_std = average.StandardDeviation;

            average.AddValue(value);

            // Вычисляем нормализованное отклонение (Δ/σ) для визуализации обнаружения скачка
            var deviation_normalized = old_std > 1e-10
                ? Math.Abs(value - old_value) / old_std
                : 0;

            // Маркер для визуального выделения момента скачка
            var marker = i == 15 ? " <<< СКАЧОК" : "";

            TestContext.WriteLine(
                $"{i + 1,-10} {value,-12:F2} {average.Value,-12:F2} {average.StandardDeviation,-10:F2} {deviation_normalized,-10:F2}{marker}");
        }

        // После скачка среднее должно быстро адаптироваться к новому уровню
        Assert.IsGreaterThan(180, average.Value, "После скачка среднее должно приблизиться к 200");
    }

    /// <summary>Тест моделирования скорости чтения файла с сетевого диска</summary>
    /// <remarks>
    /// Реальный сценарий: чтение файла буферами по 4 КБ с нестабильного сетевого диска.
    /// Процесс проходит три фазы:
    /// 1. Быстрое чтение (10 МБ/с) — стабильное соединение
    /// 2. Сетевая задержка (2 МБ/с) — нагрузка на сеть
    /// 3. Восстановление (9 МБ/с) — стабилизация
    /// Алгоритм должен быстро отслеживать изменения скорости для точной оценки ETA.
    /// </remarks>
    [TestMethod]
    public void NetworkFileReading_SpeedAveraging()
    {
        // Параметры настроены для задачи мониторинга скорости чтения
        var speed_average = new AverageAdaptiveValue(
            BaseFactor: 0.15,              // Умеренное сглаживание флуктуаций скорости
            FastFactor: 0.5,               // Быстрая реакция на резкое падение скорости
            JumpThreshold: 3.0,            // Стандартный порог для обнаружения сетевых проблем
            MinSamplesForDetection: 10);   // Накопление статистики перед обнаружением скачков

        var random = new Random(42);

        // Моделируем три фазы чтения:
        // 1. Быстрое чтение (10 МБ/с) - итерации 0-19
        // 2. Сетевая задержка (2 МБ/с) - итерации 20-39
        // 3. Восстановление скорости (9 МБ/с) - итерации 40-59

        TestContext.WriteLine("=== Моделирование чтения файла с сетевого диска ===");
        TestContext.WriteLine($"{"Итерация",-10} {"Скорость",-15} {"Среднее",-15} {"Фаза",-20}");
        TestContext.WriteLine(new string('-', 65));

        for (var i = 0; i < 60; i++)
        {
            double base_speed;
            string phase;

            // Определение текущей фазы процесса
            if (i < 20)
            {
                base_speed = 10.0; // МБ/с - оптимальная скорость
                phase = "Быстрое чтение";
            }
            else if (i < 40)
            {
                base_speed = 2.0;  // МБ/с - резкое падение из-за нагрузки на сеть
                phase = "Сетевая задержка";
            }
            else
            {
                base_speed = 9.0;  // МБ/с - восстановление после устранения нагрузки
                phase = "Восстановление";
            }

            // Добавление небольших флуктуаций, характерных для сетевого I/O
            var noise = (random.NextDouble() - 0.5) * 0.5;
            var speed = base_speed + noise;

            speed_average.AddValue(speed);

            // Выводим промежуточные результаты каждые 5 итераций и при смене фазы
            if (i % 5 == 0 || i == 20 || i == 40)
            {
                TestContext.WriteLine(
                    $"{i,-10} {speed,-15:F2} {speed_average.Value,-15:F2} {phase,-20}");
            }
        }

        TestContext.WriteLine($"\nИтоговая статистика:");
        TestContext.WriteLine($"  Среднее: {speed_average.Value:F2} МБ/с");
        TestContext.WriteLine($"  Min: {speed_average.Min:F2} МБ/с");
        TestContext.WriteLine($"  Max: {speed_average.Max:F2} МБ/с");
        TestContext.WriteLine($"  σ: {speed_average.StandardDeviation:F2}");

        // Проверяем, что финальное среднее близко к последней фазе
        Assert.IsTrue(
            speed_average.Value is > 7.0 and < 11.0,
            "Финальное среднее должно быть близко к скорости восстановления");
    }

    /// <summary>Тест моделирования индикатора вертикальной скорости самолёта</summary>
    /// <remarks>
    /// Реальный сценарий: авиационный прибор (вариометр) для отображения вертикальной скорости.
    /// Требования к алгоритму:
    /// - Плавная индикация в стационарном режиме (гашение колебаний стрелки)
    /// - Быстрая реакция при смене режима полёта (набор высоты, снижение)
    /// Процесс моделирует три режима: горизонтальный полёт → набор высоты → снижение.
    /// </remarks>
    [TestMethod]
    public void AircraftVerticalSpeed_Indicator()
    {
        // Параметры настроены для авиационного прибора
        var vspeed_indicator = new AverageAdaptiveValue(
            BaseFactor: 0.05,              // Очень плавное сглаживание для устойчивой индикации
            FastFactor: 0.6,               // Высокая скорость реакции на смену режима полёта
            JumpThreshold: 2.5,            // Чувствительный порог для быстрого обнаружения
            MinSamplesForDetection: 5);    // Малое окно для оперативной реакции

        var random = new Random(42);

        // Моделируем изменение вертикальной скорости:
        // 1. Горизонтальный полёт (0 м/с) - итерации 0-14
        // 2. Набор высоты (5 м/с) - итерации 15-34
        // 3. Снижение (-3 м/с) - итерации 35-49

        TestContext.WriteLine("=== Моделирование индикатора вертикальной скорости ---");
        TestContext.WriteLine($"{"Итерация",-10} {"V_верт",-12} {"Индикатор",-12} {"Режим полёта",-20}");
        TestContext.WriteLine(new string('-', 60));

        for (var i = 0; i < 50; i++)
        {
            double base_vspeed;
            string mode;

            // Определение текущего режима полёта
            if (i < 15)
            {
                base_vspeed = 0.0;  // Горизонтальный полёт
                mode = "Горизонтальный";
            }
            else if (i < 35)
            {
                base_vspeed = 5.0;  // Набор высоты - резкая смена режима
                mode = "Набор высоты";
            }
            else
            {
                base_vspeed = -3.0; // Снижение - ещё одна смена режима
                mode = "Снижение";
            }

            // Добавление небольших инструментальных флуктуаций
            var noise = (random.NextDouble() - 0.5) * 0.3;
            var vspeed = base_vspeed + noise;

            vspeed_indicator.AddValue(vspeed);

            // Выводим ключевые точки: каждые 5 итераций и моменты смены режима
            if (i % 5 == 0 || i == 15 || i == 35)
                TestContext.WriteLine(
                    $"{i,-10} {vspeed,-12:F2} {vspeed_indicator.Value,-12:F2} {mode,-20}");
        }

        TestContext.WriteLine($"\nИтоговая статистика:");
        TestContext.WriteLine($"  Текущее показание: {vspeed_indicator.Value:F2} м/с");
        TestContext.WriteLine($"  Min: {vspeed_indicator.Min:F2} м/с");
        TestContext.WriteLine($"  Max: {vspeed_indicator.Max:F2} м/с");

        // Проверяем, что индикатор показывает снижение
        Assert.IsLessThan(-1.0, vspeed_indicator.Value, "Индикатор должен показывать снижение");
    }

    /// <summary>Тест расчёта Min/Max</summary>
    [TestMethod]
    public void MinMax_Calculation()
    {
        var average = new AverageAdaptiveValue();

        var values = new[] { 10.0, 5.0, 15.0, 3.0, 20.0 };

        foreach (var value in values)
            average.AddValue(value);

        Assert.AreEqual(3.0, average.Min);
        Assert.AreEqual(20.0, average.Max);

        var interval = average.Interval;
        Assert.AreEqual(3.0, interval.Min);
        Assert.AreEqual(20.0, interval.Max);
    }

    /// <summary>Тест деконструкции</summary>
    [TestMethod]
    public void Deconstruct_ReturnsCorrectValues()
    {
        var average = new AverageAdaptiveValue(StartValue: 100.0);
        average.AddValue(110.0);
        average.AddValue(90.0);

        var (mean, variance) = average;

        Assert.AreEqual(average.Value, mean);
        Assert.AreEqual(average.Dispersion, variance);
    }

    /// <summary>Тест операторов неявного приведения</summary>
    [TestMethod]
    public void ImplicitConversion_ToDouble()
    {
        var average = new AverageAdaptiveValue(StartValue: 42.0);

        double value = average;

        Assert.AreEqual(42.0, value);
    }

    [TestMethod]
    public void ImplicitConversion_FromDouble()
    {
        AverageAdaptiveValue average = 42.0;

        Assert.AreEqual(42.0, average.Value);
    }

    [TestMethod]
    public void ImplicitConversion_ToInterval()
    {
        var average = new AverageAdaptiveValue();
        average.AddValue(10.0);
        average.AddValue(20.0);
        average.AddValue(15.0);

        Interval interval = average;

        Assert.AreEqual(10.0, interval.Min);
        Assert.AreEqual(20.0, interval.Max);
    }

    /// <summary>Тест свойств после множественных операций</summary>
    [TestMethod]
    public void Properties_AfterMultipleOperations()
    {
        var average = new AverageAdaptiveValue(
            StartValue: 100.0,
            BaseFactor: 0.15,
            FastFactor: 0.6,
            JumpThreshold: 2.5,
            MinSamplesForDetection: 10);

        average.AddValue(110.0);
        average.AddValue(120.0);
        average.AddValue(130.0);

        Assert.AreEqual(0.15, average.BaseFactor);
        Assert.AreEqual(0.6, average.FastFactor);
        Assert.AreEqual(2.5, average.JumpThreshold);
        Assert.AreEqual(10, average.MinSamplesForDetection);
        Assert.AreEqual(100.0, average.StartValue);
        Assert.IsGreaterThan(1, average.ValuesCount);

        // Проверка возможности изменения параметров во время работы
        average.BaseFactor = 0.2;
        Assert.AreEqual(0.2, average.BaseFactor);
    }

    /// <summary>Стресс-тест с большим количеством значений и множественными скачками</summary>
    /// <remarks>
    /// Экстремальный сценарий: поток данных с частыми резкими изменениями статистики.
    /// Моделирует 500 измерений с 5 различными уровнями средних значений.
    /// Проверяет устойчивость алгоритма при множественных переходах и накоплении статистики.
    /// Важен для проверки отсутствия деградации производительности и точности.
    /// </remarks>
    [TestMethod]
    public void StressTest_MultipleJumps()
    {
        // Конфигурация для стресс-тестирования
        var average = new AverageAdaptiveValue(
            BaseFactor: 0.1,               // Стандартное медленное сглаживание
            FastFactor: 0.5,               // Стандартная быстрая адаптация
            JumpThreshold: 3.0,            // Стандартный порог обнаружения
            MinSamplesForDetection: 10);   // Достаточное окно для стабильной работы

        var random = new Random(42);

        // Пять различных уровней для множественных скачков
        var jump_points = new[] { 0, 100, 200, 300, 400 };
        var jump_values = new[] { 50.0, 150.0, 75.0, 200.0, 100.0 };

        TestContext.WriteLine("=== Стресс-тест с множественными скачками ===");
        TestContext.WriteLine($"Общее количество итераций: 500");
        TestContext.WriteLine($"{"Итерация",-10} {"Целевое",-12} {"Среднее",-12}");
        TestContext.WriteLine(new string('-', 40));

        for (var i = 0; i < 500; i++)
        {
            // Определение текущего сегмента (каждые 100 итераций — новый уровень)
            var segment = i / 100;
            var target_mean = jump_values[segment];

            // Добавление случайного шума к целевому значению
            var noise = (random.NextDouble() - 0.5) * 5.0;
            var value = target_mean + noise;

            average.AddValue(value);

            // Вывод промежуточных результатов каждые 50 итераций
            if (i % 50 == 0)
                TestContext.WriteLine($"{i,-10} {target_mean,-12:F2} {average.Value,-12:F2}");
        }

        TestContext.WriteLine($"\nИтоговая статистика:");
        TestContext.WriteLine($"  Обработано значений: {average.ValuesCount}");
        TestContext.WriteLine($"  Финальное среднее: {average.Value:F2}");
        TestContext.WriteLine($"  Min: {average.Min:F2}");
        TestContext.WriteLine($"  Max: {average.Max:F2}");

        // Проверка корректности обработки большого количества данных
        Assert.AreEqual(500, average.ValuesCount);

        // Проверка, что алгоритм адаптировался к последнему уровню (100.0)
        Assert.IsTrue(
            average.Value is > 90 and < 110,
            "Финальное среднее должно быть близко к последнему целевому значению");
    }
}
