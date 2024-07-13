using System.Text;

namespace MathCore.Net.Http.Html;

public class Text(string text) : HElementBase
{
    public string Value { get; set; } = text;

    public override string InnerText() => Value;

    public override string ToString(int level)
    {
        var spacer = GetSpacer(level);
        if (!Value.Contains('\n'))
            return new StringBuilder(spacer.Length + Value.Length)
                .Append(spacer)
                .Append(Value)
                .ToString();

        var result = new StringBuilder();

        foreach (var line in Value.EnumLines())
            result.Append(spacer).AppendLine(line);

        return result.ToString();
    }
}