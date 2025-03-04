using System.Text.RegularExpressions;
using System.Text;

/// <summary>
/// Represents a compiled pattern for parsing.
/// </summary>
internal class ParsePattern
{
    private readonly string _pattern;
    private readonly Regex _regex;
    private readonly List<PlaceholderInfo> _placeholders;

    public ParsePattern(string pattern)
    {
        _pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        _placeholders = new List<PlaceholderInfo>();

        var regexPattern = BuildRegexPattern(pattern, _placeholders);
        _regex = new Regex(regexPattern, RegexOptions.Compiled);
    }

    public bool TryMatch(string input, out ParseResult? result)
    {
        var match = _regex.Match(input);
        if (!match.Success)
        {
            result = null;
            return false;
        }

        result = CreateParseResult(match);
        return true;
    }

    public bool IsMatch(string input)
    {
        return _regex.IsMatch(input);
    }

    public IEnumerable<ParseResult> FindAllMatches(string input)
    {
        var matches = _regex.Matches(input);
        var results = new List<ParseResult>();

        foreach (Match match in matches)
        {
            if (match.Success)
            {
                results.Add(CreateParseResult(match));
            }
        }

        return results;
    }

    private ParseResult CreateParseResult(Match match)
    {
        var result = new ParseResult();

        foreach (var placeholder in _placeholders)
        {
            var group = match.Groups[placeholder.Name];
            if (group.Success)
            {
                var value = ConvertValue(group.Value, placeholder.Type);
                result.AddValue(placeholder.Name, value);
            }
        }

        return result;
    }

    private static string BuildRegexPattern(string pattern, List<PlaceholderInfo> placeholders)
    {
        var regexBuilder = new StringBuilder();
        var currentIndex = 0;

        var placeholderRegex = new Regex(@"\{([a-zA-Z0-9_]+):([a-zA-Z0-9_]+)\}");
        var matches = placeholderRegex.Matches(pattern);

        foreach (Match match in matches)
        {
            // Add the text before the placeholder
            var textBefore = pattern.Substring(currentIndex, match.Index - currentIndex);
            regexBuilder.Append(Regex.Escape(textBefore));

            // Extract placeholder name and type
            var name = match.Groups[1].Value;
            var type = match.Groups[2].Value;

            // Add the placeholder to the list
            placeholders.Add(new PlaceholderInfo(name, type));

            // Add the regex pattern for the placeholder
            var placeholderPattern = GetRegexForType(type);
            regexBuilder.Append($"(?<{name}>{placeholderPattern})");

            // Update the current index
            currentIndex = match.Index + match.Length;
        }

        // Add any remaining text after the last placeholder
        if (currentIndex < pattern.Length)
        {
            var textAfter = pattern.Substring(currentIndex);
            regexBuilder.Append(Regex.Escape(textAfter));
        }

        // For FindAllMatches, we need to modify the pattern to not require the entire string to match
        if (placeholders.Count > 0)
        {
            return regexBuilder.ToString();
        }
        else
        {
            // If there are no placeholders, just use the escaped pattern
            return $"^{Regex.Escape(pattern)}$";
        }
    }

    private static string GetRegexForType(string type)
    {
        return type.ToLowerInvariant() switch
        {
            "string" => @"[^\r\n]*.?",
            "int" => @"-?\d+",
            "decimal" or "double" or "float" => @"[\s]*-?\d+(\.\d+)?",
            "bool" => @"true|false|True|False",
            "date" or "datetime" => @"\d{4}-\d{2}-\d{2}([ T]\d{2}:\d{2}(:\d{2})?)?",
            "guid" => @"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}",
            "email" => @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}",
            "url" => @"https?://[^\s/$.?#].[^\s]*",
            "phone" => @"\+?\d{1,4}?[-.\s]?\(?\d{1,3}?\)?[-.\s]?\d{1,4}[-.\s]?\d{1,4}[-.\s]?\d{1,9}",
            "ip" => @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}",
            _ => @".*?"
        };
    }

    private static object? ConvertValue(string value, string type)
    {
        try
        {
            return type.ToLowerInvariant() switch
            {
                "string" => value,
                "int" => int.Parse(value),
                "decimal" => decimal.Parse(value),
                "double" => double.Parse(value),
                "float" => float.Parse(value),
                "bool" => bool.Parse(value),
                "date" or "datetime" => DateTime.Parse(value),
                "guid" => Guid.Parse(value),
                _ => value
            };
        }
        catch (Exception ex)
        {
            throw new ParseException($"Failed to convert value '{value}' to type '{type}'.", ex);
        }
    }
}

