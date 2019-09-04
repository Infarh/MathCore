
// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    public class Field<TObject, TValue>
    {
        private FieldInfo _FieldInfo;
        private string _Name;
        private TObject _Object;
        private bool _Private;

        public bool IsExist => _FieldInfo != null;

        public string Name { get => _Name; set => Initialize(_Object, _Name = value, _Private); }

        public TObject Object { get => _Object; set => Initialize(_Object = value, _Name, _Private); }

        public bool Private { get => _Private; set => Initialize(_Object, _Name, _Private = value); }

        public TValue Value { get => (TValue)_FieldInfo.GetValue(_Object); set => _FieldInfo.SetValue(_Object, value); }

        public Field(TObject o, string Name, bool Private = false) => Initialize(_Object = o, _Name = Name, _Private = Private);

        private void Initialize(TObject o, string FieldName, bool IsPrivate)
        {
            var type = typeof(TObject);
            if(type == typeof(TObject) && o != null)
                type = o.GetType();

            var is_private = IsPrivate ? BindingFlags.NonPublic : BindingFlags.Public;
            var is_static = o is null ? BindingFlags.Static : BindingFlags.Instance;

            _FieldInfo = type.GetField(FieldName, is_private | is_static);
        }
    }
}