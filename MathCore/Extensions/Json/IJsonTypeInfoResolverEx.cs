#nullable enable
#if NET8_0_OR_GREATER

using System.Text.Json.Serialization.Metadata;

namespace MathCore.Extensions.Json;

public static class IJsonTypeInfoResolverEx
{
    public static IJsonTypeInfoResolver WithAddedModifier<T>(this IJsonTypeInfoResolver resolver, Action<JsonTypeInfo> modifier) =>
        resolver.WithAddedModifier(t =>
        {
            if (t.Type == typeof(T))
                modifier(t);
        });

    public static IJsonTypeInfoResolver WithAddedModifier<T1, T2>(this IJsonTypeInfoResolver resolver, Action<JsonTypeInfo> modifier) =>
        resolver.WithAddedModifier(t =>
        {
            if (t.Type == typeof(T1) || t.Type == typeof(T2))
                modifier(t);
        });

    public static IJsonTypeInfoResolver WithAddedModifier<T1, T2, T3>(this IJsonTypeInfoResolver resolver, Action<JsonTypeInfo> modifier) =>
        resolver.WithAddedModifier(t =>
        {
            if (t.Type == typeof(T1) || t.Type == typeof(T2) || t.Type == typeof(T3))
                modifier(t);
        });

    public static IJsonTypeInfoResolver WithAddedModifier(
        this IJsonTypeInfoResolver resolver,
        Action<JsonTypeInfo> modifier,
        params IReadOnlyList<Type> Types) =>
        resolver.WithAddedModifier(t =>
        {
            if (Types.Count == 0)
                modifier(t);
            else
                foreach (var type in Types)
                {
                    if (t.Type != type) continue;
                    modifier(t);
                    break;
                }
        });

    public static IJsonTypeInfoResolver RemoveProperty<T>(this IJsonTypeInfoResolver resolver, string PropertyName, StringComparison comparison = StringComparison.Ordinal) =>
        resolver.WithAddedModifier<T>(t => t.Properties.Remove(t.Properties.First(p => p.Name.Equals(PropertyName, comparison))));

    public static IJsonTypeInfoResolver RemoveProperty<T1, T2>(this IJsonTypeInfoResolver resolver, string PropertyName, StringComparison comparison = StringComparison.Ordinal) =>
        resolver.WithAddedModifier<T1, T2>(t => t.Properties.Remove(t.Properties.First(p => p.Name.Equals(PropertyName, comparison))));

    public static IJsonTypeInfoResolver RemoveProperty<T1, T2, T3>(this IJsonTypeInfoResolver resolver, string PropertyName, StringComparison comparison = StringComparison.Ordinal) =>
        resolver.WithAddedModifier<T1, T2, T3>(t => t.Properties.Remove(t.Properties.First(p => p.Name.Equals(PropertyName, comparison))));

    public static IJsonTypeInfoResolver RemoveProperty(
        this IJsonTypeInfoResolver resolver,
        string PropertyName,
        params IReadOnlyList<Type> Types) =>
        resolver.WithAddedModifier(t => t.Properties.Remove(t.Properties.First(p => p.Name.Equals(PropertyName, StringComparison.Ordinal))), Types);

    public static IJsonTypeInfoResolver RemoveProperty(
        this IJsonTypeInfoResolver resolver,
        string PropertyName,
        StringComparison comparison,
        params IReadOnlyList<Type> Types) =>
        resolver.WithAddedModifier(t => t.Properties.Remove(t.Properties.First(p => p.Name.Equals(PropertyName, comparison))), Types);

    public static IJsonTypeInfoResolver OnDeserialized<T>(this IJsonTypeInfoResolver resolver, Action<T> OnDeserialized) =>
        resolver.WithAddedModifier<T>(t => t.OnDeserialized += o => OnDeserialized((T)o));
}

#endif