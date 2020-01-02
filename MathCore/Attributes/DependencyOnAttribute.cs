// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
namespace System
{
    /// <summary>Зависимость от</summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed class DependencyOnAttribute : Attribute
    {
        /// <summary>Элемент, от которого существует зависимость</summary>
        public string Name { get; set; }

        /// <summary>Инициализация нового экземпляра <see cref="DependencyOnAttribute"/></summary>
        public DependencyOnAttribute() { }

        /// <summary>Инициализация нового экземпляра <see cref="DependencyOnAttribute"/></summary>
        /// <param name="Name">Имя элемента, от которого зависит помеченный элемент</param>
        public DependencyOnAttribute(string Name) => this.Name = Name;
    }
}