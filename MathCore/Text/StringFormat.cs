using System.Text;
using System.Text.RegularExpressions;

namespace MathCore.Text;


#if NET8_0_OR_GREATER
public partial class StringFormat(string Format, RegexOptions Opts = default)
{
    [GeneratedRegex(@"\{(?<Name>[^}]+)\}", RegexOptions.Compiled)]
    private static partial Regex GetValueInfoRegex();

    private static readonly Regex __ValueInfoRegex = GetValueInfoRegex();
#else
public class StringFormat(string Format, RegexOptions Opts = default)
{
    private static readonly Regex __ValueInfoRegex = new(@"\{(?<Name>[^}]+)\}", RegexOptions.Compiled);
#endif

    private readonly Regex _Regex = GetRegex(Format, Opts);

    private static Regex GetRegex(string format, RegexOptions opts)
    {
        var pattern = new StringBuilder();

        var last_pos = 0;
        foreach (Match match in __ValueInfoRegex.Matches(format))
        {
            pattern.Append(format[last_pos..match.Index]);
            last_pos = match.Index + match.Length;

            var field_name = match.Groups["Name"].Value
#if NET8_0_OR_GREATER
                .AsSpan();
#else
                .AsStringPtr();
#endif

            var is_required = field_name[^1] == '?';
            if (is_required)
                field_name = field_name.TrimEnd('?');

            pattern.Append($"(?<{field_name}>.{(is_required ? '*' : '+')}?)");
        }

        pattern.Append(format[last_pos..]);

        return new(pattern.ToString(), opts);
    }

    public Dictionary<string, string> Parse(string str)
    {
        if (_Regex.Match(str) is not { Success: true, Groups: var values })
            return [];

#if NET8_0_OR_GREATER
        return values
            .Cast<Group>()
            .Skip(1)
            .Distinct(v => v.Name)
            .ToDictionary(v => v.Name, v => v.Value);
#else
        var result = new Dictionary<string, string>();
        foreach (var group_name in _Regex.GetGroupNames())
            if (!result.ContainsKey(group_name))
                result.Add(group_name, values[group_name].Value);

        return result;
#endif
    }
}
