namespace MathCore.Threading;

/// <summary>Блокировщик асинхронного доступа к именованному ресурсу</summary>
public sealed class NamedLock : IDisposable
{
    /// <summary>Информация о блокировке ресурса</summary>
    private sealed class ResourceLockInfo
    {
        /// <summary>Признак занятости ресурса</summary>
        public bool IsHeld { get; set; }

        /// <summary>Счетчик активных операций блокировки (включая ожидающие)</summary>
        public int ActiveLocks { get; set; }

        /// <summary>Очередь ожидающих захвата ресурса</summary>
        public Queue<Waiter> Waiters { get; } = new();
    }

    /// <summary>Ожидающий входа в критическую секцию</summary>
    private sealed class Waiter
    {
        public TaskCompletionSource<bool> Tcs { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously); // продолжения вне блокировки
        public volatile bool Canceled; // признак отмены ожидания
    }

    /// <summary>Контроль блокировки</summary>
    /// <remarks>Инициализация нового контроллера блокировки ресурса</remarks>
    /// <param name="Lock">Блокировщик доступа</param>
    /// <param name="ResourceName">Имя блокируемого ресурса</param>
    public readonly struct LockController(NamedLock Lock, string ResourceName)
    {
        /// <summary>Блокировщик доступа</summary>
        private readonly NamedLock _Lock = Lock;

        /// <summary>разблокировать ресурс</summary>
        public void Unlock() => _Lock.Unlock(ResourceName);

        /// <summary>разблокировать ресурс</summary>
        public Task UnlockAsync(CancellationToken Cancel = default) => _Lock.UnlockAsync(ResourceName, Cancel);

        /// <summary>Разрушение блокировки</summary>
        public void Dispose() => Unlock();

        /// <summary>Разрушение блокировки</summary>
        public Task DisposeAsync() => UnlockAsync();
    }

    /// <summary>Блокировщик именованных ресурсов по умолчанию</summary>
    public static NamedLock Default { get; } = new();

    /// <summary>Семафор блокировки доступа к словарю заблокированных ресурсов</summary>
    private SemaphoreSlim _Lock = new(1, 1);

    /// <summary>Словарь семафоров заблокированных именованных ресурсов</summary>
    private readonly Dictionary<string, ResourceLockInfo> _Resources = [];

    /// <summary>Заблокировать ресурс и получить контроллер блокировки для конструкции using</summary>
    /// <param name="ResourceName">Имя блокируемого ресурса</param>
    /// <returns>Контроллер блокировки указанного ресурса</returns>
    public LockController this[string ResourceName]
    {
        get
        {
            Lock(ResourceName);
            return new(this, ResourceName);
        }
    }

    /// <summary>Заблокировать ресурс</summary>
    /// <param name="Resource">Имя блокируемого ресурса</param>
    public void Lock(string Resource)
    {
        _Lock.Wait();

        if (_Resources.TryGetValue(Resource, out var info))
        {
            info.ActiveLocks++;
            if (!info.IsHeld && info.Waiters.Count == 0)
            {
                info.IsHeld = true; // ресурс свободен, захватываем сразу
                _Lock.Release();
                return;
            }

            var waiter = new Waiter();
            info.Waiters.Enqueue(waiter);
            _Lock.Release();

            // Блокирующее ожидание выдачи доступа
            waiter.Tcs.Task.GetAwaiter().GetResult();
        }
        else
        {
            _Resources.Add(Resource, new() { IsHeld = true, ActiveLocks = 1 });
            _Lock.Release();
        }
    }

    /// <summary>Заблокировать ресурс асинхронно</summary>
    /// <param name="Resource">Имя блокируемого ресурса</param>
    /// <param name="Cancel">Флаг отмены асинхронной операции</param>
    /// <returns>Задача ожидания блокировки указанного именованного ресурса</returns>
    public async Task LockAsync(string Resource, CancellationToken Cancel = default)
    {
        await _Lock.WaitAsync(Cancel).ConfigureAwait(false);

        if (_Resources.TryGetValue(Resource, out var info))
        {
            info.ActiveLocks++;

            // Если ресурс свободен и нет очереди ожидания, забираем сразу
            if (!info.IsHeld && info.Waiters.Count == 0)
            {
                info.IsHeld = true;
                _Lock.Release();
                return;
            }

            var waiter = new Waiter();
            info.Waiters.Enqueue(waiter);
            _Lock.Release();

            using var _ = Cancel.CanBeCanceled
                ? Cancel.Register(() => CancelWaiter(Resource, waiter))
                : default;

            await waiter.Tcs.Task.ConfigureAwait(false);
        }
        else
        {
            _Resources.Add(Resource, new() { IsHeld = true, ActiveLocks = 1 });
            _Lock.Release();
        }
    }

    private void CancelWaiter(string Resource, Waiter Waiter)
    {
        _Lock.Wait();
        if (!_Resources.TryGetValue(Resource, out var info))
        {
            _Lock.Release();
            Waiter.Tcs.TrySetCanceled();
            return;
        }

        if (!Waiter.Tcs.Task.IsCompleted)
        {
            Waiter.Canceled = true;
            info.ActiveLocks--;
        }

        var remove = info.ActiveLocks == 0 && info.Waiters.All(w => w.Canceled);

        if (remove)
            _Resources.Remove(Resource);

        _Lock.Release();

        Waiter.Tcs.TrySetCanceled();
    }

    /// <summary>Разблокировать указанный именованный ресурс</summary>
    /// <param name="Resource">Имя блокируемого ресурса</param>
    public void Unlock(string Resource)
    {
        _Lock.Wait();

        if (!_Resources.TryGetValue(Resource, out var info))
        {
            _Lock.Release();
            return;
        }

        info.ActiveLocks--;

        // Ищем следующего некорректно отменённого ожидателя
        while (info.Waiters.Count > 0)
        {
            var next = info.Waiters.Dequeue();
            if (next.Canceled)
                continue; // пропускаем отменённых

            // Передаём владение следующему ожидающему
            info.IsHeld = true;
            _Lock.Release();
            next.Tcs.TrySetResult(true);
            return;
        }

        // Очередь пуста
        info.IsHeld = false;
        var should_remove = info.ActiveLocks == 0;
        if (should_remove)
            _Resources.Remove(Resource);

        _Lock.Release();
    }

    /// <summary>Разблокировать указанный именованный ресурс асинхронно</summary>
    /// <param name="Resource">Имя блокируемого ресурса</param>
    /// <param name="Cancel">Флаг отмены асинхронно операции</param>
    /// <returns>Задача завершения процесса разблокировки указанного ресурса</returns>
    public Task UnlockAsync(string Resource, CancellationToken Cancel = default)
    {
        Unlock(Resource); // асинхронных операций здесь нет
        return Task.CompletedTask;
    }

    /// <summary>Уничтожить блокировщик ресурсов и освободить все блокировки</summary>
    public void Dispose()
    {
        _Lock.Wait();

        foreach (var (name, info) in _Resources.ToArray())
        {
            // Отменяем всех ожидающих
            while (info.Waiters.Count > 0)
            {
                var w = info.Waiters.Dequeue();
                // выставляем отмену/ошибку вне блокировки, но флаг сразу
                w.Canceled = true;
            }

            info.ActiveLocks = 0;
            info.IsHeld = false;

            _Resources.Remove(name);
        }

        _Lock.Release();
        _Lock.Dispose();
    }
}