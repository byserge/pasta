using Pasta.Core;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Pasta.BasicEffects
{
    /// <summary>
    /// Allows to draw a line.
    /// </summary>
	public class LineEffect: PointsBaseEffect, IEditableEffect, IMouseAware
	{
		/// <summary>
		/// The line width.
		/// </summary>
		private float lineWidth = 2f;

		/// <summary>
		/// Inflate width and height of all rectangles to take into account line thickness.
		/// </summary>
		private Size inflateSize;

		/// <summary>
		/// The pen to draw a line.
		/// </summary>
		private Pen pen;

		public LineEffect()
		{
			var inflateWidthHeight = (int)Math.Ceiling(lineWidth);
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
            context.Graphics.DrawLine(pen, points[0], points[1]);
            context.Graphics.SmoothingMode = smoothingMode;
        }
	}
}
