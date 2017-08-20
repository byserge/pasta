using System.Drawing;

namespace Pasta.Core
{
    /// <summary>
    /// Describes an effect that selects part of image.
    /// </summary>
    public interface ISelectionEffect
    {
        /// <summary>
        /// The selection rectangle.
        /// </summary>
        Rectangle Selection { get; }
    }
}
