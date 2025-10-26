#nullable enable
namespace MathCore;

/// <summary>Кольцевой буфер истории с доступом по числу шагов назад</summary>
public sealed class HistoryBuffer<T>(int capacity)
{
    private readonly T[] _Items = capacity > 0 ? new T[capacity] : [];
    private int _Count;
    private int _Head;

    /// <summary>Ёмкость буфера</summary>
    public int Capacity => _Items.Length;

    /// <summary>Текущее число элементов в буфере</summary>
    public int Count => _Count;

    /// <summary>Добавляет элемент в историю</summary>
    public void Enqueue(T item)
    {
        if (Capacity == 0) return;

        _Items[_Head] = item;
        _Head = (_Head + 1) % Capacity;

        if (_Count < Capacity)
            _Count++;
    }

    /// <summary>Возвращает элемент истории k шагов назад, где 0 — последний добавленный</summary>
    public T this[int LastStep]
    {
        get
        {
            if (LastStep < 0 || LastStep >= _Count)
                throw new ArgumentOutOfRangeException(nameof(LastStep));

            var idx = (_Head - 1 - LastStep) % Capacity;
            if (idx < 0)
                idx += Capacity;
            return _Items[idx];
        }
    }

    /// <summary>Очищает буфер</summary>
    public void Clear() { _Count = 0; _Head = 0; }
}

/// <summary>Дискретная динамическая система с поддержкой истории состояний</summary>
public class DynamicSystem<TState, TInput>(int Order)
    where TState : struct
    where TInput : struct
{
    private HistoryBuffer<HistoryItem>? _History = new(Order);

    /// <summary>Элемент истории состояния во времени</summary>
    public readonly record struct HistoryItem(TState State, double Time)
    {
        /// <summary>Неявное приведение к типу состояния</summary>
        public static implicit operator TState(HistoryItem item) => item.State;
    }

    /// <summary>Текущее состояние системы</summary>
    public HistoryItem State { get; protected set; }

    /// <summary>Представление истории для вычисления шага</summary>
    public readonly record struct History(HistoryBuffer<HistoryItem>? history, double Time)
    {
        /// <summary>Доступ к элементу истории k шагов назад, где 0 — предыдущий шаг</summary>
        public HistoryItem this[int steps] => history?[steps] ?? throw new InvalidOperationException("History is disabled");

        /// <summary>Число доступных элементов истории</summary>
        public int Count => history?.Count ?? 0;
    }

    /// <summary>Набор «дифференциальных уравнений», формирующих вклады в новое состояние</summary>
    public required IReadOnlyList<Func<TInput, History, TState>> DiffEquations { get; init; }

    /// <summary>Агрегатор, собирающий новое состояние из предыдущего, вкладов и времени</summary>
    public required Func<TState, TState[], double, TState> Aggregator { get; init; }

    private TState[]? _DiffStates;

    /// <summary>Выполняет один дискретный шаг обновления состояния</summary>
    public TState Update(double Time, TInput input)
    {
        // Сначала фиксируем предыдущее состояние в истории, чтобы s[0] был прошлым шагом
        _History?.Enqueue(State);

        var states = _DiffStates ??= new TState[DiffEquations.Count];

        var s = new History(_History, Time);
        for (var i = 0; i < DiffEquations.Count; i++)
            states[i] = DiffEquations[i](input, s);

        var new_state = Aggregator(State, _DiffStates, Time);

        State = new(new_state, Time);

        return new_state;
    }

    /// <summary>Сбрасывает состояние и очищает историю</summary>
    public void Reset(TState state = default, double time = 0)
    {
        State = new(state, time);
        _History = new(Order); // сохраняем настроенный объём истории
        // Стартовое состояние НЕ добавляем в историю, чтобы при первом Update s[0] ссылался на него один раз
    }
}


