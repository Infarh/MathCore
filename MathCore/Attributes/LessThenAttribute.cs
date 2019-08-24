// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
namespace System
{
    /// <summary>Значение должно быть больше, чем</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class LessThenAttribute : Attribute
    {
        private object _Value;

        /// <summary>Пороговое значение</summary>
        public object Value
        {
            get => _Value;
            set
            {
                if(!(value is IComparable)) throw new ArgumentException("Значение должно поддерживать интерфейс IComparable", nameof(value));
                _Value = value;
            }
        }

        /// <summary>Инициализация нового экземпляра <see cref="LessThenAttribute"/></summary>
        public LessThenAttribute() { }

        /// <summary>Инициализация нового экземпляра <see cref="LessThenAttribute"/></summary>
        /// <param name="Value">Максимально допустимое значение</param>
        public LessThenAttribute(object Value)
        {
            if(!(Value is IComparable)) throw new ArgumentException("Значение должно поддерживать интерфейс IComparable", nameof(Value));
            this.Value = Value;
        }

        /// <inheritdoc />
        public override string ToString() => $"value < {Value}";
    }
}