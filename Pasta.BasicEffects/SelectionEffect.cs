using Pasta.Core;
using System.Drawing;

namespace Pasta.BasicEffects
{
	/// <summary>
	/// The effect of selection of some rectangular area.
	/// All unselected area is grayed.
	/// Supports editing selection with mouse.
	/// </summary>
	public class SelectionEffect : PointsBaseEffect, ISelectionEffect, IEditableEffect, IMouseAware
	{
		/// <summary>
		/// The brush to gray out outside the selection.
		/// </summary>
		private readonly Brush grayBrush = new SolidBrush(Color.FromArgb(100, Color.Black));

		/// <summary>
		/// The pen to highlight the border of the selection - especially important on black backgrounds.
		/// </summary>
		private readonly Pen borderPen = Pens.White;

		#region ISelectionEffect
		public Rectangle Selection => RectangleExtensions.FromPoints(points);
		#endregion

		protected override Size InflateSize { get; } = new Size(2, 2);

		protected override void ApplyEffect()
		{
			var grayedRectangles = context.Bounds.Except(Selection);

			context.Graphics.FillRectangles(grayBrush, grayedRectangles);

			if (Selection.HasArea())
			{
				var borderRectangle = new Rectangle(Selection.X - 1, Selection.Y - 1, Selection.Width + 1, Selection.Height + 1);
				
				context.Graphics.DrawRectangle(borderPen, borderRectangle);
			}
		}
	}
}
