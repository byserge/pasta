using Pasta.Core;
using Pasta.Plugin;
using Pasta.Screenshot.Effects;
using Pasta.Screenshot.ExportActions;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pasta.Screenshot
{
	public partial class EditorForm : Form
	{
		private EffectsManager effectsManager;
		private ExportManager exportManager;
		private PluginManager pluginManager;

		private EffectInfo selectedEffectInfo;

		public EditorForm()
		{
			InitializeComponent();

			InitializePlugins();

			effectsManager = new EffectsManager();
			RegisterEffects();

			exportManager = new ExportManager();
			RegisterExportActions();

			UpdateEffectsToolbar();
            UpdateActionsToolbar();
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
					ImageScaling = ToolStripItemImageScaling.None
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
                    ImageScaling = ToolStripItemImageScaling.None
                };

                actionsToolStrip.Items.Add(button);
            }
        }

        private void InitializePlugins()
		{
			pluginManager = new PluginManager();
			var pluginInterfaces = new[]
			{
				typeof(IEffect),
				typeof(IEditableEffect),
				typeof(IExportAction)
			};
			pluginManager.LoadFrom("Plugins", pluginInterfaces);
		}

        private static Type[] additionalInterfaces = new[]
                {
                    typeof(IEffect),
                    typeof(IMouseAware),
                    typeof(IEditableEffect)
                };

        private void RegisterEffects()
		{
			foreach (var plugin in pluginManager.Plugins)
            {
                var selectionTypes = plugin.GetPluginTypes<ISelectionEffect>();
                var screenshotTypes = plugin.GetPluginTypes<IScreenshotEffect>();

                var effectTypes = plugin.GetPluginTypes<IEffect>();
                var effectsInfo = effectTypes
                    .Except(selectionTypes)
                    .Except(screenshotTypes)
                    .Select(type => BuildEffectInfo(type, plugin));

                var selectionConstructors = selectionTypes
                    .Select(type => new Func<ISelectionEffect>(() => plugin.CreatePlugin<ISelectionEffect>(type, additionalInterfaces)));
                var screenshotConstructors = screenshotTypes
                    .Select(type => new Func<IScreenshotEffect>(() => plugin.CreatePlugin<IScreenshotEffect>(type, additionalInterfaces)));
                effectsManager.Register(effectsInfo);
                effectsManager.Register(selectionConstructors);
                effectsManager.Register(screenshotConstructors);
            }
        }

        private static EffectInfo BuildEffectInfo(string type, PluginHost plugin)
        {
            var name = type.Split('.').Last();
            var resourceName = name + ".png";
            Stream stream;
            try
            {
                stream = plugin.GetPluginResourceStream(resourceName);
            }
            catch (PluginException)
            {
                // TODO: log exception
                stream = null;
            }

            var image = stream == null ? null : Image.FromStream(stream);
            return new EffectInfo(
                name,
                image,
                new Func<IEffect>(() => plugin.CreatePlugin<IEffect>(type, additionalInterfaces)));
        }

        private static ExportActionInfo BuildExprotActionInfo(string type, PluginHost plugin)
        {
            var name = type.Split('.').Last();
            var resourceName = name + ".png";
            Stream stream;
            try
            {
                stream = plugin.GetPluginResourceStream(resourceName);
            }
            catch (PluginException)
            {
                // TODO: log exception
                stream = null;
            }

            var image = stream == null ? null : Image.FromStream(stream);
            return new ExportActionInfo(
                name,
                image,
                new Func<IExportAction>(() => plugin.CreatePlugin<IExportAction>(type)));
        }

        private void RegisterExportActions()
		{
			foreach (var plugin in pluginManager.Plugins)
			{
				var actionTypes = plugin.GetPluginTypes<IExportAction>();
				var actionsInfo = actionTypes.Select(actionType => BuildExprotActionInfo(actionType, plugin));
				exportManager.Register(actionsInfo);
			}
		}

		private void EditorForm_Load(object sender, EventArgs e)
		{
			effectsManager.Invalidated += Effects_Invalidated;
			PrintScreen();
		}

		private void EditorForm_FormClosed(object sender, FormClosedEventArgs e)
		{
            effectsManager.Dispose();
            exportManager.Dispose();
			pluginManager.Dispose();
			effectsManager.Invalidated -= Effects_Invalidated;
		}

		private void PrintScreen()
		{
			var bounds = Screen.AllScreens
				.Select(screen => screen.Bounds)
				.Aggregate((acc, screenBounds) => Rectangle.Union(acc, screenBounds));
			this.Location = bounds.Location;
			this.Size = bounds.Size;

			effectsManager.CaptureScreen();
			effectsManager.StartSelection();
			Invalidate(false);
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

		private void effectsToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			var button = e.ClickedItem as ToolStripButton;
			if (button == null)
			{
				return;
			}

			button.Checked = !button.Checked;
			if (button.Checked)
			{
				selectedEffectInfo = button.Tag as EffectInfo;
			}
			else
			{
				selectedEffectInfo = null;
			}
		}

        private async void actionsToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var button = e.ClickedItem as ToolStripButton;
            if (button == null)
            {
                return;
            }

            var actionInfo = button.Tag as ExportActionInfo;

            await RunExportAction(actionInfo);

            Close();
        }
    }
}
