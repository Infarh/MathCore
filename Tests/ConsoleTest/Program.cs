using System.Runtime.CompilerServices;
using System.Text;

using static ValueSelector;

ValueSelector? v = new();
v.Add($"qwe:{123}{"qwe"} asd:{true,5:qwe}");

StringBuilder s = new();
s.AppendLine($"qwe:{123}");


Console.WriteLine("End.");

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