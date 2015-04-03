# region Includes

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RobX.Library.Commons;

# endregion

namespace RobX.Simulator
{
    /// <summary>
    /// This class draws simulation frames using Microsoft XNA Framework 4.0
    /// </summary>
    public class Drawer
    {
        # region Private Variables

        /// <summary>
        /// Total elapsed time of the game.
        /// </summary>
        private TimeSpan TotalTime = TimeSpan.Zero;

        # endregion

        # region Private Functions: Partial Draw Functions

        /// <summary>
        /// Draw ground of the environment
        /// </summary>
        /// <param name="Frame">SpriteBatch instance of the current frame.</param>
        /// <param name="Color">Color of the ground.</param>
        /// <param name="GroundWidth">Ground width in millimeters.</param>
        /// <param name="GroundHeight">Ground height in millimeters.</param>
        /// <param name="xscale">Conversion scale of ground width to screen width (converts millimeters to pixels).</param>
        /// <param name="yscale">Conversion scale of ground height to screen height (converts millimeters to pixels).</param>
        private void DrawGround(SpriteBatch Frame, Color Color, int GroundWidth, int GroundHeight, double xscale, double yscale)
        {
            GraphicsEngine.FillRect(Frame, new Rectangle(0, 0, (int)(GroundWidth * xscale), (int)(GroundHeight * yscale)), Color);
        }

        /// <summary>
        /// Draws vertical and horizontal grids on the screen.
        /// </summary>
        /// <param name="Frame">SpriteBatch instance of the current frame.</param>
        /// <param name="gridColor">Color of the grid lines.</param>
        /// <param name="fontColor">Color of the font.</param>
        /// <param name="ScreenWidth">Screen width in millimeters.</param>
        /// <param name="ScreenHeight">Screen height in millimeters.</param>
        /// <param name="XCenter">X position of (0, 0) on screen in millimeters.</param>
        /// <param name="YCenter">Y position of (0, 0) on screen in millimeters.</param>
        /// <param name="xscale">Conversion scale of ground width to screen width (converts millimeters to pixels).</param>
        /// <param name="yscale">Conversion scale of ground height to screen height (converts millimeters to pixels).</param>
        /// <param name="LineWidth">Width of grid lines.</param>
        /// <param name="VerticalGridDistance">Distance between vertical grids in millimeters.</param>
        /// <param name="HorizontalGridDistance">Distance between horizontal grids in millimeters.</param>
        private void DrawGrids(SpriteBatch Frame, Color gridColor, Color fontColor, int ScreenWidth, int ScreenHeight, int XCenter, int YCenter,
            double xscale, double yscale, int LineWidth = 1, int VerticalGridDistance = 500, int HorizontalGridDistance = 500)
        {
            // Pattern of the dashed lines of the grids
            var Pattern = new int[] { 3, 5 };

            // Calculate distance between vertical grid lines
            var VerticalLines = ScreenWidth / VerticalGridDistance;

            // Draw vertical grid lines
            for (var i = -VerticalLines; i <= VerticalLines; ++i)
            {
                var dist = i * VerticalGridDistance;
                var distm = (double)dist / 1000; // Convert millimeters to meters

                var linex = (int)(xscale * ConvertX(dist, XCenter));
                Frame.DrawVerticalLine(linex, 0, (int)(ScreenHeight * yscale), gridColor, LineWidth, Pattern);
                Frame.DrawString((distm).ToString("0.##") + " m", fontColor, GraphicsEngine.GridFont, linex, 0);
            }

            // Calculate distance between horizontal grid lines
            var HorizontalLines = ScreenHeight / HorizontalGridDistance;

            // Draw horizontal grid lines
            for (var i = -HorizontalLines; i <= HorizontalLines; ++i)
            {
                var dist = i * HorizontalGridDistance;
                var distm = (double)dist / 1000; // Convert millimeters to meters

                var liney = (int)(yscale * ConvertY(dist, YCenter));
                Frame.DrawHorizontalLine(0, (int)(ScreenWidth * xscale), liney, gridColor, LineWidth, Pattern);
                Frame.DrawString((distm).ToString("0.##") + " m", fontColor, GraphicsEngine.GridFont, 0, liney);
            }
        }

        /// <summary>
        /// Draws the robot on the screen.
        /// </summary>
        /// <param name="Frame">SpriteBatch instance of the current frame.</param>
        /// <param name="Env">Environment variable that contains simulation environment settings.</param>
        /// <param name="Robot"> Robot that should be drawn.</param>
        /// <param name="XCenter">X position of (0, 0) on screen in millimeters.</param>
        /// <param name="YCenter">Y position of (0, 0) on screen in millimeters.</param>
        /// <param name="xscale">Conversion scale of ground width to screen width (converts millimeters to pixels).</param>
        /// <param name="yscale">Conversion scale of ground height to screen height (converts millimeters to pixels).</param>
        private void DrawRobot(SpriteBatch Frame, Environment Env, Robot Robot, int XCenter, int YCenter,
            double xscale = 1.0F, double yscale = 1.0F)
        {
            // Rectangle that the robot should fit in
            var DestRect = new Rectangle((int)(xscale * (ConvertX((int)Env.Robot.X, XCenter) - Library.Commons.Robot.Radius)),
                (int)(yscale * (ConvertY((int)Env.Robot.Y, YCenter) - Library.Commons.Robot.Radius)),
                (int)(xscale * 2 * Library.Commons.Robot.Radius), (int)(yscale * 2 * Library.Commons.Robot.Radius));

            // Calculate integer angle of the robot
            var AngleIndex = ((int)(ConvertAngle(Env.Robot.Angle)) % 360 + 360) % 360;
            
            // Draw the robot
            Frame.Draw(Robot.Image[AngleIndex], DestRect, Color.White);
        }

        /// <summary>
        /// Draws robot traces on the screen
        /// </summary>
        /// <param name="Frame">SpriteBatch instance of the current frame.</param>
        /// <param name="Env">Environment variable that contains simulation environment settings.</param>
        /// <param name="XCenter">X position of (0, 0) on screen in millimeters.</param>
        /// <param name="YCenter">Y position of (0, 0) on screen in millimeters.</param>
        /// <param name="color">Color of the robot trace.</param>
        /// <param name="xscale">Conversion scale of ground width to screen width (converts millimeters to pixels).</param>
        /// <param name="yscale">Conversion scale of ground height to screen height (converts millimeters to pixels).</param>
        private void DrawRobotTrace(SpriteBatch Frame, Environment Env, int XCenter, int YCenter, Color color,
            double xscale = 1.0F, double yscale = 1.0F)
        {
            // Return if there are not enough points to draw trace
            if (Env.Robot.Trace.Count < 2) return;

            // Calculate scaled positions of the trace points
            var points = new Vector2[Env.Robot.Trace.Count];
            for (var i = 0; i < points.Length; ++i)
            {
                points[i].X = (float)(xscale * ConvertX((int)Env.Robot.Trace[i].X, XCenter));
                points[i].Y = (float)(yscale * ConvertY((int)Env.Robot.Trace[i].Y, YCenter));
            }

            // Draw points
            Frame.DrawPath(points, color, 1);
        }

        /// <summary>
        /// Draws obstacles in simulation screen.
        /// </summary>
        /// <param name="Frame">SpriteBatch instance of the current frame.</param>
        /// <param name="Env">Environment variable that contains simulation environment settings.</param>
        /// <param name="XCenter">X position of (0, 0) on screen in millimeters.</param>
        /// <param name="YCenter">Y position of (0, 0) on screen in millimeters.</param>
        /// <param name="xscale">Conversion scale of ground width to screen width (converts millimeters to pixels).</param>
        /// <param name="yscale">Conversion scale of ground height to screen height (converts millimeters to pixels).</param>
        private void DrawObstacles(SpriteBatch Frame, Environment Env, int XCenter, int YCenter,
            double xscale = 1.0F, double yscale = 1.0F)
        {
            for (var i = 0; i < Env.Obstacles.Count; ++i)
            {
                // Calculate scaled properties of the obstacle
                var obs = Env.Obstacles[i];
                Vector2[] points = null;
                var Rect = new Rectangle();
                var color = (Color)obs.Color;

                if (obs.Type == Obstacle.ObstacleType.RectangleFilled ||
                    obs.Type == Obstacle.ObstacleType.RectangleBorder)
                {
                    Rect = new Rectangle((int)(xscale * ConvertX(obs.Rectangle.X, XCenter)),
                        (int)(yscale * (ConvertY(obs.Rectangle.Y, YCenter) - obs.Rectangle.Height)),
                        (int)(xscale * obs.Rectangle.Width),
                        (int)(yscale * obs.Rectangle.Height));

                    if (obs.IsIntersected(Env.Robot.X, Env.Robot.Y, Library.Commons.Robot.Radius))
                        color = obs.CollisionColor;
                }
                else
                {
                    points = new Vector2[obs.Points.Length];
                    for (var p = 0; p < points.Length; ++i)
                    {
                        points[p].X = (float)(xscale * ConvertX((int)obs.Points[p].X, XCenter));
                        points[p].Y = (float)(yscale * ConvertY((int)obs.Points[p].Y, YCenter));
                    }
                }

                // Draw obstacle
                if (obs.Type == Obstacle.ObstacleType.RectangleFilled)
                    Frame.FillRect(Rect, color);
                else if (obs.Type == Obstacle.ObstacleType.RectangleBorder)
                    Frame.DrawRect(Rect, color, obs.BorderWidth);
                else if (obs.Type == Obstacle.ObstacleType.Polygon)
                    Frame.DrawPolygon(obs.Points, color, obs.BorderWidth);
            }
        }

        /// <summary>
        /// Draws statistics (i.e. simulation time, etc.) on the screen.
        /// </summary>
        /// <param name="Frame">SpriteBatch instance of the current frame.</param>
        /// <param name="Env">Environment variable that contains simulation environment settings.</param>
        /// <param name="Robot">Robot instance.</param>
        /// <param name="color">Color of the text</param>
        /// <param name="TotalTime">Total elapsed time of the simulation.</param>
        /// <param name="xscale">Conversion scale of ground width to screen width (converts millimeters to pixels).</param>
        /// <param name="yscale">Conversion scale of ground height to screen height (converts millimeters to pixels).</param>
        private void DrawStatistics(SpriteBatch Frame, Environment Env, Robot Robot, Color color,
            TimeSpan TotalTime, double xscale = 1.0F, double yscale = 1.0F)
        {
            // Calculate screen dimensions
            var ScreenWidth = (int)(Env.Ground.Width * xscale);
            var ScreenHeight = (int)(Env.Ground.Height * yscale);

            // ------------ Print simulation time in seconds ----------------
            var DurationInMilliSecs = (int)(TotalTime.TotalMilliseconds);

            var str = "Simulation time: " + (DurationInMilliSecs / 1000F).ToString("0.000") + " s";
            var textrect = Frame.DrawString(str, color, GraphicsEngine.StatisticsFont,
                ScreenWidth, ScreenHeight, GraphicsEngine.HorizontalTextAlign.Right, GraphicsEngine.VerticalTextAlign.Bottom);

            // ------------ Print frame rate in fps -------------------
            str = "Frame rate: " + SimController.DrawFPS + " fps";
            textrect = Frame.DrawString(str, color, GraphicsEngine.StatisticsFont, ScreenWidth, textrect.Top,
                GraphicsEngine.HorizontalTextAlign.Right, GraphicsEngine.VerticalTextAlign.Bottom);

            // ------------ Print update rate in Hz -------------------
            str = "Update rate: " + SimController.UpdateRate + " Hz";
            textrect = Frame.DrawString(str, color, GraphicsEngine.StatisticsFont, ScreenWidth, textrect.Top,
                GraphicsEngine.HorizontalTextAlign.Right, GraphicsEngine.VerticalTextAlign.Bottom);

            // ------------ Print robot angle in degrees -------------------
            var Angl = ((Env.Robot.Angle + 360) % 360);
            if (Angl > 180) Angl -= 360;

            str = "Robot angle: " + Angl.ToString("0.0") + " degs";
            textrect = Frame.DrawString(str, color, GraphicsEngine.StatisticsFont, ScreenWidth, textrect.Top,
                GraphicsEngine.HorizontalTextAlign.Right, GraphicsEngine.VerticalTextAlign.Bottom);

            // ------------ Print robot position in meters -------------------
            str = "Robot pos: (" + (Env.Robot.X / 1000).ToString("0.00") + ", " +
                (Env.Robot.Y / 1000).ToString("0.00") + ") m";
            textrect = Frame.DrawString(str, color, GraphicsEngine.StatisticsFont, ScreenWidth, textrect.Top,
                GraphicsEngine.HorizontalTextAlign.Right, GraphicsEngine.VerticalTextAlign.Bottom);

            // ------------ Print robot encoder counts -------------------
            str = "Encoders: (" + Robot.Encoder1 + ", " + Robot.Encoder2 + ")";
            textrect = Frame.DrawString(str, color, GraphicsEngine.StatisticsFont, ScreenWidth, textrect.Top,
                GraphicsEngine.HorizontalTextAlign.Right, GraphicsEngine.VerticalTextAlign.Bottom);

            // ------------ Print Simulation Speed -------------------
            str = Simulator.SimulationSpeed.ToString("0.0") + "x";
            Frame.DrawString(str, Properties.Settings.Default.SimulationSpeedColor, GraphicsEngine.SimulationSpeedFont, 30, 
                ScreenHeight, GraphicsEngine.HorizontalTextAlign.Left, GraphicsEngine.VerticalTextAlign.Bottom);
        }

        # endregion

        # region Private Function: Render Subframe

        /// <summary>
        /// Renders next subframe of the simulation.
        /// </summary>
        /// <param name="SubFrame">SpriteBatch instance of the current frame.</param>
        /// <param name="ScreenWidth">Screen width in pixels.</param>
        /// <param name="ScreenHeight">Screen height in pixels.</param>
        /// <param name="Environment">Environment variable that contains simulation environment settings.</param>
        /// <param name="Robot">Robot instance.</param>
        /// <param name="RenderType">Rendering type of the subframe.</param>
        /// <param name="xscale">Conversion scale of ground width to screen width (converts millimeters to pixels).</param>
        /// <param name="yscale">Conversion scale of ground height to screen height (converts millimeters to pixels).</param>
        private void RenderSubframe(ref SpriteBatch SubFrame, int ScreenWidth, int ScreenHeight,
            ref Environment Environment, Robot Robot, 
            Simulator.RenderOptions.RenderType RenderType = Simulator.RenderOptions.RenderType.StaticAxis_ZeroCentered, 
            double xscale = 1.0F, double yscale = 1.0F)
        {
            // Calculate offset of the center of the screen (in millimeters)
            int xoffset, yoffset;
            if (RenderType == Simulator.RenderOptions.RenderType.StaticAxis_ZeroCentered)
            {
                xoffset = Environment.Ground.Width / 2;
                yoffset = Environment.Ground.Height / 2;
            }
            else // if (RenderType == Simulator.RenderOptions.RenderType.StaticAxis_ZeroCornered)
            {
                xoffset = 0;
                yoffset = Environment.Ground.Height;
            }

            try
            {
                // Draw ground
                DrawGround(SubFrame, Properties.Settings.Default.GroundColor, Environment.Ground.Width, Environment.Ground.Height, xscale, yscale);

                // Draw obstacles
                if (Properties.Settings.Default.DrawObstacles)
                    DrawObstacles(SubFrame, Environment, xoffset, yoffset, xscale, yscale);

                // Draw grids
                if (Properties.Settings.Default.DrawGrids)
                    DrawGrids(SubFrame, Properties.Settings.Default.GridLineColor, Properties.Settings.Default.GridFontColor,
                        Environment.Ground.Width, Environment.Ground.Height, xoffset, yoffset, xscale, yscale);

                // Draw robot traces
                if (Properties.Settings.Default.DrawRobotTrace)
                    DrawRobotTrace(SubFrame, Environment, xoffset, yoffset, Properties.Settings.Default.RobotTraceColor, xscale, yscale);

                // Draw robot
                DrawRobot(SubFrame, Environment, Robot, xoffset, yoffset, xscale, yscale);
            }
            catch { }
        }

        # endregion

        # region Private Functions: Correction of X, Y and Angle

        /// <summary>
        /// Convert an x position to place in ground
        /// </summary>
        /// <param name="X">X position</param>
        /// <param name="CenterX">X position of (0,0)</param>
        /// <returns>Converted x position</returns>
        private int ConvertX(int X, int CenterX) { return CenterX + X; }

        /// <summary>
        /// Convert a y position to place in ground
        /// </summary>
        /// <param name="Y">Y position</param>
        /// <param name="CenterY">Y position of (0,0)</param>
        /// <returns>Converted y position</returns>
        private int ConvertY(int Y, int CenterY) { return CenterY - Y; }

        /// <summary>
        /// Convert an angle to angle in drawing
        /// </summary>
        /// <param name="Angle">Angle in degres or radians</param>
        /// <returns>Converted angle</returns>
        private double ConvertAngle(double Angle) { return -Angle; }

        # endregion

        # region Public Function: Render Frame

        /// <summary>
        /// Renders next frame of simulation.
        /// </summary>
        /// <param name="Frame">SpriteBatch instance of the current frame.</param>
        /// <param name="ScreenWidth">Screen width in pixels.</param>
        /// <param name="ScreenHeight">Screen height in pixels.</param>
        /// <param name="Environment">Environment variable that contains simulation environment settings.</param>
        /// <param name="Robot">Robot instance.</param>
        /// <param name="ElapsedTime">The time elapsed since the previous frame.</param>
        /// <param name="RenderType">Rendering type of the frame.</param>
        public void Render(ref SpriteBatch Frame, int ScreenWidth, int ScreenHeight, ref Environment Environment,
            Robot Robot, TimeSpan ElapsedTime,
            Simulator.RenderOptions.RenderType RenderType = Simulator.RenderOptions.RenderType.StaticAxis_ZeroCentered)
        {
            try
            {
                // Calculate the total time of the simulation
                TotalTime += TimeSpan.FromTicks((long)(ElapsedTime.Ticks * Simulator.SimulationSpeed));

                // Calculate conversion scales from real-world to screen dimensions
                var xscale = (double)ScreenWidth / Environment.Ground.Width;
                var yscale = (double)ScreenHeight / Environment.Ground.Height;

                // Render subframe
                RenderSubframe(ref Frame, ScreenWidth, ScreenHeight, ref Environment, Robot, RenderType, xscale, yscale);

                // Draw statistics (simulation time, etc.)
                if (Properties.Settings.Default.DrawStatistics)
                    DrawStatistics(Frame, Environment, Robot, Properties.Settings.Default.StatisticsFontColor, TotalTime, xscale, yscale);
            }
            catch { }

        }

        # endregion
    }
}
