using Pasta.Core;
using System;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Pasta.BasicExport
{
    /// <summary>
    /// Action to export an image into the clipboard.
    /// </summary>
    public class ClipboardExportAction : IExportAction
    {
        /// <summary>
        /// Exports the image into the clipboard.
        /// </summary>
        /// <param name="image">The image to export.</param>
        /// <returns>The task to be awaited on.</returns>
        public Task ExportAsync(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            try
            {
                Clipboard.SetImage(image);
            }
            catch (System.Runtime.InteropServices.ExternalException ex)
            {
                return Task.FromException(ex);
            }
            catch (System.Threading.ThreadStateException ex)
            {
                return Task.FromException(ex);
            }

            return Task.CompletedTask;
        }
    }
}
