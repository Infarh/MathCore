using Ex = System.Linq.Expressions.Expression;
// ReSharper disable UnusedMember.Global

namespace MathCore.Evaluations
{
    /// <summary>Вычисление значения свойства объекта</summary>
    /// <typeparam name="TObject">Тип объекта, свойство которого надо получить</typeparam>
    /// <typeparam name="TValue">Тип значения свойства</typeparam>
    public class PropertyValueEvulation<TObject, TValue> : UnaryOperatorEvulation<TObject, TValue>
    {
        /// <summary>Инициализация нового вычисления значения свойства объекта</summary>
        /// <param name="PropertyName">Имя свойства</param>
        public PropertyValueEvulation(string PropertyName) : base(e => Ex.Property(e, PropertyName)) { }

        /// <summary>Инициализация нового вычисления свйоства объекта</summary>
        /// <param name="obj">Вычисление объекта, свойство которого надо получить</param>
        /// <param name="PropertyName">Имя свойства объекта</param>
        public PropertyValueEvulation(Evulation<TObject> obj, string PropertyName) : base(e => Ex.Property(e, PropertyName), obj) { }
    }
}