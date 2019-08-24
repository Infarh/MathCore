// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
namespace System
{
    /// <summary>Атрибут признака нереализованности</summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public sealed class NotImplementedAttribute : Attribute
    {
        /// <summary>Сообщение</summary>
        public string Message { get; set; }

        /// <summary>Новый атрибут нереализованности</summary>
        public NotImplementedAttribute() { }

        /// <summary>Новый атрибут нереализованности</summary>
        /// <param name="Message">Сообщение (почему не раелизовано?)</param>
        public NotImplementedAttribute(string Message) => this.Message = Message;

        /// <summary>Признак атрибута "по умолчанию"</summary>
        /// <returns>Истина, если указан незаполненный атрибут</returns>
        public override bool IsDefaultAttribute() => string.IsNullOrEmpty(Message);
    }
}
