using Audabit.Common.Observability.Emitters;
using Audabit.Common.Observability.Events;
using Audabit.Common.Observability.Extensions;
using Audabit.Common.Observability.Tests.Unit.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Audabit.Common.Observability.Tests.Unit.Extensions;

public class EmitterExtensionsTests
{
    public class TestEvent(string eventDescription, string messageKey = "Message") : LoggingEvent(eventDescription)
    {
        public string Message
        {
            get => Properties.TryGetValue(messageKey, out var value) ? value.ToString() ?? string.Empty : string.Empty;
            set => Properties[messageKey] = value;
        }
    }

    public class EmitterExtensionsTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly ILogger<EmitterExtensionsTests> _logger;
        protected readonly Emitter<EmitterExtensionsTests> _emitter;

        public EmitterExtensionsTestsBase()
        {
            _fixture = FixtureFactory.Create();
            _logger = Substitute.For<ILogger<EmitterExtensionsTests>>();
            _emitter = new Emitter<EmitterExtensionsTests>(_logger);
        }

        protected void RaiseEventForLevel(LogLevel logLevel, TestEvent testEvent, Dictionary<string, object>? properties = null)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    if (properties != null) _emitter.RaiseDebug(testEvent, properties);
                    else _emitter.RaiseDebug(testEvent);
                    break;
                case LogLevel.Information:
                    if (properties != null) _emitter.RaiseInformation(testEvent, properties);
                    else _emitter.RaiseInformation(testEvent);
                    break;
                case LogLevel.Warning:
                    if (properties != null) _emitter.RaiseWarning(testEvent, properties);
                    else _emitter.RaiseWarning(testEvent);
                    break;
                case LogLevel.Error:
                    if (properties != null) _emitter.RaiseError(testEvent, properties);
                    else _emitter.RaiseError(testEvent);
                    break;
                case LogLevel.Critical:
                    if (properties != null) _emitter.RaiseCritical(testEvent, properties);
                    else _emitter.RaiseCritical(testEvent);
                    break;
                case LogLevel.Trace:
                    if (properties != null) _emitter.RaiseTrace(testEvent, properties);
                    else _emitter.RaiseTrace(testEvent);
                    break;
            }
        }
    }

    public class RaiseWithLogLevel : EmitterExtensionsTestsBase
    {
        [Theory]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Warning)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.Trace)]
        public void GivenValidEvent_ShouldRaiseWithCorrectLevel(LogLevel logLevel)
        {
            // Arrange
            _logger.IsEnabled(logLevel).Returns(true);
            var eventDescription = _fixture.Create<string>();
            var message = _fixture.Create<string>();
            var testEvent = new TestEvent(eventDescription) { Message = message };

            // Act
            RaiseEventForLevel(logLevel, testEvent);

            // Assert
            _logger.Received(1).Log(
                logLevel,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                null,
                Arg.Any<Func<object, Exception?, string>>());
        }

        [Theory]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Warning)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.Trace)]
        public void GivenValidEventWithProperties_ShouldRaiseWithCorrectLevel(LogLevel logLevel)
        {
            // Arrange
            _logger.IsEnabled(logLevel).Returns(true);
            var eventDescription = _fixture.Create<string>();
            var message = _fixture.Create<string>();
            var key = _fixture.Create<string>();
            var value = _fixture.Create<string>();
            var testEvent = new TestEvent(eventDescription) { Message = message };
            var properties = new Dictionary<string, object> { { key, value } };

            // Act
            RaiseEventForLevel(logLevel, testEvent, properties);

            // Assert
            _logger.Received(1).Log(
                logLevel,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                null,
                Arg.Any<Func<object, Exception?, string>>());
        }

        [Fact]
        public void GivenNullEmitter_ShouldThrowArgumentNullException()
        {
            // Arrange
            IEmitter<EmitterExtensionsTests>? emitter = null;
            var eventDescription = _fixture.Create<string>();
            var testEvent = new TestEvent(eventDescription);

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => emitter!.RaiseDebug(testEvent));
        }

        [Fact]
        public void GivenNullData_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException>(() => _emitter.RaiseDebug<EmitterExtensionsTests, TestEvent>(null!));
        }
    }
}