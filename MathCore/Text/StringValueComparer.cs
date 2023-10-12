namespace MathCore.Text;

public class StringValueComparer : IEqualityComparer<string>
{
    /// <summary>0x3ffeffff = 1073676287</summary>
    public static int Base { get; set; } = Consts.BigPrime_int;

    public static int HashValue(string str) => HashValue(str, Base);

    public static int HashValue(string str, int @base, long hash0 = Consts.BigPrime_long)
    {
        if (str.Length == 0) return -1;

        foreach (var c in str)
            hash0 = unchecked((hash0 * Consts.BigPrime_long) ^ c ^ @base);

        return (int)(hash0 >> 32 ^ hash0 ^ @base);
    }

    public int? HashBase { get; set; }

    public StringValueComparer() { }

    public StringValueComparer(int HashBase) => this.HashBase = HashBase;

    public bool Equals(string s1, string s2) => string.Equals(s1, s2);

    public int GetHashCode(string str) => HashValue(str, HashBase ?? Base);
}
