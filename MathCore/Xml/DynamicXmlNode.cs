using System.Dynamic;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace System.Xml.Linq
{
    public class DynamicXmlNode : DynamicObject
    {
        private readonly XElement _Node;
        public DynamicXmlNode() { }
        public DynamicXmlNode(XElement node) => _Node = node;
        public DynamicXmlNode(string name) => _Node = new XElement(name);
        public override bool TrySetMember(SetMemberBinder binder, object value)
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

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var lv_GetNode = _Node.Element(binder.Name);
            if(lv_GetNode != null)
            {
                result = new DynamicXmlNode(lv_GetNode);
                return true;
            }
            result = null;
            return false;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
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
            var xtype = typeof(XElement);
            try
            {
                const BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance;
                result = xtype.InvokeMember(binder.Name, flags, null, _Node, args);
                return true;
            } catch
            {
                result = null;
                return false;
            }
        }
    }
}
