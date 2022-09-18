#nullable enable
using System;

using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC;

public partial interface IServiceManager
{
    ServiceRegistration RegisterSingleCall(Type ServiceType);
        
    ServiceRegistration RegisterSingleCall(Type InterfaceType, Type ServiceType);
        
    SingleCallServiceRegistration<TService> RegisterSingleCall<TService>() where TService : class;
        
    SingleCallServiceRegistration<TService> RegisterSingleCall<TServiceInterface, TService>() where TService : class, TServiceInterface;
        
    SingleCallServiceRegistration<TService> RegisterSingleCall<TService>(Func<TService> FactoryMethod) where TService : class;
}