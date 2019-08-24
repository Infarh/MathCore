// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
namespace System
{
    /// <summary>Значение должно быть в диапазоне значений</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class ValueBetweenAttribute : Attribute
    {
        /// <summary>Инжнее пороговое значение</summary>
        private object _Min;

        /// <summary>Инжнее пороговое значение</summary>
        private object _Max;

        /// <summary>Инжнее пороговое значение</summary>
        public object Min
        {
            get => _Min;
            set
            {
                if(!(value is IComparable)) throw new ArgumentException("Значение Min должно поддерживать интерфейс IComparable", nameof(value));
                _Min = value;
            }
        }

        /// <summary>Инжнее пороговое значение</summary>
        public object Max
        {
            get => _Max;
            set
            {
                if(!(value is IComparable)) throw new ArgumentException("Значение Max должно поддерживать интерфейс IComparable", nameof(value));
                _Max = value;
            }
        }

        /// <summary>Инициализация нового экземпляра <see cref="GreaterOrEqualAttribute"/></summary>
        public ValueBetweenAttribute() { }

        /// <summary>Инициализация нового экземпляра <see cref="GreaterOrEqualAttribute"/></summary>
        /// <param name="Min">Минимальное значение</param>
        /// <param name="Max">Максимальное значение</param>
        public ValueBetweenAttribute(object Min, object Max)
        {
            if(!(Min is IComparable)) throw new ArgumentException("Значение Min должно поддерживать интерфейс IComparable", nameof(Min));
            if(!(Max is IComparable)) throw new ArgumentException("Значение Max должно поддерживать интерфейс IComparable", nameof(Max));
            this.Min = Min;
            this.Max = Max;
        }

        /// <inheritdoc />
        public override string ToString() => $"value in {{{Min} ... {Max}}}";
    }
}