#nullable enable
namespace MathCore.Threading.Tasks;

public class ProgressSplitter
{
    private readonly int _OperationsCount;

    private readonly IProgress<double>[]? _Progress;

    public int OperationsCount => _OperationsCount;

    public IProgress<double>? this[int Operation] => _Progress?[Operation];

    public ProgressSplitter(IProgress<double>? MainProgress, int OperationsCount)
    {
        _OperationsCount = OperationsCount;

        if(MainProgress is null) return;

        _Progress = new IProgress<double>[OperationsCount];
        var progress_values = new double[OperationsCount];
        for (var i = 0; i < OperationsCount; i++)
        {
            var i0 = i;

            _Progress[i] = new SimpleProgress<double>(
                p =>
                {
                    progress_values[i0] = p;
                    var progress = progress_values.Sum() / OperationsCount;
                    MainProgress.Report(progress);
                });
        }
    }
}
