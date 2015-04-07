# region Includes

using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

# endregion

namespace RobX.Simulator
{
    /// <summary>
    /// A class that provides basic drawing functionality using XNA framework. 
    /// </summary>
    public static class GraphicsEngine
    {
        # region Private Fields

        private static Texture2D _pixel;

        # endregion

        # region Public Fields

        /// <summary>
        /// Font used for drawing grid annotation text on the screen.
        /// </summary>
        public static SpriteFont GridFont;

        /// <summary>
        /// Font used for drawing statistics text on the screen.
        /// </summary>
        public static SpriteFont StatisticsFont;

        /// <summary>
        /// Font used for drawing simulation speed text on the screen.
        /// </summary>
        public static SpriteFont SimulationSpeedFont;

        # endregion

        # region Public Static Methods (Draw Functions)

        /// <summary>
        /// Initializes the current graphics engine for the first time of use.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device used to draw.</param>
        /// <param name="gridFont">Font for drawing the grid annotation text on the screen.</param>
        /// <param name="statisticsFont">Font for drawing statistics text on the screen.</param>
        /// <param name="simulationSpeedFont">Font for drawing simulation speed text on the screen.</param>
        public static void Initialize(GraphicsDevice graphicsDevice, SpriteFont gridFont,
            SpriteFont statisticsFont, SpriteFont simulationSpeedFont)
        {
            _pixel = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _pixel.SetData(new[] { Color.White });
            GridFont = gridFont;
            StatisticsFont = statisticsFont;
            SimulationSpeedFont = simulationSpeedFont;
        }

        /// <summary>
        /// Draw a filled rectangle on the screen.
        /// </summary>
        /// <param name="sprite">SpriteBatch used for drawing.</param>
        /// <param name="rect">Dimensions of the rectangle to draw on the screen in pixels.</param>
        /// <param name="color">Color of the rectangle.</param>
        public static void FillRect(this SpriteBatch sprite, Rectangle rect, Color color)
        {
            sprite.Draw(_pixel, rect, color);
        }

        /// <summary>
        /// Draw a rectangle border on the screen.
        /// </summary>
        /// <param name="sprite">SpriteBatch used for drawing.</param>
        /// <param name="rect">Dimensions of the rectangle to draw on the screen in pixels.</param>
        /// <param name="color">Color of the rectangle.</param>
        /// <param name="horizontalBorderWidth">Width of the horizontal border of the rectangle in pixels.</param>
        /// <param name="verticalBorderWidth">Width of the vertical border of the rectangle in pixels.</param>
        public static void DrawRect(this SpriteBatch sprite, Rectangle rect, Color color, int horizontalBorderWidth = 20, int verticalBorderWidth = 20)
        {
            // Draw top line
            sprite.FillRect(new Rectangle(rect.X, rect.Y, rect.Width, horizontalBorderWidth), color);

            // Draw left line
            sprite.FillRect(new Rectangle(rect.X, rect.Y, verticalBorderWidth, rect.Height), color);

            // Draw right line
            sprite.FillRect(new Rectangle((rect.X + rect.Width - verticalBorderWidth), rect.Y, verticalBorderWidth, rect.Height), color);

            // Draw bottom line
            sprite.FillRect(new Rectangle(rect.X, rect.Y + rect.Height - horizontalBorderWidth, rect.Width, horizontalBorderWidth), color);
        }

        /// <summary>
        /// Draw a horizontal line on the screen.
        /// </summary>
        /// <param name="sprite">SpriteBatch used for drawing.</param>
        /// <param name="startX">X coordinate of the starting (left) point of the line.</param>
        /// <param name="endX">X coordinate of the ending (right) point of the line.</param>
        /// <param name="y">Y coordinate of the line.</param>
        /// <param name="color">Color of the line.</param>
        /// <param name="lineWidth">Width of the line in pixels.</param>
        /// <param name="pattern">Pattern used to draw the line. If pattern is null, draws a solid line. Otherwise uses the pattern 
        /// array to draw a dashed line.</param>
        public static void DrawHorizontalLine(this SpriteBatch sprite, int startX, int endX, int y, Color color, int lineWidth = 10, int[] pattern = null)
        {
            if (pattern == null)
            {
                sprite.FillRect(new Rectangle(startX, y - lineWidth / 2, endX - startX, lineWidth), color);
                return;
            }

            var x = startX;
            for (; x <= endX - pattern[0]; x += pattern[0] + pattern[1])
                sprite.DrawHorizontalLine(x, x + pattern[0], y, color, lineWidth);

            if (x <= endX)
                // ReSharper disable once TailRecursiveCall
                sprite.DrawHorizontalLine(x, endX, y, color, lineWidth);
        }

        /// <summary>
        /// Draw a vertical line on the screen.
        /// </summary>
        /// <param name="sprite">SpriteBatch used for drawing.</param>
        /// <param name="x">X coordinate of the line.</param>
        /// <param name="startY">Y coordinate of the starting (top) point of the line.</param>
        /// <param name="endY">Y coordinate of the ending (bottom) point of the line.</param>
        /// <param name="color">Color of the line.</param>
        /// <param name="lineWidth">Width of the line in pixels.</param>
        /// <param name="pattern">Pattern used to draw the line. If pattern is null, draws a solid line. Otherwise uses the pattern 
        /// array to draw a dashed line.</param>
        public static void DrawVerticalLine(this SpriteBatch sprite, int x, int startY, int endY, 
            Color color, int lineWidth = 10, int[] pattern = null)
        {
            if (pattern == null)
            {
                sprite.FillRect(new Rectangle(x - lineWidth / 2, startY, lineWidth, endY - startY), color);
                return;
            }

            var y = startY;
            for (; y <= endY - pattern[0]; y += pattern[0] + pattern[1])
                sprite.DrawVerticalLine(x, y, y + pattern[0], color, lineWidth);

            if (y <= endY)
                // ReSharper disable once TailRecursiveCall
                sprite.DrawVerticalLine(x, y, endY, color, lineWidth);
        }

        /// <summary>
        /// Draw (write) a string of text on the screen.
        /// </summary>
        /// <param name="sprite">SpriteBatch used for drawing.</param>
        /// <param name="str">String of text to write on the screen.</param>
        /// <param name="color">Color of the text.</param>
        /// <param name="font">Font of the text.</param>
        /// <param name="x">X coordinate of the text.</param>
        /// <param name="y">Y Coordinate of the text.</param>
        /// <param name="alignHorizontal">Horizontal alignment of the text with respect to (X, Y).</param>
        /// <param name="alignVertical">Vertical alignment of the text with respect to (X, Y).</param>
        /// <returns>Returns the position and dimensions of the rectangle surrounding the written text.</returns>
        public static Rectangle DrawString(this SpriteBatch sprite, string str, Color color,
            SpriteFont font, int x = 0, int y = 0,
            HorizontalTextAlign alignHorizontal = HorizontalTextAlign.Center,
            VerticalTextAlign alignVertical = VerticalTextAlign.Center)
        {
            // Measure size of the drawn string
            var strsize = font.MeasureString(str);

            // Calculate actual x and y positions of upper-left corner of text
            var strX = x;
            var strY = y;

            switch (alignHorizontal)
            {
                case HorizontalTextAlign.Center:
                    strX -= (int)strsize.X / 2;
                    break;
                case HorizontalTextAlign.Right:
                    strX -= (int)strsize.X;
                    break;
            }

            switch (alignVertical)
            {
                case VerticalTextAlign.Center:
                    strY -= (int)strsize.Y / 2;
                    break;
                case VerticalTextAlign.Bottom:
                    strY -= (int)strsize.Y;
                    break;
            }

            if (x == 0) strX = 3;
            if (y == 0) strY = 3;

            // Draw text on the frame
            sprite.DrawString(font, str, new Vector2(strX, strY), color);

            return new Rectangle(strX, strY, (int)strsize.X, (int)strsize.Y);
        }

        /// <summary>
        /// Draw a line segment on the screen.
        /// </summary>
        /// <param name="sprite">SpriteBatch used for drawing.</param>
        /// <param name="point1">Starting point of the line.</param>
        /// <param name="point2">Ending point of the line.</param>
        /// <param name="color">Color of the line.</param>
        /// <param name="lineWidth">Width of the line in pixels.</param>
        public static void DrawLineSegment(this SpriteBatch sprite, PointF point1, 
            PointF point2, Color color, int lineWidth)
        {

            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            var length = Vector2.Distance(PointFtoVector2(point1), PointFtoVector2(point2));

            sprite.Draw(_pixel, PointFtoVector2(point1), null, color, angle, Vector2.Zero, 
                new Vector2(length, lineWidth), SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Draw a path on the screen.
        /// </summary>
        /// <param name="sprite">SpriteBatch used for drawing.</param>
        /// <param name="points">Ordered collection of points defining the path.</param>
        /// <param name="color">Color of the path.</param>
        /// <param name="lineWidth">Width of the lines (in pixels) connecting the consecutive points in the path.</param>
        public static void DrawPath(this SpriteBatch sprite, PointF[] points, Color color, int lineWidth = 1)
        {
            var count = points.Length;
            if (count < 2) return;
            
            for (var i = 0; i < count - 1; i++)
                DrawLineSegment(sprite, points[i], points[i + 1], color, lineWidth);
        }

        /// <summary>
        /// Draw a polygon on the screen.
        /// </summary>
        /// <param name="sprite">SpriteBatch used for drawing.</param>
        /// <param name="points">Ordered collection of points defining the polygon.</param>
        /// <param name="color">Color of the polygon border.</param>
        /// <param name="lineWidth">Width of the lines (in pixels) connecting the consecutive points of the polygon.</param>
        public static void DrawPolygon(this SpriteBatch sprite, PointF[] points, Color color, int lineWidth = 1)
        {
            var count = points.Length;
            if (count < 2) return;

            DrawPath(sprite, points, color, lineWidth);
            DrawLineSegment(sprite, points[count - 1], points[0], color, lineWidth);
        }

        # endregion

        # region Public Static Methods (Useful Methods)

        /// <summary>
        /// Converts XNA color structure (Microsoft.XNA.Framework.Color) to GDI color structure (System.Drawing.Color).
        /// </summary>
        /// <param name="xnaColor">XNA color (Microsoft.XNA.Framework.Color) object.</param>
        /// <returns>GDI color (System.Drawing.Color) object.</returns>
        public static System.Drawing.Color XnaColorToSystem(Color xnaColor)
        {
            return System.Drawing.Color.FromArgb(xnaColor.A, xnaColor.R, xnaColor.G, xnaColor.B);
        }

        /// <summary>
        /// Converts GDI color structure (System.Drawing.Color) to XNA color structure (Microsoft.XNA.Framework.Color).
        /// </summary>
        /// <param name="gdiColor">GDI color (System.Drawing.Color) object.</param>
        /// <returns>XNA color (Microsoft.XNA.Framework.Color) object.</returns>
        public static Color SystemColorToXna(System.Drawing.Color gdiColor)
        {
            return new Color(gdiColor.R, gdiColor.G, gdiColor.B, gdiColor.A);
        }

        /// <summary>
        /// Converts GDI PointF structure (System.Drawing.PointF) to XNA Vector2 structure (Microsoft.XNA.Framework.Vector2).
        /// </summary>
        /// <param name="gdiPointF">GDI PointF (System.Drawing.PointF) object.</param>
        /// <returns>XNA Vector2 (Microsoft.XNA.Framework.Vector2) object.</returns>
        public static Vector2 PointFtoVector2(PointF gdiPointF)
        {
            return new Vector2(gdiPointF.X, gdiPointF.Y);
        }

        /// <summary>
        /// Converts XNA Vector2 structure (Microsoft.XNA.Framework.Vector2) to GDI PointF structure (System.Drawing.PointF).
        /// </summary>
        /// <param name="xnaVector2">XNA Vector2 (Microsoft.XNA.Framework.Vector2) object.</param>
        /// <returns>GDI PointF (System.Drawing.PointF) object.</returns>
        public static PointF Vector2ToPointF(Vector2 xnaVector2)
        {
            return new PointF(xnaVector2.X, xnaVector2.Y);
        }

        # endregion

        # region Enums

        /// <summary>
        /// Horizontal alignments for a text string.
        /// </summary>
        public enum HorizontalTextAlign
        {
            /// <summary>
            /// Align text to the left.
            /// </summary>
            Left = 0,

            /// <summary>
            /// Align text to the right.
            /// </summary>
            Right = 1,

            /// <summary>
            /// Align text to the center.
            /// </summary>
            Center = 2
        }

        /// <summary>
        /// Vertical alignments for a text string.
        /// </summary>
        public enum VerticalTextAlign
        {
            /// <summary>
            /// Align text to the top.
            /// </summary>
            Top = 0,

            /// <summary>
            /// Align text to the bottom.
            /// </summary>
            Bottom = 1,

            /// <summary>
            /// Align text to the center.
            /// </summary>
            Center = 2
        }

        # endregion
    }
}
