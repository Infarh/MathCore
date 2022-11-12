// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable once CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable AnnotateNotNullTypeMember
// ReSharper disable MemberCanBeProtected.Global
namespace System.Linq.Expressions;

public class ItemBase
{
    protected readonly object _Object;
    protected readonly Type _ObjectType;
    protected readonly string _Name;
    protected AttributesExtractor _ObjectAttributes;

    public string Name => _Name;

    public object Object => _Object;

    public AttributesExtractor ObjectAttribute => _ObjectAttributes ??= new AttributesExtractor(_ObjectType);

    protected ItemBase(object Obj, string Name) : this(Obj.GetType(), Name) => _Object = Obj;

    public ItemBase(Type type, string Name)
    {
        _ObjectType = type;
        _Name       = Name;
    }
}