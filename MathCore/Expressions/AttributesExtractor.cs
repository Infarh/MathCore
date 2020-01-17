using System.Collections.Generic;
using System.Reflection;
using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions
{
    public class AttributesExtractor
    {
        private readonly MemberInfo _Info;

        public bool Inherit { get; set; }

        [CanBeNull] public Attribute this[string Name] => GetAttributes(Name, Inherit).FirstOrDefault();

        [CanBeNull] public Attribute this[string Name, bool IsInherit] => GetAttributes(Name, IsInherit).FirstOrDefault();

        [CanBeNull]
        public object this[string Name, string ValueName]
        {
            get
            {
                var attribute = this[Name];
                var type = attribute?.GetType();
                var property = type?.GetProperty(ValueName, BindingFlags.Instance | BindingFlags.Public);
                return property is null || !property.CanRead ? null : property.GetValue(attribute, null);
            }
        }

        public AttributesExtractor(MemberInfo Info) => _Info = Info;

        [NotNull] public IEnumerable<Attribute> GetAttributes(string Name) => GetAttributes(Name, Inherit);

        [NotNull]
        public IEnumerable<Attribute> GetAttributes(string Name, bool IsInherit) =>
            _Info.GetCustomAttributes(IsInherit)
               .Cast<Attribute>()
               .Where(a => a.GetType().Name.StartsWith(Name));
    }
}