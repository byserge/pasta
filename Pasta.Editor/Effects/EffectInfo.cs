using Pasta.Core;
using System;
using System.Drawing;

namespace Pasta.Editor.Effects
{
	/// <summary>
	/// Represents effect info used to create a new effect.
	/// </summary>
	internal class EffectInfo
	{
		/// <summary>
		/// The effect name
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// The effect icon image.
		/// </summary>
		public Image IconImage { get; private set; }

		/// <summary>
		/// The effect constructor function.
		/// </summary>
		public Func<IEffect> Constructor { get; private set; }

		public EffectInfo(string name, Image iconImage, Func<IEffect> constructor)
		{
			Name = name;
			IconImage = iconImage;
			Constructor = constructor;
		}
	}
}
