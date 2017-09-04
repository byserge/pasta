using Pasta.Core;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System;
using System.Collections.ObjectModel;

namespace Pasta.Screenshot.Effects
{
	internal class EffectsManager : MarshalByRefObject, IDisposable
	{
		/// <summary>
		/// Registered effect types information.
		/// </summary>
		private List<EffectInfo> effectsInfo = new List<EffectInfo>();


		private Func<ISelectionEffect> selectionConstructor;
		private Func<IScreenshotEffect> screenshotConstructor;

		private List<IEffect> effects = new List<IEffect>();

		/// <summary>
		/// The effect is being edited.
		/// </summary>
		private IEditableEffect EditableEffect => selectedEffect as IEditableEffect;

		/// <summary>
		/// The effect is currently selected.
		/// </summary>
		private IEffect selectedEffect;

		private EffectContext context = new EffectContext();

		public event EventHandler<InvalidatedEventArgs> Invalidated;

		/// <summary>
		/// Registered effects info.
		/// </summary>
		public IReadOnlyCollection<EffectInfo> EffectsInfo { get; }

		public EffectsManager()
		{
			EffectsInfo = new ReadOnlyCollection<EffectInfo>(effectsInfo);
			context.Invalidated += Effect_Invalidated;
		}

		#region IMouseAware

		public void OnMouseDown(MouseAwareArgs e)
		{
			foreach (var mouseAwareEffect in effects.OfType<IMouseAware>())
			{
				mouseAwareEffect.OnMouseDown(e);
			}
		}

		public void OnMouseUp(MouseAwareArgs e)
		{
			foreach (var mouseAwareEffect in effects.OfType<IMouseAware>())
			{
				mouseAwareEffect.OnMouseUp(e);
			}
		}

		public void OnMouseMove(MouseAwareArgs e)
		{
			foreach (var mouseAwareEffect in effects.OfType<IMouseAware>())
			{
				mouseAwareEffect.OnMouseMove(e);
			}
		}

		#endregion

		public void AddEffect(IEffect effect)
		{
			if (effect == null)
			{
				throw new ArgumentNullException(nameof(effect));
			}

			effects.Add(effect);
			SelectEffect(effect);
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

		/// <summary>
		/// Registers effect types.
		/// </summary>
		/// <param name="effectsInfo">Effects info to create effects.</param>
		public void Register(IEnumerable<EffectInfo> effectsInfo)
		{
			this.effectsInfo.AddRange(effectsInfo);
		}

		/// <summary>
		/// Captures the entire screen.
		/// </summary>
		public void CaptureScreen()
		{
			var screenshotEffect = screenshotConstructor();
			screenshotEffect.CaptureScreen(context);
			AddEffect(screenshotEffect as IEffect);
		}

		/// <summary>
		/// Toogles selection mode.
		/// </summary>
		public void StartSelection()
		{
			var selectionEffect = selectionConstructor();
			AddEffect(selectionEffect as IEffect);
		}

		/// <summary>
		/// Registers selection effect types.
		/// </summary>
		/// <param name="effectConstructors">Contsturctors to create an effect</param>
		public void Register(IEnumerable<Func<ISelectionEffect>> effectConstructors)
		{
			var constructor = effectConstructors.FirstOrDefault();
			if (constructor != null)
			{
				this.selectionConstructor = constructor;
			}
		}

		/// <summary>
		/// Registers screenshot effect types.
		/// </summary>
		/// <param name="effectConstructors">Contsturctors to create an effect</param>
		public void Register(IEnumerable<Func<IScreenshotEffect>> effectConstructors)
		{
			var constructor = effectConstructors.FirstOrDefault();
			if (constructor != null)
			{
				this.screenshotConstructor = constructor;
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

			if (needNewSelection)
			{
				var effectToSelect = effects.Count == 0 ? null : effects[effects.Count - 1];
				SelectEffect(effectToSelect);
			}
		}

		public void ApplyEffects(Graphics graphics, Rectangle bounds, Rectangle clipBounds)
		{
			context.Graphics = graphics;
			context.Bounds = bounds;
			graphics.FillRectangle(Brushes.White, bounds);

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
				EditableEffect.StartEdit(context);
			}
		}

		public void EndEdit()
		{
			if (EditableEffect != null)
			{
				EditableEffect.CommitEdit();
			}
		}

		private void Effect_Invalidated(object sender, InvalidatedEventArgs e)
		{
			Invalidated?.Invoke(this, e);
		}

        public void Dispose()
        {
            effects.Clear();
            effectsInfo.Clear();
            selectionConstructor = null;
            screenshotConstructor = null;
        }
    }
}
