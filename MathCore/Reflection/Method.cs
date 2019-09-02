using System.ComponentModel;
using System.Diagnostics.Contracts;

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    public class Method<TObject, TResult>
    {
        // Fields
        private MethodInfo _MethodInfo;
        private string _Name;
        private TObject _Object;
        private bool _Private;

        private Func<object[], TResult> _Method;

        // Methods
        public string Name
        {
            get => _Name;
            set
            {
                Contract.Requires(!string.IsNullOrEmpty(value));
                Initialize(_Object, _Name = value, _Private);
            }
        }

        public TObject Object
        {
            get => _Object;
            set => Initialize(_Object = value, _Name, _Private);
        }

        // Properties

        public bool Private
        {
            get => _Private;
            set => Initialize(_Object, _Name, _Private = value);
        }

        public Method(TObject o, string Name, bool Private = false)
        {
            Contract.Requires(!string.IsNullOrEmpty(Name));
            Initialize(_Object = o, _Name = Name, _Private = Private);
        }

        private void Initialize(TObject o, string Name, bool Private)
        {
            Contract.Requires(Name != null);
            Contract.Requires(Name != "");

            var IsPublic = Private ? BindingFlags.NonPublic : BindingFlags.Public;
            var IsStatic = o is null ? BindingFlags.Static : BindingFlags.Instance;

            var type = typeof(TObject);
            if(type == typeof(object) && o != null)
                type = o.GetType();

            _MethodInfo = type.GetMethod(Name, IsStatic | IsPublic);

            _Method = o != null && o is ISynchronizeInvoke
                ? (Func<object[], TResult>)(Args => (TResult)((ISynchronizeInvoke)o).Invoke((Func<object[], TResult>)PrivateInvoke, new object[] { Args }))
                : PrivateInvoke;
        }

        public TResult Invoke(params object[] Args) => _Method(Args);

        private TResult PrivateInvoke(params object[] Args) => (TResult)_MethodInfo.Invoke(_Object, Args);


    }
}