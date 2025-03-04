/// <summary>
/// The exception that is thrown when a parsing operation fails.
/// </summary>
public class ParseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ParseException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ParseException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ParseException class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ParseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}