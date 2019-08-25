
namespace System.Reflection
{
    public static class MethodExtentions
    {
        // Methods
        public static Method<TObject, TValue> GetMethod<TObject, TValue>(this TObject o, string Name, bool Private = false) => new Method<TObject, TValue>(o, Name, Private);
    }
}