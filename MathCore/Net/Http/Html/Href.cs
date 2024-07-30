namespace MathCore.Net.Http.Html;

public class Href : TypedElement
{
    public string Link
    {
        get => Attributes.FirstOrDefault(a => a.AttributeName == "href")?.Value;
        set
        {
            var href = Attributes.FirstOrDefault(a => a.AttributeName == "href");
            if (href is null) Attributes.Add(new("href", value)); else href.Value = value;
        }
    }
    public Href(params HElementBase[] elements) : base("a", elements) => Attributes.Add(new("href", ""));

    public Href(string link, params HElementBase[] elements) : base("a", elements) => Attributes.Add(new("href", link));
}