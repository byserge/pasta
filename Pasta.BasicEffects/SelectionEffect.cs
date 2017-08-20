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
    public class SelectionEffect : ISelectionEffect, IEditableEffect, IMouseAware, IMayInvalidate
    {
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

        #region ISelectionEffect
        public Rectangle Selection => selectionRectangle;
        #endregion

        #region IMayInvalidate

        public event EventHandler<InvalidatedEventArgs> Invalidated;

        #endregion

        #region IEditableEffect

        public void Apply(IEffectApplyContext context)
        {
            var grayedRectangles = context.Bounds.Except(selectionRectangle);

            context.Graphics.FillRectangles(grayBrush, grayedRectangles);
        }

        public void CancelEdit()
        {
            isEditing = false;
        }

        public void CommitEdit()
        {
            isEditing = false;
        }

        public void StartEdit(IEffectEditContext context)
        {
            isEditing = true;
        }

        #endregion

        #region IMouseAware

        public void OnMouseDown(MouseEventArgs e)
        {
            if (!isEditing)
                return;

            if (e.Button != MouseButtons.Left)
                return;

            isMouseDown = true;
            mouseDownPoint = e.Location;
        }

        public void OnMouseMove(MouseEventArgs e)
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

            var invalidatedRectangle = Rectangle.Union(selectionRectangle, newSelectionRectangle);

            selectionRectangle = newSelectionRectangle;

            Invalidated?.Invoke(this, new InvalidatedEventArgs(invalidatedRectangle));
        }

        public void OnMouseUp(MouseEventArgs e)
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
