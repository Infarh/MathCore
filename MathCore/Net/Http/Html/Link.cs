namespace MathCore.Net.Http.Html;

public class Link : TypedElement
{
    /// <inheritdoc />
    public override bool OnlyOpen { get => true; set => throw new NotSupportedException(); }

    public string Relation
    {
        get => Attributes.FirstOrDefault(a => a.AttributeName.Equals("rel", StringComparison.InvariantCultureIgnoreCase))?.Value;
        set
        {
            if (Attributes.FirstOrDefault(a => a.AttributeName.Equals("rel", StringComparison.InvariantCultureIgnoreCase)) is { } attribute) 
                attribute.Value = value;
            else 
                Attributes.Add(new("rel", value));
        }
    }

    public string Reference
    {
        get => Attributes.FirstOrDefault(a => a.AttributeName.Equals("href", StringComparison.InvariantCultureIgnoreCase))?.Value;
        set
        {
            if (Attributes.FirstOrDefault(a => a.AttributeName.Equals("href", StringComparison.InvariantCultureIgnoreCase)) is { } attribute) 
                attribute.Value = value;
            else 
                Attributes.Add(new("href", value));
        }
    }

    public Link() : base("link") { }

    public Link(string relation, string href) : base("link")
    {
        List<HAttribute> attributes = null;
        if (!string.IsNullOrWhiteSpace(relation)) (attributes = Attributes).Add(new("rel", relation));
        if (!string.IsNullOrWhiteSpace(href)) (attributes ?? Attributes).Add(new("href", href));
    }
}