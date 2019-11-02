using MathCore.Annotations;

namespace MathCore
{
    /// <summary>Объект, обладающий именем</summary>
    public interface INamedRead
    {
        /// <summary>Имя объекта</summary>
        string Name { get; }
    }

    /// <summary>Объект с возможностью задавать имя</summary>
    public interface INamed : INamedRead
    {
        /// <summary>Имя объекта</summary>
        new string Name { get; set; }
    }

    /// <summary>Методы-расширения интерфейса именованных объектов</summary>
    public static class NamedInterfaceExtensions
    {
        /// <summary>Изменить имя объекта</summary>
        /// <param name="obj">Объект, имя которого требуется изменить</param>
        /// <param name="NewName">Новое имя объекта</param>
        public static void Rename([NotNull] this INamed obj, string NewName) => obj.Name = NewName;
    }
}