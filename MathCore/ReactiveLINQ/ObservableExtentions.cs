using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable ObjectCreationAsStatement

// ReSharper disable UnusedMethodReturnValue.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive
{
    /// <summary>Методы-расширения интерфейса наблюдаемых объектов <see cref="IObservableEx{T}"/></summary>
    public static class ObservableExtentions
    {
        /// <summary>Добавить наблюдатель в список наблюдателей и получить объект-отписчик</summary>
        /// <typeparam name="T">Тип значений наблюдаемого объекта</typeparam>
        /// <param name="Observers">Коллекция наблюдателей</param>
        /// <param name="Observer">Добавляемый наблюдатель</param>
        /// <returns>Объект, удаляющий наблюдатель из списка наблюдателей в случае своей отписки</returns>
        [NotNull]
        public static IDisposable AddObserver<T>([NotNull] this ICollection<IObserver<T>> Observers, [NotNull] IObserver<T> Observer) => ObserverLink<T>.GetLink(Observers, Observer);

        public static void OnCompleted<T>([CanBeNull] this IEnumerable<IObserver<T>> Observers)
        {
            if (Observers is null) return;
            foreach(var observer in Observers)
                observer.OnCompleted();
        }

        public static void OnError<T>([CanBeNull] this IEnumerable<IObserver<T>> Observers, Exception error)
        {
            if (Observers is null) return;
            foreach(var observer in Observers)
                observer.OnError(error);
        }

        public static void OnNext<T>([CanBeNull] this IEnumerable<IObserver<T>> Observers, T value)
        {
            if (Observers is null) return;
            foreach(var observer in Observers)
                observer.OnNext(value);
        }

        public static void OnReset<T>([CanBeNull] this IEnumerable<IObserverEx<T>> Observers)
        {
            if (Observers is null) return;
            foreach(var observer in Observers)
                observer.OnReset();
        }

        /// <summary>Метод получения наблюдаемого объекта для объекта, реализующего интерфейс <see cref="INotifyPropertyChanged"/>  для указанного имени свойства</summary>
        /// <typeparam name="T">Тип значения свойства</typeparam>
        /// <param name="obj">Наблюдаемый объект</param>
        /// <param name="ProperyName">Имя свойства</param>
        /// <returns>Объект-наблюдатель за свойством</returns>
        [NotNull]
        public static IObservable<T> FromProperty<T>([NotNull] this INotifyPropertyChanged obj, [NotNull] string ProperyName) => new Property<T>(obj, ProperyName);

        /// <summary>Метод фильтрации событий</summary>
        /// <typeparam name="T">Тип объектов событий</typeparam>
        /// <param name="observable">Исходный объект-наблюдатель</param>
        /// <param name="Predicate">Метод фильтрации</param>
        /// <returns>Объекнаблюдатель с установленным методом фильтрации</returns>
        [NotNull]
        public static IObservableEx<T> Where<T>([NotNull] this IObservable<T> observable, [NotNull] Func<T, bool> Predicate) => new WhereLamdaObservableEx<T>(observable, Predicate);

        /// <summary>Метод фильтрации событий</summary>
        /// <typeparam name="T">Тип объектов событий</typeparam>
        /// <param name="observable">Исходный объект-наблюдатель</param>
        /// <param name="predicate">Метод фильтрации</param>
        /// <param name="ElseAction">Метод обработки невошедших событий</param>
        /// <returns>Объекнаблюдатель с установленным методом фильтрации</returns>
        [NotNull]
        public static IObservableEx<T> Where<T>([NotNull] this IObservable<T> observable, [NotNull] Func<T, bool> predicate, [NotNull] Action<T> ElseAction)
        {
            bool Predicate(T t)
            {
                var result = predicate(t);
                if (!result) ElseAction(t);
                return result;
            }

            return new WhereLamdaObservableEx<T>(observable, Predicate);
        }

        /// <summary>Метод преобразования объектов событий</summary>
        /// <typeparam name="T">Тип исходных объектов событий</typeparam>
        /// <typeparam name="Q">Тип результирующих объектов событий</typeparam>
        /// <param name="observable">Исходный объект-наблюдатель</param>
        /// <param name="Selector">Объект-наблюдатель с преобразованными объектами событий</param>
        /// <returns>Объект-наблюдатель с преобразованными типами объектов</returns>
        [NotNull]
        public static IObservableEx<Q> Select<T, Q>([NotNull] this IObservable<T> observable, [NotNull] Func<T, Q> Selector) => new SelectLamdaObservableEx<T, Q>(observable, Selector);

        [NotNull]
        public static TimeIntervalObservable Interval_Seconds(double TimeInterval, bool Started = false) => Interval(TimeSpan.FromSeconds(TimeInterval), Started);

        [NotNull]
        public static TimeIntervalObservable Interval_MiliSeconds(double TimeInterval, bool Started = false) => Interval(TimeSpan.FromMilliseconds(TimeInterval), Started);

        [NotNull]
        public static TimeIntervalObservable Interval_Minutes(double TimeInterval, bool Started = false) => Interval(TimeSpan.FromMinutes(TimeInterval), Started);

        [NotNull]
        public static TimeIntervalObservable Interval_Hours(double TimeInterval, bool Started = false) => Interval(TimeSpan.FromHours(TimeInterval), Started);

        [NotNull]
        public static TimeIntervalObservable Interval(this TimeSpan TimeInterval, bool Started = false) => new TimeIntervalObservable(TimeInterval, Started);

        /// <summary>Метод получения синхронно задержанных во времени событий</summary>
        /// <typeparam name="T">Тип объектов событий</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="Interval">Интервал времени задержки событий</param>
        /// <returns>ОБъект-наблюдатель, события которого синхронно задержаны во времени на указанный интервал</returns>
        [NotNull]
        public static IObservableEx<T> WhaitSync<T>([NotNull] this IObservable<T> Observable, TimeSpan Interval) => new LamdaObservable<T>(Observable, (o, t) => { Thread.Sleep(Interval); o.OnNext(t); });

        /// <summary>Метод получения задержанных во времени событий</summary>
        /// <typeparam name="T">Тип объектов событий</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="Interval">Интервал времени задержки событий</param>
        /// <returns>ОБъект-наблюдатель, события которого задержаны во времени на указанный интервал</returns>
        [NotNull]
        public static IObservableEx<T> WhaitAsync<T>(this IObservable<T> Observable, TimeSpan Interval)
        {
            Action<IObserver<T>, T> OnNext = (o, t) => { Thread.Sleep(Interval); o.OnNext(t); };
            void NextAsync(IObserver<T> o, T t) => OnNext.BeginInvoke(o, t, null, null);
            return new LamdaObservable<T>(Observable, NextAsync);
        }

        /// <summary>Метод получения объекта-наблюдателя для указанного события</summary>
        /// <typeparam name="TEventArgs">Тип аргументов события</typeparam>
        /// <param name="Obj">Наблюдаемый объект</param>
        /// <param name="EventName">Имя события</param>
        /// <returns>Объект-ниблюдатель за событием</returns>
        [NotNull]
        public static IObservableEx<TEventArgs> FromEvent<TEventArgs>([NotNull] this object Obj, [NotNull] string EventName)
            where TEventArgs : EventArgs =>
            new EventObservableEx<TEventArgs>(Obj, EventName);

        /// <summary>Метод получения объекта-наблюдателя из объекта-перечисления</summary>
        /// <typeparam name="T">Тип объектов перечисления</typeparam>
        /// <param name="collection">Перечисление объектов</param>
        /// <param name="observable">Созданный объект-наблюдатель за перечислением объектов коллекции</param>
        /// <returns>Новое перечисление объектов, перечисление объектов которого вызывает события в наблюдателе</returns>
        [NotNull]
        public static IEnumerable<T> GetObservable<T>([NotNull] this IEnumerable<T> collection, [NotNull] out IObservable<T> observable)
        {
            observable = new SimpleObservableEx<T>();
            return collection
                .ForeachLazy(((SimpleObservableEx<T>)observable).OnNext)
                .OnComplite(((SimpleObservableEx<T>)observable).OnCompleted);
        }

        [NotNull]
        public static IObservable<T> ToObservable<T>(this IEnumerable<T> collection) => new ObservableCollectionEnumerator<T>(collection);

        /// <summary>Метод получения объекта-нибулюдателя, пропускающего после создания указанное число событий</summary>
        /// <typeparam name="T">Тип объектов события</typeparam>
        /// <param name="observable">Исходный объект-наблюдатель</param>
        /// <param name="Count">Количество пропускаемых событий</param>
        /// <returns>Объект-наблюдатель с указанным количеством пропускаемых событий</returns>
        [NotNull]
        public static IObservableEx<T> Take<T>([NotNull] this IObservable<T> observable, int Count) => new TakeObservable<T>(observable, Count);

        /// <summary>Метод обработки последовательности событий с учётом разрешающей и запрещающей последовательностей</summary>
        /// <typeparam name="T">Тип объектов событий наблюдаемого объекта</typeparam>
        /// <typeparam name="O">Тип объектов событий разрешающей последовательности</typeparam>
        /// <typeparam name="C">Тип объектов событий запрещающей последовательности</typeparam>
        /// <param name="source">Объект-наблюдатель источник событий</param>
        /// <param name="Open">Объект-наблюдатель разрешающий событий в выходной последовательности</param>
        /// <param name="Close">Объект-наблюдатель запрещающий событий в выходной последовательности</param>
        /// <param name="IsOpen">Исходное состояния разрешения событий в выходной последовательности (по умолчанию разрешено)</param>
        /// <returns>Управляемый объект-наблюдатель</returns>
        [NotNull]
        public static IObservableEx<T> Take<T, O, C>([NotNull] this IObservable<T> source, [NotNull] IObservable<O> Open, [NotNull] IObservable<C> Close, bool IsOpen = true)
        {
            var t = source as TriggeredObservable<T> ?? new TriggeredObservable<T>(source, IsOpen);
            Open.ForeachAction(o => t.Open = true);
            Close.ForeachAction(c => t.Open = false);
            return t;
        }

        /// <summary>Метод получения объекта-наблюдателя, события в котором пропускаются в выходную последовательность только до появления события в управляющей последовательности</summary>
        /// <typeparam name="T">Тип исходных объектов события</typeparam>
        /// <typeparam name="Q">Тип события управляющей последовательности</typeparam>
        /// <param name="source">Исходный объект-наблюдатель</param>
        /// <param name="Selector">Обект-наблюдатель управляющей последовательности</param>
        /// <param name="IsOpen">Исходное состояние выходной последовательности</param>
        /// <returns>Объект-наблюдатель управляемый управляющей последовательностью</returns>
        [NotNull]
        public static IObservableEx<T> TakeUntil<T, Q>(this IObservable<T> source, IObservable<Q> Selector, bool IsOpen = true)
        {
            var o = source as TriggeredObservable<T> ?? new TriggeredObservable<T>(source, IsOpen);
            Selector.ForeachAction(q => o.Open = false);
            return o;
        }

        /// <summary>Метод получения объекта-наблюдателя, события в котором пропускаются в выходную последовательность только после появления события в управляющей последовательности</summary>
        /// <typeparam name="T">Тип исходных объектов события</typeparam>
        /// <typeparam name="Q">Тип события управляющей последовательности</typeparam>
        /// <param name="source">Исходный объект-наблюдатель</param>
        /// <param name="Selector">Обект-наблюдатель управляющей последовательности</param>
        /// <param name="IsOpen">Исходное состояние выходной последовательности</param>
        /// <returns>Объект-наблюдатель управляемый управляющей последовательностью</returns>
        [NotNull]
        public static IObservableEx<T> SkipWhile<T, Q>(this IObservable<T> source, IObservable<Q> Selector, bool IsOpen = false)
        {
            var o = source as TriggeredObservable<T> ?? new TriggeredObservable<T>(source, IsOpen);
            Selector.ForeachAction(q => o.Open = true);
            return o;
        }

        /// <summary>Метод обработки события <see cref="IObserverEx{T}.Next"/></summary>
        /// <typeparam name="T">Тип объектов наблюдения</typeparam>
        /// <param name="observable">Исходный обект-наблюдатель</param>
        /// <param name="action">Метод обработки события <see cref="IObserverEx{T}.Next"/></param>
        /// <returns>Исходный объект-наблюдатель</returns>
        [CanBeNull]
        public static IObservable<T> ForeachAction<T>([NotNull] this IObservable<T> observable, [NotNull] Action<T> action) => 
            observable.InitializeObject(action, (o, a) => new LamdaObserver<T>(o, a)); 

        [NotNull]
        public static IDisposable Subscribe<T>([NotNull] this IObservable<T> ObservableObject, [NotNull] Action<T> action) => new LamdaObserver<T>(ObservableObject, action);

        /// <summary>Метод обработки события <see cref="IObserverEx{T}.Next"/></summary>
        /// <typeparam name="T">Тип объектов наблюдения</typeparam>
        /// <param name="observable">Исходный обект-наблюдатель</param>
        /// <param name="action">Метод обработки события <see cref="IObserverEx{T}.Next"/></param>
        /// <param name="where">Метод выборки события <see cref="IObserverEx{T}.Next"/></param>
        /// <returns>Исходный объект-наблюдатель</returns>
        [CanBeNull]
        public static IObservable<T> ForeachAction<T>([NotNull] this IObservable<T> observable, [NotNull] Action<T> action, [NotNull] Func<T, bool> where) => observable.InitializeObject(where, action, (o, w, a) => new LamdaObserver<T>(o, t => { if(w(t)) a(t); }));

        /// <summary>Метод обработки события <see cref="IObserverEx{T}.Next"/></summary>
        /// <typeparam name="T">Тип объектов наблюдения</typeparam>
        /// <param name="observable">Исходный обект-наблюдатель</param>
        /// <param name="action">Метод обработки события <see cref="IObserverEx{T}.Next"/></param>
        /// <returns>Исходный объект-наблюдатель</returns>
        [CanBeNull]
        public static IObservable<T> ForeachAction<T>(this IObservable<T> observable, Action<T, int> action)
        {
            var i = 0;
            // ReSharper disable once HeapView.CanAvoidClosure
            return observable.InitializeObject(o => new LamdaObserver<T>(o, t => action(t, i++)));
        }

        /// <summary>Метод обработки события <see cref="IObserverEx{T}.Next"/></summary>
        /// <typeparam name="T">Тип объектов наблюдения</typeparam>
        /// <param name="observable">Исходный обект-наблюдатель</param>
        /// <param name="action">Метод обработки события <see cref="IObserverEx{T}.Next"/></param>
        /// <param name="where">Метод выборки события <see cref="IObserverEx{T}.Next"/></param>
        /// <returns>Исходный объект-наблюдатель</returns>
        [CanBeNull]
        public static IObservable<T> ForeachAction<T>(this IObservable<T> observable, Action<T, int> action, Func<T, int, bool> where)
        {
            var i = 0;
            // ReSharper disable once HeapView.CanAvoidClosure
            return observable.InitializeObject(o => new LamdaObserver<T>(o, t => { if(where(t, i)) action(t, i++); }));
        }

        /// <summary>Метод обработки события <see cref="Exception"/></summary>
        /// <typeparam name="T">Тип объектов наблюдения</typeparam>
        /// <param name="observable">Исходный обект-наблюдатель</param>
        /// <param name="OnError">Метод обработки события <see cref="Exception"/></param>
        /// <returns>Исходный объект-наблюдатель</returns>
        [CanBeNull]
        public static IObservable<T> OnError<T>(this IObservable<T> observable, Action<Exception> OnError) => observable.InitializeObject(OnError, (o, e) => new LamdaObserver<T>(o, OnError: e));

        /// <summary>Метод обработки события <see cref="IObserverEx{T}.Complited"/></summary>
        /// <typeparam name="T">Тип объектов наблюдения</typeparam>
        /// <param name="observable">Исходный обект-наблюдатель</param>
        /// <param name="OnComplited">Метод обработки события <see cref="IObserverEx{T}.Complited"/></param>
        /// <returns>Исходный объект-наблюдатель</returns>
        [CanBeNull]
        public static IObservable<T> OnComplited<T>(this IObservable<T> observable, Action OnComplited) => observable.InitializeObject(OnComplited, (o, c) => new LamdaObserver<T>(o, OnComplited: c));

        /// <summary>Метод обработки события <see cref="IObserverEx{T}.Reset"/></summary>
        /// <typeparam name="T">Тип объектов наблюдения</typeparam>
        /// <param name="observable">Исходный обект-наблюдатель</param>
        /// <param name="OnReset">Метод обработки события <see cref="IObserverEx{T}.Reset"/></param>
        /// <returns>Исходный объект-наблюдатель</returns>
        [CanBeNull]
        public static IObservable<T> OnReset<T>(this IObservable<T> observable, Action OnReset) => observable.InitializeObject(OnReset, (o, r) => new LamdaObserver<T>(o, OnReset: r));

        /// <summary>Создать метод генерации наблюдаемого объекта из шаблона асинхронной операции</summary>
        /// <typeparam name="T">Тип результата</typeparam>
        /// <param name="BeginInvoke">Метод начала асинхронной операции</param>
        /// <param name="EndInvoke">Метод завершения асинхронной операции</param>
        /// <returns>Фунцкия, возвращающая наблюдаемый объект, генерирующий своё значение в момент завершения асинхронной операции</returns>
        [NotNull]
        public static Func<IObservableEx<T>> FromAsyncPattern<T>(Func<AsyncCallback, object, IAsyncResult> BeginInvoke, Func<IAsyncResult, T> EndInvoke) => () => new AsyncPatternObservable<T>(BeginInvoke, EndInvoke);

        /// <summary>Метод-лианиризатор событий для наблюдаемого объекта, возвращающего коллекцию объектов типа <see cref="T"/></summary>
        /// <typeparam name="T">Тип результирующих объектов</typeparam>
        /// <param name="o">Объект-наблюдатель коллекции</param>
        /// <returns>Объект-наблюдатель элементов коллекции</returns>
        [NotNull]
        public static IObservableEx<T> SelectMany<T>(this IObservable<IEnumerable<T>> o)
        {
            var result = new SimpleObservableEx<T>();
            o.ForeachAction(t => t.Foreach(result.OnNext));
            o.OnComplited(result.OnCompleted);
            (o as IObservableEx<IEnumerable<T>>)?.OnReset(result.OnReset);
            o.OnError(result.OnError);
            return result;
        }

        [NotNull]
        public static IObservableEx<TResult> SelectMany<TSource, TResult>(
            [NotNull] this IObservable<TSource> source,
            [NotNull] Func<TSource, IEnumerable<TResult>> selector)
        {
            if(source is null) throw new ArgumentNullException(nameof(source));
            if(selector is null) throw new ArgumentNullException(nameof(selector));

            var result = new SimpleObservableEx<TResult>();
            source.ForeachAction(t => selector(t).Foreach(result.OnNext));
            source.OnComplited(result.OnCompleted);
            (source as IObservableEx<IEnumerable<TSource>>)?.OnReset(result.OnReset);
            source.OnError(result.OnError);
            return result;
        }

        [NotNull]
        public static IObservableEx<TResult> SelectMany<TSource, TResult>(
            [NotNull] this IObservable<TSource> source,
            [NotNull] Func<TSource, int, IEnumerable<TResult>> selector)
        {
            if(source is null) throw new ArgumentNullException(nameof(source));
            if(selector is null) throw new ArgumentNullException(nameof(selector));

            var result = new SimpleObservableEx<TResult>();
            var i = 0;
            source.ForeachAction(t => selector(t, i++).Foreach(result.OnNext));
            source.OnComplited(result.OnCompleted);
            (source as IObservableEx<IEnumerable<TSource>>)?.OnReset(result.OnReset);
            source.OnError(result.OnError);
            return result;
        }

        [NotNull]
        public static IObservableEx<TResult> SelectMany<TSource, TCollection, TResult>(
            [NotNull] this IObservable<TSource> source,
            [NotNull] Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
            [NotNull] Func<TSource, TCollection, TResult> ResultSelector)
        {
            if(source is null) throw new ArgumentNullException(nameof(source));
            if(collectionSelector is null) throw new ArgumentNullException(nameof(collectionSelector));
            if(ResultSelector is null) throw new ArgumentNullException(nameof(ResultSelector));

            var result = new SimpleObservableEx<TResult>();
            var i = 0;
            source.ForeachAction(t => collectionSelector(t, i++).Foreach(ResultSelector, result, t, (r, selector, rr, tt) => rr.OnNext(selector(tt, r))));
            source.OnComplited(result.OnCompleted);
            (source as IObservableEx<IEnumerable<TSource>>)?.OnReset(result.OnReset);
            source.OnError(result.OnError);
            return result;
        }

        [NotNull]
        public static IObservableEx<TResult> SelectMany<TSource, TCollection, TResult>(
            [NotNull] this IObservable<TSource> source,
            [NotNull] Func<TSource, IEnumerable<TCollection>> collectionSelector,
            [NotNull] Func<TSource, TCollection, TResult> ResultSelector)
        {
            if(source is null) throw new ArgumentNullException(nameof(source));
            if(collectionSelector is null) throw new ArgumentNullException(nameof(collectionSelector));
            if(ResultSelector is null) throw new ArgumentNullException(nameof(ResultSelector));

            var result = new SimpleObservableEx<TResult>();
            source.ForeachAction(t => collectionSelector(t).Foreach(ResultSelector, result, t, (r, selector, rr, tt) => rr.OnNext(selector(tt, r))));
            source.OnComplited(result.OnCompleted);
            (source as IObservableEx<IEnumerable<TSource>>)?.OnReset(result.OnReset);
            source.OnError(result.OnError);
            return result;
        }

        /// <summary>Получить первое значение наблюдаемого объекта</summary>
        /// <typeparam name="T">Тип значений объекта</typeparam>
        /// <param name="ObservableObject">Наблюдаемый объект</param>
        /// <returns>Первое значение наблюдаемого объекта</returns>
        public static T Single<T>([NotNull] this IObservable<T> ObservableObject)
        {
            var w = new AutoResetEvent(false);
            var value = default(T);
            ObservableObject.Subscribe(t => { value = t; w.Set(); });
            w.WaitOne();
            return value;
        }

        [NotNull]
        public static IObservableEx<T[]> Buffer<T>([NotNull] this IObservable<T> ObservableObject, int BufferLength,
            int BufferPeriod = 0, int BufferPhase = 0) => new CountedBufferedObservable<T>(ObservableObject, BufferLength, BufferPeriod, BufferPhase);
    }

    internal abstract class BufferedObservable<T> : SimpleObservableEx<T[]>
    {
        private readonly int _BufferLength;
        private readonly IObserver<T> _Observer;
        private readonly Queue<Queue<T>> _Buffer;
        protected readonly object _SyncRoot = new object();

        protected BufferedObservable([NotNull] IObservable<T> ObservableObject, int QueueLength, int BufferLength = 0)
        {
            _BufferLength = BufferLength;
            _Observer = new LamdaObserver<T>(ObservableObject, OnNext, OnCompleted, OnReset, OnError);
            _Buffer = new Queue<Queue<T>>(QueueLength);
        }

        protected abstract void OnNext(T value);

        [CanBeNull]
        protected T[] AddValue(T Value)
        {
            lock (_SyncRoot)
            {
                foreach(var b in _Buffer) b.Enqueue(Value);
                var f = _Buffer.Peek();
                if(f.Count < _BufferLength) return null;
                _Buffer.Dequeue();
                return f.ToArray();
            }
        }

        protected void AddBuffer() { lock (_SyncRoot) _Buffer.Enqueue(new Queue<T>(_BufferLength)); }
    }

    internal class CountedBufferedObservable<T> : BufferedObservable<T>
    {
        private volatile int _Phase;
        private readonly int _BufferPeriod;
        private readonly int _BufferPhase;

        public CountedBufferedObservable([NotNull] IObservable<T> ObservableObject, int BufferLength,
            int BufferPeriod = 0, int BufferPhase = 0) : base(ObservableObject, BufferPeriod / BufferLength + 1, BufferLength)
        {
            _BufferPeriod = BufferPeriod;
            _BufferPhase = BufferPhase;
        }
        protected override void OnNext(T value)
        {
            T[] r;
            lock (_SyncRoot)
            {
                if(_Phase++ % _BufferPeriod == _BufferPhase) AddBuffer();
                r = AddValue(value);
            }
            if(r != null) OnNext(r);
        }
    }
}