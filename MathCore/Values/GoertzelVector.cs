#nullable enable
using System;

namespace MathCore.Values;

public class GoertzelVector
{
    /// <summary>Поворачивающий множитель</summary>
    private readonly Complex[] _W;
    private readonly Complex[] _State;

    /// <summary>Предыдущее состояние алгоритма</summary>
    private readonly double[] _s1;

    /// <summary>Состояние алгоритма два шага назад</summary>
    private readonly double[] _s2;

    /// <summary>Предыдущее состояние алгоритма</summary>
    public double[] State1 => _s1;

    /// <summary>Состояние алгоритма два шага назад</summary>
    public double[] State2 => _s2;

    /// <summary>Текущее значение частотной компоненты спектра</summary>
    public Complex[] State => _State;

    public GoertzelVector(double[] f0)
    {
        //_W = Complex.Exp(Consts.pi2 * f0);

        var N = f0.Length;
        _s1 = new double[N];
        _s2 = new double[N];
        _State = new Complex[N];
        _W = new Complex[N];
        for(var i = 0; i < N; i++)
            _W[i] = Complex.Exp(Consts.pi2 * f0[i]);
    }

    /// <summary>Сброс состояния фильтра</summary>
    public void Reset()
    {
        Array.Clear(_s1, 0, _s1.Length);
        Array.Clear(_s2, 0, _s2.Length);
    }

    /// <summary>Добавление нового значения</summary>
    /// <param name="x">Добавляемое значение</param>
    /// <returns>Текущее значение частотной компоненты</returns>
    public Complex[] Add(double x)
    {
        var n = _State.Length;

        for (var i = 0; i < n; i++)
        {
            var s = x + 2 * _W[i].Re * _s1[i] - _s2[i];

            (_State[i], _s1[i], _s2[i]) = (_W[i] * s - _s1[i], s, _s1[i]);
        }

        return _State;
    }
}