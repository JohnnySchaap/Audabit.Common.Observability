namespace Audabit.Common.Observability.Events;

/// <summary>
/// Base class for all logging events with common service metadata.
/// </summary>
/// <remarks>
/// This abstract class provides common functionality for all logging events including:
/// <list type="bullet">
/// <item><description>Service name management (set once globally)</description></item>
/// <item><description>Event name generation with service prefix</description></item>
/// <item><description>Properties dictionary for structured logging</description></item>
/// </list>
/// All derived event classes are marked with [ExcludeFromCodeCoverage] as they are data containers.
/// </remarks>
public abstract class LoggingEvent : IEvent
{
    private static string? _serviceName;

    /// <summary>
    /// Gets the service name for all events. Returns "Service" if not set.
    /// </summary>
    internal static string ServiceName => _serviceName ?? "Service";

    /// <summary>
    /// Sets the service name for all events. Can only be set once.
    /// </summary>
    /// <param name="serviceName">The name of the service.</param>
    /// <exception cref="ArgumentException">Thrown when serviceName is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when ServiceName has already been set to a different value.</exception>
    /// <remarks>
    /// This method uses thread-safe atomic operations to ensure the service name can only be set once
    /// across the entire application lifetime. This is typically called during application startup.
    /// If the service name is already set to the same value, this method does nothing (idempotent).
    /// </remarks>
    public static void SetServiceName(string serviceName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        var existingValue = Interlocked.CompareExchange(ref _serviceName, serviceName, null);

        // If service name was already set, verify it matches
        if (existingValue != null && existingValue != serviceName)
        {
            throw new InvalidOperationException($"ServiceName has already been set to '{existingValue}' and cannot be changed to '{serviceName}'.");
        }
    }

    /// <summary>
    /// Gets the dictionary of properties for this event. Derived classes can add additional properties.
    /// </summary>
    protected Dictionary<string, object> Properties { get; }

    /// <summary>
    /// Gets the unique name of the event in the format "{ServiceName}.{EventDescription}".
    /// </summary>
    public string EventName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingEvent"/> class.
    /// </summary>
    /// <param name="eventDescription">The description of the event (e.g., "WeatherForecastGenerating").</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="eventDescription"/> is null or whitespace.</exception>
    /// <remarks>
    /// The constructor automatically adds "Service name" and "Event description" to the Properties dictionary
    /// and generates the EventName in the format "{ServiceName}.{eventDescription}".
    /// </remarks>
    protected LoggingEvent(string eventDescription)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventDescription);

        Properties = new Dictionary<string, object>
        {
            { "Service name", ServiceName },
            { "Event description", eventDescription }
        };

        EventName = $"{ServiceName}.{eventDescription}";
    }

    /// <summary>
    /// Builds an immutable dictionary of event properties for structured logging.
    /// </summary>
    /// <returns>A read-only dictionary of event properties.</returns>
    /// <remarks>
    /// This method returns the Properties dictionary as a read-only collection.
    /// Derived classes can override this method to add computed or transformed properties.
    /// </remarks>
    public virtual IReadOnlyDictionary<string, object> BuildProperties()
    {
        return Properties;
    }
}