
namespace System.Reflection
{
    public static class FieldExtentions
    {
        public static Field<TObject, TValue> GetField<TObject, TValue>(this TObject o, string Name, bool Private = false)
        {
            return new Field<TObject, TValue>(o, Name, Private);
        }
    }
}