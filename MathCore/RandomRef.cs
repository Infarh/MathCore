namespace MathCore;

public struct RandomRef(int seed)
{
    private const uint __StateOffset = Consts.BigPrime_int;
    private const ulong __StateFactor = Consts.BigPrime_long;

    private ulong _State = (ulong)seed;

    private ulong Core() => _State = unchecked((~_State ^ (_State * __StateFactor)) + __StateOffset);

    public int Next() => Math.Abs((int)Core());

    public int Next(int Max) => Next() % Max;

    public int Next(int Min, int Max) => Min + Next(Max - Min);

    public double NextDouble() => (double)Core() / uint.MaxValue;
}
