namespace MathCore.Text;

public class StringValueComparer : IEqualityComparer<string>
{
    public static int Base { get; set; } = new Random().Next();

    public static int HashValue(string str)
    {
        if (str.Length == 0) return -1;

        var hash = Consts.BigPrime_long;
        foreach (var c in str)
            hash = unchecked((hash * Consts.BigPrime_long) ^ c ^ Base);

        return (int)(hash >> 32 ^ hash ^ Base);
    }

    public bool Equals(string s1, string s2) => string.Equals(s1, s2);

    public int GetHashCode(string str) => HashValue(str);
}
