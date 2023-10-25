using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathCore.Threading;

/// <summary>Блокировщик асинхронного доступа к именованному ресурсу</summary>
public sealed class NamedLock : IDisposable
{
    /// <summary>Контроль блокировки</summary>
    public readonly struct LockController
    {
        /// <summary>Блокировщик доступа</summary>
        private readonly NamedLock _Lock;

        /// <summary>Имя блокируемого ресурса</summary>
        private readonly string _ResourceName;

        /// <summary>Инициализация нового контроллера блокировки ресурса</summary>
        /// <param name="Lock">Блокировщик доступа</param>
        /// <param name="ResourceName">Имя блокируемого ресурса</param>
        public LockController(NamedLock Lock, string ResourceName)
        {
            _Lock = Lock;
            _ResourceName = ResourceName;
        }

        /// <summary>разблокировать ресурс</summary>
        public void Unlock() => _Lock.Unlock(_ResourceName);

        /// <summary>разблокировать ресурс</summary>
        public Task UnlockAsync(CancellationToken Cancel = default) => _Lock.UnlockAsync(_ResourceName, Cancel);

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
    private readonly Dictionary<string, SemaphoreSlim> _Resources = new();

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

        if (_Resources.TryGetValue(Resource, out var resource_lock))
        {
            _Lock.Release();
            resource_lock.Wait();
        }
        else
        {
            _Resources.Add(Resource, new(0, 1));
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

        if (_Resources.TryGetValue(Resource, out var resource_lock))
        {
            _Lock.Release();
            await resource_lock.WaitAsync(Cancel).ConfigureAwait(false);
        }
        else
        {
            _Resources.Add(Resource, new(0, 1));
            _Lock.Release();
        }
    }

    /// <summary>Разблокировать указанный именованный ресурс</summary>
    /// <param name="Resource">Имя блокируемого ресурса</param>
    public void Unlock(string Resource)
    {
        _Lock.Wait();

        if (!_Resources.TryGetValue(Resource, out var resource_lock))
        {
            _Lock.Release();
            return;
        }

        resource_lock.Release();

        if (_Lock.CurrentCount == 1)
        {
            _Resources.Remove(Resource);
            resource_lock.Dispose();
        }

        _Lock.Release();
    }

    /// <summary>Разблокировать указанный именованный ресурс асинхронно</summary>
    /// <param name="Resource">Имя блокируемого ресурса</param>
    /// <param name="Cancel">Флаг отмены асинхронно операции</param>
    /// <returns>Задача завершения процесса разблокировки указанного ресурса</returns>
    public async Task UnlockAsync(string Resource, CancellationToken Cancel = default)
    {
        await _Lock.WaitAsync(Cancel).ConfigureAwait(false);

        if (!_Resources.TryGetValue(Resource, out var resource_lock))
        {
            _Lock.Release();
            return;
        }

        resource_lock.Release();

        if (_Lock.CurrentCount == 1)
        {
            _Resources.Remove(Resource);
            resource_lock.Dispose();
        }

        _Lock.Release();
    }

    /// <summary>Уничтожить блокировщик ресурсов и освободить все блокировки</summary>
    public void Dispose()
    {
        _Lock.Wait();

        foreach (var resource_lock in _Resources.Values)
        {
            resource_lock.Release();
            resource_lock.Dispose();
        }

        _Resources.Clear();

        _Lock.Release();
        _Lock.Dispose();
    }
}