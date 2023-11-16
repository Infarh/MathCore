#nullable enable
namespace MathCore;

public class LambdaStateMachine<TState, TValue>(TState state = default)
{
    public delegate TState Rule(TState State, TValue Value);

    public class NewValueEventArgs(TState state, TValue value) : EventArgs
    {
        public TState? State { get; set; } = state;

        public TValue? Value => value;
    }

    public class NewStateEventArgs(TState Old, TState New, TValue value) : EventArgs
    {
        public TState? OldState { get; } = Old;
        public TState? NewState { get; } = New;
        public TValue? Value { get; } = value;
    }

    public event EventHandler<NewValueEventArgs>? NewValue;
    public event EventHandler<NewStateEventArgs>? NewState;

    protected virtual TState OnNewValue(TValue Value)
    {
        var e = new NewValueEventArgs(state, Value);
        OnNewValue(e);
        return e.State;
    }

    protected virtual void OnNewValue(NewValueEventArgs e) => NewValue?.Invoke(this, e);

    protected virtual void OnNewState(TState OldState, TState NewState, TValue Value = default) => this.NewState?.Invoke(this, new NewStateEventArgs(OldState, NewState, Value));

    public TState State
    {
        get => state;
        set
        {
            if(Equals(state, value)) return;
            OnNewState(state, state = value);
        }
    }

    public void Add(TValue Value)
    {
        var new_state = OnNewValue(Value);
        if(Equals(new_state, state)) return;
        OnNewState(state, state = new_state, Value);
    }
}