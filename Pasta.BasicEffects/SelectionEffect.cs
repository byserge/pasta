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
	public class SelectionEffect : PointsBaseEffect, ISelectionEffect, IEditableEffect, IMouseAware
	{
		/// <summary>
		/// The brush to gray out outside the selection.
		/// </summary>
		private Brush grayBrush = new SolidBrush(Color.FromArgb(100, Color.Black));

		#region ISelectionEffect
		public Rectangle Selection => RectangleExtensions.FromPoints(points);
        #endregion

        protected override void ApplyEffect()
        {
            var grayedRectangles = context.Bounds.Except(Selection);

            context.Graphics.FillRectangles(grayBrush, grayedRectangles);
        }        
	}
}
