using Audabit.Common.Observability.Events;
using Microsoft.Extensions.Logging;

namespace Audabit.Common.Observability.Emitters;

/// <summary>
/// Emits structured logging events with a specific category type.
/// </summary>
/// <typeparam name="T">The category type for logger classification.</typeparam>
public interface IEmitter<T>
{
    /// <summary>
    /// Raises an event with the specified log level and optional additional properties.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to raise.</typeparam>
    /// <param name="data">The event data to log.</param>
    /// <param name="logLevel">The log level for this event.</param>
    /// <param name="properties">Additional properties to include with the event.</param>
    /// <exception cref="ArgumentNullException">Thrown when data is null.</exception>
    void Raise<TEvent>(TEvent data, LogLevel logLevel, Dictionary<string, object>? properties = null) where TEvent : IEvent;
}