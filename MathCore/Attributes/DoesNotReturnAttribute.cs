#if !NET8_0_OR_GREATER

using System;

namespace MathCore.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
internal sealed class DoesNotReturnAttribute : Attribute;

#endif