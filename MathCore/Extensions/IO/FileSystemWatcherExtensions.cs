#nullable enable
using System.Linq.Reactive;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.IO;

public static class FileSystemWatcherExtensions
{
    public static IObservable<FileSystemEventArgs> ToObservable_Changed(this FileSystemWatcher Watcher) => Watcher.FromEvent<FileSystemEventArgs>("Changed");

    public static IObservable<FileSystemEventArgs> ToObservable_Created(this FileSystemWatcher Watcher) => Watcher.FromEvent<FileSystemEventArgs>("Created");

    public static IObservable<FileSystemEventArgs> ToObservable_Deleted(this FileSystemWatcher Watcher) => Watcher.FromEvent<FileSystemEventArgs>("Deleted");

    public static IObservable<FileSystemEventArgs> ToObservable_Disposed(this FileSystemWatcher Watcher) => Watcher.FromEvent<FileSystemEventArgs>("Disposed");

    public static LambdaDisposableObject<FileSystemWatcher> SuspendEvents(this FileSystemWatcher watcher) => new(watcher.InitializeObject(w => w!.EnableRaisingEvents = false)!, (w, s) => w.EnableRaisingEvents = (bool)s, watcher.EnableRaisingEvents);
}