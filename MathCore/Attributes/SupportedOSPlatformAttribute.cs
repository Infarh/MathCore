#if NETSTANDARD2_0

namespace System.Runtime.Versioning;

/// <summary>Атрибут ограничения поддерживаемой платформы</summary>
[AttributeUsage(
    AttributeTargets.Assembly |
    AttributeTargets.Class |
    AttributeTargets.Constructor |
    AttributeTargets.Enum |
    AttributeTargets.Event |
    AttributeTargets.Field |
    AttributeTargets.Interface |
    AttributeTargets.Method |
    AttributeTargets.Module |
    AttributeTargets.Property |
    AttributeTargets.Struct,
    AllowMultiple = true,
    Inherited = false)]
public sealed class SupportedOSPlatformAttribute(string PlatformName) : Attribute
{
    /// <summary>Имя платформы</summary>
    public string PlatformName { get; } = PlatformName;
}

/// <summary>Атрибут ограничения неподдерживаемой платформы</summary>
[AttributeUsage(
    AttributeTargets.Assembly |
    AttributeTargets.Class |
    AttributeTargets.Constructor |
    AttributeTargets.Enum |
    AttributeTargets.Event |
    AttributeTargets.Field |
    AttributeTargets.Interface |
    AttributeTargets.Method |
    AttributeTargets.Module |
    AttributeTargets.Property |
    AttributeTargets.Struct,
    AllowMultiple = true,
    Inherited = false)]
public sealed class UnsupportedOSPlatformAttribute(string PlatformName) : Attribute
{
    /// <summary>Имя платформы</summary>
    public string PlatformName { get; } = PlatformName;
}

#endif
