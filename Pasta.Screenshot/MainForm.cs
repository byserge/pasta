using Pasta.Core;
using Pasta.Plugin;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Pasta.Screenshot
{
	public partial class MainForm : Form
	{
		private KeyboardHook keyboardHook;

		private IEditorManager editorManager;

		private PluginManager pluginManager;

		private bool isClosing = false;

		private AboutForm aboutForm;

		public MainForm()
		{
			InitializeComponent();

			InitializePlugins();

			InitializeEditorManager();

			RegisterEditorPlugins();

			InitializeKeyboardHook();

			this.WindowState = FormWindowState.Minimized;
		}

		private void InitializeEditorManager()
		{
			var editorManagerPlugin = pluginManager.Plugins.First(plugin => plugin.HasPluginInterfaces(new[] { typeof(IEditorManager) }));
			var pluginType = pluginManager.GetPluginTypes<IEditorManager>().First();
			editorManager = editorManagerPlugin.CreatePlugin<IEditorManager>(pluginType);
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
				typeof(IExportAction),
				typeof(IEditorManager)
			};

			pluginManager.LoadFrom("Plugins", pluginInterfaces);
		}

		private void RegisterEditorPlugins()
		{
			int id = 1;
			foreach (var plugin in pluginManager.Plugins)
			{
				editorManager.RegisterPlugin(id++, plugin);
			}
		}

		private void KeyboardHook_OnKeyHook(object sender, KeyEventArgs e)
		{
			editorManager.TakeScreenshot();
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
