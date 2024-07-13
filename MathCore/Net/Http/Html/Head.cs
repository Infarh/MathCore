namespace MathCore.Net.Http.Html;

public class Head : TypedElement { public Head(params HElementBase[] elements) : base("head", elements) => AlwaysOpen = true; }