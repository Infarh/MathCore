#nullable enable
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using MathCore.Annotations;

using static System.Linq.Expressions.Expression;
// ReSharper disable UnusedType.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable ConvertToAutoProperty
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore;

/// <summary>Инициализатор объекта по структуре xml-файла</summary>
/// <typeparam name="TObject">Тип инициализируемого объекта</typeparam>
public class XmlInitializer<TObject> : ICollection<XmlInitializer<TObject>.Rule>, IIndexableRead<int, XmlInitializer<TObject>.Rule>
{
    #region Правила инициализации

    /// <summary>Правило инициализации</summary>
    public abstract class Rule
    {
        /// <summary>XPath-выражение - путь в структуре xml</summary>
        private readonly string _XPath;

        /// <summary>Выражение, применяемое при обнаружении значения указанного XPath-пути</summary>
        private readonly Expression<Action<TObject, string>> _PropertyBody;

        /// <summary>Скомпилированный метод инициализации</summary>
        public readonly Action<TObject, string> _Update;

        /// <summary>Выражение, применяемое при обнаружении значения указанного XPath-пути</summary>
        public Expression<Action<TObject, string>> UpdateExpression => _PropertyBody;

        /// <summary>XPath-выражение - путь в структуре xml</summary>
        public string XPath => _XPath;

        /// <summary>Правилу не нужен объект</summary>
        public bool IsObjectLess { get; }

        /// <summary>Скомпилированный метод инициализации</summary>
        public Action<TObject, string> Update => _Update;

        /// <summary>Инициализация нового правила</summary>
        /// <param name="XPath">XPath-путь в структуре xml</param>
        /// <param name="Expression">Выражение инициализации</param>
        /// <param name="IsObjectLess">Правилу не нужен объект</param>
        protected Rule(string XPath, Expression<Action<TObject, string>> Expression, bool IsObjectLess = false)
        {
            this.IsObjectLess = IsObjectLess;
            _XPath            = XPath;
            _PropertyBody     = Expression;
            _Update           = Expression.Compile();
        }

        /// <summary>Выполнить инициализацию</summary>
        /// <param name="obj">Инициализируемый объект</param>
        /// <param name="e">Корень структуры xml</param>
        /// <param name="ns">Описание пространств имён</param>
        public void Execute(TObject obj, XElement e, IXmlNamespaceResolver ns) => e
           .GetXPathValues(_XPath, ns)                              //Пытаемся найти все значения по указанному пути
           .Foreach(_Update, obj, (s, update, o) => update!(o!, s!)); //Каждое найденное значение передаём в метод инициализации

        /// <summary>Выполнить инициализацию</summary>
        /// <param name="e">Корень структуры xml</param>
        /// <param name="ns">Описание пространств имён</param>
        public void Execute(XElement e, IXmlNamespaceResolver ns) => e
           .GetXPathValues(_XPath, ns)                           //Пытаемся найти все значения по указанному пути
           .Foreach(_Update, (s, update) => update!(default!, s!)); //Каждое найденное значение передаём в метод инициализации
    }

    /// <summary>Типизированное правило инициализации</summary>
    /// <typeparam name="TValue">Тип значения инициализируемого параметра объекта</typeparam>
    public sealed class Rule<TValue> : Rule
    {
        /// <summary>Получить выражение инициализации для выражения определения свойства</summary>
        /// <param name="expression">Выражение определения инициализируемого параметра объекта</param>
        /// <param name="ValueConverterExpression">Выражение преобразования строки в тип объекта</param>
        /// <returns>Выражение инициализации, как процедура с параметром - объектом и строкой xml-узла</returns>
        private static Expression<Action<TObject, string>> GetExpression
        (
            Expression<Func<TObject, TValue>> expression,
            Expression<Func<string, TValue>>? ValueConverterExpression
        )
        {
            //Параметр выражения, содержащий объект
            var p_obj = expression.Parameters[0];
            //Параметр выражения конвертера, содержащий строку xml-структуры
            var p_str = ValueConverterExpression?.Parameters[0] ?? Parameter(typeof(string), "s");
            //Тело выражения конвертера из строки в нужный тип данных
            var converter_body = ValueConverterExpression?.Body;
            //Если конвертера не указано (тело отсутствует)
            if(converter_body is null)
            {
                //Проверяем - можно ли преобразовать строку напрямую к нужному типу параметра
                if(typeof(TValue).IsAssignableFrom(typeof(string)))
                    //Если да, то создаём выражение прямого присвоения
                    return Lambda<Action<TObject, string>>(Assign(expression.Body, p_str), p_obj, p_str);
                //Если прямое присвоение строки не возможно, то
                //Определяем конвертер для целевого типа
                var c = TypeDescriptor.GetConverter(typeof(TValue));
                //Если преобразователь не может осуществить требуемое преобразование типов,
                if(!c.CanConvertFrom(typeof(string))) //то генерируем исключение
                    throw new NotSupportedException(
                        $"Невозможно автоматически преобразовать тип {typeof(string)} в {typeof(TValue)}");
                //Если конвертер может преобразовать строку в указанный тип данных
                //Создаём тело конвертера
                converter_body = Convert //Узел выражения преобразования типа object в целевой тип TValue
                (                        //Узел вызова метода преобразования типа у объекта преобразователя
                    Call
                    (
                        Constant(c),                                        //Узел выражения, содержащий значение объекта преобразователя
                        ((Func<string, object?>)c.ConvertFromString).Method, //Описание метода
                        p_str                                               //Параметр метода - строковый
                    ),
                    typeof(TValue)
                );
            }
            //Если выражение содержит не указание конкретного члена класса (поля, или свойства), то генерируем исключение
            if(expression.Body is not MemberExpression) throw new NotSupportedException("Выражение не поддерживается");
            return Lambda<Action<TObject, string>> //Создаём лямбда-выражение
            (                                      //содержащее
                Assign                             //выражение присвоения
                (
                    expression.Body, //телу выражения определения члена объекта
                    converter_body   //тело метода-конвертера
                ),
                p_obj, //Первый параметр - объект
                p_str  //Второй параметр - строка xml-структуры
            );
        }

        /// <summary>Получить выражение инициализации объекта по выражению инициализации и выражению конвертера</summary>
        /// <param name="expression">Выражение инициализации</param>
        /// <param name="ValueConverterExpression">Конвертер преобразования строки в нужный тип данных</param>
        /// <returns>Выражение инициализации объекта по строке xml-структуры</returns>
        private static Expression<Action<TObject, string>> GetExpression
        (
            Expression<Action<TObject, TValue>> expression,
            Expression<Func<string, TValue>>? ValueConverterExpression
        )
        {
            //Параметр выражения инициализации, содержащий объект инициализации
            var p_obj = expression.Parameters[0];
            //Параметр выражения инициализации, содержащий целевое значение параметра целевого типа
            var p_arg = expression.Parameters[1];
            //Тело выражения инициализации
            var expr_body = expression.Body;
            //Параметр выражения конвертера, содержащий строку
            var p_str = ValueConverterExpression?.Parameters[0] ?? Parameter(typeof(string), "s");
            //Тело выражения конвертера
            var converter_body = ValueConverterExpression?.Body;
            //Если конвертер не указан - тело конвертера отсутствует
            if(converter_body is null)
            {
                //Проверяем - возможно ли прямое присвоение строкового параметру целевого типа
                if(typeof(TValue).IsAssignableFrom(typeof(string)))
                    //Если присвоение возможно, то заменяем в теле выражения инициализации 
                    //целевой параметр на строковый параметр и возвращаем лямбда-выражение
                    return Lambda<Action<TObject, string>>(expr_body.Replace(p_arg, p_str), p_obj, p_str);
                //Если прямое присвоение не возможно, то пытаемся выполнить преобразование с помощью конвертера
                var c = TypeDescriptor.GetConverter(typeof(TValue));
                //Если преобразование типов невозможно, то
                if(!c.CanConvertFrom(typeof(string))) //генерируем исключение
                    throw new NotSupportedException(
                        $"Невозможно автоматически преобразовать тип {typeof(string)} в {typeof(TValue)}");
                //Если преобразование возможно, то создаём тело выражения-конвертера
                converter_body = Convert //Создаём узел приведения к целевому типу
                (
                    Call //Создаём узел вызова метода
                    (
                        Constant(c),                                        //Значение объекта-преобразования типов
                        ((Func<string, object?>)c.ConvertFromString).Method, //Описание метода
                        p_str                                               //Строковый параметр
                    ),
                    typeof(TValue) //Целевой тип
                );
            }
            //создаём лямбда-выражение инициализации объекта
            return Lambda<Action<TObject, string>>
            ( //За основу берём тело исходного выражения инициализации
                expr_body.Replace(p_arg, converter_body),
                //Заменяем в дереве параметр целевого объекта на тело конвертера
                p_obj, //Параметр - инициализируемый объект
                p_str  //Параметр - строка xml-структуры
            );
        }

        private static Expression<Action<TObject, string>> GetExpression(
            Expression<Action<TValue>> InitializationExpression, 
            Expression<Func<string, TValue>>? ValueConverterExpression)
        {
            var p_obj     = Parameter(typeof(TObject), "o");         //Параметр выражения инициализации, содержащий объект инициализации
            var p_arg     = InitializationExpression.Parameters[0];  //Параметр выражения инициализации, содержащий параметр инициализации
            var expr_body = InitializationExpression.Body;             //Тело выражения инициализации
            var p_str     = ValueConverterExpression?.Parameters[0] ?? Parameter(typeof(string), "s"); //Параметр выражения конвертера, содержащий строку
                
            var converter_body = ValueConverterExpression?.Body; //Тело выражения конвертера
            if(converter_body is null)                           //Если конвертер не указан - тело конвертера отсутствует
            {
                if(typeof(TValue).IsAssignableFrom(typeof(string))) //Проверяем - возможно ли прямое присвоение строкового параметру целевого типа
                    //Если присвоение возможно, то заменяем в теле выражения инициализации
                    //целевой параметр на строковый параметр и возвращаем лямбда-выражение
                    return Lambda<Action<TObject, string>>(expr_body.Replace(p_arg, p_str), p_obj, p_str);

                var converter = TypeDescriptor.GetConverter(typeof(TValue)); //Если прямое присвоение не возможно, то пытаемся выполнить преобразование с помощью конвертера
                    
                if(!converter.CanConvertFrom(typeof(string))) //Если преобразование типов невозможно, то генерируем исключение
                    throw new NotSupportedException($"Невозможно автоматически преобразовать тип {typeof(string)} в {typeof(TValue)}");

                //Если преобразование возможно, то создаём тело выражения-конвертера
                converter_body = Convert //Создаём узел приведения к целевому типу
                (
                    Call //Создаём узел вызова метода
                    (
                        Constant(converter),                                        //Значение объекта-преобразования типов
                        ((Func<string, object?>)converter.ConvertFromString).Method, //Описание метода
                        p_str                                                       //Строковый параметр
                    ),
                    typeof(TValue) //Целевой тип
                );
            }
            //создаём лямбда-выражение инициализации объекта
            return Lambda<Action<TObject, string>>
            ( //За основу берём тело исходного выражения инициализации
                expr_body.Replace(p_arg, converter_body),
                //Заменяем в дереве параметр целевого объекта на тело конвертера
                p_obj, //Параметр - инициализируемый объект
                p_str  //Параметр - строка xml-структуры
            );
        }

        /// <summary>Инициализация нового экземпляра <see cref="Rule{TValue}"/></summary>
        /// <param name="XPath">Путь к значению внутри xml-файла</param>
        /// <param name="expression">Выражение преобразования значения объекта инициализации к типу <typeparamref name="TValue"/>, выполняемое для инициализации значения объекта</param>
        /// <param name="converter">Выражение преобразования строкового типа значения из xml-файла в <typeparamref name="TValue"/></param>
        public Rule
        (
            string XPath,
            Expression<Func<TObject, TValue>> expression,
            Expression<Func<string, TValue>>? converter = null
        ) : base(XPath, GetExpression(expression, converter))
        { }

        /// <summary>Инициализация нового экземпляра <see cref="Rule{TValue}"/></summary>
        /// <param name="XPath">Путь к значению внутри xml-файла</param>
        /// <param name="expression">Выражение действия над объектом инициализации и типом <typeparamref name="TValue"/>, выполняемое для присвоения значения</param>
        /// <param name="converter">Выражение преобразования строкового типа значения из xml-файла в <typeparamref name="TValue"/></param>
        public Rule
        (
            string XPath,
            Expression<Action<TObject, TValue>> expression,
            Expression<Func<string, TValue>>? converter = null
        ) : base(XPath, GetExpression(expression, converter))
        { }

        /// <summary>Инициализация нового экземпляра <see cref="Rule{TValue}"/></summary>
        /// <param name="XPath">Путь к значению внутри xml-файла</param>
        /// <param name="expression">Выражение действия над объектом инициализации и типом <typeparamref name="TValue"/>, выполняемое для присвоения значения</param>
        public Rule
        (
            string XPath,
            Expression<Action<TObject, string>> expression
        ) : base(XPath, expression)
        { }

        /// <summary>Инициализация нового экземпляра <see cref="Rule{TValue}"/></summary>
        /// <param name="XPath">Путь к значению внутри xml-файла</param>
        /// <param name="expression">Выражение действия над объектом инициализации, выполняемое для присвоения значения</param>
        /// <param name="converter">Выражение преобразования строкового типа значения из xml-файла в <typeparamref name="TValue"/></param>
        public Rule
        (
            string XPath,
            Expression<Action<TValue>> expression,
            Expression<Func<string, TValue>>? converter = null
        ) : base(XPath, GetExpression(expression, converter), true)
        { }
    }

    #endregion

    /// <summary>Набор правил</summary>
    private readonly List<Rule> _Rules = new();

    /// <summary>Число правил инициализации</summary>
    public int Count => _Rules.Count;

    /// <summary>Правило инициализации с указанным индексом</summary>
    /// <param name="i">Индекс правила инициализации в списке</param>
    /// <returns>Правило инициализации с указанным индексом</returns>
    public Rule this[int i] => _Rules[i];

    /// <summary>Менеджер пространств имён xml-файла</summary>
    public XmlNamespaceManager Namespace { get; set; } = new(new NameTable());

    /// <summary>Инициализация нового экземпляра <see cref="XmlInitializer{TObject}"/></summary>
    public XmlInitializer() { }

    /// <summary>Инициализация нового экземпляра <see cref="XmlInitializer{TObject}"/></summary>
    /// <param name="Rules">Список правил инициализации</param>
    public XmlInitializer(params Rule[] Rules) : this() => _Rules.AddRange(Rules);

    #region Методы обработки XML

    #region Linq to xml

    /// <summary>Инициализировать объект</summary>
    /// <param name="obj">Инициализируемый объект</param>
    /// <param name="xml">Xml-документ - источник данных процесса инициализации</param>
    public void Initialize([DisallowNull] TObject obj, XDocument? xml) => Initialize(obj, xml?.Root, Namespace);

    /// <summary>Инициализировать объект</summary>
    /// <param name="obj">Инициализируемый объект</param>
    /// <param name="xml">Узел Xml-документа - источник данных процесса инициализации</param>
    public void Initialize([DisallowNull] TObject obj, XElement xml) => Initialize(obj, xml, Namespace);

    /// <summary>Инициализировать объект</summary>
    /// <param name="xml">Xml-документ - источник данных процесса инициализации</param>
    /// <param name="XmlNamespace">Пространство имён данных процесса инициализации</param>
    public void Initialize(
        XDocument? xml,
        IXmlNamespaceResolver XmlNamespace)
        => Initialize(xml?.Root, XmlNamespace);

    /// <summary>Инициализировать объект</summary>
    /// <param name="obj">Инициализируемый объект</param>
    /// <param name="xml">Xml-документ - источник данных процесса инициализации</param>
    /// <param name="XmlNamespace">Пространство имён данных процесса инициализации</param>
    public void Initialize(
        [DisallowNull] TObject obj, 
        XDocument? xml, 
        IXmlNamespaceResolver XmlNamespace)
        => Initialize(obj, xml?.Root, XmlNamespace);

    /// <summary>Инициализировать объект</summary>
    /// <param name="obj">Инициализируемый объект</param>
    /// <param name="xml">Узел Xml-документа - источник данных процесса инициализации</param>
    /// <param name="XmlNamespace">Пространство имён данных процесса инициализации</param>
    public void Initialize(
        [DisallowNull] TObject obj, 
        XElement? xml, 
        IXmlNamespaceResolver XmlNamespace)
    {
        if(xml is null) return;
        foreach (var rule in _Rules)
            rule.Execute(obj, xml, XmlNamespace);
    }

    /// <summary>Инициализировать объект</summary>
    /// <param name="xml">Узел Xml-документа - источник данных процесса инициализации</param>
    /// <param name="XmlNamespace">Пространство имён данных процесса инициализации</param>
    public void Initialize(
        XElement? xml,
        IXmlNamespaceResolver XmlNamespace)
    {
        if(xml is null) return;
        foreach (var rule in _Rules.Where(rule => rule.IsObjectLess))
            rule.Execute(xml, XmlNamespace);
    }

    #endregion

    /// <summary>Инициализировать объект данными из xml-документа</summary>
    /// <param name="obj">инициализируемый объект</param>
    /// <param name="xml">Объект чтения xml-документа</param>
    // <include file='http://informer.gismeteo.ru/rss/27612.xml' path='//rss/channel/title/*' />
    public void Initialize([DisallowNull] TObject obj, XmlReader xml) => Initialize(obj, xml, Namespace);

    /// <summary>Инициализировать объект данными из xml-документа</summary>
    /// <param name="obj">инициализируемый объект</param>
    /// <param name="xml">Объект чтения xml-документа</param>
    /// <param name="XmlNamespace">Пространство имён данных процесса инициализации</param>
    public void Initialize([DisallowNull] TObject obj, XmlReader xml, XmlNamespaceManager? XmlNamespace)
    {
        var path = XmlNamespace is null ? new XPathCollection() : new XPathCollection(XmlNamespace);
        _Rules.Select(rule => new XPathQuery(rule.XPath) { Tag = rule })
           .ForeachLazy(path.Add)
           .Foreach(obj, (q, o) => q.QueryMatch += (_, e) => ((Rule)q.Tag).Update(o!, e.Argument));
        var reader = new XPathReader(xml, path);
        while(reader.ReadUntilMatch()) { }
    }

    #endregion

    /// <summary>Добавить правило с указанием выражения получения члена объекта</summary>
    /// <typeparam name="TPropertyValue">Тип значения свойства</typeparam>
    /// <param name="XPath">Путь в xml-структуре</param>
    /// <param name="InitializationExpression">Выражение, определяющее член объекта</param>
    /// <param name="DataConverterExpression">Выражение преобразования строки в целевой тип данных</param>
    public void Add<TPropertyValue>
    (
        string XPath,
        Expression<Func<TObject, TPropertyValue>> InitializationExpression,
        Expression<Func<string, TPropertyValue>>? DataConverterExpression = null
    ) => _Rules.Add(new Rule<TPropertyValue>(XPath, InitializationExpression, DataConverterExpression));

    /// <summary>Добавить правило инициализации объекта <typeparamref name="TObject"/></summary>
    /// <param name="XPath">Путь к данным внутри xml-файла</param>
    /// <param name="InitializationExpression">Выражение, определяющее как записать данные в объект после извлечения их из xml-файла</param>
    /// <param name="DataConverterExpression">Выражение, определяющее как требуется преобразовать строку, полученную из xml-файла в тип данных, необходимый объекту</param>
    /// <typeparam name="TArgument">Тип устанавливаемого значения для объекта инициализации</typeparam>
    public void Add<TArgument>
    (
        string XPath,
        Expression<Action<TObject, TArgument>> InitializationExpression,
        Expression<Func<string, TArgument>>? DataConverterExpression = null
    ) => _Rules.Add(new Rule<TArgument>(XPath, InitializationExpression, DataConverterExpression));

    /// <summary>Добавить правило инициализации</summary>
    /// <param name="XPath">Путь к данным внутри xml-файла</param>
    /// <param name="InitializationExpression">Выражение, определяющее как записать строку в объект после извлечения её из xml-файла</param>
    /// <param name="DataConverterExpression">Выражение, определяющее как требуется преобразовать строку, полученную из xml-файла в строку, необходимую объекту</param>
    public void Add
    (
        string XPath,
        Expression<Action<TObject, string>> InitializationExpression,
        Expression<Func<string, string>>? DataConverterExpression = null
    ) => _Rules.Add
    (
        DataConverterExpression is null
            ? new Rule<string>(XPath, InitializationExpression)
            : new Rule<string>(XPath, InitializationExpression, DataConverterExpression)
    );

    /// <summary>Добавить правило инициализации без инициализируемого объекта</summary>
    /// <param name="XPath">Путь к данным внутри xml-файла</param>
    /// <param name="InitializationExpression">Выражение, определяющее что требуется сделать с данными после извлечения их из xml-файла</param>
    /// <param name="DataConverterExpression">Выражение, определяющее как требуется преобразовать строку, полученную из xml-файла в тип данных, необходимый объекту</param>
    /// <typeparam name="TArgument">Тип устанавливаемого значения для объекта инициализации</typeparam>
    public void Add<TArgument>
    (
        string XPath,
        Expression<Action<TArgument>> InitializationExpression,
        Expression<Func<string, TArgument>>? DataConverterExpression = null
    ) => _Rules.Add(new Rule<TArgument>(XPath, InitializationExpression, DataConverterExpression));

    /// <summary>Добавить правило инициализации без инициализируемого объекта</summary>
    /// <param name="XPath">Путь к данным внутри xml-файла</param>
    /// <param name="InitializationExpression">Выражение, определяющее что требуется сделать со строкой после её извлечения из xml-файла</param>
    /// <param name="DataConverterExpression">Выражение, определяющее как требуется преобразовать строку, полученную из xml-файла в строку, необходимую для процесса инициализации</param>
    public void Add
    (
        string XPath,
        Expression<Action<string>> InitializationExpression,
        Expression<Func<string, string>>? DataConverterExpression = null
    ) => _Rules.Add(new Rule<string>(XPath, InitializationExpression, DataConverterExpression));

    /// <summary>Удалить все правила инициализации</summary>
    public void Clear() => _Rules.Clear();

    /// <inheritdoc />
    bool ICollection<Rule>.IsReadOnly => false;

    /// <inheritdoc />
    void ICollection<Rule>.Add(Rule? item) => _Rules.Add(item);

    /// <inheritdoc />
    bool ICollection<Rule>.Contains(Rule? item) => _Rules.Contains(item);

    /// <inheritdoc />
    void ICollection<Rule>.CopyTo(Rule[] array, int ArrayIndex) => _Rules.CopyTo(array, ArrayIndex);

    /// <inheritdoc />
    bool ICollection<Rule>.Remove(Rule? item) => _Rules.Remove(item);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Rules).GetEnumerator();

    /// <inheritdoc />
    IEnumerator<Rule> IEnumerable<Rule>.GetEnumerator() => _Rules.GetEnumerator();
}