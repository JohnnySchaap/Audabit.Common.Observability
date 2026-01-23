using Audabit.Common.Observability.Events;
using Microsoft.Extensions.Logging;

namespace Audabit.Common.Observability.Emitters;

/// <summary>
/// Default implementation of <see cref="IEmitter{T}"/> for emitting structured logging events.
/// </summary>
/// <typeparam name="T">The category type for logger classification.</typeparam>
public class Emitter<T>(ILogger<T> logger) : IEmitter<T>
{
    /// <summary>
    /// Raises an event with the specified log level and optional additional properties.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to raise.</typeparam>
    /// <param name="data">The event data to log.</param>
    /// <param name="logLevel">The log level for this event.</param>
    /// <param name="properties">Additional properties to include with the event.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// This method builds a structured logging event by:
    /// <list type="bullet">
    /// <item><description>Extracting properties from the event data</description></item>
    /// <item><description>Merging with any additional properties provided</description></item>
    /// <item><description>Creating an EventId from the event name</description></item>
    /// <item><description>Logging with a scoped context containing all properties</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// If the logger is not enabled for the specified log level, the method returns immediately without processing.
    /// </para>
    /// <para>
    /// Property keys are automatically converted to PascalCase for consistency in log output.
    /// </para>
    /// </remarks>
    /// <example>
    /// Emit a structured event:
    /// <code>
    /// var userEvent = new UserCreatedEvent(userId, username);
    /// emitter.Raise(userEvent, LogLevel.Information);
    /// </code>
    /// </example>
    public void Raise<TEvent>(TEvent data, LogLevel logLevel, Dictionary<string, object>? properties = null) where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(data);

        if (!logger.IsEnabled(logLevel))
        {
            return;
        }

        // Combine <data> properties with additional <properties>
        var source = data.BuildProperties();
        var eventProperties = new Dictionary<string, object>(source.Count);
        foreach (var kvp in source)
        {
            eventProperties[ToPascalCase(kvp.Key)] = kvp.Value;
        }
        if (properties != null)
        {
            foreach (var property in properties)
            {
                eventProperties[ToPascalCase(property.Key)] = property.Value;
            }
        }

        var eventId = new EventId(data.EventName.GetHashCode(), data.EventName);

        using (logger.BeginScope(eventProperties))
        {
            logger.Log(logLevel, eventId, "{EventName}", data.EventName);
        }
    }

    private static string ToPascalCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return char.ToUpperInvariant(value[0]) + value[1..];
    }
}