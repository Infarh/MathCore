using MathCore.Annotations;
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    /// <summary>Класс методов-расширений для <see cref="object"/>, обеспечивающих взаимодействие с его свойствами</summary>
    public static class PropertyExtensions
    {
        /// <summary>Получить объект управления и доступа к информации указанного свойства</summary>
        /// <param name="o">Объект, свойство которого требуется контролировать</param>
        /// <param name="Name">Имя контролируемого свойства</param>
        /// <param name="Private">Осуществить доступ к непубличным свойствам</param>
        /// <typeparam name="TObject">Тип объекта, свойство которого запрашивается</typeparam>
        /// <typeparam name="TValue">Тип значения свойства</typeparam>
        /// <returns>Объект контроля свойства объекта</returns>
        [NotNull] public static Property<TObject, TValue> GetProperty<TObject, TValue>(this TObject o, string Name, bool Private = false) => new Property<TObject, TValue>(o, Name, Private);
    }
}