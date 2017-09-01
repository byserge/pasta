using Pasta.Core;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Pasta.BasicEffects
{
	public class ArrowEffect: IEditableEffect, IMouseAware
	{
		/// <summary>
		/// Stores start position before editing.
		/// </summary>
		private Point previousStartPosition = Point.Empty;

		/// <summary>
		/// Stores end position before editing.
		/// </summary>
		private Point previousEndPosition = Point.Empty;

		/// <summary>
		/// The arrow start position.
		/// </summary>
		private Point startPosition;

		/// <summary>
		/// The arrow end position.
		/// </summary>
		private Point endPosition;

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
		/// Determines if the mouse left button has been pushed down.
		/// </summary>
		private bool isMouseDown = false;

		/// <summary>
		/// Determines if the effect is being edited.
		/// </summary>
		private bool isEditing = false;

		/// <summary>
		/// The pen to draw the arrow.
		/// </summary>
		private Pen pen;

		/// <summary>
		/// Cached for editing context.
		/// </summary>
		private EffectContext context = null;

		public ArrowEffect()
		{
			var inflateWidthHeight = (int)Math.Ceiling(lineWidth + arrowWidth + arrowHeight);
			inflateSize = new Size(inflateWidthHeight, inflateWidthHeight);
			// TODO: move settings to config
			pen = new Pen(Color.Red, lineWidth);
			pen.CustomEndCap = new AdjustableArrowCap(arrowWidth, arrowHeight, true);
		}

		#region IEditableEffect

		public void Apply(EffectContext context)
		{
			if (startPosition == endPosition)
				return;

			var smoothingMode = context.Graphics.SmoothingMode;
			context.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			context.Graphics.DrawLine(pen, startPosition, endPosition);
			context.Graphics.SmoothingMode = smoothingMode;
		}

		public void CancelEdit()
		{
			if (!isEditing)
				return;

			var previousRectangle = RectangleExtensions.FromPoints(previousEndPosition, previousStartPosition);
			var currentRectangle = RectangleExtensions.FromPoints(startPosition, endPosition);

			var invalidationRectangle = previousRectangle.IsEmpty
				? currentRectangle
				: Rectangle.Union(currentRectangle, previousRectangle);
			bool needInvalidation = currentRectangle != previousRectangle
				&& !invalidationRectangle.IsEmpty;

			startPosition = previousStartPosition;
			endPosition = previousEndPosition;
			if (needInvalidation)
			{
				invalidationRectangle.Inflate(inflateSize);
				context.Invalidate(invalidationRectangle);
			}

			isEditing = false;
		}

		public void CommitEdit()
		{
			if (!isEditing)
				return;

			isEditing = false;
		}

		public void StartEdit(EffectContext context)
		{
			this.context = context;

			if (isEditing)
				return;

			previousStartPosition = startPosition;
			previousEndPosition = endPosition;
			isEditing = true;
		}

		#endregion

		#region IMouseAware

		public void OnMouseDown(MouseAwareArgs e)
		{
			if (!isEditing)
				return;

			if (e.Button != MouseButtons.Left)
				return;

			isMouseDown = true;

			var prevRectangle = RectangleExtensions.FromPoints(startPosition, endPosition);
			var invalidatedRectangle = prevRectangle;
			invalidatedRectangle.Inflate(inflateSize);
			this.context.Invalidate(invalidatedRectangle);

			startPosition = e.Location;
			endPosition = e.Location;
		}

		public void OnMouseMove(MouseAwareArgs e)
		{
			if (!isEditing)
				return;

			if (!isMouseDown)
				return;

			var point = e.Location;

			// Calculate rectangle
			var newRectangle = RectangleExtensions.FromPoints(e.Location, startPosition);
			var prevRectangle = RectangleExtensions.FromPoints(startPosition, endPosition);

			var invalidatedRectangle = prevRectangle.IsEmpty
				? newRectangle
				: Rectangle.Union(newRectangle, prevRectangle);

			endPosition = e.Location;

			invalidatedRectangle.Inflate(inflateSize);
			this.context.Invalidate(invalidatedRectangle);
		}

		public void OnMouseUp(MouseAwareArgs e)
		{
			if (!isEditing)
				return;

			if (e.Button != MouseButtons.Left)
				return;

			isMouseDown = false;
		}

		#endregion
	}
}
