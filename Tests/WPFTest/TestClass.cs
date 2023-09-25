using System.IO;
using System.Net.Http;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable AccessToModifiedClosure
// ReSharper disable ConvertToUsingDeclaration
// ReSharper disable InconsistentNaming
// ReSharper disable InvertIf

namespace WPFTest;

public class TestClass
{
    private async Task UpdateImageAsync(string url, CancellationToken Cancel = default)
    {
        var image = img;

        var http = new HttpClient();

        await using var ms = await http.GetStreamAsync(url, Cancel)
           .ConfigureAwait(false);

        var iamge_converter = new ImageSourceConverter();
        image.Source = (ImageSource)iamge_converter.ConvertFrom(ms);
    }

    private Image img = null;


    private (Action<string> adder, Action clearer) GetApplier(List<string> Strings)
    {
        var set = new HashSet<string>();
        Action<string> applier = s =>
        {
            if (s is null || s.Length == 0) return;
            if (!set.Add(s)) return;

            Strings.Add(s);
        };

        Action clearer = () => set.Clear();

        return (applier, clearer);
    }

    private List<string> GetAllNotNullStrings(IEnumerable<string> ss) =>
        ss.Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
}
