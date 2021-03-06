﻿using System.Drawing;
using System.Threading.Tasks;

namespace Pasta.Core
{
    /// <summary>
    /// Describes an action that can be applied to an image with effects.
    /// </summary>
    public interface IExportAction
    {
        /// <summary>
        /// Exports the image to some storage in sync way.
        /// Can be long-running operation, but can't use task here because Task can't pass AppDomain boundaries.
        /// </summary>
        /// <param name="context">The export context containing the image to export.</param>
        void Export(ExportContext context);
    }
}
