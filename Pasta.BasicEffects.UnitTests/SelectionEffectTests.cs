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
		/// There are no updates and no selection on mouse move if a mouse button hasn't been pressed
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

		/// <summary>
		/// Tests that
		/// There are no updates and no selection on mouse move if the left mouse button hasn't been click.
		/// </summary>
		[TestMethod]
		public void OnMouseMove_LeftMouseButtonHasntBeenClicked_NoUpdate()
		{
			var requestedInvalidateRectangles = new List<Rectangle>();
			var selectionEffect = new SelectionEffect();
			selectionEffect.Invalidated += (sender, args) => requestedInvalidateRectangles.Add(args.Rectangle);
			var editContext = new Mock<IEffectEditContext>();

			selectionEffect.StartEdit(editContext.Object);
			selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Right | MouseButtons.Middle, 1, 10, 10, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Right | MouseButtons.Middle, 1, 20, 20, 0));
			selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Right | MouseButtons.Middle, 1, 20, 20, 0));

			Assert.AreEqual(Rectangle.Empty, selectionEffect.Selection);
			Assert.AreEqual(0, requestedInvalidateRectangles.Count);
		}

		/// <summary>
		/// Tests that
		/// Selection is calculated correctly and Invalidated called once if there were left mouse down, move, up 
		/// in editing mode.
		/// </summary>
		[TestMethod]
		public void OnMouseMove_RectangleSelectedWithMouse_Update()
		{
			var requestedInvalidateRectangles = new List<Rectangle>();
			var selectionEffect = new SelectionEffect();
			selectionEffect.Invalidated += (sender, args) => requestedInvalidateRectangles.Add(args.Rectangle);
			var editContext = new Mock<IEffectEditContext>();

			selectionEffect.StartEdit(editContext.Object);
			selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 5, 10, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 20, 30, 0));
			selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 20, 20, 0));

			var expectedSelection = new Rectangle(5, 10, 15, 20);
			Assert.AreEqual(expectedSelection, selectionEffect.Selection);
			Assert.AreEqual(1, requestedInvalidateRectangles.Count);
			Assert.AreEqual(expectedSelection, requestedInvalidateRectangles[0]);
		}

		/// <summary>
		/// Tests that
		/// Selection is calculated correctly and Invalidated called twice if there were left mouse down, 2 moves and up 
		/// in editing mode.
		/// </summary>
		[TestMethod]
		public void OnMouseMove_MouseMovedTwice_InvalidatedCalledTwice()
		{
			var requestedInvalidateRectangles = new List<Rectangle>();
			var selectionEffect = new SelectionEffect();
			selectionEffect.Invalidated += (sender, args) => requestedInvalidateRectangles.Add(args.Rectangle);
			var editContext = new Mock<IEffectEditContext>();

			selectionEffect.StartEdit(editContext.Object);
			selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 5, 10, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 20, 30, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 100, 200, 0));
			selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 100, 200, 0));

			var expectedSelection = new Rectangle(5, 10, 95, 190);
			var expectedInvalidations = new[]
			{
				new Rectangle(5, 10, 15, 20),
				new Rectangle(5, 10, 95, 190)
			};

			Assert.AreEqual(expectedSelection, selectionEffect.Selection);
			CollectionAssert.AreEqual(expectedInvalidations, requestedInvalidateRectangles);
		}

		/// <summary>
		/// Tests that
		/// Selection is calculated correctly, Invalidated called twice and the second invalidated rectangle includes the first one
		/// if there were 2 left mouse down, 2 moves and 2 ups (two separate selections were made) in editing mode.
		/// </summary>
		[TestMethod]
		public void OnMouseMove_TwoSelectionAttempts_InvalidatedCalledTwice()
		{
			var requestedInvalidateRectangles = new List<Rectangle>();
			var selectionEffect = new SelectionEffect();
			selectionEffect.Invalidated += (sender, args) => requestedInvalidateRectangles.Add(args.Rectangle);
			var editContext = new Mock<IEffectEditContext>();

			selectionEffect.StartEdit(editContext.Object);
			selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 5, 10, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 20, 30, 0));
			selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 20, 30, 0));

			selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 50, 60, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 100, 200, 0));
			selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 100, 200, 0));

			var expectedSelection = new Rectangle(50, 60, 50, 140);
			var expectedInvalidations = new[]
			{
				new Rectangle(5, 10, 15, 20),
				// the union of the first selection and the second selection
				new Rectangle(5, 10, 95, 190)
			};

			Assert.AreEqual(expectedSelection, selectionEffect.Selection);
			CollectionAssert.AreEqual(expectedInvalidations, requestedInvalidateRectangles);
		}

		/// <summary>
		/// Tests that
		/// Selection is calculated correctly, Invalidated called twice and the second invalidated rectangle includes the first one
		/// if there were 2 left mouse down, 2 moves and 2 ups (two separate selections were made) in editing mode.
		/// The reverse case - when mouse moves from right bottom to left top corner
		/// </summary>
		[TestMethod]
		public void OnMouseMove_TwoSelectionAttemptsReverseDirection_InvalidatedCalledTwice()
		{
			var requestedInvalidateRectangles = new List<Rectangle>();
			var selectionEffect = new SelectionEffect();
			selectionEffect.Invalidated += (sender, args) => requestedInvalidateRectangles.Add(args.Rectangle);
			var editContext = new Mock<IEffectEditContext>();

			selectionEffect.StartEdit(editContext.Object);
			selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 20, 30, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 5, 10, 0));
			selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 5, 10, 0));

			selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 100, 200, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 50, 60, 0));
			selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 50, 60, 0));

			var expectedSelection = new Rectangle(50, 60, 50, 140);
			var expectedInvalidations = new[]
			{
				new Rectangle(5, 10, 15, 20),
				// the union of the first selection and the second selection
				new Rectangle(5, 10, 95, 190)
			};

			Assert.AreEqual(expectedSelection, selectionEffect.Selection);
			CollectionAssert.AreEqual(expectedInvalidations, requestedInvalidateRectangles);
		}
		#endregion

		#region CancelEdit
		/// <summary>
		/// Tests that
		/// Cancel edit restores previous selection and calls Invalidated.
		/// </summary>
		[TestMethod]
		public void CancelEdit_CommitThenCancel_SelectionIsRestored()
		{
			var requestedInvalidateRectangles = new List<Rectangle>();
			var selectionEffect = new SelectionEffect();
			selectionEffect.Invalidated += (sender, args) => requestedInvalidateRectangles.Add(args.Rectangle);
			var editContext = new Mock<IEffectEditContext>();

			selectionEffect.StartEdit(editContext.Object);
			selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 20, 30, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 5, 10, 0));
			selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 5, 10, 0));
			selectionEffect.CommitEdit();

			selectionEffect.StartEdit(editContext.Object);
			selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 100, 200, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 50, 60, 0));
			selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 50, 60, 0));
			selectionEffect.CancelEdit();

			var expectedSelection = new Rectangle(5, 10, 15, 20);
			var expectedInvalidations = new[]
			{
				new Rectangle(5, 10, 15, 20),
				// the union of the first selection and the second selection
				new Rectangle(5, 10, 95, 190),
				// cancel should invalidate union of both rectangles
				new Rectangle(5, 10, 95, 190)
			};

			Assert.AreEqual(expectedSelection, selectionEffect.Selection);
			CollectionAssert.AreEqual(expectedInvalidations, requestedInvalidateRectangles);
		}

		/// <summary>
		/// Tests that
		/// Cancel edit exits edit mode, Selection doesn't update and Invalidated is not called.
		/// </summary>
		[TestMethod]
		public void CancelEdit_MouseMoveAfterCancel_NoUpdates()
		{
			var requestedInvalidateRectangles = new List<Rectangle>();
			var selectionEffect = new SelectionEffect();
			selectionEffect.Invalidated += (sender, args) => requestedInvalidateRectangles.Add(args.Rectangle);
			var editContext = new Mock<IEffectEditContext>();

			selectionEffect.StartEdit(editContext.Object);
			selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 20, 30, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 5, 10, 0));
			selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 5, 10, 0));
			selectionEffect.CancelEdit();

			selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 100, 200, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 50, 60, 0));
			selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 50, 60, 0));

			var expectedSelection = Rectangle.Empty;
			var expectedInvalidations = new[]
			{
				new Rectangle(5, 10, 15, 20),
				new Rectangle(5, 10, 15, 20)
			};

			Assert.AreEqual(expectedSelection, selectionEffect.Selection);
			CollectionAssert.AreEqual(expectedInvalidations, requestedInvalidateRectangles);
		}

		/// <summary>
		/// Tests that
		/// Cancel edit without editing mode does nothing, Selection doesn't update and Invalidated is not called.
		/// </summary>
		[TestMethod]
		public void CancelEdit_AfterCommit_NoUpdates()
		{
			var requestedInvalidateRectangles = new List<Rectangle>();
			var selectionEffect = new SelectionEffect();
			selectionEffect.Invalidated += (sender, args) => requestedInvalidateRectangles.Add(args.Rectangle);
			var editContext = new Mock<IEffectEditContext>();

			selectionEffect.StartEdit(editContext.Object);
			selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 20, 30, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 5, 10, 0));
			selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 5, 10, 0));
			selectionEffect.CommitEdit();

			selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 100, 200, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 50, 60, 0));
			selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 50, 60, 0));
			selectionEffect.CancelEdit();

			var expectedSelection = new Rectangle(5, 10, 15, 20);
			var expectedInvalidations = new[]
			{
				new Rectangle(5, 10, 15, 20)
			};

			Assert.AreEqual(expectedSelection, selectionEffect.Selection);
			CollectionAssert.AreEqual(expectedInvalidations, requestedInvalidateRectangles);
		}
		#endregion

		#region CommitEdit
		/// <summary>
		/// Tests that
		/// Commit edit exits edit mode, Selection doesn't update and Invalidated is not called.
		/// </summary>
		[TestMethod]
		public void CommitEdit_MouseMoveAfterCommit_NoUpdates()
		{
			var requestedInvalidateRectangles = new List<Rectangle>();
			var selectionEffect = new SelectionEffect();
			selectionEffect.Invalidated += (sender, args) => requestedInvalidateRectangles.Add(args.Rectangle);
			var editContext = new Mock<IEffectEditContext>();

			selectionEffect.StartEdit(editContext.Object);
			selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 20, 30, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 5, 10, 0));
			selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 5, 10, 0));
			selectionEffect.CommitEdit();

			selectionEffect.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 100, 200, 0));
			selectionEffect.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 50, 60, 0));
			selectionEffect.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 50, 60, 0));

			var expectedSelection = new Rectangle(5, 10, 15, 20);
			var expectedInvalidations = new[]
			{
				new Rectangle(5, 10, 15, 20)
			};

			Assert.AreEqual(expectedSelection, selectionEffect.Selection);
			CollectionAssert.AreEqual(expectedInvalidations, requestedInvalidateRectangles);
		}
		#endregion
	}
}
