namespace ConsoleTest;

public static class ConsoleChartTest
{
    public static void Run()
    {
        static double Function(double x) => MathEx.Sinc(Consts.pi2 * x);

        var xx = Interval.Range(-5, 5, 0.01).ToArray();
        var yy = xx.ToArray(Function);

        var chart = new ConsoleChart(Console.WindowWidth, Console.WindowHeight-1, Console.Out);
        chart.Plot(xx, yy);
    }
}