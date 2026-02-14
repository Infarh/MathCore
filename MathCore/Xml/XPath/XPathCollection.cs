//------------------------------------------------------------------------------
// <copyright file="XPathCollection.cs" company="Microsoft">
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

// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

/// <summary>Коллекция запросов XPath для сопоставления с XML-документом</summary>
/// <remarks>
/// Коллекция предназначена для связывания строковых выражений XPath с их скомпилированными представлениями.
/// Поддерживает добавление, удаление и проверку соответствия запросов текущему узлу XML-документа.
/// Автоматически отслеживает состояние всех запросов при навигации по документу.
/// </remarks>
/// <example>
/// <code>
/// var ns_manager = new XmlNamespaceManager(new NameTable());
/// var collection = new XPathCollection(ns_manager);
/// 
/// // Добавление запросов
/// var query1 = collection.Add("/root/item[@id='1']");
/// var query2 = collection.Add("//element[text()='value']");
/// 
/// // Проверка наличия запроса
/// if (collection.Contains("/root/item[@id='1']"))
/// {
///     Console.WriteLine("Запрос найден");
/// }
/// 
/// // Удаление запроса
/// collection.Remove(query1);
/// </code>
/// </example>
public class XPathCollection() : ICollection
{
    #region Fields

    private readonly Hashtable _XPatches = [];

    private int _Key; // Количество запросов, добавленных в коллекцию в качестве ключей
    private XPathReader _Reader;

    #endregion

    #region Properties

    /// <summary>Устанавливает читатель XPath для коллекции</summary>
    internal XPathReader SetReader { set => _Reader = value; }

    /// <summary>Количество обработанных запросов</summary>
    internal int ProcessCount { get; set; } = 0;

    /// <summary>Менеджер пространств имён XML для разрешения префиксов в запросах XPath</summary>
    public XmlNamespaceManager NamespaceManager { set; get; }

    /// <summary>Получает запрос XPath по индексу</summary>
    /// <param name="index">Индекс запроса в коллекции</param>
    /// <returns>Запрос XPath</returns>
    public XPathQuery this[int index] => (XPathQuery)_XPatches[index]!;

    /// <summary>Количество запросов в коллекции</summary>
    public int Count => _XPatches.Count;

    /// <summary>Объект для синхронизации доступа к коллекции</summary>
    public object SyncRoot => this;

    /// <summary>Указывает, синхронизирован ли доступ к коллекции</summary>
    public bool IsSynchronized => false;

    /// <summary>Указывает, доступна ли коллекция только для чтения</summary>
    public bool IsReadOnly => false;

    /// <summary>Указывает, имеет ли коллекция фиксированный размер</summary>
    public bool IsFixedSize => false;

    #endregion

    #region Constructors

    /// <summary>Создаёт экземпляр коллекции запросов XPath с указанным менеджером пространств имён</summary>
    /// <param name="NsManager">Менеджер пространств имён XML</param>
    /// <example>
    /// <code>
    /// var name_table = new NameTable();
    /// var ns_manager = new XmlNamespaceManager(name_table);
    /// ns_manager.AddNamespace("ns", "http://example.com/schema");
    /// 
    /// var collection = new XPathCollection(ns_manager);
    /// collection.Add("//ns:element");
    /// </code>
    /// </example>
    public XPathCollection(XmlNamespaceManager NsManager) : this() => NamespaceManager = NsManager;

    #endregion

    #region Methods

    /// <summary>Проверяет, совпадает ли хотя бы один запрос с текущим узлом, и добавляет индексы совпавших запросов в список</summary>
    /// <param name="list">Список для заполнения индексами совпавших запросов</param>
    /// <param name="depth">Глубина текущего узла в документе</param>
    /// <returns>true, если найдено хотя бы одно совпадение; иначе false</returns>
    /// <exception cref="ArgumentNullException">Параметр <paramref name="list"/> равен <see langword="null" /></exception>
    /// <example>
    /// <code>
    /// var collection = new XPathCollection(ns_manager);
    /// collection.Add("/root/item");
    /// collection.Add("//element");
    /// 
    /// var matched_indices = new ArrayList();
    /// if (collection.MatchesAny(matched_indices, 2))
    /// {
    ///     Console.WriteLine($"Найдено совпадений: {matched_indices.Count}");
    /// }
    /// </code>
    /// </example>
    internal bool MatchesAny(ArrayList list, int depth)
    {

        if (list is null)
            throw new ArgumentNullException(nameof(list));

        list.Clear();

        var ret = false;
        foreach (var expr in this.Cast<XPathQuery>().Where(expr => expr.Match()))
        {
            list.Add(expr.Key);
            ret = true;
        }
        return ret;
    }

    /// <summary>Проверяет, содержит ли коллекция запросы к атрибутам</summary>
    /// <returns>true, если есть хотя бы один запрос к атрибутам; иначе false</returns>
    /// <remarks>Если текущий обрабатываемый запрос является запросом атрибута, навигация переместится к атрибуту вместо чтения узла</remarks>
    internal bool CurrentContainAttributeQuery() => this.Cast<XPathQuery>().Any(expr => expr.IsAttributeQuery());

    /// <summary>Продвигает все запросы к следующему узлу</summary>
    /// <param name="reader">Читатель XPath для навигации</param>
    /// <remarks>Вызывается при переходе к следующему узлу в XML-документе для обновления состояния всех запросов</remarks>
    internal void Advance(XPathReader reader)
    {
        foreach (XPathQuery expr in this)
            expr.Advance(reader);
    }

    /// <summary>Продвигает все запросы до совпадения с текущим узлом</summary>
    /// <param name="reader">Читатель XPath для навигации</param>
    /// <remarks>Если коллекция не содержит запросов к атрибутам, сбрасывает флаг обработки атрибутов в читателе</remarks>
    internal void AdvanceUntil(XPathReader reader)
    {
        foreach (XPathQuery expr in this)
            expr.AdvanceUntil(reader);

        if (!CurrentContainAttributeQuery())
            reader.ProcessAttribute = -1;
    }

    /// <summary>Проверяет, совпадает ли хотя бы один запрос с текущим узлом</summary>
    /// <returns>true, если найдено хотя бы одно совпадение; иначе false</returns>
    /// <remarks>Проверяет все запросы в коллекции и возвращает true при первом совпадении</remarks>
    internal bool MatchAnyQuery() => this.Cast<XPathQuery>().Any(expr => expr.Match());

    /// <summary>Проверяет, содержится ли указанный запрос в коллекции</summary>
    /// <param name="expr">Запрос XPath для проверки</param>
    /// <returns>true, если запрос содержится в коллекции; иначе false</returns>
    public bool Contains(XPathQuery expr) => _XPatches.ContainsValue(expr);

    /// <summary>Проверяет, содержится ли запрос с указанным выражением XPath в коллекции</summary>
    /// <param name="xpath">Строковое выражение XPath</param>
    /// <returns>true, если запрос с таким выражением содержится в коллекции; иначе false</returns>
    /// <example>
    /// <code>
    /// var collection = new XPathCollection(ns_manager);
    /// collection.Add("/root/item");
    /// 
    /// if (collection.Contains("/root/item"))
    /// {
    ///     Console.WriteLine("Запрос найден в коллекции");
    /// }
    /// </code>
    /// </example>
    public bool Contains(string xpath) => _XPatches.Cast<XPathQuery>().Any(expr => expr.ToString() == xpath);

    /// <summary>Добавляет новый запрос XPath в коллекцию</summary>
    /// <param name="expression">Строковое выражение XPath</param>
    /// <returns>Созданный запрос XPath</returns>
    /// <remarks>
    /// Если читатель уже инициализирован и находится в интерактивном состоянии, новый запрос автоматически синхронизируется с текущей позицией читателя.
    /// </remarks>
    /// <example>
    /// <code>
    /// var collection = new XPathCollection(ns_manager);
    /// var query1 = collection.Add("/root/item[@id='1']");
    /// var query2 = collection.Add("//element[text()='value']");
    /// 
    /// Console.WriteLine($"Добавлено запросов: {collection.Count}");
    /// </code>
    /// </example>
    public XPathQuery Add(string expression)
    {
        XPathQuery xpath_expr;

        if (_Reader is null)
            xpath_expr = new(expression);
        else
        {
            xpath_expr = new(expression, _Reader.Depth);
            if (_Reader.ReadState == ReadState.Interactive)
                xpath_expr.Advance(_Reader);
        }

        xpath_expr.Key = _Key;

        _XPatches.Add(_Key++, xpath_expr);

        return xpath_expr;
    }

    /// <summary>Добавляет существующий запрос XPath в коллекцию</summary>
    /// <param name="XPathExpr">Запрос XPath для добавления</param>
    /// <returns>Индекс добавленного запроса</returns>
    public int Add(XPathQuery XPathExpr)
    {
        XPathExpr.Key = _Key;
        _XPatches.Add(_Key++, XPathExpr);
        return _Key - 1;
    }

    /// <summary>Удаляет все запросы из коллекции</summary>
    /// <example>
    /// <code>
    /// var collection = new XPathCollection(ns_manager);
    /// collection.Add("/root/item");
    /// collection.Add("//element");
    /// 
    /// collection.Clear();
    /// Console.WriteLine($"Запросов в коллекции: {collection.Count}"); // 0
    /// </code>
    /// </example>
    public void Clear() => _XPatches.Clear();

    /// <summary>Удаляет указанный запрос из коллекции</summary>
    /// <param name="XPathExpr">Запрос XPath для удаления</param>
    public void Remove(XPathQuery XPathExpr) => _XPatches.Remove(XPathExpr.Key);

    /// <summary>Удаляет все запросы с указанным выражением XPath из коллекции</summary>
    /// <param name="xpath">Строковое выражение XPath</param>
    /// <example>
    /// <code>
    /// var collection = new XPathCollection(ns_manager);
    /// collection.Add("/root/item");
    /// collection.Add("/root/item"); // Дубликат
    /// 
    /// collection.Remove("/root/item"); // Удалит оба запроса
    /// </code>
    /// </example>
    public void Remove(string xpath) => _XPatches
       .Cast<XPathQuery>()
       .Where(expr => expr.ToString() == xpath)
       .Select(p => p.Key)
       .Foreach(Remove);

    /// <summary>Удаляет запрос с указанным индексом из коллекции</summary>
    /// <param name="index">Индекс запроса для удаления</param>
    public void Remove(int index) => _XPatches.Remove(index);

    #endregion

    #region Interfaces

    /// <summary>Копирует элементы коллекции в массив</summary>
    /// <param name="array">Массив назначения</param>
    /// <param name="index">Индекс начала копирования</param>
    public void CopyTo(Array array, int index) { }

    /// <summary>Возвращает перечислитель для обхода запросов в коллекции</summary>
    /// <returns>Перечислитель коллекции</returns>
    public IEnumerator GetEnumerator() => new XPathCollectionEnumerator(_XPatches);

    #endregion
}