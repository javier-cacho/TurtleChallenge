# Turtle Challenge
The game consits in guiding a turtle through a mine field to an exit. 

# How to use
Download the project from GitHub and compile in Visual Studio. 
For running the application locate the file TurtleChallenge.exe in the bin/Debug or bin/Release folder, depending on the configuration on which you run the compile.
The application requires two different files, one for the game-settings and another for the turtle moves. 
The relative paths of the files from the application folder should be provided as parameters, separated by an space.

    TurtleChallenge.exe game-settings moves

# Input files structure
In each file the records are separated by a new line character and the fields are separated by a comma character (`,`), and with no empty line at the end.
## game-settings file structure
In the game settings file each line represents different data about the game.
1. First line of the file represents the board size
2. Second line represents the initial pose (position and orientation) of the turtle. 
3. Third line is for the exit position
4. From the fourth line on, each line represents a mine position.

### Example 
    Line 1 20,10 --> Board of width 20 and height 10
    Line 2 0,1,E --> Initial pose of the turtle, x=0, y=1, and heading East
    Line 3 5,7   --> Position for the exit, x=5, y =7
    Line 4 8,9   --> Mine 1 at positon x=8 and y=9
    Line 5 7,6   --> Mine 2 at position x=7 and y=6    
    
## moves file structure
Each line of the moves file represents a turtle walk -sequence of moves, that will finish with a success result, mine result or none of them.
The moves are represented by the letters ´m for moving forward and `r for rotate.
### Example
    Line 1 mmmrrrmmm --> Turtle walk from initial point, to be checked against the settings file
    Line 2 mmrmmrmmr --> Another turtle walk from initial point, to be checked against the settings file
    Line 3 rmmmrmmrm --> One last turtle walk from initial point, to be checked against the settings file

# Code structure
The code is structured in the following way 
* `Program.cs`, located in the TurtleChallenge folder, contains the logic of the presentation to the console
* `Model` folder inside TurtleChallenge folder, contains the entities used by the Program.cs and returned by the repository.
* `Repository` folder inside TurtleChallenge folder, contains a `Repository.cs` with an interface defining the contract to obtain the entities from the files and a implementation of it. 
