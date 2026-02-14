# Планировщики задач (`MathCore.Threading.Tasks.Schedulers`)

Данный каталог содержит набор специализированных реализаций `TaskScheduler`, предназначенных для управления порядком, приоритетами и степенью параллелизма выполнения задач в TPL.

## Состав

- `CurrentThreadTaskScheduler`
  - Выполняет задачи синхронно, немедленно и строго в текущем потоке.

- `LimitedConcurrencyLevelTaskScheduler`
  - Ограничивает количество одновременно выполняемых задач заданным уровнем параллелизма.

- `ReprioritizableTaskScheduler`
  - Поддерживает переприоритизацию уже поставленных в очередь задач (перемещение в начало или конец очереди).

- `QueuedTaskScheduler`
  - Планировщик с поддержкой нескольких очередей, приоритетов и round-robin справедливости.
  - Может работать:
    - на собственных потоках;
    - поверх целевого `TaskScheduler` с ограничением параллелизма.

- `ConcurrentExclusiveInterleave`
  - Координирует выполнение двух планировщиков: параллельного ("читатели") и эксклюзивного ("писатели").
  - Полезно для сценариев типа Reader/Writer.

> Примечание
>
> В каталоге также присутствуют дополнительные планировщики и вспомогательные типы (например, `StaTaskScheduler`, `OrderedTaskScheduler`, `WorkStealingTaskScheduler` и т.д.). Для них документация будет расширена аналогичным образом.

## Примеры использования

### 1) `ConcurrentExclusiveInterleave`: «читатели» и «писатели»

```csharp
var interleave = new ConcurrentExclusiveInterleave();

var read_tasks = Enumerable.Range(0, 4)
    .Select(_ => Task.Factory.StartNew(
        () =>
        {
            // можно выполнять параллельно
        },
        CancellationToken.None,
        TaskCreationOptions.None,
        interleave.ConcurrentTaskScheduler))
    .ToArray();

var write_task = Task.Factory.StartNew(
    () =>
    {
        // выполняется эксклюзивно относительно read_tasks
    },
    CancellationToken.None,
    TaskCreationOptions.None,
    interleave.ExclusiveTaskScheduler);

Task.WaitAll([.. read_tasks, write_task]);
```

### 2) `QueuedTaskScheduler`: приоритеты и несколько очередей

```csharp
using var scheduler = new QueuedTaskScheduler(ThreadCount: 2, ThreadName: "QTS");

var high_priority = scheduler.ActivateNewQueue(priority: -1);
var normal       = scheduler.ActivateNewQueue(priority: 0);

var t1 = Task.Factory.StartNew(
    () => { /* срочно */ },
    CancellationToken.None,
    TaskCreationOptions.None,
    high_priority);

var t2 = Task.Factory.StartNew(
    () => { /* обычно */ },
    CancellationToken.None,
    TaskCreationOptions.None,
    normal);

Task.WaitAll(t1, t2);
```

### 3) `LimitedConcurrencyLevelTaskScheduler`: ограничение параллелизма

```csharp
var limited = new LimitedConcurrencyLevelTaskScheduler(MaximumConcurrencyLevel: 2);

var tasks = Enumerable.Range(0, 10)
    .Select(_ => Task.Factory.StartNew(
        () => Thread.Sleep(100),
        CancellationToken.None,
        TaskCreationOptions.None,
        limited))
    .ToArray();

Task.WaitAll(tasks);
```

### 4) `ReprioritizableTaskScheduler`: изменение положения задачи в очереди

```csharp
var scheduler = new ReprioritizableTaskScheduler();

var t1 = Task.Factory.StartNew(() => { /* позже */ },
    CancellationToken.None, TaskCreationOptions.None, scheduler);

var t2 = Task.Factory.StartNew(() => { /* раньше */ },
    CancellationToken.None, TaskCreationOptions.None, scheduler);

scheduler.Prioritize(t2);

Task.WaitAll(t1, t2);
```

### 5) `CurrentThreadTaskScheduler`: синхронный запуск в текущем потоке

```csharp
var scheduler = new CurrentThreadTaskScheduler();

var task = Task.Factory.StartNew(
    () => 42,
    CancellationToken.None,
    TaskCreationOptions.None,
    scheduler);

var result = task.Result;
```
