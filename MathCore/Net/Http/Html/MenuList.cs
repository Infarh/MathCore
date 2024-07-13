namespace MathCore.Net.Http.Html;

public class MenuList(params ListItem[] items) : TypedElement("menu", items.Cast<HElementBase>().ToArray());