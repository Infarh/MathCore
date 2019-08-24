namespace System
{
    public static class IDisposableExtensions
    {
        public static void Using<T>(this T obj, Action<T> action) where T : IDisposable
        {
            try
            {
                action(obj);
            }
            finally
            {
                obj.Dispose();
            }
        }
    }
}