using System.Collections;
using System.Reflection;

using MathCore.Graphs;

namespace MathCore.Extensions.Reflection;

public static class AssemblyEx
{
#if NET8_0_OR_GREATER
    public static (System.Runtime.Loader.AssemblyLoadContext, Assembly asm) LoadToContext(this Assembly assembly, string? ContextName = null, bool Collectable = true)
    {
        var context = new System.Runtime.Loader.AssemblyLoadContext(ContextName, Collectable);

        var asm = context.LoadFromAssemblyPath(assembly.Location);

        return (context, asm);
    }

    public static AssemblyIterator AsDependenciesGraph(this Assembly assembly) => new(assembly, new($"{assembly.FullName}-{Guid.NewGuid()}", true));

    public readonly struct AssemblyIterator(Assembly assembly, System.Runtime.Loader.AssemblyLoadContext? context) : IGraphNode<Assembly>, IDisposable
    {
        public Assembly Value { get; } = assembly;

        public IEnumerable<IGraphNode<Assembly>> Childs
        {
            get
            {
                if(context is null)
                    foreach (var assembly_name in Value.GetReferencedAssemblies())
                        yield return new AssemblyIterator(Assembly.Load(assembly_name), null);
                else
                    foreach (var assembly_name in Value.GetReferencedAssemblies())
                        yield return new AssemblyIterator(context.LoadFromAssemblyName(assembly_name), context);
            }
        }

        public IEnumerator<IGraphNode<Assembly>> GetEnumerator() => Childs.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose() => context?.Unload();
    }
#endif

    extension(Assembly asm)
    {
        public Version GetVersion(string? EnvVersionVarName = null)
        {
            string? version;
            if (EnvVersionVarName is { Length: > 0 })
            {
                version = Environment.GetEnvironmentVariable(EnvVersionVarName);
                if (version is { Length: > 0 })
                    return Version.Parse(version);
            }

            var executing_assembly = Assembly.GetExecutingAssembly();

            var version_attribute = executing_assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            version = version_attribute?.InformationalVersion;
            if (version is not { Length: > 0 })
                version = executing_assembly.GetName().Version?.ToString();
            else
            if (version.IndexOf('+') is > 0 and var plus_index)
                version = version[..plus_index];

            if (EnvVersionVarName is { Length: > 0 })
                Environment.SetEnvironmentVariable(EnvVersionVarName, version);

            return Version.Parse(version ?? "0.0.0");
        }
    }
}
