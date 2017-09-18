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
    public partial class MainForm : Form
    {
        private KeyboardHook keyboardHook;

        private EffectsManager effectsManager;
        private ExportManager exportManager;
        private PluginManager pluginManager;

        private bool isClosing = false;

        private AboutForm aboutForm;

        public MainForm()
        {
            InitializeComponent();

            InitializePlugins();

            effectsManager = new EffectsManager();
            RegisterEffects();

            exportManager = new ExportManager();
            RegisterExportActions();

            InitializeKeyboardHook();
        }

        private void InitializeKeyboardHook()
        {
            keyboardHook = new KeyboardHook(keys => keys == Keys.PrintScreen);
            keyboardHook.OnKeyHook += KeyboardHook_OnKeyHook;
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

        private void KeyboardHook_OnKeyHook(object sender, KeyEventArgs e)
        {
            var editorForm = new EditorForm(this.effectsManager, this.exportManager);
            editorForm.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isClosing = true;
            Close();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (keyboardHook != null)
            {
                keyboardHook.Dispose();
            }

            effectsManager.Dispose();
            exportManager.Dispose();
            pluginManager.Dispose();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !isClosing)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (aboutForm != null)
            {
                aboutForm.Activate();
                return;
            }

            using (aboutForm = new AboutForm())
            {
                aboutForm.ShowDialog(this);
                aboutForm = null;
            }
        }
    }
}
