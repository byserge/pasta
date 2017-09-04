using Pasta.Core;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Pasta.BasicEffects
{
	public class RectangleEffect: PointsBaseEffect, IEditableEffect, IMouseAware
	{
		/// <summary>
		/// The arrow line width.
		/// </summary>
		private float lineWidth = 2f;

		/// <summary>
		/// Inflate width and height of all rectangles to take into account lines width.
		/// </summary>
		private Size inflateSize;

		/// <summary>
		/// The pen to draw the rectangle.
		/// </summary>
		private Pen pen;

		public RectangleEffect()
		{
			var inflateWidthHeight = (int)Math.Ceiling(lineWidth + 2);
			inflateSize = new Size(inflateWidthHeight, inflateWidthHeight);
			// TODO: move settings to config
			pen = new Pen(Color.Red, lineWidth);
		}

        protected override Size InflateSize => inflateSize;

        protected override void ApplyEffect()
        {
            if (points.Count < 2)
                return;

            var smoothingMode = context.Graphics.SmoothingMode;
            context.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            context.Graphics.DrawRectangle(pen, RectangleExtensions.FromPoints(points));
            context.Graphics.SmoothingMode = smoothingMode;
        }
	}
}
