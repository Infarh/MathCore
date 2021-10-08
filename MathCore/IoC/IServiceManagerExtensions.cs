#nullable enable
using System;

namespace MathCore.IoC
{
    /// <summary>Методы-расширения для менеджера сервисов</summary>
    // ReSharper disable once InconsistentNaming
    public static class IServiceManagerExtensions
    {
        /// <summary>Получить сервис и сгенерировать исключение если сервис не зарегистрирован</summary>
        /// <typeparam name="T">Тип сервиса</typeparam>
        /// <param name="Manager">Менеджер сервисов</param>
        /// <returns>Запрошенный экземпляр сервиса</returns>
        public static T GetRequired<T>(this IServiceManager Manager) 
            where T : class => 
            Manager.Get<T>() 
            ?? throw new InvalidOperationException("Сервис не найден");
    }
}
