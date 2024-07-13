using System.Text;

namespace MathCore.Net.Http.Html;

public abstract class HElementBase
{
    public static string SpacerPattern { get; set; } = "    ";

    protected static string GetSpacer(int level)
    {
        if (level <= 0)
            return string.Empty;

        var pattern = SpacerPattern;
        var result = new StringBuilder(pattern.Length * level);

        for (var i = 0; i < level; i++)
            result.Append(pattern);

        return result.ToString();
    }

    public abstract string InnerText();

    public abstract string ToString(int level);

    /// <inheritdoc />
    public override string ToString() => ToString(0);


    public static implicit operator HElementBase(string text) => new Text(text);
}