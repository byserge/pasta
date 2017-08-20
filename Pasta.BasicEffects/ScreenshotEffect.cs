using Pasta.Core;
using Screna;
using System;
using System.Drawing;

namespace Pasta.BasicEffects
{
    public class ScreenshotEffect : IEffect
    {
        private Bitmap screenCapture;

        public ScreenshotEffect()
        {
            screenCapture = ScreenShot.Capture();
        }

        public void Apply(IEffectApplyContext context)
        {
            var g = context.Graphics;
            g.DrawImage(screenCapture, Point.Empty);
        }
    }
}
