using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MathCore.Annotations;

namespace System.Linq.Reactive
{
    /// <summary>Класс объектов-связей между наблюдателем и списком наблюдателей, позволяющих удалять наблюдатель из писка наблюдателей в случае если объект удаляется из памяти</summary>
    /// <typeparam name="T">Тип значений наблюдаемого объекта</typeparam>
    internal sealed class ObserverLink<T> : IDisposable
    {
        /// <summary>Получить хэш-код связи</summary>
        /// <param name="Observers">Коллекция наблюдателей</param>
        /// <param name="Observer">Добавляемый наблюдатель</param>
        /// <returns>Хэш-код связи</returns>
        private static int GetHash([NotNull] ICollection<IObserver<T>> Observers, [NotNull] IObserver<T> Observer) { unchecked { return Observer.GetHashCode() * 397 ^ Observers.GetHashCode(); } }

        /// <summary>Словарь связей</summary>
        [NotNull]
        private static readonly Dictionary<int, ObserverLink<T>> __Links = new Dictionary<int, ObserverLink<T>>();

        /// <summary>Получить связь между наблюдателем и списком наблюдателей</summary>
        /// <param name="Observers">Коллекция наблюдателей</param>
        /// <param name="Observer">Добавляемый наблюдатель</param>
        /// <returns>Связь между наблюдателем и списком наблюдателей</returns>
        [NotNull]
        public static ObserverLink<T> GetLink([NotNull] ICollection<IObserver<T>> Observers, [NotNull] IObserver<T> Observer)
        {
            var hash = GetHash(Observers, Observer);
            lock(__Links)
                return __Links.TryGetValue(hash, out ObserverLink<T> link)
                    ? link
                    : (__Links[hash] = new ObserverLink<T>(Observers, Observer));
        }

        /// <summary>Удаляемый наблюдатель</summary>
        [NotNull]
        private IObserver<T> _Observer;
        /// <summary>Коллекция наблюдателей, из которой требуется удалить отслеживаемый наблюдатель</summary>
        [NotNull]
        private ICollection<IObserver<T>> _Observers;
        /// <summary>Объект межпотоковой синхроницации</summary>
        [NotNull]
        private readonly object _SyncRoot = new object();

        /// <summary>Инициализация новой связи между списком наблюдателей и отслеживаемым наблюдателем</summary>
        /// <param name="Observers">Список наблюдателей</param>
        /// <param name="Observer">Отслеживаемый наблюдатель</param>
        private ObserverLink([NotNull] ICollection<IObserver<T>> Observers, [NotNull]IObserver<T> Observer)
        {
            Contract.Requires(Observer != null);
            Contract.Requires(Observers != null);
            Contract.Ensures(_Observer != null);
            Contract.Ensures(_Observers != null);
            Contract.Ensures(_Observers.Count > 0);
            Contract.Ensures(Contract.Exists(_Observers, o => Equals(o, _Observer)));
            _Observers = Observers;
            _Observer = Observer;
            if(!_Observers.Contains(_Observer))
                _Observers.Add(_Observer);
        }

        void IDisposable.Dispose()
        {
            Contract.Ensures(_Observer is null);
            Contract.Ensures(_Observers is null);
            Contract.Ensures(!Contract.Exists(_Observers, o => Equals(o, _Observer)));
            if(_Observer is null) return;
            lock(_SyncRoot)
            {
                if(_Observer is null) return;
                lock(__Links)
                {
                    __Links.Remove(GetHash(_Observers, _Observer));
                    _Observers.Remove(_Observer);
                }
                _Observer = null;
                _Observers = null;
            }
        }


    }
}