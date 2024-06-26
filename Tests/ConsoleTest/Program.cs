using System.Runtime.CompilerServices;

using MathCore.Statistic;

using ConsoleTest;

using MathCore.Statistic;
using MathCore.Text;

ValueSelector? v = new();
v.Add($"qwe:{123}{"qwe"} asd:{true,5:qwe}");

var s = "123;345:567";
var ps = s.AsStringPtr().Split(';', ':');

foreach (var value in ps.Select(v => v.ParseDouble()))
{

}


#pragma warning disable CS8321 // Local function is declared but never used

//await ScriptingTest.RunAsync();

var format = new StringFormat("time:{time} value:{value}");

var values = format.Parse("time:123 value:qwe");

AppDomainTest.Run();

//var asm = AssemblyBuilder.DefineDynamicAssembly(new("Test"), AssemblyBuilderAccess.RunAndCollect);
//var mod = asm.DefineDynamicModule("main");
//var tt = mod.DefineType("TestType");
//tt.DefineConstructor()

return;

class ValueSelector
{
    public void Add([InterpolatedStringHandlerArgument("")] ref ValuesHandler values) { }

    [InterpolatedStringHandler]
    public struct ValuesHandler
    {
        public ValuesHandler(int LiteralLength, int FormattedCount, ValueSelector sel)
        {
            sel = new();
            this.LiteralLength = LiteralLength;
            this.FormattedCount = FormattedCount;
        }

        public int LiteralLength { get; }
        public int FormattedCount { get; }

        public void AppendLiteral(string value) { }
        public void AppendFormatted<T>(T value) { }
        public void AppendFormatted<T>(T value, string? format) { }
        public void AppendFormatted<T>(T value, int alignment) { }
        public void AppendFormatted<T>(T value, int alignment, string? format) { }
        public void AppendFormatted(ReadOnlySpan<char> value) { }
        public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null) { }
        public void AppendFormatted(string? value) { }
        public void AppendFormatted(string? value, int alignment = 0, string? format = null) { }
        public void AppendFormatted(object? value, int alignment = 0, string? format = null) { }
    }
}