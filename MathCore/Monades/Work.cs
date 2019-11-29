using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathCore.Annotations;

namespace MathCore.Monades
{
    public class Work
    {
        [NotNull] public static Work Start<T>([NotNull] Action<T> action, T parameter) => Start(action, parameter, null);
        [NotNull] public static Work Start([NotNull] Action action) => Start(action, null);

        [NotNull] internal static Work Start<T>([NotNull] Action<T> action, T parameter, [CanBeNull] Work BaseWork) => new Work(Execute(action, parameter), BaseWork);
        [NotNull] internal static Work Start([NotNull] Action action, [CanBeNull] Work BaseWork) => new Work(Execute(action), BaseWork);

        [NotNull] public static Work<T> StartWithResult<T>([NotNull] Func<T> function) => Work<T>.Start(function);

        [CanBeNull] private readonly Work _BaseWork;

        [CanBeNull] private readonly Exception _Error;

        [NotNull, ItemNotNull]
        public IEnumerable<Exception> Errors
        {
            get
            {
                if(_BaseWork != null)
                    foreach (var error in _BaseWork.Errors)
                        yield return error;
                if (_Error != null)
                    yield return _Error;
            }
        }

        [CanBeNull] public Exception LastError => Errors.FirstOrDefault();

        [NotNull, ItemNotNull]
        public IEnumerable<Work> SubWorks
        {
            get
            {
                var work = _BaseWork;
                while (work != null)
                {
                    yield return work;
                    work = work._BaseWork;
                }
            }
        }

        public virtual bool Success => _Error is null && SubWorks.All(work => work.Success);

        public virtual bool Failure => !Success;

        protected Work(Exception Error, Work BaseWork = null)
        {
            _BaseWork = BaseWork;
            _Error = Error;
        }

        [CanBeNull]
        private static Exception Execute([NotNull] Action action)
        {
            try
            {
                action();
                return null;
            }
            catch (Exception error)
            {
                return error;
            }
        } 

        [CanBeNull]
        protected static Exception Execute<T>([NotNull] Action<T> action, T value)
        {
            try
            {
                action(value);
                return null;
            }
            catch (Exception error)
            {
                return error;
            }
        }

        [NotNull] public Work Anyway([NotNull] Action action) => new Work(Execute(action), this);

        [NotNull] public Work<T> Anyway<T>([NotNull] Func<T> function) => Work<T>.Start(function, this);

        [NotNull] public Work IfSuccess([NotNull] Action action) => Success 
            ? Anyway(action) 
            : new Work(null, this);

        [NotNull] public Work IfFailure([NotNull] Action action) => Success 
            ? new Work(null, _BaseWork) 
            : Anyway(action);

        [NotNull] public Work IfFailure([NotNull] Action<Exception> action) => Success 
            ? new Work(null, this) 
            : new Work(Execute(action, _Error), this);

        [NotNull] public Work SetSuccess() => new SuccessesWork(this);

        private sealed class SuccessesWork : Work
        {
            public override bool Success => true;

            public override bool Failure => false;

            public SuccessesWork(Work BaseWork) : base(null, BaseWork) { }
        }

        [NotNull] public Work SetFailure([CanBeNull] Exception error) => new Work(error, this);
    }

    public sealed class Work<T> : Work
    {
        [NotNull] public static Work<T> Start([NotNull] Func<T> function) => Start(function, null);
        [NotNull] public static Work<T> Start<TParameter>([NotNull] Func<TParameter, T> function, TParameter parameter) => Start(function, parameter, null);

        [NotNull] internal static Work<T> Start([NotNull] Func<T> function, [CanBeNull] Work BaseWork) => new Work<T>(Execute(function), BaseWork);
        [NotNull] internal static Work<T> Start<TParameter>([NotNull] Func<TParameter, T> function, TParameter parameter, [CanBeNull] Work BaseWork) => new Work<T>(Execute(function, parameter), BaseWork);

        public T Result { get; }

        private Work((T Result, Exception Error) Value, Work BaseWork = null) : base(Value.Error, BaseWork) => Result = Value.Result;

        private static (T Result, Exception Error) Execute(Func<T> function)
        {
            try
            {
                return (function(), null);
            }
            catch (Exception error)
            {
                return (default, error);
            }
        } 

        private static (T Result, Exception Error) Execute<TParameter>(Func<TParameter, T> function, TParameter parameter)
        {
            try
            {
                return (function(parameter), null);
            }
            catch (Exception error)
            {
                return (default, error);
            }
        }  

        private static (TResult Result, Exception Error) Execute<TResult>(Func<T, TResult> selector, T Value)
        {
            try
            {
                return (selector(Value), null);
            }
            catch (Exception error)
            {
                return (default, error);
            }
        }

        [NotNull] public Work Anyway([NotNull] Action<T> action) => Start(action, Result, this);

        [NotNull] public Work<TResult> Anyway<TResult>([NotNull] Func<T, TResult> selector) => Work<TResult>.Start(selector, Result);

        [NotNull] 
        public Work<TResult> IfSuccess<TResult>([NotNull] Func<T, TResult> selector) => Success 
            ? Work<TResult>.Start(selector, Result)
            : new Work<TResult>((default, null), this);

        [NotNull] 
        public Work<T> IfSuccess([NotNull] Action<T> action) => Success
            ? new Work<T>((Result, Execute(action, Result)), this)
            : new Work<T>((default, null), this);

        //public Work<TResult> IfFailure<TResult>(Func<Exception, TResult> selector) => Success
        //    ? new Work<TResult>() : 
    }
}
