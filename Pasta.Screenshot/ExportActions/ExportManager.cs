using Pasta.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;

namespace Pasta.Screenshot.ExportActions
{
    /// <summary>
    /// Manages export actions.
    /// </summary>
    public class ExportManager
    {
        /// <summary>
        /// The list of registered export actions.
        /// </summary>
        private List<IExportAction> registeredActions = new List<IExportAction>();

        public ExportManager()
        {
        }

        #region IExportAction
        public Task ExportAsync(Image image)
        {
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return Task.Factory.StartNew(
                () => registeredActions[0].ExportAsync(image), 
                new CancellationToken(), 
                TaskCreationOptions.None, 
                scheduler);
        }
        #endregion

        /// <summary>
        /// Registers the given export action.
        /// </summary>
        /// <param name="exportAction">The action to register.</param>
        public void Register(IExportAction exportAction)
        {
            registeredActions.Add(exportAction);
        }

        /// <summary>
        /// Registers the given exprot actions.
        /// </summary>
        /// <param name="exportActions">The actions to export</param>
        public void Register(IEnumerable<IExportAction> exportActions)
        {
            foreach (var exportAction in exportActions)
                Register(exportAction);
        }
    }
}
