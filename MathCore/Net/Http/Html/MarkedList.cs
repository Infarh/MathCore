using System.Collections;
using System.Text;

namespace MathCore.Net.Http.Html;

public class MarkedList : TypedElement
{
    private IEnumerable Items { get; set; }

    public MarkedList(params ListItem[] items) : base("ul", [.. items]) { }

    public MarkedList(IEnumerable items) : base("ul") => Items = items;

    public void Add(IEnumerable items) => Items = Items?.Concat(items) ?? items;

    public override string ToString(int level)
    {
        var spacer = GetSpacer(level);
        var result = new StringBuilder()
            .Append(spacer)
            .Append('<')
            .Append(Name);

        foreach (var attr in Attributes)
            result.Append(' ').Append(attr);

        result.AppendLine(">");

        var inner_html = InnerHtml(level);
        if (!string.IsNullOrEmpty(inner_html))
            result.AppendLine(inner_html);

        //var items = Items;
        if (Items is { } items)
            foreach (var item in items)
            {
                if (item is null) continue;
                var inner_text = item.ToString();
                var spacer2 = GetSpacer(level + 1);


                if (inner_text!.Contains('\n'))
                {
                    result.Append(spacer2).AppendLine("<li>");
                    foreach (var line in inner_text.EnumLines())
                        result.Append(spacer2).Append(spacer).AppendLine(line);
                    result.Append(spacer2).AppendLine("</li>");
                }
                else
                    result.Append(spacer2).Append("<li>").Append(inner_text).AppendLine("</li>");
            }

        result.Append(spacer).Append("</").Append(Name).Append('>');
        return result.ToString();
    }
}