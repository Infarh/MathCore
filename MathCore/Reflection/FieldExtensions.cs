using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    public static class FieldExtensions
    {
        [NotNull] public static Field<TObject, TValue> GetField<TObject, TValue>(this TObject o, string Name, bool Private = false) => new Field<TObject, TValue>(o, Name, Private);
    }
}