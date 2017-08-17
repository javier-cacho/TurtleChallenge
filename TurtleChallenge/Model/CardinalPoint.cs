using System;
using System.Collections.Generic;

namespace JavierCacho.EducationalProjects.AspNetCore.TurtleChallenge.Model
{
    /// <summary>
    /// Class that manages the turtle orientation
    /// </summary>
    public static class CardinalPoint
    {
        /// <summary>
        /// Contains the diferent cardinal points
        /// </summary>
        public enum CardinalPointType
        {
            North = 'N',
            South = 'S',
            West = 'W',
            East = 'E'
        }


        /// <summary>
        /// Returns IEnumerable with all the cardinal points
        /// </summary>
        /// <returns>Next cardinal point in clockwise order</returns>
        public static IList<CardinalPointType> CardinalPointList
        {
                get {
                    return
                      new List<CardinalPointType>
                      {
                    CardinalPointType.North, CardinalPointType.East, CardinalPointType.South, CardinalPointType.West
                      };
                }
        }

        /// <summary>
        /// Obtains the next cardinal point in clockwise order
        /// </summary>
        /// <param name="currentOrientation">Current cardinal point which the turtle is heading</param>
        /// <returns>Next cardinal point in clockwise order</returns>
        public static CardinalPointType? Next(CardinalPointType? currentOrientation)
        {
            if (currentOrientation == null) return null; 

            IList<CardinalPointType> sortedCardinalPoints = CardinalPointList;

            int currentOrientationIndex = sortedCardinalPoints.IndexOf((CardinalPointType)currentOrientation);
            int nextOrientationIndex = (currentOrientationIndex + 1) % (sortedCardinalPoints.Count);

            return sortedCardinalPoints[nextOrientationIndex];
        }


    }
}