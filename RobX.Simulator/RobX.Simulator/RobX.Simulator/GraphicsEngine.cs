# region Includes

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

# endregion

namespace RobX.Simulator
{
    public static class GraphicsEngine
    {
        private static Texture2D pixel;
        public static SpriteFont GridFont;
        public static SpriteFont StatisticsFont;
        public static SpriteFont SimulationSpeedFont;

        public static void Initialize(GraphicsDevice graphicsDevice, SpriteFont gridFont,
            SpriteFont statisticsFont, SpriteFont simulationSpeedFont)
        {
            pixel = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
            GridFont = gridFont;
            StatisticsFont = statisticsFont;
            SimulationSpeedFont = simulationSpeedFont;
        }

        public static void FillRect(this SpriteBatch Sprite, Rectangle Rect, Color Color)
        {
            Sprite.Draw(pixel, Rect, Color);
        }

        public static void DrawRect(this SpriteBatch Sprite, Rectangle Rect, Color Color, int BorderWidth = 20)
        {
            // Draw top line
            Sprite.FillRect(new Rectangle(Rect.X, Rect.Y, Rect.Width, BorderWidth), Color);

            // Draw left line
            Sprite.FillRect(new Rectangle(Rect.X, Rect.Y, BorderWidth, Rect.Height), Color);

            // Draw right line
            Sprite.FillRect(new Rectangle((Rect.X + Rect.Width - BorderWidth), Rect.Y, BorderWidth, Rect.Height), Color);

            // Draw bottom line
            Sprite.FillRect(new Rectangle(Rect.X, Rect.Y + Rect.Height - BorderWidth, Rect.Width, BorderWidth), Color);
        }

        public static void DrawHorizontalLine(this SpriteBatch Sprite, int StartX, int EndX, int Y, Color Color, int LineWidth = 10, int[] Pattern = null)
        {
            if (Pattern == null)
            {
                Sprite.FillRect(new Rectangle(StartX, Y - LineWidth / 2, EndX - StartX, LineWidth), Color);
                return;
            }

            var X = StartX;
            for (; X <= EndX - Pattern[0]; X += Pattern[0] + Pattern[1])
                Sprite.DrawHorizontalLine(X, X + Pattern[0], Y, Color, LineWidth);

            if (X <= EndX)
                Sprite.DrawHorizontalLine(X, EndX, Y, Color, LineWidth);
        }

        public static void DrawVerticalLine(this SpriteBatch Sprite, int X, int StartY, int EndY, Color Color, int LineWidth = 10, int[] Pattern = null)
        {
            if (Pattern == null)
            {
                Sprite.FillRect(new Rectangle(X - LineWidth / 2, StartY, LineWidth, EndY - StartY), Color);
                return;
            }

            var Y = StartY;
            for (; Y <= EndY - Pattern[0]; Y += Pattern[0] + Pattern[1])
                Sprite.DrawVerticalLine(X, Y, Y + Pattern[0], Color, LineWidth);

            if (Y <= EndY)
                Sprite.DrawVerticalLine(X, Y, EndY, Color, LineWidth);
        }



        public static Rectangle DrawString(this SpriteBatch Sprite, string str, Color color,
            SpriteFont Font, int X = 0, int Y = 0,
            HorizontalTextAlign AlignHorizontal = HorizontalTextAlign.Center,
            VerticalTextAlign AlignVertical = VerticalTextAlign.Center)
        {
            // Measure size of the drawn string
            var strsize = Font.MeasureString(str);

            // Calculate actual x and y positions of upper-left corner of text
            var x = X;
            var y = Y;

            if (AlignHorizontal == HorizontalTextAlign.Center)
                x -= (int)strsize.X / 2;
            else if (AlignHorizontal == HorizontalTextAlign.Right)
                x -= (int)strsize.X;

            if (AlignVertical == VerticalTextAlign.Center)
                y -= (int)strsize.Y / 2;
            else if (AlignVertical == VerticalTextAlign.Bottom)
                y -= (int)strsize.Y;

            if (X == 0) x = 3;
            if (Y == 0) y = 3;

            // Draw text on the frame
            Sprite.DrawString(Font, str, new Vector2(x, y), color);

            return new Rectangle(x, y, (int)strsize.X, (int)strsize.Y);
        }

        public static void DrawLineSegment(this SpriteBatch Sprite, Vector2 point1, Vector2 point2, Color color, int lineWidth)
        {
            var angle = (float)System.Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            var length = Vector2.Distance(point1, point2);

            Sprite.Draw(pixel, point1, null, color, angle, Vector2.Zero, new Vector2(length, lineWidth), SpriteEffects.None, 0f);
        }

        public static void DrawPath(this SpriteBatch Sprite, Vector2[] points, Color color, int lineWidth = 1)
        {
            var Count = points.Length;
            if (Count >= 2)
                for (var i = 0; i < Count - 1; i++)
                    DrawLineSegment(Sprite, points[i], points[i + 1], color, lineWidth);
        }

        public static void DrawPolygon(this SpriteBatch Sprite, Vector2[] points, Color color, int lineWidth = 1)
        {
            var Count = points.Length;
            if (Count >= 2)
            {
                DrawPath(Sprite, points, color, lineWidth);
                DrawLineSegment(Sprite, points[Count - 1], points[0], color, lineWidth);
            }
        }

        /// <summary>
        /// Horizontal alignments for a text string
        /// </summary>
        public enum HorizontalTextAlign
        {
            Left = 0,
            Right = 1,
            Center = 2
        }

        /// <summary>
        /// Vertical alignments for a text string
        /// </summary>
        public enum VerticalTextAlign
        {
            Top = 0,
            Bottom = 1,
            Center = 2
        }
    }

}
