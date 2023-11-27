#nullable enable
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

namespace MathCore.Logging;

public sealed class Log : IEnumerable<LogItem>, INotifyPropertyChanged, INotifyCollectionChanged
{
    public class LogPool
    {
        private readonly Dictionary<string, Log> _LogDictionary = [];

        public Log this[string Name] => _LogDictionary.GetValueOrAddNew(Name, name => new Log(name));

        internal LogPool() { }

        public bool Remove(Log log) => _LogDictionary.Remove(log.Name);

        public void Clear() => _LogDictionary.Clear();

        public bool Contain(string Name) => _LogDictionary.ContainsKey(Name);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string PropertyName = null!) => PropertyChanged.Start(this, PropertyName);

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    private void OnCollectionChanged(NotifyCollectionChangedEventArgs Args) => CollectionChanged.Start(this, Args);

    public static LogPool Pool { get; } = new();

    private readonly string _Name;
    private LogType _Type;
    private readonly List<LogItem> _Items = [];

    public int ItemsCount => _Items.Count;

    public LogItem? First => ItemsCount == 0 ? null : _Items[0];
    public LogItem? Last => ItemsCount == 0 ? null : _Items[^1];

    public TimeSpan TimeDelta
    {
        get
        {
            this.GetMinMax(i => i.Time.Ticks, out var begin, out var end);
            return end.Time - begin.Time;
        }
    }

    public string Name => _Name;

    public LogType Type
    {
        get => _Type;
        set
        {
            if(_Type == value) return;
            _Type = value;
            OnPropertyChanged();
        }
    }

    public LogItem this[int Index] => _Items[Index];

    public LogItem this[DateTime time] => _Items
       .Select(item => (item, delta: (item.Time - time).TotalSeconds.Abs()))
       .GetMin(i => i.delta)
       .item;

    public LogItem? this[string value] => _Items.FirstOrDefault(i => i.Value == value);

    private Log(string Name) => _Name = Name;

    public void Clear()
    {
        _Items.Clear();
        OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
    }

    public LogItem Add(string value, LogType type = LogType.Information) => Add(DateTime.Now, value, type);

    public LogItem Add(DateTime time, string value, LogType type = LogType.Information)
    {
        var item = new LogItem(time, value, type);
        _Items.Add(item);
        OnCollectionChanged(new(NotifyCollectionChangedAction.Add, new[] { item }));
        return item;
    }

    public override string ToString() => $"{_Name}[{_Type}]:{ItemsCount}";

    public IEnumerator<LogItem> GetEnumerator() => _Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}