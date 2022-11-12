using MathCore.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace WPFTest.Services;

public static class ServiceRegistrator
{
    public static IServiceCollection AddServices(this IServiceCollection services) => services
       .AddSingleton<IMessenger, Messenger>();
}