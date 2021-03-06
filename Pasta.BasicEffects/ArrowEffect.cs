﻿using Pasta.Core;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Pasta.BasicEffects
{
    /// <summary>
    /// Allows to draw an arrow.
    /// </summary>
	public class ArrowEffect: PointsBaseEffect, IEditableEffect, IMouseAware
	{
		/// <summary>
		/// The arrow line width.
		/// </summary>
		private float lineWidth = 2f;

		/// <summary>
		/// The arrow width.
		/// </summary>
		private float arrowWidth = 3f;

		/// <summary>
		/// The arrow height.
		/// </summary>
		private float arrowHeight = 5f;

		/// <summary>
		/// Inflate width and height of all rectangles to take into account arrow and line thickness.
		/// </summary>
		private Size inflateSize;

		/// <summary>
		/// The pen to draw the arrow.
		/// </summary>
		private Pen pen;

		public ArrowEffect()
		{
			var inflateWidthHeight = (int)Math.Ceiling(lineWidth + arrowWidth + arrowHeight);
			inflateSize = new Size(inflateWidthHeight, inflateWidthHeight);
			// TODO: move settings to config
			pen = new Pen(Color.Red, lineWidth);
			pen.CustomEndCap = new AdjustableArrowCap(arrowWidth, arrowHeight, true);
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
