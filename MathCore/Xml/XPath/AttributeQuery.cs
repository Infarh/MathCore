// ReSharper disable once CheckNamespace
namespace System.Xml.XPath;

/// <summary>Представляет запрос для выборки атрибутов узла</summary>
/// <remarks>
/// Используется для обработки XPath-запросов типа:
/// - e/@a (выбор атрибута по имени)
/// - e/attribute::node() (выбор всех атрибутов)
/// - e/attribute::text() (не возвращает результатов)
/// </remarks>
internal sealed class AttributeQuery : BaseAxisQuery
{
    #region Constructors

    /// <summary>Инициализирует новый экземпляр класса AttributeQuery</summary>
    /// <param name="QyParent">Родительский запрос</param>
    /// <param name="name">Имя атрибута</param>
    /// <param name="prefix">Префикс пространства имён</param>
    /// <param name="type">Тип узла XPath</param>
    internal AttributeQuery(Query QyParent, string name, string prefix, XPathNodeType type) : base(QyParent, name, prefix, type) { }

    #endregion

    #region Methods

    /// <summary>Проверяет, соответствует ли узел условиям запроса</summary>
    /// <param name="reader">XPath-читатель для проверки узла</param>
    /// <returns>true, если узел соответствует условиям запроса; иначе false</returns>
    /// <remarks>
    /// Существует две ситуации для сопоставления атрибутов:
    /// 1) Пользователь переместил читатель к атрибуту в контексте текущего элемента
    /// 2) Пользователь находится в контексте элемента, и поскольку это запрос атрибута, необходимо переместиться к атрибуту самостоятельно
    /// </remarks>
    internal override bool MatchNode(XPathReader reader)
    {
        var ret = true;

        if (NodeType == XPathNodeType.All) return ret;
        if (!MatchType(NodeType, reader.NodeType))
            ret = false;
        else if (Name != string.Empty && (Name != reader.Name || Prefix != reader.Prefix))
            ret = false;

        return ret;
    }

    /// <summary>Получает значение атрибута из указанного узла</summary>
    /// <param name="reader">XPath-читатель, позиционированный на элементе</param>
    /// <returns>Значение атрибута или null, если атрибут не найден</returns>
    /// <remarks>Читатель должен быть позиционирован на узле элемента. После получения значения читатель возвращается к родительскому элементу</remarks>
    internal override object GetValue(XPathReader reader)
    {
        var base_reader = reader.BaseReader;

        object? ret = null;

        if (base_reader.MoveToAttribute(Name))
            ret = reader.Value;

        // Вернуться к родительскому элементу
        base_reader.MoveToElement();
        return ret;
    }

    #endregion
}