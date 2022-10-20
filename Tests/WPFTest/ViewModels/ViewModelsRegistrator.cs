using Microsoft.Extensions.DependencyInjection;

namespace WPFTest.ViewModels;

public static class ViewModelsRegistrator
{
    public static IServiceCollection AddViews(this IServiceCollection Services) => Services
       .AddSingleton<MainWindowViewModel>()
       .AddSingleton<SecondViewModel>()
    ;
}