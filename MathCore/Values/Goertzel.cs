namespace MathCore.Values;

/// <summary>Алгоритм Гёрцеля расчёта частотной компоненты спектра</summary>
public class Goertzel : IResettable
{
    /// <summary>Поворачивающий множитель</summary>
    private readonly Complex _W;
    
    /// <summary>Предыдущее состояние алгоритма</summary>
    private double _s1;
    
    /// <summary>Состояние алгоритма два шага назад</summary>
    private double _s2;

    private int _SamplesCount;

    /// <summary>Предыдущее состояние алгоритма</summary>
    public double State1 => _s1;

    /// <summary>Состояние алгоритма два шага назад</summary>
    public double State2 => _s2;

    /// <summary>Текущее значение частотной компоненты спектра</summary>
    public Complex State { get; private set; }

    public Complex NormState => State / _SamplesCount;

    public int SamplesCount => _SamplesCount;

    public Goertzel(double f0) => _W = Complex.Exp(Consts.pi2 * f0);

    /// <summary>Сброс состояния фильтра</summary>
    public void Reset()
    {
        _s1 = 0;
        _s2 = 0;
        _SamplesCount = 0;
    }

    /// <summary>Добавление нового значения</summary>
    /// <param name="x">Добавляемое значение</param>
    /// <returns>Текущее значение частотной компоненты</returns>
    public Complex Add(double x)
    {
        _SamplesCount++;
        var s = x + 2 * _W.Re * _s1 - _s2;
        var y = _W * s - _s1;
        State = y;

        _s2 = _s1;
        _s1 = s;

        return y / _SamplesCount;
    }

    public static Goertzel operator +(Goertzel goertzel, double s)
    {
        goertzel.Add(s);
        return goertzel;
    }

    public static implicit operator Complex(Goertzel goertzel) => goertzel.NormState;
}

/// <summary>Комплексный Алгоритм Гёрцеля расчёта частотной компоненты спектра</summary>
public class GoertzelComplex : IResettable
{
    /// <summary>Поворачивающий множитель</summary>
    private readonly Complex _W;

    /// <summary>Предыдущее состояние алгоритма</summary>
    private Complex _s1;

    /// <summary>Состояние алгоритма два шага назад</summary>
    private Complex _s2;

    private int _SamplesCount;

    /// <summary>Предыдущее состояние алгоритма</summary>
    public Complex State1 => _s1;

    /// <summary>Состояние алгоритма два шага назад</summary>
    public Complex State2 => _s2;

    /// <summary>Текущее значение частотной компоненты спектра</summary>
    public Complex State { get; private set; }

    public Complex NormState => State / _SamplesCount;

    public GoertzelComplex(double f0) => _W = Complex.Exp(Consts.pi2 * f0);

    /// <summary>Сброс состояния фильтра</summary>
    public void Reset()
    {
        _s1 = 0;
        _s2 = 0;
        _SamplesCount = 0;
    }

    /// <summary>Добавление нового значения</summary>
    /// <param name="x">Добавляемое значение</param>
    /// <returns>Текущее значение частотной компоненты</returns>
    public Complex Add(Complex x)
    {
        _SamplesCount++;
        var s = x + 2 * _W.Re * _s1 - _s2;
        var y = _W * s - _s1;
        State = y;

        _s2 = _s1;
        _s1 = s;

        return y / _SamplesCount;
    }

    public static GoertzelComplex operator +(GoertzelComplex goertzel, double s)
    {
        goertzel.Add(s);
        return goertzel;
    }

    public static implicit operator Complex(GoertzelComplex goertzel) => goertzel.NormState;
}

/// <summary>Алгоритм Гёрцеля расчёта частотной компоненты спектра</summary>
public readonly ref struct GoertzelStruct
{
    /// <summary>Поворачивающий множитель</summary>
    private readonly Complex _W;

    /// <summary>Предыдущее состояние алгоритма</summary>
    private readonly double _s1;

    /// <summary>Состояние алгоритма два шага назад</summary>
    private readonly double _s2;

    private readonly int _SamplesCount;

    /// <summary>Предыдущее состояние алгоритма</summary>
    public double State1 => _s1;

    /// <summary>Состояние алгоритма два шага назад</summary>
    public double State2 => _s2;

    /// <summary>Текущее значение частотной компоненты спектра</summary>
    public Complex State { get; }

    public Complex NormState => State / _SamplesCount;

    public GoertzelStruct(double f0)
    {
        _W = Complex.Exp(Consts.pi2 * f0);
        State = Complex.Zero;
        _s1 = 0;
        _s2 = 0;
        _SamplesCount = 0;
    }

    private GoertzelStruct(Complex W0, Complex State, double s1, double s2, int SamplesCount)
    {
        _W = W0;
        this.State = State;
        _s1 = s1;
        _s2 = s2;
        _SamplesCount = SamplesCount;
    }

    /// <summary>Добавление нового значения</summary>
    /// <param name="x">Добавляемое значение</param>
    /// <returns>Текущее значение частотной компоненты</returns>
    public GoertzelStruct Add(double x)
    {
        var s = x + 2 * _W.Re * _s1 - _s2;
        var y = _W * s - _s1;
        return new(_W, y, s, _s1, _SamplesCount + 1);
    }

    public static GoertzelStruct operator +(GoertzelStruct goertzel, double s) => goertzel.Add(s);

    public static implicit operator Complex(GoertzelStruct goertzel) => goertzel.NormState;
}

/// <summary>Алгоритм Гёрцеля расчёта частотной компоненты спектра</summary>
public readonly ref struct GoertzelComplexStruct
{
    /// <summary>Поворачивающий множитель</summary>
    private readonly Complex _W;

    /// <summary>Предыдущее состояние алгоритма</summary>
    private readonly Complex _s1;

    /// <summary>Состояние алгоритма два шага назад</summary>
    private readonly Complex _s2;

    private readonly int _SamplesCount;

    /// <summary>Предыдущее состояние алгоритма</summary>
    public Complex State1 => _s1;

    /// <summary>Состояние алгоритма два шага назад</summary>
    public Complex State2 => _s2;

    /// <summary>Текущее значение частотной компоненты спектра</summary>
    public Complex State { get; }

    public Complex NormState => State / _SamplesCount;

    public GoertzelComplexStruct(double f0)
    {
        _W = Complex.Exp(Consts.pi2 * f0);
        State = Complex.Zero;
        _s1 = Complex.Zero;
        _s2 = Complex.Zero;
        _SamplesCount = 0;
    }

    private GoertzelComplexStruct(Complex W0, Complex State, Complex s1, Complex s2, int SamplesCount)
    {
        _W = W0;
        this.State = State;
        _s1 = s1;
        _s2 = s2;
        _SamplesCount = SamplesCount;
    }

    /// <summary>Добавление нового значения</summary>
    /// <param name="x">Добавляемое значение</param>
    /// <returns>Текущее значение частотной компоненты</returns>
    public GoertzelComplexStruct Add(Complex x)
    {
        var s = x + 2 * _W.Re * _s1 - _s2;
        var y = _W * s - _s1;
        return new(_W, y, s, _s1, _SamplesCount + 1);
    }

    public static GoertzelComplexStruct operator +(GoertzelComplexStruct goertzel, Complex s) => goertzel.Add(s);

    public static implicit operator Complex(GoertzelComplexStruct goertzel) => goertzel.State;
}