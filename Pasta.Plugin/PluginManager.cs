using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pasta.Plugin
{
    /// <summary>
    /// Manages the list of plugins by running them in a separate AppDomains.
    /// </summary>
    public class PluginManager : IDisposable
    {
        /// <summary>
        /// Manager for AppDomain creation.
        /// </summary>
        private AppDomainManager domainManager = new AppDomainManager();

        /// <summary>
        /// Id of the next AppDomain.
        /// Used in names.
        /// </summary>
        private int nextDomainId = 1;

        /// <summary>
        /// The reference to all created AppDomains.
        /// </summary>
        private Dictionary<string, AppDomain> appDomains = new Dictionary<string, AppDomain>();

        /// <summary>
        /// The reference to all create PluginHosts.
        /// </summary>
        private Dictionary<string, PluginHost> pluginHosts = new Dictionary<string, PluginHost>();


        /// <summary>
        /// The list of all plugins
        /// </summary>
        public IEnumerable<PluginHost> Plugins => pluginHosts.Values;

        /// <summary>
        /// Clears the resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var domainInfo in appDomains)
            {
                var domain = domainInfo.Value;
                UnloadDomain(domain);
            }

            appDomains.Clear();
        }
        
        /// <summary>
        /// Get all plugin types of the specified interface from all loaded plugins
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        public IReadOnlyCollection<string> GetPluginTypes<TInterface>() where TInterface: class
        {
            return pluginHosts.Values
                .SelectMany(host => host.GetPluginTypes<TInterface>())
                .ToArray();
        }

        /// <summary>
        /// Loads all plugins from the specified directory.
        /// </summary>
        /// <param name="pluginsPath">The path to plugins directory</param>
        /// <param name="pluginInterfaces">The list of interfaces that are considered to be plugins.</param>
        public void LoadFrom(string pluginsPath, Type[] pluginInterfaces)
        {
            if (!Path.IsPathRooted(pluginsPath))
            {
                pluginsPath = Path.Combine(Directory.GetCurrentDirectory(), pluginsPath);
            }

            string[] pluginFilesPaths;
            try
            {
                pluginFilesPaths = Directory.GetFiles(pluginsPath, "*.dll");
            }
            catch (DirectoryNotFoundException)
            {
                // No directory - no plugins
                return;
            }

            foreach (var pluginFilePath in pluginFilesPaths)
            {
                var host = CreateHost(pluginFilePath);
                pluginHosts.Add(pluginFilePath, host);
                if (!host.HasPluginInterfaces(pluginInterfaces))
                {
                    UnloadPlugin(host);
                }
            }
        }

        /// <summary>
        /// Unloads a plugin from the application.
        /// </summary>
        /// <param name="plugin">Plugin to unload.</param>
        public void UnloadPlugin(PluginHost plugin)
        {
            // TODO: maintain plugin info objects to avoid that strange lookups.
            var pluginInfo = pluginHosts.First(pair => pair.Value == plugin);
            var domain = appDomains[pluginInfo.Key];

            pluginHosts.Remove(pluginInfo.Key);
            appDomains.Remove(pluginInfo.Key);
            UnloadDomain(domain);
        }

        /// <summary>
        /// Unloads the given domain.
        /// </summary>
        /// <param name="domain">Domain to unload.</param>
        private static void UnloadDomain(AppDomain domain)
        {
            try
            {
                if (!domain.IsFinalizingForUnload())
                    AppDomain.Unload(domain);
            }
            catch
            {
                // TODO: catch unload domain errors correctly
            }
        }

        /// <summary>
        /// Creates plugin host and registers the plugin domain.
        /// </summary>
        /// <param name="pluginFilePath"></param>
        /// <returns>The created plugin host.</returns>
        private PluginHost CreateHost(string pluginFilePath)
        {
            var setup = new AppDomainSetup
            {
                ShadowCopyFiles = "true",
                LoaderOptimization = LoaderOptimization.MultiDomain
            };
            var configFileName = string.Format("{0}.config", pluginFilePath);
            if (File.Exists(configFileName))
            {
                setup.ConfigurationFile = configFileName;
            }

            var domain = domainManager.CreateDomain(GetNextDomainName(pluginFilePath), null, setup);
            RegisterDomain(pluginFilePath, domain);

            var pluginHost = domain.CreateInstanceAndUnwrap(typeof(PluginHost).Assembly.FullName, typeof(PluginHost).FullName) as PluginHost;

            pluginHost.LoadPluginAssembly(pluginFilePath);

            return pluginHost;
        }

        /// <summary>
        /// Generates unique friendly name for the new AppDomain.
        /// </summary>
        /// <param name="pluginFilePath">The plugin file path</param>
        /// <returns>The generated friendly AppDomain name.</returns>
        private string GetNextDomainName(string pluginFilePath)
        {
            return $"Plugin-AppDomain-{nextDomainId++}-{Path.GetFileNameWithoutExtension(pluginFilePath)}";
        }

        /// <summary>
        /// Adds domain to the list for tracking.
        /// </summary>
        /// <param name="pluginFilePath">FilePath to domain.</param>
        /// <param name="domain">Domain.</param>
        private void RegisterDomain(string pluginFilePath, AppDomain domain)
        {
            appDomains.Add(pluginFilePath, domain);
        }
    }
}
