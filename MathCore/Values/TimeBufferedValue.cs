using System;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedParameter.Local

namespace MathCore.Values
{
    public class TimeBufferedValue<TValue> : IFactory<TValue>
    {
        private readonly LazyValue<TValue> _Value;
        private DateTime _LastAccessTime = DateTime.MinValue;

        public TimeBufferedValue(Func<TValue> Generator, TimeSpan Timeout) => 
            _Value = new LazyValue<TValue>(() =>
            {
                _LastAccessTime = DateTime.Now;
                return Generator();
            });

        public TValue Create() => throw new NotImplementedException();
    }
}