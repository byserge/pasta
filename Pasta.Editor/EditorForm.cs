using Pasta.Core;
using Pasta.Editor.Effects;
using Pasta.Editor.ExportActions;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pasta.Editor
{
	internal partial class EditorForm : Form
	{
		private EffectInfo selectedEffectInfo;
		private readonly EffectsManager effectsManager;
		private readonly ExportManager exportManager;

		public EditorForm(EffectsManager effectsManager, ExportManager exportManager)
		{
			this.effectsManager = effectsManager;
			this.exportManager = exportManager;

			InitializeComponent();

			UpdateEffectsToolbar();
			UpdateActionsToolbar();

#if !DEBUG
            this.TopMost = true;
#endif
		}

		/// <summary>
		/// Takes a screenshot.
		/// </summary>
		public void PrintScreen()
		{
			this.effectsManager.Clear();
			effectsManager.CaptureScreen();
			effectsManager.StartSelection();
		}

		private void UpdateEffectsToolbar()
		{
			effectsToolStrip.Items.Clear();
			foreach (var effectInfo in effectsManager.EffectsInfo)
			{
				var button = new ToolStripButton
				{
					Text = effectInfo.Name,
					Tag = effectInfo,
					Image = effectInfo.IconImage,
					DisplayStyle = ToolStripItemDisplayStyle.Image,
					ImageScaling = ToolStripItemImageScaling.SizeToFit
				};

				effectsToolStrip.Items.Add(button);
			}
		}

		private void UpdateActionsToolbar()
		{
			actionsToolStrip.Items.Clear();
			foreach (var actionInfo in exportManager.ActionsInfo)
			{
				var button = new ToolStripButton
				{
					Text = actionInfo.Name,
					Tag = actionInfo,
					Image = actionInfo.IconImage,
					DisplayStyle = ToolStripItemDisplayStyle.Image,
					ImageScaling = ToolStripItemImageScaling.SizeToFit,
				};

				actionsToolStrip.Items.Add(button);
			}
		}

		private void EditorForm_Load(object sender, EventArgs e)
		{
			effectsManager.Invalidated += Effects_Invalidated;

			var bounds = Screen.AllScreens
				.Select(screen => screen.Bounds)
				.Aggregate(Rectangle.Union);
			this.Location = bounds.Location;
			this.Size = bounds.Size;
		}

		private void EditorForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			effectsManager.Invalidated -= Effects_Invalidated;
		}

		private async Task RunExportAction(ExportActionInfo actionInfo)
		{
			using (var image = effectsManager.CreateImage(this.Size))
			{
				var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
				await Task.Factory.StartNew(
					() =>
					{
						var action = actionInfo.Constructor();
						var context = new ExportContext(image);
						action.Export(context);
					},
					new CancellationToken(),
					TaskCreationOptions.None,
					scheduler);
			}
		}

		private void EditorForm_Paint(object sender, PaintEventArgs e)
		{
			effectsManager.ApplyEffects(e.Graphics, this.Bounds, e.ClipRectangle);
		}

		private void EditorForm_MouseDown(object sender, MouseEventArgs e)
		{
			// Effect info is selected -> create new effect mode
			if (selectedEffectInfo != null)
			{
				var effect = selectedEffectInfo.Constructor();
				effectsManager.AddEffect(effect);
			}

			effectsManager.OnMouseDown(new MouseAwareArgs(e));
		}

		private void EditorForm_MouseUp(object sender, MouseEventArgs e)
		{
			effectsManager.OnMouseUp(new MouseAwareArgs(e));

			// Effect info is selected -> create new effect mode
			if (selectedEffectInfo != null)
			{
				effectsManager.EndEdit();
			}

		}

		private void EditorForm_MouseMove(object sender, MouseEventArgs e)
		{
			effectsManager.OnMouseMove(new MouseAwareArgs(e));
		}

		private void Effects_Invalidated(object sender, InvalidatedEventArgs e)
		{
			this.Invalidate(e.Rectangle, false);
		}

		private void EffectsToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			var button = e.ClickedItem as ToolStripButton;
			if (button == null)
			{
				return;
			}

			button.Checked = !button.Checked;
			if (button.Checked)
			{
				// Uncheck all other buttons
				effectsToolStrip.Items
					.OfType<ToolStripButton>()
					.Where(b => b != button)
					.ToList()
					.ForEach(b => b.Checked = false);
				selectedEffectInfo = button.Tag as EffectInfo;
			}
			else
			{
				selectedEffectInfo = null;
			}
		}

		private async void ActionsToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (!(e.ClickedItem is ToolStripButton button))
			{
				return;
			}

			if (SynchronizationContext.Current == null)
			{
				SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
			}

			var actionInfo = button.Tag as ExportActionInfo;

			await RunExportAction(actionInfo);

			Close();
		}

		private void EditorForm_Deactivate(object sender, EventArgs e)
		{
			if (!Disposing)
				Close();
		}

		private async void EditorForm_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Escape)
			{
				Close();
				return;
			}
		}

		private async void EditorForm_KeyDown(object sender, KeyEventArgs e)
		{
			var action = exportManager.ActionsInfo.FirstOrDefault(x => x.KeyShortcut == e.KeyData);
			if (action != null)
			{
				if (SynchronizationContext.Current == null)
				{
					SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
				}

				await RunExportAction(action);
				Close();
				return;
			}
		}
	}
}
