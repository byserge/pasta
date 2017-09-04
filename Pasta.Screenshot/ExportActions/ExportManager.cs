using Pasta.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Collections.ObjectModel;
using System;

namespace Pasta.Screenshot.ExportActions
{
    /// <summary>
    /// Manages export actions.
    /// </summary>
    public class ExportManager : IDisposable
    {
        /// <summary>
		/// Registered export action types information.
		/// </summary>
		private List<ExportActionInfo> actionsInfo = new List<ExportActionInfo>();

        /// <summary>
		/// Registered export actions info.
		/// </summary>
		public IReadOnlyCollection<ExportActionInfo> ActionsInfo { get; }

        public ExportManager()
        {
            ActionsInfo = new ReadOnlyCollection<ExportActionInfo>(actionsInfo);
        }

        /// <summary>
		/// Registers export action types.
		/// </summary>
		/// <param name="actionsInfo">Export actions info to create export action.</param>
		public void Register(IEnumerable<ExportActionInfo> actionsInfo)
        {
            this.actionsInfo.AddRange(actionsInfo);
        }

        public void Dispose()
        {
            actionsInfo.Clear();
        }
    }
}
