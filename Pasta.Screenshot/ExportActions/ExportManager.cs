using Pasta.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Pasta.BasicExport;

namespace Pasta.Screenshot.ExportActions
{
    /// <summary>
    /// Manages export actions.
    /// </summary>
    public class ExportManager : IExportAction
    {
        /// <summary>
        /// The list of registered export actions.
        /// </summary>
        private List<IExportAction> registeredActions = new List<IExportAction>();

        public ExportManager()
        {
            Register(new ClipboardExportAction());
        }

        #region IExportAction
        public Task ExportAsync(Image image)
        {
            return registeredActions[0].ExportAsync(image);
        }
        #endregion

        /// <summary>
        /// Registers the given export action.
        /// </summary>
        /// <param name="exportAction">The action to register.</param>
        private void Register(IExportAction exportAction)
        {
            registeredActions.Add(exportAction);
        }
    }
}
