// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    public readonly ref struct PauseToken
    {
        private static readonly Task __CompletedTask = Task.FromResult(true);

        private readonly PauseTokenSource _Source;

        public bool IsPaused => _Source != null && _Source.IsPaused;

        public Task WaitWhilePausedAsync() => IsPaused ? _Source.WaitWhilePausedAsync() : __CompletedTask;

        internal PauseToken(PauseTokenSource source) => _Source = source;
    }
}
