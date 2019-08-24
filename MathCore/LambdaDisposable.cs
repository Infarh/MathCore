namespace System
{
    /// <summary>Объект, выполняющий указанное действие при сборке мусора</summary>
    public class LambdaDisposable : IDisposable
    {
        public static LambdaDisposable OnDisposed(Action OnDispose) => new LambdaDisposable(OnDispose);

        protected Action _DisposableAction;

        /// <summary>Инициализация нового уничтожаемого объекта с указанием действия при уничтожении</summary>
        /// <param name="DisposableAction">Дейтсвие, выполняемое при уничтожении объекта</param>
        public LambdaDisposable(Action DisposableAction = null) => _DisposableAction = DisposableAction;

        /// <summary>Метод уничтожения объекта, вызывающий указанное действие</summary>
        public virtual void Dispose() => _DisposableAction?.Invoke();
    }

    public class LambdaDisposableObject<T> : LambdaDisposable
    {
        private readonly T _Obj;
        private readonly Action<T, object> _ObjectDisposableAction;

        public T Object => _Obj;
        public object Parameter { get; set; }

        public LambdaDisposableObject(T obj, Action<T, object> ObjectDisposableAction = null, object parameter = null, Action BaseDisposableAction = null) :base(BaseDisposableAction)
        {
            Parameter = parameter;
            _Obj = obj;
            _ObjectDisposableAction = ObjectDisposableAction;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _ObjectDisposableAction?.Invoke(_Obj, Parameter);
            (_Obj as IDisposable)?.Dispose();
            base.Dispose();
        }
    }
}
