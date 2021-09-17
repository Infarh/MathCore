using System;
using System.Diagnostics.CodeAnalysis;
using MathCore.Annotations;
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.IoC
{
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public partial interface IServiceManager : IDisposable, ICloneable<IServiceManager>
    {
        [CanBeNull] object this[[NotNull] Type ServiceType] { get; }

        IServiceRegistrations ServiceRegistrations { get; }

        TServiceInterface Get<TServiceInterface>() where TServiceInterface : class;

        object Get(Type ServiceType);

        /// <summary>Создать объект, возможно неизвестый менеджеру</summary>
        /// <typeparam name="TObject">Тип требуемого объекта</typeparam>
        /// <param name="parameters">Параметры объекта</param>
        /// <returns>Экземпляр объекта в случае его успешного создания</returns>
        [NotNull]
        TObject Create<TObject>([NotNull] params object[] parameters) where TObject : class;

        /// <summary>Создать объект, возможно неизвестый менеджеру</summary>
        /// <param name="ObjectType">Тип требуемого объекта</param>
        /// <param name="parameters">Параметры объекта</param>
        /// <returns>Экземпляр объекта в случае его успешного создания</returns>
        [NotNull]
        object Create([NotNull] Type ObjectType, [NotNull] params object[] parameters);

        ServiceManagerAccessor<TService> ServiceAccessor<TService>() where TService : class;
    }
}