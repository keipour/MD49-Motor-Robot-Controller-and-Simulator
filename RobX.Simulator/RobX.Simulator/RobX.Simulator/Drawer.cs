# region Includes

using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RobX.Library.Commons;
using RobX.Simulator.Properties;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

# endregion

namespace RobX.Simulator
{
    /// <summary>
    /// This class draws simulation frames using Microsoft XNA Framework 4.0.
    /// </summary>
    public class Drawer
    {
        # region Private Variables

        /// <summary>
        /// Total elapsed time of the game.
        /// </summary>
        private TimeSpan _totalTime = TimeSpan.Zero;

        # endregion

        # region Private Functions: Partial Draw Functions

        /// <summary>
        /// Draw ground of the environment
        /// </summary>
        /// <param name="frame">SpriteBatch instance of the current frame.</param>
        /// <param name="color">Color of the ground.</param>
        /// <param name="groundWidth">Ground width in millimeters.</param>
        /// <param name="groundHeight">Ground height in millimeters.</param>
        /// <param name="xscale">Conversion scale of ground width to screen width (converts millimeters to pixels).</param>
        /// <param name="yscale">Conversion scale of ground height to screen height (converts millimeters to pixels).</param>
        private static void DrawGround(SpriteBatch frame, Color color, int groundWidth, int groundHeight, double xscale, double yscale)
        {
            frame.FillRect(new Rectangle(0, 0, (int)(groundWidth * xscale), (int)(groundHeight * yscale)), color);
        }

        /// <summary>
        /// Draws vertical and horizontal grids on the screen.
        /// </summary>
        /// <param name="frame">SpriteBatch instance of the current frame.</param>
        /// <param name="gridColor">Color of the grid lines.</param>
        /// <param name="fontColor">Color of the font.</param>
        /// <param name="screenWidth">Screen width in millimeters.</param>
        /// <param name="screenHeight">Screen height in millimeters.</param>
        /// <param name="xCenter">X position of (0, 0) on screen in millimeters.</param>
        /// <param name="yCenter">Y position of (0, 0) on screen in millimeters.</param>
        /// <param name="xscale">Conversion scale of ground width to screen width (converts millimeters to pixels).</param>
        /// <param name="yscale">Conversion scale of ground height to screen height (converts millimeters to pixels).</param>
        /// <param name="lineWidth">Width of grid lines.</param>
        /// <param name="verticalGridDistance">Distance between vertical grids in millimeters.</param>
        /// <param name="horizontalGridDistance">Distance between horizontal grids in millimeters.</param>
        private static void DrawGrids(SpriteBatch frame, Color gridColor, Color fontColor, int screenWidth, int screenHeight, int xCenter, int yCenter,
            double xscale, double yscale, int lineWidth = 1, int verticalGridDistance = 500, int horizontalGridDistance = 500)
        {
            // Pattern of the dashed lines of the grids
            var pattern = new[] { 3, 5 };

            // Calculate distance between vertical grid lines
            var verticalLines = screenWidth / verticalGridDistance;

            // Draw vertical grid lines
            for (var i = -verticalLines; i <= verticalLines; ++i)
            {
                var dist = i * verticalGridDistance;
                var distm = (double)dist / 1000; // Convert millimeters to meters

                var linex = (int)(xscale * ConvertX(dist, xCenter));
                frame.DrawVerticalLine(linex, 0, (int)(screenHeight * yscale), gridColor, lineWidth, pattern);
                frame.DrawString((distm).ToString("0.##") + " m", fontColor, GraphicsEngine.GridFont, linex);
            }

            // Calculate distance between horizontal grid lines
            var horizontalLines = screenHeight / horizontalGridDistance;

            // Draw horizontal grid lines
            for (var i = -horizontalLines; i <= horizontalLines; ++i)
            {
                var dist = i * horizontalGridDistance;
                var distm = (double)dist / 1000; // Convert millimeters to meters

                var liney = (int)(yscale * ConvertY(dist, yCenter));
                frame.DrawHorizontalLine(0, (int)(screenWidth * xscale), liney, gridColor, lineWidth, pattern);
                frame.DrawString((distm).ToString("0.##") + " m", fontColor, GraphicsEngine.GridFont, 0, liney);
            }
        }

        /// <summary>
        /// Draws the robot on the screen.
        /// </summary>
        /// <param name="frame">SpriteBatch instance of the current frame.</param>
        /// <param name="env">Environment variable that contains simulation environment settings.</param>
        /// <param name="robot"> Robot that should be drawn.</param>
        /// <param name="xCenter">X position of (0, 0) on screen in millimeters.</param>
        /// <param name="yCenter">Y position of (0, 0) on screen in millimeters.</param>
        /// <param name="xscale">Conversion scale of ground width to screen width (converts millimeters to pixels).</param>
        /// <param name="yscale">Conversion scale of ground height to screen height (converts millimeters to pixels).</param>
        private static void DrawRobot(SpriteBatch frame, Environment env, Robot robot, int xCenter, int yCenter,
            double xscale = 1.0F, double yscale = 1.0F)
        {
            // Rectangle that the robot should fit in
            var destRect = new Rectangle((int)(xscale * (ConvertX((int)env.Robot.X, xCenter) - Library.Robot.Robot.Radius)),
                (int)(yscale * (ConvertY((int)env.Robot.Y, yCenter) - Library.Robot.Robot.Radius)),
                (int)(xscale * 2 * Library.Robot.Robot.Radius), (int)(yscale * 2 * Library.Robot.Robot.Radius));

            // Calculate integer angle of the robot
            var angleIndex = ((int)(ConvertAngle(env.Robot.Angle)) % 360 + 360) % 360;
            
            // Draw the robot
            frame.Draw(robot.Image[angleIndex], destRect, Color.White);
        }

        /// <summary>
        /// Draws robot traces on the screen
        /// </summary>
        /// <param name="frame">SpriteBatch instance of the current frame.</param>
        /// <param name="env">Environment variable that contains simulation environment settings.</param>
        /// <param name="xCenter">X position of (0, 0) on screen in millimeters.</param>
        /// <param name="yCenter">Y position of (0, 0) on screen in millimeters.</param>
        /// <param name="color">Color of the robot trace.</param>
        /// <param name="xscale">Conversion scale of ground width to screen width (converts millimeters to pixels).</param>
        /// <param name="yscale">Conversion scale of ground height to screen height (converts millimeters to pixels).</param>
        private static void DrawRobotTrace(SpriteBatch frame, Environment env, int xCenter, int yCenter, Color color,
            double xscale = 1.0F, double yscale = 1.0F)
        {
            // Return if there are not enough points to draw trace
            if (env.Robot.Trace.Count < 2) return;

            // Calculate scaled positions of the trace points
            var points = new PointF[env.Robot.Trace.Count];
            for (var i = 0; i < points.Length; ++i)
            {
                points[i].X = (float)(xscale * ConvertX((int)env.Robot.Trace[i].X, xCenter));
                points[i].Y = (float)(yscale * ConvertY((int)env.Robot.Trace[i].Y, yCenter));
            }

            // Draw points
            frame.DrawPath(points, color);
        }

        /// <summary>
        /// Draws obstacles in simulation screen.
        /// </summary>
        /// <param name="frame">SpriteBatch instance of the current frame.</param>
        /// <param name="env">Environment variable that contains simulation environment settings.</param>
        /// <param name="xCenter">X position of (0, 0) on screen in millimeters.</param>
        /// <param name="yCenter">Y position of (0, 0) on screen in millimeters.</param>
        /// <param name="xscale">Conversion scale of ground width to screen width (converts millimeters to pixels).</param>
        /// <param name="yscale">Conversion scale of ground height to screen height (converts millimeters to pixels).</param>
        private static void DrawObstacles(SpriteBatch frame, Environment env, int xCenter, int yCenter,
            double xscale = 1.0F, double yscale = 1.0F)
        {
            for (var i = 0; i < env.Obstacles.Count; ++i)
            {
                // Calculate scaled properties of the obstacle
                var obs = env.Obstacles[i];
                var rect = new Rectangle();
                var color = GraphicsEngine.SystemColorToXna(obs.Color);

                if (obs.Type == Obstacle.ObstacleType.RectangleFilled ||
                    obs.Type == Obstacle.ObstacleType.RectangleBorder)
                {
                    rect = new Rectangle((int)(xscale * ConvertX(obs.Rectangle.X, xCenter)),
                        (int)(yscale * (ConvertY(obs.Rectangle.Y, yCenter) - obs.Rectangle.Height)),
                        (int)(xscale * obs.Rectangle.Width),
                        (int)(yscale * obs.Rectangle.Height));

                    if (obs.IsIntersected(env.Robot.X, env.Robot.Y, Library.Robot.Robot.Radius))
                        color = GraphicsEngine.SystemColorToXna(obs.CollisionColor);
                }
                else
                {
                    var points = new Vector2[obs.Points.Length];
                    for (var p = 0; p < points.Length; ++i)
                    {
                        points[p].X = (float)(xscale * ConvertX((int)obs.Points[p].X, xCenter));
                        points[p].Y = (float)(yscale * ConvertY((int)obs.Points[p].Y, yCenter));
                    }
                }

                // Draw obstacle
                switch (obs.Type)
                {
                    case Obstacle.ObstacleType.RectangleFilled:
                        frame.FillRect(rect, color);
                        break;
                    case Obstacle.ObstacleType.RectangleBorder:
                        frame.DrawRect(rect, color, (int)(obs.BorderWidth * yscale), (int)(obs.BorderWidth * xscale));
                        break;
                    case Obstacle.ObstacleType.Polygon:
                        frame.DrawPolygon(obs.Points, color, obs.BorderWidth);
                        break;
                }
            }
        }

        /// <summary>
        /// Draws statistics (i.e. simulation time, etc.) on the screen.
        /// </summary>
        /// <param name="frame">SpriteBatch instance of the current frame.</param>
        /// <param name="env">Environment variable that contains simulation environment settings.</param>
        /// <param name="robot">Robot instance.</param>
        /// <param name="color">Color of the text</param>
        /// <param name="totalTime">Total elapsed time of the simulation.</param>
        /// <param name="xscale">Conversion scale of ground width to screen width (converts millimeters to pixels).</param>
        /// <param name="yscale">Conversion scale of ground height to screen height (converts millimeters to pixels).</param>
        private static void DrawStatistics(SpriteBatch frame, Environment env, Robot robot, Color color,
            TimeSpan totalTime, double xscale = 1.0F, double yscale = 1.0F)
        {
            // Calculate screen dimensions
            var screenWidth = (int)(env.Ground.Width * xscale);
            var screenHeight = (int)(env.Ground.Height * yscale);

            // ------------ Print simulation time in seconds ----------------
            var durationInMilliSecs = (int)(totalTime.TotalMilliseconds);

            var str = "Simulation time: " + (durationInMilliSecs / 1000F).ToString("0.000") + " s";
            var textrect = frame.DrawString(str, color, GraphicsEngine.StatisticsFont,
                screenWidth, screenHeight, GraphicsEngine.HorizontalTextAlign.Right, GraphicsEngine.VerticalTextAlign.Bottom);

            // ------------ Print frame rate in fps -------------------
            str = "Frame rate: " + SimController.DrawFps + " fps";
            textrect = frame.DrawString(str, color, GraphicsEngine.StatisticsFont, screenWidth, textrect.Top,
                GraphicsEngine.HorizontalTextAlign.Right, GraphicsEngine.VerticalTextAlign.Bottom);

            // ------------ Print update rate in Hz -------------------
            str = "Update rate: " + SimController.UpdateRate + " Hz";
            textrect = frame.DrawString(str, color, GraphicsEngine.StatisticsFont, screenWidth, textrect.Top,
                GraphicsEngine.HorizontalTextAlign.Right, GraphicsEngine.VerticalTextAlign.Bottom);

            // ------------ Print robot angle in degrees -------------------
            var angl = ((env.Robot.Angle + 360) % 360);
            if (angl > 180) angl -= 360;

            str = "Robot angle: " + angl.ToString("0.0") + " degs";
            textrect = frame.DrawString(str, color, GraphicsEngine.StatisticsFont, screenWidth, textrect.Top,
                GraphicsEngine.HorizontalTextAlign.Right, GraphicsEngine.VerticalTextAlign.Bottom);

            // ------------ Print robot position in meters -------------------
            str = "Robot pos: (" + (env.Robot.X / 1000).ToString("0.00") + ", " +
                (env.Robot.Y / 1000).ToString("0.00") + ") m";
            textrect = frame.DrawString(str, color, GraphicsEngine.StatisticsFont, screenWidth, textrect.Top,
                GraphicsEngine.HorizontalTextAlign.Right, GraphicsEngine.VerticalTextAlign.Bottom);

            // ------------ Print robot encoder counts -------------------
            str = "Encoders: (" + robot.Encoder1 + ", " + robot.Encoder2 + ")";
            frame.DrawString(str, color, GraphicsEngine.StatisticsFont, screenWidth, textrect.Top,
                GraphicsEngine.HorizontalTextAlign.Right, GraphicsEngine.VerticalTextAlign.Bottom);

            // ------------ Print Simulation Speed -------------------
            str = Simulator.SimulationSpeed.ToString("0.0") + "x";
            frame.DrawString(str, Settings.Default.SimulationSpeedColor, GraphicsEngine.SimulationSpeedFont, 30, 
                screenHeight, GraphicsEngine.HorizontalTextAlign.Left, GraphicsEngine.VerticalTextAlign.Bottom);
        }

        # endregion

        # region Private Function: Render Subframe

        /// <summary>
        /// Renders next subframe of the simulation.
        /// </summary>
        /// <param name="subFrame">SpriteBatch instance of the current frame.</param>
        /// <param name="environment">Environment variable that contains simulation environment settings.</param>
        /// <param name="robot">Robot instance.</param>
        /// <param name="renderType">Rendering type of the subframe.</param>
        /// <param name="xscale">Conversion scale of ground width to screen width (converts millimeters to pixels).</param>
        /// <param name="yscale">Conversion scale of ground height to screen height (converts millimeters to pixels).</param>
        private static void RenderSubframe(ref SpriteBatch subFrame, ref Environment environment, Robot robot, 
            Simulator.RenderOptions.RenderType renderType = Simulator.RenderOptions.RenderType.StaticAxisZeroCentered, 
            double xscale = 1.0F, double yscale = 1.0F)
        {
            // Calculate offset of the center of the screen (in millimeters)
            int xoffset, yoffset;
            if (renderType == Simulator.RenderOptions.RenderType.StaticAxisZeroCentered)
            {
                xoffset = environment.Ground.Width / 2;
                yoffset = environment.Ground.Height / 2;
            }
            else // if (RenderType == Simulator.RenderOptions.RenderType.StaticAxis_ZeroCornered)
            {
                xoffset = 0;
                yoffset = environment.Ground.Height;
            }

            try
            {
                // Draw ground
                DrawGround(subFrame, Settings.Default.GroundColor, environment.Ground.Width, environment.Ground.Height, xscale, yscale);

                // Draw obstacles
                if (Settings.Default.DrawObstacles)
                    DrawObstacles(subFrame, environment, xoffset, yoffset, xscale, yscale);

                // Draw grids
                if (Settings.Default.DrawGrids)
                    DrawGrids(subFrame, Settings.Default.GridLineColor, Settings.Default.GridFontColor,
                        environment.Ground.Width, environment.Ground.Height, xoffset, yoffset, xscale, yscale);

                // Draw robot traces
                if (Settings.Default.DrawRobotTrace)
                    DrawRobotTrace(subFrame, environment, xoffset, yoffset, Settings.Default.RobotTraceColor, xscale, yscale);

                // Draw robot
                DrawRobot(subFrame, environment, robot, xoffset, yoffset, xscale, yscale);
            }
            catch
            {
                // ignored
            }
        }

        # endregion

        # region Private Functions: Correction of X, Y and Angle

        /// <summary>
        /// Convert an x position to place in ground
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="centerX">X position of (0,0)</param>
        /// <returns>Converted x position</returns>
        private static int ConvertX(int x, int centerX) { return centerX + x; }

        /// <summary>
        /// Convert a y position to place in ground
        /// </summary>
        /// <param name="y">Y position</param>
        /// <param name="centerY">Y position of (0,0)</param>
        /// <returns>Converted y position</returns>
        private static int ConvertY(int y, int centerY) { return centerY - y; }

        /// <summary>
        /// Convert an angle to angle in drawing
        /// </summary>
        /// <param name="angle">Angle in degres or radians</param>
        /// <returns>Converted angle</returns>
        private static double ConvertAngle(double angle) { return -angle; }

        # endregion

        # region Public Function: Render Frame

        /// <summary>
        /// Renders next frame of simulation.
        /// </summary>
        /// <param name="frame">SpriteBatch instance of the current frame.</param>
        /// <param name="screenWidth">Screen width in pixels.</param>
        /// <param name="screenHeight">Screen height in pixels.</param>
        /// <param name="environment">Environment variable that contains simulation environment settings.</param>
        /// <param name="robot">Robot instance.</param>
        /// <param name="elapsedTime">The time elapsed since the previous frame.</param>
        /// <param name="renderType">Rendering type of the frame.</param>
        public void Render(ref SpriteBatch frame, int screenWidth, int screenHeight, ref Environment environment,
            Robot robot, TimeSpan elapsedTime,
            Simulator.RenderOptions.RenderType renderType = Simulator.RenderOptions.RenderType.StaticAxisZeroCentered)
        {
            try
            {
                // Calculate the total time of the simulation
                _totalTime += TimeSpan.FromTicks((long)(elapsedTime.Ticks * Simulator.SimulationSpeed));

                // Calculate conversion scales from real-world to screen dimensions
                var xscale = (double)screenWidth / environment.Ground.Width;
                var yscale = (double)screenHeight / environment.Ground.Height;

                // Render subframe
                RenderSubframe(ref frame, ref environment, robot, renderType, xscale, yscale);

                // Draw statistics (simulation time, etc.)
                if (Settings.Default.DrawStatistics)
                    DrawStatistics(frame, environment, robot, Settings.Default.StatisticsFontColor, _totalTime, xscale, yscale);
            }
            catch
            {
                // ignored
            }
        }

        # endregion
    }
}
