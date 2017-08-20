using Pasta.Core;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System;

namespace Pasta.Screenshot.Effects
{
    internal class EffectsManager : IMouseAware, IMayInvalidate
    {
        private List<IEffect> effects = new List<IEffect>();

        /// <summary>
        /// The effect is being edited.
        /// </summary>
        private IEditableEffect EditableEffect => selectedEffect as IEditableEffect;
        
        /// <summary>
        /// The effect is currently selected.
        /// </summary>
        private IEffect selectedEffect;

        #region IMayInvalidate

        public event EventHandler<InvalidatedEventArgs> Invalidated;

        #endregion

        #region IMouseAware

        public void OnMouseDown(MouseEventArgs e)
        {
            foreach (var mouseAwareEffect in effects.OfType<IMouseAware>())
            {
                mouseAwareEffect.OnMouseDown(e);
            }
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            foreach (var mouseAwareEffect in effects.OfType<IMouseAware>())
            {
                mouseAwareEffect.OnMouseUp(e);
            }
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            foreach (var mouseAwareEffect in effects.OfType<IMouseAware>())
            {
                mouseAwareEffect.OnMouseMove(e);
            }
        }

        #endregion

        public void AddEffect(IEffect effect)
        {
            effects.Add(effect);
            SelectEffect(effect);
            SubscribeToEffectEvents(effect);
        }

        /// <summary>
        /// Creates an image and applies all effects to it.
        /// </summary>
        /// <param name="size">The size of the image.</param>
        /// <returns>The image with all effects applied.</returns>
        public Image CreateImage(Size size)
        {
            // Create an image and apply effects
            var bmp = new Bitmap(size.Width, size.Height);
            using (var graphics = Graphics.FromImage(bmp))
            {
                var bounds = new Rectangle(Point.Empty, size);
                this.ApplyEffects(graphics, bounds, bounds);
            }

            // Look for a selection effect to crop a part of image
            var selectionEffect = effects.OfType<ISelectionEffect>().FirstOrDefault();
            if (selectionEffect == null)
            {
                return bmp;
            }

            // Crop a part of image according to the selection effect.
            try
            {
                var selection = selectionEffect.Selection;
                return bmp.Clone(selection, bmp.PixelFormat);
            }
            finally
            {
                bmp.Dispose();
            }

        }

        public void RemoveEffect(IEffect effect)
        {
            bool needNewSelection = selectedEffect == effect;
            if (needNewSelection)
            {
                SelectEffect(null);
            }

            effects.Remove(effect);
            UnsubscribeFromEffectEvents(effect);

            if (needNewSelection)
            {
                var effectToSelect = effects.Count == 0 ? null : effects[effects.Count - 1];
                SelectEffect(effectToSelect);
            }
        }

        public void ApplyEffects(Graphics graphics, Rectangle bounds, Rectangle clipBounds)
        {
            graphics.FillRectangle(Brushes.White, bounds);
            var context = new EffectApplyContext(graphics, bounds);
            effects.ForEach(effect => effect.Apply(context));
        }

        private void SelectEffect(IEffect effect)
        {
            if (effect == selectedEffect)
            {
                return;
            }

            EndEdit();

            selectedEffect = effect;

            StartEdit();
        }

        private void StartEdit()
        {
            if (EditableEffect != null)
            {
                EditableEffect.StartEdit(new EffectEditContext());
            }
        }

        private void EndEdit()
        {
            if (EditableEffect != null)
            {
                EditableEffect.CommitEdit();
            }
        }

        private void SubscribeToEffectEvents(IEffect effect)
        {
            var mayInvalidate = effect as IMayInvalidate;
            if (mayInvalidate != null)
            {
                mayInvalidate.Invalidated += Effect_Invalidated;
            }
        }

        private void UnsubscribeFromEffectEvents(IEffect effect)
        {
            var mayInvalidate = effect as IMayInvalidate;
            if (mayInvalidate != null)
            {
                mayInvalidate.Invalidated -= Effect_Invalidated;
            }
        }

        private void Effect_Invalidated(object sender, InvalidatedEventArgs e)
        {
            Invalidated?.Invoke(this, e);
        }
    }
}
