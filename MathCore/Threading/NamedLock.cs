namespace MathCore.Threading;

/// <summary>Блокировщик асинхронного доступа к именованному ресурсу</summary>
public sealed class NamedLock : IDisposable
{
    /// <summary>Информация о блокировке ресурса</summary>
    private sealed class ResourceLockInfo
    {
        /// <summary>Семафор для блокировки доступа</summary>
        public required SemaphoreSlim Semaphore { get; init; }

        /// <summary>Счетчик активных блокировок</summary>
        public int ActiveLocks { get; set; }
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

        if (_Resources.TryGetValue(Resource, out var lock_info))
        {
            lock_info.ActiveLocks++;
            _Lock.Release();
            lock_info.Semaphore.Wait();
        }
        else
        {
            _Resources.Add(Resource, new() { Semaphore = new(0, 1), ActiveLocks = 1 });
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

        if (_Resources.TryGetValue(Resource, out var lock_info))
        {
            lock_info.ActiveLocks++;
            _Lock.Release();
            await lock_info.Semaphore.WaitAsync(Cancel).ConfigureAwait(false);
        }
        else
        {
            _Resources.Add(Resource, new() { Semaphore = new(0, 1), ActiveLocks = 1 });
            _Lock.Release();
        }
    }

    /// <summary>Разблокировать указанный именованный ресурс</summary>
    /// <param name="Resource">Имя блокируемого ресурса</param>
    public void Unlock(string Resource)
    {
        _Lock.Wait();

        if (!_Resources.TryGetValue(Resource, out var lock_info))
        {
            _Lock.Release();
            return;
        }

        lock_info.ActiveLocks--;
        var should_remove = lock_info.ActiveLocks == 0;

        if (should_remove)
        {
            _Resources.Remove(Resource);
        }

        _Lock.Release();

        lock_info.Semaphore.Release();

        if (should_remove)
        {
            lock_info.Semaphore.Dispose();
        }
    }

    /// <summary>Разблокировать указанный именованный ресурс асинхронно</summary>
    /// <param name="Resource">Имя блокируемого ресурса</param>
    /// <param name="Cancel">Флаг отмены асинхронно операции</param>
    /// <returns>Задача завершения процесса разблокировки указанного ресурса</returns>
    public async Task UnlockAsync(string Resource, CancellationToken Cancel = default)
    {
        _Lock.Wait();

        if (!_Resources.TryGetValue(Resource, out var lock_info))
        {
            _Lock.Release();
            return;
        }

        lock_info.ActiveLocks--;
        var should_remove = lock_info.ActiveLocks == 0;

        if (should_remove)
        {
            _Resources.Remove(Resource);
        }

        _Lock.Release();

        lock_info.Semaphore.Release();

        if (should_remove)
        {
            lock_info.Semaphore.Dispose();
        }
    }

    /// <summary>Уничтожить блокировщик ресурсов и освободить все блокировки</summary>
    public void Dispose()
    {
        _Lock.Wait();

        foreach (var lock_info in _Resources.Values)
        {
            lock_info.Semaphore.Release();
            lock_info.Semaphore.Dispose();
        }

        _Resources.Clear();

        _Lock.Release();
        _Lock.Dispose();
    }
}