using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Класс методов-расширений для перечислений <see cref="Enum"/></summary>
    public static class EnumExtensions
    {
        /// <summary>Получить описание поля - значение атрибута <see cref="DescriptionAttribute"/></summary>
        /// <exception cref="TypeLoadException">Если невозможно загрузить атрибуты типа</exception>
        /// <exception cref="InvalidOperationException">
        /// Если этот член принадлежит типу, который загружается в контекст только для отражения.
        /// Смотрите раздел Как загрузить сборки в контекст только для отражения.
        /// </exception>
        [CanBeNull]
        public static TAttribute[] GetValueAttribute<TAttribute>([NotNull] this Enum value) where TAttribute : Attribute
        {
            var attribute_type = typeof(TAttribute);
            var value_type = value.GetType();
            var field = value_type.GetField(value.ToString());
            return (TAttribute[])field.GetCustomAttributes(attribute_type, false);
        }

        [NotNull]
        private static TValue[] GetAttributeValues<TAttribute, TValue>([NotNull] this Enum value, Func<TAttribute, TValue> Selector)
            where TAttribute : Attribute
        {
            var attributes = value.GetValueAttribute<TAttribute>();
            if (attributes is null || attributes.Length == 0) return Array.Empty<TValue>();
            var result = new TValue[attributes.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = Selector(attributes[i]);
            return result;
        }

        /// <summary>Получить описание поля - значение атрибута <see cref="DescriptionAttribute"/></summary>
        /// <exception cref="TypeLoadException">Если невозможно загрузить атрибуты типа</exception>
        [CanBeNull]
        public static string GetDescription([NotNull] this Enum value)
        {
            var descriptions = value.GetAttributeValues<DescriptionAttribute, string>(a => a.Description);
            return descriptions.Length > 0
                ? string.Join(Environment.NewLine, descriptions)
                : string.Empty;
        }

        public static IEnumerable<string> GetDescriptions([NotNull] this Enum value) => value.GetAttributeValues<DescriptionAttribute, string>(a => a.Description);

        /// <summary>Получить описание поля - значение атрибута <see cref="DisplayNameAttribute"/></summary>
        /// <exception cref="TypeLoadException">Если невозможно загрузить атрибуты типа</exception>
        [CanBeNull]
        public static string GetDisplayName([NotNull] this Enum value)
        {
            var descriptions = value.GetAttributeValues<DisplayNameAttribute, string>(a => a.DisplayName);
            return descriptions.Length > 0
                ? string.Join(Environment.NewLine, descriptions)
                : string.Empty;
        }

        /// <summary>Получить описание поля - значение атрибута <see cref="DisplayNameAttribute"/></summary>
        /// <exception cref="TypeLoadException">Если невозможно загрузить атрибуты типа</exception>
        [CanBeNull]
        public static IEnumerable<string> GetDisplayNames([NotNull] this Enum value) => value.GetAttributeValues<DisplayNameAttribute, string>(a => a.DisplayName);
    }
}