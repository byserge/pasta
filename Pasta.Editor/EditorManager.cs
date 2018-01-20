using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Pasta.Core;
using Pasta.Editor.Effects;
using Pasta.Editor.ExportActions;
using Pasta.Plugin;

namespace Pasta.Editor
{
	/// <summary>
	/// Manages editor forms.
	/// </summary>
	public class EditorManager : IEditorManager
	{
		private readonly EffectsManager effectsManager = new EffectsManager();
		private readonly ExportManager exportManager = new ExportManager();
		private object lockObj = new object();

		/// <summary>
		/// Current editor form.
		/// </summary>
		private EditorForm editorForm = null;

		public void TakeScreenshot()
		{

			if (editorForm != null)
				return;

			lock (lockObj)
			{
				if (editorForm != null)
					return;

				editorForm = new EditorForm(this.effectsManager, this.exportManager);
				editorForm.PrintScreen();
			}

			editorForm.Closed += (sender, args) =>
			{
				if (editorForm != null)
				{
					var form = editorForm;
					editorForm = null;
					form.Dispose();
				}
			};

			editorForm.Show();
		}

		private static readonly Type[] AdditionalInterfaces = new[]
		{
			typeof(IEffect),
			typeof(IMouseAware),
			typeof(IEditableEffect)
		};

		public void RegisterPlugin(int id, object plugin)
		{
			if (!(plugin is PluginHost typedPlugin))
				return;

			var selectionTypes = typedPlugin.GetPluginTypes<ISelectionEffect>();
			var screenshotTypes = typedPlugin.GetPluginTypes<IScreenshotEffect>();

			var effectTypes = typedPlugin.GetPluginTypes<IEffect>();
			var effectsInfo = effectTypes
				.Except(selectionTypes)
				.Except(screenshotTypes)
				.Select(type => BuildEffectInfo(type, typedPlugin));

			var selectionConstructors = selectionTypes
				.Select(type => new Func<ISelectionEffect>(() => typedPlugin.CreatePlugin<ISelectionEffect>(type, AdditionalInterfaces)));
			var screenshotConstructors = screenshotTypes
				.Select(type => new Func<IScreenshotEffect>(() => typedPlugin.CreatePlugin<IScreenshotEffect>(type, AdditionalInterfaces)));
			effectsManager.Register(effectsInfo);
			effectsManager.Register(selectionConstructors);
			effectsManager.Register(screenshotConstructors);

			var actionTypes = typedPlugin.GetPluginTypes<IExportAction>();
			var actionsInfo = actionTypes.Select(actionType => BuildExportActionInfo(actionType, typedPlugin));
			exportManager.Register(actionsInfo);
		}

		public void UnregisterPlugin(int id)
		{
			throw new NotImplementedException();
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
				new Func<IEffect>(() => plugin.CreatePlugin<IEffect>(type, AdditionalInterfaces)));
		}

		private static ExportActionInfo BuildExportActionInfo(string type, PluginHost plugin)
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

			var keys = name.Contains("Clipboard") ? (Keys.C | Keys.Control) : Keys.None;

			var image = stream == null ? null : Image.FromStream(stream);
			return new ExportActionInfo(
				name,
				image,
				new Func<IExportAction>(() => plugin.CreatePlugin<IExportAction>(type)),
				keys);
		}
	}
}
