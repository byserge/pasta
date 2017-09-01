using Pasta.Core;
using Screna;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Pasta.BasicEffects
{
    /// <summary>
    /// Default screenshot effect.
    /// </summary>
    public class ScreenshotEffect : IScreenshotEffect, IEffect
    {
        public void Apply(EffectContext context)
        {
            var screenCapture = context["screenshotImage"] as Image;
            if (screenCapture == null)
            {
                return;
            }

            var g = context.Graphics;
            g.DrawImage(screenCapture, Point.Empty);
        }

        public void CaptureScreen(EffectContext context)
        {
            var screenshotImage = ScreenShot.Capture();
            using (var stream = new MemoryStream())
            {
                screenshotImage.Save(stream, ImageFormat.Png);
                var imageReference = context.CreateImage(stream);
                context["screenshotImage"] = imageReference;
            }
        }
    }
}
