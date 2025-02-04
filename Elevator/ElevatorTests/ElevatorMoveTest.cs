using Xunit;
using Moq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ConsoleOutput : IDisposable
{
    private readonly Elevator _elevator = new Elevator();
    private StringWriter stringWriter;
    private TextWriter originalOutput;

    public ConsoleOutput()
    {
        stringWriter = new StringWriter();
        originalOutput = Console.Out;
        Console.SetOut(stringWriter);
    }

    public string GetOuput()
    {
        return stringWriter.ToString().Trim();
    }

    public void Dispose()
    {
        Console.SetOut(originalOutput);
        stringWriter.Dispose();
    }

    [Fact]
    public void ValidInputFormat()
    {
        // Arrange
        string validInput = "[{0,'up',3}, {6,'down',2}]";

        using (var consoleOutput = new ConsoleOutput())
        {
            // Act
            MatchCollection matches = _elevator.ValidateInput(validInput);

            // Assert
            Assert.Equal(2, matches.Count);
            Assert.Equal("0", matches[0].Groups[1].Value);
            Assert.Equal("up", matches[0].Groups[2].Value);
            Assert.Equal("3", matches[0].Groups[3].Value);
        }
    }

    [Fact]
    public void InvalidInputFormat_MissingBrackets()
    {
        // Arrange
        string invalidInput = "{0,'up',3}"; 
        
        using (var consoleOutput = new ConsoleOutput())
        {
            // Act
            MatchCollection matches = _elevator.ValidateInput(invalidInput);

            // Assert
            string output = consoleOutput.GetOuput();
            Assert.Contains("Error: Invalid input format. Ensure only valid entries are provided.", output);
            Assert.Empty(matches);
        }
    }
    
    [Fact]
    public void InvalidInputFormat_ExtraCharacters()
    {
        // Arrange
        string invalidInput = "[{0,'up',3}, extra]"; 

        using (var consoleOutput = new ConsoleOutput())
        {
            // Act
            MatchCollection matches = _elevator.ValidateInput(invalidInput);

            // Assert
            string output = consoleOutput.GetOuput();
            Assert.Contains("Error: Invalid input format. Ensure only valid entries are provided.", output);
            Assert.Empty(matches);
        }
    }

    [Fact]
    public void ElevatorParsesUpRequest()
    {
        // Arrange
        using (var consoleOutput = new ConsoleOutput())
        {
            // Act
            MatchCollection matches = _elevator.ValidateInput("[{0,'up',3}]");

            bool parsed = _elevator.ParseInput(matches);
            Assert.True(parsed);

            _elevator.Move();

            // Assert
            string[] output = consoleOutput.GetOuput().Split(Environment.NewLine);
            string[] expectedOutput = [
                "Opening doors on the ground floor.",
                "Elevator moving from the ground floor to the 3rd floor.",
                "Opening doors on the 3rd floor."
            ];
            Assert.Equal(output, expectedOutput);
        }
    }

    [Fact]
    public void ElevatorParsesDownRequest()
    {
        // Arrange
        using (var consoleOutput = new ConsoleOutput())
        {
            // Act
            MatchCollection matches = _elevator.ValidateInput("[{2,'down',1}]");

            bool parsed = _elevator.ParseInput(matches);
            Assert.True(parsed);

            _elevator.Move();

            // Assert
            string[] output = consoleOutput.GetOuput().Split(Environment.NewLine);
            string[] expectedOutput = [
                "Elevator moving from the ground floor to the 2nd floor.",
                "Opening doors on the 2nd floor.",
                "Elevator moving from the 2nd floor to the 1st floor.",
                "Opening doors on the 1st floor."
            ];
            Assert.Equal(output, expectedOutput);
        }
    }

    [Fact]
    public void ElevatorParsesSingleMultiRequest()
    {
        // Arrange
        using (var consoleOutput = new ConsoleOutput())
        {
            MatchCollection matches = _elevator.ValidateInput("[{0,'up',3},{2,'down',1},{6,'down',2},{4,'up',7},{7,'down',2},{4,'up',9}]");

            // Act
            if (matches.Count > 0)
            {
                _elevator.ParseInput(matches);
                _elevator.Move();  
            }

            // Assert
            string[] output = consoleOutput.GetOuput().Split(Environment.NewLine);
            string[] expectedOutput = [
                "Opening doors on the ground floor.",
                "Elevator moving from the ground floor to the 3rd floor.",
                "Opening doors on the 3rd floor.",
                "Elevator moving from the 3rd floor to the 4th floor.",
                "Opening doors on the 4th floor.",
                "Elevator moving from the 4th floor to the 7th floor.",
                "Opening doors on the 7th floor.",
                "Elevator moving from the 7th floor to the 9th floor.",
                "Opening doors on the 9th floor.",
                "Elevator moving from the 9th floor to the 7th floor.",
                "Opening doors on the 7th floor.",
                "Elevator moving from the 7th floor to the 6th floor.",
                "Opening doors on the 6th floor.",
                "Elevator moving from the 6th floor to the 2nd floor.",
                "Opening doors on the 2nd floor.",
                "Elevator moving from the 2nd floor to the 1st floor.",
                "Opening doors on the 1st floor."
            ];
            Assert.Equal(output, expectedOutput);


            Console.SetOut(new StringWriter());
        }
    }

    
    [Fact]
    public void ElevatorParsesMultipleMoveRequests()
    {
        // Arrange
        using (var consoleOutput = new ConsoleOutput())
        {
            MatchCollection matches;
            
            matches = _elevator.ValidateInput("[{0,'up',3},{4,'up',7},{7,'down',6},{4,'up',9}]");
            // Act
            if (matches.Count > 0)
            {
                _elevator.ParseInput(matches);
                _elevator.Move();  
            }

            
            // do second elevator move - should go up to 7
            matches = _elevator.ValidateInput("[{ 9, 'down', 8}]");
            if (matches.Count > 0)
            {
                _elevator.ParseInput(matches);
                _elevator.Move();  
            }


            // do third elevator move - should continue going down
            matches = _elevator.ValidateInput("[{ 7, 'down', 6}]");
            if (matches.Count > 0)
            {
                _elevator.ParseInput(matches);
                _elevator.Move();  
            }

            
            // do fourth elevator move - continue moving down, then move up 
            matches = _elevator.ValidateInput("[{ 5, 'down', 4 }, { 1, 'up', 4 }]");
            if (matches.Count > 0)
            {
                _elevator.ParseInput(matches);
                _elevator.Move();  
            }


            // do fifth move - elevator going up, but only receives down request - move down
            matches = _elevator.ValidateInput("[{ 2, 'down', 1 }]");
            if (matches.Count > 0)
            {
                _elevator.ParseInput(matches);
                _elevator.Move();  
            }

            // Assert
            string[] output = consoleOutput.GetOuput().Split(Environment.NewLine);
            string[] expectedOutput = [
                "Opening doors on the ground floor.",
                "Elevator moving from the ground floor to the 3rd floor.",
                "Opening doors on the 3rd floor.",
                "Elevator moving from the 3rd floor to the 4th floor.",
                "Opening doors on the 4th floor.",
                "Elevator moving from the 4th floor to the 7th floor.",
                "Opening doors on the 7th floor.",
                "Elevator moving from the 7th floor to the 9th floor.",
                "Opening doors on the 9th floor.",
                "Elevator moving from the 9th floor to the 7th floor.",
                "Opening doors on the 7th floor.",
                "Elevator moving from the 7th floor to the 6th floor.",
                "Opening doors on the 6th floor.",
                
            // do second elevator move but the current floor is now the 6th, elevator is currently moving down
            // the elevator should move up to 9th and then down

                "Elevator moving from the 6th floor to the 9th floor.",
                "Opening doors on the 9th floor.",
                "Elevator moving from the 9th floor to the 8th floor.",
                "Opening doors on the 8th floor.",
                
            // do third elevator move, elevator is currently on the 8th floor and should continue going down
                "Elevator moving from the 8th floor to the 7th floor.",
                "Opening doors on the 7th floor.",
                "Elevator moving from the 7th floor to the 6th floor.",
                "Opening doors on the 6th floor.",
            // do fourth elevator move, elevator is currently on the 6th floor should continue moving down, then move up 
                "Elevator moving from the 6th floor to the 5th floor.",
                "Opening doors on the 5th floor.",
                "Elevator moving from the 5th floor to the 4th floor.",
                "Opening doors on the 4th floor.",
                "Elevator moving from the 4th floor to the 1st floor.",
                "Opening doors on the 1st floor.",
                "Elevator moving from the 1st floor to the 4th floor.",
                "Opening doors on the 4th floor.",

            // fifth move - elevator is current on the 4th floor and set to going up, but only receives down request - move down
                "Elevator moving from the 4th floor to the 2nd floor.",
                "Opening doors on the 2nd floor.",
                "Elevator moving from the 2nd floor to the 1st floor.",
                "Opening doors on the 1st floor."
            ];

            Assert.Equal(output, expectedOutput);
        }
    }
}