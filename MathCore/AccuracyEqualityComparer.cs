using System;
using System.Collections.Generic;

namespace MathCore;

public class AccuracyEqualityComparer : IEqualityComparer<double>
{
    private readonly double _Accuracy;

    public AccuracyEqualityComparer(double Accuracy) => _Accuracy = Accuracy;

    public bool Equals(double x, double y)
    {
        var delta = Math.Abs(x - y);
        return delta <= _Accuracy;
    }

    public int GetHashCode(double x)
    {
        var v = Math.Round(x / _Accuracy) * _Accuracy;
        return v.GetHashCode();
    }
}   