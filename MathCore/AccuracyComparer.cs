namespace MathCore;

public class AccuracyComparer : IComparer<double>
{
    private readonly double _Accuracy;

    public AccuracyComparer(double Accuracy) => _Accuracy = Accuracy;

    public int Compare(double x, double y)
    {
        var x1 = Math.Round(x / _Accuracy) * _Accuracy;
        var y1 = Math.Round(y / _Accuracy) * _Accuracy;
        return Comparer<double>.Default.Compare(x1, y1);
    }
}