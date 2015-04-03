# region Includes

using Microsoft.Xna.Framework;
using RobX.Library.Commons;

# endregion

namespace RobX.Simulator
{
    /// <summary>
    /// Defines all simulation settings set by the user
    /// </summary>
    public static class UserDefinitions
    {
        /// <summary>
        /// Defines all simulation settings set by the user
        /// </summary>
        /// <param name="sim">Simulator variable</param>
        public static void DefineSimulation(ref Simulator sim)
        {
            // Prof. Jamzad's Image Processing and Computer Vision Lab
            sim.Environment.Ground.Width = 6700;
            sim.Environment.Ground.Height = 7550;

            // Add obstacles to the environment
            sim.AddObstacle(new Obstacle(2300, 2730, 1600, 1600));
            sim.AddObstacle(new Obstacle(0, 0, 520, 900));
            sim.AddObstacle(new Obstacle(520, 0, 3370, 820));
            sim.AddObstacle(new Obstacle(3900, 0, 910, 390));
            sim.AddObstacle(new Obstacle(4840, 0, 480, 610));
            sim.AddObstacle(new Obstacle(4840 + 480, 0, 1500, 550));
            sim.AddObstacle(new Obstacle(0, 2070, 580, 525));
            sim.AddObstacle(new Obstacle(0, 2600, 890, 3300));
            sim.AddObstacle(new Obstacle(0, sim.Environment.Ground.Height - 1630, 1540, 1630));
            sim.AddObstacle(new Obstacle(1510, sim.Environment.Ground.Height - 380, 330, 380));
            sim.AddObstacle(new Obstacle(1840, sim.Environment.Ground.Height - 570, 510, 570));
            sim.AddObstacle(new Obstacle(1840 + 510, sim.Environment.Ground.Height - 430, 305, 430));
            sim.AddObstacle(new Obstacle(2570, sim.Environment.Ground.Height - 1630, 1500, 1630));
            sim.AddObstacle(new Obstacle(sim.Environment.Ground.Width - 2605, sim.Environment.Ground.Height - 555, 505, 555));
            sim.AddObstacle(new Obstacle(sim.Environment.Ground.Width - 2000, sim.Environment.Ground.Height - 1630, 2000, 1630));
            sim.AddObstacle(new Obstacle(sim.Environment.Ground.Width - 1640, sim.Environment.Ground.Height - 1830 - 1300, 1640, 1300));
            sim.AddObstacle(new Obstacle(sim.Environment.Ground.Width - 1700, 2310, 1700, 1300));
            sim.AddObstacle(new Obstacle(sim.Environment.Ground.Width - 650, 1530, 650, 600));
            sim.AddObstacle(new Obstacle(0, 0, sim.Environment.Ground.Width, sim.Environment.Ground.Height, false, 10));
            sim.AddObstacle(new Obstacle(sim.Environment.Ground.Width - 100, 210, 100, 1040, Color.Brown));

            // Set robot position
            sim.Environment.Robot.X = 2000;
            sim.Environment.Robot.Y = 5000;
            sim.Environment.Robot.Angle = 90;
        }
    }
}
