using Audabit.Common.Observability.Emitters;
using Audabit.Common.Observability.Events;
using Audabit.Common.Observability.Tests.Unit.TestHelpers;
using Microsoft.Extensions.Logging;

namespace Audabit.Common.Observability.Tests.Unit.Emitters;

public class EmitterTests
{
    public class EmitterTestsBase
    {
        protected readonly Fixture _fixture;
        protected readonly ILogger<EmitterTests> _logger;
        protected readonly Emitter<EmitterTests> _emitter;

        public EmitterTestsBase()
        {
            _fixture = FixtureFactory.Create();
            _logger = Substitute.For<ILogger<EmitterTests>>();
            _emitter = new Emitter<EmitterTests>(_logger);
        }
    }

    public class Raise : EmitterTestsBase
    {
        [Fact]
        public void GivenNullData_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException>(() => _emitter.Raise<TestEvent>(null!, LogLevel.Information));
        }

        [Theory, AutoData]
        public void GivenValidEvent_ShouldLogEvent(string message)
        {
            // Arrange
            _logger.IsEnabled(LogLevel.Information).Returns(true);
            var testEvent = new TestEvent("TestEvent") { Message = message };

            // Act
            _emitter.Raise(testEvent, LogLevel.Information);

            // Assert
            _logger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                null,
                Arg.Any<Func<object, Exception?, string>>());
        }

        [Theory, AutoData]
        public void GivenLoggerNotEnabled_ShouldNotLog(string message)
        {
            // Arrange
            _logger.IsEnabled(LogLevel.Debug).Returns(false);
            var testEvent = new TestEvent("TestEvent") { Message = message };

            // Act
            _emitter.Raise(testEvent, LogLevel.Debug);

            // Assert
            _logger.DidNotReceive().Log(
                Arg.Any<LogLevel>(),
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>());
        }

        [Theory, AutoData]
        public void GivenAdditionalProperties_ShouldIncludeThem(string message, string customKey, string customValue)
        {
            // Arrange
            _logger.IsEnabled(LogLevel.Information).Returns(true);
            var testEvent = new TestEvent("TestEvent") { Message = message };
            var additionalProperties = new Dictionary<string, object> { { customKey, customValue } };

            // Act
            _emitter.Raise(testEvent, LogLevel.Information, additionalProperties);

            // Assert
            _logger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                null,
                Arg.Any<Func<object, Exception?, string>>());
        }
    }

    public class Raise_LoggingEvent : EmitterTestsBase
    {
        [Theory, AutoData]
        public void GivenLoggingEvent_ShouldUseInfoLevel(string message, string eventDescription)
        {
            // Arrange
            _logger.IsEnabled(LogLevel.Information).Returns(true);
            var testEvent = new TestEvent(eventDescription) { Message = message };

            // Act
            _emitter.Raise(testEvent, LogLevel.Information);

            // Assert
            _logger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                null,
                Arg.Any<Func<object, Exception?, string>>());
        }
    }

    public class Raise_ErrorEvent : EmitterTestsBase
    {
        [Theory, AutoData]
        public void GivenErrorEvent_ShouldUseErrorLevel(string errorMessage, string message, string eventDescription)
        {
            // Arrange
            _logger.IsEnabled(LogLevel.Error).Returns(true);
            var exception = new InvalidOperationException(errorMessage);
            var errorEvent = new TestErrorEvent(eventDescription, message, exception);

            // Act
            _emitter.Raise(errorEvent, LogLevel.Error);

            // Assert
            _logger.Received(1).Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>());
        }
    }

    private class TestEvent(string eventDescription) : LoggingEvent(eventDescription)
    {
        public string Message
        {
            get => Properties.TryGetValue("Message", out var value) ? value.ToString() ?? string.Empty : string.Empty;
            set => Properties["Message"] = value;
        }
    }

    private class TestErrorEvent(string eventDescription, string errorMessage, Exception? exception = null) : ErrorEvent(eventDescription, errorMessage, exception)
    {
        public string Message
        {
            get => Properties.TryGetValue("Message", out var value) ? value.ToString() ?? string.Empty : string.Empty;
            set => Properties["Message"] = value;
        }
    }
}