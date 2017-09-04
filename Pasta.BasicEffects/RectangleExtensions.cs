using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pasta.BasicEffects
{
	internal static class RectangleExtensions
	{
		/// <summary>
		/// Calculates the difference of two rectangles.
		/// </summary>
		/// <param name="a">The minuend rectangle to subtract from.</param>
		/// <param name="b">The subtrahend rectangle.</param>
		/// <returns>The difference <paramref name="a"/>\<paramref name="b"/> split into rectangles.</returns>
		public static Rectangle[] Except(this Rectangle a, Rectangle b)
		{
			// No intersection - the difference is the minuend rectangle.
			if (!a.IntersectsWith(b))
			{
				return new[] { a };
			}

			var rectList = new List<Rectangle>(4);
			// left rectangle
			if (b.Left > a.Left)
			{
				rectList.Add(new Rectangle(
					a.Left,
					a.Top,
					Math.Max(0, b.Left - a.Left),
					a.Height));
			}
			// right rectangle
			if (b.Right < a.Right)
			{
				rectList.Add(new Rectangle(
					b.Right,
					a.Top,
					Math.Max(0, a.Right - b.Right),
					a.Height));
			}

			var width = Math.Min(a.Right, b.Right) - Math.Max(a.Left, b.Left);
			// top rectangle
			if (b.Top > a.Top)
			{
				rectList.Add(new Rectangle(
					Math.Max(a.Left, b.Left),
					a.Top,
					width,
					b.Top - a.Top));
			}
			// bottom rectangle
			if (b.Bottom < a.Bottom && width > 0)
			{

				rectList.Add(new Rectangle(
					Math.Max(a.Left, b.Left),
					b.Bottom,
					width,
					a.Bottom - b.Bottom));
			}

			return rectList.ToArray();
		}

        /// <summary>
        /// Builds a min rectangle containing all the points from the given list.
        /// </summary>
        /// <param name="points">The list of points.</param>
        /// <returns>The rectanlge containing all the points.</returns>
        public static Rectangle FromPoints(IEnumerable<Point> points)
        {
            var left = Int32.MaxValue;
            var top = Int32.MaxValue;
            var bottom = Int32.MinValue;
            var right = Int32.MinValue;
            foreach (var point in points)
            {
                left = Math.Min(left, point.X);
                top = Math.Min(top, point.Y);
                bottom = Math.Max(bottom, point.Y);
                right = Math.Max(right, point.X);
            }

            if (left == Int32.MaxValue
                || right == Int32.MinValue
                || top == Int32.MaxValue
                || bottom == Int32.MinValue)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Builds a rectangle from two points on the ends of a diagonal.
        /// </summary>
        /// <param name="a">The first point on diagonal.</param>
        /// <param name="b">The second point on diagonal.</param>
        /// <returns>The built rectangle.</returns>
        public static Rectangle FromPoints(Point a, Point b)
		{
			var location = new Point(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
			var size = new Size(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
			return new Rectangle(location, size);
		}

        /// <summary>
        /// Checks whether the rectangle has area and consequently may be painted.
        /// </summary>
        /// <param name="r">The rectangle to check.</param>
        /// <returns>True if the rectangle has visible area, False - </returns>
        public static bool HasArea(this Rectangle r)
        {
            return r.Width > 0 && r.Height > 0;
        }
	}
}
