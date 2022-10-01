#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MathCore.Mediator;

public interface IMessenger
{
    public void AddHandle<T>(EventHandler<T> Handler);

    public void AddHandle<T>(string Address, EventHandler<T> Handler);

    public bool RemoveHandler<T>(EventHandler<T> Handler);

    public bool RemoveHandler<T>(string Address, EventHandler<T> Handler);

    public void Send<T>(object Sender, T Message);

    public void Send<T>(object Sender, string Address, T Message);

    public Task SendAsync<T>(object Sender, T Message, CancellationToken Cancel = default);

    public Task SendAsync<T>(object Sender, string Address, T Message, CancellationToken Cancel = default);
}