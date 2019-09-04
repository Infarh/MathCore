using MathCore.Annotations;

// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
namespace System
{
    /// <summary>Описание</summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class ElementDescriptionAttribute : Attribute
    {
        /// <summary>Описание</summary>
        [NotNull]
        public string Description { set; get; }

        /// <summary>Инициализация нового экземпляра <see cref=""/></summary> 
        public ElementDescriptionAttribute() { }

        /// <summary>Инициализация нового экземпляра <see cref=""/></summary>
        /// <param name="Description">Описание</param>
        public ElementDescriptionAttribute([NotNull] string Description) => this.Description = Description;

        /// <inheritdoc />
        public override string ToString() => Description;
    }
}