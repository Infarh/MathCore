using System;
using System.Diagnostics.CodeAnalysis;

using MathCore.Annotations;
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.IoC
{
    /// <summary>Менеджер сервисов</summary>
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public partial interface IServiceManager : IDisposable, ICloneable<IServiceManager>, IServiceProvider
    {
        /// <summary>Сервис по требуемому типу</summary>
        /// <param name="ServiceType">Тип необходимого сервиса</param>
        /// <returns>Экземпляр затребованного сервиса</returns>
        [CanBeNull] object this[[Annotations.NotNull] Type ServiceType] { get; }

        /// <summary>Информация о зарегистрированных сервисах</summary>
        IServiceRegistrations ServiceRegistrations { get; }

        /// <summary>Получить экземпляр сервиса</summary>
        /// <typeparam name="TServiceInterface">Тип сервиса</typeparam>
        /// <returns>Экземпляр сервиса</returns>
        [CanBeNull] TServiceInterface Get<TServiceInterface>() where TServiceInterface : class;
        /// <summary>Получить экземпляр сервиса</summary>
        /// <typeparam name="TServiceInterface">Тип сервиса</typeparam>
        /// <param name="parameters">Параметры создания экземпляра сервиса</param>
        /// <returns>Экземпляр сервиса</returns>
        [CanBeNull] TServiceInterface Get<TServiceInterface>([Annotations.NotNull] params object[] parameters) where TServiceInterface : class;

        /// <summary>Получить экземпляр сервиса</summary>
        /// <param name="ServiceType">Тип сервиса</param>
        /// <returns>Экземпляр сервиса</returns>
        [CanBeNull] object Get([Annotations.NotNull] Type ServiceType);
        /// <summary>Получить экземпляр сервиса</summary>
        /// <param name="ServiceType">Тип сервиса</param>
        /// <param name="parameters">Параметры создания экземпляра сервиса</param>
        /// <returns>Экземпляр сервиса</returns>
        [CanBeNull] object Get([Annotations.NotNull] Type ServiceType, [Annotations.NotNull] params object[] parameters);

        /// <summary>Создать объект, возможно неизвестый менеджеру</summary>
        /// <typeparam name="TObject">Тип требуемого объекта</typeparam>
        /// <param name="parameters">Параметры объекта</param>
        /// <returns>Экземпляр объекта в случае его успешного создания</returns>
        [Annotations.NotNull]
        TObject Create<TObject>([Annotations.NotNull] params object[] parameters) where TObject : class;

        /// <summary>Создать объект, возможно неизвестный менеджеру</summary>
        /// <param name="ObjectType">Тип требуемого объекта</param>
        /// <param name="parameters">Параметры объекта</param>
        /// <returns>Экземпляр объекта в случае его успешного создания</returns>
        [Annotations.NotNull]
        object Create([Annotations.NotNull] Type ObjectType, [Annotations.NotNull] params object[] parameters);

        /// <summary>Объект доступа к экземплярам сервиса <typeparamref name="TService"/></summary>
        /// <typeparam name="TService">Тип требуемого сервиса</typeparam>
        /// <returns>Объект доступа к экземплярам сервиса <typeparamref name="TService"/></returns>
        ServiceManagerAccessor<TService> ServiceAccessor<TService>() where TService : class;

        /// <summary>Выполнить метод</summary>
        /// <param name="Instance">Экземпляр объекта</param>
        /// <param name="MethodName">Имя метода, который требуется выполнить</param>
        /// <returns>Результат выполнения</returns>
        object Run(object Instance, string MethodName);

        /// <summary>Выполнить статический метод</summary>
        /// <typeparam name="T">Тип, в котором объявлен статический метод</typeparam>
        /// <param name="StaticMethodName">Имя статического метода, который требуется выполнить</param>
        /// <returns>Результат выполнения</returns>
        object Run<T>(string StaticMethodName);
    }
}