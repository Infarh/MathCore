using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    public static class ConstructorExtensions
    {
        // Methods
        [NotNull] public static Constructor<TObject> GetConstructor<TObject>(this TObject o, bool Private = false, params Type[] Types) => new Constructor<TObject>(o, Private, Types);
    }
}