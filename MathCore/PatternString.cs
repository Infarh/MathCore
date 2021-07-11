using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MathCore
{
    /// <summary>Строковый процессор, формирующий строку на основе шаблона</summary>
    public class PatternString : IEnumerable<KeyValuePair<string, object>>
    {
        /// <summary>Регулярное выражение поиска составных частей шаблона</summary>
        private static readonly Regex __Regex = new(@"\{(?<name>\w+)(?::(?<format>[^}]+))?\}", RegexOptions.Compiled);

        private readonly string _Pattern;
        private readonly Dictionary<string, object> _Tokens = new();
        private string _Regex;

        public IFormatProvider FormatProvider { get; init; } = CultureInfo.CurrentCulture;

        public string Regex
        {
            get => _Regex ?? __Regex.ToString();
            set
            {
                if (value is null)
                {
                    _Regex = null;
                    return;
                }

                if (!value.Contains("(?<name>"))
                    throw new ArgumentException("Строка регулярного выражения должна содержать группу (?:<name>...)");
                _Regex = value;
            }
        }

        public Func<object> this[string Token]
        {
            get => !_Tokens.TryGetValue(Token, out var value) ? null : value is Func<object> f ? f : value is Func<string, object> nf ? () => nf(Token) : () => value;
            set => _Tokens[Token] = value;
        }

        public PatternString(string Pattern) => _Pattern = Pattern ?? throw new ArgumentNullException(nameof(Pattern));

        public void Add(string Token, object value) => _Tokens[Token] = value;
        public void Add(string Token, Func<object> Selector) => _Tokens[Token] = Selector;
        public void Add(string Token, Func<string, object> Selector) => _Tokens[Token] = Selector;

        public void Remove(string Token) => _Tokens.Remove(Token);

        public bool Conteins(string Token) => _Tokens.ContainsKey(Token);

        public override string ToString()
        {
            if (_Pattern is not { Length: > 0 } pattern)
                return string.Empty;

            if (_Regex is { Length: > 0 } regex)
                return System.Text.RegularExpressions.Regex.Replace(pattern, regex, PatternPartSelector);

            return __Regex.Replace(pattern, PatternPartSelector);
        }

        private string PatternPartSelector(Match Match)
        {
            var name = Match.Groups["name"].Value;
            if (!_Tokens.TryGetValue(name, out var selector)) return Match.Value;
            var value = selector switch
            {
                Func<object> f => f(),
                Func<string, object> f => f(name),
                _ => selector
            };
            return Match.Groups["format"].Success && value is IFormattable formattable_value
                ? formattable_value.ToString(Match.Groups["format"].Value, FormatProvider)
                : value as string ?? value?.ToString();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _Tokens.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Tokens).GetEnumerator();
    }
}
