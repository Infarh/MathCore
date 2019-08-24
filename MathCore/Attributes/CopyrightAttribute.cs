// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
namespace System
{
    /// <summary>Сведения об автораских правах на участок кода</summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class CopyrightAttribute : Attribute
    {
        /// <summary>Авторские права</summary>
        public string Copyright { set; get; }

        /// <summary>Ссылка на источник</summary>
        public string url { get; set; }

        /// <summary>Инициализация нового экземпляра <see cref="CopyrightAttribute"/></summary>
        /// <param name="Copyright">Авторские права</param>
        public CopyrightAttribute(string Copyright) => this.Copyright = Copyright;

        /// <inheritdoc />
        public override string ToString() => $"Copyright: {Copyright}";
    }
}
