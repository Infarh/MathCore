using System;
using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evulations
{
    /// <summary>Вычисление операции с одним операндом</summary>
    /// <typeparam name="TObject">Тип значения операнда</typeparam>
    /// <typeparam name="TValue">Тип значения вычисления</typeparam>
    public class UnaryOperatorEvulation<TObject, TValue> : ConvertEvulation<TObject, TValue>
    {
        /// <summary>Метод, генерирующий функция преобразования значения операнда в значение вычисления</summary>
        /// <param name="OP">Функция преобразования выражения, вычисляющего значение операнда, в выражение, определяющее значение вычисления</param>
        /// <returns></returns>
        protected static Func<TObject, TValue> GetOperation(Func<Ex, Ex> OP)
        {
            var p = Ex.Parameter(typeof(TObject), "p");
            return Ex.Lambda<Func<TObject, TValue>>(OP(p), p).Compile();
        }

        /// <summary>Функция преобразования выражения операнда в выражение вычисления над этим операндом</summary>
        private readonly Func<Ex, Ex> _Operator;

        /// <summary>Инициализация нового унарного вычисления</summary>
        /// <param name="Operator">ОПератор преобразования выражения операнда в выражение вычисления</param>
        public UnaryOperatorEvulation(Func<Ex, Ex> Operator)
            : base(GetOperation(Operator)) => _Operator = Operator;

        /// <summary>Инициализация нового унарного вычисления</summary>
        /// <param name="Operator">Оператор преобразования выражения операнда в выражение вычисления</param>
        /// <param name="value">Вычисление операнда</param>
        public UnaryOperatorEvulation(Func<Ex, Ex> Operator, Evulation<TObject> value)
            : base(value, GetOperation(Operator)) => _Operator = Operator;

        /// <inheritdoc />
        public override Ex GetExpression() => _Operator(InputEvulation.GetExpression());
    }
}