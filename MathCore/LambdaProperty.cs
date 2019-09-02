using System;
using System.ComponentModel;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable UnusedMember.Global

namespace MathCore
{
    public abstract class LambdaPropertyBase
    {
        protected static readonly PropertyChangedEventArgs __ValuePropertyCahnged = new PropertyChangedEventArgs("Value");
    }

    /// <summary>Класс объектов-свойств, определяемых методами установки и чтения значения</summary>
    public class LambdaProperty<T> : LambdaPropertyBase, INotifyPropertyChanged, IEquatable<LambdaProperty<T>>
    {
        private event PropertyChangedEventHandler _PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            [DST]
            add
            {
                if(value != null) lock(_PropertyChanged)
                        if(_PropertyChanged is null)
                            _PropertyChanged = value;
                        else
                            _PropertyChanged += value;
            }
            [DST]
            remove
            {
                if(value != null) lock(_PropertyChanged)
                        if(_PropertyChanged != null)
                            _PropertyChanged -= value;
            }
        }


        /// <summary>Метод получения значения свойства</summary>
        private Func<T> _GetMethod;

        /// <summary>Метод установки значения свойства</summary>
        private Action<T> _SetMethod;

        /// <summary>Метод получения значения свойства</summary>
        public Func<T> GetMethod
        {
            [DST]
            get => _GetMethod;
            [DST]
            set => _GetMethod = value;
        }

        /// <summary>Метод установки значения свойства</summary>
        public Action<T> SetMethod
        {
            [DST]
            get => _SetMethod;
            [DST]
            set => _SetMethod = value;
        }

        /// <summary>Признак возможности чтения значения свойства (если задан метод чтения)</summary>
        public bool CanRead { [DST] get => _GetMethod != null; }

        /// <summary>Признак возможности устанавливать значение свойства (если задан метод записи)</summary>
        public bool CanWrite { [DST] get => _SetMethod != null; }

        /// <summary>Значение свойства</summary>
        public T Value
        {
            [DST]
            get => _GetMethod();
            [DST]
            set
            {
                _SetMethod(value);
                _PropertyChanged?.Invoke(this, __ValuePropertyCahnged);
            }
        }

        /// <summary>Новое лямда свойство</summary>
        /// <param name="GetMethod">Метод чтения значения</param>
        /// <param name="SetMethod">Метод записи значения</param>
        [DST]
        public LambdaProperty(Func<T> GetMethod, Action<T> SetMethod)
        {
            this.GetMethod = GetMethod;
            this.SetMethod = SetMethod;
        }

        public override string ToString() => $"lProperty({(CanRead ? "R" : "")}.{(CanWrite ? "W" : "")}){(CanRead ? $":Value = {Value}" : "")}";

        public override bool Equals(object obj)
        {
            return obj is { }
                && (ReferenceEquals(this, obj) 
                    || obj.GetType() == typeof(LambdaProperty<T>) 
                    && Equals((LambdaProperty<T>) obj));
        }

        /// <summary>Указывает, равен ли текущий объект другому объекту того же типа.</summary>
        /// <returns>true, если текущий объект равен параметру <paramref name="other"/>, в противном случае — false.</returns>
        /// <param name="other">Объект, который требуется сравнить с данным объектом.</param>
        public bool Equals(LambdaProperty<T> other)
        {
            return other is { }
                && (ReferenceEquals(this, other) 
                    || Equals(other._GetMethod, _GetMethod) 
                    && Equals(other._SetMethod, _SetMethod));
        }

        /// <summary>Играет роль хэш-функции для определенного типа. </summary>
        /// <returns>Хэш-код для текущего объекта <see cref="T:System.Object"/>.</returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_GetMethod?.GetHashCode() ?? 0) * 397) ^ (_SetMethod?.GetHashCode() ?? 0);
            }
        }
    }
}
