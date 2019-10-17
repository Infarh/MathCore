using System.Diagnostics;
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Runtime.Serialization.Formatters.Binary
{
	public static class BinarySerializerExtentions
	{
		[CanBeNull] private static BinaryFormatter __Formatter;

        [DebuggerStepThrough, NotNull] 
        public static BinaryFormatter GetSerializer() => __Formatter ??= new BinaryFormatter();
    }
}