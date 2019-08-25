using System;
using System.ComponentModel;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global

namespace MathCore
{
    public interface IFactory<out T>
    {
        /// <summary>Создать новый объект</summary>
        /// <returns>Новый объект типа <typeparamref name="T"/></returns>
        [DST] T Create();
    }

    /// <summary>Генератор объектов типа <typeparamref name="T"/></summary>
    /// <typeparam name="T">Тип генерируемых объектов</typeparam>
    public class Factory<T> : INotifyPropertyChanged, IFactory<T>
    {
        /* ------------------------------------------------------------------------------------------ */

        private event PropertyChangedEventHandler _PropertyChanged;

        /// <summary>Событие возникает при генерации новой строки</summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            [DST] add => _PropertyChanged += value;
            [DST] remove => _PropertyChanged -= value;
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => _PropertyChanged?.Invoke(this, e);

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Метод генерации объектов</summary>
        private Func<T> _FactoryMethod;

        private T _Last;
        private readonly PropertyChangedEventArgs _PropertyLastChengedArgs = new PropertyChangedEventArgs(nameof(Last));

        protected bool _RaiseLastChangedEvents = true;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Последний сгенерированный объект</summary>
        public T Last
        {
            [DST]
            get => _Last;
            private set
            {
                _Last = value;
                if(_RaiseLastChangedEvents)
                    OnPropertyChanged(_PropertyLastChengedArgs);
            }
        }

        /// <summary>Метод генерации объектов типа <typeparamref name="T"/></summary>
        public Func<T> FactoryMethod
        {
            [DST]
            get => _FactoryMethod;
            [DST]
            set => _FactoryMethod = value;
        }

        /* ------------------------------------------------------------------------------------------ */

        protected Factory() { }

        /// <summary>Новый генератор объектов типа <typeparamref name="T"/></summary>
        /// <param name="CreateMethod">Метод генерации объектов типа <typeparamref name="T"/></param>
        [DST]
        public Factory(Func<T> CreateMethod) => _FactoryMethod = CreateMethod;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Создать новый объект</summary>
        /// <returns>Новый объект типа <typeparamref name="T"/></returns>
        [DST]
        public virtual T Create() => _FactoryMethod == null ? default : Last = _FactoryMethod();

        /* ------------------------------------------------------------------------------------------ */

        [DST]
        public override int GetHashCode() => typeof(T).GetHashCode() ^ _FactoryMethod.GetHashCode();

        /* ------------------------------------------------------------------------------------------ */
    }
}