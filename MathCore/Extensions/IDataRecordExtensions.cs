using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Data
{
    public static class IDataRecordExtensions
    {
        public static T Field<T>([NotNull] this IDataRecord record, int i)
        {
            if(!record.IsDBNull(i))
                return (T)record.GetValue(i);
            var type = typeof(T);
            if(type.IsClass || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return default;
            throw new NullReferenceException();
        }

        public static T Field<T>([NotNull] this IDataRecord record, [NotNull] string ColumnName)
        {
            var ordinal = record.GetOrdinal(ColumnName);

            if(!record.IsDBNull(ordinal))
                return (T)record.GetValue(ordinal);
            var type = typeof(T);

            if(type.IsClass || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return default;
            throw new NullReferenceException();
        }
    }
}