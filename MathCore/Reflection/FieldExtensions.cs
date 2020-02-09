using MathCore.Annotations;
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    /// <summary>Класс методов-расширений для полей класса</summary>
    public static class FieldExtensions
    {
        /// <summary>Получить объект контроля поля объекта</summary>
        /// <typeparam name="TObject">Тип объекта, поле которого требуется контролировать</typeparam>
        /// <typeparam name="TValue">Тип значения поля</typeparam>
        /// <param name="o">Объект, поле которого требуется контролировать</param>
        /// <param name="Name">Имя контролируемого поля</param>
        /// <param name="Private">Искать приватное поле?</param>
        /// <returns>Объект, осуществляющий контроль поля</returns>
        [NotNull] public static Field<TObject, TValue> GetField<TObject, TValue>(this TObject o, string Name, bool Private = false) => new Field<TObject, TValue>(o, Name, Private);
    }
}