using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic
{
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    public static class IClonableExtensions
    {
        public static T CloneObject<T>(this T obj) where T : ICloneable
        {
            var result = (T)obj.Clone();
            (result as IInitializable)?.Initialize();
            return result;
        }

        public static T CloneObject<T>(this T obj, Action<T> Initializer) where T : ICloneable
        {
            var result = (T)obj.Clone();
            (result as IInitializable)?.Initialize();
            Initializer(result);
            return result;
        }

        public static T CloneObject<T, TParameter>(this T obj, Action<T, TParameter> Initializer, TParameter parameter) where T : ICloneable
        {
            var result = (T)obj.Clone();
            (result as IInitializable)?.Initialize();
            (result as IInitializable<TParameter>)?.Initialize(parameter);
            Initializer(result, parameter);
            return result;
        }

        public static T CloneObject<T, TParameter>(this T obj, TParameter parameter) where T : ICloneable, IInitializable<TParameter>
        {
            var result = (T)obj.Clone();
            result.Initialize(parameter);
            return result;
        }
    }
}