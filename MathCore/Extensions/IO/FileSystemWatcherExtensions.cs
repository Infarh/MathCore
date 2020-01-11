using System.Linq.Reactive;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.IO
{
    public static class FileSystemWatcherExtensions
    {
        [NotNull] public static IObservable<FileSystemEventArgs> ToObservable_Changed([NotNull] this FileSystemWatcher Watcher) => Watcher.FromEvent<FileSystemEventArgs>("Changed");

        [NotNull] public static IObservable<FileSystemEventArgs> ToObservable_Created([NotNull] this FileSystemWatcher Watcher) => Watcher.FromEvent<FileSystemEventArgs>("Created");

        [NotNull] public static IObservable<FileSystemEventArgs> ToObservable_Deleted([NotNull] this FileSystemWatcher Watcher) => Watcher.FromEvent<FileSystemEventArgs>("Deleted");

        [NotNull] public static IObservable<FileSystemEventArgs> ToObservable_Disposed([NotNull] this FileSystemWatcher Watcher) => Watcher.FromEvent<FileSystemEventArgs>("Disposed");

        [NotNull] public static LambdaDisposableObject<FileSystemWatcher> SuspendEvents([NotNull] this FileSystemWatcher watcher) => new LambdaDisposableObject<FileSystemWatcher>(watcher.InitializeObject(w => w.EnableRaisingEvents = false), (w, s) => w.EnableRaisingEvents = (bool)s, watcher.EnableRaisingEvents);
    }
}