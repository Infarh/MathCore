using System.Diagnostics.CodeAnalysis;
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    public static class ICloneableExtensions
    {
        [NotNull]
        public static T CloneObject<T>([NotNull] this T obj) where T : ICloneable
        {
            var result = (T)obj.Clone();
            (result as IInitializable)?.Initialize();
            return result;
        }

        [NotNull]
        public static T CloneObject<T>([NotNull] this T obj, [NotNull] Action<T> Initializer) where T : ICloneable
        {
            var result = (T)obj.Clone();
            (result as IInitializable)?.Initialize();
            Initializer(result);
            return result;
        }

        [NotNull]
        public static T CloneObject<T, TParameter>([NotNull] this T obj, [NotNull] Action<T, TParameter> Initializer, TParameter parameter)
            where T : ICloneable
        {
            var result = (T)obj.Clone();
            (result as IInitializable)?.Initialize();
            (result as IInitializable<TParameter>)?.Initialize(parameter);
            Initializer(result, parameter);
            return result;
        }

        [NotNull]
        public static T CloneObject<T, TParameter>([NotNull] this T obj, TParameter parameter)
            where T : ICloneable, IInitializable<TParameter>
        {
            var result = (T)obj.Clone();
            result.Initialize(parameter);
            return result;
        }
    }
}