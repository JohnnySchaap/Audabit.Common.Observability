using Audabit.Common.Observability.Events;
using Audabit.Common.Observability.Tests.Unit.TestHelpers;

namespace Audabit.Common.Observability.Tests.Unit.Events;

public class LoggingEventTests
{
    public class LoggingEventTestsBase
    {
        protected readonly Fixture _fixture;

        public LoggingEventTestsBase()
        {
            _fixture = FixtureFactory.Create();
        }
    }

    public class SetServiceName : LoggingEventTestsBase
    {
        [Theory, AutoData]
        public void GivenValidName_ShouldSetServiceName(string serviceName, string eventDescription)
        {
            // Act
            LoggingEvent.SetServiceName(serviceName);

            // Assert
            var testEvent = new TestLoggingEvent(eventDescription);
            testEvent.EventName.ShouldContain(serviceName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenInvalidName_ShouldThrowArgumentException(string? invalidName)
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() => LoggingEvent.SetServiceName(invalidName!));
        }
    }

    public class BuildProperties : LoggingEventTestsBase
    {
        [Theory, AutoData]
        public void GivenEvent_ShouldIncludeEventName(string eventDescription)
        {
            // Arrange
            var testEvent = new TestLoggingEvent(eventDescription);

            // Act
            var properties = testEvent.BuildProperties();

            // Assert
            properties.ShouldContainKey("Event description");
            properties["Event description"].ShouldBe(eventDescription);
        }
    }

    public class Name : LoggingEventTestsBase
    {
        [Theory, AutoData]
        public void GivenEvent_ShouldFollowNamingConvention(string eventDescription)
        {
            // Arrange
            var testEvent = new TestLoggingEvent(eventDescription);

            // Act
            var name = testEvent.EventName;

            // Assert
            name.ShouldContain($".{eventDescription}");
        }
    }

    private class TestLoggingEvent(string eventDescription) : LoggingEvent(eventDescription)
    {
    }
}