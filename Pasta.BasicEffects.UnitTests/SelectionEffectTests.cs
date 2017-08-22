using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pasta.Core;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Pasta.BasicEffects.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="SelectionEffect"/>
    /// </summary>
    [TestClass]
    public class SelectionEffectTests
    {
        #region OnMouseMove
        /// <summary>
        /// Tests that
        /// There are no updates and no selection on mouse move if the effect is not in edit mode.
        /// </summary>
        [TestMethod]
        public void OnMouseMove_NoEditMode_NoUpdate()
        {
            var requestedInvalidateRectangles = new List<Rectangle>();
            var selectionEffect = new SelectionEffect();
            selectionEffect.Invalidated += (sender, args) => requestedInvalidateRectangles.Add(args.Rectangle);

            selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 10, 10, 0));
            selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 20, 20, 0));
            selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 20, 20, 0));

            Assert.AreEqual(Rectangle.Empty, selectionEffect.Selection);
            Assert.AreEqual(0, requestedInvalidateRectangles.Count);
        }

        /// <summary>
        /// Tests that
        /// There are no updates and no selection on mouse move if the left button hasn't been pressed
        /// </summary>
        [TestMethod]
        public void OnMouseMove_OnMouseDownHasntCalled_NoUpdate()
        {
            var requestedInvalidateRectangles = new List<Rectangle>();
            var selectionEffect = new SelectionEffect();
            selectionEffect.Invalidated += (sender, args) => requestedInvalidateRectangles.Add(args.Rectangle);
            var editContext = new Mock<IEffectEditContext>();

            selectionEffect.StartEdit(editContext.Object);
            selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 20, 20, 0));
            selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 20, 20, 0));

            Assert.AreEqual(Rectangle.Empty, selectionEffect.Selection);
            Assert.AreEqual(0, requestedInvalidateRectangles.Count);
        }
        #endregion
    }
}
