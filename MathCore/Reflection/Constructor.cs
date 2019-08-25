
namespace System.Reflection
{
    public class Constructor<T>
    {
        // Fields
        private ConstructorInfo _Info;
        private T _Object;
        private bool _Private;
        private Type[] _Types;

        // Methods
        public Constructor(T o, bool Private = false, params Type[] Types) => Initialize(_Object = o, Types, _Private = Private);

        // Properties
        public bool IsExist => (_Info != null);

        public T Object
        {
            get => _Object;
            set => Initialize(_Object = value, _Types, _Private);
        }

        public bool Private
        {
            get => _Private;
            set => Initialize(_Object, _Types, _Private = value);
        }

        public Type[] Types
        {
            get => _Types;
            set => Initialize(_Object, _Types = value, _Private);
        }

        private void Initialize(T o, Type[] Types, bool Private)
        {
            _Info =
                o.GetType().GetConstructor(
                    BindingFlags.Instance | (Private ? BindingFlags.NonPublic : BindingFlags.Public), null, Types, null);
        }
    }
}