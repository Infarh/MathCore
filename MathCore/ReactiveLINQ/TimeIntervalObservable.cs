using System.Threading;
using System.Threading.Tasks;

namespace System.Linq.Reactive
{
    public class TimeIntervalObservable : SimpleObservableEx<TimeSpan>
    {
        private readonly TimeSpan _Interval;
        private readonly bool _Async;
        private bool _Work;
        private readonly object _SyncObject = new object();
        private Thread _Thread;

        public TimeIntervalObservable(TimeSpan interval, bool Start = false, bool Async = false)
        {
            _Interval = interval;
            _Async = Async;
            if(Start) this.Start();
        }

        protected override void OnReset(IObserverEx<TimeSpan> observer) => throw new NotSupportedException();

        public void Start()
        {
            if(_Work) return;
            lock (_SyncObject)
            {
                if(_Work) return;
                _Work = true;
                var NeedReset = _Thread != null;
                _Thread = _Async
                    ? new Thread(AsyncThreadMethod) { IsBackground = true }
                    : new Thread(SyncThreadMethod) { IsBackground = true };
                if(NeedReset) base.OnReset();
                _Thread.Start();
            }
        }

        public void Stop()
        {
            if(!_Work) return;
            lock (_SyncObject)
            {
                if(!_Work) return;
                _Work = false;
                if(!_Thread.Join(_Interval.Milliseconds))
                    _Thread.Abort();
            }
            foreach(var observer in _Observers.ToArray())
                observer.OnCompleted();
        }

        public override void OnNext(TimeSpan item) => throw new NotSupportedException();

        private void SyncThreadMethod()
        {
            while(_Work)
            {
                var t = DateTime.Now.TimeOfDay;
                base.OnNext(t);
                Thread.Sleep(_Interval);
            }
        }

        private void AsyncThreadMethod()
        {
            Action<TimeSpan> next = base.OnNext;
            while(_Work)
            {
                var t = DateTime.Now.TimeOfDay;
                next.BeginInvoke(t, null, null);
                Thread.Sleep(_Interval);
            }
        }

        public override async void Dispose()
        {
            base.Dispose();
            await Task.Factory.StartNew((Action)Stop);
        }
    }
}