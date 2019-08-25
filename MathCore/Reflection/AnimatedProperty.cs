using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MathCore.Reflection
{
    public class AnimatedProperty<TObject, TValue> : Property<TObject, TValue>
    {
        private readonly int _Samples;
        private readonly int _Timeout;
        private readonly Func<int, int, TValue> _Translator;

        private bool _Enabled;
        private Thread _Thread;
        private ThreadPriority _Priority = ThreadPriority.Normal;

        public bool Enable { get => _Enabled;
            set { if(value) Start(); else Stop(); } }

        public ThreadPriority Priority
        {
            get => _Priority;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _Priority = value;
                if(_Enabled) _Thread.Priority = value;
            }
        }

        public AnimatedProperty(TObject o, string Name,
            int Samples, int Timeout, Func<int, int, TValue> Translator,
            bool Private = false)
            : base(o, Name, Private)
        {
            Contract.Requires(Samples > 0);
            Contract.Requires(Timeout >= 0);
            Contract.Requires(Translator != null);

            _Samples = Samples;
            _Timeout = Timeout;
            _Translator = Translator;
        }

        public void Start()
        {
            if(_Enabled) return;
            lock(this)
            {
                if(_Enabled) return;
                _Thread = new Thread(Do) { Priority = _Priority };
                _Enabled = true;
                _Thread.Start();
            }
        }

        public void Stop()
        {
            if(!_Enabled) return;
            lock(this)
            {
                if(!_Enabled) return;
                _Enabled = false;
                _Thread.Abort();
                if(!_Thread.Join(2 * _Timeout) && _Thread.IsAlive)
                    _Thread.Interrupt();
                _Thread = null;
            }
        }

        private void Do()
        {
            var count = _Samples;
            var timeout = _Timeout;
            for(var i = 0; _Enabled && i < count; i++)
            {
                Value = _Translator(i, count);
                Thread.Sleep(timeout);
            }
        }

        [ContractInvariantMethod]
        private void InvariantCheck()
        {
            Contract.Invariant(_Samples > 0);
            Contract.Invariant(_Timeout >= 0);
        }
    }
}