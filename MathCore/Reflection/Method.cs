using System.ComponentModel;
using MathCore.Annotations;

// ReSharper disable UnusedMember.Global
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
            set => Initialize(_Object, _Name = value, _Private);
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

        public Method(TObject o, string Name, bool Private = false) => Initialize(_Object = o, _Name = Name, _Private = Private);

        private void Initialize([CanBeNull] TObject obj, [NotNull] string MethodName, bool IsPrivate)
        {
            var IsPublic = IsPrivate ? BindingFlags.NonPublic : BindingFlags.Public;
            var IsStatic = obj is null ? BindingFlags.Static : BindingFlags.Instance;

            var type = typeof(TObject);
            if(type == typeof(object) && obj != null)
                type = obj.GetType();

            _MethodInfo = type.GetMethod(MethodName, IsStatic | IsPublic);

            _Method = obj != null && obj is ISynchronizeInvoke
                ? (Func<object[], TResult>)(Args => (TResult)((ISynchronizeInvoke)obj).Invoke((Func<object[], TResult>)PrivateInvoke, new object[] { Args }))
                : PrivateInvoke;
        }

        public TResult Invoke(params object[] Args) => _Method(Args);

        private TResult PrivateInvoke(params object[] Args) => (TResult)_MethodInfo.Invoke(_Object, Args);
    }
}