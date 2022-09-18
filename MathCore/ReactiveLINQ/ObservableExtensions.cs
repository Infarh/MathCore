using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMethodReturnValue.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Reactive
{
    /// <summary>Методы-расширения интерфейса наблюдаемых объектов <see cref="IObservableEx{T}"/></summary>
    public static class ObservableExtensions
    {
        /// <summary>Добавить наблюдатель в список наблюдателей и получить объект-отписчик</summary>
        /// <typeparam name="T">Тип значений наблюдаемого объекта</typeparam>
        /// <param name="Observers">Коллекция наблюдателей</param>
        /// <param name="Observer">Добавляемый наблюдатель</param>
        /// <returns>Объект, удаляющий наблюдатель из списка наблюдателей в случае своей отписки</returns>
        [NotNull]
        public static IDisposable AddObserver<T>([NotNull] this ICollection<IObserver<T>> Observers, [NotNull] IObserver<T> Observer) =>
            ObserverLink<T>.GetLink(Observers, Observer);

        public static void OnCompleted<T>([CanBeNull] this IEnumerable<IObserver<T>> Observers)
        {
            if (Observers is null) return;
            foreach (var observer in Observers)
                observer.OnCompleted();
        }

        public static void OnError<T>([CanBeNull] this IEnumerable<IObserver<T>> Observers, Exception error)
        {
            if (Observers is null) return;
            foreach (var observer in Observers)
                observer.OnError(error);
        }

        public static void OnNext<T>([CanBeNull] this IEnumerable<IObserver<T>> Observers, T value)
        {
            if (Observers is null) return;
            foreach (var observer in Observers)
                observer.OnNext(value);
        }

        public static void OnReset<T>([CanBeNull] this IEnumerable<IObserverEx<T>> Observers)
        {
            if (Observers is null) return;
            foreach (var observer in Observers)
                observer.OnReset();
        }

        /// <summary>Метод получения наблюдаемого объекта для объекта, реализующего интерфейс <see cref="INotifyPropertyChanged"/>  для указанного имени свойства</summary>
        /// <typeparam name="T">Тип значения свойства</typeparam>
        /// <param name="Obj">Наблюдаемый объект</param>
        /// <param name="ProperName">Имя свойства</param>
        /// <returns>Объект-наблюдатель за свойством</returns>
        [NotNull]
        public static IObservable<T> FromProperty<T>([NotNull] this INotifyPropertyChanged Obj, [NotNull] string ProperName) =>
            new Property<T>(Obj, ProperName);

        /// <summary>Метод фильтрации событий</summary>
        /// <typeparam name="T">Тип объектов событий</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="Predicate">Метод фильтрации</param>
        /// <returns>Объект-наблюдатель с установленным методом фильтрации</returns>
        [NotNull]
        public static IObservableEx<T> Where<T>([NotNull] this IObservable<T> Observable, [NotNull] Func<T, bool> Predicate) =>
            new WhereLambdaObservableEx<T>(Observable, Predicate);

        /// <summary>Метод фильтрации событий</summary>
        /// <typeparam name="T">Тип объектов событий</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="Predicate">Метод фильтрации</param>
        /// <param name="ElseAction">Метод обработки невошедших событий</param>
        /// <returns>Объект-наблюдатель с установленным методом фильтрации</returns>
        [NotNull]
        public static IObservableEx<T> Where<T>(
            [NotNull] this IObservable<T> Observable,
            [NotNull] Func<T, bool> Predicate,
            [NotNull] Action<T> ElseAction)
        {
            bool Selector(T t)
            {
                var result = Predicate(t);
                if (!result) ElseAction(t);
                return result;
            }

            return new WhereLambdaObservableEx<T>(Observable, Selector);
        }

        /// <summary>Метод преобразования объектов событий</summary>
        /// <typeparam name="T">Тип исходных объектов событий</typeparam>
        /// <typeparam name="TResult">Тип результирующих объектов событий</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="Selector">Объект-наблюдатель с преобразованными объектами событий</param>
        /// <returns>Объект-наблюдатель с преобразованными типами объектов</returns>
        [NotNull]
        public static IObservableEx<TResult> Select<T, TResult>([NotNull] this IObservable<T> Observable, [NotNull] Func<T, TResult> Selector) =>
            new SelectLambdaObservableEx<T, TResult>(Observable, Selector);

        [NotNull]
        public static TimeIntervalObservable Interval_Seconds(double TimeInterval, bool Started = false) =>
            Interval(TimeSpan.FromSeconds(TimeInterval), Started);

        [NotNull]
        public static TimeIntervalObservable Interval_MilliSeconds(double TimeInterval, bool Started = false) =>
            Interval(TimeSpan.FromMilliseconds(TimeInterval), Started);

        [NotNull]
        public static TimeIntervalObservable Interval_Minutes(double TimeInterval, bool Started = false) =>
            Interval(TimeSpan.FromMinutes(TimeInterval), Started);

        [NotNull]
        public static TimeIntervalObservable Interval_Hours(double TimeInterval, bool Started = false) =>
            Interval(TimeSpan.FromHours(TimeInterval), Started);

        [NotNull]
        public static TimeIntervalObservable Interval(this TimeSpan TimeInterval, bool Started = false) =>
            new(TimeInterval, Started);

        /// <summary>Метод получения синхронно задержанных во времени событий</summary>
        /// <typeparam name="T">Тип объектов событий</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="Interval">Интервал времени задержки событий</param>
        /// <returns>Объект-наблюдатель, события которого синхронно задержаны во времени на указанный интервал</returns>
        [NotNull]
        public static IObservableEx<T> WhitSync<T>([NotNull] this IObservable<T> Observable, TimeSpan Interval) =>
            new LambdaObservable<T>(Observable, (o, t) => { Thread.Sleep(Interval); o.OnNext(t); });

        /// <summary>Метод получения задержанных во времени событий</summary>
        /// <typeparam name="T">Тип объектов событий</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="Interval">Интервал времени задержки событий</param>
        /// <returns>Объект-наблюдатель, события которого задержаны во времени на указанный интервал</returns>
        [NotNull]
        public static IObservableEx<T> WhitAsync<T>(this IObservable<T> Observable, TimeSpan Interval)
        {
            async Task OnNext(IObserver<T> o, T t)
            {
                await Task.Delay(Interval).ConfigureAwait(false);
                o.OnNext(t);
            }

            void NextAsync(IObserver<T> o, T t) => Task.Run(() => OnNext(o, t));
            return new LambdaObservable<T>(Observable, NextAsync);
        }

        /// <summary>Метод получения объекта-наблюдателя для указанного события</summary>
        /// <typeparam name="TEventArgs">Тип аргументов события</typeparam>
        /// <param name="Obj">Наблюдаемый объект</param>
        /// <param name="EventName">Имя события</param>
        /// <returns>Объект-наблюдатель за событием</returns>
        [NotNull]
        public static IObservableEx<TEventArgs> FromEvent<TEventArgs>([NotNull] this object Obj, [NotNull] string EventName)
            where TEventArgs : EventArgs =>
            new EventObservableEx<TEventArgs>(Obj, EventName);

        /// <summary>Метод получения объекта-наблюдателя из объекта-перечисления</summary>
        /// <typeparam name="T">Тип объектов перечисления</typeparam>
        /// <param name="Enumerable">Перечисление объектов</param>
        /// <param name="Observable">Созданный объект-наблюдатель за перечислением объектов коллекции</param>
        /// <returns>Новое перечисление объектов, перечисление объектов которого вызывает события в наблюдателе</returns>
        [NotNull]
        public static IEnumerable<T> GetObservable<T>([NotNull] this IEnumerable<T> Enumerable, [NotNull] out IObservable<T> Observable)
        {
            Observable = new SimpleObservableEx<T>();
            return Enumerable
                .ForeachLazy(((SimpleObservableEx<T>)Observable).OnNext)
                .OnComplete(((SimpleObservableEx<T>)Observable).OnCompleted);
        }

        [NotNull]
        public static IObservable<T> ToObservable<T>(this IEnumerable<T> Enumerable) => new ObservableCollectionEnumerator<T>(Enumerable);

        /// <summary>Метод получения объекта-наблюдателя, пропускающего после создания указанное число событий</summary>
        /// <typeparam name="T">Тип объектов события</typeparam>
        /// <param name="Source">Исходный объект-наблюдатель</param>
        /// <param name="Count">Количество пропускаемых событий</param>
        /// <returns>Объект-наблюдатель с указанным количеством пропускаемых событий</returns>
        [NotNull]
        public static IObservableEx<T> Take<T>([NotNull] this IObservable<T> Source, int Count) =>
            new TakeObservable<T>(Source, Count);

        /// <summary>Метод обработки последовательности событий с учётом разрешающей и запрещающей последовательностей</summary>
        /// <typeparam name="TSource">Тип объектов событий наблюдаемого объекта</typeparam>
        /// <typeparam name="TOpen">Тип объектов событий разрешающей последовательности</typeparam>
        /// <typeparam name="TClose">Тип объектов событий запрещающей последовательности</typeparam>
        /// <param name="Observable">Объект-наблюдатель источник событий</param>
        /// <param name="Open">Объект-наблюдатель разрешающий событий в выходной последовательности</param>
        /// <param name="Close">Объект-наблюдатель запрещающий событий в выходной последовательности</param>
        /// <param name="InitialState">Исходное состояния разрешения событий в выходной последовательности (по умолчанию разрешено)</param>
        /// <returns>Управляемый объект-наблюдатель</returns>
        [NotNull]
        public static IObservableEx<TSource> Take<TSource, TOpen, TClose>(
            [NotNull] this IObservable<TSource> Observable,
            [NotNull] IObservable<TOpen> Open,
            [NotNull] IObservable<TClose> Close,
            bool InitialState = true)
        {
            var t = Observable as TriggeredObservable<TSource> ?? new TriggeredObservable<TSource>(Observable, InitialState);
            Open.ForeachAction(_ => t.State = true);
            Close.ForeachAction(_ => t.State = false);
            return t;
        }

        /// <summary>Метод получения объекта-наблюдателя, события в котором пропускаются в выходную последовательность только до появления события в управляющей последовательности</summary>
        /// <typeparam name="T">Тип исходных объектов события</typeparam>
        /// <typeparam name="TResult">Тип события управляющей последовательности</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="Selector">Объект-наблюдатель управляющей последовательности</param>
        /// <param name="IsOpen">Исходное состояние выходной последовательности</param>
        /// <returns>Объект-наблюдатель управляемый управляющей последовательностью</returns>
        [NotNull]
        public static IObservableEx<T> TakeUntil<T, TResult>(
            this IObservable<T> Observable,
            [NotNull] IObservable<TResult> Selector,
            bool IsOpen = true)
        {
            var o = Observable as TriggeredObservable<T> ?? new TriggeredObservable<T>(Observable, IsOpen);
            Selector.ForeachAction(_ => o.State = false);
            return o;
        }

        /// <summary>Метод получения объекта-наблюдателя, события в котором пропускаются в выходную последовательность только после появления события в управляющей последовательности</summary>
        /// <typeparam name="T">Тип исходных объектов события</typeparam>
        /// <typeparam name="TResult">Тип события управляющей последовательности</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="Selector">Объект-наблюдатель управляющей последовательности</param>
        /// <param name="IsOpen">Исходное состояние выходной последовательности</param>
        /// <returns>Объект-наблюдатель управляемый управляющей последовательностью</returns>
        [NotNull]
        public static IObservableEx<T> SkipWhile<T, TResult>(
            this IObservable<T> Observable,
            [NotNull] IObservable<TResult> Selector,
            bool IsOpen = false)
        {
            var o = Observable as TriggeredObservable<T> ?? new TriggeredObservable<T>(Observable, IsOpen);
            Selector.ForeachAction(_ => o.State = true);
            return o;
        }

        /// <summary>Метод обработки события <see cref="IObserverEx{T}.Next"/></summary>
        /// <typeparam name="T">Тип объектов наблюдения</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="Action">Метод обработки события <see cref="IObserverEx{T}.Next"/></param>
        /// <returns>Исходный объект-наблюдатель</returns>
        [NotNull]
        public static IObservable<T> ForeachAction<T>([NotNull] this IObservable<T> Observable, [NotNull] Action<T> Action) =>
            Observable.InitializeObject(Action, (o, a) => new LambdaObserver<T>(o, a))
            ?? throw new InvalidOperationException("Возвращена пустая ссылка");

        /// <summary>Подписаться на уведомления наблюдаемого объекта <see cref="IObservable{T}"/></summary>
        /// <param name="Source">Источник событий</param>
        /// <param name="Action">Действие, выполняемое при возникновении события</param>
        /// <typeparam name="T">Тип значения событий</typeparam>
        /// <returns>Объект отписки</returns>
        [NotNull]
        public static IDisposable Subscribe<T>([NotNull] this IObservable<T> Source, [NotNull] Action<T> Action) =>
            new LambdaObserver<T>(Source, Action);

        /// <summary>Метод обработки события <see cref="IObserverEx{T}.Next"/></summary>
        /// <typeparam name="T">Тип объектов наблюдения</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="Action">Метод обработки события <see cref="IObserverEx{T}.Next"/></param>
        /// <param name="Where">Метод выборки события <see cref="IObserverEx{T}.Next"/></param>
        /// <returns>Исходный объект-наблюдатель</returns>
        [CanBeNull]
        public static IObservable<T> ForeachAction<T>(
            [NotNull] this IObservable<T> Observable,
            [NotNull] Action<T> Action,
            [NotNull] Func<T, bool> Where) =>
            Observable.InitializeObject(Where, Action, (o, w, a) => new LambdaObserver<T>(o, t => { if (w(t)) a(t); }));

        /// <summary>Метод обработки события <see cref="IObserverEx{T}.Next"/></summary>
        /// <typeparam name="T">Тип объектов наблюдения</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="Action">Метод обработки события <see cref="IObserverEx{T}.Next"/></param>
        /// <returns>Исходный объект-наблюдатель</returns>
        [CanBeNull]
        public static IObservable<T> ForeachAction<T>(this IObservable<T> Observable, Action<T, int> Action)
        {
            var i = 0;
            // ReSharper disable once HeapView.CanAvoidClosure
            return Observable.InitializeObject(o => new LambdaObserver<T>(o, t => Action(t, i++)));
        }

        /// <summary>Метод обработки события <see cref="IObserverEx{T}.Next"/></summary>
        /// <typeparam name="T">Тип объектов наблюдения</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="Action">Метод обработки события <see cref="IObserverEx{T}.Next"/></param>
        /// <param name="Where">Метод выборки события <see cref="IObserverEx{T}.Next"/></param>
        /// <returns>Исходный объект-наблюдатель</returns>
        [CanBeNull]
        public static IObservable<T> ForeachAction<T>(this IObservable<T> Observable, Action<T, int> Action, Func<T, int, bool> Where)
        {
            var i = 0;
            // ReSharper disable once HeapView.CanAvoidClosure
            return Observable.InitializeObject(o => new LambdaObserver<T>(o, t => { if (Where(t, i)) Action(t, i++); }));
        }

        /// <summary>Метод обработки события <see cref="Exception"/></summary>
        /// <typeparam name="T">Тип объектов наблюдения</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="OnError">Метод обработки события <see cref="Exception"/></param>
        /// <returns>Исходный объект-наблюдатель</returns>
        [CanBeNull]
        public static IObservable<T> OnError<T>(this IObservable<T> Observable, Action<Exception> OnError) =>
            Observable.InitializeObject(OnError, (o, e) => new LambdaObserver<T>(o, OnError: e));

        /// <summary>Метод обработки события <see cref="IObserverEx{T}.Completed"/></summary>
        /// <typeparam name="T">Тип объектов наблюдения</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="OnCompleted">Метод обработки события <see cref="IObserverEx{T}.Completed"/></param>
        /// <returns>Исходный объект-наблюдатель</returns>
        [CanBeNull]
        public static IObservable<T> OnCompleted<T>(this IObservable<T> Observable, Action OnCompleted) =>
            Observable.InitializeObject(OnCompleted, (o, c) => new LambdaObserver<T>(o, OnCompleted: c));

        /// <summary>Метод обработки события <see cref="IObserverEx{T}.Reset"/></summary>
        /// <typeparam name="T">Тип объектов наблюдения</typeparam>
        /// <param name="Observable">Исходный объект-наблюдатель</param>
        /// <param name="OnReset">Метод обработки события <see cref="IObserverEx{T}.Reset"/></param>
        /// <returns>Исходный объект-наблюдатель</returns>
        [CanBeNull]
        public static IObservable<T> OnReset<T>(this IObservable<T> Observable, Action OnReset) =>
            Observable.InitializeObject(OnReset, (o, r) => new LambdaObserver<T>(o, OnReset: r));

        /// <summary>Создать метод генерации наблюдаемого объекта из шаблона асинхронной операции</summary>
        /// <typeparam name="T">Тип результата</typeparam>
        /// <param name="BeginInvoke">Метод начала асинхронной операции</param>
        /// <param name="EndInvoke">Метод завершения асинхронной операции</param>
        /// <returns>Функция, возвращающая наблюдаемый объект, генерирующий своё значение в момент завершения асинхронной операции</returns>
        [NotNull]
        public static Func<IObservableEx<T>> FromAsyncPattern<T>(Func<AsyncCallback, object, IAsyncResult> BeginInvoke, Func<IAsyncResult, T> EndInvoke) =>
            () => new AsyncPatternObservable<T>(BeginInvoke, EndInvoke);

        /// <summary>Метод-лианиризатор событий для наблюдаемого объекта, возвращающего коллекцию объектов типа <see cref="T"/></summary>
        /// <typeparam name="T">Тип результирующих объектов</typeparam>
        /// <param name="Observable">Объект-наблюдатель коллекции</param>
        /// <returns>Объект-наблюдатель элементов коллекции</returns>
        [NotNull]
        public static IObservableEx<T> SelectMany<T>([NotNull] this IObservable<IEnumerable<T>> Observable)
        {
            var result = new SimpleObservableEx<T>();
            Observable.ForeachAction(t => t.Foreach(result.OnNext));
            Observable.OnCompleted(result.OnCompleted);
            (Observable as IObservableEx<IEnumerable<T>>)?.OnReset(result.OnReset);
            Observable.OnError(result.OnError);
            return result;
        }

        [NotNull]
        public static IObservableEx<TResult> SelectMany<TSource, TResult>(
            [NotNull] this IObservable<TSource> Observable,
            [NotNull] Func<TSource, IEnumerable<TResult>> Selector)
        {
            if (Observable is null) throw new ArgumentNullException(nameof(Observable));
            if (Selector is null) throw new ArgumentNullException(nameof(Selector));

            var result = new SimpleObservableEx<TResult>();
            Observable.ForeachAction(t => Selector(t).Foreach(result.OnNext));
            Observable.OnCompleted(result.OnCompleted);
            (Observable as IObservableEx<IEnumerable<TSource>>)?.OnReset(result.OnReset);
            Observable.OnError(result.OnError);
            return result;
        }

        [NotNull]
        public static IObservableEx<TResult> SelectMany<TSource, TResult>(
            [NotNull] this IObservable<TSource> Observable,
            [NotNull] Func<TSource, int, IEnumerable<TResult>> Selector)
        {
            if (Observable is null) throw new ArgumentNullException(nameof(Observable));
            if (Selector is null) throw new ArgumentNullException(nameof(Selector));

            var result = new SimpleObservableEx<TResult>();
            var i = 0;
            Observable.ForeachAction(t => Selector(t, i++).Foreach(result.OnNext));
            Observable.OnCompleted(result.OnCompleted);
            (Observable as IObservableEx<IEnumerable<TSource>>)?.OnReset(result.OnReset);
            Observable.OnError(result.OnError);
            return result;
        }

        [NotNull]
        public static IObservableEx<TResult> SelectMany<TSource, TCollection, TResult>(
            [NotNull] this IObservable<TSource> Observable,
            [NotNull] Func<TSource, int, IEnumerable<TCollection>> CollectionSelector,
            [NotNull] Func<TSource, TCollection, TResult> ResultSelector)
        {
            if (Observable is null) throw new ArgumentNullException(nameof(Observable));
            if (CollectionSelector is null) throw new ArgumentNullException(nameof(CollectionSelector));
            if (ResultSelector is null) throw new ArgumentNullException(nameof(ResultSelector));

            var result = new SimpleObservableEx<TResult>();
            var i = 0;
            Observable.ForeachAction(t => CollectionSelector(t, i++).Foreach(ResultSelector, result, t, (r, selector, rr, tt) => rr.OnNext(selector(tt, r))));
            Observable.OnCompleted(result.OnCompleted);
            (Observable as IObservableEx<IEnumerable<TSource>>)?.OnReset(result.OnReset);
            Observable.OnError(result.OnError);
            return result;
        }

        [NotNull]
        public static IObservableEx<TResult> SelectMany<TSource, TCollection, TResult>(
            [NotNull] this IObservable<TSource> Observable,
            [NotNull] Func<TSource, IEnumerable<TCollection>> CollectionSelector,
            [NotNull] Func<TSource, TCollection, TResult> ResultSelector)
        {
            if (Observable is null) throw new ArgumentNullException(nameof(Observable));
            if (CollectionSelector is null) throw new ArgumentNullException(nameof(CollectionSelector));
            if (ResultSelector is null) throw new ArgumentNullException(nameof(ResultSelector));

            var result = new SimpleObservableEx<TResult>();
            Observable.ForeachAction(t => CollectionSelector(t).Foreach(ResultSelector, result, t, (r, selector, rr, tt) => rr.OnNext(selector(tt, r))));
            Observable.OnCompleted(result.OnCompleted);
            (Observable as IObservableEx<IEnumerable<TSource>>)?.OnReset(result.OnReset);
            Observable.OnError(result.OnError);
            return result;
        }

        /// <summary>Получить первое значение наблюдаемого объекта</summary>
        /// <typeparam name="T">Тип значений объекта</typeparam>
        /// <param name="Observable">Наблюдаемый объект</param>
        /// <returns>Первое значение наблюдаемого объекта</returns>
        public static T Single<T>([NotNull] this IObservable<T> Observable)
        {
            var w = new AutoResetEvent(false);
            var value = default(T);
            Observable.Subscribe(t => { value = t; w.Set(); });
            w.WaitOne();
            return value;
        }

        [NotNull]
        public static IObservableEx<T[]> Buffer<T>(
            [NotNull] this IObservable<T> Observable,
            int BufferLength,
            int BufferPeriod = 0,
            int BufferPhase = 0)
            => new CountedBufferedObservable<T>(Observable, BufferLength, BufferPeriod, BufferPhase);
    }
}