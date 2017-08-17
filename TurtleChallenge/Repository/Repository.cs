using System;
using System.Collections.Generic;
using System.Linq;
using JavierCacho.EducationalProjects.AspNetCore.TurtleChallenge.Model;
using static JavierCacho.EducationalProjects.AspNetCore.TurtleChallenge.Model.Movement;
using System.IO;

namespace JavierCacho.EducationalProjects.AspNetCore.TurtleChallenge.Repository
{

    /// <summary>
    /// This contract defines the methods to return entities from the 
    /// repository
    /// </summary>
    interface ITurtleChallengeRepository
    {
        /// <summary>
        /// Obtains the Board entity from the repository file
        /// </summary>
        /// <param name="settingsFileName">Relative path to the settings file, including the name</param>
        /// <returns>Board Entity</returns>
        Board GetBoardSize(string settingsFileName);

        /// <summary>
        /// Obtains the exit location from the repository file
        /// </summary>
        /// <param name="settingsFileName">Relative path to the settings file, including the name</param>
        /// <returns>Pose object with exit position</returns>
        Pose GetExitPosition(string settingsFileName);

        /// <summary>
        /// Obtains a List with the location of each mine, from the 
        /// repository file
        /// </summary>
        /// <param name="settingsFileName">Relative path to the settings file, including the name</param>
        /// <returns>List with the mines positions</returns>
        IList<Pose> GetMinesPosition(string settingsFileName);

        /// <summary>
        /// Obtains the list of each path that the turtle will walk, 
        /// expresed as a list of ordered positions.
        /// </summary>
        /// <param name="movesFileName">Relative path to the moves file, including the name</param>
        /// <returns>List with each path, expressed itself as a List of positions</returns>
        IList<List<Pose>> GetTurtlePositions (string settingsFileName, string movesFileName);
    }

    public class TurtleChallengeRepository : ITurtleChallengeRepository
    {
        #region Constants 
        private const int SETTINGS_BOARD_SIZE_LINE  = 0;
        private const int SETTINGS_TURTLE_POSE_LINE = 1;
        private const int SETTINGS_EXIT_POSE_LINE   = 2;
        private const int SETTINGS_MINES_STARTING_LINE = 3;
        private const char FIELD_SEPARATOR = ',';
        #endregion

        #region Public Methods
        public Board GetBoardSize(string settingsFileName)
        {
            var settingsFileLines = File.ReadAllLines(settingsFileName);
            var boardSizeLine = settingsFileLines[SETTINGS_BOARD_SIZE_LINE].Split(FIELD_SEPARATOR);

            int width = 0;
            int height = 0;
            bool isInvalidBoardSize = boardSizeLine.Length != 2
                || !int.TryParse(boardSizeLine[0], out width)
                || !int.TryParse(boardSizeLine[1], out height);

            if (isInvalidBoardSize)
            {
                throw new ArgumentException("Settings file contains incorrect data for board size");
            }

            return new Board { Height = height, Width = width };

        }

        public Pose GetExitPosition(string settingsFileName)
        {
            var settingsFileLines = File.ReadAllLines(settingsFileName);
            var exitLine = settingsFileLines[SETTINGS_EXIT_POSE_LINE].Split(FIELD_SEPARATOR);

            int horizontalPosition = 0;
            int verticalPosition = 0;
            if (exitLine.Length != 2
                || !int.TryParse(exitLine[0], out horizontalPosition)
                || !int.TryParse(exitLine[1], out verticalPosition))
            {
                throw new ArgumentException("Settings file contains incorrect data for exit location");
            }

            return new Pose { HorizontalPosition = horizontalPosition, VerticalPosition = verticalPosition };
        }

        public IList<Pose> GetMinesPosition(string settingsFileName)
        {
            var settingsFileLines = File.ReadAllLines(settingsFileName);
            int minesNumber = settingsFileLines.Length - SETTINGS_MINES_STARTING_LINE;
            var minesLines = settingsFileLines.ToList().GetRange(SETTINGS_MINES_STARTING_LINE, minesNumber);

            IList<Pose> minePositionList = new List<Pose>();

            foreach (var line in minesLines)
            {
                int horizontalPosition = 0;
                int verticalPosition = 0;
                var lineFields = line.Split(FIELD_SEPARATOR);
                bool isInvalidMinePosition =
                    lineFields.Count() != 2
                    || !int.TryParse(lineFields[0], out horizontalPosition)
                    || !int.TryParse(lineFields[1], out verticalPosition);

                if (isInvalidMinePosition)
                {
                    throw new ArgumentException("Settings file contains incorrect data for at least one mine location");
                }

                minePositionList.Add
                    (
                        new Pose { HorizontalPosition = horizontalPosition, VerticalPosition = verticalPosition }
                    );
            }

            return minePositionList;
        }


        public IList<List<Pose>> GetTurtlePositions(string settingsFileName, string movesFileName)
        {
            var initialTurtlePose = GetInitialTurtlePose(settingsFileName);
            var turtleMoves = GetTurtleMoves(movesFileName);

            var turtleWalkDisplacements = GetTurtleDisplacementsPerWalk(initialTurtlePose, turtleMoves);

            IList<List<Pose>> walkSet = new List<List<Pose>>();

            for (var walkDisplacement = 0; walkDisplacement < turtleWalkDisplacements.Count(); walkDisplacement++)
            {
                List<Pose> walkPositions =
                    new List<Pose>
                    { new Pose()
                      {
                          HorizontalPosition = initialTurtlePose.HorizontalPosition,
                          VerticalPosition = initialTurtlePose.VerticalPosition
                      }
                    }
                ;


                for (int order = 0; order < turtleWalkDisplacements[walkDisplacement].Count(); order++)
                {

                    walkPositions.Add(
                        new Pose()
                        {
                            HorizontalPosition =
                            walkPositions[order].HorizontalPosition
                            + turtleWalkDisplacements[walkDisplacement][order].HorizontalDisplacement,

                            VerticalPosition =
                            walkPositions[order].VerticalPosition
                            + turtleWalkDisplacements[walkDisplacement][order].VerticalDisplacement,
                        }
                   );
                }
                walkSet.Add(walkPositions);
            }


            return walkSet;
        }

        #endregion


        #region Private Methods
        private Pose GetInitialTurtlePose(string settingsFileName)
        {
            var settingsFileLines = File.ReadAllLines(settingsFileName);
            var turtlePoseLine = settingsFileLines[SETTINGS_TURTLE_POSE_LINE].Split(FIELD_SEPARATOR);

            if (turtlePoseLine.Length != 3)
            {
                throw new ArgumentException("Settings file contains incorrect data for turtle pose");
            }

            int horizontalPosition = 0;
            int verticalPosition = 0;
            char cardinalPoint = Char.ToUpper(turtlePoseLine[2][0]);
            bool isInvalidTurtlePose =
                !int.TryParse(turtlePoseLine[0], out horizontalPosition)
                || !int.TryParse(turtlePoseLine[1], out verticalPosition)
                || turtlePoseLine[2].Length != 1
                || !CardinalPoint.CardinalPointList.Any(cp => (char)cp == cardinalPoint);

            if (isInvalidTurtlePose)
            {
                throw new ArgumentException("Settings file contains incorrect data for turtle pose");
            }

            return new Pose { HorizontalPosition = horizontalPosition, VerticalPosition = verticalPosition, Orientation = (CardinalPoint.CardinalPointType)cardinalPoint };
        }

        private IEnumerable<IList<MovementType>> GetTurtleMoves(string movesFileName)
        {
            var movesFileLines = File.ReadAllLines(movesFileName);


            bool isInvalidMove =
                movesFileLines
                .Any
                  (line =>
                    line.ToString().ToLower()
                    .Any(character => !Enum.IsDefined(typeof(MovementType), (MovementType)character))
                  );

            if (isInvalidMove) { throw new ArgumentException("At least one of the moves from the moves file is invalid"); }

            return movesFileLines.Select(line => line.Select(character => (MovementType)character).ToList());
        }

        private IList<List<Displacement>> GetTurtleDisplacementsPerWalk(Pose initialTurtlePose, IEnumerable<IList<MovementType>> walkSet)
        {

            return walkSet.Select(walk => walk.Select((move, order) =>
                   new Displacement
                   {
                       HorizontalDisplacement = (int)Movement.GetDistanceMoved(move) * MapPoseToDisplacement(MapMoveToPose(initialTurtlePose, walk))[order].HorizontalDisplacement,
                       VerticalDisplacement = (int)Movement.GetDistanceMoved(move) * MapPoseToDisplacement(MapMoveToPose(initialTurtlePose, walk))[order].VerticalDisplacement
                   }
                   ).ToList()
                   ).ToList();

        }

        private IList<Pose> MapMoveToPose(Pose initialTurtlePose, IList<MovementType> walk)
        {
            IList<Pose> movesPoses = new List<Pose>();

            for (int moveOrder = 0; moveOrder < walk.Count(); moveOrder++)
            {
                Pose pose = new Pose();

                if (moveOrder == 0)
                {
                    pose.Orientation = initialTurtlePose.Orientation; 
                    movesPoses.Add(pose);
                }
                else
                {

                    pose.Orientation =
                             walk[moveOrder] == MovementType.Forward ? movesPoses[moveOrder - 1].Orientation
                             : walk[moveOrder] == MovementType.Rotate ? CardinalPoint.Next(movesPoses[moveOrder - 1].Orientation)
                             : initialTurtlePose.Orientation;

                   movesPoses.Add(pose);
                }
            };

            return movesPoses;

        }

        private IList<Displacement> MapPoseToDisplacement(IList<Pose> orientationList)
        {
            IDictionary<CardinalPoint.CardinalPointType, Displacement> cardinalPointToDisplacementMap =
                new Dictionary<CardinalPoint.CardinalPointType, Displacement>
                {
                    { CardinalPoint.CardinalPointType.North, new Displacement { HorizontalDisplacement = 0,  VerticalDisplacement = 1 }} ,
                    { CardinalPoint.CardinalPointType.East , new Displacement { HorizontalDisplacement = 1,  VerticalDisplacement = 0 }} ,
                    { CardinalPoint.CardinalPointType.South, new Displacement { HorizontalDisplacement = 0,  VerticalDisplacement = -1}} ,
                    { CardinalPoint.CardinalPointType.West , new Displacement { HorizontalDisplacement = -1, VerticalDisplacement = 0 }}
                };

            return orientationList.Select(pose => { return cardinalPointToDisplacementMap[(CardinalPoint.CardinalPointType)pose.Orientation]; }).ToList();

        }
        #endregion


    }
}