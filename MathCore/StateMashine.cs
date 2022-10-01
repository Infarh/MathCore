#nullable enable
using System;

namespace MathCore;

public class LambdaStateMachine<TState, TValue>
{
    public delegate TState Rule(TState State, TValue Value);

    public class NewValueEventArgs : EventArgs
    {
        public TState? State { get; set; }
        public TValue? Value { get; }
        public NewValueEventArgs(TState State, TValue Value)
        {
            this.State = State;
            this.Value = Value;
        }
    }

    public class NewStateEventArgs : EventArgs
    {
        public TState? OldState { get; }
        public TState? NewState { get; }
        public TValue? Value { get; }
        public NewStateEventArgs(TState OldState, TState NewState, TValue Value)
        {
            this.OldState = OldState;
            this.NewState = NewState;
            this.Value    = Value;
        }
    }

    public event EventHandler<NewValueEventArgs>? NewValue;
    public event EventHandler<NewStateEventArgs>? NewState;

    protected virtual TState OnNewValue(TValue Value)
    {
        var e = new NewValueEventArgs(_State, Value);
        OnNewValue(e);
        return e.State;
    }

    protected virtual void OnNewValue(NewValueEventArgs e) => NewValue?.Invoke(this, e);

    protected virtual void OnNewState(TState OldState, TState NewState, TValue Value = default) => this.NewState?.Invoke(this, new NewStateEventArgs(OldState, NewState, Value));

    private TState _State;

    public TState State
    {
        get => _State;
        set
        {
            if(Equals(_State, value)) return;
            OnNewState(_State, _State = value);
        }
    }

    public LambdaStateMachine(TState State = default) => _State = State;

    public void Add(TValue Value)
    {
        var new_state = OnNewValue(Value);
        if(Equals(new_state, _State)) return;
        OnNewState(_State, _State = new_state, Value);
    }
}