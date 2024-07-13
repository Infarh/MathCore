namespace MathCore.Net.Http.Html;

public class DataList(params DataListItem[] items) : TypedElement("dd", [.. items]);