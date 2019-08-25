using System.Diagnostics;
using System.Diagnostics.Contracts;

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
        public bool IsHandled
        {
            [DebuggerStepThrough]
            get { return !_Unhandled && _IsHandled; }
            [DebuggerStepThrough]
            set { _IsHandled = value; }
        }

        /// <summary>Признак необходимости генерации исключения</summary>
        public bool NeedToThrow
        {
            [DebuggerStepThrough]
            get { return _Unhandled || !IsHandled; }
        }

        /* ------------------------------------------------------------------------------------------ */


        /// <summary>Новый аргумент события генерации исключения</summary>
        /// <param name="Error">Исключение</param>
        [DebuggerStepThrough]
        public ExceptionEventHandlerArgs(TException Error) : base(Error) => Contract.Requires(Error != null);

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Исключение обработано</summary>
        [DebuggerStepThrough]
        public void Handled() { IsHandled = true; }

        /// <summary>Исключение должно быть сгенерировано в любом случае</summary>
        [DebuggerStepThrough]
        public void Unhandled() { _Unhandled = true; }

        /* ------------------------------------------------------------------------------------------ */

        [DebuggerStepThrough]
        public static implicit operator TException(ExceptionEventHandlerArgs<TException> arg) { return arg.Argument; }

        [DebuggerStepThrough]
        public static implicit operator ExceptionEventHandlerArgs<TException>(TException exception)
        {
            return new ExceptionEventHandlerArgs<TException>(exception);
        }

        /* ------------------------------------------------------------------------------------------ */
    }
}