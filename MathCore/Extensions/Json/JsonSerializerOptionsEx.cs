#nullable enable
#if NET8_0_OR_GREATER

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace MathCore.Extensions.Json;

public static class JsonSerializerOptionsEx
{
    public static JsonSerializerOptions Idented(this JsonSerializerOptions opt) => new(opt) { WriteIndented = true };

    public static JsonSerializerOptions WithPropertyNamingPolicy(this JsonSerializerOptions opt, JsonNamingPolicy? NamingPolicy) =>
        new(opt)
        {
            PropertyNamingPolicy = NamingPolicy
        };

    public static JsonSerializerOptions WithPropertyNamingPolicyCamelCase(this JsonSerializerOptions opt) =>
        new(opt)
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    public static JsonSerializerOptions RemoveProperty<T>(this JsonSerializerOptions opt, string PropertyName, StringComparison comparison = StringComparison.Ordinal) =>
        new(opt)
        {
            TypeInfoResolverChain =
            {
                new DefaultJsonTypeInfoResolver()
                    .RemoveProperty<T>(PropertyName, comparison)
            }
        };

    public static JsonSerializerOptions RemoveProperty<T1, T2>(this JsonSerializerOptions opt, string PropertyName, StringComparison comparison = StringComparison.Ordinal) =>
        new(opt)
        {
            TypeInfoResolverChain =
            {
                new DefaultJsonTypeInfoResolver()
                    .RemoveProperty<T1, T2>(PropertyName, comparison)
            }
        };

    public static JsonSerializerOptions RemoveProperty<T1, T2, T3>(this JsonSerializerOptions opt, string PropertyName, StringComparison comparison = StringComparison.Ordinal) =>
        new(opt)
        {
            TypeInfoResolverChain =
            {
                new DefaultJsonTypeInfoResolver()
                    .RemoveProperty<T1, T2, T3>(PropertyName, comparison)
            }
        };

    public static JsonSerializerOptions RemoveProperty(
        this JsonSerializerOptions opt, 
        string PropertyName, 
        StringComparison comparison,
        params Type[] Types) =>
        new(opt)
        {
            TypeInfoResolverChain =
            {
                new DefaultJsonTypeInfoResolver()
                    .RemoveProperty(PropertyName, comparison, Types)
            }
        };

    public static JsonSerializerOptions RemoveProperty(
        this JsonSerializerOptions opt, 
        string PropertyName, 
        params Type[] Types) =>
        new(opt)
        {
            TypeInfoResolverChain =
            {
                new DefaultJsonTypeInfoResolver()
                    .RemoveProperty(PropertyName, Types)
            }
        };

    public static JsonSerializerOptions OnDeserialized<T>(this JsonSerializerOptions opt, Action<T> OnDeserialized) =>
        new(opt)
        {
            TypeInfoResolverChain =
            {
                new DefaultJsonTypeInfoResolver()
                    .OnDeserialized(OnDeserialized)
            }
        };
}

#endif