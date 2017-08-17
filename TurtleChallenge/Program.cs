using JavierCacho.EducationalProjects.AspNetCore.TurtleChallenge.Model;
using JavierCacho.EducationalProjects.AspNetCore.TurtleChallenge.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO; 

namespace JavierCacho.EducationalProjects.AspNetCore.TurtleChallenge
{

    class Program
    {
        static void Main(string[] args)
        {
            string gameSettingsFileName = args != null && args.Count() > 0 && !string.IsNullOrEmpty(args[0]) ? args[0] : null;
            string movesFileName = args != null && args.Count() > 0 && !string.IsNullOrEmpty(args[1]) ? args[1] : null;

            while (args == null || args.Count() <= 0|| string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]))
            {
                Console.WriteLine("Please, provide game-settings and moves files");
                args = Console.ReadLine().Split();

                gameSettingsFileName = args[0];
                movesFileName = args[1];

                  if (!File.Exists(gameSettingsFileName))
                  {
                    Console.WriteLine("game-settings file does not exists.");
                    args[0] = null;
                   }


                if (!File.Exists(movesFileName))
                {
                    Console.WriteLine("moves file does not exists.");
                    args[1] = null;
                }
            };

            ITurtleChallengeRepository turtleRepository = new TurtleChallengeRepository();
            var board = turtleRepository.GetBoardSize(gameSettingsFileName);
            var exitPosition = turtleRepository.GetExitPosition(gameSettingsFileName);

            if (!IsValidPosition(exitPosition, board))
            {
                Console.WriteLine("Incoherent data: Exit is outside the board!");
                Console.ReadLine();
                return; 
            }


            var minesPositions= turtleRepository.GetMinesPosition(gameSettingsFileName);

            if (minesPositions.Any(mine => !IsValidPosition(mine, board)))
            {
                Console.WriteLine("Incoherent data: There is at least one mine outside the board!");
                Console.ReadLine();
                return;
            }


            var turtlePositions= turtleRepository.GetTurtlePositions(gameSettingsFileName, movesFileName);
            
            TurtleWalkResultManager(board, exitPosition, minesPositions, turtlePositions);

            Console.WriteLine("Program has ended. Press any key to close the window");
            Console.ReadLine();
        }

        static bool IsValidPosition(Pose position, Board board)
        {
            return position.HorizontalPosition <= board.Width
                  || position.VerticalPosition <= board.Height;
        }


        /// <summary>
        /// Transform the game settings and moves into specific results for each turtle walk
        /// </summary>
        /// <param name="board">Board size</param>
        /// <param name="exitPosition">Exit location</param>
        /// <param name="minePositions">Mines locations</param>
        /// <param name="turtlePositions">Positions the turtle is walking on</param>
        static void TurtleWalkResultManager(Board board, Pose exitPosition, IList<Pose> minePositions, IList<List<Pose>> turtlePositions )
        {
            for (int walkOrder = 0; walkOrder < turtlePositions.Count(); walkOrder++)
            {
                var currentWalk = turtlePositions[walkOrder]; 

                for (int positionOrder = 0; positionOrder < currentWalk.Count(); positionOrder++)
                {
                    var currentTurtlePosition = turtlePositions[walkOrder][positionOrder];

                    if (currentTurtlePosition.HorizontalPosition == exitPosition.HorizontalPosition
                        && currentTurtlePosition.VerticalPosition == exitPosition.VerticalPosition)
                    {
                        Console.WriteLine("The turtle succeeded");
                        break;
                    }
                    else if (minePositions.Any(mine => mine.HorizontalPosition == currentTurtlePosition.HorizontalPosition 
                                                    && mine.VerticalPosition == currentTurtlePosition.VerticalPosition))
                    {
                        Console.WriteLine("The turtle was killed by a mine");
                        break;
                    }
                    else if (currentTurtlePosition.HorizontalPosition >= board.Width || currentTurtlePosition.VerticalPosition >= board.Height)
                    {
                        Console.WriteLine("The turtle gone outside the board");
                        break;
                    }
                    else if (positionOrder == currentWalk.Count()-1)
                    {
                        Console.WriteLine("The turtle is a wanderer");
                        break;
                    }
                }
            }
            
        }


    }
}
