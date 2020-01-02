using System.Reflection;

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
            var lv_EventDescriptor = _Target.GetType().GetEvent(EventName);
            _EventDescriptor = lv_EventDescriptor;
            if(_EventDescriptor is null)
                throw new ArgumentException($"Событие {EventName} не найдено", nameof(EventName));
            _EventDescriptor.AddEventHandler(_Target, _EventHandler = (s, e) => OnNext(e));
        }

        public override void Dispose()
        {
            base.Dispose();
            _EventDescriptor.RemoveEventHandler(_Target, _EventHandler);
        }
    }
}