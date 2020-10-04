using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MathCore.Threading.Tasks.Schedulers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using WPFTest.ViewModels;

namespace WPFTest
{
    public partial class App
    {
        private static IHost __Host;

        public static IHost Host => __Host ??= Microsoft.Extensions.Hosting.Host
           .CreateDefaultBuilder(Environment.GetCommandLineArgs())
           .ConfigureAppConfiguration((host, config) => config
               .AddJsonFile("appsettings.json", true, true)
            )
           .ConfigureServices((host, services) => services
               .AddViews()
           )
           .Build()
           .UseApp(Current);

        protected override async void OnStartup(StartupEventArgs e)
        {
            var scheduler = SynchronizationContextScheduler.CurrentContext;

            await Task.Yield().ConfigureAwait(false);

            var thread_pool_thread_id = Thread.CurrentThread.ManagedThreadId;

            var result = await Task.Run(async () =>
                {
                    await Task.Delay(10).ConfigureAwait(false);
                    return 42;
                })
               .ConfigureAwait(scheduler);

            var id = Thread.CurrentThread.ManagedThreadId;

            var host = Host;
            base.OnStartup(e);
            await host.StartAsync();
        }

        public static IServiceProvider Services => Host.Services;

        public static class Scheduler
        {
            public static TaskScheduler UI { get; } = new SynchronizationContextTaskScheduler(SynchronizationContext.Current);
            public static TaskScheduler InThread { get; } = new CurrentThreadTaskScheduler();
            public static TaskScheduler Ordered { get; } = new OrderedTaskScheduler();
            public static TaskScheduler Queued { get; } = new QueuedTaskScheduler();

        }
    }

    internal static class HostExtensions
    {
        public static IHost UseApp(this IHost Host, Application app)
        {
            //app.Startup += async (s, e) => await Host.StartAsync();
            app.Exit += async (s, e) => await Host.DisposeAfterAsync(host => host.StopAsync());
            return Host;
        }
    }
}
