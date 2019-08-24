
namespace System.Reflection
{
    public static class PropertyExtentions
    {
        public static Property<TObject, TValue> GetProperty<TObject, TValue>(this TObject o, string Name, bool Private = false)
        {
            return new Property<TObject, TValue>(o, Name, Private);
        }
    }
}