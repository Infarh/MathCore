using DST = System.Diagnostics.DebuggerStepThroughAttribute;

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Аргументы события исключения</summary>
    /// <typeparam name="TException">Тип исключения</typeparam>
    public class ExceptionEventHandlerArgs<TException> : EventArgs<TException> where TException : Exception
    {
        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Флаг необходимости генерации исключения</summary>
        private bool _Unhandled;

        /// <summary>Флаг признака обработки исключения обработчиками</summary>
        private bool _IsHandled;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Исключение обработано</summary>
        public bool IsHandled { [DST] get => !_Unhandled && _IsHandled; [DST] set => _IsHandled = value; }

        /// <summary>Признак необходимости генерации исключения</summary>
        public bool NeedToThrow => _Unhandled || !IsHandled;

        /* ------------------------------------------------------------------------------------------ */


        /// <summary>Новый аргумент события генерации исключения</summary>
        /// <param name="Error">Исключение</param>
        [DST]
        public ExceptionEventHandlerArgs(TException Error) : base(Error) { }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Исключение обработано</summary>
        [DST]
        public void Handled() => IsHandled = true;

        /// <summary>Исключение должно быть сгенерировано в любом случае</summary>
        [DST]
        public void Unhandled() => _Unhandled = true;

        /* ------------------------------------------------------------------------------------------ */

        [DST]
        public static implicit operator TException(ExceptionEventHandlerArgs<TException> arg) => arg.Argument;

        [DST]
        public static implicit operator ExceptionEventHandlerArgs<TException>(TException exception) => new(exception);

        /* ------------------------------------------------------------------------------------------ */
    }
}