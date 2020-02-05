using System;
using System.ComponentModel;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace MathCore
{
    /// <summary>Фабрика объектов</summary>
    /// <typeparam name="T">Тип объектов, порождаемых фабрикой</typeparam>
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

        /// <summary>Генерация события уведомления об изменении значения свойства</summary>
        /// <param name="e">Аргумент события, указывающий имя изменившегося свойства</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => _PropertyChanged?.Invoke(this, e);

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Метод генерации объектов</summary>
        private Func<T> _FactoryMethod;

        private T _Last;
        private readonly PropertyChangedEventArgs _PropertyLastChangedArgs = new PropertyChangedEventArgs(nameof(Last));

        /// <summary>Генерировать события изменения свойств</summary>
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
                    OnPropertyChanged(_PropertyLastChangedArgs);
            }
        }

        /// <summary>Метод генерации объектов типа <typeparamref name="T"/></summary>
        public Func<T> FactoryMethod
        {
            [DST] get => _FactoryMethod;
            [DST] set => _FactoryMethod = value;
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Инициализация нового экземпляра <see cref="Factory{T}"/></summary>
        protected Factory() { }

        /// <summary>Новый генератор объектов типа <typeparamref name="T"/></summary>
        /// <param name="CreateMethod">Метод генерации объектов типа <typeparamref name="T"/></param>
        [DST]
        public Factory(Func<T> CreateMethod) => _FactoryMethod = CreateMethod;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Создать новый объект</summary>
        /// <returns>Новый объект типа <typeparamref name="T"/></returns>
        [DST, CanBeNull]
        public virtual T Create() => _FactoryMethod is null ? default : Last = _FactoryMethod();

        /* ------------------------------------------------------------------------------------------ */

        /// <inheritdoc />
        [DST]
        public override int GetHashCode() => typeof(T).GetHashCode() ^ _FactoryMethod.GetHashCode();

        /* ------------------------------------------------------------------------------------------ */
    }
}