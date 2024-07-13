using System.Collections;
using System.Text;

namespace MathCore.Net.Http.Html;

public class HElement : HElementBase, IEnumerable<HElementBase>, IEnumerable<HAttribute>
{
    private string _Name;
    private List<HAttribute> _Attributes;
    private List<HElementBase> _Elements;
    private bool _AlwaysOpen;
    private bool _OnlyOpen;

    public virtual string Name { get => _Name; set => _Name = value; }

    public List<HElementBase> Elements { get => _Elements ??= []; set => _Elements = value; }

    public bool HasElements => _Elements is { Count: > 0 };

    public List<HAttribute> Attributes { get => _Attributes ??= []; set => _Attributes = value; }

    public bool HasAttributes => _Attributes is { Count: > 0 };

    public virtual bool AlwaysOpen { get => _AlwaysOpen; set => _AlwaysOpen = value; }

    public virtual bool OnlyOpen { get => _OnlyOpen; set => _OnlyOpen = value; }

    public HElement(string Name, params HElementBase[] elements)
    {
        _Name = Name;
        if (elements.Length > 0) _Elements = elements.ToList();
    }

    public void Add(params HAttribute[] attribute) => Attributes.AddRange(attribute);

    public void Add(params HElementBase[] element) => Elements.AddRange(element);

    public void Add(params object[] items)
    {
        foreach (var item in items)
        {
            switch (item)
            {
                case HAttribute attribute:
                    Attributes.Add(attribute);
                    break;
                case HElement element:
                    Elements.Add(element);
                    break;
                case string str:
                    Elements.Add(new Text(str));
                    break;
                default:
                    Elements.Add(new Text(item.ToString()));
                    break;
            }
        }
    }

    public string InnerHtml() => InnerHtml(0);

    protected string InnerHtml(int level)
    {
        if (!HasElements)
            return string.Empty;

        var result = new StringBuilder();
        foreach (var element in _Elements)
            result.AppendLine(element.ToString(level + 1));

        return result.ToString();
    }

    public override string InnerText() => InnerText(0);

    protected string InnerText(int level)
    {
        if (!HasElements)
            return string.Empty;

        var spacer = GetSpacer(level + 1);

        var result = new StringBuilder();
        foreach (var element in _Elements)
            result.Append(spacer).AppendLine(element.ToString(level + 1));

        return result.ToString();
    }

    public override string ToString(int level)
    {
        var spacer = GetSpacer(level);
        var result = new StringBuilder()
            .Append(spacer)
            .Append('<').Append(_Name);

        foreach (var attr in Attributes)
            result.Append(' ').Append(attr);

        if (!HasElements)
        {
            if (OnlyOpen)
                return result.Append('>').ToString();

            if (AlwaysOpen)
                result.Append("></").Append(_Name).Append('>');
            else
                result.Append("/>");

            return result.ToString();
        }

        if (_Elements.Count == 1)
        {
            var inner_text = InnerHtml(level);
            if (!inner_text.Contains('\n'))
                return result
                    .Append('>').Append(inner_text.Trim()).Append("</").Append(_Name).Append('>')
                    .ToString();
        }

        result.AppendLine(">");
        result.AppendLine(InnerHtml(level));
        result.Append(spacer).Append("</").Append(_Name).Append('>');

        return result.ToString();
    }

    public IEnumerator<HElementBase> GetEnumerator() => _Elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Elements).GetEnumerator();

    IEnumerator<HAttribute> IEnumerable<HAttribute>.GetEnumerator() => _Attributes.GetEnumerator();
}