using System.Collections.Generic;
using System.Reflection;

namespace System.Linq.Expressions
{
    public class AttributesExtractor
    {
        private readonly MemberInfo _Info;
        public bool Inherit { get; set; }

        public Attribute this[string Name] => GetAttributes(Name, Inherit).FirstOrDefault();
        public Attribute this[string Name, bool Inherit] => GetAttributes(Name, Inherit).FirstOrDefault();

        public object this[string Name, string ValueName]
        {
            get
            {
                var attribute = this[Name];
                if(attribute is null) return null;
                var type = attribute.GetType();
                var property = type.GetProperty(ValueName, BindingFlags.Instance | BindingFlags.Public);
                if(property is null || !property.CanRead) return null;
                return property.GetValue(attribute, null);
            }
        }

        public AttributesExtractor(MemberInfo Info) => _Info = Info;

        public IEnumerable<Attribute> GetAttributes(string Name) => GetAttributes(Name, Inherit);

        public IEnumerable<Attribute> GetAttributes(string Name, bool Inherit)
        {
            return _Info.GetCustomAttributes(Inherit)
               .Cast<Attribute>()
               .Where(a => a.GetType().Name.StartsWith(Name));
        }
    }
}