using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest;

public static class AppDomainTest
{
    public static void Run()
    {
        var context = new AssemblyLoadContext("test", true);

        context.Unloading += OnUnloading;
        context.Resolving += OnResolving;

        var asm = context.LoadFromAssemblyPath(typeof(AppDomainTest).Assembly.Location);

        var types = asm.GetTypes();

        var type = asm.GetType(typeof(DomainEntryPoint).FullName);

        var result = (string)type.GetMethod(nameof(DomainEntryPoint.Compute)).Invoke(null, [3, 5])!;

        context.Unload();
    }

    private static Assembly? OnResolving(AssemblyLoadContext context, AssemblyName assembly)
    {
        return null;
    }


    private static void OnUnloading(AssemblyLoadContext context)
    {
        
    }
}

internal static class DomainEntryPoint
{
    public static string Compute(int a, int b) => $"{AppDomain.CurrentDomain.FriendlyName}:{a + b}";
}
