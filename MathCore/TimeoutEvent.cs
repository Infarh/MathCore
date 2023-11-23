#nullable enable
namespace MathCore;

/// <summary>Класс задержки реакции на событие</summary>
/// <typeparam name="TEventArgs">Тип аргумента события</typeparam>
public class TimeoutEvent<TEventArgs> where TEventArgs : EventArgs
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Метод подписки на событие</summary>
    /// <param name="Timeout">Таймаут</param>
    /// <param name="OnTimeout">Метод обработки первичного вызова события</param>
    /// <param name="OnInvoke">Метод обработки вторичного события</param>
    /// <returns>Обработчик исходного события</returns>
    public static EventHandler<TEventArgs> Subscribe(int Timeout, EventHandler<TEventArgs> OnTimeout, EventHandler<TEventArgs>? OnInvoke = null)
    {
        var                      @event     = new TimeoutEvent<TEventArgs>(Timeout);
        EventHandler<TEventArgs> handler   = @event.Invoke;
        if(OnInvoke != null) @event.Invoked += (_, e) => OnInvoke(e?.EventSender, e?.E);
        @event.AfterTimeout += (_, e) => OnTimeout(e?.EventSender, e?.E);
        return handler;
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Константа бесконечного периода ожидания</summary>
    private const int __Infinite = System.Threading.Timeout.Infinite;

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Аргумент события</summary>
    /// <remarks>Аргумент события</remarks>
    /// <param name="Sender">Источник исходного события</param>
    /// <param name="e">Аргумент исходного события</param>
    [DST]
    public class Info(object Sender, TEventArgs e) : EventArgs
    {
        /// <summary>Источник сходного события</summary>
        public object EventSender => Sender;

        /// <summary>Аргумент исходного события</summary>
        public TEventArgs E => e;
    }

    /// <summary>Первичная генерация события</summary>
    public event EventHandler<Info>? Invoked;

    /// <summary>Метод первичной генерации события <see cref="Invoked"/></summary>
    /// <param name="e">Аргумент первичного вызова события</param>
    protected virtual void OnInvoked(Info e)
    {
        _LastEventArgs = e;
        Invoked?.Invoke(this, e);
    }

    /// <summary>Событие, возникающее после последнего вызова метода <see cref="Invoke"/> через период времени <see cref="Timeout"/></summary>
    public event EventHandler<Info>? AfterTimeout;

    /// <summary>Метод генерации события <see cref="AfterTimeout"/></summary>
    /// <param name="e">Аргумент события</param>
    [DST] protected virtual void OnAfterTimeout(Info e) => AfterTimeout?.Invoke(this, e);

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Таймер</summary>
    private readonly Timer _Timer;

    /// <summary>Период времени таймаута</summary>
    private int _Timeout;

    /// <summary>Время последнего вызова</summary>
    private DateTime _LastCallTime;

    /// <summary>Признак ожидания таймаута события</summary>
    private bool _InProcess;

    /// <summary>Аргумент последнего вызова метода <see cref="Invoke"/></summary>
    private Info _LastEventArgs = null!;

    /// <summary>Признак ожидания отмены генерации события <see cref="AfterTimeout"/></summary>
    private bool _NeedAbort;

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Задержка во времени генерации события <see cref="AfterTimeout"/> в миллисекундах</summary>
    public int Timeout { get => _Timeout; set => _Timeout = value; }

    /// <summary>Время последнего вызова метода <see cref="Invoke"/></summary>
    public DateTime LastCallTime => _LastCallTime;

    /// <summary>Признак ожидания генерации события <see cref="Invoked"/></summary>
    public bool InProcess => _InProcess;

    /// <summary>Признак отмены генерации соития <see cref="AfterTimeout"/></summary>
    public bool NeedAbort { get => _NeedAbort; set => _NeedAbort = value; }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Инициализация нового объекта задержки генерации события</summary>
    /// <param name="Timeout">Временная задержка в миллисекундах</param>
    public TimeoutEvent(int Timeout)
    {
        _Timeout = Timeout;
        _Timer   = new Timer(OnTimer, null, __Infinite, __Infinite);
    }

    /// <summary>Инициализация нового объекта задержки генерации события</summary>
    /// <param name="Timeout">Временная задержка в миллисекундах</param>
    /// <param name="OnTimeout">Метод вторичной обработки события</param>
    public TimeoutEvent(int Timeout, EventHandler<TEventArgs> OnTimeout) : this(Timeout) => AfterTimeout += (_, e) => OnTimeout(e.EventSender, e.E);

    /// <summary>Инициализация нового объекта задержки генерации события</summary>
    /// <param name="Timeout">Временная задержка в миллисекундах</param>
    /// <param name="OnTimeout">Метод вторичной обработки события</param>
    /// <param name="OnInvoke">Метод первичной обработки события</param>
    public TimeoutEvent(int Timeout, EventHandler<TEventArgs> OnTimeout, EventHandler<TEventArgs> OnInvoke) : this(Timeout, OnTimeout) => Invoked += (_, e) => OnInvoke(e.EventSender, e.E);

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Метод обработки события таймера</summary>
    /// <param name="State">Состояние таймера</param>
    private void OnTimer(object? State)
    {
        bool is_timeout;
        lock(_Timer)
        {
            if(!_InProcess) return;
            if(_NeedAbort)
            {
                _InProcess = _NeedAbort = false;
                return;
            }
            var dt = _Timeout - (DateTime.Now - _LastCallTime).TotalMilliseconds;
            // ReSharper disable once AssignmentInConditionalExpression
            if(is_timeout = dt <= 0)
                _InProcess = false;
            else
                _Timer.Change((int)dt, __Infinite);
        }
        if(is_timeout) OnAfterTimeout(_LastEventArgs);
    }

    /// <summary>Метод генерации события</summary>
    /// <param name="EventSender">Источник события</param>
    /// <param name="args">Аргументы события</param>
    public void Invoke(object? EventSender = null, TEventArgs? args = null)
    {
        lock(_Timer)
        {
            _LastCallTime = DateTime.Now;
            _NeedAbort    = false;
            if(!_InProcess)
            {
                _InProcess = true;
                _Timer.Change(_Timeout, __Infinite);
            }
        }
        OnInvoked(new Info(EventSender, args));
    }

    /// <summary>Отмена реакции на событие</summary>
    public void Abort()
    {
        lock(_Timer)
        {
            if(!_InProcess) return;
            _NeedAbort = true;
        }
    }

    /* ------------------------------------------------------------------------------------------ */
}