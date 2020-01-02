using System.Threading.Tasks;

namespace System.Xml.Serialization
{
    /// <summary>Асинхронно сериализуемый в XML объект</summary>
    public interface IXmlSerializableAsync : IXmlSerializable
    {
        /// <summary>Асинхронное чтение данных из XML</summary>
        /// <param name="reader">Источник данных XML</param>
        /// <returns>Задача процесса чтения данных</returns>
        Task ReadXmlAsync(XmlReader reader);

        /// <summary>Асинхронная запись данных в XML</summary>
        /// <param name="writer">Объект записи данных</param>
        /// <returns>Задача записи данных</returns>
        Task WriteXmlAsync(XmlWriter writer);
    } 
}