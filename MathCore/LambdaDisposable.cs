using MathCore.Annotations;

// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Объект, выполняющий указанное действие при сборке мусора</summary>
    public class LambdaDisposable : IDisposable
    {
        /// <summary>При освобождении выполнить указанное действие</summary>
        /// <param name="OnDispose">Действие, выполняемое при освобождении</param>
        /// <returns>Объект <see cref="LambdaDisposable"/></returns>
        [NotNull]
        public static LambdaDisposable OnDisposed(Action OnDispose) => new LambdaDisposable(OnDispose);

        /// <summary>Действие, выполняемое при разрушении объекта</summary>
        protected readonly Action _DisposableAction;

        /// <summary>Инициализация нового уничтожаемого объекта с указанием действия при уничтожении</summary>
        /// <param name="DisposableAction">Действие, выполняемое при уничтожении объекта</param>
        public LambdaDisposable(Action DisposableAction = null) => _DisposableAction = DisposableAction;

        /// <summary>Метод уничтожения объекта, вызывающий указанное действие</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Метод уничтожения объекта, вызывающий указанное действие</summary>
        /// <param name="Disposing">Если истина, то освободить управляемые ресурсы</param>
        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposing) return;
            _DisposableAction?.Invoke();
        }
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
        protected override void Dispose(bool Disposing)
        {
            if (!Disposing) return;
            _ObjectDisposableAction?.Invoke(_Obj, Parameter);
            (_Obj as IDisposable)?.Dispose();
            base.Dispose();
        }
    }
}