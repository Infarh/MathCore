#if !NET8_0_OR_GREATER

#nullable enable
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace System.Runtime.Serialization.Formatters.Binary;

public static class BinarySerializerExtensions
{
    private static BinaryFormatter? __Formatter;

    [DebuggerStepThrough] 
    public static BinaryFormatter GetSerializer() => __Formatter ??= new BinaryFormatter();
}

#endif