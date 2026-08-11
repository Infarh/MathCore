using System.ComponentModel;
using System.Runtime.CompilerServices;

using MathCore.Annotations;
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore;

/// <summary>Монитор прогресса операции, поддерживающий уведомление об изменении свойств</summary>
public class ProgressMonitor : INotifyPropertyChanged
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Событие изменения свойства</summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    /// <summary>Вызов события изменения свойства</summary>
    /// <param name="e">Аргументы события</param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

    /// <summary>Вызов события изменения свойства по имени</summary>
    /// <param name="PropertyName">Имя свойства</param>
    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string PropertyName = null!) => OnPropertyChanged(new PropertyChangedEventArgs(PropertyName));

    /// <summary>Событие изменения статуса</summary>
    public event EventHandler? StatusChanged;
    /// <summary>Событие изменения проверки статуса</summary>
    public event EventHandler? StatusCheckerChanged;
    /// <summary>Событие изменения информации</summary>
    public event EventHandler? InformationChanged;
    /// <summary>Событие изменения проверки информации</summary>
    public event EventHandler? InformationCheckerChanged;
    /// <summary>Событие изменения прогресса</summary>
    public event EventHandler? ProgressChanged;
    /// <summary>Событие изменения проверки прогресса</summary>
    public event EventHandler? ProgressCheckerChanged;

    /* ------------------------------------------------------------------------------------------ */

    private double _Progress;
    private string _Information = null!;
    private string _Status = null!;

    private Func<string>? _StatusStrFunc;
    private Func<string>? _InformationStrFunc;
    private Func<double>? _ProgressFunc;

    private ProgressMonitor? _ConnectedMonitor;

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Функция получения статуса</summary>
    public Func<string> StatusChecker
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => _StatusStrFunc!;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
            if (value == _StatusStrFunc) return;
            _StatusStrFunc = value;
            StatusCheckerChanged.FastStart(this);
            OnPropertyChanged();
        }
    }

    /// <summary>Функция получения информации</summary>
    public Func<string> InformationChecker
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => _InformationStrFunc!;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
            if (value == _InformationStrFunc) return;
            _InformationStrFunc = value;
            InformationCheckerChanged.FastStart(this);
            OnPropertyChanged();
        }
    }

    /// <summary>Функция получения прогресса</summary>
    public Func<double> ProgressChecker
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => _ProgressFunc!;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
            if (value == _ProgressFunc) return;
            _ProgressFunc = value;
            ProgressCheckerChanged.FastStart(this);
            OnPropertyChanged();
        }
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Текущий статус операции</summary>
    public string Status
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get
        {
            var status_f = _StatusStrFunc;
            return status_f is null ? _Status : Status = status_f();
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
            if (_Status == value) return;
            _Status = value;
            StatusChanged.FastStart(this);
            OnPropertyChanged();
        }
    }

    /// <summary>Дополнительная информация об операции</summary>
    public string Information
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get
        {
            var information_f = _InformationStrFunc;
            return information_f is null ? _Information : Information = information_f();
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
            if (_Information == value) return;
            _Information = value;
            InformationChanged.FastStart(this);
            OnPropertyChanged();
        }
    }

    /// <summary>Текущий прогресс операции (0..1)</summary>
    public double Progress
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get
        {
            var progress_f = _ProgressFunc;
            return progress_f is null ? _Progress : Progress = progress_f();
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
            if (Math.Abs(_Progress - value) < double.Epsilon) return;
            _Progress = value;
            ProgressChanged.FastStart(this);
            OnPropertyChanged();
        }
    }

    /// <summary>Строковое представление прогресса в процентах</summary>
    public string ProgressStr
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => $"{(Progress * 100).Round(2)}%";
    }

    /// <summary>Подключённый монитор</summary>
    public ProgressMonitor ConnectedMonitor
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => _ConnectedMonitor!;
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Конструктор монитора с функциями получения значений</summary>
    /// <param name="ProgressFunc">Функция получения прогресса</param>
    /// <param name="StatusFunc">Функция получения статуса</param>
    /// <param name="InformationFunc">Функция получения информации</param>
    public ProgressMonitor(
        Func<double>? ProgressFunc = null,
        Func<string>? StatusFunc = null,
        Func<string>? InformationFunc = null)
    {
        _StatusStrFunc = StatusFunc;
        _InformationStrFunc = InformationFunc;
        _ProgressFunc = ProgressFunc;
    }

    /// <summary>Конструктор монитора с начальными значениями</summary>
    /// <param name="Status">Начальный статус</param>
    /// <param name="Information">Начальная информация</param>
    /// <param name="Progress">Начальный прогресс</param>
    public ProgressMonitor(string Status, string Information = "", double Progress = 0)
    {
        _Status = Status;
        _Information = Information;
        _Progress = Progress;
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Подключает монитор для извлечения значений по функциям проверки</summary>
    /// <param name="Monitor">Подключаемый монитор</param>
    /// <exception cref="ArgumentException">Если монитор пытается подключиться к себе</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Connect(ProgressMonitor Monitor)
    {
        if (ReferenceEquals(Monitor, this))
            throw new ArgumentException("Нельзя подключать монитор к себе по методам изъятия значений");

        if (_ConnectedMonitor != null) ClearEventHandlers();
        _ConnectedMonitor = Monitor;
        StatusChecker = () => Monitor.Status;
        ProgressChecker = () => Monitor.Progress;
        InformationChecker = () => Monitor.Information;
    }

    /// <summary>Устанавливает сильную связь с монитором по обработчикам событий</summary>
    /// <param name="Monitor">Подключаемый монитор</param>
    /// <exception cref="ArgumentException">Если монитор пытается подключиться к себе</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void ConnectStrong(ProgressMonitor Monitor)
    {
        if (ReferenceEquals(Monitor, this))
            throw new ArgumentException("Нельзя подключать монитор к себе по обработчикам событий");

        Disconnect();
        _ConnectedMonitor = Monitor;
        Monitor.StatusChanged += OnMonitorStatusChanged;
        Monitor.InformationChanged += OnMonitorInformationChanged;
        Monitor.ProgressChanged += OnMonitorProgressChanged;
    }

    /// <summary>Отключает монитор</summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Disconnect()
    {
        StatusChecker = null!;
        ProgressChecker = null!;
        InformationChecker = null!;
        _ConnectedMonitor = null;
    }

    /// <summary>Устанавливает статус, прогресс и информацию одновременно</summary>
    /// <param name="status">Статус</param>
    /// <param name="progress">Прогресс</param>
    /// <param name="information">Информация</param>
    public void SetStatus(string status, double progress, string information)
    {
        Status = status;
        Progress = progress;
        Information = information;
    }

    /// <summary>Очищает обработчики событий подключённого монитора</summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void ClearEventHandlers()
    {
        if (_ConnectedMonitor is null) return;

        _ConnectedMonitor.StatusChanged -= OnMonitorStatusChanged;
        _ConnectedMonitor.InformationChanged -= OnMonitorInformationChanged;
        _ConnectedMonitor.ProgressChanged -= OnMonitorProgressChanged;
        _ConnectedMonitor = null;
    }

    private void OnMonitorStatusChanged(object? sender, EventArgs e)
    {
        var monitor = (ProgressMonitor)sender!;
        Status = monitor.Status;
    }

    private void OnMonitorInformationChanged(object? sender, EventArgs e) => Information = ((ProgressMonitor)sender!).Information;

    private void OnMonitorProgressChanged(object? sender, EventArgs e) => Progress = ((ProgressMonitor)sender!).Progress;

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Строковое представление состояния монитора</summary>
    /// <returns>Строка вида "Статус:(процент) - Информация"</returns>
    public override string ToString() => $"{Status}:({ProgressStr}) - {Information}";

    /* ------------------------------------------------------------------------------------------ */
}