using System;
using System.Drawing;

namespace Pasta.Core
{
    /// <summary>
    /// Stores event data for Invalidated event.
    /// </summary>
    [Serializable]
    public class InvalidatedEventArgs : EventArgs
    {
        /// <summary>
        /// Rectangle that's been invalidated and needs an update.
        /// </summary>
        public Rectangle Rectangle { get; private set; }

        /// <summary>
        /// Instantiates <see cref="InvalidatedEventArgs"/>.
        /// </summary>
        /// <param name="rectangle">The rectangle that's invalided.</param>
        public InvalidatedEventArgs(Rectangle rectangle)
            : base()
        {
            Rectangle = rectangle;
        }
    }
}
