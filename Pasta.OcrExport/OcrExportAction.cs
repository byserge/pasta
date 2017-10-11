using System;
using System.Diagnostics;
using Pasta.Core;
using System.Linq;
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
		/// To add more languages also modify Pasta.Screenshot post build events to copy corresponding traindata to output.
		/// The languanges to try to parse.
		/// </summary>
		private readonly string[] languages = { "eng", "rus" };

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
			var text = languages
				.Select(lang => ExtractImageTextInLanguage(tifImage, lang))
				.OrderByDescending(pair => pair.Mean)
				.First()
				.Text;
			return text;
		}

		private static (string Text, double Mean) ExtractImageTextInLanguage(byte[] tifImage, string lang)
		{
			using (var engine = new TesseractEngine(@"./tessdata", lang, EngineMode.Default))
			{
				using (var img = Pix.LoadTiffFromMemory(tifImage))
				{
					using (var page = engine.Process(img))
					{
						var text = page.GetText();
						var mean = page.GetMeanConfidence();
						return (text, mean);
					}
				}
			}
		}
	}
}
