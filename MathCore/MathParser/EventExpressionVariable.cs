using System;

namespace MathCore.MathParser
{
    /// <summary>Событийная переменная</summary>
    /// <remarks>Переменная математического выражения, значение которой определяется через генерацию события</remarks>
    public class EventExpressionVariable : ExpressionVariabel
    {
        /// <summary>Событие запроса значения переменной</summary>
        public event EventHandler<EventArgs<double>> Call;

        /// <summary>Метод генерации события</summary>
        /// <param name="Args">Аргумент события</param>
        protected virtual void OnCall(EventArgs<double> Args) => Call?.Invoke(this, Args);

        /// <summary>Аргумент события</summary>
        private readonly EventArgs<double> _EventArg = new EventArgs<double>(0);

        /// <summary>Флаг предварительной очистки значения аргумента события</summary>
        private bool _ClearAtCall;

        /// <summary>Значение переменной</summary>
        public override double Value { get => _EventArg.Argument; set => _EventArg.Argument = value; }

        /// <summary>Признак предвычислимости всегда = false</summary>
        public override bool IsPrecomputable => false;

        /// <summary>Флаг предварительной очистки значения аргумента события</summary>
        public bool ClearAtCall { get => _ClearAtCall; set => _ClearAtCall = value; }

        /// <summary>Инициализация новой событийной переменной</summary>
        public EventExpressionVariable() : this("") { }

        /// <summary>Инициализация новой событийной переменной</summary>
        /// <param name="Name">Имя переменной</param>
        public EventExpressionVariable(string Name) : base(Name) { }

        /// <summary>Получение значения переменной</summary>
        /// <returns>Значение переменой</returns>
        public override double GetValue()
        {
            if(_ClearAtCall)
                _EventArg.Argument = 0;
            OnCall(_EventArg);
            return base.Value = _EventArg.Argument;
        }

        /// <summary>Метод клонирования событийной переменной</summary>
        /// <returns>Клонированная событийная переменная</returns>
        public override ExpressionVariabel Clone() => new EventExpressionVariable(Name) { ClearAtCall = ClearAtCall, Value = Value };
    }
}