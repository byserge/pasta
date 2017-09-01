using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace Pasta.BasicEffects.UnitTests
{
	/// <summary>
	/// Unit tests for <see cref="RectangleExtensions"/>.
	/// </summary>
	[TestClass]
	public class RectangleExtensionsTests
	{
		#region Except

		/// <summary>
		/// Tests that 
		/// the difference of two non-intersecting rectangles should be the first of them (the minuend).
		/// </summary>
		[TestMethod]
		public void Except_NonIntersectedRectangles_MinuendIsReturned()
		{
			var minuend = new Rectangle(0, 0, 10, 10);
			var subtrahend = new Rectangle(11, 11, 10, 10);
			var expected = new[] { minuend };

			var difference = RectangleExtensions.Except(minuend, subtrahend);

			CollectionAssert.AreEqual(expected, difference);
		}

		/// <summary>
		/// Tests that
		/// if two rectangles have one vertex inside the other rectangle 
		/// then difference should consists of two rectangles.
		/// Option 1: subtrahend.X > minuned.X and subtrahend.Y > minuend.Y.
		/// </summary>
		[TestMethod]
		public void Except_AngleIntersectedRectanglesOption1_TwoRectanglesAreReturned()
		{
			var minuend = new Rectangle(-5, -5, 10, 10);
			var subtrahend = new Rectangle(0, 0, 6, 6);
			var expected = new[]
			{
				new Rectangle(-5, -5, 5, 10),
				new Rectangle(0, -5, 5, 5)
			};

			var difference = RectangleExtensions.Except(minuend, subtrahend);

			CollectionAssert.AreEqual(expected, difference);
		}

		/// <summary>
		/// Tests that
		/// if two rectangles have one vertex inside the other rectangle 
		/// then difference should consists of two rectangles.
		/// Option 2: subtrahend.X > minuned.X and subtrahend.Y < minuend.Y.
		/// </summary>
		[TestMethod]
		public void Except_AngleIntersectedRectanglesOption2_TwoRectanglesAreReturned()
		{
			var minuend = new Rectangle(-5, 0, 10, 10);
			var subtrahend = new Rectangle(0, -5, 6, 6);
			var expected = new[]
			{
				new Rectangle(-5, 0, 5, 10),
				new Rectangle(0, 1, 5, 9)
			};

			var difference = RectangleExtensions.Except(minuend, subtrahend);

			CollectionAssert.AreEqual(expected, difference);
		}

		/// <summary>
		/// Tests that
		/// if two rectangles have one vertex inside the other rectangle 
		/// then difference should consists of two rectangles.
		/// Option 3: subtrahend.X < minuned.X and subtrahend.Y > minuend.Y.
		/// </summary>
		[TestMethod]
		public void Except_AngleIntersectedRectanglesOption3_TwoRectanglesAreReturned()
		{
			var minuend = new Rectangle(1, 1, 3, 4);
			var subtrahend = new Rectangle(0, 2, 2, 6);
			var expected = new[]
			{
				new Rectangle(2, 1, 2, 4),
				new Rectangle(1, 1, 1, 1)
			};

			var difference = RectangleExtensions.Except(minuend, subtrahend);

			CollectionAssert.AreEqual(expected, difference);
		}

		/// <summary>
		/// Tests that
		/// if two rectangles have one vertex inside the other rectangle 
		/// then difference should consists of two rectangles.
		/// Option 4: subtrahend.X < minuned.X and subtrahend.Y < minuend.Y.
		/// </summary>
		[TestMethod]
		public void Except_AngleIntersectedRectanglesOption4_TwoRectanglesAreReturned()
		{
			var minuend = new Rectangle(0, 0, 10, 5);
			var subtrahend = new Rectangle(-3, -3, 8, 5);
			var expected = new[]
			{
				new Rectangle(5, 0, 5, 5),
				new Rectangle(0, 2, 5, 3)
			};

			var difference = RectangleExtensions.Except(minuend, subtrahend);

			CollectionAssert.AreEqual(expected, difference);
		}

		/// <summary>
		/// Tests that
		/// if the minuend contains only two vertex the subtrahend
		/// then the difference should contain three rectangles.
		/// Option 1: subtrahend Xs are generally greater.
		/// </summary>
		[TestMethod]
		public void Except_SideIntersectedRectanglesOption1_ThreeRectanglesAreReturned()
		{
			var minuend = new Rectangle(0, 0, 5, 10);
			var subtrahend = new Rectangle(2, 2, 100, 5);
			var expected = new[]
			{
				new Rectangle(0, 0, 2, 10),
				new Rectangle(2, 0, 3, 2),
				new Rectangle(2, 7, 3, 3)
			};

			var difference = RectangleExtensions.Except(minuend, subtrahend);

			CollectionAssert.AreEqual(expected, difference);
		}

		/// <summary>
		/// Tests that
		/// if the minuend contains only two vertex the subtrahend
		/// then the difference should contain three rectangles.
		/// Option 2: subtrahend Xs are generally less.
		/// </summary>
		[TestMethod]
		public void Except_SideIntersectedRectanglesOption2_ThreeRectanglesAreReturned()
		{
			var minuend = new Rectangle(0, 0, 5, 10);
			var subtrahend = new Rectangle(-100, 3, 102, 5);
			var expected = new[]
			{
				new Rectangle(2, 0, 3, 10),
				new Rectangle(0, 0, 2, 3),
				new Rectangle(0, 8, 2, 2)
			};

			var difference = RectangleExtensions.Except(minuend, subtrahend);

			CollectionAssert.AreEqual(expected, difference);
		}

		/// <summary>
		/// Tests that
		/// if the minuend contains only two vertex the subtrahend
		/// then the difference should contain three rectangles.
		/// Option 3: subtrahend Ys are generally greater.
		/// </summary>
		[TestMethod]
		public void Except_SideIntersectedRectanglesOption3_ThreeRectanglesAreReturned()
		{
			var minuend = new Rectangle(0, 0, 10, 5);
			var subtrahend = new Rectangle(2, 3, 1, 6);
			var expected = new[]
			{
				new Rectangle(0, 0, 2, 5),
				new Rectangle(3, 0, 7, 5),
				new Rectangle(2, 0, 1, 3)
			};

			var difference = RectangleExtensions.Except(minuend, subtrahend);

			CollectionAssert.AreEqual(expected, difference);
		}

		/// <summary>
		/// Tests that
		/// if the minuend contains only two vertex the subtrahend
		/// then the difference should contain three rectangles.
		/// Option 4: subtrahend Ys are generally less.
		/// </summary>
		[TestMethod]
		public void Except_SideIntersectedRectanglesOption4_ThreeRectanglesAreReturned()
		{
			var minuend = new Rectangle(0, 0, 10, 5);
			var subtrahend = new Rectangle(2, -3, 1, 6);
			var expected = new[]
			{
				new Rectangle(0, 0, 2, 5),
				new Rectangle(3, 0, 7, 5),
				new Rectangle(2, 3, 1, 2)
			};

			var difference = RectangleExtensions.Except(minuend, subtrahend);

			CollectionAssert.AreEqual(expected, difference);
		}

		/// <summary>
		/// Tests that
		/// if the minuend and the subtrahend form a cross
		/// then the difference should contain two rectangles.
		/// </summary>
		[TestMethod]
		public void Except_CrossIntersectedRectangles_TwoRectanglesAreReturned()
		{
			var minuend = new Rectangle(-2, -8, 4, 16);
			var subtrahend = new Rectangle(-8, -1, 16, 4);
			var expected = new[]
			{
				new Rectangle(-2, -8, 4, 7),
				new Rectangle(-2, 3, 4, 5)
			};

			var difference = RectangleExtensions.Except(minuend, subtrahend);

			CollectionAssert.AreEqual(expected, difference);
		}

		/// <summary>
		/// Tests that
		/// if the minuend contains the subtrahend
		/// then the difference should contain four rectangles.
		/// </summary>
		[TestMethod]
		public void Except_MinuendContainsSubtrahend_FourRectanglesAreReturned()
		{
			var minuend = new Rectangle(0, 0, 20, 40);
			var subtrahend = new Rectangle(3, 6, 7, 20);
			var expected = new[]
			{
				new Rectangle(0, 0, 3, 40),
				new Rectangle(10, 0, 10, 40),
				new Rectangle(3, 0, 7, 6),
				new Rectangle(3, 26, 7, 14)
			};

			var difference = RectangleExtensions.Except(minuend, subtrahend);

			CollectionAssert.AreEqual(expected, difference);
		}
		#endregion

		#region FromPoints

		/// <summary>
		/// Tests that
		/// If the first point is the top left corner then the rectangle is built correctly.
		/// </summary>
		[TestMethod]
		public void FromPoints_LeftTopStartPoint_CorrectRectangleIsBuilt()
		{
			var a = new Point(10, 11);
			var b = new Point(22, 25);
			var expected = new Rectangle(10, 11, 12, 14);

			var actual = RectangleExtensions.FromPoints(a, b);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Tests that
		/// If the first point is the top right corner then the rectangle is built correctly.
		/// </summary>
		[TestMethod]
		public void FromPoints_RightTopStartPoint_CorrectRectangleIsBuilt()
		{
			var a = new Point(10, 11);
			var b = new Point(-10, 25);
			var expected = new Rectangle(-10, 11, 20, 14);

			var actual = RectangleExtensions.FromPoints(a, b);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Tests that
		/// If the first point is the bottom left corner then the rectangle is built correctly.
		/// </summary>
		[TestMethod]
		public void FromPoints_LeftBottomStartPoint_CorrectRectangleIsBuilt()
		{
			var a = new Point(10, 11);
			var b = new Point(22, -1);
			var expected = new Rectangle(10, -1, 12, 12);

			var actual = RectangleExtensions.FromPoints(a, b);

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Tests that
		/// If the first point is the bottom right corner then the rectangle is built correctly.
		/// </summary>
		[TestMethod]
		public void FromPoints_RightBottomStartPoint_CorrectRectangleIsBuilt()
		{
			var a = new Point(10, 11);
			var b = new Point(-2, -1);
			var expected = new Rectangle(-2, -1, 12, 12);

			var actual = RectangleExtensions.FromPoints(a, b);

			Assert.AreEqual(expected, actual);
		}
		#endregion
	}
}
