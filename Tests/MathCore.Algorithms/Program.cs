using System.Collections;

using MathCore.Algorithms.Collections;
using MathCore.Text;

var comparer = new FixedStringComparer
{
    { "111", 3518592 },
    { "222", 3618594 },
    { "333", 3718589 },
    { "444", 3818591 },
    { "555", 3918594 },
    { "666", 4018594 },
    { "777", 4118594 },
};

var dict = new CustomDictionary<string, int>(7, comparer);

dict.Add("111", 111);
dict.Add("222", 222);
dict.Add("333", 333);
dict.Add("444", 444);
dict.Add("555", 555);
dict.Add("666", 666);
dict.Add("777", 777);


//var v222 = dict["222"];
//var v444 = dict["444"];
//var v777 = dict["777"];
//var v123 = dict["123"];
//var v321 = dict["321"];
//var v111 = dict["111"];

Console.WriteLine("End.");
//Console.ReadLine();

class FixedStringComparer(Dictionary<string, int> Codes) : IEqualityComparer<string>, IEnumerable<KeyValuePair<string, int>>
{
    public FixedStringComparer() : this(new()) { }

    public bool Equals(string x, string y) => string.Equals(x, y);

    public int GetHashCode(string obj) => Codes.GetValueOrDefault(obj, obj.GetHashCode());

    public void Add(string str, int hash) => Codes[str] = hash;

    public IEnumerator<KeyValuePair<string, int>> GetEnumerator() => Codes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}