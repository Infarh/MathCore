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
/// <remarks>Инициализация нового экземпляра <see cref="AnimatedIntProperty{TObject}"/></remarks>
/// <param name="o">Объект, свойство которого анимируется</param>
/// <param name="Name">Имя анимируемого свойства</param>
/// <param name="Samples">Число шагов анимации</param>
/// <param name="Timeout">Временной интервал анимации</param>
/// <param name="Translator">Функция, вычисляющая очередное значение свойства на основе номера шага анимации и числа шагов</param>
/// <param name="Private">Искать приватное свойство?</param>
public class AnimatedProperty<TObject, TValue>(
    TObject o,
    [NotNull] string Name,
    int Samples,
    int Timeout,
    Func<int, int, TValue> Translator,
    bool Private = false) : Property<TObject, TValue>(o, Name, Private)
{

    /// <summary>Интервал времени анимации в секундах</summary>
    private readonly int _Timeout = Timeout;

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

    /// <summary>Запуск анимации</summary>
    public void Start()
    {
        if (_Enabled) return;
        lock (this)
        {
            if (_Enabled) return;
            _Thread = new(Do) { Priority = _Priority };
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
            if (!_Thread.Join(2 * _Timeout) && _Thread.IsAlive)
                _Thread.Interrupt();
            _Thread = null;
        }
    }

    /// <summary>Метод, Выполняющий анимацию свойства</summary>
    private void Do()
    {
        var count = Samples;
        var timeout = _Timeout;
        while (Repeat)
        {
            for (var i = 0; _Enabled && i < count; i++)
            {
                Value = Translator(i, count);
                Thread.Sleep(timeout);
            }
            if (!AutoReverse) continue;
            for (var i = count - 1; _Enabled && i >= 0; i--)
            {
                Value = Translator(i, count);
                Thread.Sleep(timeout);
            }
        }
    }
}