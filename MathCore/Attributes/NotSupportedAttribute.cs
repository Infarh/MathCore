// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
namespace System
{
    /// <summary>Атрибут признака неподдерживаемости</summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public sealed class NotSupportedAttribute : Attribute
    {
        /// <summary>Сообщение</summary>
        public string Message { get; set; }

        /// <summary>Новый атрибут неподдерживаемости</summary>
        public NotSupportedAttribute() { }

        /// <summary>Новый атрибут неподдерживаемости</summary>
        /// <param name="Message">Сообщение (почему не поддерживается?)</param>
        public NotSupportedAttribute(string Message) => this.Message = Message;

        /// <summary>Признак атрибута "по умолчанию"</summary>
        /// <returns>Истина, если указан незаполненный атрибут</returns>
        public override bool IsDefaultAttribute() => string.IsNullOrEmpty(Message);
    }
}