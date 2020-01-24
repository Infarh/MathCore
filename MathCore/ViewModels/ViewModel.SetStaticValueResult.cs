using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable UnusedMember.Global

namespace MathCore.ViewModels
{
    public partial class ViewModel
    {
        public readonly struct SetStaticValueResult<T>
        {
            private readonly bool _Result;
            [CanBeNull] private readonly T _OldValue;
            [CanBeNull] private readonly T _NewValue;
            [NotNull] private readonly Action<string> _OnPropertyChanged;

            internal SetStaticValueResult(bool Result, [CanBeNull] in T OldValue, [NotNull] in Action<string> OnPropertyChanged) : this(Result, OldValue, OldValue, OnPropertyChanged) { }
            internal SetStaticValueResult(bool Result, [CanBeNull] in T OldValue, [CanBeNull] in T NewValue, [NotNull] in Action<string> OnPropertyChanged)
            {
                _Result = Result;
                _OldValue = OldValue;
                _NewValue = NewValue;
                _OnPropertyChanged = OnPropertyChanged;
            }

            public bool Then([NotNull] in Action execute)
            {
                if (_Result) execute();
                return _Result;
            }

            public bool Then([NotNull] in Action<T> execute)
            {
                if (_Result) execute(_NewValue);
                return _Result;
            }

            public bool Then([NotNull] in Action<T, T> execute)
            {
                if (_Result) execute(_OldValue, _NewValue);
                return _Result;
            }

            public SetStaticValueResult<T> Update([NotNull] in string PropertyName)
            {
                _OnPropertyChanged(PropertyName);
                return this;
            }

            public SetStaticValueResult<T> Update([NotNull, ItemCanBeNull] params string[] PropertyName)
            {
                foreach (var name in PropertyName) _OnPropertyChanged(name);
                return this;
            }

            public bool AnywayThen([NotNull] in Action execute)
            {
                execute();
                return _Result;
            }
            public bool AnywayThen([NotNull] in Action<bool> execute)
            {
                execute(_Result);
                return _Result;
            }
            public bool AnywayThen([NotNull] in Action<T> execute)
            {
                execute(_NewValue);
                return _Result;
            }
            public bool AnywayThen([NotNull] in Action<T, bool> execute)
            {
                execute(_NewValue, _Result);
                return _Result;
            }
            public bool AnywayThen([NotNull] in Action<T, T> execute)
            {
                execute(_OldValue, _NewValue);
                return _Result;
            }
            public bool AnywayThen([NotNull] in Action<T, T, bool> execute)
            {
                execute(_OldValue, _NewValue, _Result);
                return _Result;
            }

            /// <summary>Indicates whether this instance and a specified object are equal.</summary>
            /// <param name="other">The object to compare with the current instance.</param>
            public bool Equals(in SetStaticValueResult<T> other) =>
                _Result == other._Result
                && EqualityComparer<T>.Default.Equals(_OldValue, other._OldValue)
                && EqualityComparer<T>.Default.Equals(_NewValue, other._NewValue)
                && _OnPropertyChanged.Equals(other._OnPropertyChanged);

            /// <inheritdoc />
            public override bool Equals(object obj) => obj is SetStaticValueResult<T> setter && Equals(setter);

            /// <inheritdoc />
            public override int GetHashCode()
            {
                unchecked
                {
                    var hash_code = _Result.GetHashCode();
                    hash_code = (hash_code * 397) ^ EqualityComparer<T>.Default.GetHashCode(_OldValue);
                    hash_code = (hash_code * 397) ^ EqualityComparer<T>.Default.GetHashCode(_NewValue);
                    hash_code = (hash_code * 397) ^ _OnPropertyChanged.GetHashCode();
                    return hash_code;
                }
            }

            /// <summary>Оператор равенства двух значений</summary>
            public static bool operator ==(in SetStaticValueResult<T> left, in SetStaticValueResult<T> right) => left.Equals(right);

            /// <summary>Оператор неравенства двух значений</summary>
            public static bool operator !=(in SetStaticValueResult<T> left, in SetStaticValueResult<T> right) => !left.Equals(right);

        }

        public SetStaticValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] in T value, [NotNull] in Action<string> OnPropertyChanged, [NotNull, CallerMemberName] in string PropertyName = null)
        {
            if (Equals(field, value)) return new SetStaticValueResult<T>(false, field, field, OnPropertyChanged);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetStaticValueResult<T>(true, old_value, value, OnPropertyChanged);
        }

        public static SetStaticValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] in T value, in Func<T, bool> Validator, [NotNull] in Action<string> OnPropertyChanged, [NotNull, CallerMemberName] in string PropertyName = null)
        {
            if (Equals(field, value) || !Validator(value)) return new SetStaticValueResult<T>(false, field, value, OnPropertyChanged);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetStaticValueResult<T>(true, old_value, value, OnPropertyChanged);
        }
    }
}
