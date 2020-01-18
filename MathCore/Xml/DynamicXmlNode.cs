using System.Dynamic;
using System.Reflection;
using MathCore.Annotations;

// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Xml.Linq
{
    public class DynamicXmlNode : DynamicObject
    {
        private readonly XElement _Node;

        public DynamicXmlNode() { }

        public DynamicXmlNode(XElement node) => _Node = node;

        public DynamicXmlNode([NotNull] string name) => _Node = new XElement(name);

        public override bool TrySetMember(SetMemberBinder binder, [NotNull] object value)
        {
            var node = _Node.Element(binder.Name);
            if(node != null)
                node.SetValue(value);
            else
                _Node.Add(value.GetType() == typeof(DynamicXmlNode)
                    ? new XElement(binder.Name)
                    : new XElement(binder.Name, value));
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, [CanBeNull] out object result)
        {
            var get_node = _Node.Element(binder.Name);
            if(get_node != null)
            {
                result = new DynamicXmlNode(get_node);
                return true;
            }
            result = null;
            return false;
        }

        public override bool TryConvert([NotNull] ConvertBinder binder, [CanBeNull] out object result)
        {
            if(binder.Type == typeof(string))
            {
                result = _Node.Value;
                return true;
            }
            result = null;
            return false;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
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
}