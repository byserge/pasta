using Pasta.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Pasta.Core
{
	public class EffectContext : MarshalByRefObject
	{
		private Dictionary<string, object> contextItems = new Dictionary<string, object>();

		public Graphics Graphics { get; set; }
		public Rectangle Bounds { get; set; }

		/// <summary>
		/// Triggered by Invlidate() when effects request to repaint a part of the screen.
		/// </summary>
		public event EventHandler<InvalidatedEventArgs> Invalidated;

		/// <summary>
		/// Context items stored for the session.
		/// </summary>
		/// <param name="key">The Item key.</param>
		/// <returns>The context item.</returns>
		public object this[string key]
		{
			get
			{
				if (!contextItems.TryGetValue(key, out var item))
					return null;
				return item;
			}
			set
			{
				contextItems[key] = value;
			}
		}

		public Image CreateImage(Stream stream)
		{
			return Image.FromStream(stream);
		}

		public void Invalidate(Rectangle rectangle)
		{
			Invalidated?.Invoke(this, new InvalidatedEventArgs(rectangle));
		}
	}
}