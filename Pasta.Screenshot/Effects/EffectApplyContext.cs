using Pasta.Core;
using System.Drawing;

namespace Pasta.Screenshot.Effects
{
    internal class EffectApplyContext : IEffectApplyContext
    {
        private Graphics graphics;
        private Rectangle bounds;

        public Graphics Graphics => graphics;
        public Rectangle Bounds => bounds;

        public EffectApplyContext(Graphics graphics, Rectangle bounds)
        {
            this.graphics = graphics;
            this.bounds = bounds;
        }
    }
}