using System.Reflection;
using System.Runtime.CompilerServices;

using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentlySynchronizedField
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeProtected.Global

namespace MathCore.Reflection;

/// <summary>Анимированное свойство</summary>
/// <typeparam name="TObject">Тип объекта, свойство которого требуется анимировать</typeparam>
/// <typeparam name="TValue">Тип значения анимируемого свойства</typeparam>
public class AnimatedProperty<TObject, TValue> : Property<TObject, TValue>
{
    /// <summary>Число шагов анимации</summary>
    private readonly int _Samples;

    /// <summary>Интервал времени анимации в секундах</summary>
    private readonly int _Timeout;

    /// <summary>Функция, вычисляющая очередное значение свойства на основе номера шага анимации и числа шагов</summary>
    private readonly Func<int, int, TValue> _Translator;

    /// <summary>Признак активности анимации</summary>
    private bool _Enabled;

    /// <summary>поток в котором будет происходить изменение значений свойства</summary>
    private Thread _Thread;

    /// <summary>Приоритет потока анимации</summary>
    private ThreadPriority _Priority = ThreadPriority.Normal;

    /// <summary>Анимация включена</summary>
    public bool Enable { get => _Enabled; set { if (value) Start(); else Stop(); } }

    /// <summary>Приоритет потока анимации</summary>
    public ThreadPriority Priority
    {
        get => _Priority;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
            _Priority = value;
            if (_Enabled) _Thread.Priority = value;
        }
    }

    /// <summary>Повторять анимацию в цикле?</summary>
    public bool Repeat { get; set; }

    /// <summary>По завершении прямого хода анимации выполнять обратный</summary>
    public bool AutoReverse { get; set; }

    /// <summary>Инициализация нового экземпляра <see cref="AnimatedIntProperty{TObject}"/></summary>
    /// <param name="o">Объект, свойство которого анимируется</param>
    /// <param name="Name">Имя анимируемого свойства</param>
    /// <param name="Samples">Число шагов анимации</param>
    /// <param name="Timeout">Временной интервал анимации</param>
    /// <param name="Translator">Функция, вычисляющая очередное значение свойства на основе номера шага анимации и числа шагов</param>
    /// <param name="Private">Искать приватное свойство?</param>
    public AnimatedProperty(
        TObject o,
        [NotNull] string Name,
        int Samples,
        int Timeout,
        Func<int, int, TValue> Translator,
        bool Private = false)
        : base(o, Name, Private)
    {
        _Samples    = Samples;
        _Timeout    = Timeout;
        _Translator = Translator;
    }

    /// <summary>Запуск анимации</summary>
    public void Start()
    {
        if (_Enabled) return;
        lock (this)
        {
            if (_Enabled) return;
            _Thread  = new Thread(Do) { Priority = _Priority };
            _Enabled = true;
            _Thread.Start();
        }
    }

    /// <summary>Остановка анимации</summary>
    public void Stop()
    {
        if (!_Enabled) return;
        lock (this)
        {
            if (!_Enabled) return;
            _Enabled = false;
            _Thread.Abort();
            if (!_Thread.Join(2 * _Timeout) && _Thread.IsAlive)
                _Thread.Interrupt();
            _Thread = null;
        }
    }

    /// <summary>Метод, Выполняющий анимацию свойства</summary>
    private void Do()
    {
        var count   = _Samples;
        var timeout = _Timeout;
        while (Repeat)
        {
            for (var i = 0; _Enabled && i < count; i++)
            {
                Value = _Translator(i, count);
                Thread.Sleep(timeout);
            }
            if (!AutoReverse) continue;
            for (var i = count - 1; _Enabled && i >= 0; i--)
            {
                Value = _Translator(i, count);
                Thread.Sleep(timeout);
            }
        }
    }
}