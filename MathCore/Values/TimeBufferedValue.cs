#nullable enable
using System;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedParameter.Local

namespace MathCore.Values;

[NotImplemented]
public class TimeBufferedValue<TValue> : IFactory<TValue>
{
    private readonly LazyValue<TValue> _Value;
    private DateTime _LastAccessTime = DateTime.MinValue;

    public TimeBufferedValue(Func<TValue> Generator, TimeSpan Timeout) => _Value = new(() =>
    {
        _LastAccessTime = DateTime.Now;
        return Generator();
    });

    public TValue Create() => throw new NotImplementedException();
}