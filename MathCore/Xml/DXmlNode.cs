#nullable enable
using System.Dynamic;
using System.Reflection;

// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Xml.Linq;

public class DXmlNode : DynamicObject
{
    public static DXmlNode From(XmlElement xml) => new(XElement.Load(xml.CreateNavigator().ReadSubtree()));

    public static DXmlNode From(XElement xml) => new(xml);

    private readonly XElement _Node;

    public DXmlNode(string name = "obj") : this(new XElement(name)) { }

    public DXmlNode(XElement node) => _Node = node;

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        var node = _Node.Element(binder.Name);
        if(node != null)
            node.SetValue(value);
        else
            _Node.Add(value.GetType() == typeof(DXmlNode)
                ? new XElement(binder.Name)
                : new XElement(binder.Name, value));
        return true;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        var get_node = _Node.Element(binder.Name);
        if(get_node != null)
        {
            result = new DXmlNode(get_node);
            return true;
        }
        result = null;
        return false;
    }

    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        var conversion_type = binder.Type;
        if(conversion_type == typeof(string))
        {
            result = _Node.Value;
            return true;
        }

        var converter = conversion_type.GetTypeConverter();
        if (converter.CanConvertFrom(typeof(string)))
        {
            result = converter.ConvertFrom(_Node.Value);
            return true;
        }

        result = null;
        return false;
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object? result)
    {
        var x_type = typeof(XElement);
        try
        {
            const BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance;
            result = x_type.InvokeMember(binder.Name, flags, null, _Node, args);
            return true;
        } catch
        {
            result = null;
            return false;
        }
    }
}