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

        ServiceManagerAccessor<TService> ServiceAccessor<TService>() where TService : class;
    }
}