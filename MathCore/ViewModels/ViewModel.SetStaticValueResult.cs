using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using MathCore.Annotations;

namespace MathCore.ViewModels
{
    public partial class ViewModel
    {
        public class SetStaticValueResult<T>
        {
            private readonly bool _Result;
            [CanBeNull] private readonly T _OldValue;
            [CanBeNull] private readonly T _NewValue;
            [NotNull] private readonly Action<string> _OnPropertyChanged;

            internal SetStaticValueResult(bool Result, [CanBeNull] T OldValue, [NotNull] Action<string> OnPropertyChanged) : this(Result, OldValue, OldValue, OnPropertyChanged) { }
            internal SetStaticValueResult(bool Result, [CanBeNull] T OldValue, [CanBeNull] T NewValue, [NotNull] Action<string> OnPropertyChanged)
            {
                _Result = Result;
                _OldValue = OldValue;
                _NewValue = NewValue;
                _OnPropertyChanged = OnPropertyChanged;
            }

            public bool Then([NotNull] Action execute)
            {
                if (_Result) execute();
                return _Result;
            }

            public bool Then([NotNull] Action<T> execute)
            {
                if (_Result) execute(_NewValue);
                return _Result;
            }

            public bool Then([NotNull] Action<T, T> execute)
            {
                if (_Result) execute(_OldValue, _NewValue);
                return _Result;
            }

            [NotNull]
            public SetStaticValueResult<T> Update([NotNull] string PropertyName)
            {
                _OnPropertyChanged(PropertyName);
                return this;
            }

            [NotNull]
            public SetStaticValueResult<T> Update([NotNull, ItemCanBeNull] params string[] PropertyName)
            {
                foreach (var name in PropertyName) _OnPropertyChanged(name);
                return this;
            }

            public bool AnywayThen([NotNull] Action execute)
            {
                execute();
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<bool> execute)
            {
                execute(_Result);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T> execute)
            {
                execute(_NewValue);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T, bool> execute)
            {
                execute(_NewValue, _Result);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T, T> execute)
            {
                execute(_OldValue, _NewValue);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T, T, bool> execute)
            {
                execute(_OldValue, _NewValue, _Result);
                return _Result;
            }
        }

        [NotNull]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public SetStaticValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] Action<string> OnPropertyChanged, [NotNull] [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return new SetStaticValueResult<T>(false, field, field, OnPropertyChanged);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetStaticValueResult<T>(true, old_value, value, OnPropertyChanged);
        }

        [NotNull]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public static SetStaticValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] T value, Func<T, bool> value_checker, [NotNull] Action<string> OnPropertyChanged, [NotNull] [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value) || !value_checker(value)) return new SetStaticValueResult<T>(false, field, value, OnPropertyChanged);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetStaticValueResult<T>(true, old_value, value, OnPropertyChanged);
        }
    }
}
