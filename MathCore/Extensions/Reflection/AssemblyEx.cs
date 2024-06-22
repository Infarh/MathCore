#nullable enable
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
}
