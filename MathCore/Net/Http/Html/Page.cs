using System.Text;

namespace MathCore.Net.Http.Html;

public class Page
{
    private Head _Head = new() { AlwaysOpen = true };

    private Body _Body = new() { AlwaysOpen = true };

    public Head Head { get => _Head; set => _Head = value ?? []; }

    public Body Body { get => _Body; set => _Body = value ?? []; }

    public string Title
    {
        get => _Head.Elements.OfType<HElement>().FirstOrDefault(e => e.Name == "title")?.InnerText();
        set
        {
            if (_Head.Elements.OfType<Title>().FirstOrDefault() is { } title)
            {
                var title_elements = title.Elements;
                title_elements.Clear();
                title_elements.Add(new Text(value));
            }

            _Head.Elements.Add(new Title(new Text(value)));
        }
    }

    public override string ToString() => new StringBuilder()
        .AppendLine("<!DOCTYPE html>")
        .Append(new HElement("html", _Head, _Body))
        .ToString();
}