﻿using System.Text.RegularExpressions;

namespace Benchmarks;

public class CSVRegex(char Separator)
{
    private readonly Regex _Regex = new($"""(?<=(?:{Separator}|\n|^))("(?:(?:"")*[^"]*)*"|[^"{Separator}\n]*|(?:\n|$))""", RegexOptions.Compiled);

    public int Parse(string Line)
    {
        var i = 0;
        var result = 0;
        foreach (Match item in _Regex.Matches(Line))
            result += i++ switch
            {
                0 => int.Parse(item.Value),
                _ => item.Length
            };

        return result;
    }
}