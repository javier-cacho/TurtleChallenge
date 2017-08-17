using System;
using System.Collections.Generic;

namespace JavierCacho.EducationalProjects.AspNetCore.TurtleChallenge.Model
{
    /// <summary>
    /// Represents a turtle movement along the board
    /// </summary>
    public static class Movement
    {
        public enum MovementType
        {
            Forward = 'm',
            Rotate = 'r'
        }

        public static int? GetDistanceMoved(MovementType movementType)
        {
            return movementType == MovementType.Forward ? 1 : 0;
        }

    }
}
