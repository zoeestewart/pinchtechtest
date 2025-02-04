
# Elevator 
This is a simple simulation of a 10 floor Elevator system that allows users to input commands and control the elevator's movement. The program supports commands in a specific format and processes requests to move the elevator between floors.

##Features
- Validate input format and values.
- Track up and down requests.
- Move the elevator between floors.
- Display errors for invalid input.

##Requirements
- .NET 9.0 SDK or later.
- Visual Studio, Visual Studio Code, or any C# compatible IDE.

##Setup
```
dotnet restore
```

##Running the program
```
dotnet run --project src/Elevator.csproj
```

#You need to input commands in the following format:
[{floor, 'direction', destination}]

#Where:
floor: The current floor of the elevator (0 to 10).
direction: Either 'up' or 'down' to indicate the direction.
destination: The target floor to reach (0 to 10).

#Example Input
{0, 'up', 3} — Move the elevator from floor 0 to floor 3.
{2, 'down', 1} — Move the elevator from floor 2 to floor 1.

#You can input multiple requests at once:
[{0, 'up', 3}, {2, 'down', 1}]

##Exit the Program
To exit the program, simply type exit and press Enter.

