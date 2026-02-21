# Particle Swarm Optimization

Модуль реализует алгоритм оптимизации роем частиц (Particle Swarm Optimization, PSO) для решения задач минимизации и максимизации функций. Представлены три реализации для различных размерностей задач: одномерная, двумерная и многомерная.

## Описание алгоритма

Алгоритм оптимизации роем частиц основан на моделировании поведения стаи птиц или рыб, ищущих пищу. Каждая частица движется в пространстве поиска, стремясь к двум целям:
1. **Локальной лучшей позиции** — собственному лучшему решению, найденному частицей
2. **Глобальной лучшей позиции** — лучшему решению, найденному всем роем

## Классы

### 1. `Swarm` — многомерная оптимизация

Класс для оптимизации функций произвольной размерности.

#### Конструктор
```csharp
public Swarm(int ParticleCount = 100)
```

#### Параметры
- `ParticleCount` — количество частиц в рое (по умолчанию 100)

#### Свойства
- **`Inertia`** — вес инерции (0 < значение < 1). Контролирует степень влияния предыдущей скорости частицы
- **`LocalWeight`** — коэффициент локального веса (по умолчанию 1.49445). Контролирует притяжение к собственному лучшему решению
- **`GlobalWeight`** — коэффициент глобального веса (по умолчанию 1.49445). Контролирует притяжение к лучшему решению роя

#### Методы минимизации

##### С явным заданием границ
```csharp
public void Minimize(
    in Func<double[], double> F,
    in double[] MinX,
    in double[] MaxX,
    int IterationCount,
    out double[] X,
    out double Value)
```

##### С использованием интервалов
```csharp
public void Minimize(
    in Func<double[], double> F,
    in Interval[] IntervalX,
    in int IterationCount,
    out double[] X,
    out double Value)
```

#### Методы максимизации

Аналогичны методам минимизации, но ищут максимум функции:
```csharp
public void Maximize(
    in Func<double[], double> F,
    in double[] MinX,
    in double[] MaxX,
    in int IterationCount,
    out double[] X,
    out double Value)
```

#### Пример использования
```csharp
// Минимизация функции Сферы: f(x) = x1^2 + x2^2 + x3^2
var swarm = new Swarm(ParticleCount: 50);
swarm.Inertia = 0.7;
swarm.LocalWeight = 1.49445;
swarm.GlobalWeight = 1.49445;

Func<double[], double> sphereFunction = x => x.Sum(v => v * v);

swarm.Minimize(
    F: sphereFunction,
    MinX: [-5, -5, -5],
    MaxX: [5, 5, 5],
    IterationCount: 100,
    out var bestX,
    out var bestValue
);

Console.WriteLine($"Оптимум найден в точке: ({string.Join(", ", bestX)})");
Console.WriteLine($"Значение функции: {bestValue}");
```

#### Пример с максимизацией
```csharp
// Максимизация функции Растригина (многомодальная функция)
var swarm = new Swarm(ParticleCount: 100);
swarm.Inertia = 0.729;

Func<double[], double> rastrigin = x => 
    -10 * x.Length + x.Sum(v => v * v - 10 * Math.Cos(2 * Math.PI * v));

swarm.Maximize(
    F: x => -rastrigin(x), // инвертируем для максимизации
    MinX: [-5.12, -5.12],
    MaxX: [5.12, 5.12],
    IterationCount: 200,
    out var bestX,
    out var bestValue
);
```

---

### 2. `Swarm1D` — одномерная оптимизация

Оптимизированная реализация для функций одного переменного. Использует скалярные значения вместо массивов для лучшей производительности.

#### Конструктор
```csharp
public Swarm1D(int ParticleCount = 100)
```

#### Свойства
Аналогичны `Swarm`:
- `Inertia`
- `LocalWeight`
- `GlobalWeight`

#### Методы минимизации

##### С явным заданием границ
```csharp
public void Minimize(
    in Func<double, double> F,
    in double MinX,
    in double MaxX,
    in int IterationCount,
    out double X,
    out double Value)
```

##### С использованием интервала
```csharp
public void Minimize(
    in Func<double, double> F,
    in Interval IntervalX,
    int IterationCount,
    out double X,
    out double Value)
```

#### Методы максимизации
```csharp
public void Maximize(
    in Func<double, double> F,
    in double MinX,
    in double MaxX,
    in int IterationCount,
    out double X,
    out double Value)
```

#### Пример использования
```csharp
// Минимизация одномерной функции: f(x) = x^2 - 4x + 5
var swarm1d = new Swarm1D(ParticleCount: 30);
swarm1d.Inertia = 0.7;

Func<double, double> function = x => x * x - 4 * x + 5;

swarm1d.Minimize(
    F: function,
    MinX: -10,
    MaxX: 10,
    IterationCount: 50,
    out var bestX,
    out var bestValue
);

Console.WriteLine($"Минимум в точке x = {bestX}");
Console.WriteLine($"Значение функции = {bestValue}");
// Ожидаемый результат: x ≈ 2, f(x) ≈ 1
```

#### Пример максимизации
```csharp
// Максимизация функции: f(x) = -x^2 + 4x
var swarm1d = new Swarm1D(50);
swarm1d.Inertia = 0.8;

swarm1d.Maximize(
    F: x => -x * x + 4 * x,
    MinX: -10,
    MaxX: 10,
    IterationCount: 100,
    out var bestX,
    out var bestValue
);

Console.WriteLine($"Максимум в точке x = {bestX}"); // ≈ 2
Console.WriteLine($"Значение функции = {bestValue}"); // ≈ 4
```

---

### 3. `Swarm2D` — двумерная оптимизация

Оптимизированная реализация для функций двух переменных. Обеспечивает удобный интерфейс для работы с двумерными задачами.

#### Конструктор
```csharp
public Swarm2D(int ParticleCount = 100)
```

#### Свойства
Аналогичны другим классам:
- `Inertia`
- `LocalWeight`
- `GlobalWeight`

#### Методы минимизации

##### С явным заданием границ
```csharp
public void Minimize(
    in Func<double, double, double> F,
    in double MinX,
    in double MaxX,
    in double MinY,
    in double MaxY,
    in int IterationCount,
    out double X,
    out double Y,
    out double Value)
```

##### С использованием интервалов
```csharp
public void Minimize(
    in Func<double, double, double> F,
    in Interval IntervalX,
    in Interval IntervalY,
    in int IterationCount,
    out double X,
    out double Y,
    out double Value)
```

#### Методы максимизации
```csharp
public void Maximize(
    in Func<double, double, double> F,
    in Interval IntervalX,
    in Interval IntervalY,
    in int IterationCount,
    out double X,
    out double Y,
    out double Value)
```

#### Пример использования
```csharp
// Минимизация двумерной функции Бута: f(x,y) = (x + 2y - 7)^2 + (2x + y - 5)^2
var swarm2d = new Swarm2D(ParticleCount: 50);
swarm2d.Inertia = 0.72;
swarm2d.LocalWeight = 1.49445;
swarm2d.GlobalWeight = 1.49445;

Func<double, double, double> boothFunction = (x, y) =>
{
    var a = x + 2 * y - 7;
    var b = 2 * x + y - 5;
    return a * a + b * b;
};

swarm2d.Minimize(
    F: boothFunction,
    MinX: -10,
    MaxX: 10,
    MinY: -10,
    MaxY: 10,
    IterationCount: 150,
    out var optimalX,
    out var optimalY,
    out var minValue
);

Console.WriteLine($"Оптимум: x = {optimalX:F4}, y = {optimalY:F4}");
Console.WriteLine($"Минимальное значение: {minValue:F4}");
// Ожидаемый результат: x ≈ 1, y ≈ 3, f ≈ 0
```

#### Пример максимизации
```csharp
// Максимизация функции Рози: f(x,y) = (1-x)^2 + 100(y-x^2)^2
// Инвертируем для максимизации (так как нужно найти минимум)
var swarm2d = new Swarm2D(100);
swarm2d.Inertia = 0.729;

Func<double, double, double> rosenbrock = (x, y) =>
{
    var a = 1 - x;
    var b = y - x * x;
    return a * a + 100 * b * b;
};

swarm2d.Maximize(
    F: (x, y) => -rosenbrock(x, y), // инвертируем
    MinX: -2,
    MaxX: 2,
    MinY: -1,
    MaxY: 3,
    IterationCount: 300,
    out var bestX,
    out var bestY,
    out var maxValue
);

Console.WriteLine($"Найденный оптимум: ({bestX:F4}, {bestY:F4})");
Console.WriteLine($"Значение функции: {-maxValue:F4}");
```

---

## Рекомендации по использованию

### Выбор класса
- **`Swarm1D`** — для одномерных функций (быстрее и проще)
- **`Swarm2D`** — для двумерных функций (удобный интерфейс)
- **`Swarm`** — для трёхмерных и многомерных функций

### Настройка параметров
1. **Количество частиц**: обычно 30–100. Больше частиц = медленнее, но часто лучше
2. **Инерция** (0.4–0.9): большое значение = лучше глобальный поиск, малое = лучше локальный поиск
3. **Веса** (обычно 1.4–2.0): контролируют баланс между локальным и глобальным поиском
4. **Количество итераций**: зависит от сложности функции

### Советы по оптимизации
- Начните с небольшого количества итераций для проверки
- Если алгоритм сходится рано, увеличьте инерцию
- Для мультимодальных функций используйте больше частиц
- Используйте ограничения (интервалы) близко к области минимума

## Типы используемых данных
- **`Interval`** — структура для представления интервала [Min, Max]
  - `Normalize(value)` — нормализует значение в пределы интервала

## Примеры тестовых функций

### Функция Сферы (выпуклая)
```csharp
Func<double[], double> sphere = x => x.Sum(v => v * v);
```

### Функция Растригина (многомодальная)
```csharp
Func<double[], double> rastrigin = x => 
    10 * x.Length + x.Sum(v => v * v - 10 * Math.Cos(2 * Math.PI * v));
```

### Функция Розенброка (долина)
```csharp
Func<double, double, double> rosenbrock = (x, y) =>
{
    var a = 1 - x;
    var b = y - x * x;
    return a * a + 100 * b * b;
};
```

---

## Производительность

- **`Swarm1D`** — оптимизирована для одномерных задач, использует скалярные операции
- **`Swarm2D`** — оптимизирована для двумерных задач, использует специализированные операции
- **`Swarm`** — универсальна для произвольной размерности, может быть медленнее для малых размерностей

Для задач с 1–2 переменными рекомендуется использовать специализированные классы `Swarm1D` и `Swarm2D`.

---

## Исключения

- **`ArgumentOutOfRangeException`** — выбрасывается при попытке установить недопустимое значение `Inertia` (должно быть в диапазоне (0, 1))

---

## Примечания

- Алгоритм использует случайные числа, поэтому результаты могут слегка отличаться при разных запусках
- PSO хорошо работает для непрерывных задач оптимизации
- Алгоритм не гарантирует нахождение глобального оптимума, но часто находит хорошие приближения
