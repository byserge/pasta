using Pasta.Core;
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
        /// <param name="context">The export context containing the image to export.</param>
        public void Export(ExportContext context)
        {
            Clipboard.SetImage(context.Image);
        }
    }
}
