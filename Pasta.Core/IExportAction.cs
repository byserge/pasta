using System.Drawing;
using System.Threading.Tasks;

namespace Pasta.Core
{
    /// <summary>
    /// Describes an action that can be applied to an image with effects.
    /// </summary>
    public interface IExportAction
    {
        /// <summary>
        /// Exports the image to some storage in async way.
        /// </summary>
        /// <param name="image">The image to export.</param>
        /// <returns>The task to be awaited on.</returns>
        Task ExportAsync(Image image);
    }
}
