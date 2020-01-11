// ReSharper disable once CheckNamespace
namespace System
{
    [Serializable]
    public delegate TReturn EventHandlerReturn<in TEventArgs, out TReturn>(object Sender, TEventArgs args)
                where TEventArgs : EventArgs;
}