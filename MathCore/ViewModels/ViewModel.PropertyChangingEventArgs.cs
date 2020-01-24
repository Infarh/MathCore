using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertToAutoProperty

namespace MathCore.ViewModels
{
    public partial class ViewModel
    {
        public class PropertyChangingEventArgs<T> : PropertyChangingEventArgs
        {
            private readonly T _OldValue;
            private T _NewValue;

            public T OldValue => _OldValue;

            public T NewValue { get => _NewValue; set => _NewValue = value; }

            public PropertyChangingEventArgs(in T OldValue, in T NewValue, in string PropertyName) : base(PropertyName)
            {
                _OldValue = OldValue;
                _NewValue = NewValue;
            }
        }
    }
}
