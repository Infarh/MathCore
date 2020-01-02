using System.Linq;
using System.ComponentModel;
using System.Text;
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class EnumExtensions
    {
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded. </exception>
        /// <exception cref="InvalidOperationException">This member belongs to a type that is loaded into the reflection-only context. See How to: Load Assemblies into the Reflection-Only Context.</exception>
        [CanBeNull]
        public static TAttribute[] GetValueAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var attribute_type = typeof(TAttribute);
            var value_type = value.GetType();
            var field = value_type.GetField(value.ToString());
            return (TAttribute[])field.GetCustomAttributes(attribute_type, false);
        }

        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded. </exception>
        [CanBeNull]
        public static string GetValueDescription(this Enum value)
        {
            var attributes = value.GetValueAttribute<DescriptionAttribute>();
            return attributes?.Length == 0 ? "" : attributes?.Aggregate(new StringBuilder(), (S, a) => S.AppendLine(a.Description), S => S.ToString());
        }
    }
}