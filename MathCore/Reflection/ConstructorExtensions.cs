using MathCore.Annotations;
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    /// <summary>Класс методов-расширений для взаимодействия с конструкторами классов по средствам рефлексии</summary>
    public static class ConstructorExtensions
    {
        /// <summary>Получить конструктор для объекта</summary>
        /// <param name="o">Объект, конструктор класса которого требуется получить</param>
        /// <param name="Private">Искать приватный конструктор?</param>
        /// <param name="ArgumentTypes">Перечисление типов параметров искомого конструктора</param>
        /// <typeparam name="TObject">Тип объекта, для которого извлекается конструктор</typeparam>
        /// <returns>Объект, управляющий конструктором класса объекта</returns>
        [NotNull] public static Constructor<TObject> GetObjectConstructor<TObject>([NotNull] this TObject o, bool Private = false, [NotNull] params Type[] ArgumentTypes) => new Constructor<TObject>(o, Private, ArgumentTypes);

        /// <summary>Получить конструктор для объекта</summary>
        /// <param name="type">Тип, из которого требуется извлечь конструктор</param>
        /// <param name="Private">Искать приватный конструктор?</param>
        /// <param name="ArgumentTypes">Перечисление типов параметров искомого конструктора</param>
        /// <typeparam name="TObject">Тип объекта, для которого извлекается конструктор</typeparam>
        /// <returns>Объект, управляющий конструктором класса объекта</returns>
        [NotNull] public static Constructor<TObject> GetTypeConstructor<TObject>(this Type type, bool Private = false, [NotNull] params Type[] ArgumentTypes) => new Constructor<TObject>(type, Private, ArgumentTypes);
    }
}