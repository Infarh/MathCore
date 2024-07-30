namespace MathCore.Net.Http.Html;

public class Script : TypedElement
{
    public override bool AlwaysOpen { get => true; set => throw new NotSupportedException(); }

    public string Source
    {
        get => Attributes.FirstOrDefault(a => a.AttributeName.Equals("src", StringComparison.InvariantCultureIgnoreCase))?.Value;
        set
        {
            if (Attributes.FirstOrDefault(a => a.AttributeName.Equals("src", StringComparison.InvariantCultureIgnoreCase)) is { } attribute) 
                attribute.Value = value;
            else 
                Attributes.Add(new("src", value));
        }
    }

    public Script() : base("script") { }

    public Script(string script) : base("script", new Text(script)) { }
}