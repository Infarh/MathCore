#nullable enable
namespace MathCore.Threading.Tasks;

public class SimpleProgress<T>(Action<T> ProgressReporter) : IProgress<T>
{
    public SimpleProgress() : this(null) { }

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


    public void Report(T value)
    {
        ProgressReporter?.Invoke(value);
        OnProgressChanged(value);
    }
}
