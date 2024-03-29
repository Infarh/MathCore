﻿#nullable enable
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

/// <summary>
/// Indicates that the value of the marked element could be <c>null</c> sometimes,
/// so the check for <c>null</c> is necessary before its usage
/// </summary>
/// <example><code>
/// [CanBeNull] public object Test() { return null; }
/// public void UseTest() {
///   var p = Test();
///   var s = p.ToString(); // Warning: Possible 'System.NullReferenceException'
/// }
/// </code></example>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.ReturnValue)]
public sealed class CanBeNullAttribute : Attribute;

/// <summary>
/// Indicates that the value of the marked element could never be <c>null</c>
/// </summary>
/// <example><code>
/// [NotNull] public object Foo() {
///   return null; // Warning: Possible 'null' assignment
/// }
/// </code></example>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.ReturnValue)]
public sealed class NotNullAttribute : Attribute;

#if !NET8_0_OR_GREATER
/// <summary>Specifies that the output will be non-null if the named parameter is non-null.</summary>
/// <remarks>Initializes the attribute with the associated parameter name.</remarks>
/// <param name="parameterName">
/// The associated parameter name.  The output will be non-null if the argument to the parameter specified is non-null.
/// </param>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true)]
public sealed class NotNullIfNotNullAttribute(string parameterName) : Attribute
{
    /// <summary>Gets the associated parameter name.</summary>
    public string ParameterName { get; } = parameterName;
}

/// <summary>Specifies that null is disallowed as an input even if the corresponding type allows it.</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
public sealed class DisallowNullAttribute : Attribute;

/// <summary>Specifies that when a method returns <see cref="ReturnValue"/>, the parameter may be null even if the corresponding type disallows it.</summary>
/// <remarks>Initializes the attribute with the specified return value condition.</remarks>
/// <param name="returnValue">
/// The return value condition. If the method returns this value, the associated parameter may be null.
/// </param>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class MaybeNullWhenAttribute(bool returnValue) : Attribute
{
    /// <summary>Gets the return value condition.</summary>
    public bool ReturnValue { get; } = returnValue;
}

/// <summary>Specifies that when a method returns <see cref="ReturnValue"/>, the parameter will not be null even if the corresponding type allows it.</summary>
/// <remarks>Initializes the attribute with the specified return value condition.</remarks>
/// <param name="returnValue">
/// The return value condition. If the method returns this value, the associated parameter will not be null.
/// </param>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class NotNullWhenAttribute(bool returnValue) : Attribute
{

    /// <summary>Gets the return value condition.</summary>
    public bool ReturnValue { get; } = returnValue;
}
#endif

/// <summary>
/// Can be applied to symbols of types derived from IEnumerable as well as to symbols of Task
/// and Lazy classes to indicate that the value of a collection item, of the Task.Result property
/// or of the Lazy.Value property can never be null.
/// </summary>
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

/// <summary>
/// Can be applied to symbols of types derived from IEnumerable as well as to symbols of Task
/// and Lazy classes to indicate that the value of a collection item, of the Task.Result property
/// or of the Lazy.Value property can be null.
/// </summary>
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

/// <summary>
/// Indicates that the marked method builds string by format pattern and (optional) arguments.
/// Parameter, which contains format string, should be given in constructor. The format string
/// should be in <see cref="string.Format(IFormatProvider,string,object[])"/>-like form
/// </summary>
/// <example><code>
/// [StringFormatMethod("message")]
/// public void ShowError(string message, params object[] args) { /* do something */ }
/// public void Foo() {
///   ShowError("Failed: {0}"); // Warning: Non-existing argument in format string
/// }
/// </code></example>
/// <param name="formatParameterName">
/// Specifies which parameter of an annotated method should be treated as format-string
/// </param>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
public sealed class StringFormatMethodAttribute(string formatParameterName) : Attribute
{
    public string FormatParameterName { get; } = formatParameterName;
}

/// <summary>
/// Indicates that the function argument should be string literal and match one
/// of the parameters of the caller function. For example, ReSharper annotates
/// the parameter of <see cref="System.ArgumentNullException"/>
/// </summary>
/// <example><code>
/// public void Foo(string param) {
///   if (param is null)
///     throw new ArgumentNullException("par"); // Warning: Cannot resolve symbol
/// }
/// </code></example>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class InvokerParameterNameAttribute : Attribute;

/// <summary>
/// Indicates that the method is contained in a type that implements
/// <see cref="System.ComponentModel.INotifyPropertyChanged"/> interface
/// and this method is used to notify that some property value changed
/// </summary>
/// <remarks>
/// The method should be non-static and conform to one of the supported signatures:
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
/// Examples of generated notifications:
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

    public string ParameterName { get; }
}

// ReSharper disable CommentTypo
/// <summary>
/// Describes dependency between method input and output
/// </summary>
/// <syntax>
/// <p>Function Definition Table syntax:</p>
/// <list>
/// <item>FDT      ::= FDTRow [;FDTRow]*</item>
/// <item>FDTRow   ::= Input =&gt; Output | Output &lt;= Input</item>
/// <item>Input    ::= ParameterName: Value [, Input]*</item>
/// <item>Output   ::= [ParameterName: Value]* {halt|stop|void|nothing|Value}</item>
/// <item>Value    ::= true | false | null | notnull | canbenull</item>
/// </list>
/// If method has single input parameter, it's name could be omitted.<br/>
/// Using <c>halt</c> (or <c>void</c>/<c>nothing</c>, which is the same)
/// for method output means that the methos doesn't return normally.<br/>
/// <c>canbenull</c> annotation is only applicable for output parameters.<br/>
/// You can use multiple <c>[ContractAnnotation]</c> for each FDT row,
/// or use single attribute with rows separated by semicolon.<br/>
/// </syntax>
/// <examples><list>
/// <item><code>
/// [ContractAnnotation("=> halt")]
/// public void TerminationMethod()
/// </code></item>
/// <item><code>
/// [ContractAnnotation("halt &lt;= condition: false")]
/// public void Assert(bool condition, string text) // regular assertion method
/// </code></item>
/// <item><code>
/// [ContractAnnotation("s:null => true")]
/// public bool IsNullOrEmpty(string s) // string.IsNullOrEmpty()
/// </code></item>
/// <item><code>
/// // A method that returns null if the parameter is null, and not null if the parameter is not null
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

/// <summary>
/// Indicates that marked element should be localized or not
/// </summary>
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

/// <summary>
/// Indicates that the value of the marked type (or its derivatives)
/// cannot be compared using '==' or '!=' operators and <c>Equals()</c>
/// should be used instead. However, using '==' or '!=' for comparison
/// with <c>null</c> is always permitted.
/// </summary>
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

/// <summary>
/// When applied to a target attribute, specifies a requirement for any type marked
/// with the target attribute to implement or inherit specific type or types.
/// </summary>
/// <example><code>
/// [BaseTypeRequired(typeof(IComponent)] // Specify requirement
/// public class ComponentAttribute : Attribute;
/// [Component] // ComponentAttribute requires implementing IComponent interface
/// public class MyComponent : IComponent { }
/// </code></example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
[BaseTypeRequired(typeof(Attribute))]
public sealed class BaseTypeRequiredAttribute(Type baseType) : Attribute
{
    public Type BaseType { get; } = baseType;
}

/// <summary>
/// Indicates that the marked symbol is used implicitly
/// (e.g. via reflection, in external library), so this symbol
/// will not be marked as unused (as well as by other usage inspections)
/// </summary>
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

/// <summary>
/// Should be used on attributes and causes ReSharper
/// to not mark symbols marked with such attributes as unused
/// (as well as by other usage inspections)
/// </summary>
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
    /// <summary>Only entity marked with attribute considered used</summary>
    Access = 1,
    /// <summary>Indicates implicit assignment to a member</summary>
    Assign = 2,
    /// <summary>
    /// Indicates implicit instantiation of a type with fixed constructor signature.
    /// That means any unused constructor parameters won't be reported as such.
    /// </summary>
    InstantiatedWithFixedConstructorSignature = 4,
    /// <summary>Indicates implicit instantiation of a type</summary>
    InstantiatedNoFixedConstructorSignature = 8,
}

/// <summary>
/// Specify what is considered used implicitly
/// when marked with <see cref="MeansImplicitUseAttribute"/>
/// or <see cref="UsedImplicitlyAttribute"/>
/// </summary>
[Flags]
public enum ImplicitUseTargetFlags
{
    Default = Itself,
    Itself = 1,
    /// <summary>Members of entity marked with attribute are considered used</summary>
    Members = 2,
    /// <summary>Entity marked with attribute and all its members considered used</summary>
    WithMembers = Itself | Members
}

/// <summary>
/// This attribute is intended to mark publicly available API
/// which should not be removed and so is treated as used
/// </summary>
[MeansImplicitUse]
public sealed class PublicAPIAttribute : Attribute
{
    public PublicAPIAttribute() { }
    public PublicAPIAttribute(string comment) => Comment = comment;

    public string Comment { get; }
}

/// <summary>
/// Tells code analysis engine if the parameter is completely handled
/// when the invoked method is on stack. If the parameter is a delegate,
/// indicates that delegate is executed while the method is executed.
/// If the parameter is an enumerable, indicates that it is enumerated
/// while the method is executed
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class InstantHandleAttribute : Attribute;

/// <summary>
/// Indicates that a parameter is a path to a file or a folder
/// within a web project. Path can be relative or absolute,
/// starting from web root (~)
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class PathReferenceAttribute : Attribute
{
    public PathReferenceAttribute() { }
    public PathReferenceAttribute([PathReference] string basePath) => BasePath = basePath;

    public string BasePath { get; }
}

// ASP.NET MVC attributes

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

/// <summary>
/// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter
/// is an MVC action. If applied to a method, the MVC action name is calculated
/// implicitly from the context. Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String)</c>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
public sealed class AspMvcActionAttribute : Attribute
{
    public AspMvcActionAttribute() { }
    public AspMvcActionAttribute(string anonymousProperty) => AnonymousProperty = anonymousProperty;

    public string AnonymousProperty { get; }
}

/// <summary>
/// ASP.NET MVC attribute. Indicates that a parameter is an MVC area.
/// Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String)</c>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class AspMvcAreaAttribute : PathReferenceAttribute
{
    public AspMvcAreaAttribute() { }
    public AspMvcAreaAttribute(string anonymousProperty) => AnonymousProperty = anonymousProperty;

    public string AnonymousProperty { get; }
}

/// <summary>
/// ASP.NET MVC attribute. If applied to a parameter, indicates that
/// the parameter is an MVC controller. If applied to a method,
/// the MVC controller name is calculated implicitly from the context.
/// Use this attribute for custom wrappers similar to 
/// <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String, String)</c>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
public sealed class AspMvcControllerAttribute : Attribute
{
    public AspMvcControllerAttribute() { }
    public AspMvcControllerAttribute(string anonymousProperty) => AnonymousProperty = anonymousProperty;

    public string AnonymousProperty { get; }
}

/// <summary>
/// ASP.NET MVC attribute. Indicates that a parameter is an MVC Master.
/// Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Controller.View(String, String)</c>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class AspMvcMasterAttribute : Attribute;

/// <summary>
/// ASP.NET MVC attribute. Indicates that a parameter is an MVC model type.
/// Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Controller.View(String, Object)</c>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class AspMvcModelTypeAttribute : Attribute;

/// <summary>
/// ASP.NET MVC attribute. If applied to a parameter, indicates that
/// the parameter is an MVC partial view. If applied to a method,
/// the MVC partial view name is calculated implicitly from the context.
/// Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Html.RenderPartialExtensions.RenderPartial(HtmlHelper, String)</c>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
public sealed class AspMvcPartialViewAttribute : PathReferenceAttribute;

/// <summary>
/// ASP.NET MVC attribute. Allows disabling all inspections
/// for MVC views within a class or a method.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AspMvcPressuresViewErrorAttribute : Attribute;

/// <summary>
/// ASP.NET MVC attribute. Indicates that a parameter is an MVC display template.
/// Use this attribute for custom wrappers similar to 
/// <c>System.Web.Mvc.Html.DisplayExtensions.DisplayForModel(HtmlHelper, String)</c>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class AspMvcDisplayTemplateAttribute : Attribute;

/// <summary>
/// ASP.NET MVC attribute. Indicates that a parameter is an MVC editor template.
/// Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Html.EditorExtensions.EditorForModel(HtmlHelper, String)</c>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class AspMvcEditorTemplateAttribute : Attribute;

/// <summary>
/// ASP.NET MVC attribute. Indicates that a parameter is an MVC template.
/// Use this attribute for custom wrappers similar to
/// <c>System.ComponentModel.DataAnnotations.UIHintAttribute(System.String)</c>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class AspMvcTemplateAttribute : Attribute;

/// <summary>
/// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter
/// is an MVC view. If applied to a method, the MVC view name is calculated implicitly
/// from the context. Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Controller.View(Object)</c>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
public sealed class AspMvcViewAttribute : PathReferenceAttribute;

/// <summary>
/// ASP.NET MVC attribute. When applied to a parameter of an attribute,
/// indicates that this parameter is an MVC action name
/// </summary>
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

// Razor attributes

/// <summary>
/// Razor attribute. Indicates that a parameter or a method is a Razor section.
/// Use this attribute for custom wrappers similar to 
/// <c>System.Web.WebPages.WebPageBase.RenderSection(String)</c>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
public sealed class RazorSectionAttribute : Attribute;