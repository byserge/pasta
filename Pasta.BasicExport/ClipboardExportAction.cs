using Pasta.Core;
using System;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

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
        public void ExportAsync(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            Clipboard.SetImage(image);
        }
    }
}
