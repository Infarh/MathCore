﻿#nullable enable
using System;
using System.Runtime.CompilerServices;
// ReSharper disable UnusedMember.Global

namespace MathCore.Values;

/// <summary>Усредняемая по времени величина</summary>
[Serializable]
public class TimeAverageValue : IValue<double>, IResettable
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Последнее время доступа</summary>
    protected DateTime _LastTime;

    /// <summary>Постоянная времени усреднения</summary>
    private double _Tau;

    protected DateTime _StartTime;

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Постоянная времени</summary>
    public double Tau
    {
        get => _Tau;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => _Tau = value;
    }

    /// <summary>Значение</summary>
    public double Value { get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

    /// <summary>Признак инициализации</summary>
    public bool Initialized { get; protected set; }

    public DateTime StartTime => _StartTime;

    public TimeSpan ElapsedTime => DateTime.Now - _StartTime;

    public double OverrideTime => ElapsedTime.TotalSeconds / Tau;

    /* ------------------------------------------------------------------------------------------ */

    ///// <summary>Усредняемая по времени величина</summary>
    //public TimeAverageValue() { Initialized = false; }

    /// <summary>Усредняемая по времени величина</summary>
    /// <param name="tau">Постоянная времени усреднения >= 0</param>
    public TimeAverageValue(double tau)
    {
        Initialized = false;
        _StartTime  = _LastTime = DateTime.Now;
        Tau         = tau;
    }

    /// <summary>Усредняемая по времени величина</summary>
    /// <param name="Value">Начальное значение</param><param name="tau">постоянная времени >= 0</param>
    public TimeAverageValue(double Value, double tau)
        : this(tau)
    {
        this.Value  = Value;
        _StartTime  = _LastTime = DateTime.Now;
        Initialized = true;
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Добавить значение</summary>
    /// <param name="value">Значение</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual double Add(double value)
    {
        var current_time = DateTime.Now;
        if(!Initialized || Math.Abs(_Tau) < double.Epsilon)
        {
            Initialized = true;
            Value       = value;
            _StartTime  = _LastTime = current_time;
            return value;
        }

        var delta = value - Value;
        var dt    = (current_time - _LastTime).TotalSeconds;
        _LastTime = current_time;
        if(Math.Abs(delta) < double.Epsilon) return Value;
        Value += delta * dt / _Tau;
        return Value;
    }

    /// <summary>Сброс состояния</summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void Reset() => Initialized = false;

    /* ------------------------------------------------------------------------------------------ */
}