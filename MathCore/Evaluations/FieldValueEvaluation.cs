using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evaluations
{
    /// <summary>Вычисление значения поля объекта</summary>
    /// <typeparam name="TObject">Тип объекта, поле которого надо получить</typeparam>
    /// <typeparam name="TValue">Тип значения поля</typeparam>
    public class FieldValueEvaluation<TObject, TValue> : UnaryOperatorEvaluation<TObject, TValue>
    {
        /// <summary>Инициализация нового вычисления значения поля объекта</summary>
        /// <param name="PropertyName">Имя поля</param>
        public FieldValueEvaluation(string PropertyName) : base(e => Ex.Field(e, PropertyName)) { }

        /// <summary>Инициализация нового вычисления поля объекта</summary>
        /// <param name="obj">Вычисление объекта, поле которого надо получить</param>
        /// <param name="FieldName">Имя поля объекта</param>
        public FieldValueEvaluation(Evaluation<TObject> obj, string FieldName) : base(e => Ex.Field(e, FieldName), obj) { }
    }
}