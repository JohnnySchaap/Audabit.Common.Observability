namespace Audabit.Common.Observability.Events;

/// <summary>
/// Represents a structured event for observability logging.
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Gets the unique name of the event.
    /// </summary>
    string EventName { get; }

    /// <summary>
    /// Builds an immutable dictionary of event properties for structured logging.
    /// </summary>
    /// <returns>A read-only dictionary of event properties.</returns>
    IReadOnlyDictionary<string, object> BuildProperties();
}