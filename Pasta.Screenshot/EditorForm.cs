using Pasta.Core;
using Pasta.Plugin;
using Pasta.Screenshot.Effects;
using Pasta.Screenshot.ExportActions;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Pasta.Screenshot
{
    public partial class EditorForm : Form
    {
        private EffectsManager effectsManager;
        private ExportManager exportManager;
        private PluginManager pluginManager;

        public EditorForm()
        {
            InitializeComponent();

            InitializePlugins();

            effectsManager = new EffectsManager();
            RegisterEffects();

            exportManager = new ExportManager();
            RegisterExportActions();
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

        private void RegisterEffects()
        {
            var additionalInterfaces = new[]
                {
                    typeof(IEffect),
                    typeof(IMouseAware),
                    typeof(IEditableEffect)
                };

            foreach (var plugin in pluginManager.Plugins)
            {
                var selectionTypes = plugin.GetPluginTypes<ISelectionEffect>();
                var screenshotTypes = plugin.GetPluginTypes<IScreenshotEffect>();

                var effectTypes = plugin.GetPluginTypes<IEffect>();
                var effectConstructors = effectTypes
                    .Except(selectionTypes)
                    .Except(screenshotTypes)
                    .Select(type => new Func<IEffect>(() => plugin.CreatePlugin<IEffect>(type, additionalInterfaces)));
                var selectionConstructors = selectionTypes
                    .Select(type => new Func<ISelectionEffect>(() => plugin.CreatePlugin<ISelectionEffect>(type, additionalInterfaces)));
                var screenshotConstructors = screenshotTypes
                    .Select(type => new Func<IScreenshotEffect>(() => plugin.CreatePlugin<IScreenshotEffect>(type, additionalInterfaces)));
                effectsManager.Register(effectConstructors);
                effectsManager.Register(selectionConstructors);
                effectsManager.Register(screenshotConstructors);
            }
        }

        private void RegisterExportActions()
        {
            foreach (var plugin in pluginManager.Plugins)
            {
                var actionTypes = plugin.GetPluginTypes<IExportAction>();
                var actions = actionTypes.Select(actionType => plugin.CreatePlugin<IExportAction>(actionType));
                exportManager.Register(actions);
            }
        }

        private void EditorForm_Load(object sender, EventArgs e)
        {
            effectsManager.Invalidated += Effects_Invalidated;            
            PrintScreen();
        }

        private void EditorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
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

        private async void EditorForm_DoubleClick(object sender, EventArgs e)
        {
            using (var image = effectsManager.CreateImage(this.Size))
            {
                await exportManager.ExportAsync(image);
            }

            Close();
        }

        private void EditorForm_Paint(object sender, PaintEventArgs e)
        {
            effectsManager.ApplyEffects(e.Graphics, this.Bounds, e.ClipRectangle);
        }

        private void EditorForm_MouseDown(object sender, MouseEventArgs e)
        {
            effectsManager.OnMouseDown(new MouseAwareArgs(e));
        }

        private void EditorForm_MouseUp(object sender, MouseEventArgs e)
        {
            effectsManager.OnMouseUp(new MouseAwareArgs(e));
        }

        private void EditorForm_MouseMove(object sender, MouseEventArgs e)
        {
            effectsManager.OnMouseMove(new MouseAwareArgs(e));
        }

        private void Effects_Invalidated(object sender, InvalidatedEventArgs e)
        {
            this.Invalidate(e.Rectangle, false);
        }
    }
}
