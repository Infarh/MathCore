//------------------------------------------------------------------------------
// <copyright file="XPathReader.cs" company="Microsoft">
//     
//      Copyright (c) 2002 Microsoft Corporation.  All rights reserved.
//     
//      The use and distribution terms for this software are contained in the file
//      named license.txt, which can be found in the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by the
//      terms of this license.
//     
//      You must not remove this notice, or any other, from this software.
//     
// </copyright>
//------------------------------------------------------------------------------

using System.Collections;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

/// <summary>Предоставляет возможность чтения XML-документов с использованием XPath-запросов для фильтрации узлов</summary>
/// <remarks>
/// XPathReader расширяет функциональность XmlReader, добавляя поддержку XPath-запросов.
/// Это позволяет эффективно читать только те узлы, которые соответствуют заданным XPath-выражениям.
/// </remarks>
/// <example>
/// <code>
/// var xpath_collection = new XPathCollection { "//book[@category='fiction']" };
/// using var reader = new XPathReader("books.xml", xpath_collection);
/// while (reader.ReadUntilMatch())
/// {
///     if (reader.Match(0))
///         Console.WriteLine(reader.ReadOuterXml());
/// }
/// </code>
/// </example>
public class XPathReader : XmlReader
{
    #region Fields

    private readonly XPathCollection _XPathCollection;

    #endregion

    #region Properties

    /// <summary>Получает тип текущего узла</summary>
    public override XmlNodeType NodeType => BaseReader.NodeType;

    /// <summary>Получает имя текущего узла, включая префикс пространства имён</summary>
    public override string Name => BaseReader.Name;

    /// <summary>Получает имя текущего узла без префикса пространства имён</summary>
    public override string LocalName => BaseReader.LocalName;

    /// <summary>Получает URI пространства имён (как определено в спецификации W3C Namespace) текущей области пространства имён</summary>
    public override string NamespaceURI => BaseReader.NamespaceURI;

    /// <summary>Получает префикс пространства имён, связанный с текущим узлом</summary>
    public override string Prefix => BaseReader.Prefix;

    /// <summary>Получает значение, указывающее, имеет ли <see cref='XPathReader.Value' /> возвращаемое значение</summary>
    public override bool HasValue => BaseReader.HasValue;

    /// <summary>Получает текстовое значение текущего узла</summary>
    public override string Value => BaseReader.Value;

    /// <summary>Получает глубину вложенности текущего узла в стеке XML-элементов</summary>
    public override int Depth => BaseReader.Depth;

    /// <summary>Получает базовый URI текущего узла</summary>
    public override string BaseURI => BaseReader.BaseURI;

    /// <summary>Получает значение, указывающее, является ли текущий узел пустым элементом (например, &lt;MyElement/&gt;)</summary>
    public override bool IsEmptyElement => BaseReader.IsEmptyElement;

    /// <summary>Получает значение, указывающее, является ли текущий узел атрибутом, который был создан из значения по умолчанию, определённого в DTD или схеме</summary>
    public override bool IsDefault => BaseReader.IsDefault;

    /// <summary>Получает символ кавычек, используемый для заключения значения атрибута узла</summary>
    public override char QuoteChar => BaseReader.QuoteChar;

    /// <summary>Получает текущую область xml:space</summary>
    public override XmlSpace XmlSpace => BaseReader.XmlSpace;

    /// <summary>Получает текущую область xml:lang</summary>
    public override string XmlLang => BaseReader.XmlLang;

    /// <summary>Получает количество атрибутов текущего узла</summary>
    public override int AttributeCount => BaseReader.AttributeCount;

    /// <summary>Получает значение атрибута с указанным индексом</summary>
    /// <param name="i">Индекс атрибута</param>
    public override string this[int i] => BaseReader[i];

    /// <summary>Получает значение атрибута с указанным <see cref='XPathReader.Name' /></summary>
    /// <param name="name">Имя атрибута</param>
    /// <exception cref="InvalidOperationException">Метод <see cref="T:System.Xml.XmlReader" /> был вызван до завершения предыдущей асинхронной операции. В этом случае выбрасывается <see cref="T:System.InvalidOperationException" /> с сообщением "An asynchronous operation is already in progress."</exception>
    public override string? this[string name] => BaseReader[name];

    /// <summary>Получает значение атрибута с указанным <see cref='XPathReader.LocalName' /> и <see cref='XPathReader.NamespaceURI' /></summary>
    /// <param name="name">Локальное имя атрибута</param>
    /// <param name="NamespaceUri">URI пространства имён</param>
    public override string? this[string name, string? NamespaceUri] => BaseReader[name, NamespaceUri];

    /// <summary>Получает значение, указывающее, может ли читатель разрешать сущности</summary>
    public override bool CanResolveEntity => BaseReader.CanResolveEntity;

    /// <summary>Получает значение, указывающее, расположен ли XmlReader в конце потока</summary>
    public override bool EOF => BaseReader.EOF;

    /// <summary>Получает состояние чтения потока</summary>
    public override ReadState ReadState => BaseReader.ReadState;

    /// <summary>Получает XmlNameTable, связанную с данной реализацией</summary>
    public override XmlNameTable NameTable => BaseReader.NameTable;

    /// <summary>Получает базовый XmlReader, используемый для чтения XML-документа</summary>
    internal XmlReader BaseReader { get; }

    /// <summary>Получает или задаёт индекс обрабатываемого атрибута</summary>
    internal int ProcessAttribute { get; set; } = -1;

    /// <summary>Получает текущий метод чтения</summary>
    internal ReadMethods ReadMethod { get; private set; } = ReadMethods.None;

    #endregion

    #region Constructors

    private XPathReader() { }

    /// <summary>Инициализирует новый экземпляр класса XPathReader с указанным XmlReader и коллекцией XPath-запросов</summary>
    /// <param name="Reader">XML-читатель для чтения документа</param>
    /// <param name="XPathCollection">Коллекция XPath-запросов для фильтрации узлов</param>
    /// <example>
    /// <code>
    /// var xml_reader = XmlReader.Create("data.xml");
    /// var xpath_collection = new XPathCollection { "//item[@type='important']" };
    /// var xpath_reader = new XPathReader(xml_reader, xpath_collection);
    /// </code>
    /// </example>
    public XPathReader(XmlReader Reader, XPathCollection XPathCollection) : this()
    {
        _XPathCollection = XPathCollection;
        XPathCollection.SetReader = this;
        BaseReader = Reader;
    }

    /// <summary>Инициализирует новый экземпляр класса XPathReader с указанным URL и XPath-запросом</summary>
    /// <param name="Url">URL XML-документа</param>
    /// <param name="XPath">XPath-запрос для фильтрации узлов</param>
    /// <example>
    /// <code>
    /// var reader = new XPathReader("https://example.com/data.xml", "//product[@price&lt;100]");
    /// </code>
    /// </example>
    public XPathReader(string Url, string XPath) : this()
    {
        BaseReader = new XmlTextReader(Url);
        _XPathCollection = [XPath];
    }

    /// <summary>Инициализирует новый экземпляр класса XPathReader с указанным TextReader и XPath-запросом</summary>
    /// <param name="Reader">Текстовый читатель для чтения XML-документа</param>
    /// <param name="XPath">XPath-запрос для фильтрации узлов</param>
    /// <example>
    /// <code>
    /// var text_reader = new StringReader("&lt;root&gt;&lt;item id='1'/&gt;&lt;/root&gt;");
    /// var reader = new XPathReader(text_reader, "//item[@id='1']");
    /// </code>
    /// </example>
    public XPathReader(TextReader Reader, string XPath) : this()
    {
        BaseReader = new XmlTextReader(Reader);
        _XPathCollection = [XPath];
    }

    /// <summary>Инициализирует новый экземпляр класса XPathReader с указанным URL и коллекцией XPath-запросов</summary>
    /// <param name="Url">URL XML-документа</param>
    /// <param name="XPathCollection">Коллекция XPath-запросов для фильтрации узлов</param>
    public XPathReader(string Url, XPathCollection XPathCollection) : this(new XmlTextReader(Url), XPathCollection) { }

    #endregion

    #region Methods

    /// <summary>Проверяет, соответствует ли текущий узел XPath-запросу по указанному индексу</summary>
    /// <param name="QueryIndex">Индекс запроса в коллекции</param>
    /// <returns>true, если текущий узел соответствует запросу; иначе false</returns>
    /// <example>
    /// <code>
    /// if (reader.Match(0))
    ///     Console.WriteLine("Узел соответствует первому запросу");
    /// </code>
    /// </example>
    public bool Match(int QueryIndex) => _XPathCollection[QueryIndex] != null && _XPathCollection[QueryIndex].Match();

    /// <summary>Проверяет, соответствует ли текущий узел указанному XPath-запросу</summary>
    /// <param name="XPathQuery">XPath-запрос для проверки</param>
    /// <returns>true, если текущий узел соответствует запросу; иначе false</returns>
    public bool Match(string _) => true;

    /// <summary>Проверяет, соответствует ли текущий узел указанному объекту XPath-запроса</summary>
    /// <param name="XPathExpr">Объект XPath-запроса</param>
    /// <returns>true, если текущий узел соответствует запросу и запрос содержится в коллекции; иначе false</returns>
    public bool Match(XPathQuery XPathExpr) => _XPathCollection.Contains(XPathExpr) && XPathExpr.Match();

    /// <summary>Проверяет, соответствует ли текущий узел хотя бы одному из запросов в списке</summary>
    /// <param name="QueryList">Список запросов для проверки</param>
    /// <returns>true, если текущий узел соответствует хотя бы одному запросу; иначе false</returns>
    public bool MatchesAny(ArrayList QueryList) => _XPathCollection.MatchesAny(QueryList, BaseReader.Depth);

    /// <summary>Читает документ до тех пор, пока не будет найден узел, соответствующий одному из XPath-запросов</summary>
    /// <returns>true, если найден соответствующий узел; false, если достигнут конец документа</returns>
    /// <example>
    /// <code>
    /// while (reader.ReadUntilMatch())
    /// {
    ///     Console.WriteLine($"Найден узел: {reader.Name}");
    /// }
    /// </code>
    /// </example>
    public bool ReadUntilMatch()
    {
        while (true)
            if (ProcessAttribute > 0)
            {
                // необходимо обработать атрибуты по одному

                if (MoveToNextAttribute())
                {
                    if (_XPathCollection.MatchAnyQuery())
                        return true;
                }
                else
                    ProcessAttribute = -1; // остановить обработку атрибутов
            }
            else if (!BaseReader.Read())
                return false;
            else
            {
                _XPathCollection.AdvanceUntil(this);
                if (_XPathCollection.MatchAnyQuery()) return true;
            }
    }

    /// <summary>Читает следующий узел из потока</summary>
    /// <returns>true, если следующий узел был успешно прочитан; false, если узлов больше нет</returns>
    public override bool Read()
    {
        ReadMethod = ReadMethods.Read;

        var ret = BaseReader.Read();
        if (ret) _XPathCollection.Advance(this);
        return ret;
    }

    /// <summary>Перемещается к атрибуту с указанным именем</summary>
    /// <param name="AttributeName">Имя атрибута</param>
    /// <returns>true, если атрибут найден; иначе false</returns>
    public override bool MoveToAttribute(string AttributeName)
    {
        ReadMethod = ReadMethods.MoveToAttribute;

        var ret = BaseReader.MoveToAttribute(AttributeName);
        if (ret) _XPathCollection.Advance(this);
        return ret;
    }

    /// <summary>Перемещается к атрибуту с указанным локальным именем и URI пространства имён</summary>
    /// <param name="AttributeName">Локальное имя атрибута</param>
    /// <param name="ns">URI пространства имён</param>
    /// <returns>true, если атрибут найден; иначе false</returns>
    public override bool MoveToAttribute(string AttributeName, string? ns)
    {
        var result = BaseReader.MoveToAttribute(AttributeName, ns);
        if (result) _XPathCollection.Advance(this);
        return result;
    }

    /// <summary>Перемещается к атрибуту с указанным индексом</summary>
    /// <param name="i">Индекс атрибута</param>
    public override void MoveToAttribute(int i)
    {
        ReadMethod = ReadMethods.MoveToAttribute;
        BaseReader.MoveToAttribute(i);
        _XPathCollection.Advance(this);
    }

    /// <summary>Перемещается к первому атрибуту</summary>
    /// <returns>true, если есть атрибуты; иначе false</returns>
    public override bool MoveToFirstAttribute()
    {
        var result = BaseReader.MoveToFirstAttribute();
        if (result) _XPathCollection.Advance(this);
        return result;
    }

    /// <summary>Перемещается к следующему атрибуту</summary>
    /// <returns>true, если есть следующий атрибут; иначе false</returns>
    public override bool MoveToNextAttribute()
    {
        var result = BaseReader.MoveToNextAttribute();
        if (result) _XPathCollection.Advance(this);
        return result;
    }

    /// <summary>Перемещается к элементу, содержащему текущий узел атрибута</summary>
    /// <returns>true, если читатель находится на атрибуте и успешно переместился к элементу; иначе false</returns>
    public override bool MoveToElement()
    {
        ReadMethod = ReadMethods.MoveToElement;
        var result = BaseReader.MoveToElement();
        if (result) _XPathCollection.Advance(this);
        return result;
    }

    /// <summary>Получает значение атрибута с указанным именем</summary>
    /// <param name="AttributeName">Имя атрибута</param>
    /// <returns>Значение указанного атрибута</returns>
    public override string? GetAttribute(string AttributeName) => BaseReader.GetAttribute(AttributeName);

    /// <summary>Получает значение атрибута с указанным локальным именем и URI пространства имён</summary>
    /// <param name="AttributeName">Локальное имя атрибута</param>
    /// <param name="NamespaceUri">URI пространства имён</param>
    /// <returns>Значение указанного атрибута</returns>
    public override string? GetAttribute(string AttributeName, string? NamespaceUri)
        => BaseReader.GetAttribute(AttributeName, NamespaceUri);

    /// <summary>Получает значение атрибута с указанным индексом</summary>
    /// <param name="i">Индекс атрибута</param>
    /// <returns>Значение указанного атрибута</returns>
    public override string GetAttribute(int i) => BaseReader.GetAttribute(i);

    /// <summary>Закрывает поток, изменяет <see cref='XPathReader.ReadState' /> на Closed и устанавливает все свойства в нулевые значения</summary>
    public override void Close() => BaseReader.Close();

    /// <summary>Читает содержимое элемента в виде строки</summary>
    /// <returns>Содержимое элемента в виде строки</returns>
    public override string ReadString() => BaseReader.ReadString();

    /// <summary>Разрешает префикс пространства имён в области текущего элемента</summary>
    /// <param name="Prefix">Префикс пространства имён для разрешения</param>
    /// <returns>URI пространства имён, на которое указывает префикс</returns>
    public override string? LookupNamespace(string Prefix) => BaseReader.LookupNamespace(Prefix);

    /// <summary>Разрешает ссылку на сущность для узлов типа EntityReference</summary>
    public override void ResolveEntity() => BaseReader.ResolveEntity();

    /// <summary>Разбирает значение атрибута на один или несколько узлов типа Text и/или EntityReference</summary>
    /// <returns>true, если есть узлы для чтения</returns>
    public override bool ReadAttributeValue() => BaseReader.ReadAttributeValue();

    /// <summary>Читает всё содержимое (включая разметку) в виде строки</summary>
    /// <returns>Всё содержимое XML в виде строки</returns>
    public override string ReadInnerXml() => BaseReader.ReadInnerXml();

    /// <summary>Читает текущий узел и всё его содержимое (включая разметку) в виде строки</summary>
    /// <returns>Текущий узел и всё его содержимое в виде строки</returns>
    public override string ReadOuterXml() => BaseReader.ReadOuterXml();

    /// <summary>Сопоставляет префикс с пространством имён</summary>
    /// <param name="prefix">Префикс для сопоставления</param>
    /// <returns>true, если префикс соответствует текущему пространству имён; иначе false</returns>
    internal bool MapPrefixWithNamespace(string prefix) =>
        _XPathCollection.NamespaceManager?.LookupNamespace(prefix) == NamespaceURI;

    #endregion
}