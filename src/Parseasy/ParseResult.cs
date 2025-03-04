/// <summary>
/// Represents the result of a successful parse operation.
/// </summary>
public class ParseResult
{
    private readonly Dictionary<string, object?> _values = new Dictionary<string, object?>();

    internal void AddValue(string name, object? value)
    {
        _values[name] = value;
    }

    /// <summary>
    /// Gets the value associated with the specified name.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="name">The name of the value to get.</param>
    /// <returns>The value converted to the specified type.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the name is not found in the result.</exception>
    /// <exception cref="InvalidCastException">Thrown when the value cannot be converted to the specified type.</exception>
    public T GetValue<T>(string name)
    {
        if (!_values.TryGetValue(name, out var value))
        {
            throw new KeyNotFoundException($"The name '{name}' was not found in the parse result.");
        }

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Cannot convert value to type {typeof(T).Name}.", ex);
        }
    }

    /// <summary>
    /// Tries to get the value associated with the specified name.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="name">The name of the value to get.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified name, if found and convertible to the specified type; otherwise, the default value for the type.</param>
    /// <returns>true if the value was found and successfully converted; otherwise, false.</returns>
    public bool TryGetValue<T>(string name, out T value)
    {
        value = default!;

        if (!_values.TryGetValue(name, out var objValue))
        {
            return false;
        }

        try
        {
            value = (T)Convert.ChangeType(objValue, typeof(T));
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets all the names of the values in the result.
    /// </summary>
    /// <returns>An enumerable collection of names.</returns>
    public IEnumerable<string> GetNames()
    {
        return _values.Keys;
    }

    /// <summary>
    /// Gets the number of values in the result.
    /// </summary>
    public int Count => _values.Count;
}
