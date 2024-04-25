using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ConsoleTest;

public static class ScriptingTest
{
    public static async Task RunAsync(CancellationToken Cancel = default)
    {
        var opts = ScriptOptions.Default
            //.WithReferences(AppDomain.CurrentDomain.GetAssemblies())
            .WithImports("System");

        var src = "Console.WriteLine(\"Hello from C# script\")";
        var script = CSharpScript.Create(src, opts);

        var result = await script.RunAsync(cancellationToken: Cancel);
    }
}
