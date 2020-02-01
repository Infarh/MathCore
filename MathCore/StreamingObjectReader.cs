using System.IO;
using System.Linq.Reactive;
using System.Runtime.CompilerServices;
using MathCore.Values;
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Класс поточного чтения объектов из потока данных</summary>
    /// <typeparam name="T">Тип читаемых объектов</typeparam>
    public abstract class StreamingObjectReader<T> : Processor, IObservable<T>
    // http://www.rsdn.ru/article/dotnet/ReactiveExtensions.xml
    {
        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Событие чтения нового объекта из потока данных</summary>
        public event EventHandler<EventArgs<T>> Readed;

        /// <summary>Источник события чтения объекта из потока данных</summary>
        /// <param name="e">Аргумент события, содержащий прочитанный объект</param>
        protected virtual void OnReaded(EventArgs<T> e) => Readed?.Invoke(this, e);

        /// <summary>Источник события чтения объекта из потока данных</summary>
        /// <param name="obj">Прочитанный объект</param>
        protected virtual void OnReaded(T obj)
        {
            OnReaded(new EventArgs<T>(obj));
            _ObservableObject?.OnNext(obj);
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Потока данных</summary>
#pragma warning disable CA2213 // Disposable fields should be disposed
        private readonly Stream _DataStream;
#pragma warning restore CA2213 // Disposable fields should be disposed

        private long _StartStreamPosition;
        private long _LastStreamPosition;

        private readonly StreamDataSpeedValue _Speed;
        private SimpleObservableEx<T> _ObservableObject;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Потока данных</summary>
        public Stream DataStream => _DataStream;

        /// <summary>Процент готовности</summary>
        public double Completed => _DataStream.Position / (double)_DataStream.Length;

        /// <summary>Скорость обработки данных</summary>
        public StreamDataSpeedValue Speed => _Speed;

        /// <summary>Оставшееся время до окончания обработки</summary>
        public TimeSpan? RemainingTime
        {
            get
            {
                var speed = Speed.Value;
                if(Math.Abs(speed) < double.Epsilon) return null;
                var length = DataStream.Length;
                var position = DataStream.Position;
                if(!Enable) return null;

                var length_left = length - position;
                var time_sec = length_left / speed;
                return double.IsNaN(time_sec) || double.IsInfinity(time_sec) 
                    ? (TimeSpan?)null 
                    : TimeSpan.FromSeconds(time_sec);
            }
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Инициализация нового экземпляра <see cref="StreamingObjectReader{T}"/></summary>
        /// <param name="DataStream">Поток байт из которого требуется читать объекты</param>
        protected StreamingObjectReader(Stream DataStream)
        {
            _Speed = new StreamDataSpeedValue(_DataStream = DataStream);
            Monitor.ProgressChecker = () => Completed;
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <inheritdoc />
        protected override void Initializer()
        {
            _LastStreamPosition = _StartStreamPosition = _DataStream.Position;
            _Speed.Reset();
            base.Initializer();
            Monitor.Status = "Reading...";
            if(_DataStream is FileStream stream)
                Monitor.Information = $"File:{stream.Name}";
            Monitor.InformationChecker = () =>
            {

                var time = RemainingTime;
                return time is null ? Speed.ToString() :
                    $"{Speed}: {((TimeSpan)time).ToShortString()}";
            };
        }

        protected abstract T Read();

        /// <inheritdoc />
        protected override void MainAction()
        {
            try
            {
                T obj;
                if(_DataStream.Position == _DataStream.Length || ((obj = Read()) is null))
                {
                    _Enabled = false;
                    return;
                }
                OnReaded(obj);
            } catch(Exception)
            {
                _Enabled = false;
                throw;
            }
        }

        public virtual void Reset()
        {
            lock (_StartStopSectionLocker)
            {
                Stop();
                _DataStream.Position = 0;
            }
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if(_ObservableObject is null) _ObservableObject = new SimpleObservableEx<T>();
            return _ObservableObject.Subscribe(observer);
        }

        /* ------------------------------------------------------------------------------------------ */
    }
}