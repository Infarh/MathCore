using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    public static class PropertyExtensions
    {
        [NotNull] public static Property<TObject, TValue> GetProperty<TObject, TValue>(this TObject o, string Name, bool Private = false) => new Property<TObject, TValue>(o, Name, Private);
    }
}