﻿# region Includes

using System.Collections.Generic;
using System.Drawing;
using RobX.Library.Commons;

# endregion

namespace RobX.Simulator
{
    /// <summary>
    /// Class containing simulation envitonment parameters (obstacles, robot position, etc.).
    /// </summary>
    public class Environment
    {
        # region Public Variables 

        /// <summary>
        /// Variable for Robot settings.
        /// </summary>
        public readonly RobotProperties Robot = new RobotProperties();

        /// <summary>
        /// Variable for Ground settings.
        /// </summary>
        public readonly GroundProperties Ground = new GroundProperties();

        /// <summary>
        /// The list of all obstacles in the environment.
        /// </summary>
        public readonly List<Obstacle> Obstacles = new List<Obstacle>();

        # endregion

        # region Public Classes 

        /// <summary>
        /// Class for ground properties.
        /// </summary>
        public class GroundProperties
        {
            /// <summary>
            /// Default ground width in millimeters.
            /// </summary>
            private const int DefaultWidth = 5000;

            /// <summary>
            /// Default ground height in millimeters.
            /// </summary>
            private const int DefaultHeight = 5000;

            /// <summary>
            /// The width of the ground in millimeters.
            /// </summary>
            public int Width = DefaultWidth;

            /// <summary>
            /// The height of the ground in millimeters.
            /// </summary>
            public int Height = DefaultHeight;
        }

        /// <summary>
        /// Class for physical robot properties.
        /// </summary>
        public class RobotProperties
        {
            /// <summary>
            /// The vertical position of the robot in millimeters.
            /// </summary>
            public double X;

            /// <summary>
            /// The horizontal position of the robot in millimeters.
            /// </summary>
            public double Y;

            /// <summary>
            /// The robot angle (in degrees).
            /// </summary>
            public double Angle;

            /// <summary>
            /// The trace (path) of the robot in the simulation.
            /// </summary>
            public readonly List<PointF> Trace = new List<PointF>();
        }

        # endregion
    }
}
