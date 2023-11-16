#nullable enable
namespace MathCore.Threading.Tasks;

public class SimpleProgress<T> : IProgress<T>
{
    private event EventHandler<EventArgs<T>>? ProgressChangedInternal;
    private EventArgs<T>? _Arg;

    public event EventHandler<EventArgs<T>>? ProgressChanged
    {
        add
        {
            ProgressChangedInternal += value;
            _Arg ??= new(default!);
        }
        remove
        {
            ProgressChangedInternal -= value;
            if (ProgressChangedInternal is null)
                _Arg = null;
        }
    }

    protected virtual void OnProgressChanged(T NewProgress)
    {
        if(_Arg is not { } arg) return;
        arg.Argument = NewProgress;
        ProgressChangedInternal?.Invoke(this, arg);
    }

    private readonly Action<T>? _ProgressReporter;

    public SimpleProgress() { }

    public SimpleProgress(Action<T> ProgressReporter) => _ProgressReporter = ProgressReporter;

    public void Report(T value)
    {
        _ProgressReporter?.Invoke(value);
        OnProgressChanged(value);
    }
}
