using Ex = System.Linq.Expressions.Expression;

namespace MathCore.Evulations
{
    /// <summary>Вычисление значения поля объекта</summary>
    /// <typeparam name="TObject">Тип объекта, поле которого надо получить</typeparam>
    /// <typeparam name="TValue">Тип значения поля</typeparam>
    public class FieldValueEvulation<TObject, TValue> : UnaryOperatorEvulation<TObject, TValue>
    {
        /// <summary>Инициализация нового вычисления значения поля объекта</summary>
        /// <param name="PropertyName">Имя поля</param>
        public FieldValueEvulation(string PropertyName) : base(e => Ex.Field(e, PropertyName)) { }

        /// <summary>Инициализация нового вычисления поля объекта</summary>
        /// <param name="obj">Вычисление объекта, поле которого надо получить</param>
        /// <param name="FieldName">Имя поля объекта</param>
        public FieldValueEvulation(Evulation<TObject> obj, string FieldName) : base(e => Ex.Field(e, FieldName), obj) { }
    }
}