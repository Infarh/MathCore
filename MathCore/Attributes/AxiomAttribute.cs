// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
namespace System
{
    /// <summary>Помеенное данным атрибутом не требует доказательств, или проверок</summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class AxiomAttribute : Attribute
    {
        /// <summary>Дополнительный комментарий</summary>
        public string Comment { get; set; }

        /// <summary>Инициализация нового экземпляра <see cref="AxiomAttribute"/></summary>
        public AxiomAttribute() { }

        /// <summary>Инициализация нового экземпляра <see cref="AxiomAttribute"/></summary>
        /// <param name="Comment">Дополнительный комментарий</param>
        public AxiomAttribute(string Comment) => this.Comment = Comment;

        /// <inheritdoc />
        public override string ToString() => $"Comment: {Comment}";
    }
}