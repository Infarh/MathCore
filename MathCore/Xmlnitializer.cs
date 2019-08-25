using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using MathCore.Annotations;
using static System.Linq.Expressions.Expression;

namespace MathCore
{
    /// <summary>Инициализатор объекта по структуре xml-файла</summary>
    /// <typeparam name="TObject">Тип инициализируемого объекта</typeparam>
    public class XmInitializer<TObject> : ICollection<XmInitializer<TObject>.Rule>, IIndexableRead<int, XmInitializer<TObject>.Rule>
    {
        #region Rules region

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

            public bool IsObjectLess { get; private set; }

            /// <summary>Скомпилированный метод инициализации</summary>
            public Action<TObject, string> Update => _Update;

            /// <summary>Инициализация нового правила</summary>
            /// <param name="XPath">XPath-путь в структуре xml</param>
            /// <param name="Expression">Выражение инициализации</param>
            protected Rule([NotNull] string XPath, [NotNull] Expression<Action<TObject, string>> Expression, bool IsObjectLess = false)
            {
                this.IsObjectLess = IsObjectLess;
                _XPath = XPath;
                _PropertyBody = Expression;
                _Update = Expression.Compile();
            }


            /// <summary>Выполнить инициализацию</summary>
            /// <param name="obj">Инициализируемый объект</param>
            /// <param name="e">Корень структуры xml</param>
            /// <param name="ns">Описание пространств имён</param>
            public void Execute(TObject obj, XElement e, [NotNull] IXmlNamespaceResolver ns) => e
                .GetXPathValues(_XPath, ns) //Пытаемся найти все значения по указанному пути
                .Foreach(s => _Update(obj, s)); //Каждое найденное значение передаём в метод инициализации

            /// <summary>Выполнить инициализацию</summary>
            /// <param name="e">Корень структуры xml</param>
            /// <param name="ns">Описание пространств имён</param>
            public void Execute(XElement e, [NotNull] IXmlNamespaceResolver ns) => e
                .GetXPathValues(_XPath, ns) //Пытаемся найти все значения по указанному пути
                .Foreach(s => _Update(default, s)); //Каждое найденное значение передаём в метод инициализации
        }

        /// <summary>Типизированное правило инициализации</summary>
        /// <typeparam name="TValue">Тип значения инициализируемого параметра объекта</typeparam>
        public sealed class Rule<TValue> : Rule
        {
            /// <summary>Получить выражение инициализации для выражения определения свойства</summary>
            /// <param name="expression">Выражение определения инициализируемого параметра объекта</param>
            /// <param name="converter">Выражение преобразования строки в тип объекта</param>
            /// <returns>Выражение инициализации, как процедура с параметром - объектом и строкой xml-узла</returns>
            private static Expression<Action<TObject, string>> GetExpression
            (
                [NotNull] Expression<Func<TObject, TValue>> expression,
                [CanBeNull] Expression<Func<string, TValue>> converter
            )
            {
                //Параметр выражения, содержащий объект
                var pObj = expression.Parameters[0];
                //Параметр выражения конвертера, содержащий строку xml-структуры
                var pStr = converter?.Parameters[0] ?? Parameter(typeof(string), "s");
                //Тело выражения конвертера из строки в нужный тип данных
                var converter_body = converter?.Body;
                //Если конвертера не указано (тело отсутствует)
                if(converter_body == null)
                {
                    //Проверяем - можно ли преобразовать строку напрямую к нужному типу параметра
                    if(typeof(TValue).IsAssignableFrom(typeof(string)))
                        //Если да, то создаём выражение прямого присвоения
                        return Lambda<Action<TObject, string>>(Assign(expression.Body, pStr), pObj, pStr);
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
                        ( //Узел вызова метода преобразования типа у объекта преобразователя
                            Call
                                (
                                    Constant(c), //Узел выражения, содержащий значение объекта преобразователя
                                    ((Func<string, object>)c.ConvertFromString).Method, //Описание метода
                                    pStr //Параметр метода - строковый
                                ),
                            typeof(TValue)
                        );
                }
                //Если выражение содержит не указание конкретного члена класса (поля, или свйоства), то генерируем исключение
                if(!(expression.Body is MemberExpression)) throw new NotImplementedException();
                return Lambda<Action<TObject, string>> //Создаём лямда-выражение
                    ( //содержащее
                        Assign //выражение присвоения
                            (
                                expression.Body, //телу выражения определения члена объекта
                                converter_body //тело метода-конвертера
                            ),
                        pObj, //Первый параметр - объект
                        pStr //Второй параметр - строка xml-структуры
                    );
            }

            /// <summary>Получить выражение инициализации объекта по выражению инициализации и выражению конвертера</summary>
            /// <param name="expression">Выражение инициализации</param>
            /// <param name="converter">Конвертер преобразования строки в нужный тип данных</param>
            /// <returns>Выражение инициализации объекта по строке xml-структуры</returns>
            private static Expression<Action<TObject, string>> GetExpression
            (
                [NotNull] Expression<Action<TObject, TValue>> expression,
                [CanBeNull] Expression<Func<string, TValue>> converter
            )
            {
                //Параметр выражения инициализации, содержащий объект инициализации
                var pObj = expression.Parameters[0];
                //Парамметр выражения инициализации, содержащий целевое значение параметра целевого типа
                var pArg = expression.Parameters[1];
                //Тело выражения инициализации
                var expr_body = expression.Body;
                //Параметр выражения конвертера, содержащий строку
                var pStr = converter?.Parameters[0] ?? Parameter(typeof(string), "s");
                //Тело выражения конвертера
                var converter_body = converter?.Body;
                //Если конвертер не указан - тело конвертера отсутствует
                if(converter_body == null)
                {
                    //Проверяем - возможно ли прямое привоение строкового параметру целевого типа
                    if(typeof(TValue).IsAssignableFrom(typeof(string)))
                        //Если присвоение возможно, то заменяем в теле выражения инициализации 
                        //целевой параметр на строковый параметр и возвращаем лямда-выражение
                        return Lambda<Action<TObject, string>>(expr_body.Replace(pArg, pStr), pObj, pStr);
                    //Если прямое присвоение не возможно, то пытаемся выполнить преобразование с помощю конвертера
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
                                    Constant(c), //Значение объекта-преобразования типов
                                    ((Func<string, object>)c.ConvertFromString).Method, //Описание метода
                                    pStr //Строковый параметр
                                ),
                            typeof(TValue) //Целевой тип
                        );
                }
                //создаём лямда-выражение инициализации объекта
                return Lambda<Action<TObject, string>>
                    ( //За основу берём тело исходного выражения инициализации
                        expr_body.Replace(pArg, converter_body),
                        //Заменяем в дереве параметр целевого объекта на тело конвертера
                        pObj, //Параметр - инициализируемый объект
                        pStr //Параметр - строка xml-структуры
                    );
            }

            private static Expression<Action<TObject, string>> GetExpression(Expression<Action<TValue>> expression, Expression<Func<string, TValue>> converter)
            {
                //Параметр выражения инициализации, содержащий объект инициализации
                var pObj = Parameter(typeof(TObject), "o");
                //Параметр выражения инициализации, содержащий параметр инициализации
                var pArg = expression.Parameters[0];
                //Тело выражения инициализации
                var expr_body = expression.Body;
                //Параметр выражения конвертера, содержащий строку
                var pStr = converter?.Parameters[0] ?? Parameter(typeof(string), "s");
                //Тело выражения конвертера
                var converter_body = converter?.Body;
                //Если конвертер не указан - тело конвертера отсутствует
                if(converter_body == null)
                {
                    //Проверяем - возможно ли прямое привоение строкового параметру целевого типа
                    if(typeof(TValue).IsAssignableFrom(typeof(string)))
                        //Если присвоение возможно, то заменяем в теле выражения инициализации 
                        //целевой параметр на строковый параметр и возвращаем лямда-выражение
                        return Lambda<Action<TObject, string>>(expr_body.Replace(pArg, pStr), pObj, pStr);
                    //Если прямое присвоение не возможно, то пытаемся выполнить преобразование с помощю конвертера
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
                                    Constant(c), //Значение объекта-преобразования типов
                                    ((Func<string, object>)c.ConvertFromString).Method, //Описание метода
                                    pStr //Строковый параметр
                                ),
                            typeof(TValue) //Целевой тип
                        );
                }
                //создаём лямда-выражение инициализации объекта
                return Lambda<Action<TObject, string>>
                    ( //За основу берём тело исходного выражения инициализации
                        expr_body.Replace(pArg, converter_body),
                        //Заменяем в дереве параметр целевого объекта на тело конвертера
                        pObj, //Параметр - инициализируемый объект
                        pStr //Параметр - строка xml-структуры
                    );
            }

            public Rule
            (
                [NotNull] string xPath,
                [NotNull] Expression<Func<TObject, TValue>> expression,
                [CanBeNull] Expression<Func<string, TValue>> converter = null
            ) : base(xPath, GetExpression(expression, converter))
            { }

            public Rule
            (
                [NotNull] string xPath,
                [NotNull] Expression<Action<TObject, TValue>> expression,
                [CanBeNull] Expression<Func<string, TValue>> converter = null
            ) : base(xPath, GetExpression(expression, converter))
            { }

            public Rule
            (
                [NotNull] string xPath,
                [NotNull] Expression<Action<TObject, string>> expression
            ) : base(xPath, expression)
            { }

            public Rule
            (
                [NotNull] string xPath,
                [NotNull] Expression<Action<TValue>> expression,
                [CanBeNull] Expression<Func<string, TValue>> converter = null
            ) : base(xPath, GetExpression(expression, converter), true)
            { }
        }

        #endregion


        [NotNull]
        private readonly List<Rule> _Rules = new List<Rule>();

        public int Count => _Rules.Count;

        public Rule this[int i] => _Rules[i];

        [NotNull]
        public XmlNamespaceManager Namespace { get; set; } = new XmlNamespaceManager(new NameTable());

        public XmInitializer() { }
        public XmInitializer([NotNull]params Rule[] Rules) : this() => _Rules.AddRange(Rules);

        #region Методы обработки XML

        #region Linq to xml

        public void Initialize([NotNull] TObject obj, XDocument doc) => Initialize(obj, doc?.Root, Namespace);
        public void Initialize([NotNull] TObject obj, XElement e) => Initialize(obj, e, Namespace);

        public void Initialize(XDocument doc, [NotNull] IXmlNamespaceResolver ns) => Initialize(doc?.Root, ns);

        public void Initialize([NotNull] TObject obj, XDocument doc, [NotNull] IXmlNamespaceResolver ns)
            => Initialize(obj, doc?.Root, ns);

        public void Initialize([NotNull] TObject obj, XElement e, [NotNull] IXmlNamespaceResolver ns)
        {
            if(e == null) return;
            _Rules.ForEach(r => r.Execute(obj, e, ns));
        }

        public void Initialize(XElement e, [NotNull] IXmlNamespaceResolver ns)
        {
            if(e == null) return;
            _Rules.Where(r => r.IsObjectLess).Foreach(r => r.Execute(e, ns));
        }

        #endregion

        // <include file='http://informer.gismeteo.ru/rss/27612.xml' path='//rss/channel/title/*' />
        /// <devdoc>
        ///     <para>
        ///         Gets the type of the current node.
        ///     </para>
        /// </devdoc>
        public void Initialize([NotNull] TObject obj, [NotNull] XmlReader reader) => Initialize(obj, reader, Namespace);
        public void Initialize([NotNull] TObject obj, [NotNull] XmlReader Reader, [CanBeNull]XmlNamespaceManager ns)
        {
            var path = ns == null ? new XPathCollection() : new XPathCollection(ns);
            _Rules.Select(r => new XPathQuery(r.XPath) { Tag = r })
                .ForeachLazy(q => path.Add(q))
                .Foreach(q => q.QueryMatch += (s, e) => ((Rule)q.Tag).Update(obj, e.Argument));
            var reader = new XPathReader(Reader, path);
            while(reader.ReadUntilMatch()) { }
        }


        #endregion

        /// <summary>Добавить правило с указанием выражения получения члена объекта</summary>
        /// <typeparam name="TPropertyValue">Тип значения свойства</typeparam>
        /// <param name="path">Путь в xml-структуре</param>
        /// <param name="expr">Выражение, определяющее член объекта</param>
        /// <param name="conv">Выражение преобразования строки в целевой тип данных</param>
        public void Add<TPropertyValue>
        (
            [NotNull]string path,
            [NotNull]Expression<Func<TObject, TPropertyValue>> expr,
            [CanBeNull]Expression<Func<string, TPropertyValue>> conv = null
        ) => _Rules.Add(new Rule<TPropertyValue>(path, expr, conv));

        public void Add<TArgument>
        (
            [NotNull]string path,
            [NotNull]Expression<Action<TObject, TArgument>> expr,
            [CanBeNull]Expression<Func<string, TArgument>> conv = null
        ) => _Rules.Add(new Rule<TArgument>(path, expr, conv));

        public void Add
        (
            [NotNull]string path,
            [NotNull]Expression<Action<TObject, string>> expr,
            [CanBeNull]Expression<Func<string, string>> conv = null
        ) => _Rules.Add
            (
                conv == null
                    ? new Rule<string>(path, expr)
                    : new Rule<string>(path, expr, conv)
            );

        public void Add<TArgumet>
        (
            [NotNull]string path,
            [NotNull]Expression<Action<TArgumet>> expr,
            [CanBeNull]Expression<Func<string, TArgumet>> conv = null
        ) => _Rules.Add(new Rule<TArgumet>(path, expr, conv));

        public void Add
        (
            [NotNull]string path,
            [NotNull]Expression<Action<string>> expr,
            [CanBeNull]Expression<Func<string, string>> conv = null
        ) => _Rules.Add(new Rule<string>(path, expr, conv));

        public void Clear() => _Rules.Clear();

        bool ICollection<Rule>.IsReadOnly => false;

        void ICollection<Rule>.Add([CanBeNull]Rule item) => _Rules.Add(item);

        bool ICollection<Rule>.Contains([CanBeNull]Rule item) => _Rules.Contains(item);

        void ICollection<Rule>.CopyTo(Rule[] array, int arrayIndex) => _Rules.CopyTo(array, arrayIndex);

        bool ICollection<Rule>.Remove([CanBeNull]Rule item) => _Rules.Remove(item);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Rules).GetEnumerator();

        IEnumerator<Rule> IEnumerable<Rule>.GetEnumerator() => _Rules.GetEnumerator();
    }
}
