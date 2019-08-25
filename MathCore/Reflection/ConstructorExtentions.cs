
namespace System.Reflection
{
    public static class ConstructorExtentions
    {
        // Methods
        public static Constructor<TObject> GetConstructor<TObject>(this TObject o, bool Private = false, params Type[] Types) => new Constructor<TObject>(o, Private, Types);
    }
}