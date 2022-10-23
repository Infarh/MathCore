using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Windows.Media;

using Microsoft.Extensions.Primitives;

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
        System.Windows.Controls.Image image = img;

        var http = new HttpClient();

        await using var ms = await http.GetStreamAsync(url, Cancel)
           .ConfigureAwait(false);

        var iamgeConverter = new ImageSourceConverter();
        image.Source = (ImageSource)iamgeConverter.ConvertFrom(ms);
    }

    private System.Windows.Controls.Image img = null;


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
