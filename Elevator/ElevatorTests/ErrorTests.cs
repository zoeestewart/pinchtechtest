using Xunit;
using Moq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class GetErrorsTests
{
    [Fact]
    public void Error_WhenFloorAndDestinationAreSame()
    {
        // Arrange
        Elevator elevator = new Elevator();

        // Act
        List<string> errors = elevator.GetErrors(3, 3, "up");

        // Assert
        Assert.Contains("Floor and destination must be different", errors);
    }

    [Fact]
    public void Error_DirectionVaild()
    {
        // Arrange
        Elevator elevator = new Elevator();

        // Act
        List<string> errors = elevator.GetErrors(3, 3, "middle");

        // Assert
      
        Assert.Contains("Direction must be 'up' or 'down'", errors);
    }
    [Fact]
    public void Error_DestinationToHigh()
    {
        // Arrange
        Elevator elevator = new Elevator();

        // Act
        List<string> errors = elevator.GetErrors(3, 13, "up");

        // Assert
        Assert.Contains("Destination must be between 0 and 10", errors);
    }

    [Fact]
    public void Error_DestinationToLow()
    {
        // Arrange
        Elevator elevator = new Elevator();

        // Act
        List<string> errors = elevator.GetErrors(3, -4, "up");

        // Assert
        Assert.Contains("Destination must be between 0 and 10", errors);
    }
    
    [Fact]
    public void Error_FloorToHigh()
    {
        // Arrange
        Elevator elevator = new Elevator();

        // Act
        List<string> errors = elevator.GetErrors(13, 3, "down");

        // Assert
        Assert.Contains("Floor must be between 0 and 10", errors);
    }
    
    [Fact]
    public void Error_FloorToLow()
    {
        // Arrange
        Elevator elevator = new Elevator();

        // Act
        List<string> errors = elevator.GetErrors(-2, 4, "up");

        // Assert
        Assert.Contains("Floor must be between 0 and 10", errors);
    }

    [Fact]
    public void Error_UpDirectionDestinationBelowFloor()
    {
        // Arrange
        Elevator elevator = new Elevator();

        // Act
        List<string> errors = elevator.GetErrors(3, 2, "up");

        // Assert
        Assert.Contains("Destination must be above the onboarding floor for 'up' requests", errors);
    }

    [Fact]
    public void Error_DownDirectionDestinationAboveFloor()
    {
        // Arrange
        Elevator elevator = new Elevator();

        // Act
        List<string> errors = elevator.GetErrors(3, 4, "down");

        // Assert
        Assert.Contains("Destination must be below the onboarding floor for 'down' requests", errors);
    }

    [Fact]
    public void NoErrors()
    {
        // Arrange
        Elevator elevator = new Elevator();

        // Act
        List<string> errors = elevator.GetErrors(3, 4, "up");

        // Assert
        Assert.Empty(errors);
    }
    
    [Fact]
    public void MultipleErrors()
    {
        // Arrange
        Elevator elevator = new Elevator();

        // Act
        List<string> errors = elevator.GetErrors(13, 13, "middle");

        // Assert
        Assert.Contains("Floor must be between 0 and 10", errors);
        Assert.Contains("Destination must be between 0 and 10", errors);
        Assert.Contains("Floor and destination must be different", errors);
        Assert.Contains("Direction must be 'up' or 'down'", errors);
    }
}
