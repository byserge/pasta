namespace Pasta.Core
{
	/// <summary>
	/// Describes manager that creates editor form, takes screenshots and allows working with them.
	/// </summary>
	public interface IEditorManager
	{
		/// <summary>
		/// Takes screenshot.
		/// </summary>
		void TakeScreenshot();

		/// <summary>
		/// Registers a plugin.
		/// </summary>
		/// <param name="id">The plugin id.</param>
		/// <param name="plugin">The plugin to register.</param>
		void RegisterPlugin(int id, object plugin);

		/// <summary>
		/// Unregisters the plugin by id.
		/// </summary>
		/// <param name="id">The id of the plugin to unregister.</param>
		void UnregisterPlugin(int id);
	}
}
