namespace System.Data
{
    public static class IDataRecordExtensons
    {
        public static T Field<T>(this IDataRecord record, int i)
        {
            if(!record.IsDBNull(i))
                return (T)record.GetValue(i);
            var type = typeof(T);
            if(type.IsClass || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return default;
            throw new NullReferenceException();
        }

        public static T Field<T>(this IDataRecord record, string ColumnName)
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