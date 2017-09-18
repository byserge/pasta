using Pasta.Core;
using System;
using System.Windows.Forms;
using Tesseract;

namespace Pasta.OcrExport
{
    /// <summary>
    /// Action to parse text from the image
    /// </summary>
    public class OcrExportAction : IExportAction
    {
        /// <summary>
        /// Exports the image into the clipboard.
        /// </summary>
        /// <param name="context">The export context containing the image to export.</param>
        public void Export(ExportContext context)
        {
            var imageBytes = context.CreateImageByteArray(System.Drawing.Imaging.ImageFormat.Tiff.Guid);
            Clipboard.SetText(ParseImageText(imageBytes));
        }

        private string ParseImageText(byte[] tifImage)
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadTiffFromMemory(tifImage))
                {
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        return text;
                    }
                }
            }
        }
    }
}
