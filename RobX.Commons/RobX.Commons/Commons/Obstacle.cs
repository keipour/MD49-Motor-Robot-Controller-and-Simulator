# region Includes

using System;
using Microsoft.Xna.Framework;

# endregion

namespace RobX.Commons
{
    /// <summary>
    /// Class that represents an obstacle in the environment.
    /// </summary>
    public class Obstacle
    {
        # region Public Enums

        /// <summary>
        /// Shape types of an obstacle.
        /// </summary>
        public enum ObstacleType
        {
            /// <summary>
            /// The obstacle is a rectangle (not filled).
            /// </summary>
            RectangleBorder = 0,

            /// <summary>
            /// The obstacle is a filled rectangle.
            /// </summary>
            RectangleFilled = 1,

            /// <summary>
            /// The obstacle is a polygon border.
            /// </summary>
            Polygon = 2
        }

        # endregion

        # region Public Fields

        /// <summary>
        /// Contains type (shape) of the current obstacle.
        /// </summary>
        public ObstacleType Type = ObstacleType.RectangleFilled;

        /// <summary>
        /// Contains rectangle for the current obstacle (if type is rectangle, ellipse or pie).
        /// </summary>
        public Rectangle Rectangle = new Rectangle(0, 0, 0, 0);

        /// <summary>
        /// Contains points for the current obstacle (if type is Polygon).
        /// </summary>
        public Vector2[] Points = null;

        /// <summary>
        /// Color of the obstacle when there is no collision with robot.
        /// </summary>
        public Color Color = Color.LightSkyBlue;

        /// <summary>
        /// Color of the obstacle when there is collision with robot (works only for rectangle obstacles).
        /// </summary>
        public Color CollisionColor = Color.Red;
        
        /// <summary>
        /// Width of the border (not for FillRectangle).
        /// </summary>
        public int BorderWidth = 4;

        # endregion

        # region Public Functions: Set Shapes

        /// <summary>
        /// Set obstacle as a filled rectangle with the default color.
        /// </summary>
        /// <param name="x">X position of left border in millimeters.</param>
        /// <param name="y">Y position of upper border in millimeters.</param>
        /// <param name="width">Width of the rectangle in millimeters.</param>
        /// <param name="height">Height of the rectangle in millimeters.</param>
        public void SetRectangleFilled(int x, int y, int width, int height)
        {
            SetRectangleFilled(x, y, width, height, Color);
        }

        /// <summary>
        /// Set obstacle as a filled rectangle with a specified color.
        /// </summary>
        /// <param name="x">X position of left border in millimeters.</param>
        /// <param name="y">Y position of upper border in millimeters.</param>
        /// <param name="width">Width of the rectangle in millimeters.</param>
        /// <param name="height">Height of the rectangle in millimeters.</param>
        /// <param name="color">Color of the rectangle.</param>
        public void SetRectangleFilled(int x, int y, int width, int height, Color color)
        {
            Rectangle = new Rectangle(x, y, width, height);
            Type = ObstacleType.RectangleFilled;
            Color = color;
        }

        /// <summary>
        /// Set obstacle as a rectangle border with the default border color.
        /// </summary>
        /// <param name="x">X position of left border in millimeters.</param>
        /// <param name="y">Y position of upper border in millimeters.</param>
        /// <param name="width">Width of the rectangle in millimeters.</param>
        /// <param name="height">Height of the rectangle in millimeters.</param>
        /// <param name="borderwidth">Width of the border in pixels.</param>
        public void SetRectangleBorder(int x, int y, int width, int height, int borderwidth = 4)
        {
            SetRectangleBorder(x, y, width, height, Color, borderwidth);
        }

        /// <summary>
        /// Set obstacle as a rectangle border with a specified border color.
        /// </summary>
        /// <param name="x">X position of left border in millimeters.</param>
        /// <param name="y">Y position of upper border in millimeters.</param>
        /// <param name="width">Width of the rectangle in millimeters.</param>
        /// <param name="height">Height of the rectangle in millimeters.</param>
        /// <param name="color">Color of the rectangle border.</param>
        /// <param name="borderwidth">Width of the border in pixels.</param>
        public void SetRectangleBorder(int x, int y, int width, int height, Color color, int borderwidth = 4)
        {
            Rectangle = new Rectangle(x, y, width, height);
            Type = ObstacleType.RectangleBorder;
            BorderWidth = borderwidth;
            Color = color;
        }

        /// <summary>
        /// Set obstcle as a polygon border.
        /// </summary>
        /// <param name="points">Array of vertex points of the polygon (positions in millimeters).</param>
        /// <param name="borderwidth">Width of the polygon border in pixels.</param>
        public void SetPolygon(Vector2[] points, int borderwidth = 4)
        {
            SetPolygon(points, Color, borderwidth);
        }

        /// <summary>
        /// Set obstcle as a polygon border.
        /// </summary>
        /// <param name="points">Array of vertex points of the polygon (positions in millimeters).</param>
        /// <param name="color">Color of the polygon border.</param>
        /// <param name="borderwidth">Width of the polygon border in pixels.</param>
        public void SetPolygon(Vector2[] points, Color color, int borderwidth = 4)
        {
            Points = points;
            Type = ObstacleType.Polygon;
            BorderWidth = borderwidth;
            Color = color;
        }

        # endregion

        # region Public Function: Collision Detection

        /// <summary>
        /// Detects collision of the obstacle with a circle (RobX robot). 
        /// WARNING: Currently works only for rectangle obstacles. 
        /// </summary>
        /// <param name="CenterX">X position of center of the robot circle in millimeters.</param>
        /// <param name="CenterY">Y position of center of the robot circle in millimeters.</param>
        /// <param name="Radius">Radius of the robot in millimeters.</param>
        /// <returns>Returns true if a rectangle obstacle is intersected with the robot; otherwise returns false.</returns>
        public bool IsIntersected(double CenterX, double CenterY, double Radius)
        {
            if (Type == ObstacleType.RectangleFilled || Type == ObstacleType.RectangleBorder)
                return IsIntersected_Rect_Circle(CenterX, CenterY, Radius);
            return false;
        }

        # endregion

        # region Public Constructors

        /// <summary>
        /// Default empty constructor for Obstacle class.
        /// </summary>
        public Obstacle() { }

        /// <summary>
        /// Construct a rectangle (filled or border-only) obstacle with the specified color.
        /// </summary>
        /// <param name="x">X position of left border of the rectangle in millimeters.</param>
        /// <param name="y">Y position of upper border of the rectangle in millimeters.</param>
        /// <param name="width">Width of the rectangle in millimeters.</param>
        /// <param name="height">Height of the rectangle in millimeters.</param>
        /// <param name="color">Color of the rectangle or rectangle border.</param>
        /// <param name="fill">If true, fill the rectangle; otherwise the obstacle is only a border.</param>
        /// <param name="borderwidth">Width of the rectangle border (works only when fill parameter is false).</param>
        public Obstacle(int x, int y, int width, int height, Color color, bool fill = true, int borderwidth = 4)
        {
            if (fill == false)
                SetRectangleBorder(x, y, width, height, color, borderwidth);
            else
                SetRectangleFilled(x, y, width, height, color);
        }

        /// <summary>
        /// Construct a rectangle (filled or border-only) obstacle with the default color.
        /// </summary>
        /// <param name="x">X position of left border of the rectangle in millimeters.</param>
        /// <param name="y">Y position of upper border of the rectangle in millimeters.</param>
        /// <param name="width">Width of the rectangle in millimeters.</param>
        /// <param name="height">Height of the rectangle in millimeters.</param>
        /// <param name="fill">If true, fill the rectangle; otherwise the obstacle is only a border.</param>
        /// <param name="borderwidth">Width of the rectangle border (works only when fill parameter is false).</param>
        public Obstacle(int x, int y, int width, int height, bool fill = true, int borderwidth = 4)
        {
            if (fill == false)
                SetRectangleBorder(x, y, width, height, borderwidth);
            else
                SetRectangleFilled(x, y, width, height);
        }

        /// <summary>
        /// Construct obstcle as a polygon border with the specified color.
        /// </summary>
        /// <param name="points">Array of vertex points of the polygon (positions in millimeters).</param>
        /// <param name="color">Color of the polygon border.</param>
        /// <param name="borderwidth">Width of the polygon border.</param>
        public Obstacle(Vector2[] points, Color color, int borderwidth = 4)
        {
            SetPolygon(points, color, borderwidth);
        }

        /// <summary>
        /// Construct obstcle as a polygon border with the default color.
        /// </summary>
        /// <param name="points">Array of vertex points of the polygon (positions in millimeters)</param>
        /// <param name="borderwidth">Width of the polygon border.</param>
        public Obstacle(Vector2[] points, int borderwidth = 4)
        {
            SetPolygon(points, borderwidth);
        }

        # endregion

        # region Private Methods

        /// <summary>
        /// Detects collision of a rectangle obstacle with a circle. 
        /// </summary>
        /// <param name="CenterX">X position of center of the circle in millimeters.</param>
        /// <param name="CenterY">Y position of center of the circle in millimeters.</param>
        /// <param name="Radius">Radius of the circle in millimeters.</param>
        /// <returns>Returns true if the obstacle is intersected with the circle; otherwise returns false.</returns>
        private bool IsIntersected_Rect_Circle(double CenterX, double CenterY, double Radius)
        {
            if (Type == ObstacleType.RectangleFilled)
            {
                Vector2 rectangleCenter = new Vector2((Rectangle.X + Rectangle.Width / 2),
                                                 (Rectangle.Y + Rectangle.Height / 2));

                double width = Rectangle.Width / 2;
                double height = Rectangle.Height / 2;

                double dx = Math.Abs(CenterX - rectangleCenter.X);
                double dy = Math.Abs(CenterY - rectangleCenter.Y);

                if (dx > (Radius + width) || dy > (Radius + height)) return false;

                double distX = Math.Abs(CenterX - Rectangle.X - width);
                double distY = Math.Abs(CenterY - Rectangle.Y - height);

                if (distX <= (width))
                    return true;

                if (distY <= (height))
                    return true;

                double cornerDistanceSq = Math.Pow(distX - width, 2) + Math.Pow(distY - height, 2);

                return (cornerDistanceSq <= (Math.Pow(Radius, 2)));
            }
            else
            {
                Vector2 pc = new Vector2((float)CenterX, (float)CenterY);
                Vector2 p1 = new Vector2(Rectangle.Left, Rectangle.Top);
                Vector2 p2 = new Vector2(Rectangle.Left, Rectangle.Bottom);
                Vector2 p3 = new Vector2(Rectangle.Right, Rectangle.Bottom);
                Vector2 p4 = new Vector2(Rectangle.Right, Rectangle.Top);
                if (FindDistanceToLine(pc, p1, p2) <= Radius) return true;
                if (FindDistanceToLine(pc, p1, p4) <= Radius) return true;
                if (FindDistanceToLine(pc, p2, p3) <= Radius) return true;
                if (FindDistanceToLine(pc, p3, p4) <= Radius) return true;
                return false;
            }
        }

        /// <summary>
        /// Calculate the distance between point pt and the line segment p1 --> p2.
        /// </summary>
        /// <param name="pt">Point for which the distance should be calculated.</param>
        /// <param name="p1">Point 1 of the line segment.</param>
        /// <param name="p2">Point 2 of the line segment.</param>
        /// <returns>The distance between the point pt and the segment p1 --> p2.</returns>
        private static double FindDistanceToLine(Vector2 pt, Vector2 p1, Vector2 p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                dx = pt.X - p1.X - t * dx;
                dy = pt.Y - p1.Y - t * dy;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

        # endregion
    }
}
