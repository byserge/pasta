using Pasta.Core;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Pasta.BasicEffects
{
	/// <summary>
	/// Base class for effects that are result of several points function:
	/// For example, Arrow Effect has start point and end point.
	/// Provides base functionality for mouse processing and invalidation logic.
	/// </summary>
	public abstract class PointsBaseEffect : IEditableEffect, IMouseAware
	{
		/// <summary>
		/// List of points of the effect.
		/// </summary>
		protected List<Point> points = new List<Point>();

		/// <summary>
		/// List of points of the effect before editing.
		/// </summary>
		protected List<Point> previousPoints = new List<Point>();

		/// <summary>
		/// Inflate width and height of all rectangles to take into outstanding parts of the effect.
		/// </summary>
		protected virtual Size InflateSize { get; } = new Size(0, 0);

		/// <summary>
		/// Determines if the mouse left button has been pushed down.
		/// </summary>
		protected bool isMouseDown = false;

		/// <summary>
		/// Determines if the effect is being edited.
		/// </summary>
		protected bool isEditing = false;

		/// <summary>
		/// Cached for editing context.
		/// </summary>
		protected EffectContext context = null;

		/// <summary>
		/// Internal logic for applying the inherited effect.
		/// </summary>
		protected abstract void ApplyEffect();

		/// <summary>
		/// Internal logic to apply when left mouse is pressed.
		/// Just do the points update here.
		/// </summary>
		/// <param name="e">Mouse args.</param>
		protected virtual void ProcessMouseDown(MouseAwareArgs e)
		{
			points.Clear();
			points.Add(e.Location);
		}

		/// <summary>
		/// Internal logic to apply when mouse is moved with left button pressed.
		/// Just do the points update here.
		/// </summary>
		/// <param name="e">Mouse args.</param>
		protected virtual void ProcessMouseMove(MouseAwareArgs e)
		{
			if (points.Count == 0)
				points.Add(e.Location);
			else if (points.Count == 1)
				points.Add(e.Location);
			else
				points[points.Count - 1] = e.Location;
		}

		/// <summary>
		/// Internal logic to apply when left mouse button is released.
		/// Just do the points update here.
		/// </summary>
		/// <param name="e">Mouse args.</param>
		protected virtual void ProcessMouseUp(MouseAwareArgs e)
		{

		}

		/// <summary>
		/// Calculates the rectangle that for sure includes all the effect drawings.
		/// </summary>
		/// <param name="points">Points of the effect.</param>
		/// <returns>The bounding rectangle.</returns>
		protected virtual Rectangle GetEffectRectangle(List<Point> points)
		{
			var rectangle = RectangleExtensions.FromPoints(points);
			rectangle = InflateRectangle(rectangle);

			return rectangle;
		}

		/// <summary>
		/// Calculates rectangles to be sure that all outstanding parts of the effect drawing are included.
		/// </summary>
		/// <param name="rectangle">The bounding rectangle around points.</param>
		/// <returns>The inflated rectangle.</returns>
		protected virtual Rectangle InflateRectangle(Rectangle rectangle)
		{
			rectangle.Inflate(InflateSize);
			return rectangle;
		}

		#region IEditableEffect

		public void Apply(EffectContext context)
		{
			this.context = context;
			ApplyEffect();

			if (System.Diagnostics.Debugger.IsAttached)
			{
				var rectangle = GetEffectRectangle(points);
				rectangle.Width--;
				rectangle.Height--;
				context.Graphics.DrawRectangle(Pens.Green, rectangle);
			}
		}

		public void CancelEdit()
		{
			if (!isEditing)
				return;

			var previousRectangle = GetEffectRectangle(previousPoints);
			var currentRectangle = GetEffectRectangle(points);

			var invalidationRectangle = CalculateInvalidatedRectangle(previousRectangle, currentRectangle);

			CopyPoints(previousPoints, points);

			if (invalidationRectangle != null)
			{
				context.Invalidate(invalidationRectangle.Value);
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

			CopyPoints(points, previousPoints);
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

			var prevRectangle = GetEffectRectangle(points);

			ProcessMouseDown(e);

			var currRectangle = GetEffectRectangle(points);
			var invalidationRectangle = CalculateInvalidatedRectangle(prevRectangle, currRectangle);
			if (invalidationRectangle != null)
				this.context.Invalidate(invalidationRectangle.Value);
		}

		public void OnMouseMove(MouseAwareArgs e)
		{
			if (!isEditing)
				return;

			if (!isMouseDown)
				return;

			var prevRectangle = GetEffectRectangle(points);

			ProcessMouseMove(e);

			var currRectangle = GetEffectRectangle(points);
			var invalidationRectangle = CalculateInvalidatedRectangle(prevRectangle, currRectangle);
			if (invalidationRectangle != null)
				this.context.Invalidate(invalidationRectangle.Value);
		}

		public void OnMouseUp(MouseAwareArgs e)
		{
			if (!isEditing)
				return;

			if (e.Button != MouseButtons.Left)
				return;

			isMouseDown = false;

			ProcessMouseUp(e);
		}

		#endregion

		/// <summary>
		/// Overwrites destination list with the source one.
		/// </summary>
		/// <param name="source">The source list of points.</param>
		/// <param name="destination">The destination list.</param>
		private void CopyPoints(List<Point> source, List<Point> destination)
		{
			destination.Clear();
			destination.AddRange(source);
		}

		/// <summary>
		/// Calculates rectangular area to be repainted based on two rectangles with possible updates.
		/// If one of the rectangles is empty - the other one is returned.
		/// </summary>
		/// <param name="r1">The first rectangle with updates.</param>
		/// <param name="r2">The second rectangle with updates.</param>
		/// <returns>The invalidated rectangle or null in case invalidation is not required.</returns>
		private Rectangle? CalculateInvalidatedRectangle(Rectangle r1, Rectangle r2)
		{
			Rectangle invalidationRectangle;
			if (!r1.HasArea())
				invalidationRectangle = r2;
			else if (!r2.HasArea())
				invalidationRectangle = r1;
			else
				invalidationRectangle = Rectangle.Union(r1, r2);
			bool needInvalidation = invalidationRectangle.HasArea();

			if (needInvalidation)
				return invalidationRectangle;
			return null;
		}
	}
}
