using System;
using System.Collections.Generic;
using static JavierCacho.EducationalProjects.AspNetCore.TurtleChallenge.Model.CardinalPoint;

namespace JavierCacho.EducationalProjects.AspNetCore.TurtleChallenge.Model
{
    /// <summary>
    /// Represents an static position on the board
    /// </summary>
    public class Pose
    {
        public int HorizontalPosition { get; set; }
        public int VerticalPosition { get; set; }
        public CardinalPointType? Orientation { get; set; }
    }
}