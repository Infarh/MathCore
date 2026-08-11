using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

namespace MathCore.Logging;

/// <summary>Лог-журнал, содержащий коллекцию элементов <see cref="LogItem"/></summary>
public sealed class Log : IEnumerable<LogItem>, INotifyPropertyChanged, INotifyCollectionChanged
{
    /// <summary>Пул лог-журналов</summary>
    public class LogPool
    {
        private readonly Dictionary<string, Log> _LogDictionary = [];

        /// <summary>Лог по имени</summary>
        /// <param name="Name">Имя лога</param>
        /// <returns>Лог</returns>
        public Log this[string Name] => _LogDictionary.GetValueOrAddNew(Name, name => new(name));

        internal LogPool() { }

        /// <summary>Удаляет лог из пула</summary>
        /// <param name="log">Удаляемый лог</param>
        /// <returns>Истина, если лог удалён</returns>
        public bool Remove(Log log) => _LogDictionary.Remove(log.Name);

        /// <summary>Очищает пул логов</summary>
        public void Clear() => _LogDictionary.Clear();

        /// <summary>Содержит ли пул лог с указанным именем</summary>
        /// <param name="Name">Имя лога</param>
        /// <returns>Истина, если лог существует</returns>
        public bool Contain(string Name) => _LogDictionary.ContainsKey(Name);
    }

    /// <summary>Событие изменения свойства</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Вызов события изменения свойства</summary>
    /// <param name="PropertyName">Имя свойства</param>
    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string PropertyName = null!) => PropertyChanged.Start(this, PropertyName);

    /// <summary>Событие изменения коллекции</summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    private void OnCollectionChanged(NotifyCollectionChangedEventArgs Args) => CollectionChanged.Start(this, Args);

    /// <summary>Пул логов по умолчанию</summary>
    public static LogPool Pool { get; } = new();

    private readonly string _Name;
    private LogType _Type;
    private readonly List<LogItem> _Items = [];

    /// <summary>Количество элементов лога</summary>
    public int ItemsCount => _Items.Count;

    /// <summary>Первый элемент лога</summary>
    public LogItem? First => ItemsCount == 0 ? null : _Items[0];
    /// <summary>Последний элемент лога</summary>
    public LogItem? Last => ItemsCount == 0 ? null : _Items[^1];

    /// <summary>Временной интервал между первым и последним элементами</summary>
    public TimeSpan TimeDelta
    {
        get
        {
            this.GetMinMax(i => i.Time.Ticks, out var begin, out var end);
            if (begin is null || end is null) return TimeSpan.Zero;
            return end.Time - begin.Time;
        }
    }

    /// <summary>Имя лога</summary>
    public string Name => _Name;

    /// <summary>Тип лога</summary>
    public LogType Type
    {
        get => _Type;
        set
        {
            if (_Type == value) return;
            _Type = value;
            OnPropertyChanged();
        }
    }

    /// <summary>Элемент лога по индексу</summary>
    /// <param name="Index">Индекс элемента</param>
    /// <returns>Элемент лога</returns>
    public LogItem this[int Index] => _Items[Index];

    /// <summary>Элемент лога, ближайший по времени</summary>
    /// <param name="time">Время</param>
    /// <returns>Элемент лога</returns>
    public LogItem this[DateTime time] => _Items
       .Select(item => (item, delta: (item.Time - time).TotalSeconds.Abs()))
       .GetMin(i => i.delta)
       .item;

    /// <summary>Элемент лога по значению</summary>
    /// <param name="value">Значение элемента</param>
    /// <returns>Элемент лога или null</returns>
    public LogItem? this[string value] => _Items.FirstOrDefault(i => i.Value == value);

    private Log(string Name) => _Name = Name;

    /// <summary>Очищает лог</summary>
    public void Clear()
    {
        _Items.Clear();
        OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>Добавляет элемент в лог</summary>
    /// <param name="value">Значение</param>
    /// <param name="type">Тип записи</param>
    /// <returns>Добавленный элемент</returns>
    public LogItem Add(string value, LogType type = LogType.Information) => Add(DateTime.Now, value, type);

    /// <summary>Добавляет элемент в лог с указанным временем</summary>
    /// <param name="time">Время записи</param>
    /// <param name="value">Значение</param>
    /// <param name="type">Тип записи</param>
    /// <returns>Добавленный элемент</returns>
    public LogItem Add(DateTime time, string value, LogType type = LogType.Information)
    {
        var item = new LogItem(time, value, type);
        _Items.Add(item);
        OnCollectionChanged(new(NotifyCollectionChangedAction.Add, new[] { item }));
        return item;
    }

    /// <summary>Строковое представление лога</summary>
    /// <returns>Строковое представление</returns>
    public override string ToString() => $"{_Name}[{_Type}]:{ItemsCount}";

    /// <summary>Перечислитель элементов лога</summary>
    /// <returns>Перечислитель</returns>
    public IEnumerator<LogItem> GetEnumerator() => _Items.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}