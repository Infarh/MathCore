namespace MathCore;

public class AccuracyComparer(double Accuracy) : IComparer<double>
{
    public int Compare(double x, double y)
    {
        var x1 = Math.Round(x / Accuracy) * Accuracy;
        var y1 = Math.Round(y / Accuracy) * Accuracy;
        return Comparer<double>.Default.Compare(x1, y1);
    }
}