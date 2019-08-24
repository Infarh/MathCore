using System;
using System.ComponentModel;
using System.Diagnostics;
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
        private event PropertyChangedEventHandler e_PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            [DebuggerStepThrough]
            add
            {
                if(value != null) lock(e_PropertyChanged)
                        if(e_PropertyChanged == null)
                            e_PropertyChanged = value;
                        else
                            e_PropertyChanged += value;
            }
            [DebuggerStepThrough]
            remove
            {
                if(value != null) lock(e_PropertyChanged)
                        if(e_PropertyChanged != null)
                            e_PropertyChanged -= value;
            }
        }


        /// <summary>Метод получения значения свойства</summary>
        private Func<T> _GetMethod;

        /// <summary>Метод установки значения свойства</summary>
        private Action<T> _SetMethod;

        /// <summary>Метод получения значения свойства</summary>
        public Func<T> GetMethod
        {
            [DebuggerStepThrough]
            get => _GetMethod;
            [DebuggerStepThrough]
            set => _GetMethod = value;
        }

        /// <summary>Метод установки значения свойства</summary>
        public Action<T> SetMethod
        {
            [DebuggerStepThrough]
            get => _SetMethod;
            [DebuggerStepThrough]
            set => _SetMethod = value;
        }

        /// <summary>Признак возможности чтения значения свойства (если задан метод чтения)</summary>
        public bool CanRead { [DebuggerStepThrough] get => _GetMethod != null; }

        /// <summary>Признак возможности устанавливать значение свойства (если задан метод записи)</summary>
        public bool CanWrite { [DebuggerStepThrough] get => _SetMethod != null; }

        /// <summary>Значение свойства</summary>
        public T Value
        {
            [DebuggerStepThrough]
            get => _GetMethod();
            [DebuggerStepThrough]
            set
            {
                _SetMethod(value);
                e_PropertyChanged?.Invoke(this, __ValuePropertyCahnged);
            }
        }

        /// <summary>Новое лямда свойство</summary>
        /// <param name="GetMethod">Метод чтения значения</param>
        /// <param name="SetMethod">Метод записи значения</param>
        [DebuggerStepThrough]
        public LambdaProperty(Func<T> GetMethod, Action<T> SetMethod)
        {
            this.GetMethod = GetMethod;
            this.SetMethod = SetMethod;
        }

        public override string ToString()
        {
            return $"lProperty({(CanRead ? "R" : "")}.{(CanWrite ? "W" : "")}){(CanRead ? $":Value = {Value}" : "")}";
        }

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(null, obj) 
                && (ReferenceEquals(this, obj) 
                    || obj.GetType() == typeof(LambdaProperty<T>) 
                    && Equals((LambdaProperty<T>) obj));
        }

        /// <summary>Указывает, равен ли текущий объект другому объекту того же типа.</summary>
        /// <returns>true, если текущий объект равен параметру <paramref name="other"/>, в противном случае — false.</returns>
        /// <param name="other">Объект, который требуется сравнить с данным объектом.</param>
        public bool Equals(LambdaProperty<T> other)
        {
            return !ReferenceEquals(null, other) 
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
