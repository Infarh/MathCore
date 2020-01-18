using System.Reflection;

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive
{
    internal sealed class EventObservableEx<TEventArgs> : SimpleObservableEx<TEventArgs>
        where TEventArgs : EventArgs
    {
        private readonly EventHandler<TEventArgs> _EventHandler;
        private readonly object _Target;
        private readonly EventInfo _EventDescriptor;

        public EventObservableEx(object Obj, string EventName)
        {
            _Target = Obj;
            var event_descriptor = _Target.GetType().GetEvent(EventName);
            _EventDescriptor = event_descriptor;
            if(_EventDescriptor is null)
                throw new ArgumentException($"Событие {EventName} не найдено", nameof(EventName));
            _EventDescriptor.AddEventHandler(_Target, _EventHandler = (s, e) => OnNext(e));
        }

        protected override void Dispose(bool Disposing)
        {
            base.Dispose(Disposing);
            _EventDescriptor.RemoveEventHandler(_Target, _EventHandler);
        }
    }
}