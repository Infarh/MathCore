using System;

namespace MathCore.DataGenericSouces
{
    public abstract class DataSource<T>
    {
        public event EventHandler Updated;

        protected virtual void OnUpdated(EventArgs Args) => Updated.Start(this, Args);

        private readonly Func<DataHost<T>, T> _DataExtractor;

        private T _Value;

        public DataHost<T> Host { get; }

        public T Value => _Value;

        protected DataSource(DataHost<T> Host, Func<DataHost<T>, T> DataExtractor)
        {
            _DataExtractor = DataExtractor;
            this.Host = Host;
            Host.Updated += OnHostUpdated;
        }

        public T GetValue() => _Value = _DataExtractor(Host);

        protected virtual void OnHostUpdated(object sender, EventArgs e) => OnUpdated(e);
    }
}