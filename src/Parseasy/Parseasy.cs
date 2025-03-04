using System.Linq;
using System.Threading;

namespace Parseasy
{
    /// <summary>
    /// Provides methods for parsing text based on patterns with typed placeholders.
    /// </summary>
    public static class Parser
    {
        private static readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<string, ParsePattern> _patternCache = new Dictionary<string, ParsePattern>();

        /// <summary>
        /// Attempts to parse the input string according to the specified pattern.
        /// </summary>
        /// <param name="input">The input string to parse.</param>
        /// <param name="pattern">The pattern with typed placeholders (e.g., {name:string}, {age:int}).</param>
        /// <param name="result">When this method returns, contains the parsed values if parsing succeeded, or null if parsing failed.</param>
        /// <returns>true if parsing succeeded; otherwise, false.</returns>
        /// <example>
        /// <code>
        /// string input = "Name: John, Age: 30";
        /// string pattern = "Name: {name:string}, Age: {age:int}";
        /// 
        /// if (Parser.TryParse(input, pattern, out var result))
        /// {
        ///     string name = result.GetValue&lt;string&gt;("name");
        ///     int age = result.GetValue&lt;int&gt;("age");
        ///     Console.WriteLine($"Name: {name}, Age: {age}");
        /// }
        /// </code>
        /// </example>
        public static bool TryParse(string input, string pattern, out ParseResult? result)
        {
            if (string.IsNullOrEmpty(input))
            {
                result = null;
                return false;
            }

            var parsePattern = GetOrCreatePattern(pattern);
            return parsePattern.TryMatch(input, out result);
        }

        /// <summary>
        /// Parses the input string according to the specified pattern.
        /// </summary>
        /// <param name="input">The input string to parse.</param>
        /// <param name="pattern">The pattern with typed placeholders (e.g., {name:string}, {age:int}).</param>
        /// <returns>A ParseResult containing the extracted values.</returns>
        /// <exception cref="ParseException">Thrown when the input does not match the pattern.</exception>
        /// <example>
        /// <code>
        /// string input = "Name: John, Age: 30";
        /// string pattern = "Name: {name:string}, Age: {age:int}";
        /// 
        /// try
        /// {
        ///     var result = Parser.Parse(input, pattern);
        ///     string name = result.GetValue&lt;string&gt;("name");
        ///     int age = result.GetValue&lt;int&gt;("age");
        ///     Console.WriteLine($"Name: {name}, Age: {age}");
        /// }
        /// catch (ParseException ex)
        /// {
        ///     Console.WriteLine($"Parsing failed: {ex.Message}");
        /// }
        /// </code>
        /// </example>
        public static ParseResult Parse(string input, string pattern)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input cannot be null or empty.", nameof(input));
            }

            var parsePattern = GetOrCreatePattern(pattern);
            if (!parsePattern.TryMatch(input, out var result) || result == null)
            {
                throw new ParseException($"Input does not match the pattern: {pattern}");
            }

            return result;
        }

        /// <summary>
        /// Determines whether the input string matches the specified pattern.
        /// </summary>
        /// <param name="input">The input string to check.</param>
        /// <param name="pattern">The pattern with typed placeholders.</param>
        /// <returns>true if the input matches the pattern; otherwise, false.</returns>
        /// <example>
        /// <code>
        /// string input = "Name: John, Age: 30";
        /// string pattern = "Name: {name:string}, Age: {age:int}";
        /// 
        /// if (Parser.IsMatch(input, pattern))
        /// {
        ///     Console.WriteLine("Input matches the pattern.");
        /// }
        /// </code>
        /// </example>
        public static bool IsMatch(string input, string pattern)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var parsePattern = GetOrCreatePattern(pattern);
            return parsePattern.IsMatch(input);
        }

        /// <summary>
        /// Finds all matches of the pattern in the input string.
        /// </summary>
        /// <param name="input">The input string to search.</param>
        /// <param name="pattern">The pattern with typed placeholders.</param>
        /// <returns>A collection of ParseResult objects for each match found.</returns>
        /// <example>
        /// <code>
        /// string input = "Name: John, Age: 30\nName: Jane, Age: 25";
        /// string pattern = "Name: {name:string}, Age: {age:int}";
        /// 
        /// var matches = Parser.FindAllMatches(input, pattern);
        /// foreach (var match in matches)
        /// {
        ///     string name = match.GetValue&lt;string&gt;("name");
        ///     int age = match.GetValue&lt;int&gt;("age");
        ///     Console.WriteLine($"Name: {name}, Age: {age}");
        /// }
        /// </code>
        /// </example>
        public static IEnumerable<ParseResult> FindAllMatches(string input, string pattern)
        {
            if (string.IsNullOrEmpty(input))
            {
                return Enumerable.Empty<ParseResult>();
            }

            var parsePattern = GetOrCreatePattern(pattern);
            return parsePattern.FindAllMatches(input);
        }

        private static ParsePattern GetOrCreatePattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentException("Pattern cannot be null or empty.", nameof(pattern));
            }

            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                if (_patternCache.TryGetValue(pattern, out var cachedPattern))
                {
                    return cachedPattern;
                }

                _cacheLock.EnterWriteLock();
                try
                {
                    var newPattern = new ParsePattern(pattern);
                    _patternCache[pattern] = newPattern;
                    return newPattern;
                }
                finally
                {
                    _cacheLock.ExitWriteLock();
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
        }
    }
}