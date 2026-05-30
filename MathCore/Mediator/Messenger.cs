using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MathCore.Mediator;

/// <summary>Реализация простой шины сообщений для регистрации и вызова обработчиков по типу и адресу</summary>
public class Messenger : IMessenger
{
    private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, List<Delegate>>> _Handlers = new();

    /// <summary>Планировщик задач, в контексте которого выполняются асинхронные вызовы обработчиков</summary>
    public TaskScheduler TaskScheduler { get; set; } = TaskScheduler.Default;

    /// <summary>Добавляет обработчик для сообщений типа <typeparamref name="T"/> на адрес по умолчанию</summary>
    /// <param name="Handler">Обработчик события</param>
    public void AddHandle<T>(EventHandler<T> Handler) => AddHandle("", Handler);

    /// <summary>Добавляет обработчик для сообщений типа <typeparamref name="T"/> на указанный адрес</summary>
    /// <param name="Address">Адрес/топик сообщения</param>
    /// <param name="Handler">Обработчик события</param>
    public void AddHandle<T>(string? Address, EventHandler<T> Handler)
    {
        var messages_type = typeof(T);
        var handlers = _Handlers.GetOrAdd(messages_type, _ => new ConcurrentDictionary<string, List<Delegate>>());
        var address_handlers = handlers.GetOrAdd(Address ?? string.Empty, _ => new List<Delegate>());
        lock (address_handlers)
            address_handlers.Add(Handler);
    }

    /// <summary>Удаляет обработчик для сообщений типа <typeparamref name="T"/>, зарегистрированный на любом адресе</summary>
    /// <param name="Handler">Обработчик для удаления</param>
    /// <returns>True если обработчик был найден и удалён</returns>
    public bool RemoveHandler<T>(EventHandler<T> Handler)
    {
        var messages_type = typeof(T);
        if (!_Handlers.TryGetValue(messages_type, out var handlers)) return false;

        var found = false;
        foreach (var kv in handlers.ToArray())
        {
            var address = kv.Key;
            var address_handlers = kv.Value;
            lock (address_handlers)
            {
                if (address_handlers.Remove(Handler))
                {
                    found = true;
                    if (address_handlers.Count == 0)
                        handlers.TryRemove(address, out _);
                    break;
                }
            }
        }

        if (handlers.IsEmpty)
            _Handlers.TryRemove(messages_type, out _);

        return found;
    }

    /// <summary>Удаляет обработчик для сообщений типа <typeparamref name="T"/>, зарегистрированный на указанном адресе</summary>
    /// <param name="Address">Адрес/топик</param>
    /// <param name="Handler">Обработчик для удаления</param>
    /// <returns>True если обработчик был найден и удалён</returns>
    public bool RemoveHandler<T>(string Address, EventHandler<T> Handler)
    {
        var messages_type = typeof(T);

        if (!_Handlers.TryGetValue(messages_type, out var handlers)) return false;
        if (!handlers.TryGetValue(Address ?? string.Empty, out var address_handlers)) return false;

        lock (address_handlers)
            return address_handlers.Remove(Handler);
    }

    /// <summary>Синхронно отправляет сообщение типа <typeparamref name="T"/> на адрес по умолчанию</summary>
    /// <param name="Sender">Отправитель сообщения</param>
    /// <param name="Message">Тело сообщения</param>
    public void Send<T>(object Sender, T Message) => Send(Sender, string.Empty, Message);

    /// <summary>Синхронно отправляет сообщение типа <typeparamref name="T"/> на указанный адрес</summary>
    /// <param name="Sender">Отправитель сообщения</param>
    /// <param name="Address">Адрес/топик</param>
    /// <param name="Message">Тело сообщения</param>
    public void Send<T>(object Sender, string Address, T Message)
    {
        var message_type = typeof(T);
        if (!_Handlers.TryGetValue(message_type, out var handlers)) return;
        if (!handlers.TryGetValue(Address, out var address_handlers)) return;

        EventHandler<T>[] delegates;
        lock (address_handlers) delegates = address_handlers.Cast<EventHandler<T>>().ToArray();

        foreach (var handler in delegates)
            handler(Sender, Message);
    }

    /// <summary>Асинхронно отправляет сообщение типа <typeparamref name="T"/> на адрес по умолчанию</summary>
    /// <param name="Sender">Отправитель сообщения</param>
    /// <param name="Message">Тело сообщения</param>
    /// <param name="Cancel">Токен отмены</param>
    public Task SendAsync<T>(object Sender, T Message, CancellationToken Cancel = default) => SendAsync(Sender, string.Empty, Message, Cancel);

    /// <summary>Асинхронно отправляет сообщение типа <typeparamref name="T"/> на указанный адрес</summary>
    /// <param name="Sender">Отправитель сообщения</param>
    /// <param name="Address">Адрес/топик</param>
    /// <param name="Message">Тело сообщения</param>
    /// <param name="Cancel">Токен отмены</param>
    public async Task SendAsync<T>(object Sender, string Address, T Message, CancellationToken Cancel = default)
    {
        var message_type = typeof(T);
        if (!_Handlers.TryGetValue(message_type, out var handlers)) return;
        if (!handlers.TryGetValue(Address, out var address_handlers)) return;

        EventHandler<T>[] delegates;
        lock (address_handlers) delegates = address_handlers.Cast<EventHandler<T>>().ToArray();

        await TaskScheduler.SwitchContext();

        foreach (var handler in delegates)
        {
            Cancel.ThrowIfCancellationRequested();
            handler(Sender, Message);
        }
    }
}