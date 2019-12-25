using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MathCore.Threading
{
    public sealed class MaxConcurrencySynchronizationContext : SynchronizationContext
    {
        private readonly SemaphoreSlim _Semaphore;

        public MaxConcurrencySynchronizationContext(int maxConcurrencyLevel) => _Semaphore = new SemaphoreSlim(maxConcurrencyLevel);

        private void OnSemaphoreReleased(Task SemaphoreWaitTask, object CallState)
        {
            var d = (SendOrPostCallback)((object[])CallState)[0];
            var state = ((object[])CallState)[1];
            try
            {
                d(state);
            }
            finally
            {
                _Semaphore.Release();
            }
        }

        public override void Post(SendOrPostCallback d, object state) =>
            _Semaphore
                .WaitAsync()
                .ContinueWith(
                    OnSemaphoreReleased, 
                    new [] { d, state }, 
                    default, 
                    TaskContinuationOptions.None, 
                    TaskScheduler.Default);

        public override void Send(SendOrPostCallback d, object state)
        {
            _Semaphore.Wait();
            try
            {
                d(state);
            }
            finally
            {
                _Semaphore.Release();
            }
        }
    }
}
