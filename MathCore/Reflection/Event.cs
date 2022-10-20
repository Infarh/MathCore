using MathCore.Annotations;
// ReSharper disable EventNeverSubscribedTo.Global

// ReSharper disable once CheckNamespace
namespace System.Reflection;

/// <summary>Событие</summary>
/// <typeparam name="TObject">Тип объекта-источника события</typeparam>
/// <typeparam name="TEventArgs">Тип аргумента события</typeparam>
public class Event<TObject, TEventArgs> where TEventArgs : EventArgs
{
    /// <summary>Событие</summary>
    public event EventHandler<TEventArgs> EventHandler
    {
        add => _EventInfo.AddEventHandler(_Object, value);
        remove => _EventInfo.RemoveEventHandler(_Object, value);
    }

    /// <summary>Описание события</summary>
    private readonly EventInfo _EventInfo;
    /// <summary>Объект-источник</summary>
    private readonly TObject _Object;

    /// <summary>Событие</summary>
    /// <param name="o">Объект-источник события</param>
    /// <param name="Name">Название события</param>
    /// <param name="Private">Приватность описания события в классе объекта</param>
    public Event([CanBeNull] TObject o, [NotNull] string Name, bool Private = false)
    {
        _Object = o;

        var type = typeof(TObject);
        if (type == typeof(object) && o != null)
            type = o.GetType();

        var is_private = Private ? BindingFlags.NonPublic : BindingFlags.Public;
        var is_static  = o is null ? BindingFlags.Static : BindingFlags.Instance;

        _EventInfo = type.GetEvent(Name, is_private | is_static);
    }
}