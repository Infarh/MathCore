namespace MathCore;

public readonly ref partial struct StringPtr
{
    public delegate T Selector<out T>(StringPtr p);
}
