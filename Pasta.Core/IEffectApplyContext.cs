using System.Drawing;

namespace Pasta.Core
{
    public interface IEffectApplyContext
    {
        /// <summary>
        /// The drawing surface to apply effects to.
        /// </summary>
        Graphics Graphics { get; }

        /// <summary>
        /// The bounds of drawing surface.
        /// </summary>
        Rectangle Bounds { get; }
    }
}
