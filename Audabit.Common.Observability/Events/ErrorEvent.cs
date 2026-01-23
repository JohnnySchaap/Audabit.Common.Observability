namespace Audabit.Common.Observability.Events;

/// <summary>
/// Represents an error event with exception details and custom properties.
/// </summary>
public class ErrorEvent : LoggingEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorEvent"/> class.
    /// </summary>
    /// <param name="eventDescription">The description of the event.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="exception">The exception that caused the error, if any.</param>
    /// <param name="properties">Additional custom properties to include in the event.</param>
    /// <exception cref="ArgumentException">Thrown when eventDescription or errorMessage is null or whitespace.</exception>
    public ErrorEvent(string eventDescription, string errorMessage, Exception? exception = null, Dictionary<string, object>? properties = null)
        : base(eventDescription)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

        Properties["Error message"] = errorMessage;
        if (exception != null)
        {
            Properties["Exception type"] = exception.GetType().Name;
            Properties["Exception message"] = exception.Message;
            if (exception.StackTrace != null)
            {
                Properties["Stack"] = exception.StackTrace;
            }

            var baseException = exception.GetBaseException();
            Properties["Base.Exception type"] = baseException.GetType().Name;
            Properties["Base.Exception message"] = baseException.Message;
            if (baseException.StackTrace != null)
            {
                Properties["Base.Stack"] = baseException.StackTrace;
            }
        }

        if (properties != null)
        {
            foreach (var property in properties)
            {
                Properties[property.Key] = property.Value;
            }
        }
    }
}