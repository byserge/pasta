using Pasta.Core;
using System.Drawing;
using System;
using System.Windows.Forms;

namespace Pasta.BasicEffects
{
	/// <summary>
	/// The effect of selection of some rectangular area.
	/// All unselected area is grayed.
	/// Supports editing selection with mouse.
	/// </summary>
	public class SelectionEffect : ISelectionEffect, IEditableEffect, IMouseAware
	{
		/// <summary>
		/// Stores selection area before going to edit mode to allow cancel.
		/// </summary>
		private Rectangle previousSelection = Rectangle.Empty;
		/// <summary>
		/// Stores selection area
		/// </summary>
		private Rectangle selectionRectangle = Rectangle.Empty;

		/// <summary>
		/// Stores the point at which the mouse left button was pushed down.
		/// </summary>
		private Point mouseDownPoint;

		/// <summary>
		/// Determines if the mouse left button has been pushed down.
		/// </summary>
		private bool isMouseDown = false;

		/// <summary>
		/// Determines if the effect is being edited.
		/// </summary>
		private bool isEditing = false;

		/// <summary>
		/// The brush to gray out outside the selection.
		/// </summary>
		private Brush grayBrush = new SolidBrush(Color.FromArgb(100, Color.Black));

        /// <summary>
        /// Cached for editing context.
        /// </summary>
        private EffectContext context = null;

		#region ISelectionEffect
		public Rectangle Selection => selectionRectangle;
		#endregion

		#region IEditableEffect

		public void Apply(EffectContext context)
		{
			var grayedRectangles = context.Bounds.Except(selectionRectangle);

			context.Graphics.FillRectangles(grayBrush, grayedRectangles);
		}

		public void CancelEdit()
		{
			if (!isEditing)
				return;

			var invalidationRectangle = previousSelection.IsEmpty
				? selectionRectangle
				: Rectangle.Union(selectionRectangle, previousSelection);
			bool needInvalidation = selectionRectangle != previousSelection && !invalidationRectangle.IsEmpty;

			selectionRectangle = previousSelection;
			if (needInvalidation)
				context.Invalidate(invalidationRectangle);

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

			previousSelection = selectionRectangle;
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
			mouseDownPoint = e.Location;
		}

		public void OnMouseMove(MouseAwareArgs e)
		{
			if (!isEditing)
				return;

			if (!isMouseDown)
				return;

			var point = e.Location;

			// Calculate rectangle
			var location = new Point(Math.Min(point.X, mouseDownPoint.X), Math.Min(point.Y, mouseDownPoint.Y));
			var size = new Size(Math.Abs(point.X - mouseDownPoint.X), Math.Abs(point.Y - mouseDownPoint.Y));

			var newSelectionRectangle = new Rectangle(location, size);

			var invalidatedRectangle = selectionRectangle.IsEmpty
				? newSelectionRectangle
				: Rectangle.Union(selectionRectangle, newSelectionRectangle);

			selectionRectangle = newSelectionRectangle;

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
