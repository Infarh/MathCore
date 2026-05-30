using System;
using System.Threading;
using System.Threading.Tasks;

namespace MathCore.Mediator;

/// <summary>Простой шина сообщений для регистрации и вызова обработчиков событий по типу и адресации</summary>
public interface IMessenger
{
    /// <summary>Добавляет обработчик для сообщений типа <typeparamref name="T"/> на адрес по умолчанию</summary>
    /// <param name="Handler">Обработчик события</param>
    void AddHandle<T>(EventHandler<T> Handler);

    /// <summary>Добавляет обработчик для сообщений типа <typeparamref name="T"/> на указанный адрес</summary>
    /// <param name="Address">Адрес/топик сообщения</param>
    /// <param name="Handler">Обработчик события</param>
    void AddHandle<T>(string Address, EventHandler<T> Handler);

    /// <summary>Удаляет обработчик для сообщений типа <typeparamref name="T"/>, зарегистрированный на любом адресе</summary>
    /// <param name="Handler">Обработчик для удаления</param>
    /// <returns>True если обработчик был найден и удалён</returns>
    bool RemoveHandler<T>(EventHandler<T> Handler);

    /// <summary>Удаляет обработчик для сообщений типа <typeparamref name="T"/>, зарегистрированный на указанном адресе</summary>
    /// <param name="Address">Адрес/топик</param>
    /// <param name="Handler">Обработчик для удаления</param>
    /// <returns>True если обработчик был найден и удалён</returns>
    bool RemoveHandler<T>(string Address, EventHandler<T> Handler);

    /// <summary>Синхронно отправляет сообщение типа <typeparamref name="T"/> на адрес по умолчанию</summary>
    /// <param name="Sender">Отправитель сообщения</param>
    /// <param name="Message">Тело сообщения</param>
    void Send<T>(object Sender, T Message);

    /// <summary>Синхронно отправляет сообщение типа <typeparamref name="T"/> на указанный адрес</summary>
    /// <param name="Sender">Отправитель сообщения</param>
    /// <param name="Address">Адрес/топик</param>
    /// <param name="Message">Тело сообщения</param>
    void Send<T>(object Sender, string Address, T Message);

    /// <summary>Асинхронно отправляет сообщение типа <typeparamref name="T"/> на адрес по умолчанию</summary>
    /// <param name="Sender">Отправитель сообщения</param>
    /// <param name="Message">Тело сообщения</param>
    /// <param name="Cancel">Токен отмены</param>
    Task SendAsync<T>(object Sender, T Message, CancellationToken Cancel = default);

    /// <summary>Асинхронно отправляет сообщение типа <typeparamref name="T"/> на указанный адрес</summary>
    /// <param name="Sender">Отправитель сообщения</param>
    /// <param name="Address">Адрес/топик</param>
    /// <param name="Message">Тело сообщения</param>
    /// <param name="Cancel">Токен отмены</param>
    Task SendAsync<T>(object Sender, string Address, T Message, CancellationToken Cancel = default);
}