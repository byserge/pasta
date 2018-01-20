using Pasta.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pasta.Editor.ExportActions
{
	/// <summary>
	/// Represents export action info used to create a new action.
	/// </summary>
	internal class ExportActionInfo
    {
        /// <summary>
        /// The export action name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The export action icon image.
        /// </summary>
        public Image IconImage { get; private set; }

        /// <summary>
        /// The export action constructor function.
        /// </summary>
        public Func<IExportAction> Constructor { get; }

		/// <summary>
		/// The keys to trigger the action.
		/// </summary>
		public Keys KeyShortcut { get;  }

		public ExportActionInfo(string name, Image iconImage, Func<IExportAction> constructor, Keys keys)
		{
			Name = name;
			IconImage = iconImage;
			Constructor = constructor;
			KeyShortcut = keys;
		}
	}
}
