using System.Diagnostics.CodeAnalysis;

// ReSharper disable StringLiteralTypo

namespace MathCore.Tests.Extensions;

[TestClass, SuppressMessage("ReSharper", "InconsistentNaming")]
public class IDictionaryExtensionsTests
{
    [TestMethod]
    public void AddValue_Test()
    {
        var dictionary = new Dictionary<string, IList<string>>();
        dictionary.AddValue("t1", "0");
        Assert.AreEqual(1, dictionary.Count);
        Assert.IsTrue(dictionary.ContainsKey("t1"));
        Assert.AreEqual("0", dictionary["t1"][0]);

        dictionary.AddValue("t1", "5");
        Assert.AreEqual(1, dictionary.Count);
        Assert.AreEqual("5", dictionary["t1"][1]);

        dictionary.AddValue("q3", "6");
        Assert.AreEqual(2, dictionary.Count);
        Assert.IsTrue(dictionary.ContainsKey("q3"));
        Assert.AreEqual("6", dictionary["q3"][0]);
    }

    [TestMethod]
    public void AddValue_KeySelector_Test()
    {
        var dictionary = new Dictionary<string, IList<string>>();

        dictionary.AddValue(5, i => i.ToString(), i => new('a', i));
        Assert.AreEqual(1, dictionary.Count);
        Assert.IsTrue(dictionary.ContainsKey("5"));
        Assert.AreEqual("aaaaa", dictionary["5"][0]);

        dictionary.AddValue(7, _ => "5", i => new('a', i));
        Assert.AreEqual(1, dictionary.Count);
        Assert.AreEqual("aaaaaaa", dictionary["5"][1]);

        dictionary.AddValue(7, i => i.ToString(), i => new('a', i));
        Assert.AreEqual(2, dictionary.Count);
        Assert.IsTrue(dictionary.ContainsKey("7"));
        Assert.AreEqual("aaaaaaa", dictionary["7"][0]);
    }

    [TestMethod]
    public void AddValue_KeyValueSelector_Test()
    {
        var dictionary = new Dictionary<int, IList<string>>();

        dictionary.AddValue("aa", s => s.Length);
        dictionary.AddValue("bb", s => s.Length);
        dictionary.AddValue("abc", s => s.Length);
        dictionary.AddValue("xyz", s => s.Length);

        Assert.AreEqual(2, dictionary.Count);

        Assert.IsTrue(dictionary.ContainsKey(2));
        Assert.AreEqual("aa", dictionary[2][0]);
        Assert.AreEqual("bb", dictionary[2][1]);

        Assert.IsTrue(dictionary.ContainsKey(3));
        Assert.AreEqual("abc", dictionary[3][0]);
        Assert.AreEqual("xyz", dictionary[3][1]);
    }

    [TestMethod]
    public void AddValues_Test()
    {
        var dictionary = new Dictionary<int, string>();

        dictionary.AddValues(new[] { "a", "bb", "ccc", "dddd" }, s => s.Length);

        Assert.AreEqual(4, dictionary.Count);

        Assert.IsTrue(dictionary.ContainsKey(1));
        Assert.AreEqual("a", dictionary[1]);
        Assert.IsTrue(dictionary.ContainsKey(2));
        Assert.AreEqual("bb", dictionary[2]);
        Assert.IsTrue(dictionary.ContainsKey(3));
        Assert.AreEqual("ccc", dictionary[3]);
        Assert.IsTrue(dictionary.ContainsKey(4));
        Assert.AreEqual("dddd", dictionary[4]);
    }

    [TestMethod]
    public void GetValueOrAddNew_KeySelector_Test()
    {
        var dictionary = new Dictionary<int, string>();
        Assert.IsFalse(dictionary.ContainsKey(5));
        Assert.AreEqual(0, dictionary.Count);
        Assert.AreEqual("5", dictionary.GetValueOrAddNew(5, i => i.ToString()));
        Assert.IsTrue(dictionary.ContainsKey(5));
        Assert.AreEqual(1, dictionary.Count);
        dictionary.Add(7, "qwe");
        Assert.AreEqual("qwe", dictionary.GetValueOrAddNew(7, _ => "qwe"));
        Assert.AreEqual(2, dictionary.Count);
    }

    [TestMethod]
    public void GetValueOrAddNew_KeySelector_Interface_Test()
    {
        IDictionary<int, string> dictionary = new Dictionary<int, string>();
        Assert.IsFalse(dictionary.ContainsKey(5));
        Assert.AreEqual(0, dictionary.Count);
        Assert.AreEqual("5", dictionary.GetValueOrAddNew(5, i => i.ToString()));
        Assert.IsTrue(dictionary.ContainsKey(5));
        Assert.AreEqual(1, dictionary.Count);
        dictionary.Add(7, "qwe");
        Assert.AreEqual("qwe", dictionary.GetValueOrAddNew(7, _ => "qwe"));
        Assert.AreEqual(2, dictionary.Count);
    }

    [TestMethod]
    public void GetValueOrAddNew_Test()
    {
        var dictionary = new Dictionary<int, string>();
        Assert.AreEqual("test", dictionary.GetValueOrAddNew(5, () => "test"));
        Assert.AreEqual(1, dictionary.Count);
        Assert.IsTrue(dictionary.ContainsKey(5));
        Assert.AreEqual("test", dictionary[5]);
    }

    [TestMethod]
    public void GetValueOrAddNew_Interface_Test()
    {
        IDictionary<int, string> dictionary = new Dictionary<int, string>();
        Assert.AreEqual("test", dictionary.GetValueOrAddNew(5, () => "test"));
        Assert.AreEqual(1, dictionary.Count);
        Assert.IsTrue(dictionary.ContainsKey(5));
        Assert.AreEqual("test", dictionary[5]);
    }

    [TestMethod]
    public void GetValue_Test()
    {
        var dictionary = new Dictionary<int, string> { { 7, "value7" } };
        Assert.AreEqual("value7", dictionary.GetValue(7, "123"));
        Assert.AreEqual("123", dictionary.GetValue(0, "123"));
    }

    [TestMethod]
    public void GetValue_Interface_Test()
    {
        IDictionary<int, string> dictionary = new Dictionary<int, string> { { 7, "value7" } };
        Assert.AreEqual("value7", dictionary.GetValue(7, "123"));
        Assert.AreEqual("123", dictionary.GetValue(0, "123"));
    }

    [TestMethod]
    public void GetValue_object_Test()
    {
        var dictionary = new Dictionary<string, object>
        {
            { "a", 5 },
            { "b", new Complex(3, 7) }
        };
        Assert.AreEqual(5, dictionary.GetValue<int>("a"));
        Assert.AreEqual(Complex.Mod(3, 7), dictionary.GetValue<Complex>("b"));
        Assert.AreEqual(null, dictionary.GetValue<List<int>>("list"));
    }

    [TestMethod]
    public void Initialize_Test()
    {
        var j = 0;
        var dictionary = new Dictionary<int, string>()
           .Initialize(5, () => new(j, j++.ToString()));
        for (var i = 0; i < 5; i++)
        {
            Assert.IsTrue(dictionary.ContainsKey(i));
            Assert.AreEqual(i.ToString(), dictionary[i]);
        }
    }

    [TestMethod]
    public void Initialize_Index_Test()
    {
        var dictionary = new Dictionary<int, string>()
           .Initialize(5, i => new(i, i.ToString()));
        for (var i = 0; i < 5; i++)
        {
            Assert.IsTrue(dictionary.ContainsKey(i));
            Assert.AreEqual(i.ToString(), dictionary[i]);
        }
    }

    [TestMethod]
    public void Initialize_Keys_Test()
    {
        var dictionary = new Dictionary<int, string>()
           .Initialize(new[] { 0, 1, 2, 3, 4 }, i => i.ToString());
        for (var i = 0; i < 5; i++)
        {
            Assert.IsTrue(dictionary.ContainsKey(i));
            Assert.AreEqual(i.ToString(), dictionary[i]);
        }
    }

    [TestMethod]
    public void RemoveWhere_Test()
    {
        var dictionary = new Dictionary<int, string>()
           .Initialize(10, i => new(i, i.ToString()));
        dictionary.RemoveWhere(kv => kv.Key % 2 != 0);
        Assert.IsFalse(dictionary.Any(kv => kv.Key % 2 != 0));
    }

    [TestMethod]
    public void ToPatternString_Test()
    {
        const string pattern_string = "{var1:000}-{var2:f3}[{date:yy-MM-ddTHH-mm-ss}].{str}";
        const int    var1           = 10;
        const double var2           = Math.PI;
        var          date           = DateTime.Now;
        const string str            = "Hello!";

        var expected_string = $"{var1:000}-{var2:f3}[{date:yy-MM-ddTHH-mm-ss}].{str}";

        var dict = new Dictionary<string, object>
        {
            { "var1", var1 },
            { "var2", var2 },
            { "date", date },
            { "str", str }
        };

        var actual_string = dict.ToPatternString(pattern_string);

        Assert.That.Value(actual_string).IsEqual(expected_string);
    }
}