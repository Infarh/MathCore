namespace MathCore.Net.Http.Html;

public class DataListItem(HElementBase dd, HElementBase dt) : TypedElement("dd")
{
    public HElementBase DD { get; set; } = dd;
    public HElementBase DT { get; set; } = dt;

    /// <inheritdoc />
    public override string ToString(int level) => ToString(GetSpacer(level), level);

    private string ToString(string spacer, int level) =>
        $"""
         {spacer}<dd>
         {DD.ToString(level + 1)}
         {spacer}</dd>
         {spacer}<dt>
         {DT.ToString(level + 1)}
         {spacer}</dt>
         """;
}