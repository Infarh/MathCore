// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
namespace System
{
    /// <summary>Ссылка</summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class HyperlinkAttribute : Attribute
    {
        /// <summary>Ссылка</summary>
        public string Link { get; set; }

        /// <summary>Инициализация нового экземпляра <see cref="HyperlinkAttribute"/></summary>
        public HyperlinkAttribute() {  }

        /// <summary>Инициализация нового экземпляра <see cref="HyperlinkAttribute"/></summary>
        /// <param name="Link">Текст ссылки</param>
        public HyperlinkAttribute(string Link) => this.Link = Link;

        /// <summary>Оператор неявного приведения типа <see cref="HyperlinkAttribute"/> к <see cref="Uri"/></summary>
        /// <param name="A">Атрибут ссылки, преобразуемый в <see cref="Uri"/></param>
        public static implicit operator Uri(HyperlinkAttribute A) => new(A.Link);
    }
}