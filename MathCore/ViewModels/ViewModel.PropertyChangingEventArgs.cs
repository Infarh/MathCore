using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MathCore.ViewModels
{
    public partial class ViewModel
    {
        public class PropertyChangingEventArgs<T> : PropertyChangingEventArgs
        {
            public T OldValue { get; }

            public T NewValue { get; set; }

            /// <inheritdoc />
            public PropertyChangingEventArgs(T OldValue, T NewValue, string PropertyName) : base(PropertyName)
            {
                this.OldValue = OldValue;
                this.NewValue = NewValue;
            }
        }
    }
}
