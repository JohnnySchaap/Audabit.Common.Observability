using Audabit.Common.Observability.Events;
using Audabit.Common.Observability.Tests.Unit.TestHelpers;

namespace Audabit.Common.Observability.Tests.Unit.Events;

public class ErrorEventTests
{
    public class ErrorEventTestsBase
    {
        protected readonly Fixture _fixture;

        public ErrorEventTestsBase()
        {
            _fixture = FixtureFactory.Create();
        }
    }

    public class Constructor : ErrorEventTestsBase
    {
        [Theory, AutoData]
        public void GivenValidParameters_ShouldCreateEvent(string eventDescription, string errorMessage)
        {
            // Act
            var errorEvent = new ErrorEvent(eventDescription, errorMessage);

            // Assert
            errorEvent.ShouldNotBeNull();
            errorEvent.EventName.ShouldContain(eventDescription);
        }

        [Theory, AutoData]
        public void GivenException_ShouldIncludeExceptionDetails(string eventDescription, string errorMessage, string exceptionMessage)
        {
            // Arrange
            var exception = new InvalidOperationException(exceptionMessage);

            // Act
            var errorEvent = new ErrorEvent(eventDescription, errorMessage, exception);

            // Assert
            var properties = errorEvent.BuildProperties();
            properties.ShouldContainKey("Error message");
            properties.ShouldContainKey("Exception type");
            properties.ShouldContainKey("Exception message");
            properties.ShouldContainKey("Base.Exception type");
            properties.ShouldContainKey("Base.Exception message");
            properties["Error message"].ShouldBe(errorMessage);
            properties["Exception type"].ShouldBe("InvalidOperationException");
            properties["Exception message"].ShouldBe(exceptionMessage);
        }

        [Theory, AutoData]
        public void GivenExceptionWithStackTrace_ShouldIncludeStackTrace(string eventDescription, string errorMessage, string exceptionMessage)
        {
            // Arrange
            Exception? capturedException;
            try
            {
                throw new InvalidOperationException(exceptionMessage);
            }
            catch (InvalidOperationException ex)
            {
                capturedException = ex;
            }

            // Act
            var errorEvent = new ErrorEvent(eventDescription, errorMessage, capturedException);

            // Assert
            var properties = errorEvent.BuildProperties();
            properties.ShouldContainKey("Stack");
            properties["Stack"].ShouldNotBeNull();
        }

        [Theory, AutoData]
        public void GivenExceptionWithInnerException_ShouldIncludeBaseException(string eventDescription, string errorMessage, string innerMessage, string outerMessage)
        {
            // Arrange
            var innerException = new ArgumentException(innerMessage);
            var exception = new InvalidOperationException(outerMessage, innerException);

            // Act
            var errorEvent = new ErrorEvent(eventDescription, errorMessage, exception);

            // Assert
            var properties = errorEvent.BuildProperties();
            properties.ShouldContainKey("Base.Exception type");
            properties.ShouldContainKey("Base.Exception message");
            properties["Base.Exception type"].ShouldBe("ArgumentException");
            properties["Base.Exception message"].ShouldBe(innerMessage);
        }

        [Theory, AutoData]
        public void GivenAdditionalProperties_ShouldIncludeThem(string eventDescription, string errorMessage, string customKey1, string customValue1, string customKey2, int customValue2)
        {
            // Arrange
            var additionalProperties = new Dictionary<string, object>
            {
                { customKey1, customValue1 },
                { customKey2, customValue2 }
            };

            // Act
            var errorEvent = new ErrorEvent(eventDescription, errorMessage, null, additionalProperties);

            // Assert
            var properties = errorEvent.BuildProperties();
            properties.ShouldContainKey(customKey1);
            properties.ShouldContainKey(customKey2);
            properties[customKey1].ShouldBe(customValue1);
            properties[customKey2].ShouldBe(customValue2);
        }

        [Theory, AutoData]
        public void GivenNullOrWhitespaceErrorMessage_ShouldThrowArgumentException(string eventDescription)
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() => new ErrorEvent(eventDescription, null!));
            Should.Throw<ArgumentException>(() => new ErrorEvent(eventDescription, ""));
            Should.Throw<ArgumentException>(() => new ErrorEvent(eventDescription, "   "));
        }

        [Theory, AutoData]
        public void GivenNullOrWhitespaceEventDescription_ShouldThrowArgumentException(string errorMessage)
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() => new ErrorEvent(null!, errorMessage));
            Should.Throw<ArgumentException>(() => new ErrorEvent("", errorMessage));
            Should.Throw<ArgumentException>(() => new ErrorEvent("   ", errorMessage));
        }
    }

    public class BuildProperties : ErrorEventTestsBase
    {
        [Theory, AutoData]
        public void GivenErrorEvent_ShouldIncludeErrorMessage(string eventDescription, string errorMessage)
        {
            // Arrange
            var errorEvent = new ErrorEvent(eventDescription, errorMessage);

            // Act
            var properties = errorEvent.BuildProperties();

            // Assert
            properties.ShouldContainKey("Error message");
            properties["Error message"].ShouldBe(errorMessage);
        }

        [Theory, AutoData]
        public void GivenErrorEventWithoutException_ShouldNotIncludeExceptionProperties(string eventDescription, string errorMessage)
        {
            // Arrange
            var errorEvent = new ErrorEvent(eventDescription, errorMessage);

            // Act
            var properties = errorEvent.BuildProperties();

            // Assert
            properties.ShouldNotContainKey("Exception");
            properties.ShouldNotContainKey("Exception type");
            properties.ShouldNotContainKey("Exception message");
        }
    }
}