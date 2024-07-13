using System.Text;

namespace MathCore.Net.Http.Html;

public class HAttribute(string Name, string Value)
{
    public string AttributeName { get => Name; set => Name = value; }

    private string _Value = Value;
    public string Value { get => _Value; set => _Value = value; }

    public override string ToString() => new StringBuilder(Name.Length + _Value.Length + 3)
        .Append(Name)
        .Append(' ').Append('"')
        .Append(_Value)
        .Append('"')
        .ToString();
}