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
        public class SetValueResult<T>
        {
            private readonly bool _Result;
            [CanBeNull] private readonly T _OldValue;
            [CanBeNull] private readonly T _NewValue;
            [NotNull] private readonly ViewModel _Model;

            internal SetValueResult(bool Result, [CanBeNull] T OldValue, [NotNull] ViewModel model) : this(Result, OldValue, OldValue, model) { }
            internal SetValueResult(bool Result, [CanBeNull] T OldValue, [CanBeNull] T NewValue, [NotNull] ViewModel model)
            {
                _Result = Result;
                _OldValue = OldValue;
                _NewValue = NewValue;
                _Model = model;
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
            public SetValueResult<T> Update([NotNull] string PropertyName)
            {
                _Model.OnPropertyChanged(PropertyName);
                return this;
            }

            [NotNull]
            public SetValueResult<T> Update([NotNull] params string[] PropertyName)
            {
                foreach (var name in PropertyName) _Model.OnPropertyChanged(name);
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
        [NotifyPropertyChangedInvocator]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        protected virtual SetValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return new SetValueResult<T>(false, field, field, this);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetValueResult<T>(true, old_value, value, this);
        }

        [NotifyPropertyChangedInvocator]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        protected virtual SetValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] Func<T, bool> value_checker, [NotNull] [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value) || !value_checker(value)) return new SetValueResult<T>(false, field, value, this);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetValueResult<T>(true, old_value, value, this);
        }
    }
}
