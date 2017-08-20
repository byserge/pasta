using System;

namespace Pasta.Core
{
    /// <summary>
    /// Describes objects that may trigger invalidation.
    /// </summary>
    public interface IMayInvalidate
    {
        /// <summary>
        /// Raised when the object changes so it requires repainting.
        /// </summary>
        event EventHandler<InvalidatedEventArgs> Invalidated;
    }
}
