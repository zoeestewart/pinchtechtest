using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Elevator
{
    private int currentFloor = 0;
    private bool goingUp = true;
    private bool elevatorChangedDirections = true;
    private HashSet<string> errorMessages = new();
    private SortedSet<int> upRequests = new();
    private SortedSet<int> downRequests = new();

    public string GetMovePrefix(int floor) {
        return floor switch
        {
            0 => "ground",
            1 => "1st",
            2 => "2nd",
            3 => "3rd",
            _ => $"{floor}th"
        };
    }

    private void OpenDoors(int floor)
    {
        Console.WriteLine($"Opening doors on the {GetMovePrefix(floor)} floor.");
    }

    /*
        Get a list of errors that invaildated the input values 
    */
    public List<string> GetErrors(int floor, int destination, string direction)
    {
        List<string> errors = new();

        if (floor < 0 || floor > 10)
            errors.Add("Floor must be between 0 and 10");
        if (destination < 0 || destination > 10)
            errors.Add("Destination must be between 0 and 10");
        if (floor == destination)
            errors.Add("Floor and destination must be different");
        if (direction != "up" && direction != "down")
            errors.Add("Direction must be 'up' or 'down'");
        if (direction == "up" && destination < floor)
            errors.Add("Destination must be above the onboarding floor for 'up' requests");
        if (direction == "down" && destination > floor)
            errors.Add("Destination must be below the onboarding floor for 'down' requests");
        return errors;
    }

    /*
        Validate the input format and 
        output a console message if the input is invalid
        return the matches if the input is valid
    */
    public MatchCollection ValidateInput(string input)
    {
        string entryPattern = @"\{\s*(\d+)\s*,\s*'(up|down)'\s*,\s*(\d+)\s*\}";
        string pattern = @"^\[\s*(" + entryPattern + @",?\s*)*\]$";
        
        if (!Regex.IsMatch(input, pattern)) {
            Console.WriteLine("Error: Invalid input format. Ensure only valid entries are provided.");
           return Regex.Matches("", entryPattern);
        }
        
        return Regex.Matches(input, entryPattern);
    }

    /*
        Only add vaild input requests to the upRequests and downRequests
    */
    public bool ParseInput(MatchCollection matches) {
        SortedSet<int> newUpRequests = new();
        SortedSet<int> newDownRequests = new();

        foreach (Match match in matches)
        {
            int floor = int.Parse(match.Groups[1].Value);
            string direction = match.Groups[2].Value;
            int destination = int.Parse(match.Groups[3].Value);
            var errors = GetErrors(floor, destination, direction);

            // Only add the request if there are no errors on this input or existing errors
            if (errors.Count == 0) {
                if (direction == "up") {
                    newUpRequests.Add(floor);
                    newUpRequests.Add(destination);
                } else {
                    newDownRequests.Add(floor);
                    newDownRequests.Add(destination);
                }
            } 
            else 
            {
                errorMessages.Add($"Error for {match.Value}: {string.Join(", ", errors)}");
            }
        }

        // if there was any vaildation issues with the input, output the errors
        // dont process any input requests
        if (errorMessages.Count > 0) {
            foreach (var msg in errorMessages)
                Console.WriteLine(msg);
            return false;
        } else {
            // if there are no errors, add the new requests to the existing requests
            upRequests.UnionWith(newUpRequests);
            downRequests.UnionWith(newDownRequests);
            return true;
        }
    }

    /*
        Get the next floor for the elevator to move to based on the current floor and the requests
    */
    private int GetNextFloor()
    {   
        if (goingUp) 
        {   
            var floorsAboveCurrent = upRequests.Where(c => c >= currentFloor).ToList();
            int nextUpFloor = floorsAboveCurrent.Count > 0 ? floorsAboveCurrent.First() : -1;
            
            // if there are more floors to up to and it just changed directions
            // but the elevator is already above all the floors in the list
            // or the floor found is the current floor ( in the case, we need to make sure that was the lowest value)
            // find the lowest value in the up requests, to get the lowest person and go up again

            if( elevatorChangedDirections == false && upRequests.Count > 0 && 
                (nextUpFloor == currentFloor || nextUpFloor == -1)
            ) {
                nextUpFloor = upRequests.Min();
            } 
            
            // if a floor was ultimately found return it
            if (nextUpFloor != -1) {
                upRequests.Remove(nextUpFloor);
                return nextUpFloor;
            } else {
                // else switch directions
                goingUp = false;
                elevatorChangedDirections = true;
            }
        }
        
        if (!goingUp) 
        {
            var floorsBelowCurrent = downRequests.Where(c => c <= currentFloor).ToList();
            int nextDownFloor = floorsBelowCurrent.Count > 0 ? floorsBelowCurrent.Last() : -1;
            
            // if there are more floors to down to and it just changed directions
            // but the elevator is already below all the floors in the list
            // or the floor found is the current floor ( in the case, we need to make sure that was the highest value)
            // find the highest value in the down requests, to get the highest person and go down again
            if( elevatorChangedDirections == true && downRequests.Count > 0 && 
                (nextDownFloor == currentFloor || nextDownFloor == -1)
            ) {
                nextDownFloor = downRequests.Max();
            } 
            
            // if a floor was ultimately found return it
            if (nextDownFloor != -1) {
                downRequests.Remove(nextDownFloor);
                return nextDownFloor;
            } else {
                // else switch directions
                goingUp = true;
                elevatorChangedDirections = false;
            }
        }
        
        return currentFloor; // No movement if no valid request found
    }

    /*
        Move the elevator to the next floor
        And open the doors if the elevator is at the destination floor
    */
    public void Move()
    {   
        // have this so if the first move is to the current floor, make the doors open
        bool firstMove = true;
        while (upRequests.Count > 0 || downRequests.Count > 0) {
            int nextFloor = GetNextFloor();
            if (currentFloor != nextFloor) {
                Console.WriteLine($"Elevator moving from the {GetMovePrefix(currentFloor)} floor to the {GetMovePrefix(nextFloor)} floor.");
                currentFloor = nextFloor;
                OpenDoors(currentFloor);
            } else if (currentFloor == nextFloor && firstMove) {
                OpenDoors(currentFloor);
            }
            firstMove = false;
        }
    }
}

public class Program
{
    public static void Main()
    {
        Elevator elevator = new Elevator();
        Console.WriteLine("Enter commands as {floor,direction,destination}[] or type 'exit' to quit:");

        while (true)
        {
            Console.Write("> ");
            string? input = Console.ReadLine();
            
            if (input == null || input.ToLower() == "exit") break;

            MatchCollection entries = elevator.ValidateInput(input);
            
            if (entries.Count > 0 && elevator.ParseInput(entries))
            {
                elevator.Move();
            }
        
        }
        Console.WriteLine("Exiting... Goodbye!");
    }
}
