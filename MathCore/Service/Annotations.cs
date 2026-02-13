using System.Diagnostics.CodeAnalysis;
// ReSharper disable UnusedType.Global
#pragma warning disable CS9113 // Parameter is unread.

#pragma warning disable 1591
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace MathCore.Annotations;

/// <summary>Указывает, что значение помеченного элемента может иногда быть <c>null</c>, поэтому перед использованием необходима проверка на <c>null</c></summary>
/// <example><code>
/// [CanBeNull] public object Test() { return null; }
/// public void UseTest() {
///   var p = Test();
///   var s = p.ToString(); // Warning: Possible 'System.NullReferenceException'
/// }
/// </code></example>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.ReturnValue)]
public sealed class CanBeNullAttribute : Attribute;

/// <summary>Указывает, что значение помеченного элемента никогда не может быть <c>null</c></summary>
/// <example><code>
/// [NotNull] public object Foo() {
///   return null; // Warning: Possible 'null' assignment
/// }
/// </code></example>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.ReturnValue)]
public sealed class NotNullAttribute : Attribute;

#if !NET8_0_OR_GREATER
/// <summary>Указывает, что выходное значение будет отличным от null, если указанный параметр отличен от null</summary>
/// <remarks>Инициализирует атрибут с именем связанного параметра</remarks>
/// <param name="parameterName">Имя связанного параметра. Выходное значение будет отличным от null, если аргумент указанного параметра отличен от null</param>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true)]
public sealed class NotNullIfNotNullAttribute(string parameterName) : Attribute
{
    /// <summary>Возвращает имя связанного параметра</summary>
    public string ParameterName { get; } = parameterName;
}

/// <summary>Указывает, что null не допускается в качестве входного значения, даже если соответствующий тип это разрешает</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
public sealed class DisallowNullAttribute : Attribute;

/// <summary>Указывает, что когда метод возвращает <see cref="ReturnValue"/>, параметр может быть null, даже если соответствующий тип это запрещает</summary>
/// <remarks>Инициализирует атрибут с указанным условием возвращаемого значения</remarks>
/// <param name="returnValue">Условие возвращаемого значения. Если метод возвращает это значение, связанный параметр может быть null</param>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class MaybeNullWhenAttribute(bool returnValue) : Attribute
{
    /// <summary>Возвращает условие возвращаемого значения</summary>
    public bool ReturnValue { get; } = returnValue;
}

/// <summary>Указывает, что когда метод возвращает <see cref="ReturnValue"/>, параметр не будет null, даже если соответствующий тип это разрешает</summary>
/// <remarks>Инициализирует атрибут с указанным условием возвращаемого значения</remarks>
/// <param name="returnValue">Условие возвращаемого значения. Если метод возвращает это значение, связанный параметр не будет null</param>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class NotNullWhenAttribute(bool returnValue) : Attribute
{

    /// <summary>Возвращает условие возвращаемого значения</summary>
    public bool ReturnValue { get; } = returnValue;
}
#endif

/// <summary>Может применяться к символам типов, производных от IEnumerable, а также к символам классов Task и Lazy, чтобы указать, что значение элемента коллекции, свойства Task.Result или свойства Lazy.Value никогда не может быть null</summary>
/// <example><code>
/// public void Foo([ItemNotNull]List&lt;string&gt; books)
/// {
///   foreach (var book in books) {
///     if (book != null) // Warning: Expression is always true
///      Console.WriteLine(book.ToUpper());
///   }
/// }
/// </code></example>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Delegate)]
public sealed class ItemNotNullAttribute : Attribute;

/// <summary>Может применяться к символам типов, производных от IEnumerable, а также к символам классов Task и Lazy, чтобы указать, что значение элемента коллекции, свойства Task.Result или свойства Lazy.Value может быть null</summary>
/// <example><code>
/// public void Foo([ItemCanBeNull]List&lt;string&gt; books)
/// {
///   foreach (var book in books)
///   {
///     // Warning: Possible 'System.NullReferenceException'
///     Console.WriteLine(book.ToUpper());
///   }
/// }
/// </code></example>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Delegate)]
public sealed class ItemCanBeNullAttribute : Attribute;

/// <summary>Указывает, что помеченный метод строит строку по шаблону формата и (опционально) аргументам. Параметр, содержащий строку формата, должен быть указан в конструкторе. Строка формата должна быть в виде, подобном <see cref="string.Format(IFormatProvider,string,object[])"/></summary>
/// <example><code>
/// [StringFormatMethod("message")]
/// public void ShowError(string message, params object[] args) { /* do something */ }
/// public void Foo() {
///   ShowError("Failed: {0}"); // Warning: Non-existing argument in format string
/// }
/// </code></example>
/// <param name="formatParameterName">Указывает, какой параметр аннотированного метода должен рассматриваться как строка формата</param>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
public sealed class StringFormatMethodAttribute(string formatParameterName) : Attribute
{
    public string FormatParameterName { get; } = formatParameterName;
}

/// <summary>Указывает, что аргумент функции должен быть строковым литералом и совпадать с одним из параметров вызывающей функции. Например, ReSharper аннотирует параметр <see cref="System.ArgumentNullException"/></summary>
/// <example><code>
/// public void Foo(string param) {
///   if (param is null)
///     throw new ArgumentNullException("par"); // Warning: Cannot resolve symbol
/// }
/// </code></example>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class InvokerParameterNameAttribute : Attribute;

/// <summary>Указывает, что метод содержится в типе, реализующем интерфейс <see cref="System.ComponentModel.INotifyPropertyChanged"/>, и этот метод используется для уведомления об изменении значения некоторого свойства</summary>
/// <remarks>
/// Метод должен быть нестатическим и соответствовать одной из поддерживаемых сигнатур:
/// <list>
/// <item><c>NotifyChanged(string)</c></item>
/// <item><c>NotifyChanged(params string[])</c></item>
/// <item><c>NotifyChanged{T}(Expression{Func{T}})</c></item>
/// <item><c>NotifyChanged{T,U}(Expression{Func{T,U}})</c></item>
/// <item><c>SetProperty{T}(ref T, T, string)</c></item>
/// </list>
/// </remarks>
/// <example><code>
/// public class Foo : INotifyPropertyChanged {
///   public event PropertyChangedEventHandler PropertyChanged;
///   [NotifyPropertyChangedInvocator]
///   protected virtual void NotifyChanged(string propertyName) { ... }
///
///   private string _name;
///   public string Name {
///     get { return _name; }
///     set { _name = value; NotifyChanged("LastName"); /* Warning */ }
///   }
/// }
/// </code>
/// Примеры генерируемых уведомлений:
/// <list>
/// <item><c>NotifyChanged("Property")</c></item>
/// <item><c>NotifyChanged(() =&gt; Property)</c></item>
/// <item><c>NotifyChanged((VM x) =&gt; x.Property)</c></item>
/// <item><c>SetProperty(ref myField, value, "Property")</c></item>
/// </list>
/// </example>
[AttributeUsage(AttributeTargets.Method)]
public sealed class NotifyPropertyChangedInvocatorAttribute : Attribute
{
    public NotifyPropertyChangedInvocatorAttribute() { }
    public NotifyPropertyChangedInvocatorAttribute(string parameterName) => ParameterName = parameterName;

    public string ParameterName { get; } = null!;
}

// ReSharper disable CommentTypo
/// <summary>Описывает зависимость между входом и выходом метода</summary>
/// <syntax>
/// <p>Синтаксис таблицы определения функций:</p>
/// <list>
/// <item>FDT      ::= FDTRow [;FDTRow]*</item>
/// <item>FDTRow   ::= Input =&gt; Output | Output &lt;= Input</item>
/// <item>Input    ::= ParameterName: Value [, Input]*</item>
/// <item>Output   ::= [ParameterName: Value]* {halt|stop|void|nothing|Value}</item>
/// <item>Value    ::= true | false | null | notnull | canbenull</item>
/// </list>
/// Если метод имеет один входной параметр, его имя может быть опущено.<br/>
/// Использование <c>halt</c> (или <c>void</c>/<c>nothing</c>, что одно и то же)
/// для выхода метода означает, что метод не возвращается нормально.<br/>
/// Аннотация <c>canbenull</c> применима только к выходным параметрам.<br/>
/// Вы можете использовать несколько <c>[ContractAnnotation]</c> для каждой строки FDT,
/// или использовать один атрибут со строками, разделенными точкой с запятой.<br/>
/// </syntax>
/// <examples><list>
/// <item><code>
/// [ContractAnnotation("=> halt")]
/// public void TerminationMethod()
/// </code></item>
/// <item><code>
/// [ContractAnnotation("halt &lt;= condition: false")]
/// public void Assert(bool condition, string text) // обычный метод утверждения
/// </code></item>
/// <item><code>
/// [ContractAnnotation("s:null => true")]
/// public bool IsNullOrEmpty(string s) // string.IsNullOrEmpty()
/// </code></item>
/// <item><code>
/// // Метод, который возвращает null, если параметр null, и не null, если параметр не null
/// [ContractAnnotation("null => null; notnull => notnull")]
/// public object Transform(object data) 
/// </code></item>
/// <item><code>
/// [ContractAnnotation("s:null=>false; =>true,result:notnull; =>false, result:null")]
/// public bool TryParse(string s, out Person result)
/// </code></item>
/// </list></examples>
// ReSharper restore CommentTypo
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class ContractAnnotationAttribute(string contract, bool forceFullStates) : Attribute
{
    public ContractAnnotationAttribute(string contract) : this(contract, false) { }

    public string Contract { get; } = contract;
    public bool ForceFullStates { get; } = forceFullStates;
}

/// <summary>Указывает, должен ли помеченный элемент быть локализован</summary>
/// <example><code>
/// [LocalizationRequiredAttribute(true)]
/// public class Foo {
///   private string str = "my string"; // Warning: Localizable string
/// }
/// </code></example>
[AttributeUsage(AttributeTargets.All)]
public sealed class LocalizationRequiredAttribute(bool required) : Attribute
{
    public LocalizationRequiredAttribute() : this(true) { }

    public bool Required { get; } = required;
}

/// <summary>Указывает, что значение помеченного типа (или его производных) не может сравниваться с помощью операторов '==' или '!=' и вместо этого следует использовать <c>Equals()</c>. Однако использование '==' или '!=' для сравнения с <c>null</c> всегда разрешено</summary>
/// <example><code>
/// [CannotApplyEqualityOperator]
/// class NoEquality { }
/// class UsesNoEquality {
///   public void Test() {
///     var ca1 = new NoEquality();
///     var ca2 = new NoEquality();
///     if (ca1 != null) { // OK
///       bool condition = ca1 == ca2; // Warning
///     }
///   }
/// }
/// </code></example>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CannotApplyEqualityOperatorAttribute : Attribute;

/// <summary>При применении к целевому атрибуту указывает требование для любого типа, помеченного целевым атрибутом, реализовывать или наследовать определенный тип или типы</summary>
/// <example><code>
/// [BaseTypeRequired(typeof(IComponent)] // Указываем требование
/// public class ComponentAttribute : Attribute;
/// [Component] // ComponentAttribute требует реализации интерфейса IComponent
/// public class MyComponent : IComponent { }
/// </code></example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
[BaseTypeRequired(typeof(Attribute))]
public sealed class BaseTypeRequiredAttribute(Type baseType) : Attribute
{
    public Type BaseType { get; } = baseType;
}

/// <summary>Указывает, что помеченный символ используется неявно (например, через рефлексию, во внешней библиотеке), поэтому этот символ не будет помечен как неиспользуемый (как и другими инспекциями использования)</summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags) : Attribute
{
    public UsedImplicitlyAttribute()
        : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default) { }

    public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags)
        : this(useKindFlags, ImplicitUseTargetFlags.Default) { }

    public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
        : this(ImplicitUseKindFlags.Default, targetFlags) { }

    public ImplicitUseKindFlags UseKindFlags { get; } = useKindFlags;
    public ImplicitUseTargetFlags TargetFlags { get; } = targetFlags;
}

/// <summary>Должен использоваться на атрибутах и заставляет ReSharper не помечать символы, отмеченные такими атрибутами, как неиспользуемые (как и другими инспекциями использования)</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class MeansImplicitUseAttribute(
    ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags) : Attribute
{
    public MeansImplicitUseAttribute()
        : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default) { }

    public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
        : this(useKindFlags, ImplicitUseTargetFlags.Default) { }

    public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
        : this(ImplicitUseKindFlags.Default, targetFlags) { }

    [UsedImplicitly] public ImplicitUseKindFlags UseKindFlags { get; } = useKindFlags;
    [UsedImplicitly] public ImplicitUseTargetFlags TargetFlags { get; } = targetFlags;
}

[Flags]
public enum ImplicitUseKindFlags
{
    Default = Access | Assign | InstantiatedWithFixedConstructorSignature,
    /// <summary>Только сущность, помеченная атрибутом, считается используемой</summary>
    Access = 1,
    /// <summary>Указывает неявное присваивание члену</summary>
    Assign = 2,
    /// <summary>Указывает неявное создание экземпляра типа с фиксированной сигнатурой конструктора. Это означает, что любые неиспользуемые параметры конструктора не будут помечены как таковые</summary>
    InstantiatedWithFixedConstructorSignature = 4,
    /// <summary>Указывает неявное создание экземпляра типа</summary>
    InstantiatedNoFixedConstructorSignature = 8,
}

/// <summary>Указывает, что считается неявно используемым при пометке <see cref="MeansImplicitUseAttribute"/> или <see cref="UsedImplicitlyAttribute"/></summary>
[Flags]
public enum ImplicitUseTargetFlags
{
    Default = Itself,
    Itself = 1,
    /// <summary>Члены сущности, помеченной атрибутом, считаются используемыми</summary>
    Members = 2,
    /// <summary>Сущность, помеченная атрибутом, и все её члены считаются используемыми</summary>
    WithMembers = Itself | Members
}

/// <summary>Этот атрибут предназначен для пометки публично доступного API, который не должен быть удален и поэтому рассматривается как используемый</summary>
[MeansImplicitUse]
public sealed class PublicAPIAttribute : Attribute
{
    public PublicAPIAttribute() { }
    public PublicAPIAttribute(string comment) => Comment = comment;

    public string Comment { get; } = null!;
}

/// <summary>Сообщает движку анализа кода, полностью ли обрабатывается параметр при выполнении вызванного метода. Если параметр является делегатом, указывает, что делегат выполняется во время выполнения метода. Если параметр является перечислением, указывает, что оно перечисляется во время выполнения метода</summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class InstantHandleAttribute : Attribute;

/// <summary>Указывает, что параметр является путем к файлу или папке в веб-проекте. Путь может быть относительным или абсолютным, начинающимся с корня веб-сайта (~)</summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class PathReferenceAttribute : Attribute
{
    public PathReferenceAttribute() { }
    public PathReferenceAttribute([PathReference] string basePath) => BasePath = basePath;

    public string BasePath { get; } = null!;
}

// Атрибуты ASP.NET MVC

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
[method: SuppressMessage("Стиль", "IDE0060:Удалите неиспользуемый параметр", Justification = "<Ожидание>")]
public sealed class AspMvcAreaMasterLocationFormatAttribute(string format) : Attribute;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
[method: SuppressMessage("Стиль", "IDE0060:Удалите неиспользуемый параметр", Justification = "<Ожидание>")]
public sealed class AspMvcAreaPartialViewLocationFormatAttribute(string format) : Attribute;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
[method: SuppressMessage("Стиль", "IDE0060:Удалите неиспользуемый параметр", Justification = "<Ожидание>")]
public sealed class AspMvcAreaViewLocationFormatAttribute(string format) : Attribute;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
[method: SuppressMessage("Стиль", "IDE0060:Удалите неиспользуемый параметр", Justification = "<Ожидание>")]
public sealed class AspMvcMasterLocationFormatAttribute(string format) : Attribute;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
[method: SuppressMessage("Стиль", "IDE0060:Удалите неиспользуемый параметр", Justification = "<Ожидание>")]
public sealed class AspMvcPartialViewLocationFormatAttribute(string format) : Attribute;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
[method: SuppressMessage("Стиль", "IDE0060:Удалите неиспользуемый параметр", Justification = "<Ожидание>")]
public sealed class AspMvcViewLocationFormatAttribute(string format) : Attribute;

/// <summary>Атрибут ASP.NET MVC. Если применен к параметру, указывает, что параметр является действием MVC. Если применен к методу, имя действия MVC вычисляется неявно из контекста. Используйте этот атрибут для пользовательских оберток, подобных <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String)</c></summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
public sealed class AspMvcActionAttribute : Attribute
{
    public AspMvcActionAttribute() { }
    public AspMvcActionAttribute(string anonymousProperty) => AnonymousProperty = anonymousProperty;

    public string AnonymousProperty { get; } = null!;
}

/// <summary>Атрибут ASP.NET MVC. Указывает, что параметр является областью MVC. Используйте этот атрибут для пользовательских оберток, подобных <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String)</c></summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class AspMvcAreaAttribute : PathReferenceAttribute
{
    public AspMvcAreaAttribute() { }
    public AspMvcAreaAttribute(string anonymousProperty) => AnonymousProperty = anonymousProperty;

    public string AnonymousProperty { get; } = null!;
}

/// <summary>Атрибут ASP.NET MVC. Если применен к параметру, указывает, что параметр является контроллером MVC. Если применен к методу, имя контроллера MVC вычисляется неявно из контекста. Используйте этот атрибут для пользовательских оберток, подобных <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String, String)</c></summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
public sealed class AspMvcControllerAttribute : Attribute
{
    public AspMvcControllerAttribute() { }
    public AspMvcControllerAttribute(string anonymousProperty) => AnonymousProperty = anonymousProperty;

    public string AnonymousProperty { get; } = null!;
}

/// <summary>Атрибут ASP.NET MVC. Указывает, что параметр является главной страницей MVC. Используйте этот атрибут для пользовательских оберток, подобных <c>System.Web.Mvc.Controller.View(String, String)</c></summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class AspMvcMasterAttribute : Attribute;

/// <summary>Атрибут ASP.NET MVC. Указывает, что параметр является типом модели MVC. Используйте этот атрибут для пользовательских оберток, подобных <c>System.Web.Mvc.Controller.View(String, Object)</c></summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class AspMvcModelTypeAttribute : Attribute;

/// <summary>Атрибут ASP.NET MVC. Если применен к параметру, указывает, что параметр является частичным представлением MVC. Если применен к методу, имя частичного представления MVC вычисляется неявно из контекста. Используйте этот атрибут для пользовательских оберток, подобных <c>System.Web.Mvc.Html.RenderPartialExtensions.RenderPartial(HtmlHelper, String)</c></summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
public sealed class AspMvcPartialViewAttribute : PathReferenceAttribute;

/// <summary>Атрибут ASP.NET MVC. Позволяет отключить все инспекции для представлений MVC в пределах класса или метода</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AspMvcPressuresViewErrorAttribute : Attribute;

/// <summary>Атрибут ASP.NET MVC. Указывает, что параметр является шаблоном отображения MVC. Используйте этот атрибут для пользовательских оберток, подобных <c>System.Web.Mvc.Html.DisplayExtensions.DisplayForModel(HtmlHelper, String)</c></summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class AspMvcDisplayTemplateAttribute : Attribute;

/// <summary>Атрибут ASP.NET MVC. Указывает, что параметр является шаблоном редактора MVC. Используйте этот атрибут для пользовательских оберток, подобных <c>System.Web.Mvc.Html.EditorExtensions.EditorForModel(HtmlHelper, String)</c></summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class AspMvcEditorTemplateAttribute : Attribute;

/// <summary>Атрибут ASP.NET MVC. Указывает, что параметр является шаблоном MVC. Используйте этот атрибут для пользовательских оберток, подобных <c>System.ComponentModel.DataAnnotations.UIHintAttribute(System.String)</c></summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class AspMvcTemplateAttribute : Attribute;

/// <summary>Атрибут ASP.NET MVC. Если применен к параметру, указывает, что параметр является представлением MVC. Если применен к методу, имя представления MVC вычисляется неявно из контекста. Используйте этот атрибут для пользовательских оберток, подобных <c>System.Web.Mvc.Controller.View(Object)</c></summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
public sealed class AspMvcViewAttribute : PathReferenceAttribute;

/// <summary>Атрибут ASP.NET MVC. При применении к параметру атрибута указывает, что этот параметр является именем действия MVC</summary>
/// <example><code>
/// [ActionName("Foo")]
/// public ActionResult Login(string returnUrl) {
///   ViewBag.ReturnUrl = Url.Action("Foo"); // OK
///   return RedirectToAction("Bar"); // Error: Cannot resolve action
/// }
/// </code></example>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class AspMvcActionSelectorAttribute : Attribute;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field)]
public sealed class HtmlElementAttributesAttribute : Attribute
{
    public HtmlElementAttributesAttribute() { }
    public HtmlElementAttributesAttribute(string name) => Name = name;

    public string Name { get; }
}

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
public sealed class HtmlAttributeValueAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}

// Атрибуты Razor

/// <summary>Атрибут Razor. Указывает, что параметр или метод является секцией Razor. Используйте этот атрибут для пользовательских оберток, подобных <c>System.Web.WebPages.WebPageBase.RenderSection(String)</c></summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
public sealed class RazorSectionAttribute : Attribute;