namespace System
{
    /// <summary>Указывает процедуру без параметров, определённую в классе, которая должна быть вызвана после изменения значения свойства</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class ChangedHandlerAttribute : Attribute
    {
        /// <summary>Имя метода-реакции на изменение значения свойства</summary>
        public string MethodName { get; set; }

        /// <summary>Инициализация нового экземпляра <see cref="DependencyOnAttribute"/></summary>
        public ChangedHandlerAttribute() { }

        /// <summary>Инициализация нового экземпляра <see cref="DependencyOnAttribute"/></summary>
        /// <param name="MethodName">Имя метода-реакции на изменение значения свойства</param>
        public ChangedHandlerAttribute(string MethodName) => this.MethodName = MethodName;
    }
}