namespace MathCore.Net.Http.Html;

public abstract class TypedElement(string Name, params HElementBase[] elements) : HElement(Name, elements)
{
    public override string Name { get => base.Name; set => throw new NotSupportedException("Изменить имя типизированного элемента нельзя"); }
}