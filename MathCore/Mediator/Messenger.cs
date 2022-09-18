using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MathCore.Annotations;

namespace MathCore.Mediator
{
    public class Messenger : IMessenger
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, List<Delegate>>> _Handlers =
            new();

        public TaskScheduler TaskScheduler { get; set; } = TaskScheduler.Default;

        public void AddHandle<T>(EventHandler<T> Handler) => AddHandle("", Handler);

        public void AddHandle<T>(string Address, EventHandler<T> Handler)
        {
            var messages_type = typeof(T);
            var handlers = _Handlers.GetOrAdd(messages_type, _ => new ConcurrentDictionary<string, List<Delegate>>());
            var address_handlers = handlers.GetOrAdd(Address ?? "", _ => new List<Delegate>());
            lock (address_handlers)
                address_handlers.Add(Handler);
        }

        public bool RemoveHandler<T>(EventHandler<T> Handler) => throw new NotImplementedException();

        public bool RemoveHandler<T>(string Address, EventHandler<T> Handler)
        {
            var messages_type = typeof(T);

            if (!_Handlers.TryGetValue(messages_type, out var handlers)) return false;
            if (!handlers.TryGetValue(Address ?? "", out var address_handlers)) return false;

            lock (address_handlers)
                return address_handlers.Remove(Handler);
        }

        public void Send<T>(object Sender, T Message) => Send(Sender, "", Message);

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

        [NotNull] public Task SendAsync<T>(object Sender, T Message, CancellationToken Cancel = default) => SendAsync(Sender, "", Message, Cancel);

        [NotNull]
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
}