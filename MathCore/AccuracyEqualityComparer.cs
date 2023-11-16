namespace MathCore;

public class AccuracyEqualityComparer(double Accuracy) : IEqualityComparer<double>
{
    public bool Equals(double x, double y)
    {
        var delta = Math.Abs(x - y);
        return delta <= Accuracy;
    }

    public int GetHashCode(double x)
    {
        var v = Math.Round(x / Accuracy) * Accuracy;
        return v.GetHashCode();
    }
}   