using Audabit.Common.Observability.Emitters;
using Audabit.Common.Observability.Events;
using Microsoft.Extensions.Logging;

namespace Audabit.Common.Observability.Extensions;

/// <summary>
/// Provides convenience extension methods for emitting events at specific log levels.
/// </summary>
public static class EmitterExtensions
{
    /// <summary>
    /// Raises a debug-level event.
    /// </summary>
    /// <typeparam name="TCategory">The category type for logger classification.</typeparam>
    /// <typeparam name="T">The type of event to raise.</typeparam>
    /// <param name="emitter">The emitter instance.</param>
    /// <param name="data">The event data to log.</param>
    /// <param name="properties">Additional properties to include with the event.</param>
    /// <exception cref="ArgumentNullException">Thrown when emitter or data is null.</exception>
    public static void RaiseDebug<TCategory, T>(this IEmitter<TCategory> emitter, T data, Dictionary<string, object>? properties = null) where T : IEvent
    {
        ArgumentNullException.ThrowIfNull(emitter);
        ArgumentNullException.ThrowIfNull(data);
        emitter.Raise(data, LogLevel.Debug, properties);
    }

    /// <summary>
    /// Raises an information-level event.
    /// </summary>
    /// <typeparam name="TCategory">The category type for logger classification.</typeparam>
    /// <typeparam name="T">The type of event to raise.</typeparam>
    /// <param name="emitter">The emitter instance.</param>
    /// <param name="data">The event data to log.</param>
    /// <param name="properties">Additional properties to include with the event.</param>
    /// <exception cref="ArgumentNullException">Thrown when emitter or data is null.</exception>
    public static void RaiseInformation<TCategory, T>(this IEmitter<TCategory> emitter, T data, Dictionary<string, object>? properties = null) where T : IEvent
    {
        ArgumentNullException.ThrowIfNull(emitter);
        ArgumentNullException.ThrowIfNull(data);
        emitter.Raise(data, LogLevel.Information, properties);
    }

    /// <summary>
    /// Raises a warning-level event.
    /// </summary>
    /// <typeparam name="TCategory">The category type for logger classification.</typeparam>
    /// <typeparam name="T">The type of event to raise.</typeparam>
    /// <param name="emitter">The emitter instance.</param>
    /// <param name="data">The event data to log.</param>
    /// <param name="properties">Additional properties to include with the event.</param>
    /// <exception cref="ArgumentNullException">Thrown when emitter or data is null.</exception>
    public static void RaiseWarning<TCategory, T>(this IEmitter<TCategory> emitter, T data, Dictionary<string, object>? properties = null) where T : IEvent
    {
        ArgumentNullException.ThrowIfNull(emitter);
        ArgumentNullException.ThrowIfNull(data);
        emitter.Raise(data, LogLevel.Warning, properties);
    }

    /// <summary>
    /// Raises an error-level event.
    /// </summary>
    /// <typeparam name="TCategory">The category type for logger classification.</typeparam>
    /// <typeparam name="T">The type of event to raise.</typeparam>
    /// <param name="emitter">The emitter instance.</param>
    /// <param name="data">The event data to log.</param>
    /// <param name="properties">Additional properties to include with the event.</param>
    /// <exception cref="ArgumentNullException">Thrown when emitter or data is null.</exception>
    public static void RaiseError<TCategory, T>(this IEmitter<TCategory> emitter, T data, Dictionary<string, object>? properties = null) where T : IEvent
    {
        ArgumentNullException.ThrowIfNull(emitter);
        ArgumentNullException.ThrowIfNull(data);
        emitter.Raise(data, LogLevel.Error, properties);
    }

    /// <summary>
    /// Raises a critical-level event.
    /// </summary>
    /// <typeparam name="TCategory">The category type for logger classification.</typeparam>
    /// <typeparam name="T">The type of event to raise.</typeparam>
    /// <param name="emitter">The emitter instance.</param>
    /// <param name="data">The event data to log.</param>
    /// <param name="properties">Additional properties to include with the event.</param>
    /// <exception cref="ArgumentNullException">Thrown when emitter or data is null.</exception>
    public static void RaiseCritical<TCategory, T>(this IEmitter<TCategory> emitter, T data, Dictionary<string, object>? properties = null) where T : IEvent
    {
        ArgumentNullException.ThrowIfNull(emitter);
        ArgumentNullException.ThrowIfNull(data);
        emitter.Raise(data, LogLevel.Critical, properties);
    }

    /// <summary>
    /// Raises a trace-level event.
    /// </summary>
    /// <typeparam name="TCategory">The category type for logger classification.</typeparam>
    /// <typeparam name="T">The type of event to raise.</typeparam>
    /// <param name="emitter">The emitter instance.</param>
    /// <param name="data">The event data to log.</param>
    /// <param name="properties">Additional properties to include with the event.</param>
    /// <exception cref="ArgumentNullException">Thrown when emitter or data is null.</exception>
    public static void RaiseTrace<TCategory, T>(this IEmitter<TCategory> emitter, T data, Dictionary<string, object>? properties = null) where T : IEvent
    {
        ArgumentNullException.ThrowIfNull(emitter);
        ArgumentNullException.ThrowIfNull(data);
        emitter.Raise(data, LogLevel.Trace, properties);
    }
}