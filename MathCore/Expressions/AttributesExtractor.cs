using System.Reflection;

using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions;

/// <summary>Объект, осуществляющий извлечение атрибута с указанным именем из информации о методе</summary>
public class AttributesExtractor
{
    /// <summary>Информация о методе</summary>
    private readonly MemberInfo _Info;

    /// <summary>Искать в данных предков цепочки наследования классов/интерфейсов</summary>
    public bool Inherit { get; set; }

    /// <summary>Атрибут (если найден) с указанным именем</summary>
    /// <param name="Name">Имя искомого атрибута</param>
    /// <returns>Атрибут, с указанным именем, либо <see langword="null"/></returns>
    [CanBeNull] public Attribute this[string Name] => GetAttributes(Name, Inherit).FirstOrDefault();

    /// <summary>Атрибут (если найден) с указанным именем</summary>
    /// <param name="Name">Имя искомого атрибута</param>
    /// <param name="IsInherit">Искать в данных предков цепочки наследования классов/интерфейсов</param>
    /// <returns>Атрибут, с указанным именем, либо <see langword="null"/></returns>
    [CanBeNull] public Attribute this[string Name, bool IsInherit] => GetAttributes(Name, IsInherit).FirstOrDefault();

    /// <summary>Получить значение свойства атрибута, определяемое именем атрибута и именем свойства атрибута</summary>
    /// <param name="Name">Имя искомого атрибута</param>
    /// <param name="ValueName">Имя свойства атрибута, значение которого надо получить</param>
    /// <returns>Значение свойства атрибута, если тот был найден и если у него было найдено указанное свойство</returns>
    [CanBeNull]
    public object this[string Name, string ValueName]
    {
        get
        {
            var attribute = this[Name];
            var type      = attribute?.GetType();
            var property  = type?.GetProperty(ValueName, BindingFlags.Instance | BindingFlags.Public);
            return property is null || !property.CanRead ? null : property.GetValue(attribute, null);
        }
    }

    /// <summary>Инициализация нового экземпляра <see cref="AttributesExtractor"/></summary>
    /// <param name="Info">Мета-информация о методе, для для которого надо осуществлять поиск атрибутов</param>
    public AttributesExtractor(MemberInfo Info) => _Info = Info;

    /// <summary>Получить все атрибуты с указанным именем</summary>
    /// <param name="Name">Имя искомых атрибутов</param>
    /// <returns>Перечисление атрибутов с указанным именем</returns>
    [NotNull] public IEnumerable<Attribute> GetAttributes(string Name) => GetAttributes(Name, Inherit);

    /// <summary>Получить все атрибуты с указанным именем</summary>
    /// <param name="Name">Имя искомых атрибутов</param>
    /// <param name="IsInherit">Искать в данных предков цепочки наследования классов/интерфейсов</param>
    /// <returns>Перечисление атрибутов с указанным именем</returns>
    [NotNull]
    public IEnumerable<Attribute> GetAttributes(string Name, bool IsInherit) =>
        _Info.GetCustomAttributes(IsInherit)
           .Cast<Attribute>()
           .Where(a => a.GetType().Name.StartsWith(Name));
}