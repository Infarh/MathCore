using System;
using System.Diagnostics.Contracts;

namespace MathCore
{
    /// <summary>Форматтер строки с помощью лямда-выражения</summary>
    public class LambdaToString : Factory<string>
    {
        /// <summary>Новый ламбда-форматтер</summary>
        /// <param name="CreateMethod">Метод генерации строки</param>
        public LambdaToString(Func<string> CreateMethod)
            : base(CreateMethod)
        {
            Contract.Requires(CreateMethod != null);
            _RaiseLastChangedEvents = false;
        }

        public override string ToString() => Create();
    }
}
